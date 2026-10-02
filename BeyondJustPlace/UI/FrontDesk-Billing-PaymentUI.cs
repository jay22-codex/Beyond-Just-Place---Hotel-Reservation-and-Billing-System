using System;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_Billing_PaymentUI : Form
    {
        private BillingService service =
            new BillingService();

        private BillingInfo selectedBilling;

        public bool PaymentCompleted { get; private set; }

        public FrontDesk_Billing_PaymentUI()
        {
            InitializeComponent();

            SetupPayment();
        }

        public FrontDesk_Billing_PaymentUI(
            BillingInfo billing)
        {
            InitializeComponent();

            selectedBilling = billing;

            SetupPayment();

            DisplayBilling();
        }

        private void SetupPayment()
        {
            cmbPaymentMethod.Items.Clear();

            cmbPaymentMethod.Items.Add("Cash");
            cmbPaymentMethod.Items.Add("GCash");
            cmbPaymentMethod.Items.Add("Card");

            cmbPaymentMethod.SelectedIndex = -1;

            btnConfirmPayment.Click -=
                btnConfirmPayment_Click;

            btnConfirmPayment.Click +=
                btnConfirmPayment_Click;
        }

        private void FrontDesk_Billing_PaymentUI_Load(
            object sender,
            EventArgs e)
        {
            if (selectedBilling != null)
            {
                DisplayBilling();
            }
        }

        private void DisplayBilling()
        {
            if (selectedBilling == null)
                return;

            lblGuestNameOutput.Text =
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

            lblPaymentAmount.Text =
                "₱" +
                selectedBilling.TotalBill
                .ToString("N2");
        }

        private void btnConfirmPayment_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (selectedBilling == null)
                {
                    MessageBox.Show(
                        "No billing information found.");

                    return;
                }

                if (cmbPaymentMethod.SelectedIndex < 0)
                {
                    MessageBox.Show(
                        "Please select payment method.");

                    return;
                }

                DialogResult result =
                    MessageBox.Show(
                        "Confirm payment of ₱" +
                        selectedBilling.TotalBill
                        .ToString("N2") +
                        " using " +
                        cmbPaymentMethod.Text +
                        "?",
                        "Confirm Payment",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    service.ConfirmPayment(
                        selectedBilling,
                        cmbPaymentMethod.Text);

                    PaymentCompleted = true;

                    MessageBox.Show(
                        "Payment successful.\n" +
                        "Method: " +
                        cmbPaymentMethod.Text +
                        "\nAmount: ₱" +
                        selectedBilling.TotalBill
                        .ToString("N2"));

                    this.Close();
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