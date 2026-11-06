
using System;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_Billing_PaymentUI : Form
    {
        private readonly BillingInfo selectedBilling;

        // Check-In/Check-Out reads this after the Billing form closes.
        public bool PaymentCompleted { get; private set; }

        // Keep this constructor for the Windows Forms Designer.
        public FrontDesk_Billing_PaymentUI() : this(null)
        {
        }

        // Called by Check-Out and FrontDeskDashboard.OpenBilling().
        public FrontDesk_Billing_PaymentUI(BillingInfo billing)
        {
            InitializeComponent();
            selectedBilling = billing;

            // Prevent duplicate event calls whether or not the Designer
            // already connects these two events.
            this.Load -= FrontDesk_Billing_PaymentUI_Load;
            this.Load += FrontDesk_Billing_PaymentUI_Load;

            Button confirm = FindControl<Button>(
                "btnConfirmPayment", "button2");

            if (confirm != null)
            {
                confirm.Click -= btnConfirmPayment_Click;
                confirm.Click += btnConfirmPayment_Click;
            }
        }

        private void FrontDesk_Billing_PaymentUI_Load(
            object sender, EventArgs e)
        {
            // Supports both versions of your Billing Designer.
            ComboBox method = FindControl<ComboBox>(
                "cmbPaymentMethod", "cmbRoomType");

            if (method != null)
            {
                method.Items.Clear();
                method.Items.AddRange(
                    new object[] { "Cash", "GCash", "Card" });

                method.DropDownStyle = ComboBoxStyle.DropDownList;
                method.SelectedIndex = -1;
            }

            // Older Designer uses a payment amount TextBox.
            TextBox amountBox = FindControl<TextBox>("textBox3");

            if (amountBox != null)
                amountBox.ReadOnly = true;

            // Hide obsolete guest-search fields in the older Designer.
            // Guests are selected from Check-Out.
            if (FindControl<TextBox>("textBox2") != null)
            {
                Control searchLabel =
                    FindControl<Control>("label23");

                Control searchText =
                    FindControl<Control>("textBox2");

                Control searchButton =
                    FindControl<Control>("button1");

                if (searchLabel != null)
                    searchLabel.Visible = false;

                if (searchText != null)
                    searchText.Visible = false;

                if (searchButton != null)
                    searchButton.Visible = false;
            }

            groupBox3.Text = "Selected Guest";

            DisplayBilling();
        }

        // =========================================
        // DISPLAY GUEST AND BILLING INFORMATION
        // =========================================

        private void DisplayBilling()
        {
            Button confirm = FindControl<Button>(
                "btnConfirmPayment", "button2");

            if (selectedBilling == null)
            {
                SetOutput(
                    "No guest selected",
                    "lblGuestNameOutput",
                    "label19");

                SetOutput(
                    "Output",
                    "lblRoomOutput",
                    "label18");

                SetOutput(
                    "Output",
                    "lblStayOutput",
                    "label17");

                lblRoomCharges.Text = "PHP 0.00";
                lblOtherCharges.Text = "PHP 0.00";
                lblTotalBill.Text = "PHP 0.00";

                SetOutput(
                    "0.00",
                    "lblPaymentAmount",
                    "textBox3");

                if (confirm != null)
                    confirm.Enabled = false;

                return;
            }

            // GUEST NAME
            SetOutput(
                selectedBilling.GuestName,
                "lblGuestNameOutput",
                "label19");

            // ROOM NUMBER
            SetOutput(
                selectedBilling.RoomNumber,
                "lblRoomOutput",
                "label18");

            // LENGTH OF STAY
            SetOutput(
                selectedBilling.StayDays + " night(s)",
                "lblStayOutput",
                "label17");

            // ROOM CHARGES
            lblRoomCharges.Text =
                "PHP " +
                selectedBilling.RoomCharges.ToString("N2");

            // OTHER CHARGES
            lblOtherCharges.Text =
                "PHP " +
                selectedBilling.OtherCharges.ToString("N2");

            // TOTAL BILL
            lblTotalBill.Text =
                "PHP " +
                selectedBilling.TotalBill.ToString("N2");

            // PAYMENT AMOUNT
            SetOutput(
                selectedBilling.TotalBill.ToString("N2"),
                "lblPaymentAmount",
                "textBox3");

            if (confirm != null)
            {
                confirm.Enabled =
                    selectedBilling.ReservationId > 0 &&
                    !PaymentCompleted;
            }
        }

        // =========================================
        // CONFIRM PAYMENT AND CHECK-OUT
        // =========================================

        private void btnConfirmPayment_Click(
            object sender, EventArgs e)
        {
            if (PaymentCompleted)
                return;

            if (selectedBilling == null ||
                selectedBilling.ReservationId <= 0)
            {
                MessageBox.Show(
                    "Select a checked-in guest in Check-Out first.",
                    "No Guest",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            ComboBox method = FindControl<ComboBox>(
                "cmbPaymentMethod", "cmbRoomType");

            if (method == null || method.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Select Cash, GCash, or Card.",
                    "Payment Method",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            DialogResult answer = MessageBox.Show(
                "Confirm that payment has been received?\n\n" +
                "Guest: " + selectedBilling.GuestName + "\n" +
                "Room: " + selectedBilling.RoomNumber + "\n" +
                "Amount: PHP " +
                    selectedBilling.TotalBill.ToString("N2") + "\n" +
                "Method: " + method.Text,
                "Confirm Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            Button confirm = FindControl<Button>(
                "btnConfirmPayment", "button2");

            try
            {
                if (confirm != null)
                    confirm.Enabled = false;

                // Save the payment and update the reservation
                // to Checked Out in one SQL transaction.
                FrontDeskOperationsService operations =
                    new FrontDeskOperationsService();

                operations.CompleteCheckout(
                    selectedBilling,
                    method.Text);

                // Report success only after SQL completes.
                PaymentCompleted = true;

                selectedBilling.ReservationStatus = "Checked Out";

                MessageBox.Show(
                    "Payment recorded in the database.\n" +
                    "Guest checked out successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Return to the Front Desk Dashboard if this
                // is running as an MDI child.
                FrontDeskDashboard dashboard =
                    MdiParent as FrontDeskDashboard;

                if (dashboard != null)
                {
                    dashboard.ShowDashboard();
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Payment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed &&
                    confirm != null &&
                    !PaymentCompleted)
                {
                    confirm.Enabled = true;
                }
            }
        }

        // =========================================
        // FIND CONTROLS FROM EITHER DESIGNER VERSION
        // =========================================

        private T FindControl<T>(params string[] names)
            where T : Control
        {
            foreach (string name in names)
            {
                Control[] matches = Controls.Find(name, true);

                foreach (Control match in matches)
                {
                    T typed = match as T;

                    if (typed != null)
                        return typed;
                }
            }

            return null;
        }

        private void SetOutput(
            string value,
            params string[] controlNames)
        {
            Control control =
                FindControl<Control>(controlNames);

            if (control != null)
                control.Text = value;
        }
    }
}
