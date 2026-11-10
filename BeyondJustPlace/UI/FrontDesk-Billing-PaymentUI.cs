using System;
using System.Globalization;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_Billing_PaymentUI : Form
    {
        // Create the SQL service only when a staff member searches or pays.
        // This keeps the Windows Forms Designer usable without a live DB.
        private FrontDeskOperationsService operations;

        private FrontDeskOperationsService Operations
        {
            get
            {
                if (operations == null)
                    operations = new FrontDeskOperationsService();
                return operations;
            }
        }

        private BillingInfo selectedBilling;

        // Used by Check-In / Check-Out after ShowDialog closes.
        public bool PaymentCompleted { get; private set; }

        // Required by Visual Studio WinForms Designer.
        public FrontDesk_Billing_PaymentUI() : this(null)
        {
        }

        // Called from FrontDeskDashboard.OpenBilling(selectedBilling).
        public FrontDesk_Billing_PaymentUI(BillingInfo billing)
        {
            InitializeComponent();
            selectedBilling = billing;

            // Event handlers are wired here, not twice in Designer.cs.
            this.Load += BillingForm_Load;
            btnSearchGuest.Click += BtnSearchGuest_Click;
            btnConfirmPayment.Click += BtnConfirmPayment_Click;
            btnCancel.Click += BtnCancel_Click;
            cmbPaymentMethod.SelectedIndexChanged += PaymentMethod_Changed;
            txtAmountReceived.TextChanged += AmountReceived_Changed;

            textBox1.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SearchCheckedInGuest();
                }
            };
        }

        private void BillingForm_Load(object sender, EventArgs e)
        {
            cmbPaymentMethod.Items.Clear();
            cmbPaymentMethod.Items.AddRange(
                new object[] { "Cash", "GCash", "Card" });
            cmbPaymentMethod.SelectedIndex = -1;

            if (selectedBilling != null)
                textBox1.Text = selectedBilling.GuestName;

            DisplayBilling();
        }

        private void BtnSearchGuest_Click(object sender, EventArgs e)
        {
            SearchCheckedInGuest();
        }

        private void SearchCheckedInGuest()
        {
            string keyword = textBox1.Text.Trim();
            if (keyword.Length == 0)
            {
                MessageBox.Show("Enter a guest name or reservation ID.");
                return;
            }

            try
            {
                BillingInfo result;
                int reservationId;

                // Exact reservation ID search or existing name search.
                if (int.TryParse(keyword, out reservationId))
                    result = Operations.GetCheckedInBillingByReservationId(reservationId);
                else
                    result = Operations.GetCheckedInBilling(keyword);

                if (result == null)
                {
                    selectedBilling = null;
                    DisplayBilling();
                    MessageBox.Show(
                        "No checked-in reservation found for that search.",
                        "Search Guest", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                selectedBilling = result;
                PaymentCompleted = false;
                cmbPaymentMethod.SelectedIndex = -1;
                DisplayBilling();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Guest Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayBilling()
        {
            bool hasGuest = selectedBilling != null;
            btnConfirmPayment.Enabled = hasGuest && !PaymentCompleted;

            lblReservationIDOutput.Text = hasGuest
                ? selectedBilling.ReservationId.ToString()
                : "Output";
            lblGuestNameOutput.Text = hasGuest
                ? selectedBilling.GuestName : "Output";
            lblRoomOutput.Text = hasGuest
                ? selectedBilling.RoomNumber : "Output";
            lblStayOutput.Text = hasGuest
                ? selectedBilling.StayDays + " night(s)" : "Output";

            // Existing label3, lblCheckOutOutput and label5 retain
            // their Designer names; only their displayed values change.
            label3.Text = hasGuest && selectedBilling.CheckInDate.HasValue
                ? selectedBilling.CheckInDate.Value.ToString("MMM dd, yyyy")
                : "Output";
            lblCheckOutOutput.Text =
                hasGuest && selectedBilling.CheckOutDate.HasValue
                ? selectedBilling.CheckOutDate.Value.ToString("MMM dd, yyyy")
                : "Output";
            label5.Text = hasGuest
                ? selectedBilling.ReservationStatus : "Output";

            lblRoomChargesRate.Text = hasGuest
                ? Money(selectedBilling.RoomRate) : "0.00";
            lblRoomChargesQty.Text = hasGuest
                ? selectedBilling.StayDays.ToString() : "0";
            lblRoomChargesAmount.Text = hasGuest
                ? Money(selectedBilling.RoomCharges) : "0.00";

            lblOtherChargesRate.Text = hasGuest
                ? Money(selectedBilling.OtherCharges) : "0.00";
            lblOtherChargesQty.Text =
                hasGuest && selectedBilling.OtherCharges > 0m ? "1" : "0";
            lblOtherChargesAmount.Text = hasGuest
                ? Money(selectedBilling.OtherCharges) : "0.00";

            lblTotalBill.Text = hasGuest
                ? "PHP " + Money(selectedBilling.TotalBill) : "PHP 0.00";
            lblPaymentAmountOutput.Text = lblTotalBill.Text;

            cmbPaymentMethod.Enabled = hasGuest;
            txtAmountReceived.Enabled = hasGuest;
            txtAmountReceived.Text = "0.00";
            lblChangeOutput.Text = "PHP 0.00";
        }

        private static string Money(decimal value)
        {
            return value.ToString("N2", CultureInfo.CurrentCulture);
        }

        private void PaymentMethod_Changed(object sender, EventArgs e)
        {
            if (selectedBilling == null) return;

            bool isCash = cmbPaymentMethod.Text == "Cash";
            txtAmountReceived.ReadOnly = !isCash;

            if (!isCash && cmbPaymentMethod.SelectedIndex >= 0)
                txtAmountReceived.Text = Money(selectedBilling.TotalBill);
            else
                txtAmountReceived.Text = "0.00";

            UpdateChange();
        }

        private void AmountReceived_Changed(object sender, EventArgs e)
        {
            UpdateChange();
        }

        private bool TryGetAmountReceived(out decimal amount)
        {
            return decimal.TryParse(txtAmountReceived.Text.Trim(),
                NumberStyles.Currency, CultureInfo.CurrentCulture,
                out amount);
        }

        private void UpdateChange()
        {
            decimal received;
            if (selectedBilling == null ||
                !TryGetAmountReceived(out received) || received < 0m)
            {
                lblChangeOutput.Text = "PHP 0.00";
                return;
            }

            decimal change = cmbPaymentMethod.Text == "Cash"
                ? Math.Max(0m, received - selectedBilling.TotalBill)
                : 0m;
            lblChangeOutput.Text = "PHP " + Money(change);
        }

        private void BtnConfirmPayment_Click(object sender, EventArgs e)
        {
            if (PaymentCompleted) return;

            if (selectedBilling == null)
            {
                MessageBox.Show("Search for a checked-in guest first.");
                return;
            }
            if (cmbPaymentMethod.SelectedIndex < 0)
            {
                MessageBox.Show("Choose Cash, GCash, or Card.");
                return;
            }

            decimal received;
            if (!TryGetAmountReceived(out received) || received < 0m)
            {
                MessageBox.Show("Enter a valid payment amount.");
                return;
            }

            if (cmbPaymentMethod.Text == "Cash" &&
                received < selectedBilling.TotalBill)
            {
                MessageBox.Show("Cash received must cover the total bill.");
                return;
            }
            if (cmbPaymentMethod.Text != "Cash" &&
                received != selectedBilling.TotalBill)
            {
                MessageBox.Show(
                    "GCash or Card must be confirmed for the full bill amount.");
                return;
            }

            string warning = cmbPaymentMethod.Text == "Cash"
                ? "Confirm that you received the cash?"
                : "Confirm that the GCash/Card payment was received " +
                  "and verified outside this application?";

            DialogResult response = MessageBox.Show(
                warning + "\n\nGuest: " + selectedBilling.GuestName +
                "\nReservation ID: " + selectedBilling.ReservationId +
                "\nTotal Bill: PHP " + Money(selectedBilling.TotalBill) +
                "\nMethod: " + cmbPaymentMethod.Text,
                "Confirm Payment and Check-Out",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (response != DialogResult.Yes) return;

            try
            {
                btnConfirmPayment.Enabled = false;

                // This existing SQL operation records the full bill
                // and marks the reservation Checked Out atomically.
                Operations.CompleteCheckout(selectedBilling,
                    cmbPaymentMethod.Text);

                PaymentCompleted = true;
                selectedBilling.ReservationStatus = "Checked Out";

                MessageBox.Show(
                    "Payment saved to SQL. Guest checked out successfully.",
                    "Payment Completed", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                FrontDeskDashboard dashboard = MdiParent as FrontDeskDashboard;
                if (dashboard != null)
                    dashboard.ShowDashboard();
                else
                    Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Payment Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && !PaymentCompleted)
                    btnConfirmPayment.Enabled = true;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            FrontDeskDashboard dashboard = MdiParent as FrontDeskDashboard;
            if (dashboard != null)
                dashboard.ShowDashboard();
            else
                Close();
        }
    }
}