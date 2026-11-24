using System;
using System.Data;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    public partial class FrontDeskHome : Form
    {
        private readonly ReservationService counts = new ReservationService();
        private readonly ReservationOverviewService overview = new ReservationOverviewService();
        private readonly Timer refreshTimer = new Timer();
        private bool refreshing;

        public FrontDeskHome()
        {
            InitializeComponent();
            refreshTimer.Interval=5000;
            refreshTimer.Tick += (s,e) => RefreshDashboardData();
            txtDashboardSearch.KeyDown += (s,e) => { if(e.KeyCode==Keys.Enter) {e.SuppressKeyPress=true; RefreshDashboardData();} };
            Resize += (s,e) => ArrangeCards();
            ArrangeCards();
            ConfigureTable();
            UiPolish.Apply(this);
        }
        protected override void OnShown(EventArgs e)
        {base.OnShown(e);LoadDashboard(true);refreshTimer.Start();}
        protected override void OnFormClosed(FormClosedEventArgs e)
        {refreshTimer.Stop();refreshTimer.Dispose();base.OnFormClosed(e);}
        public void RefreshDashboardData(){LoadDashboard(false);}
        private void ArrangeCards()
        {
            int space=18, left=26, available=Math.Max(700,ClientSize.Width-52);
            int cardWidth=(available-3*space)/4;
            GroupBox[] cards={groupBoxTotalReservation,groupBox1,groupBox2,groupBox3};
            for(int i=0;i<cards.Length;i++)
            {cards[i].Left=left+i*(cardWidth+space);cards[i].Top=82;cards[i].Width=cardWidth;
             label3.Width=Math.Max(80,groupBoxTotalReservation.Width-45);
             label4.Width=Math.Max(80,groupBox1.Width-45);
             label5.Width=Math.Max(80,groupBox2.Width-45);
             label6.Width=Math.Max(80,groupBox3.Width-45);}
            groupBox4.Left=26;groupBox4.Top=280;groupBox4.Width=Math.Max(700,ClientSize.Width-52);
            groupBox4.Height=Math.Max(260,ClientSize.Height-305);
        }
        private void ConfigureTable()
        {
            dgvRecentReservations.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            dgvRecentReservations.AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.None;
            dgvRecentReservations.RowHeadersVisible=false;
            dgvRecentReservations.RowTemplate.Height=31;
            dgvRecentReservations.ColumnHeadersHeight=38;
            dgvRecentReservations.AllowUserToResizeRows=false;
            dgvRecentReservations.BackgroundColor=System.Drawing.Color.White;
            dgvRecentReservations.AlternatingRowsDefaultCellStyle.BackColor=
                System.Drawing.Color.FromArgb(246,245,252);
            dgvRecentReservations.DefaultCellStyle.WrapMode=DataGridViewTriState.False;
        }

        private void LoadDashboard(bool showError)
        {
            if(refreshing || IsDisposed)return;
            refreshing=true;
            try
            {
                label3.Text=counts.GetTodayReservationCount().ToString();
                label4.Text=counts.GetCurrentCheckedInCount().ToString();
                label5.Text=counts.GetExpectedCheckInsTodayCount().ToString();
                label6.Text=counts.GetPendingReservationCount().ToString();
                DataTable table=overview.GetReservations(txtDashboardSearch.Text);
                dgvRecentReservations.DataSource=table;
                string[] columns={"ReservationId","GuestName","ContactNumber","Email","RoomNumbers",
                    "RoomCount","BedCount","NumberOfGuests","CheckInDate","CheckOutDate","ReservationStatus","DepositStatus","IdVerified"};
                foreach(DataGridViewColumn c in dgvRecentReservations.Columns)
                    c.Visible=Array.IndexOf(columns,c.Name)>=0;
                SetHeader("ReservationId","Booking #");SetHeader("GuestName","Guest name");
                SetHeader("ContactNumber","Contact");SetHeader("RoomNumbers","Room(s)");
                SetHeader("RoomCount","Room qty");SetHeader("BedCount","Beds");SetHeader("NumberOfGuests","Guests");
                SetHeader("CheckInDate","Check-in");SetHeader("CheckOutDate","Check-out");
                SetHeader("ReservationStatus","Status");SetHeader("DepositStatus","Deposit");
                SetHeader("IdVerified","ID verified");
                FitDashboardColumns();
            }
            catch(Exception ex)
            {if(showError) MessageBox.Show("Unable to load dashboard data:\n"+ex.Message,
                "Database error",MessageBoxButtons.OK,MessageBoxIcon.Error);}
            finally{refreshing=false;}
        }
        private void FitDashboardColumns()
        {
            // Compact, readable overview; full details stay in Reservation/Check-In.
            foreach (DataGridViewColumn column in dgvRecentReservations.Columns)
            {
                if (!column.Visible) continue;
                column.MinimumWidth = 65;
                column.FillWeight = 75;
            }
            SetWeight("GuestName", 145, 120);
            SetWeight("RoomNumbers", 110, 90);
            SetWeight("ContactNumber", 110, 95);
            SetWeight("Email", 125, 120);
            SetWeight("ReservationStatus", 85, 86);
            SetWeight("CheckInDate", 90, 93);
            SetWeight("CheckOutDate", 90, 93);
        }
        private void SetWeight(string name, float weight, int minWidth)
        {
            if (dgvRecentReservations.Columns.Contains(name))
            {
                dgvRecentReservations.Columns[name].FillWeight = weight;
                dgvRecentReservations.Columns[name].MinimumWidth = minWidth;
            }
        }

        private void SetHeader(string name,string text)
        {if(dgvRecentReservations.Columns.Contains(name))dgvRecentReservations.Columns[name].HeaderText=text;}
    }
}
