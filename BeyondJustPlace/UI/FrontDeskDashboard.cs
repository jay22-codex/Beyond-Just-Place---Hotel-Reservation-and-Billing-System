
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Model;

namespace UI
{
    public partial class FrontDeskDashboard : Form
    {
        private bool homeOpened = false;

        public FrontDeskDashboard()
        {
            InitializeComponent();

            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;

            ConnectNavigation();
        }

        private void ConnectNavigation()
        {
            ConnectButton(
                new string[] { "btnDashboard" },
                "Dashboard",
                btnDashboard_Click);

            ConnectButton(
                new string[] { "btnReservation" },
                "Reservation",
                btnReservation_Click);

            ConnectButton(
                new string[] {
                    "btnCheckInCheckOut",
                    "btnCheckInOut"
                },
                "Check-In/Check-Out",
                btnCheckInOut_Click);

            ConnectButton(
                new string[] { "btnBilling" },
                "Billing",
                btnBilling_Click);

            ConnectButton(
                new string[] {
                    "btnLogout",
                    "btnLogin"
                },
                "Logout",
                btnLogout_Click);
        }

        private void ConnectButton(
            string[] possibleNames,
            string buttonText,
            EventHandler handler)
        {
            Button button = null;

            foreach (string name in possibleNames)
            {
                Control[] results = Controls.Find(name, true);

                button = results.OfType<Button>().FirstOrDefault();

                if (button != null)
                    break;
            }

            if (button == null)
            {
                button = FindButtons(this)
                    .FirstOrDefault(b =>
                        Normalize(b.Text) ==
                        Normalize(buttonText));
            }

            if (button != null)
            {
                button.Click -= handler;
                button.Click += handler;
            }
        }

        private IEnumerable<Button> FindButtons(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                Button button = control as Button;

                if (button != null)
                    yield return button;

                foreach (Button child in FindButtons(control))
                    yield return child;
            }
        }

        private string Normalize(string value)
        {
            return new string(
                (value ?? "")
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }

        private void FrontDeskDashboard_Load(
            object sender,
            EventArgs e)
        {
            ShowHomeOnce();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ShowHomeOnce();
        }

        private void ShowHomeOnce()
        {
            if (homeOpened)
                return;

            homeOpened = true;
            ShowDashboard();
        }

        private void OpenChildForm(Form child)
        {
            foreach (Form existing in MdiChildren)
            {
                existing.Close();

                if (!existing.IsDisposed)
                {
                    child.Dispose();
                    return;
                }
            }

            child.MdiParent = this;
            child.FormBorderStyle = FormBorderStyle.None;

            child.Show();
            child.WindowState = FormWindowState.Maximized;

            Control[] panels =
                Controls.Find("panelSidebar", true);

            if (panels.Length > 0)
                panels[0].BringToFront();
        }

        public void ShowDashboard()
        {
            FrontDeskHome home =
                ActiveMdiChild as FrontDeskHome;

            if (home != null)
            {
                home.RefreshDashboardData();
                return;
            }

            OpenChildForm(new FrontDeskHome());
        }

        public void RefreshDashboardData()
        {
            foreach (Form form in MdiChildren)
            {
                FrontDeskHome home = form as FrontDeskHome;

                if (home != null)
                    home.RefreshDashboardData();
            }
        }

        public void OpenBilling(BillingInfo billing)
        {
            if (billing == null)
            {
                MessageBox.Show(
                    "Please select a checked-in guest first.");
                return;
            }

            OpenChildForm(
                new FrontDesk_Billing_PaymentUI(billing));
        }

        private void btnDashboard_Click(
            object sender,
            EventArgs e)
        {
            ShowDashboard();
        }

        private void btnReservation_Click(
            object sender,
            EventArgs e)
        {
            OpenChildForm(
                new FrontDesk_ReservationUI());
        }

        private void btnCheckInOut_Click(
            object sender,
            EventArgs e)
        {
            OpenChildForm(
                new FrontDesk_CheckIn_CheckOut());
        }

        private void btnBilling_Click(
            object sender,
            EventArgs e)
        {
            MessageBox.Show(
                "To process Billing, search for a checked-in " +
                "guest in Check-Out first.");

            OpenChildForm(
                new FrontDesk_CheckIn_CheckOut());
        }

        private void btnLogout_Click(
            object sender,
            EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Close();
            }
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            FrontDesk_Billing_PaymentUI payment =
                new FrontDesk_Billing_PaymentUI();
            payment.Show();
        }

        private void btnCheckinCheckout_Click(object sender, EventArgs e)
        {
            FrontDesk_CheckIn_CheckOut checkIn_CheckOut =
                new FrontDesk_CheckIn_CheckOut();

            checkIn_CheckOut.Show();
        }
    }
}
