
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class FrontDesk_CheckIn_CheckOut
    {
        private System.ComponentModel.IContainer components = null;

        private Label label2;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label9;
        private Label label10;
        private Label label11;

        private Label lblGuestnameOutput;
        private Label lblRoomOutput;
        private Label lblStayOutput;
        private Label lblRoomCharges;
        private Label lblOtherCharges;
        private Label lblTotalBill;
        private Button btnSearchCheckIn;
        private Button button1;
        private Button button3;
        private Button btnCheckOut;

        private TextBox txtCheckInGuest;
        private TextBox textBox1;

        private GroupBox groupBox4;
        private GroupBox groupBox1;
        private GroupBox groupBox2;

        private DataGridView dgvCheckInReservations;
        private DataGridViewTextBoxColumn columnGuestname;
        private DataGridViewTextBoxColumn columnRoom;
        private DataGridViewTextBoxColumn columnCheckInDate;
        private DataGridViewTextBoxColumn columnStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblGuestnameOutput = new System.Windows.Forms.Label();
            this.lblRoomOutput = new System.Windows.Forms.Label();
            this.lblStayOutput = new System.Windows.Forms.Label();
            this.lblRoomCharges = new System.Windows.Forms.Label();
            this.lblOtherCharges = new System.Windows.Forms.Label();
            this.lblTotalBill = new System.Windows.Forms.Label();
            this.btnSearchCheckIn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnCheckOut = new System.Windows.Forms.Button();
            this.txtCheckInGuest = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvCheckInReservations = new System.Windows.Forms.DataGridView();
            this.columnGuestname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnRoom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnCheckInDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckInReservations)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(596, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(229, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "Check-In / Check-Out";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find Guest Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Find Guest Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Guest:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(350, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "Room:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(615, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 17);
            this.label6.TabIndex = 7;
            this.label6.Text = "Stay:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(109, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Other Charges:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(360, 17);
            this.label9.TabIndex = 4;
            this.label9.Text = "____________________________________________";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 115);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 17);
            this.label10.TabIndex = 5;
            this.label10.Text = "Total Bill:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 17);
            this.label11.TabIndex = 0;
            this.label11.Text = "Room Charges:";
            // 
            // lblGuestnameOutput
            // 
            this.lblGuestnameOutput.AutoSize = true;
            this.lblGuestnameOutput.Location = new System.Drawing.Point(100, 82);
            this.lblGuestnameOutput.Name = "lblGuestnameOutput";
            this.lblGuestnameOutput.Size = new System.Drawing.Size(52, 17);
            this.lblGuestnameOutput.TabIndex = 4;
            this.lblGuestnameOutput.Text = "Output";
            // 
            // lblRoomOutput
            // 
            this.lblRoomOutput.AutoSize = true;
            this.lblRoomOutput.Location = new System.Drawing.Point(420, 82);
            this.lblRoomOutput.Name = "lblRoomOutput";
            this.lblRoomOutput.Size = new System.Drawing.Size(52, 17);
            this.lblRoomOutput.TabIndex = 6;
            this.lblRoomOutput.Text = "Output";
            // 
            // lblStayOutput
            // 
            this.lblStayOutput.AutoSize = true;
            this.lblStayOutput.Location = new System.Drawing.Point(675, 82);
            this.lblStayOutput.Name = "lblStayOutput";
            this.lblStayOutput.Size = new System.Drawing.Size(52, 17);
            this.lblStayOutput.TabIndex = 8;
            this.lblStayOutput.Text = "Output";
            // 
            // lblRoomCharges
            // 
            this.lblRoomCharges.AutoSize = true;
            this.lblRoomCharges.Location = new System.Drawing.Point(620, 33);
            this.lblRoomCharges.Name = "lblRoomCharges";
            this.lblRoomCharges.Size = new System.Drawing.Size(52, 17);
            this.lblRoomCharges.TabIndex = 1;
            this.lblRoomCharges.Text = "Output";
            // 
            // lblOtherCharges
            // 
            this.lblOtherCharges.AutoSize = true;
            this.lblOtherCharges.Location = new System.Drawing.Point(620, 62);
            this.lblOtherCharges.Name = "lblOtherCharges";
            this.lblOtherCharges.Size = new System.Drawing.Size(52, 17);
            this.lblOtherCharges.TabIndex = 3;
            this.lblOtherCharges.Text = "Output";
            // 
            // lblTotalBill
            // 
            this.lblTotalBill.AutoSize = true;
            this.lblTotalBill.Location = new System.Drawing.Point(620, 115);
            this.lblTotalBill.Name = "lblTotalBill";
            this.lblTotalBill.Size = new System.Drawing.Size(52, 17);
            this.lblTotalBill.TabIndex = 6;
            this.lblTotalBill.Text = "Output";
            // 
            // btnSearchCheckIn
            // 
            this.btnSearchCheckIn.Location = new System.Drawing.Point(640, 28);
            this.btnSearchCheckIn.Name = "btnSearchCheckIn";
            this.btnSearchCheckIn.Size = new System.Drawing.Size(150, 35);
            this.btnSearchCheckIn.TabIndex = 2;
            this.btnSearchCheckIn.Text = "Search";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(640, 130);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 40);
            this.button1.TabIndex = 4;
            this.button1.Text = "Confirm Check-In";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(640, 28);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 35);
            this.button3.TabIndex = 2;
            this.button3.Text = "Search";
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.Location = new System.Drawing.Point(605, 280);
            this.btnCheckOut.Name = "btnCheckOut";
            this.btnCheckOut.Size = new System.Drawing.Size(150, 50);
            this.btnCheckOut.TabIndex = 10;
            this.btnCheckOut.Text = "Confirm Check-Out";
            // 
            // txtCheckInGuest
            // 
            this.txtCheckInGuest.Location = new System.Drawing.Point(170, 30);
            this.txtCheckInGuest.Name = "txtCheckInGuest";
            this.txtCheckInGuest.Size = new System.Drawing.Size(450, 24);
            this.txtCheckInGuest.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(170, 30);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(450, 24);
            this.textBox1.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.txtCheckInGuest);
            this.groupBox4.Controls.Add(this.btnSearchCheckIn);
            this.groupBox4.Controls.Add(this.dgvCheckInReservations);
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 11F);
            this.groupBox4.Location = new System.Drawing.Point(12, 38);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(830, 190);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Check-In";
            // 
            // dgvCheckInReservations
            // 
            this.dgvCheckInReservations.AllowUserToAddRows = false;
            this.dgvCheckInReservations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnGuestname,
            this.columnRoom,
            this.columnCheckInDate,
            this.columnStatus});
            this.dgvCheckInReservations.Location = new System.Drawing.Point(28, 75);
            this.dgvCheckInReservations.Name = "dgvCheckInReservations";
            this.dgvCheckInReservations.Size = new System.Drawing.Size(595, 100);
            this.dgvCheckInReservations.TabIndex = 3;
            // 
            // columnGuestname
            // 
            this.columnGuestname.HeaderText = "Guest";
            this.columnGuestname.Name = "columnGuestname";
            // 
            // columnRoom
            // 
            this.columnRoom.HeaderText = "Room";
            this.columnRoom.Name = "columnRoom";
            // 
            // columnCheckInDate
            // 
            this.columnCheckInDate.HeaderText = "Check-In Date";
            this.columnCheckInDate.Name = "columnCheckInDate";
            // 
            // columnStatus
            // 
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblGuestnameOutput);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblRoomOutput);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblStayOutput);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnCheckOut);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 11F);
            this.groupBox1.Location = new System.Drawing.Point(20, 275);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(830, 345);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Check-Out";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.lblRoomCharges);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblOtherCharges);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.lblTotalBill);
            this.groupBox2.Location = new System.Drawing.Point(28, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(725, 150);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Final Bill";
            // 
            // FrontDesk_CheckIn_CheckOut
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1413, 837);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrontDesk_CheckIn_CheckOut";
            this.Text = "Check-In / Check-Out";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckInReservations)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
