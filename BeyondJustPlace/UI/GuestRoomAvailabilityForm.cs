using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    public partial class GuestRoomAvailabilityForm : Form
    {
        private readonly ReservationDetailsService reservations = new ReservationDetailsService();
        private readonly HotelEnhancementService hotel = new HotelEnhancementService();
        private readonly Dictionary<string, List<AvailableRoomInfo>> available =
            new Dictionary<string, List<AvailableRoomInfo>>(StringComparer.OrdinalIgnoreCase);
        private readonly List<AvailableRoomInfo> chosen = new List<AvailableRoomInfo>();
        private readonly List<Panel> cards = new List<Panel>();
        private bool searchValid;
        private bool loaded;
        private bool searching;
        private string lastCapacityWarning = "";
        private readonly Timer availabilityDebounce = new Timer { Interval = 350 };

        public GuestRoomAvailabilityForm()
        {
            InitializeComponent();
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            dtpArrival.Value = DateTime.Today;
            dtpDeparture.Value = DateTime.Today.AddDays(1);
            btnClose.Click += (s, e) => Close();
            btnSearch.Click += (s, e) => CheckAvailability(true);
            availabilityDebounce.Tick += (s, e) =>
            {
                availabilityDebounce.Stop();
                if (!IsDisposed && loaded) CheckAvailability(true);
            };
            FormClosed += (s, e) => availabilityDebounce.Dispose();
            btnContinue.Click += Continue_Click;
            dtpArrival.ValueChanged += InputsChanged;
            dtpDeparture.ValueChanged += InputsChanged;
            numAdults.ValueChanged += InputsChanged;
            numChildren.ValueChanged += InputsChanged;
            numRooms.ValueChanged += InputsChanged;
            availableList.Resize += (s, e) => ResizeCards();
            selectedList.Resize += (s, e) => ResizeSelectedRows();
            mainSplit.SizeChanged += (s, e) => AdjustSplit();
            header.Resize += (s, e) => btnClose.Left = header.ClientSize.Width - btnClose.Width - 35;
            Shown += (s, e) => { if (!loaded) { loaded = true; AdjustSplit(); CheckAvailability(false); } };
            UiPolish.Apply(this);
            UpdateSelected();
        }

        private void AdjustSplit()
        {
            if (mainSplit.Width < 940) return;
            int desired = mainSplit.Width - Math.Max(360, (int)(mainSplit.Width * 0.30));
            desired = Math.Max(570, Math.Min(mainSplit.Width - 340, desired));
            if (desired != mainSplit.SplitterDistance)
            {
                try { mainSplit.SplitterDistance = desired; }
                catch (InvalidOperationException) { }
            }
            ResizeCards();
        }

        private void InputsChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            searchValid = false;
            chosen.Clear();
            available.Clear();
            availableList.Controls.Clear();
            cards.Clear();
            lblResults.Text = "Checking rooms for " +
                ((int)numAdults.Value + (int)numChildren.Value) + " guest(s)...";
            UpdateSelected();
            availabilityDebounce.Stop();
            availabilityDebounce.Start();
        }

        private void CheckAvailability(bool showWarning)
        {
            if (searching || IsDisposed) return;
            if (dtpArrival.Value.Date < DateTime.Today ||
                dtpDeparture.Value.Date <= dtpArrival.Value.Date)
            {
                searchValid = false;
                availableList.Controls.Clear();
                lblResults.Text = "Choose valid dates: check-out must be after check-in.";
                return;
            }
            searching = true;
            try
            {
                btnSearch.Enabled = false;
                Cursor = Cursors.WaitCursor;
                chosen.Clear();
                available.Clear();
                cards.Clear();
                availableList.Controls.Clear();

                int guests = (int)numAdults.Value + (int)numChildren.Value;
                int requestedRooms = (int)numRooms.Value;
                DataTable types = hotel.GetRoomTypes(false);
                Dictionary<string, DataRow> definitions = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
                List<AvailableRoomInfo> allRooms = new List<AvailableRoomInfo>();
                foreach (DataRow row in types.Rows)
                {
                    string type = Convert.ToString(row["TypeName"]);
                    List<AvailableRoomInfo> rooms = reservations.GetAvailableRooms(
                        dtpArrival.Value.Date, dtpDeparture.Value.Date, type)
                        .Where(r => r.RoomCapacity.GetValueOrDefault() > 0).ToList();
                    available[type] = rooms;
                    definitions[type] = row;
                    allRooms.AddRange(rooms);
                }

                // The largest N available rooms give an upper bound on capacity.
                // This supports bookings containing a mixture of room types.
                int maxCapacity = allRooms.OrderByDescending(r => r.RoomCapacity.GetValueOrDefault())
                    .Take(requestedRooms).Sum(r => r.RoomCapacity.GetValueOrDefault());
                bool enoughRooms = allRooms.Count >= requestedRooms;
                bool canFit = enoughRooms && maxCapacity >= guests;

                if (canFit)
                {
                    foreach (DataRow row in types.Rows)
                    {
                        string type = Convert.ToString(row["TypeName"]);
                        List<AvailableRoomInfo> candidates;
                        if (!available.TryGetValue(type, out candidates) || candidates.Count == 0)
                            continue;
                        // Show a room type only if at least one of its available rooms can
                        // be combined with the remaining requested rooms to fit the group.
                        bool fitsInCombination = candidates.Any(candidate =>
                            candidate.RoomCapacity.GetValueOrDefault() +
                            allRooms.Where(r => r.RoomId != candidate.RoomId)
                                .OrderByDescending(r => r.RoomCapacity.GetValueOrDefault())
                                .Take(requestedRooms - 1)
                                .Sum(r => r.RoomCapacity.GetValueOrDefault()) >= guests);
                        if (fitsInCombination)
                            AddRoomCard(type, row, candidates);
                    }
                }
                searchValid = canFit;
                lblResults.Text = "Rooms for " + guests + " guest(s)  •  " + requestedRooms +
                    " room(s)  •  " + (dtpDeparture.Value.Date - dtpArrival.Value.Date).Days +
                    " night(s)  •  rates based on your dates";
                if (types.Rows.Count == 0)
                    ShowEmpty("No active room types have been set up by Admin.");
                else if (!canFit)
                    ShowEmpty("No available room combination can fit " + guests +
                        " guest(s) in " + requestedRooms + " room(s) on those dates. " +
                        "Please increase Rooms wanted or choose different dates.");
                else if (cards.Count == 0)
                    ShowEmpty("No matching room types are available for these dates.");
                UpdateSelected();
                ResizeCards();
                if (!canFit && showWarning)
                {
                    string signature = dtpArrival.Value.Date.ToString("yyyyMMdd") + "-" +
                        dtpDeparture.Value.Date.ToString("yyyyMMdd") + ":" + guests + ":" + requestedRooms;
                    if (signature != lastCapacityWarning)
                    {
                        lastCapacityWarning = signature;
                        string explanation = !enoughRooms
                            ? "Only " + allRooms.Count + " room(s) are free for the selected dates."
                            : "The largest " + requestedRooms + " room(s) only fit " +
                                maxCapacity + " guest(s), but your group has " + guests + ".";
                        MessageBox.Show(this,
                            "No available rooms can accommodate your group.\r\n\r\n" +
                            explanation +
                            "\r\n\r\nIncrease Rooms wanted or change the dates.",
                            "Guest capacity warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
                else if (canFit)
                    lastCapacityWarning = "";
            }
            catch (Exception ex)
            {
                searchValid = false;
                ShowEmpty("Room availability could not be loaded. Check LocalDB and try again.");
                MessageBox.Show(this, ex.Message, "Room availability error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                searching = false;
                btnSearch.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void ShowEmpty(string text)
        {
            Label label = GuestBookingLook.Label(text, 11);
            label.Size = new Size(740, 90);
            label.ForeColor = GuestBookingLook.Muted;
            availableList.Controls.Add(label);
        }

        private void AddRoomCard(string type, DataRow row, List<AvailableRoomInfo> rooms)
        {
            Panel card = new Panel
            {
                Name = "roomCard_" + type.Replace(" ", ""),
                BackColor = Color.White,
                Size = new Size(830, 213),
                Margin = new Padding(0, 0, 10, 14),
                BorderStyle = BorderStyle.FixedSingle
            };
            PictureBox picture = new PictureBox
            {
                Name = "roomPhoto_" + type.Replace(" ", ""),
                Location = new Point(10, 10),
                Size = new Size(260, 188),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = GuestBookingLook.LightPurple,
                Image = GuestBookingLook.Photo(type)
            };
            Label title = GuestBookingLook.Label(type + " Room", 16, true);
            title.SetBounds(286, 13, 470, 35);
            title.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            int beds = Convert.ToInt32(row["BedCount"]);
            int capacity = Convert.ToInt32(row["Capacity"]);
            int sqm = Convert.ToInt32(row["SizeSqm"]);
            Label spec = GuestBookingLook.Label(
                beds + " bed(s)   •   Up to " + capacity + " guests   •   " + sqm + " m²", 10);
            spec.SetBounds(286, 52, 480, 25);
            spec.ForeColor = GuestBookingLook.Muted;
            string description = Convert.ToString(row["Description"]);
            Label desc = GuestBookingLook.Label(description, 9.5f);
            desc.SetBounds(286, 79, 480, 50);
            desc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            desc.ForeColor = GuestBookingLook.Muted;
            decimal rate = rooms.Count > 0 ? rooms[0].RoomRate : Convert.ToDecimal(row["Rate"]);
            Label price = GuestBookingLook.Label(GuestBookingLook.Money(rate) + " / night", 14, true);
            price.SetBounds(286, 143, 250, 30);
            Label status = GuestBookingLook.Label(
                rooms.Count + " available for your dates", 9, true);
            status.SetBounds(286, 176, 310, 22);
            status.ForeColor = rooms.Count > 0 ? Color.FromArgb(42, 119, 75) : Color.FromArgb(159, 56, 61);
            Button add = GuestBookingLook.Button(rooms.Count > 0 ? "+ Add room" : "Fully booked", true);
            add.Enabled = rooms.Count > 0 && capacity > 0;
            add.Name = "btnAdd" + type.Replace(" ", "");
            add.SetBounds(590, 145, 210, 48);
            add.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            add.Click += (s, e) => AddOne(type);
            card.Controls.AddRange(new Control[] { picture, title, spec, desc, price, status, add });
            cards.Add(card);
            availableList.Controls.Add(card);
        }

        private void ResizeCards()
        {
            if (availableList == null || availableList.IsDisposed) return;
            int width = Math.Max(700, availableList.ClientSize.Width - 24);
            foreach (Panel panel in cards) panel.Width = width;
        }

        private void AddOne(string type)
        {
            if (!searchValid) return;
            int target = (int)numRooms.Value;
            if (chosen.Count >= target)
            {
                MessageBox.Show("You requested " + target + " room(s). Remove a room or increase the Rooms wanted field.",
                    "Room limit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<AvailableRoomInfo> roomList;
            if (!available.TryGetValue(type, out roomList)) return;
            AvailableRoomInfo room = roomList.FirstOrDefault(r =>
                !chosen.Any(c => c.RoomId == r.RoomId) && r.RoomCapacity.GetValueOrDefault() > 0);
            if (room == null)
            {
                MessageBox.Show("No additional " + type + " rooms are available for these dates.",
                    "Rooms unavailable", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            chosen.Add(room);
            UpdateSelected();
            if (chosen.Count == target && chosen.Sum(r => r.RoomCapacity.GetValueOrDefault()) <
                (int)numAdults.Value + (int)numChildren.Value)
            {
                MessageBox.Show("The selected rooms cannot accommodate everyone. " +
                    "Choose larger rooms or increase Rooms wanted, then check availability again.",
                    "Room capacity warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateSelected()
        {
            selectedList.SuspendLayout();
            selectedList.Controls.Clear();
            foreach (AvailableRoomInfo item in chosen)
            {
                Panel row = new Panel
                {
                    BackColor = GuestBookingLook.LightPurple,
                    Size = new Size(355, 85),
                    Margin = new Padding(0, 0, 0, 10)
                };
                Label title = GuestBookingLook.Label(item.RoomType + "  •  Room " + item.RoomNumber, 10.5f, true);
                title.SetBounds(9, 8, 295, 28);
                Label price = GuestBookingLook.Label(
                    GuestBookingLook.Money(item.RoomRate) + " / night   •   " +
                    item.RoomCapacity.GetValueOrDefault() + " guests", 9);
                price.SetBounds(9, 42, 290, 24);
                Button remove = GuestBookingLook.Button("×");
                remove.Size = new Size(33, 33);
                remove.Location = new Point(305, 8);
                remove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                remove.Click += (s, e) => { chosen.Remove(item); UpdateSelected(); };
                row.Controls.AddRange(new Control[] { title, price, remove });
                selectedList.Controls.Add(row);
            }
            selectedList.ResumeLayout();
            ResizeSelectedRows();
            int request = (int)numRooms.Value;
            int guests = (int)numAdults.Value + (int)numChildren.Value;
            int capacity = chosen.Sum(r => r.RoomCapacity.GetValueOrDefault());
            lblSelectionStatus.Text = "Selected rooms (" + chosen.Count + " / " + request + ")";
            int nights = Math.Max(1, (dtpDeparture.Value.Date - dtpArrival.Value.Date).Days);
            decimal amount = chosen.Sum(r => r.RoomRate) * nights;
            lblTotal.Text = "Room total: " + GuestBookingLook.Money(amount);
            lblCapacity.Text = chosen.Count == 0 ? "Choose available rooms on the left." :
                "Capacity: " + capacity + " guests  •  Your group: " + guests +
                (capacity < guests ? "  (not enough)" : "  (fits)");
            lblCapacity.ForeColor = capacity < guests && chosen.Count > 0
                ? Color.Firebrick : GuestBookingLook.Muted;
            btnContinue.Enabled = searchValid && chosen.Count == request && capacity >= guests;
        }

        private void ResizeSelectedRows()
        {
            if (selectedList == null || selectedList.IsDisposed) return;
            foreach (Control c in selectedList.Controls)
                c.Width = Math.Max(315, selectedList.ClientSize.Width - 23);
        }

        private void Continue_Click(object sender, EventArgs e)
        {
            if (!btnContinue.Enabled) return;
            GuestBookingSelection selection = new GuestBookingSelection
            {
                CheckIn = dtpArrival.Value.Date,
                CheckOut = dtpDeparture.Value.Date,
                Adults = (int)numAdults.Value,
                Children = (int)numChildren.Value,
                Rooms = chosen.ToList()
            };
            using (GuestBookingDetailsForm booking = new GuestBookingDetailsForm(selection))
            {
                if (booking.ShowDialog(this) == DialogResult.OK)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}
