using System;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    public partial class FrontDeskDashboard : Form
    {
        private ReservationService service = new ReservationService();
        private Timer refreshTimer = new Timer();

        public void RefreshDashboardData()
        {
            LoadDashboard(false);
        }
        public FrontDeskDashboard()
        {
            InitializeComponent();

            refreshTimer.Interval = 5000;
            refreshTimer.Tick += RefreshTimer_Tick;
        }

        private void FrontDeskDashboard_Load(object sender, EventArgs e)
        {
            LoadDashboard(true);
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadDashboard(false);
        }

        private void LoadDashboard(bool showError)
        {
            try
            {
                int todayReservation =
                    service.GetTodayReservationCount();

                int currentCheckIn =
                    service.GetCurrentCheckedInCount();

                int expectedCheckIn =
                    service.GetExpectedCheckInsTodayCount();

                int pendingReservation =
                    service.GetPendingReservationCount();

                SetNumber(this, "TodayReservation", todayReservation);

                SetNumber(this, "CurrentCheck-inGuest", currentCheckIn);

                SetNumber(this, "ExpectedCheck-insToday", expectedCheckIn);

                SetNumber(this, "PendingReservation", pendingReservation);

                listBox1.Items.Clear();

                var activities =
                    service.GetLatestActivity();

                foreach (string activity in activities)
                {
                    listBox1.Items.Add(activity);
                }
            }
            catch (Exception ex)
            {
                if (showError)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SetNumber(Control parent, string title, int value)
        {
            foreach (Control control in parent.Controls)
            {
                GroupBox groupBox = control as GroupBox;

                if (groupBox != null)
                {
                    string groupText =
                        groupBox.Text.Replace("\r", "")
                                     .Replace("\n", "")
                                     .Replace(" ", "")
                                     .ToLower();

                    string searchText =
                        title.Replace(" ", "")
                             .ToLower();

                    if (groupText.Contains(searchText))
                    {
                        foreach (Control item in groupBox.Controls)
                        {
                            Label label = item as Label;

                            if (label != null)
                            {
                                int oldNumber;

                                if (int.TryParse(label.Text, out oldNumber))
                                {
                                    label.Text = value.ToString();
                                    return;
                                }
                            }
                        }
                    }
                }

                if (control.HasChildren)
                {
                    SetNumber(control, title, value);
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            refreshTimer.Stop();

            Login login = new Login();
            login.Show();

            this.Close();
        }

        private void btnReservation_Click(object sender, EventArgs e)
        {
            FrontDesk_ReservationUI reservationUI =
                new FrontDesk_ReservationUI();

            reservationUI.Show();
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