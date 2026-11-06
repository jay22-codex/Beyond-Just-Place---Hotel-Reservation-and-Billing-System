using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        private AvailableRoomInfo selectedRoom;
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
            cmbRoomType.SelectedIndexChanged += (s, e) => ClearRoom();
            dtpCheckIn.ValueChanged += (s, e) => ClearRoom();
            dtpCheckOut.ValueChanged += (s, e) => ClearRoom();
            txtGuestName.TextChanged += (s, e) => UpdateSummary();

            dgvAvailableRooms.AllowUserToAddRows = false;
            dgvAvailableRooms.AllowUserToDeleteRows = false;
            dgvAvailableRooms.MultiSelect = false;
            dgvAvailableRooms.ReadOnly = true;
            dgvAvailableRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvailableRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAvailableRooms.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SelectRoom();
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

                FillCount(nudGuestCount, 1, 20, 1);
                FillCount(nudRoomCount, 1, 10, 1);
                FillCount(nudAdults, 1, 20, 1);
                FillCount(nudChildren, 0, 20, 0);

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

                // Placed in Search Guest Records to avoid covering
                // the Adults/Children controls in Guest Information.
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
                    MessageBox.Show("Edit the guest details, then click Update Guest Record.");
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
                        throw new InvalidOperationException("Enter a valid email address.");
                    }

                    if (!string.Equals(parsed.Address, email,
                        StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Enter a valid email address.");
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
                cmbRoomType.SelectedItem = cmbPreferredRoomType.SelectedItem;
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
                dtpCheckIn.Value.Date, dtpCheckOut.Value.Date,
                cmbRoomType.Text);

            selectedRoom = null;
            dgvAvailableRooms.Rows.Clear();

            foreach (AvailableRoomInfo room in rooms)
            {
                int rowIndex = dgvAvailableRooms.Rows.Add(
                    room.RoomNumber,
                    room.RoomType,
                    room.RoomCapacity.HasValue
                        ? room.RoomCapacity.Value.ToString() : "Not set",
                    room.ReservationFee.HasValue
                        ? "PHP " + room.ReservationFee.Value.ToString("N2") : "Not set",
                    room.RoomRate.ToString("N2"),
                    "Available");

                dgvAvailableRooms.Rows[rowIndex].Tag = room;
            }

            dgvAvailableRooms.ClearSelection();
            UpdateSummary();

            if (rooms.Count == 0)
                MessageBox.Show("No available rooms for the selected dates.");
        }

        private void SelectRoom_Click(object sender, EventArgs e)
        {
            SelectRoom();
        }

        private void SelectRoom()
        {
            if (dgvAvailableRooms.SelectedRows.Count != 1 ||
                !(dgvAvailableRooms.SelectedRows[0].Tag is AvailableRoomInfo))
            {
                MessageBox.Show("Select an available room from the table first.");
                return;
            }

            selectedRoom = (AvailableRoomInfo)dgvAvailableRooms.SelectedRows[0].Tag;
            UpdateSummary();
        }

        private void ClearRoom()
        {
            selectedRoom = null;
            dgvAvailableRooms.Rows.Clear();
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            lblSummaryGuest.Text = string.IsNullOrWhiteSpace(txtGuestName.Text)
                ? "Output" : txtGuestName.Text.Trim();

            int nights = (dtpCheckOut.Value.Date - dtpCheckIn.Value.Date).Days;
            lblSummaryStay.Text = nights > 0 ? nights + " night(s)" : "Output";

            lblSummaryRoom.Text = selectedRoom == null
                ? "Output" : selectedRoom.RoomNumber;

            lblSummaryRate.Text = selectedRoom == null
                ? "Output"
                : "PHP " + selectedRoom.RoomRate.ToString("N2") + "/night";

            string fee = selectedRoom == null
                ? "Output"
                : selectedRoom.ReservationFee.HasValue
                    ? "PHP " + selectedRoom.ReservationFee.Value.ToString("N2")
                    : "Not configured";

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

                if (selectedRoom == null)
                    throw new InvalidOperationException(
                        "Select an available room first.");

                int totalGuests = ReadCount(nudGuestCount, "Number of Guests");
                int adults = ReadCount(nudAdults, "Adults");
                int children = ReadCount(nudChildren, "Children");
                int roomCount = ReadCount(nudRoomCount, "Rooms");

                if (totalGuests != adults + children)
                    throw new InvalidOperationException(
                        "Number of Guests must equal Adults + Children.");

                if (roomCount != 1)
                    throw new InvalidOperationException(
                        "Only one room per reservation is supported. Set Rooms to 1.");

                if (selectedRoom.RoomCapacity.HasValue &&
                    totalGuests > selectedRoom.RoomCapacity.Value)
                    throw new InvalidOperationException(
                        "Guest count exceeds the selected room capacity.");

                string paymentMethod = cmbReservationFeeMethod.Text;
                if (selectedRoom.ReservationFee.GetValueOrDefault() > 0 &&
                    string.IsNullOrWhiteSpace(paymentMethod))
                    throw new InvalidOperationException(
                        "Choose a reservation fee payment method.");

                int reservationId = details.CreateReservation(
                    selectedGuest.GuestId,
                    selectedRoom,
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
                    "Payment method recorded; no actual payment was processed.");

                ClearRoom();
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
                    ClearRoom();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Reservation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
