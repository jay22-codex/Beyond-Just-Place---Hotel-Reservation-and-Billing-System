using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_ReservationUI : Form
    {
        private ReservationService reservationService;
        private ReservationDetailsService details;

        private Guest selectedGuest;
        private readonly List<AvailableRoomInfo> selectedRooms =
            new List<AvailableRoomInfo>();

        private Button btnNewGuest;

        public FrontDesk_ReservationUI()
        {
            InitializeComponent();

            Load += Reservation_Load;

            btnSaveGuest.Click += SaveGuest_Click;
            btnSearchGuest.Click += SearchGuest_Click;
            btnSelectGuest.Click += SelectGuest_Click;
            btnEditGuest.Click += EditGuest_Click;
            listBoxGuest.DoubleClick += SelectGuest_Click;

            btnCheckAvailability.Click += CheckAvailability_Click;
            btnSelectRoom.Click += SelectRoom_Click;
            btnSelectPreferredRoom.Click += PreferredRoom_Click;
            btnCreateReservation.Click += CreateReservation_Click;
            btnCancelReservation.Click += CancelReservation_Click;

            cmbPreferredRoomType.SelectedIndexChanged += PreferredRoomTypeChanged;
            cmbRoomType.SelectedIndexChanged += RoomTypeChanged;
            dtpCheckIn.ValueChanged += DatesChanged;
            dtpCheckOut.ValueChanged += DatesChanged;
            nudRoomCount.SelectedIndexChanged += RoomCountChanged;
            txtGuestName.TextChanged += (s, e) => UpdateSummary();

            dgvAvailableRooms.AllowUserToAddRows = false;
            dgvAvailableRooms.AllowUserToDeleteRows = false;
            dgvAvailableRooms.MultiSelect = true;
            dgvAvailableRooms.ReadOnly = true;
            dgvAvailableRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvailableRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAvailableRooms.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgvAvailableRooms.ClearSelection();
                    dgvAvailableRooms.Rows[e.RowIndex].Selected = true;
                    SelectRoom();
                }
            };
        }

        private void Reservation_Load(object sender, EventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            try
            {
                reservationService = new ReservationService();
                details = new ReservationDetailsService();

                dtpCheckIn.Value = DateTime.Today;
                dtpCheckOut.Value = DateTime.Today.AddDays(1);

                FillCount(nudGuestCount, 1, 50, 1);
                FillCount(nudRoomCount, 1, 10, 1);
                FillCount(nudAdults, 1, 50, 1);
                FillCount(nudChildren, 0, 50, 0);

                if (cmbIdType.Items.Count == 0)
                {
                    cmbIdType.Items.AddRange(new object[]
                    {
                        "National ID", "Passport", "Driver's License",
                        "Student ID", "Other"
                    });
                }

                cmbReservationFeeMethod.Items.Clear();
                cmbReservationFeeMethod.Items.AddRange(
                    new object[] { "Cash", "GCash", "Card" });
                cmbReservationFeeMethod.DropDownStyle = ComboBoxStyle.DropDownList;

                List<string> roomTypes = reservationService.GetRoomTypes();
                cmbRoomType.Items.Clear();
                cmbPreferredRoomType.Items.Clear();

                foreach (string roomType in roomTypes)
                {
                    cmbRoomType.Items.Add(roomType);
                    cmbPreferredRoomType.Items.Add(roomType);
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.BorderStyle = BorderStyle.FixedSingle;
                pictureBox1.BackColor = Color.White;
                UpdateRoomPreview(null);

                btnSelectRoom.Text = "Add Room(s)";
                btnSelectPreferredRoom.Text = "View Rooms";

                // Reuse the existing hidden Designer button as Clear Selected.
                button1.Visible = true;
                button1.Text = "Clear Selected";
                button1.Click += (s, args) => ClearSelectedRooms(true);

                btnNewGuest = new Button();
                btnNewGuest.Name = "btnNewGuest";
                btnNewGuest.Text = "New Guest";
                btnNewGuest.Location = new Point(505, 196);
                btnNewGuest.Size = new Size(145, 33);
                btnNewGuest.Font = new Font("Arial", 12F, FontStyle.Bold);
                btnNewGuest.BackColor = Color.FromArgb(224, 224, 224);
                btnNewGuest.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnNewGuest.UseVisualStyleBackColor = false;
                btnNewGuest.Click += (s, args) => NewGuest();
                groupBox3.Controls.Add(btnNewGuest);

                LoadGuests();
                UpdateSelectionInstruction();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static void FillCount(ComboBox combo, int min, int max, int initial)
        {
            combo.Items.Clear();
            for (int i = min; i <= max; i++)
                combo.Items.Add(i.ToString());

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.SelectedItem = initial.ToString();
        }

        private static int ReadCount(ComboBox combo, string caption)
        {
            int value;
            if (!int.TryParse(combo.Text, out value))
                throw new InvalidOperationException("Please select " + caption + ".");
            return value;
        }

        private void LoadGuests()
        {
            int guestId = selectedGuest == null ? 0 : selectedGuest.GuestId;
            listBoxGuest.BeginUpdate();
            try
            {
                listBoxGuest.Items.Clear();
                foreach (Guest guest in reservationService.SearchGuest(
                    txtSearchGuest.Text.Trim()))
                {
                    listBoxGuest.Items.Add(guest);
                    if (guest.GuestId == guestId)
                        listBoxGuest.SelectedItem = guest;
                }
            }
            finally
            {
                listBoxGuest.EndUpdate();
            }
        }

        private void SearchGuest_Click(object sender, EventArgs e)
        {
            try
            {
                LoadGuests();
                if (listBoxGuest.Items.Count == 0)
                    MessageBox.Show("No guest found.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SelectGuest_Click(object sender, EventArgs e)
        {
            LoadSelectedGuest(false);
        }

        private void EditGuest_Click(object sender, EventArgs e)
        {
            LoadSelectedGuest(true);
        }

        private void LoadSelectedGuest(bool editing)
        {
            Guest item = listBoxGuest.SelectedItem as Guest;
            if (item == null)
            {
                MessageBox.Show("Select a guest record first.");
                return;
            }

            try
            {
                Guest guest = details.GetGuest(item.GuestId);
                if (guest == null)
                {
                    MessageBox.Show("Guest not found. Search again.");
                    return;
                }

                selectedGuest = guest;
                txtGuestName.Text = guest.GuestName ?? "";
                txtContactNumber.Text = guest.ContactNumber ?? "";
                txtEmail.Text = guest.Email ?? "";
                txtAddress.Text = guest.HomeAddress ?? "";
                cmbIdType.Text = guest.IdType ?? "";
                btnSaveGuest.Text = "Update Guest Record";
                UpdateSummary();

                if (editing)
                {
                    txtGuestName.Focus();
                    MessageBox.Show(
                        "Edit the guest details, then click Update Guest Record.");
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void NewGuest()
        {
            selectedGuest = null;
            listBoxGuest.ClearSelected();
            txtGuestName.Clear();
            txtContactNumber.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            cmbIdType.SelectedIndex = -1;
            cmbIdType.Text = "";
            btnSaveGuest.Text = "+ Save Guest Record";
            UpdateSummary();
            txtGuestName.Focus();
        }

        private void SaveGuest_Click(object sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text.Trim();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    MailAddress parsed;
                    try { parsed = new MailAddress(email); }
                    catch (FormatException)
                    {
                        throw new InvalidOperationException(
                            "Enter a valid email address.");
                    }

                    if (!string.Equals(parsed.Address, email,
                        StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException(
                            "Enter a valid email address.");
                }

                Guest guest = new Guest
                {
                    GuestId = selectedGuest == null ? 0 : selectedGuest.GuestId,
                    GuestName = txtGuestName.Text.Trim(),
                    ContactNumber = txtContactNumber.Text.Trim(),
                    Email = email,
                    HomeAddress = txtAddress.Text.Trim(),
                    IdType = cmbIdType.Text.Trim()
                };

                bool updating = guest.GuestId > 0;
                selectedGuest = details.SaveGuest(guest);

                txtSearchGuest.Clear();
                LoadGuests();
                btnSaveGuest.Text = "Update Guest Record";
                UpdateSummary();

                MessageBox.Show(updating
                    ? "Guest record updated successfully."
                    : "Guest record saved successfully.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private bool GuestHasUnsavedChanges()
        {
            return selectedGuest != null &&
                (txtGuestName.Text.Trim() != (selectedGuest.GuestName ?? "") ||
                 txtContactNumber.Text.Trim() != (selectedGuest.ContactNumber ?? "") ||
                 txtEmail.Text.Trim() != (selectedGuest.Email ?? "") ||
                 txtAddress.Text.Trim() != (selectedGuest.HomeAddress ?? "") ||
                 cmbIdType.Text.Trim() != (selectedGuest.IdType ?? ""));
        }

        private void PreferredRoomTypeChanged(object sender, EventArgs e)
        {
            if (cmbPreferredRoomType.SelectedIndex >= 0)
            {
                cmbRoomType.SelectedItem = cmbPreferredRoomType.SelectedItem;
                UpdateRoomPreview(cmbPreferredRoomType.Text);
            }
            else
            {
                UpdateRoomPreview(null);
            }
        }

        private void RoomTypeChanged(object sender, EventArgs e)
        {
            dgvAvailableRooms.Rows.Clear();

            if (cmbRoomType.SelectedIndex >= 0)
            {
                if (cmbPreferredRoomType.Items.Contains(cmbRoomType.Text))
                    cmbPreferredRoomType.SelectedItem = cmbRoomType.Text;

                UpdateRoomPreview(cmbRoomType.Text);
            }

            UpdateSelectionInstruction();
        }

        private void DatesChanged(object sender, EventArgs e)
        {
            ClearSelectedRooms(false);
            dgvAvailableRooms.Rows.Clear();
            UpdateSummary();
        }

        private void RoomCountChanged(object sender, EventArgs e)
        {
            int desired;
            if (int.TryParse(nudRoomCount.Text, out desired) &&
                selectedRooms.Count > desired)
            {
                ClearSelectedRooms(false);
            }

            UpdateSelectionInstruction();
            UpdateSummary();
        }

        private void PreferredRoom_Click(object sender, EventArgs e)
        {
            if (cmbPreferredRoomType.SelectedIndex < 0)
            {
                MessageBox.Show("Select a preferred room type first.");
                return;
            }

            cmbRoomType.SelectedItem = cmbPreferredRoomType.SelectedItem;
            CheckAvailability_Click(sender, e);
        }

        private void CheckAvailability_Click(object sender, EventArgs e)
        {
            try
            {
                LoadAvailableRooms();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void LoadAvailableRooms()
        {
            if (cmbRoomType.SelectedIndex < 0)
                throw new InvalidOperationException("Please select a room type.");

            if (dtpCheckOut.Value.Date <= dtpCheckIn.Value.Date)
                throw new InvalidOperationException(
                    "Check-out date must be after check-in date.");

            List<AvailableRoomInfo> rooms = details.GetAvailableRooms(
                dtpCheckIn.Value.Date,
                dtpCheckOut.Value.Date,
                cmbRoomType.Text);

            dgvAvailableRooms.Rows.Clear();

            foreach (AvailableRoomInfo room in rooms)
            {
                bool alreadySelected = selectedRooms.Any(
                    selected => selected.RoomId == room.RoomId);

                int rowIndex = dgvAvailableRooms.Rows.Add(
                    room.RoomNumber,
                    room.RoomType,
                    room.RoomCapacity.HasValue
                        ? room.RoomCapacity.Value.ToString() : "Not set",
                    room.ReservationFee.HasValue
                        ? "PHP " + room.ReservationFee.Value.ToString("N2") : "Not set",
                    room.RoomRate.ToString("N2"),
                    alreadySelected ? "Selected" : "Available");

                dgvAvailableRooms.Rows[rowIndex].Tag = room;

                if (alreadySelected)
                    dgvAvailableRooms.Rows[rowIndex].DefaultCellStyle.BackColor =
                        Color.FromArgb(224, 224, 224);
            }

            dgvAvailableRooms.ClearSelection();
            UpdateSelectionInstruction();
            UpdateSummary();

            int desiredRooms = ReadCount(nudRoomCount, "Rooms");
            int remaining = Math.Max(0, desiredRooms - selectedRooms.Count);

            if (rooms.Count == 0)
            {
                MessageBox.Show("No available rooms for the selected dates.");
            }
            else if (remaining > 0 && rooms.Count < remaining)
            {
                MessageBox.Show(
                    "Only " + rooms.Count + " " + cmbRoomType.Text +
                    " room(s) are currently available.\n\n" +
                    "You may select these, then choose another room type " +
                    "to complete the reservation.");
            }
        }

        private void SelectRoom_Click(object sender, EventArgs e)
        {
            SelectRoom();
        }

        private void SelectRoom()
        {
            if (dgvAvailableRooms.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Select one or more available rooms from the table first.\n" +
                    "Tip: hold Ctrl to select several rows.");
                return;
            }

            int desired = ReadCount(nudRoomCount, "Rooms");
            int remaining = desired - selectedRooms.Count;

            if (remaining <= 0)
            {
                MessageBox.Show(
                    "You already selected the requested " + desired + " room(s).\n" +
                    "Use Clear Selected if you want to choose different rooms.");
                return;
            }

            List<AvailableRoomInfo> additions = new List<AvailableRoomInfo>();

            foreach (DataGridViewRow row in dgvAvailableRooms.SelectedRows)
            {
                AvailableRoomInfo room = row.Tag as AvailableRoomInfo;
                if (room == null) continue;

                if (selectedRooms.Any(r => r.RoomId == room.RoomId))
                    continue;

                additions.Add(room);
            }

            if (additions.Count == 0)
            {
                MessageBox.Show("The selected room(s) are already added.");
                return;
            }

            if (additions.Count > remaining)
            {
                MessageBox.Show(
                    "You only need " + remaining + " more room(s). " +
                    "Select only that many rows.");
                return;
            }

            selectedRooms.AddRange(additions);

            foreach (DataGridViewRow row in dgvAvailableRooms.Rows)
            {
                AvailableRoomInfo room = row.Tag as AvailableRoomInfo;
                if (room != null && selectedRooms.Any(r => r.RoomId == room.RoomId))
                {
                    row.Cells[colRoomStatus.Index].Value = "Selected";
                    row.DefaultCellStyle.BackColor = Color.FromArgb(224, 224, 224);
                }
            }

            dgvAvailableRooms.ClearSelection();
            UpdateSelectionInstruction();
            UpdateSummary();

            if (selectedRooms.Count == desired)
            {
                MessageBox.Show(
                    "All " + desired + " room(s) selected:\n" +
                    string.Join(", ", selectedRooms.Select(r => r.RoomNumber).ToArray()));
            }
            else
            {
                MessageBox.Show(
                    "Selected " + selectedRooms.Count + " of " + desired + " room(s).\n" +
                    "You can select more rooms or switch room type and add another room.");
            }
        }

        private void ClearSelectedRooms(bool showMessage)
        {
            selectedRooms.Clear();

            foreach (DataGridViewRow row in dgvAvailableRooms.Rows)
            {
                row.DefaultCellStyle.BackColor = dgvAvailableRooms.DefaultCellStyle.BackColor;
                row.Cells[colRoomStatus.Index].Value = "Available";
            }

            dgvAvailableRooms.ClearSelection();
            UpdateSelectionInstruction();
            UpdateSummary();

            if (showMessage)
                MessageBox.Show("Selected rooms cleared.");
        }

        private void UpdateSelectionInstruction()
        {
            int desired = 1;
            int.TryParse(nudRoomCount.Text, out desired);
            if (desired < 1) desired = 1;

            label11.Text =
                "Available Room(s): Select " + desired +
                " total — currently " + selectedRooms.Count + " selected";
        }

        private void UpdateSummary()
        {
            lblSummaryGuest.Text = string.IsNullOrWhiteSpace(txtGuestName.Text)
                ? "Output" : txtGuestName.Text.Trim();

            int nights = (dtpCheckOut.Value.Date - dtpCheckIn.Value.Date).Days;
            lblSummaryStay.Text = nights > 0 ? nights + " night(s)" : "Output";

            if (selectedRooms.Count == 0)
            {
                lblSummaryRoom.Text = "Output";
                lblSummaryRate.Text = "Output";
                lblSummaryReservationFee.Text = "Output";
                lblReservationFeePreview.Text = "Select room(s)";
                return;
            }

            lblSummaryRoom.Text = string.Join(", ",
                selectedRooms.Select(r => r.RoomNumber).ToArray());

            decimal totalNightlyRate = selectedRooms.Sum(r => r.RoomRate);
            lblSummaryRate.Text =
                "PHP " + totalNightlyRate.ToString("N2") + "/night total";

            decimal totalFee = selectedRooms.Sum(
                r => r.ReservationFee.GetValueOrDefault());

            string fee = "PHP " + totalFee.ToString("N2") +
                         " (" + selectedRooms.Count + " room(s))";

            lblSummaryReservationFee.Text = fee;
            lblReservationFeePreview.Text = fee;
        }

        private void CreateReservation_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedGuest == null)
                    throw new InvalidOperationException(
                        "Save or select a guest first.");

                if (GuestHasUnsavedChanges())
                    throw new InvalidOperationException(
                        "Guest details changed. Click Update Guest Record first.");

                int totalGuests = ReadCount(nudGuestCount, "Number of Guests");
                int adults = ReadCount(nudAdults, "Adults");
                int children = ReadCount(nudChildren, "Children");
                int roomCount = ReadCount(nudRoomCount, "Rooms");

                if (totalGuests != adults + children)
                    throw new InvalidOperationException(
                        "Number of Guests must equal Adults + Children.");

                if (selectedRooms.Count != roomCount)
                    throw new InvalidOperationException(
                        "You requested " + roomCount + " room(s), but selected " +
                        selectedRooms.Count + ". Select exactly " + roomCount + " room(s).");

                bool allCapacitiesKnown = selectedRooms.All(
                    r => r.RoomCapacity.HasValue);

                if (allCapacitiesKnown)
                {
                    int totalCapacity = selectedRooms.Sum(
                        r => r.RoomCapacity.GetValueOrDefault());

                    if (totalGuests > totalCapacity)
                        throw new InvalidOperationException(
                            "The selected rooms can hold only " + totalCapacity +
                            " guest(s). Add more rooms or choose larger rooms.");
                }

                decimal totalReservationFee = selectedRooms.Sum(
                    r => r.ReservationFee.GetValueOrDefault());

                string paymentMethod = cmbReservationFeeMethod.Text;
                if (totalReservationFee > 0m &&
                    string.IsNullOrWhiteSpace(paymentMethod))
                    throw new InvalidOperationException(
                        "Choose a reservation fee payment method.");

                int reservationId = details.CreateReservation(
                    selectedGuest.GuestId,
                    selectedRooms,
                    dtpCheckIn.Value.Date,
                    dtpCheckOut.Value.Date,
                    totalGuests,
                    adults,
                    children,
                    roomCount,
                    txtSpecialRequest.Text.Trim(),
                    paymentMethod);

                MessageBox.Show(
                    "Reservation #" + reservationId +
                    " saved successfully as Pending.\n\n" +
                    "Rooms: " +
                    string.Join(", ", selectedRooms.Select(r => r.RoomNumber).ToArray()) +
                    "\nTotal rooms: " + roomCount +
                    "\nTotal nightly room rate: PHP " +
                    selectedRooms.Sum(r => r.RoomRate).ToString("N2") +
                    "\nReservation fee: PHP " + totalReservationFee.ToString("N2") +
                    "\n\nPayment method recorded; no actual reservation-fee payment was processed.");

                ClearSelectedRooms(false);
                dgvAvailableRooms.Rows.Clear();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void CancelReservation_Click(object sender, EventArgs e)
        {
            if (selectedGuest == null)
            {
                MessageBox.Show("Select a guest first.");
                return;
            }

            DialogResult answer = MessageBox.Show(
                "Cancel the latest Pending or Confirmed reservation for " +
                selectedGuest.GuestName + "?",
                "Cancel Reservation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            try
            {
                bool cancelled = reservationService.CancelReservation(selectedGuest);
                MessageBox.Show(cancelled
                    ? "Reservation cancelled successfully."
                    : "No active reservation found.");

                if (cancelled)
                    ClearSelectedRooms(false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        // Uses the four actual room photos embedded in the UI assembly.
        // The Reservation Designer/layout is intentionally left unchanged.
        private void UpdateRoomPreview(string roomType)
        {
            string resourceName = GetRoomPreviewResourceName(roomType);

            Image nextImage = LoadEmbeddedRoomImage(resourceName);

            Image oldImage = pictureBox1.Image;
            pictureBox1.Image = nextImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            if (oldImage != null)
                oldImage.Dispose();
        }

        private static string GetRoomPreviewResourceName(string roomType)
        {
            string type = (roomType ?? "").Trim().ToLowerInvariant();

            if (type.Contains("suite"))
                return "UI.RoomImages.SuiteRoom.png";

            if (type.Contains("family"))
                return "UI.RoomImages.FamilyRoom.png";

            if (type.Contains("deluxe"))
                return "UI.RoomImages.DeluxeRoom.png";

            // Show the Standard room as the default preview when no type
            // has been selected yet.
            return "UI.RoomImages.StandardRoom.png";
        }

        private static Image LoadEmbeddedRoomImage(string resourceName)
        {
            using (Stream stream = typeof(FrontDesk_ReservationUI)
                .Assembly
                .GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return null;

                // Clone the image before closing the resource stream.
                using (Image source = Image.FromStream(stream))
                    return new Bitmap(source);
            }
        }

        private static void ShowError(Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Reservation Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
