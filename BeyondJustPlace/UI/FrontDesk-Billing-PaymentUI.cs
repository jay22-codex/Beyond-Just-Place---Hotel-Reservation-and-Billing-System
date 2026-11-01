using System;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_Billing_PaymentUI : Form
    {
        private readonly FrontDeskOperationsService operations = new FrontDeskOperationsService();
        private readonly BillingInfo selectedBilling;
        public bool PaymentCompleted { get; private set; }

        // Parameterless constructor is required for Visual Studio's WinForms Designer.
        public FrontDesk_Billing_PaymentUI() : this(null) { }

        public FrontDesk_Billing_PaymentUI(BillingInfo billing)
        {
            InitializeComponent();
            selectedBilling = billing;

            // Existing Designer names are used exactly as uploaded:
            // label19 = guest, label18 = room, label17 = stay,
            // cmbRoomType = payment method, textBox3 = payment amount,
            // button2 = Confirm Payment.
            groupBox3.Text = "Selected Guest";
            label23.Visible = false;
            textBox2.Visible = false;
            button1.Visible = false;
            cmbRoomType.Items.Clear();
            cmbRoomType.Items.AddRange(new object[] { "Cash", "GCash", "Card" });
            cmbRoomType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRoomType.SelectedIndex = -1;
            textBox3.ReadOnly = true;
            button2.Click += btnConfirmPayment_Click;
            Load += FrontDesk_Billing_PaymentUI_Load;
        }

        private void FrontDesk_Billing_PaymentUI_Load(object sender, EventArgs e)
        {
            if (selectedBilling == null)
            {
                label19.Text = "No guest selected";
                label18.Text = "Output";
                label17.Text = "Output";
                lblRoomCharges.Text = "₱0.00";
                lblOtherCharges.Text = "₱0.00";
                lblTotalBill.Text = "₱0.00";
                textBox3.Text = "0.00";
                button2.Enabled = false;
                return;
            }
            label19.Text = selectedBilling.GuestName;
            label18.Text = selectedBilling.RoomNumber;
            label17.Text = selectedBilling.StayDays + " night(s)";
            lblRoomCharges.Text = "₱" + selectedBilling.RoomCharges.ToString("N2");
            lblOtherCharges.Text = "₱" + selectedBilling.OtherCharges.ToString("N2");
            lblTotalBill.Text = "₱" + selectedBilling.TotalBill.ToString("N2");
            textBox3.Text = selectedBilling.TotalBill.ToString("N2");
        }

        private void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            if (selectedBilling == null)
            {
                MessageBox.Show("Go to Check-Out and select a checked-in guest first.");
                return;
            }
            if (cmbRoomType.SelectedIndex < 0)
            {
                MessageBox.Show("Select Cash, GCash, or Card.");
                return;
            }
            if (MessageBox.Show("Confirm payment of ₱" + selectedBilling.TotalBill.ToString("N2") +
                " using " + cmbRoomType.Text + "?", "Confirm Payment",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                button2.Enabled = false;
                operations.CompleteCheckout(selectedBilling, cmbRoomType.Text);
                PaymentCompleted = true;
                MessageBox.Show("Payment saved. Guest checked out successfully.");
                FrontDeskDashboard parent = MdiParent as FrontDeskDashboard;
                if (parent != null) parent.ShowDashboard();
                else Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Payment error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && !PaymentCompleted) button2.Enabled = true;
            }
        }
    }
}
