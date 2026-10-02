using System;
using System.Data;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_CheckIn_CheckOut : Form
    {
        private ReservationService reservationService =
            new ReservationService();

        private BillingService billingService =
            new BillingService();

        private BillingInfo selectedBilling;

        public FrontDesk_CheckIn_CheckOut()
        {
            InitializeComponent();

            dataGridViewReservation.ReadOnly = true;
            dataGridViewReservation.AllowUserToAddRows = false;
            dataGridViewReservation.AllowUserToDeleteRows = false;
            dataGridViewReservation.MultiSelect = false;

            dataGridViewReservation.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            lblConfirm.Click -= lblConfirm_Click;
            lblConfirm.Click += lblConfirm_Click;

            btnSearch1.Click -= btnSearch1_Click;
            btnSearch1.Click += btnSearch1_Click;

            btnCheckOut.Click -= btnCheckOut_Click;
            btnCheckOut.Click += btnCheckOut_Click;

            btnBackReservation.Click -=
                btnBackReservation_Click;

            btnBackReservation.Click +=
                btnBackReservation_Click;
        }

        private void SearchCheckIn()
        {
           
        }

        private void lblConfirm_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (dataGridViewReservation
                    .SelectedRows.Count == 0)
                {
                    MessageBox.Show(
                        "Please select a reservation.");

                    return;
                }

                DataGridViewRow row =
                    dataGridViewReservation
                    .SelectedRows[0];

                int reservationId =
                    Convert.ToInt32(row.Tag);

                reservationService.CheckInGuest(
                    reservationId);

                MessageBox.Show(
                    "Guest checked in successfully.");

                SearchCheckIn();

                RefreshDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message);
            }
        }


        private void btnCheckOut_Click(
            object sender,
            EventArgs e)
        {
            if (selectedBilling == null)
            {
                MessageBox.Show(
                    "Please search a checked-in guest first.");

                return;
            }

            FrontDesk_Billing_PaymentUI billingUI =
                new FrontDesk_Billing_PaymentUI(
                    selectedBilling);

            billingUI.ShowDialog();

            if (billingUI.PaymentCompleted)
            {
                ClearCheckOut();

                RefreshDashboard();
            }
        }

        private void ClearCheckOut()
        {
            selectedBilling = null;

            txtGuestName1.Clear();

            lblGuestnameOutput.Text =
                "Guest name Output";

            lblRoomOutput.Text =
                "Room Output";

            lblStayOutput.Text =
                "Stay Output";

            lblRoomCharges.Text =
                "0.00";

            lblOtherCharges.Text =
                "0.00";

            lblTotalBill.Text =
                "0.00";
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

        private void btnBackReservation_Click(
            object sender,
            EventArgs e)
        {
            FrontDeskDashboard dashboard =
                new FrontDeskDashboard();

            dashboard.Show();

            this.Close();
        }

        private void btnSearch1_Click(object sender, EventArgs e)
        {
            try
            {
                selectedBilling =
                    billingService.SearchGuest(
                        txtGuestName1.Text.Trim());

                if (selectedBilling == null)
                {
                    MessageBox.Show(
                        "Checked-in guest not found.");

                    ClearCheckOut();

                    return;
                }

                lblGuestnameOutput.Text =
                    selectedBilling.GuestName;

                lblRoomOutput.Text =
                    selectedBilling.RoomNumber;

                lblStayOutput.Text =
                    selectedBilling.StayDays +
                    " night(s)";

                lblRoomCharges.Text =
                    "₱" +
                    selectedBilling.RoomCharges
                    .ToString("N2");

                lblOtherCharges.Text =
                    "₱" +
                    selectedBilling.OtherCharges
                    .ToString("N2");

                lblTotalBill.Text =
                    "₱" +
                    selectedBilling.TotalBill
                    .ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable reservations =
                    reservationService.SearchCheckInReservations(
                        txtGuestName.Text.Trim());

                dataGridViewReservation.Rows.Clear();

                foreach (DataRow row in reservations.Rows)
                {
                    int index =
                        dataGridViewReservation.Rows.Add(
                            row["GuestName"].ToString(),
                            row["RoomNumber"].ToString(),
                            Convert.ToDateTime(
                                row["CheckInDate"])
                                .ToShortDateString(),
                            row["ReservationStatus"].ToString());

                    dataGridViewReservation
                        .Rows[index].Tag =
                        Convert.ToInt32(
                            row["ReservationId"]);
                }

                dataGridViewReservation.ClearSelection();

                if (reservations.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "No reservation found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message);
            }
        }
    }
}