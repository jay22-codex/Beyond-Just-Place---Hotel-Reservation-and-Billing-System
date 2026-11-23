using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class GuestBookingDetailsForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header;
        private Panel footer;
        private SplitContainer split;
        private Panel roomSummary;
        private Panel guestPanel;
        private PictureBox photo;
        private Label lblRoomTitle;
        private Label lblSpecs;
        private Label lblDescription;
        private Label lblDates;
        private Label lblCost;
        private Label lblDeposit;
        private Label lblBeds;
        private ListBox lbRooms;
        private TextBox txtName;
        private TextBox txtContact;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private TextBox txtIdNumber;
        private TextBox txtRequest;
        private ComboBox cmbIdType;
        private CheckBox chkConsent;
        private Button btnBack;
        private Button btnSubmit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.header = new Panel();
            this.footer = new Panel();
            this.split = new SplitContainer();
            this.roomSummary = new Panel();
            this.guestPanel = new Panel();
            this.photo = new PictureBox();
            this.lblRoomTitle = GuestBookingLook.Label("Selected accommodation", 18, true);
            this.lblSpecs = GuestBookingLook.Label("", 10);
            this.lblDescription = GuestBookingLook.Label("", 10);
            this.lblDates = GuestBookingLook.Label("", 11, true);
            this.lblCost = GuestBookingLook.Label("", 16, true);
            this.lblDeposit = GuestBookingLook.Label("", 10);
            this.lblBeds = GuestBookingLook.Label("", 10);
            this.lbRooms = new ListBox();
            this.txtName = new TextBox();
            this.txtContact = new TextBox();
            this.txtEmail = new TextBox();
            this.txtAddress = new TextBox();
            this.txtIdNumber = new TextBox();
            this.txtRequest = new TextBox();
            this.cmbIdType = new ComboBox();
            this.chkConsent = new CheckBox();
            this.btnBack = GuestBookingLook.Button("←  Back to rooms");
            this.btnSubmit = GuestBookingLook.Button("Submit reservation  →", true);
            ((System.ComponentModel.ISupportInitialize)(this.photo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.SuspendLayout();

            this.Name = "GuestBookingDetailsForm";
            this.Text = "Beyond Just Place - Reservation Details";
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 10F);
            this.ClientSize = new Size(1460, 880);
            this.MinimumSize = new Size(1020, 700);
            this.BackColor = GuestBookingLook.Canvas;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterParent;

            header.Name = "header";
            header.Dock = DockStyle.Top;
            header.Height = 92;
            header.BackColor = GuestBookingLook.DarkPurple;
            Label brand = GuestBookingLook.Label("BEYOND JUST PLACE", 23, true);
            brand.ForeColor = Color.White;
            brand.SetBounds(36, 10, 660, 45);
            Label sub = GuestBookingLook.Label("STEP 2 OF 2  •  RESERVATION DETAILS", 10);
            sub.ForeColor = Color.FromArgb(233, 228, 249);
            sub.SetBounds(39, 56, 660, 26);
            btnBack.Name = "btnBack";
            btnBack.SetBounds(1160, 23, 220, 43);
            btnBack.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            header.Controls.AddRange(new Control[] { brand, sub, btnBack });

            footer.Name = "footer";
            footer.Dock = DockStyle.Bottom;
            footer.Height = 88;
            footer.BackColor = Color.White;
            Label note = GuestBookingLook.Label(
                "No payment is collected here. Your request will be Pending until confirmed by the hotel.", 10);
            note.SetBounds(35, 22, 880, 44);
            note.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            note.ForeColor = GuestBookingLook.Muted;
            btnSubmit.Name = "btnSubmit";
            btnSubmit.SetBounds(1125, 21, 280, 47);
            btnSubmit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            footer.Controls.AddRange(new Control[] { note, btnSubmit });
            footer.Resize += (s, e) =>
            {
                btnSubmit.Left = Math.Max(12, footer.ClientSize.Width - btnSubmit.Width - 32);
                note.Width = Math.Max(200, btnSubmit.Left - note.Left - 16);
            };

            split.Dock = DockStyle.Fill;
            split.Name = "split";
            split.Size = new Size(1460, 700);
            split.SplitterWidth = 8;
            split.SplitterDistance = 675;
            split.BackColor = GuestBookingLook.Canvas;
            split.Panel1.Padding = new Padding(25, 22, 8, 18);
            split.Panel2.Padding = new Padding(8, 22, 25, 18);
            roomSummary.Name = "roomSummary";
            roomSummary.Dock = DockStyle.Fill;
            roomSummary.BackColor = Color.White;
            roomSummary.AutoScroll = true;
            roomSummary.Padding = new Padding(20);
            roomSummary.AutoScrollMinSize = new Size(0, 775);
            guestPanel.Name = "guestPanel";
            guestPanel.Dock = DockStyle.Fill;
            guestPanel.BackColor = Color.White;
            guestPanel.AutoScroll = true;
            guestPanel.Padding = new Padding(20);
            guestPanel.AutoScrollMinSize = new Size(0, 810);
            split.Panel1.Controls.Add(roomSummary);
            split.Panel2.Controls.Add(guestPanel);

            lblRoomTitle.SetBounds(19, 12, 590, 43);
            lblRoomTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            photo.Name = "photo";
            photo.SetBounds(19, 60, 590, 265);
            photo.SizeMode = PictureBoxSizeMode.Zoom;
            photo.BackColor = GuestBookingLook.LightPurple;
            photo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblSpecs.SetBounds(20, 341, 590, 30);
            lblDescription.SetBounds(20, 377, 590, 57);
            lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDescription.ForeColor = GuestBookingLook.Muted;
            lblDates.SetBounds(20, 440, 590, 33);
            Label roomsLabel = GuestBookingLook.Label("Selected room numbers", 10, true);
            roomsLabel.SetBounds(20, 478, 570, 25);
            lbRooms.Name = "lbRooms";
            lbRooms.SetBounds(20, 508, 590, 103);
            lbRooms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lbRooms.BorderStyle = BorderStyle.FixedSingle;
            lbRooms.Font = new Font("Segoe UI", 10);
            lblCost.SetBounds(20, 620, 590, 42);
            lblDeposit.SetBounds(20, 662, 590, 35);
            lblDeposit.ForeColor = GuestBookingLook.Muted;
            lblBeds.SetBounds(20, 701, 590, 32);
            roomSummary.Controls.AddRange(new Control[] {
                lblRoomTitle, photo, lblSpecs, lblDescription, lblDates,
                roomsLabel, lbRooms, lblCost, lblDeposit, lblBeds
            });

            Label heading = GuestBookingLook.Label("Your guest information", 19, true);
            heading.SetBounds(24, 15, 590, 47);
            Label helper = GuestBookingLook.Label(
                "Enter the primary guest's details. Your room selection is already saved above.", 10);
            helper.SetBounds(24, 65, 590, 39);
            helper.ForeColor = GuestBookingLook.Muted;
            guestPanel.Controls.Add(heading);
            guestPanel.Controls.Add(helper);

            AddField("Full name *", txtName, 116);
            AddField("Contact number *", txtContact, 193);
            AddField("Email address *", txtEmail, 270);
            AddField("Home address", txtAddress, 347);
            AddField("ID type *", cmbIdType, 424);
            AddField("ID number *", txtIdNumber, 501);
            AddField("Special requests (optional)", txtRequest, 578);
            cmbIdType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIdType.Items.AddRange(new object[] {
                "National ID", "Passport", "Driver's License", "Student ID", "Other" });
            chkConsent.Name = "chkConsent";
            chkConsent.Text = "I understand this is a pending booking request until the hotel confirms it.";
            chkConsent.Font = new Font("Segoe UI", 9.3F);
            chkConsent.ForeColor = GuestBookingLook.Ink;
            chkConsent.AutoSize = false;
            chkConsent.SetBounds(25, 667, 560, 64);
            chkConsent.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            guestPanel.Controls.Add(chkConsent);

            this.Controls.Add(split);
            this.Controls.Add(footer);
            this.Controls.Add(header);
            this.header.Resize += (s, e) => btnBack.Left = header.ClientSize.Width - btnBack.Width - 35;
            this.split.SizeChanged += (s, e) =>
            {
                if (split.Width < 940) return;
                int target = (int)(split.Width * .48);
                try { if (split.SplitterDistance != target) split.SplitterDistance = target; }
                catch (InvalidOperationException) { }
            };
            this.guestPanel.Resize += (s, e) =>
            {
                int fieldWidth = Math.Max(290, guestPanel.ClientSize.Width - 63);
                foreach (Control control in guestPanel.Controls)
                {
                    if (control == txtName || control == txtContact || control == txtEmail ||
                        control == txtAddress || control == cmbIdType ||
                        control == txtIdNumber || control == txtRequest)
                        control.Width = fieldWidth;
                    else if (control is Label && control.Left == 25)
                        control.Width = fieldWidth;
                }
                chkConsent.Width = fieldWidth;
            };
            this.roomSummary.Resize += (s, e) =>
            {
                int w = Math.Max(280, roomSummary.ClientSize.Width - 45);
                photo.Width = w;
                lbRooms.Width = w;
                lblRoomTitle.Width = w;
                lblSpecs.Width = w;
                lblDescription.Width = w;
                lblDates.Width = w;
                lblCost.Width = w;
                lblDeposit.Width = w;
                lblBeds.Width = w;
            };
            ((System.ComponentModel.ISupportInitialize)(this.photo)).EndInit();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.ResumeLayout(false);
        }

        private void AddField(string caption, Control field, int y)
        {
            Label label = GuestBookingLook.Label(caption, 10, true);
            label.SetBounds(25, y, 560, 28);
            field.Name = field.Name.Length == 0 ? "bookingField" + y : field.Name;
            field.SetBounds(25, y + 28, 560, 35);
            field.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            field.Font = new Font("Segoe UI", 11);
            if (field is TextBox)
            {
                TextBox text = (TextBox)field;
                text.BorderStyle = BorderStyle.FixedSingle;
                text.MaxLength = caption.Contains("Special requests") ? 1000 :
                    caption.Contains("address") ? 500 :
                    caption.Contains("Contact") ? 50 :
                    caption.Contains("ID number") ? 100 : 254;
            }
            guestPanel.Controls.Add(label);
            guestPanel.Controls.Add(field);
        }
    }
}
