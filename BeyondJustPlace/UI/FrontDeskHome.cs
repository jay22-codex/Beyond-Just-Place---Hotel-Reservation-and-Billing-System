using System;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    public partial class FrontDeskHome : Form
    {
        private readonly ReservationService service = new ReservationService();
        private readonly Timer refreshTimer = new Timer();

        public FrontDeskHome()
        {
            InitializeComponent();
            refreshTimer.Interval = 5000;
            refreshTimer.Tick += (sender, e) => LoadDashboard(false);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadDashboard(true);
            refreshTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            refreshTimer.Stop();
            refreshTimer.Dispose();
            base.OnFormClosed(e);
        }

        public void RefreshDashboardData()
        {
            LoadDashboard(false);
        }

        private void LoadDashboard(bool showError)
        {
            try
            {
                label3.Text = service.GetTodayReservationCount().ToString();
                label4.Text = service.GetCurrentCheckedInCount().ToString();
                label5.Text = service.GetExpectedCheckInsTodayCount().ToString();
                label6.Text = service.GetPendingReservationCount().ToString();

                listBox1.BeginUpdate();
                try
                {
                    listBox1.Items.Clear();
                    foreach (string activity in service.GetLatestActivity())
                        listBox1.Items.Add(activity);
                }
                finally
                {
                    listBox1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                if (showError)
                    MessageBox.Show("Unable to load dashboard data:\n" + ex.Message,
                        "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}