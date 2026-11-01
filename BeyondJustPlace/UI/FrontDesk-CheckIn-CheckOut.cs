using System;
using System.Data;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_CheckIn_CheckOut : Form
    {
        private readonly FrontDeskOperationsService operations = new FrontDeskOperationsService();
        private BillingInfo selectedBilling;

        public FrontDesk_CheckIn_CheckOut()
        {
            InitializeComponent();
           
            btnSearchCheckIn.Click += btnSearch_Click;
            button1.Click += btnConfirmCheckIn_Click;
            button3.Click += btnSearchCheckout_Click;
            btnCheckOut.Click += btnCheckOut_Click;

            dgvCheckInReservations.AllowUserToAddRows = false;
            dgvCheckInReservations.ReadOnly = true;
            dgvCheckInReservations.MultiSelect = false;
            dgvCheckInReservations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCheckInReservations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ClearBill();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                dgvCheckInReservations.Rows.Clear();
                DataTable matches = operations.SearchCheckIn(txtCheckInGuest.Text.Trim());
                foreach (DataRow item in matches.Rows)
                {
                    int index = dgvCheckInReservations.Rows.Add(
                        Convert.ToString(item["GuestName"]),
                        Convert.ToString(item["RoomNumber"]),
                        Convert.ToDateTime(item["CheckInDate"]).ToString("yyyy-MM-dd"),
                        Convert.ToString(item["ReservationStatus"]));
                    dgvCheckInReservations.Rows[index].Tag = Convert.ToInt32(item["ReservationId"]);
                }
                if (matches.Rows.Count == 0) MessageBox.Show("No Pending or Confirmed reservation found.");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnConfirmCheckIn_Click(object sender, EventArgs e)
        {
            if (dgvCheckInReservations.SelectedRows.Count == 0 ||
                dgvCheckInReservations.SelectedRows[0].Tag == null)
            {
                MessageBox.Show("Select a reservation from the results first.");
                return;
            }
            int id = Convert.ToInt32(dgvCheckInReservations.SelectedRows[0].Tag);
            if (MessageBox.Show("Check in reservation #" + id + "?", "Confirm Check-In",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                bool updated = operations.CheckIn(id);
                MessageBox.Show(updated ? "Guest checked in successfully." :
                    "Reservation is no longer Pending or Confirmed. Search again.");
                btnSearch_Click(sender, e);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnSearchCheckout_Click(object sender, EventArgs e)
        {
            selectedBilling = null;
            ClearBill();
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Enter the guest's name in the Check-Out section.");
                return;
            }
            try
            {
                selectedBilling = operations.GetCheckedInBilling(textBox1.Text.Trim());
                if (selectedBilling == null)
                {
                    MessageBox.Show("No checked-in reservation found.");
                    return;
                }
                lblGuestnameOutput.Text = selectedBilling.GuestName;
                lblRoomOutput.Text = selectedBilling.RoomNumber;
                lblStayOutput.Text = selectedBilling.StayDays + " night(s)";
                lblRoomCharges.Text = "₱" + selectedBilling.RoomCharges.ToString("N2");
                lblOtherCharges.Text = "₱" + selectedBilling.OtherCharges.ToString("N2");
                lblTotalBill.Text = "₱" + selectedBilling.TotalBill.ToString("N2");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            if (selectedBilling == null)
            {
                MessageBox.Show("Search a checked-in guest in the Check-Out section first.");
                return;
            }
            FrontDeskDashboard parent = MdiParent as FrontDeskDashboard;
            if (parent != null)
            {
                parent.OpenBilling(selectedBilling);
            }
            else
            {
                using (FrontDesk_Billing_PaymentUI form = new FrontDesk_Billing_PaymentUI(selectedBilling))
                {
                    form.ShowDialog(this);
                    if (form.PaymentCompleted)
                    {
                        textBox1.Clear();
                        selectedBilling = null;
                        ClearBill();
                    }
                }
            }
        }

        private void ClearBill()
        {
            lblGuestnameOutput.Text = "Output";
            lblRoomOutput.Text = "Output";
            lblStayOutput.Text = "Output";
            lblRoomCharges.Text = "₱0.00";
            lblOtherCharges.Text = "₱0.00";
            lblTotalBill.Text = "₱0.00";
        }

        private void btnBackReservation_Click(object sender, EventArgs e)
        {
            FrontDeskDashboard parent = MdiParent as FrontDeskDashboard;
            if (parent != null) parent.ShowDashboard();
            else Close();
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Check-In/Check-Out error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
