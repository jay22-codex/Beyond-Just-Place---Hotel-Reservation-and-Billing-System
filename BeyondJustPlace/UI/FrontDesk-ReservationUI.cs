
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_ReservationUI : Form
    {
        private readonly ReservationService reservationService =
            new ReservationService();

        private readonly ReservationDetailsService details =
            new ReservationDetailsService();

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
            btnEditGuest.Click += SelectGuest_Click;

            btnCheckAvailability.Click += CheckAvailability_Click;
            btnSelectRoom.Click += SelectRoom_Click;
            btnSelectPreferredRoom.Click += PreferredRoom_Click;

            btnCreateReservation.Click += CreateReservation_Click;
            btnCancelReservation.Click += CancelReservation_Click;

            listBoxGuest.DoubleClick += SelectGuest_Click;

            dgvAvailableRooms.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SelectRoom();
            };

            cmbPreferredRoomType.SelectedIndexChanged +=
                PreferredTypeChanged;

            cmbRoomType.SelectedIndexChanged +=
                (s, e) => ClearRoom();

            dtpCheckIn.ValueChanged += (s, e) =>
            {
                ClearRoom();
                UpdateSummary();
            };

            dtpCheckOut.ValueChanged += (s, e) =>
            {
                ClearRoom();
                UpdateSummary();
            };

            txtGuestName.TextChanged +=
                (s, e) => UpdateSummary();

            dgvAvailableRooms.AllowUserToAddRows = false;
            dgvAvailableRooms.ReadOnly = true;
            dgvAvailableRooms.MultiSelect = false;

            dgvAvailableRooms.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvAvailableRooms.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Reservation_Load(object sender, EventArgs e)
        {
            try
            {
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
                        "National ID",
                        "Passport",
                        "Driver's License",
                        "Student ID",
                        "Other"
                    });
                }

                cmbReservationFeeMethod.Items.Clear();

                cmbReservationFeeMethod.Items.AddRange(
                    new object[] { "Cash", "GCash", "Card" });

                cmbReservationFeeMethod.DropDownStyle =
                    ComboBoxStyle.DropDownList;

                List<string> types =
                    reservationService.GetRoomTypes();

                cmbRoomType.Items.Clear();
                cmbPreferredRoomType.Items.Clear();

                foreach (string type in types)
                {
                    cmbRoomType.Items.Add(type);
                    cmbPreferredRoomType.Items.Add(type);
                }

                // Adds a New Guest button without changing the Designer.
                btnNewGuest = new Button();
                btnNewGuest.Name = "btnNewGuest";
                btnNewGuest.Text = "New Guest";
                btnNewGuest.Left = 301;
                btnNewGuest.Top = 271;
                btnNewGuest.Width = 100;
                btnNewGuest.Height = 37;

                btnNewGuest.Click += (s, args) => NewGuest();

                groupBox4.Controls.Add(btnNewGuest);

                LoadGuests();
                UpdateSummary();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void FillCount(
            ComboBox combo,
            int min,
            int max,
            int start)
        {
            combo.Items.Clear();

            for (int n = min; n <= max; n++)
                combo.Items.Add(n.ToString());

            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.SelectedItem = start.ToString();
        }

        private int ReadCount(ComboBox combo, string label)
        {
            int result;

            if (!int.TryParse(combo.Text, out result))
                throw new InvalidOperationException(
                    "Select " + label + ".");

            return result;
        }

        private void LoadGuests()
        {
            int currentId =
                selectedGuest == null ? 0 : selectedGuest.GuestId;

            listBoxGuest.BeginUpdate();

            try
            {
                listBoxGuest.Items.Clear();

                foreach (Guest guest in
                    reservationService.SearchGuest(
                        txtSearchGuest.Text.Trim()))
                {
                    listBoxGuest.Items.Add(guest);

                    if (guest.GuestId == currentId)
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
            Guest item = listBoxGuest.SelectedItem as Guest;

            if (item == null)
            {
                MessageBox.Show("Select a guest record first.");
                return;
            }

            try
            {
                selectedGuest = details.GetGuest(item.GuestId);

                if (selectedGuest == null)
                {
                    MessageBox.Show("Guest not found. Search again.");
                    return;
                }

                txtGuestName.Text = selectedGuest.GuestName;
                txtContactNumber.Text =
                    selectedGuest.ContactNumber;

                txtEmail.Text = selectedGuest.Email;
                txtAddress.Text = selectedGuest.HomeAddress;
                cmbIdType.Text = selectedGuest.IdType;

                btnSaveGuest.Text = "Update Guest Record";

                UpdateSummary();
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
                if (!string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MailAddress address =
                        new MailAddress(txtEmail.Text.Trim());

                    if (address.Address != txtEmail.Text.Trim())
                        throw new InvalidOperationException(
                            "Enter a valid email address.");
                }

                Guest guest = new Guest
                {
                    GuestId = selectedGuest == null
                        ? 0
                        : selectedGuest.GuestId,

                    GuestName = txtGuestName.Text.Trim(),

                    ContactNumber =
                        txtContactNumber.Text.Trim(),

                    Email = txtEmail.Text.Trim(),

                    HomeAddress = txtAddress.Text.Trim(),

                    IdType = cmbIdType.Text.Trim()
                };

                bool editing = guest.GuestId > 0;

                selectedGuest = details.SaveGuest(guest);

                txtSearchGuest.Clear();
                LoadGuests();

                btnSaveGuest.Text = "Update Guest Record";

                UpdateSummary();

                MessageBox.Show(editing
                    ? "Guest updated and saved to SQL."
                    : "Guest saved to SQL.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private bool GuestHasUnsavedChanges()
        {
            return selectedGuest != null &&
                (
                    txtGuestName.Text.Trim() !=
                        (selectedGuest.GuestName ?? "") ||

                    txtContactNumber.Text.Trim() !=
                        (selectedGuest.ContactNumber ?? "") ||

                    txtEmail.Text.Trim() !=
                        (selectedGuest.Email ?? "") ||

                    txtAddress.Text.Trim() !=
                        (selectedGuest.HomeAddress ?? "") ||

                    cmbIdType.Text.Trim() !=
                        (selectedGuest.IdType ?? "")
                );
        }

        private void PreferredTypeChanged(
            object sender,
            EventArgs e)
        {
            if (cmbPreferredRoomType.SelectedIndex >= 0)
            {
                cmbRoomType.SelectedItem =
                    cmbPreferredRoomType.SelectedItem;
            }
        }

        private void PreferredRoom_Click(
            object sender,
            EventArgs e)
        {
            if (cmbPreferredRoomType.SelectedIndex < 0)
            {
                MessageBox.Show("Select a room type first.");
                return;
            }

            cmbRoomType.SelectedItem =
                cmbPreferredRoomType.SelectedItem;

            CheckAvailability_Click(sender, e);
        }

        private void CheckAvailability_Click(
            object sender,
            EventArgs e)
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
            ClearRoom();

            if (cmbRoomType.SelectedIndex < 0)
                throw new InvalidOperationException(
                    "Select a room type.");

            List<AvailableRoomInfo> rooms =
                details.GetAvailableRooms(
                    dtpCheckIn.Value.Date,
                    dtpCheckOut.Value.Date,
                    cmbRoomType.Text);

            foreach (AvailableRoomInfo room in rooms)
            {
                int index = dgvAvailableRooms.Rows.Add(
                    room.RoomNumber,
                    room.RoomType,

                    room.RoomCapacity.HasValue
                        ? room.RoomCapacity.Value.ToString()
                        : "Not set",

                    room.ReservationFee.HasValue
                        ? "PHP " +
                            room.ReservationFee.Value.ToString("N2")
                        : "Not set",

                    room.RoomRate.ToString("N2"),

                    "Available"
                );

                dgvAvailableRooms.Rows[index].Tag = room;
            }

            if (rooms.Count == 0)
                MessageBox.Show(
                    "No rooms available for those dates.");
        }

        private void SelectRoom_Click(object sender, EventArgs e)
        {
            SelectRoom();
        }

        private void SelectRoom()
        {
            if (dgvAvailableRooms.SelectedRows.Count == 0 ||
                !(dgvAvailableRooms.SelectedRows[0].Tag
                    is AvailableRoomInfo))
            {
                MessageBox.Show(
                    "Select a room from the available rooms table.");
                return;
            }

            selectedRoom =
                (AvailableRoomInfo)
                    dgvAvailableRooms.SelectedRows[0].Tag;

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
            lblSummaryGuest.Text =
                !string.IsNullOrWhiteSpace(txtGuestName.Text)
                ? txtGuestName.Text.Trim()
                : "Output";

            int nights =
                (dtpCheckOut.Value.Date -
                 dtpCheckIn.Value.Date).Days;

            lblSummaryStay.Text = nights > 0
                ? nights + " night(s)"
                : "Output";

            lblSummaryRoom.Text = selectedRoom == null
                ? "Output"
                : selectedRoom.RoomNumber;

            lblSummaryRate.Text = selectedRoom == null
                ? "Output"
                : "PHP " +
                    selectedRoom.RoomRate.ToString("N2") +
                    "/night";

            lblSummaryReservationFee.Text =
                selectedRoom == null
                ? "Output"
                : selectedRoom.ReservationFee.HasValue
                    ? "PHP " +
                        selectedRoom.ReservationFee.Value
                            .ToString("N2")
                    : "Not configured";

            lblReservationFeePreview.Text =
                selectedRoom == null
                ? "Output"
                : selectedRoom.ReservationFee.HasValue
                    ? "PHP " +
                        selectedRoom.ReservationFee.Value
                            .ToString("N2")
                    : "Not configured";
        }

        private void CreateReservation_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (selectedGuest == null)
                    throw new InvalidOperationException(
                        "Save or select the guest first.");

                if (GuestHasUnsavedChanges())
                    throw new InvalidOperationException(
                        "Guest details changed. Click Update " +
                        "Guest Record before reserving.");

                if (selectedRoom == null)
                    throw new InvalidOperationException(
                        "Select an available room first.");

                int totalGuests =
                    ReadCount(nudGuestCount, "Number of Guests");

                int adults =
                    ReadCount(nudAdults, "Adults");

                int children =
                    ReadCount(nudChildren, "Children");

                int rooms =
                    ReadCount(nudRoomCount, "Rooms");

                if (totalGuests != adults + children)
                    throw new InvalidOperationException(
                        "Number of Guests must equal " +
                        "Adults + Children.");

                if (rooms != 1)
                    throw new InvalidOperationException(
                        "The current system supports one room " +
                        "per reservation. Select Rooms = 1.");

                if (selectedRoom.RoomCapacity.HasValue &&
                    totalGuests > selectedRoom.RoomCapacity.Value)
                    throw new InvalidOperationException(
                        "Guest count exceeds the selected room capacity.");

                string method = cmbReservationFeeMethod.Text;

                if (selectedRoom.ReservationFee
                        .GetValueOrDefault() > 0 &&
                    string.IsNullOrWhiteSpace(method))
                    throw new InvalidOperationException(
                        "Choose a reservation fee payment method.");

                int id = details.CreateReservation(
                    selectedGuest.GuestId,
                    selectedRoom,
                    dtpCheckIn.Value,
                    dtpCheckOut.Value,
                    totalGuests,
                    adults,
                    children,
                    rooms,
                    txtSpecialRequest.Text.Trim(),
                    method);

                MessageBox.Show(
                    "Reservation #" + id +
                    " saved to SQL as Pending.\n\n" +
                    "The reservation fee method was recorded, " +
                    "but no actual payment was processed.");

                selectedRoom = null;
                dgvAvailableRooms.Rows.Clear();

                UpdateSummary();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void CancelReservation_Click(
            object sender,
            EventArgs e)
        {
            if (selectedGuest == null)
            {
                MessageBox.Show("Select a guest first.");
                return;
            }

            DialogResult answer = MessageBox.Show(
                "Cancel the latest Pending/Confirmed reservation for " +
                    selectedGuest.GuestName + "?",
                "Cancel Reservation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            try
            {
                bool cancelled =
                    reservationService.CancelReservation(
                        selectedGuest);

                MessageBox.Show(cancelled
                    ? "Latest reservation cancelled in SQL."
                    : "No active reservation found.");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Reservation Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
