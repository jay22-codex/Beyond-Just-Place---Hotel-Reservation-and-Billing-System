using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class GuestRoomAvailabilityForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header;
        private Panel searchPanel;
        private Panel selectedFooter;
        private Panel selectedHeader;
        private FlowLayoutPanel availableList;
        private FlowLayoutPanel selectedList;
        private SplitContainer mainSplit;
        private DateTimePicker dtpArrival;
        private DateTimePicker dtpDeparture;
        private NumericUpDown numAdults;
        private NumericUpDown numChildren;
        private NumericUpDown numRooms;
        private Button btnSearch;
        private Button btnContinue;
        private Button btnClose;
        private Label lblResults;
        private Label lblSelectionStatus;
        private Label lblTotal;
        private Label lblCapacity;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.header = new Panel();
            this.searchPanel = new Panel();
            this.mainSplit = new SplitContainer();
            this.availableList = new FlowLayoutPanel();
            this.selectedList = new FlowLayoutPanel();
            this.selectedFooter = new Panel();
            this.selectedHeader = new Panel();
            this.dtpArrival = new DateTimePicker();
            this.dtpDeparture = new DateTimePicker();
            this.numAdults = new NumericUpDown();
            this.numChildren = new NumericUpDown();
            this.numRooms = new NumericUpDown();
            this.btnSearch = GuestBookingLook.Button("Check availability", true);
            this.btnContinue = GuestBookingLook.Button("Continue to guest details  →", true);
            this.btnClose = GuestBookingLook.Button("←  Staff login");
            this.lblResults = GuestBookingLook.Label("Available room options", 15, true);
            this.lblSelectionStatus = GuestBookingLook.Label("Selected rooms (0)", 14, true);
            this.lblTotal = GuestBookingLook.Label("Estimated total: ₱0.00", 16, true);
            this.lblCapacity = GuestBookingLook.Label("Select your rooms to continue", 9);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAdults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChildren)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRooms)).BeginInit();
            this.SuspendLayout();

            this.Name = "GuestRoomAvailabilityForm";
            this.Text = "Beyond Just Place - Browse Rooms";
            this.BackColor = GuestBookingLook.Canvas;
            this.Font = new Font("Segoe UI", 10F);
            this.ClientSize = new Size(1460, 880);
            this.MinimumSize = new Size(1260, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            this.header.Name = "header";
            this.header.Height = 91;
            this.header.BackColor = GuestBookingLook.DarkPurple;
            this.header.Dock = DockStyle.Top;
            Label brand = GuestBookingLook.Label("BEYOND JUST PLACE", 23, true);
            brand.ForeColor = Color.White;
            brand.Location = new Point(35, 11);
            brand.Size = new Size(640, 43);
            Label sub = GuestBookingLook.Label("HOTEL ROOMS  /  AVAILABILITY  /  RESERVATIONS", 10);
            sub.ForeColor = Color.FromArgb(233, 228, 249);
            sub.Location = new Point(39, 55);
            sub.Size = new Size(650, 24);
            this.btnClose.Location = new Point(1160, 23);
            this.btnClose.Size = new Size(220, 43);
            this.btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.header.Controls.AddRange(new Control[] { brand, sub, btnClose });

            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Height = 151;
            this.searchPanel.BackColor = Color.White;
            this.searchPanel.Dock = DockStyle.Top;
            this.searchPanel.Padding = new Padding(22, 10, 22, 10);
            Label caption = GuestBookingLook.Label("Find the right room for your stay", 17, true);
            caption.SetBounds(34, 9, 760, 37);
            this.searchPanel.Controls.Add(caption);
            AddSearchLabel("Check-in", 34, 54);
            AddSearchLabel("Check-out", 258, 54);
            AddSearchLabel("Adults", 482, 54);
            AddSearchLabel("Children", 620, 54);
            AddSearchLabel("Rooms wanted", 758, 54);
            dtpArrival.Name = "dtpArrival";
            dtpDeparture.Name = "dtpDeparture";
            dtpArrival.Format = DateTimePickerFormat.Custom;
            dtpDeparture.Format = DateTimePickerFormat.Custom;
            dtpArrival.CustomFormat = "MMM dd, yyyy";
            dtpDeparture.CustomFormat = "MMM dd, yyyy";
            dtpArrival.SetBounds(34, 82, 203, 37);
            dtpDeparture.SetBounds(258, 82, 203, 37);
            dtpArrival.Font = new Font("Segoe UI", 11);
            dtpDeparture.Font = new Font("Segoe UI", 11);
            numAdults.Name = "numAdults";
            numAdults.Minimum = 1; numAdults.Maximum = 50; numAdults.Value = 2;
            numChildren.Name = "numChildren";
            numChildren.Minimum = 0; numChildren.Maximum = 50;
            numRooms.Name = "numRooms";
            numRooms.Minimum = 1; numRooms.Maximum = 10; numRooms.Value = 1;
            SetNumber(numAdults, 482, 82);
            SetNumber(numChildren, 620, 82);
            SetNumber(numRooms, 758, 82);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.SetBounds(942, 80, 236, 43);
            this.searchPanel.Controls.AddRange(new Control[] {
                dtpArrival, dtpDeparture, numAdults, numChildren, numRooms, btnSearch
            });

            this.mainSplit.Name = "mainSplit";
            this.mainSplit.Dock = DockStyle.Fill;
            this.mainSplit.BackColor = GuestBookingLook.Canvas;
            this.mainSplit.Size = new Size(1460, 637);
            this.mainSplit.SplitterWidth = 8;
            this.mainSplit.SplitterDistance = 975;
            this.mainSplit.Panel1.Padding = new Padding(21, 19, 9, 14);
            this.mainSplit.Panel2.Padding = new Padding(9, 19, 21, 14);
            this.mainSplit.Panel1.BackColor = GuestBookingLook.Canvas;
            this.mainSplit.Panel2.BackColor = GuestBookingLook.Canvas;

            this.lblResults.Dock = DockStyle.Top;
            this.lblResults.Height = 41;
            this.lblResults.Name = "lblResults";
            this.availableList.Name = "availableList";
            this.availableList.Dock = DockStyle.Fill;
            this.availableList.AutoScroll = true;
            this.availableList.WrapContents = false;
            this.availableList.FlowDirection = FlowDirection.TopDown;
            this.availableList.BackColor = GuestBookingLook.Canvas;
            this.availableList.Padding = new Padding(0, 0, 14, 0);
            this.mainSplit.Panel1.Controls.Add(availableList);
            this.mainSplit.Panel1.Controls.Add(lblResults);

            this.selectedHeader.Name = "selectedHeader";
            this.selectedHeader.Dock = DockStyle.Top;
            this.selectedHeader.Height = 52;
            this.selectedHeader.BackColor = Color.White;
            this.lblSelectionStatus.Dock = DockStyle.Fill;
            this.lblSelectionStatus.Padding = new Padding(13, 0, 0, 0);
            this.selectedHeader.Controls.Add(lblSelectionStatus);

            this.selectedFooter.Name = "selectedFooter";
            this.selectedFooter.Dock = DockStyle.Bottom;
            this.selectedFooter.Height = 185;
            this.selectedFooter.BackColor = Color.White;
            this.lblTotal.Dock = DockStyle.Top;
            this.lblTotal.Height = 57;
            this.lblTotal.Padding = new Padding(14, 7, 0, 0);
            this.lblCapacity.Dock = DockStyle.Top;
            this.lblCapacity.Height = 41;
            this.lblCapacity.Padding = new Padding(14, 0, 0, 0);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Dock = DockStyle.Bottom;
            this.btnContinue.Height = 49;
            this.btnContinue.Enabled = false;
            this.selectedFooter.Padding = new Padding(14, 5, 14, 16);
            this.selectedFooter.Controls.Add(btnContinue);
            this.selectedFooter.Controls.Add(lblCapacity);
            this.selectedFooter.Controls.Add(lblTotal);

            this.selectedList.Name = "selectedList";
            this.selectedList.Dock = DockStyle.Fill;
            this.selectedList.AutoScroll = true;
            this.selectedList.WrapContents = false;
            this.selectedList.FlowDirection = FlowDirection.TopDown;
            this.selectedList.BackColor = Color.White;
            this.selectedList.Padding = new Padding(11, 15, 8, 10);
            this.mainSplit.Panel2.Controls.Add(selectedList);
            this.mainSplit.Panel2.Controls.Add(selectedFooter);
            this.mainSplit.Panel2.Controls.Add(selectedHeader);

            this.Controls.Add(mainSplit);
            this.Controls.Add(searchPanel);
            this.Controls.Add(header);
            ((System.ComponentModel.ISupportInitialize)(this.numAdults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChildren)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRooms)).EndInit();
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.ResumeLayout(false);
        }

        private void AddSearchLabel(string text, int x, int y)
        {
            Label label = GuestBookingLook.Label(text, 9, true);
            int width = (text == "Adults" || text == "Children") ? 115 : 194;
            label.SetBounds(x, y, width, 24);
            this.searchPanel.Controls.Add(label);
        }

        private static void SetNumber(NumericUpDown num, int x, int y)
        {
            num.Font = new Font("Segoe UI", 11);
            num.SetBounds(x, y, 114, 37);
        }
    }
}
