
using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class WalkInCheckInForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header, bottom;
        private SplitContainer split;
        private DateTimePicker dtpIn, dtpOut;
        private NumericUpDown numAdults, numChildren, numRoomCount;
        private ComboBox cmbRoomType, cmbIdType, cmbPayment;
        private DataGridView dgvRooms;
        private TextBox txtName, txtContact, txtEmail, txtAddress, txtIdNumber, txtNotes;
        private CheckBox chkPhysicalId;
        private Button btnCheck, btnCheckIn, btnClose;
        private Label lblSelection;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Color purple = Color.FromArgb(128, 128, 255);
            Color dark = Color.FromArgb(76, 61, 135);

            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 79,
                BackColor = dark
            };

            Label title = new Label
            {
                Text = "WALK-IN GUEST  /  REGISTRATION & CHECK-IN",
                Font = new Font("Arial", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                Padding = new Padding(23, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            header.Controls.Add(title);

            bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 76,
                BackColor = Color.White
            };

            btnCheckIn = Button(
                "Complete Walk-In Check-In",
                0, 15, 258, 44, purple);

            btnClose = Button(
                "Cancel",
                0, 15, 150, 44, Color.Gainsboro);

            btnCheckIn.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            bottom.Controls.AddRange(
                new Control[] { btnClose, btnCheckIn });

            bottom.Resize += (s, e) =>
            {
                btnCheckIn.Left = bottom.ClientSize.Width - 290;
                btnClose.Left = btnCheckIn.Left - 166;
            };

            split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterWidth = 8,
                Size = new Size(1300, 670),
                BackColor = Color.FromArgb(241, 241, 246)
            };

            split.SplitterDistance = 650;
            split.Panel1.Padding = new Padding(17);
            split.Panel2.Padding = new Padding(17);

            GroupBox roomPanel = new GroupBox
            {
                Text = "1. Available Rooms for Walk-In",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Font = new Font("Arial", 11F, FontStyle.Bold)
            };

            split.Panel1.Controls.Add(roomPanel);

            Panel inputArea = new Panel
            {
                Dock = DockStyle.Top,
                Height = 178,
                BackColor = Color.White
            };

            Label schedule = Info(
                "Check-in (2:00 PM)", 18, 25, 192);

            dtpIn = new DateTimePicker
            {
                Location = new Point(17, 50),
                Width = 212,
                Format = DateTimePickerFormat.Short
            };

            Label endLabel = Info(
                "Check-out (12:00 PM)", 247, 25, 192);

            dtpOut = new DateTimePicker
            {
                Location = new Point(247, 50),
                Width = 212,
                Format = DateTimePickerFormat.Short
            };

            Label rt = Info("Room type", 17, 90, 150);

            cmbRoomType = new ComboBox
            {
                Name = "cmbRoomType",
                Location = new Point(17, 114),
                Width = 215,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label q = Info("Room count", 250, 90, 120);

            numRoomCount = Numeric(250, 114, 10, 1);

            Label a = Info("Adults", 373, 90, 100);

            numAdults = Numeric(373, 114, 50, 1);

            Label kids = Info("Children", 485, 90, 120);

            numChildren = Numeric(485, 114, 50, 0);

            btnCheck = Button(
                "Check Available Rooms",
                16, 147, 210, 30, purple);

            inputArea.Controls.AddRange(
                new Control[]
                {
                    schedule,
                    dtpIn,
                    endLabel,
                    dtpOut,
                    rt,
                    cmbRoomType,
                    q,
                    numRoomCount,
                    a,
                    numAdults,
                    kids,
                    numChildren,
                    btnCheck
                });

            lblSelection = new Label
            {
                Name = "lblSelection",
                Dock = DockStyle.Bottom,
                Height = 77,
                Text = "Hold Ctrl to select multiple room numbers.",
                Font = new Font("Arial", 10F, FontStyle.Bold),
                Padding = new Padding(15, 10, 15, 0),
                BackColor = Color.FromArgb(237, 234, 251)
            };

            dgvRooms = new DataGridView
            {
                Name = "dgvRooms",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                MultiSelect = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            dgvRooms.Columns.Add("RoomNumber", "Room");
            dgvRooms.Columns.Add("Capacity", "Capacity");
            dgvRooms.Columns.Add("BedCount", "Beds");
            dgvRooms.Columns.Add("Rate", "Rate/night");
            dgvRooms.Columns.Add("Description", "Description");

            roomPanel.Controls.Add(dgvRooms);
            roomPanel.Controls.Add(lblSelection);
            roomPanel.Controls.Add(inputArea);

            GroupBox details = new GroupBox
            {
                Text = "2. Walk-In Guest Identity & Payment",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Font = new Font("Arial", 11F, FontStyle.Bold)
            };

            split.Panel2.Controls.Add(details);

            Panel fields = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                AutoScrollMinSize = new Size(0, 780)
            };

            details.Controls.Add(fields);

            txtName = TextField(
                fields, "Full name *", 20, 25);

            txtContact = TextField(
                fields, "Contact number *", 20, 102);

            txtEmail = TextField(
                fields, "Email *", 20, 179);

            txtAddress = TextField(
                fields, "Address", 20, 256);

            InfoAdd(fields, "ID type *", 20, 333);

            cmbIdType = new ComboBox
            {
                Location = new Point(20, 362),
                Width = 455,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            fields.Controls.Add(cmbIdType);

            txtIdNumber = TextField(
                fields, "Physical ID number *", 20, 407);

            chkPhysicalId = new CheckBox
            {
                Text = "I physically checked the guest's ID and the number matches.",
                Location = new Point(20, 481),
                Width = 530,
                Height = 43,
                Font = new Font("Arial", 9.5F)
            };

            fields.Controls.Add(chkPhysicalId);

            InfoAdd(
                fields,
                "Deposit payment method (if fee applies)",
                20, 536);

            cmbPayment = new ComboBox
            {
                Location = new Point(20, 565),
                Width = 455,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            fields.Controls.Add(cmbPayment);

            txtNotes = TextField(
                fields, "Special requests (optional)", 20, 616);

            Label note = new Label
            {
                Text =
                    "GCash deposits display the hotel's actual QR, then request the transaction reference.\r\n" +
                    "The receptionist must verify the payment and ID before issuing room keys.",
                Location = new Point(20, 701),
                Size = new Size(500, 61),
                Font = new Font("Arial", 9F),
                ForeColor = dark
            };

            fields.Controls.Add(note);

            fields.Resize += (s, e) =>
            {
                int width = fields.ClientSize.Width - 51;

                foreach (Control c in fields.Controls)
                {
                    if (c is TextBox || c is ComboBox)
                        c.Width = width;
                }

                note.Width = width;
            };

            BackColor = Color.FromArgb(241, 241, 246);

            ClientSize = new Size(1320, 850);
            MinimumSize = new Size(1080, 760);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Dpi;

            Text = "Beyond Just Place - Walk-In Check-In";
            Name = "WalkInCheckInForm";

            Controls.Add(split);
            Controls.Add(bottom);
            Controls.Add(header);

            btnCheckIn.Left = ClientSize.Width - 290;
            btnClose.Left = btnCheckIn.Left - 166;
        }

        private static Label Info(
            string text, int x, int y, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 23),
                Font = new Font("Arial", 9.5F, FontStyle.Bold)
            };
        }

        private static void InfoAdd(
            Control panel, string text, int x, int y)
        {
            panel.Controls.Add(Info(text, x, y, 475));
        }

        private static TextBox TextField(
            Control panel, string label, int x, int y)
        {
            InfoAdd(panel, label, x, y);

            TextBox t = new TextBox
            {
                Location = new Point(x, y + 29),
                Width = 455,
                Font = new Font("Arial", 11F)
            };

            panel.Controls.Add(t);
            return t;
        }

        private static NumericUpDown Numeric(
            int x, int y, int max, int min)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Width = 97,
                Minimum = min,
                Maximum = max,
                Value = Math.Max(1, min)
            };
        }

        private static Button Button(
            string text, int x, int y, int w, int h, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = color,
                Font = new Font("Arial", 10F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
        }
    }
}
