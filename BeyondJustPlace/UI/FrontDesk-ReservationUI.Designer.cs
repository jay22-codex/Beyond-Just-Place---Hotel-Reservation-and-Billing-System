using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class FrontDesk_ReservationUI
    {
        private System.ComponentModel.IContainer components = null;

        private Panel header;
        private Panel searchBar;
        private Panel actionBar;
        private SplitContainer split;
        private GroupBox bookings;
        private GroupBox details;
        private DataGridView dgvReservations;
        private TextBox txtSearch;
        private TextBox txtIdNumber;
        private RichTextBox txtDetails;
        private CheckBox chkChecked;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnBrowse;
        private Button btnVerify;
        private Button btnVerifyDeposit;
        private Button btnConfirm;
        private Button btnCancelBooking;
        private Label lblCount;
        private Label lblStatus;
        private Label title;
        private Label searchText;
        private Label idText;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.header = new System.Windows.Forms.Panel();
            this.title = new System.Windows.Forms.Label();
            this.searchBar = new System.Windows.Forms.Panel();
            this.searchText = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.split = new System.Windows.Forms.SplitContainer();
            this.bookings = new System.Windows.Forms.GroupBox();
            this.dgvReservations = new System.Windows.Forms.DataGridView();
            this.details = new System.Windows.Forms.GroupBox();
            this.txtDetails = new System.Windows.Forms.RichTextBox();
            this.actionBar = new System.Windows.Forms.Panel();
            this.idText = new System.Windows.Forms.Label();
            this.txtIdNumber = new System.Windows.Forms.TextBox();
            this.chkChecked = new System.Windows.Forms.CheckBox();
            this.btnVerify = new System.Windows.Forms.Button();
            this.btnVerifyDeposit = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancelBooking = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();

            this.header.SuspendLayout();
            this.searchBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.bookings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservations)).BeginInit();
            this.details.SuspendLayout();
            this.actionBar.SuspendLayout();
            this.SuspendLayout();

            // header
            this.header.Name = "header";
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Height = 82;
            this.header.BackColor = System.Drawing.Color.FromArgb(76, 61, 135);
            this.header.Controls.Add(this.title);

            // title
            this.title.Name = "title";
            this.title.Text = "RESERVATION RECORDS  /  GUEST ID REVIEW";
            this.title.ForeColor = System.Drawing.Color.White;
            this.title.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.title.AutoSize = false;
            this.title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.title.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);

            // searchBar
            this.searchBar.Name = "searchBar";
            this.searchBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchBar.Height = 77;
            this.searchBar.BackColor = System.Drawing.Color.White;
            this.searchBar.Padding = new System.Windows.Forms.Padding(19);
            this.searchBar.Controls.Add(this.searchText);
            this.searchBar.Controls.Add(this.txtSearch);
            this.searchBar.Controls.Add(this.btnSearch);
            this.searchBar.Controls.Add(this.btnRefresh);
            this.searchBar.Controls.Add(this.btnBrowse);

            // searchText
            this.searchText.Name = "searchText";
            this.searchText.Text = "Booking # / guest name / contact / room";
            this.searchText.AutoSize = false;
            this.searchText.Location = new System.Drawing.Point(19, 12);
            this.searchText.Size = new System.Drawing.Size(370, 20);
            this.searchText.Font = new System.Drawing.Font("Arial", 9F);

            // txtSearch
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Location = new System.Drawing.Point(19, 36);
            this.txtSearch.Size = new System.Drawing.Size(510, 27);
            this.txtSearch.Font = new System.Drawing.Font("Arial", 11F);
            this.txtSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left;
            this.txtSearch.TabIndex = 0;

            // btnSearch
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Text = "Search";
            this.btnSearch.Location = new System.Drawing.Point(550, 31);
            this.btnSearch.Size = new System.Drawing.Size(115, 36);
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnSearch.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSearch.TabIndex = 1;

            // btnRefresh
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Location = new System.Drawing.Point(675, 31);
            this.btnRefresh.Size = new System.Drawing.Size(115, 36);
            this.btnRefresh.BackColor = System.Drawing.Color.Gainsboro;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnRefresh.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.TabIndex = 2;

            // btnBrowse
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Text = "Browse Rooms / New Booking";
            this.btnBrowse.Location = new System.Drawing.Point(810, 31);
            this.btnBrowse.Size = new System.Drawing.Size(270, 36);
            this.btnBrowse.BackColor = System.Drawing.Color.Gainsboro;
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnBrowse.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBrowse.TabIndex = 3;

            // split
            this.split.Name = "split";
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.split.SplitterWidth = 7;
            this.split.Size = new System.Drawing.Size(1600, 714);
            this.split.BackColor = System.Drawing.Color.FromArgb(240, 240, 244);
            this.split.SplitterDistance = 375;
            this.split.Panel1.Padding = new System.Windows.Forms.Padding(18, 12, 18, 7);
            this.split.Panel2.Padding = new System.Windows.Forms.Padding(18, 7, 18, 16);
            this.split.Panel1.Controls.Add(this.bookings);
            this.split.Panel2.Controls.Add(this.details);

            // bookings
            this.bookings.Name = "bookings";
            this.bookings.Text = "Existing Reservations - No room-type/availability selection on this screen";
            this.bookings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookings.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.bookings.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.bookings.Padding = new System.Windows.Forms.Padding(12);
            this.bookings.Controls.Add(this.dgvReservations);

            // dgvReservations
            this.dgvReservations.Name = "dgvReservations";
            this.dgvReservations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReservations.ReadOnly = true;
            this.dgvReservations.AllowUserToAddRows = false;
            this.dgvReservations.AllowUserToDeleteRows = false;
            this.dgvReservations.MultiSelect = false;
            this.dgvReservations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReservations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReservations.BackgroundColor = System.Drawing.Color.White;
            this.dgvReservations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgvReservations.RowHeadersVisible = false;
            this.dgvReservations.RowTemplate.Height = 30;
            this.dgvReservations.ColumnHeadersHeight = 35;
            this.dgvReservations.TabIndex = 4;

            // details
            this.details.Name = "details";
            this.details.Text = "Selected Guest & Booking Information";
            this.details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.details.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.details.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.details.Controls.Add(this.txtDetails);
            this.details.Controls.Add(this.actionBar);

            // txtDetails
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetails.ReadOnly = true;
            this.txtDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetails.Font = new System.Drawing.Font("Consolas", 10.2F);
            this.txtDetails.BackColor = System.Drawing.Color.White;
            this.txtDetails.TabIndex = 5;

            // actionBar
            this.actionBar.Name = "actionBar";
            this.actionBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.actionBar.Height = 121;
            this.actionBar.AutoScroll = true;
            this.actionBar.BackColor = System.Drawing.Color.FromArgb(238, 235, 251);
            this.actionBar.Controls.Add(this.idText);
            this.actionBar.Controls.Add(this.txtIdNumber);
            this.actionBar.Controls.Add(this.chkChecked);
            this.actionBar.Controls.Add(this.btnVerify);
            this.actionBar.Controls.Add(this.btnVerifyDeposit);
            this.actionBar.Controls.Add(this.btnConfirm);
            this.actionBar.Controls.Add(this.btnCancelBooking);
            this.actionBar.Controls.Add(this.lblStatus);

            // idText
            this.idText.Name = "idText";
            this.idText.Text = "Physical ID number:";
            this.idText.AutoSize = true;
            this.idText.Location = new System.Drawing.Point(18, 12);
            this.idText.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);

            // txtIdNumber
            this.txtIdNumber.Name = "txtIdNumber";
            this.txtIdNumber.Location = new System.Drawing.Point(175, 9);
            this.txtIdNumber.Size = new System.Drawing.Size(255, 29);
            this.txtIdNumber.Font = new System.Drawing.Font("Arial", 11F);
            this.txtIdNumber.TabIndex = 6;

            // chkChecked
            this.chkChecked.Name = "chkChecked";
            this.chkChecked.Text = "I checked the guest's physical ID";
            this.chkChecked.Location = new System.Drawing.Point(445, 12);
            this.chkChecked.AutoSize = true;
            this.chkChecked.Font = new System.Drawing.Font("Arial", 9.5F);
            this.chkChecked.TabIndex = 7;

            // btnVerify
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Text = "Verify Guest ID";
            this.btnVerify.Location = new System.Drawing.Point(765, 6);
            this.btnVerify.Size = new System.Drawing.Size(140, 38);
            this.btnVerify.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.btnVerify.UseVisualStyleBackColor = false;
            this.btnVerify.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnVerify.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnVerify.TabIndex = 8;

            // btnVerifyDeposit
            this.btnVerifyDeposit.Name = "btnVerifyDeposit";
            this.btnVerifyDeposit.Text = "Verify Reservation Fee";
            this.btnVerifyDeposit.Location = new System.Drawing.Point(20, 62);
            this.btnVerifyDeposit.Size = new System.Drawing.Size(206, 38);
            this.btnVerifyDeposit.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.btnVerifyDeposit.UseVisualStyleBackColor = false;
            this.btnVerifyDeposit.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnVerifyDeposit.TabIndex = 11;

            // btnConfirm
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Text = "Confirm Pending";
            this.btnConfirm.Location = new System.Drawing.Point(919, 6);
            this.btnConfirm.Size = new System.Drawing.Size(170, 38);
            this.btnConfirm.BackColor = System.Drawing.Color.Gainsboro;
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnConfirm.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.TabIndex = 9;

            // btnCancelBooking
            this.btnCancelBooking.Name = "btnCancelBooking";
            this.btnCancelBooking.Text = "Cancel Booking";
            this.btnCancelBooking.Location = new System.Drawing.Point(1098, 6);
            this.btnCancelBooking.Size = new System.Drawing.Size(170, 38);
            this.btnCancelBooking.BackColor = System.Drawing.Color.Gainsboro;
            this.btnCancelBooking.UseVisualStyleBackColor = false;
            this.btnCancelBooking.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.btnCancelBooking.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCancelBooking.TabIndex = 10;

            // lblStatus
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Select a reservation to review guest details and verify the physical ID.";
            this.lblStatus.Location = new System.Drawing.Point(242, 66);
            this.lblStatus.Size = new System.Drawing.Size(1010, 35);
            this.lblStatus.Font = new System.Drawing.Font("Arial", 10F);
            this.lblStatus.AutoEllipsis = true;

            // lblCount
            this.lblCount.Name = "lblCount";
            this.lblCount.Text = "Reservations";
            this.lblCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCount.Height = 27;
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(76, 61, 135);
            this.lblCount.BackColor = System.Drawing.Color.FromArgb(240, 240, 244);
            this.lblCount.Padding = new System.Windows.Forms.Padding(23, 4, 0, 0);

            // FrontDesk_ReservationUI
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.MinimumSize = new System.Drawing.Size(930, 680);
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 244);
            this.Text = "Beyond Just Place - Reservations";
            this.Name = "FrontDesk_ReservationUI";
            this.Controls.Add(this.split);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.searchBar);
            this.Controls.Add(this.header);

            this.header.ResumeLayout(false);
            this.searchBar.ResumeLayout(false);
            this.searchBar.PerformLayout();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.bookings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReservations)).EndInit();
            this.details.ResumeLayout(false);
            this.actionBar.ResumeLayout(false);
            this.actionBar.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
