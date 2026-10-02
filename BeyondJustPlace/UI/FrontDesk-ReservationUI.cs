using System;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_ReservationUI : Form
    {
        private ReservationService service = new ReservationService();

        private Guest selectedGuest;
        private Room selectedRoom;

        private Timer refreshTimer = new Timer();

        public FrontDesk_ReservationUI()
        {
            InitializeComponent();

            this.Load += FrontDesk_ReservationUI_Load;

            listBoxAvailability.SelectedIndexChanged +=
                listBoxAvailability_SelectedIndexChanged;

            refreshTimer.Interval = 5000;
            refreshTimer.Tick += RefreshTimer_Tick;
        }

        private void FrontDesk_ReservationUI_Load(object sender, EventArgs e)
        {
            LoadRoomTypes();

            cmbReservationStatus.Items.Clear();
            cmbReservationStatus.Items.Add("Pending");
            cmbReservationStatus.Items.Add("Confirmed");
            cmbReservationStatus.SelectedIndex = 0;

            dateTimePickerCheckIn.Value = DateTime.Today;
            dateTimePickerCheckOut.Value = DateTime.Today.AddDays(1);

            LoadGuests();

            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadGuests();

            if (cmbRoomType.SelectedIndex >= 0)
            {
                LoadAvailableRooms(false);
            }
        }

        private void LoadGuests()
        {
            try
            {
                int selectedId = 0;

                if (selectedGuest != null)
                {
                    selectedId = selectedGuest.GuestId;
                }

                var guests = service.SearchGuest(txtUsername.Text.Trim());

                listBoxGuest.Items.Clear();

                foreach (Guest guest in guests)
                {
                    listBoxGuest.Items.Add(guest);

                    if (guest.GuestId == selectedId)
                    {
                        listBoxGuest.SelectedItem = guest;
                    }
                }
            }
            catch
            {
            }
        }

        private void LoadRoomTypes()
        {
            try
            {
                cmbRoomType.Items.Clear();

                var roomTypes = service.GetRoomTypes();

                foreach (string roomType in roomTypes)
                {
                    cmbRoomType.Items.Add(roomType);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadGuests();

            if (listBoxGuest.Items.Count == 0)
            {
                MessageBox.Show("No guest found.");
            }
        }

        private void btnSelectguest_Click(object sender, EventArgs e)
        {
            if (listBoxGuest.SelectedItem == null)
            {
                MessageBox.Show("Please select a guest.");
                return;
            }

            selectedGuest = (Guest)listBoxGuest.SelectedItem;

            lblGuestname.Text = selectedGuest.GuestName;
        }

        private void txtGuestName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContactNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                selectedGuest = service.CreateGuest(
                    txtGuestName.Text.Trim(),
                    txtContactNumber.Text.Trim());

                lblGuestname.Text = selectedGuest.GuestName;

                MessageBox.Show("Guest created successfully.");

                txtGuestName.Clear();
                txtContactNumber.Clear();

                LoadGuests();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dateTimePickerCheckIn_ValueChanged(object sender, EventArgs e)
        {
            UpdateStay();

            selectedRoom = null;
            lblRoomOutput.Text = "Output";

            listBoxAvailability.Items.Clear();
        }

        private void dateTimePickerCheckOut_ValueChanged(object sender, EventArgs e)
        {
            UpdateStay();

            selectedRoom = null;
            lblRoomOutput.Text = "Output";

            listBoxAvailability.Items.Clear();
        }

        private void cmbRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedRoom = null;
            lblRoomOutput.Text = "Output";

            listBoxAvailability.Items.Clear();
        }

        private void btnCheckAvailability_Click(object sender, EventArgs e)
        {
            LoadAvailableRooms(true);
        }

        private void LoadAvailableRooms(bool showMessage)
        {
            try
            {
                if (cmbRoomType.SelectedIndex < 0)
                {
                    if (showMessage)
                    {
                        MessageBox.Show("Please select a room type.");
                    }

                    return;
                }

                if (dateTimePickerCheckOut.Value.Date <=
                    dateTimePickerCheckIn.Value.Date)
                {
                    if (showMessage)
                    {
                        MessageBox.Show(
                            "Check-out date must be later than check-in date.");
                    }

                    return;
                }

                int selectedRoomId = 0;

                if (selectedRoom != null)
                {
                    selectedRoomId = selectedRoom.RoomId;
                }

                var rooms = service.GetAvailableRooms(
                    dateTimePickerCheckIn.Value,
                    dateTimePickerCheckOut.Value,
                    cmbRoomType.Text);

                listBoxAvailability.Items.Clear();

                bool roomFound = false;

                foreach (Room room in rooms)
                {
                    listBoxAvailability.Items.Add(room);

                    if (room.RoomId == selectedRoomId)
                    {
                        listBoxAvailability.SelectedItem = room;
                        selectedRoom = room;
                        roomFound = true;
                    }
                }

                if (selectedRoomId != 0 && roomFound == false)
                {
                    selectedRoom = null;
                    lblRoomOutput.Text = "Output";
                }

                if (rooms.Count == 0 && showMessage)
                {
                    MessageBox.Show("No available rooms found.");
                }
            }
            catch (Exception ex)
            {
                if (showMessage)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void listBoxAvailability_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (listBoxAvailability.SelectedItem == null)
            {
                return;
            }

            selectedRoom = (Room)listBoxAvailability.SelectedItem;

            lblRoomOutput.Text = selectedRoom.RoomNumber;

            UpdateStay();
        }

        private void UpdateStay()
        {
            TimeSpan stay =
                dateTimePickerCheckOut.Value.Date -
                dateTimePickerCheckIn.Value.Date;

            if (stay.Days > 0)
            {
                lblStayDate.Text = stay.Days + " night(s)";
            }
            else
            {
                lblStayDate.Text = "Output";
            }
        }

        private void cmbReservationStatus_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {

        }

        private void btnCreateReservation_Click(object sender, EventArgs e)
        {
            try
            {
                int reservationId = service.CreateReservation(
                    selectedGuest,
                    selectedRoom,
                    dateTimePickerCheckIn.Value,
                    dateTimePickerCheckOut.Value,
                    cmbReservationStatus.Text);

                MessageBox.Show(
                    "Reservation created successfully!\n" +
                    "Reservation ID: " + reservationId);

                RefreshDashboard();

                ClearReservation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedGuest == null)
                {
                    MessageBox.Show("Please select a guest first.");
                    return;
                }

                DialogResult answer = MessageBox.Show(
                    "Cancel the reservation of " + selectedGuest.GuestName + "?",
                    "Cancel Reservation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (answer == DialogResult.Yes)
                {
                    bool cancelled = service.CancelReservation(selectedGuest);

                    if (cancelled)
                    {
                        MessageBox.Show("Reservation cancelled successfully.");

                        RefreshDashboard();

                        LoadGuests();

                        if (cmbRoomType.SelectedIndex >= 0)
                        {
                            LoadAvailableRooms(false);
                        }

                        ClearReservation();
                    }
                    else
                    {
                        MessageBox.Show(
                            "This guest has no Pending or Confirmed reservation.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RefreshDashboard()
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is FrontDeskDashboard)
                {
                    FrontDeskDashboard dashboard =
                        (FrontDeskDashboard)form;

                    dashboard.RefreshDashboardData();
                }
            }
        }
        private void ClearReservation()
        {
            selectedGuest = null;
            selectedRoom = null;

            lblGuestname.Text = "Output";
            lblStayDate.Text = "Output";
            lblRoomOutput.Text = "Output";

            listBoxGuest.ClearSelected();
            listBoxAvailability.Items.Clear();

            cmbRoomType.SelectedIndex = -1;

            if (cmbReservationStatus.Items.Count > 0)
            {
                cmbReservationStatus.SelectedIndex = 0;
            }

            dateTimePickerCheckIn.Value = DateTime.Today;
            dateTimePickerCheckOut.Value = DateTime.Today.AddDays(1);

            LoadGuests();
        }

        private void btnBackReservation_Click(object sender, EventArgs e)
        {
            refreshTimer.Stop();

            FrontDeskDashboard ui = new FrontDeskDashboard();

            ui.Show();
            this.Close();
        }

        private void FrontDesk_ReservationUI_Load_1(object sender, EventArgs e)
        {

        }
    }
}