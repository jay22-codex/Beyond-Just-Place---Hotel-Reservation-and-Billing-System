namespace UI
{
    partial class FrontDesk_CheckIn_CheckOut
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.btnBackReservation = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dataGridViewReservation = new System.Windows.Forms.DataGridView();
            this.columnGuestname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnRoom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnCheckInDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblConfirm = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtGuestName = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCheckOut = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblTotalBill = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblOtherCharges = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblRoomCharges = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblStayOutput = new System.Windows.Forms.Label();
            this.lblRoomOutput = new System.Windows.Forms.Label();
            this.lblGuestnameOutput = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtGuestName1 = new System.Windows.Forms.TextBox();
            this.btnSearch1 = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReservation)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label2.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(354, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 24);
            this.label2.TabIndex = 17;
            this.label2.Text = "Check-In/Check-Out";
            // 
            // btnBackReservation
            // 
            this.btnBackReservation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnBackReservation.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackReservation.Location = new System.Drawing.Point(12, 12);
            this.btnBackReservation.Name = "btnBackReservation";
            this.btnBackReservation.Size = new System.Drawing.Size(148, 55);
            this.btnBackReservation.TabIndex = 22;
            this.btnBackReservation.Text = "Back to Dashboard";
            this.btnBackReservation.UseVisualStyleBackColor = false;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.groupBox4.Controls.Add(this.dataGridViewReservation);
            this.groupBox4.Controls.Add(this.lblConfirm);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.txtGuestName);
            this.groupBox4.Controls.Add(this.btnSearch);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(21, 73);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(830, 190);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Check-In";
            // 
            // dataGridViewReservation
            // 
            this.dataGridViewReservation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewReservation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnGuestname,
            this.columnRoom,
            this.columnCheckInDate,
            this.columnStatus});
            this.dataGridViewReservation.Location = new System.Drawing.Point(28, 86);
            this.dataGridViewReservation.Name = "dataGridViewReservation";
            this.dataGridViewReservation.Size = new System.Drawing.Size(595, 89);
            this.dataGridViewReservation.TabIndex = 30;
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
            // lblConfirm
            // 
            this.lblConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblConfirm.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirm.Location = new System.Drawing.Point(641, 137);
            this.lblConfirm.Name = "lblConfirm";
            this.lblConfirm.Size = new System.Drawing.Size(166, 38);
            this.lblConfirm.TabIndex = 29;
            this.lblConfirm.Text = "Confirm Check-In";
            this.lblConfirm.UseVisualStyleBackColor = false;
            this.lblConfirm.Click += new System.EventHandler(this.lblConfirm_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 22);
            this.label1.TabIndex = 28;
            this.label1.Text = "Find Guest name:";
            // 
            // txtGuestName
            // 
            this.txtGuestName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGuestName.Location = new System.Drawing.Point(172, 21);
            this.txtGuestName.Name = "txtGuestName";
            this.txtGuestName.Size = new System.Drawing.Size(451, 38);
            this.txtGuestName.TabIndex = 24;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSearch.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Location = new System.Drawing.Point(641, 21);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(125, 38);
            this.btnSearch.TabIndex = 20;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.groupBox1.Controls.Add(this.btnCheckOut);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.lblStayOutput);
            this.groupBox1.Controls.Add(this.lblRoomOutput);
            this.groupBox1.Controls.Add(this.lblGuestnameOutput);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtGuestName1);
            this.groupBox1.Controls.Add(this.btnSearch1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(21, 269);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(830, 345);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Check-Out";
            // 
            // btnCheckOut
            // 
            this.btnCheckOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCheckOut.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckOut.Location = new System.Drawing.Point(604, 277);
            this.btnCheckOut.Name = "btnCheckOut";
            this.btnCheckOut.Size = new System.Drawing.Size(148, 55);
            this.btnCheckOut.TabIndex = 31;
            this.btnCheckOut.Text = "Confirm Check-Out";
            this.btnCheckOut.UseVisualStyleBackColor = false;
            this.btnCheckOut.Click += new System.EventHandler(this.btnCheckOut_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.groupBox2.Controls.Add(this.lblTotalBill);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.lblOtherCharges);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblRoomCharges);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(28, 119);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(724, 152);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Final Bill";
            // 
            // lblTotalBill
            // 
            this.lblTotalBill.AutoSize = true;
            this.lblTotalBill.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblTotalBill.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalBill.Location = new System.Drawing.Point(644, 118);
            this.lblTotalBill.Name = "lblTotalBill";
            this.lblTotalBill.Size = new System.Drawing.Size(49, 16);
            this.lblTotalBill.TabIndex = 37;
            this.lblTotalBill.Text = "Output";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(20, 118);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 16);
            this.label10.TabIndex = 36;
            this.label10.Text = "Total Bill:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(10, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(700, 16);
            this.label9.TabIndex = 35;
            this.label9.Text = "_________________________________________________________________________________" +
    "__________________";
            // 
            // lblOtherCharges
            // 
            this.lblOtherCharges.AutoSize = true;
            this.lblOtherCharges.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblOtherCharges.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOtherCharges.Location = new System.Drawing.Point(644, 60);
            this.lblOtherCharges.Name = "lblOtherCharges";
            this.lblOtherCharges.Size = new System.Drawing.Size(49, 16);
            this.lblOtherCharges.TabIndex = 34;
            this.lblOtherCharges.Text = "Output";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(18, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 16);
            this.label7.TabIndex = 33;
            this.label7.Text = "Other Charges:";
            // 
            // lblRoomCharges
            // 
            this.lblRoomCharges.AutoSize = true;
            this.lblRoomCharges.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblRoomCharges.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoomCharges.Location = new System.Drawing.Point(644, 34);
            this.lblRoomCharges.Name = "lblRoomCharges";
            this.lblRoomCharges.Size = new System.Drawing.Size(49, 16);
            this.lblRoomCharges.TabIndex = 32;
            this.lblRoomCharges.Text = "Output";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(18, 34);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 16);
            this.label11.TabIndex = 31;
            this.label11.Text = "Room Charges:";
            // 
            // lblStayOutput
            // 
            this.lblStayOutput.AutoSize = true;
            this.lblStayOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblStayOutput.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStayOutput.Location = new System.Drawing.Point(671, 73);
            this.lblStayOutput.Name = "lblStayOutput";
            this.lblStayOutput.Size = new System.Drawing.Size(108, 22);
            this.lblStayOutput.TabIndex = 34;
            this.lblStayOutput.Text = "Stay Output";
            // 
            // lblRoomOutput
            // 
            this.lblRoomOutput.AutoSize = true;
            this.lblRoomOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblRoomOutput.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoomOutput.Location = new System.Drawing.Point(436, 73);
            this.lblRoomOutput.Name = "lblRoomOutput";
            this.lblRoomOutput.Size = new System.Drawing.Size(123, 22);
            this.lblRoomOutput.TabIndex = 33;
            this.lblRoomOutput.Text = "Room Output";
            // 
            // lblGuestnameOutput
            // 
            this.lblGuestnameOutput.AutoSize = true;
            this.lblGuestnameOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblGuestnameOutput.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGuestnameOutput.Location = new System.Drawing.Point(159, 73);
            this.lblGuestnameOutput.Name = "lblGuestnameOutput";
            this.lblGuestnameOutput.Size = new System.Drawing.Size(169, 22);
            this.lblGuestnameOutput.TabIndex = 32;
            this.lblGuestnameOutput.Text = "Guest name Ouput";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label6.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(613, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 22);
            this.label6.TabIndex = 31;
            this.label6.Text = "Stay:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label5.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(363, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 22);
            this.label5.TabIndex = 30;
            this.label5.Text = "Room:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label4.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(37, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 22);
            this.label4.TabIndex = 29;
            this.label4.Text = "Guest name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label3.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 22);
            this.label3.TabIndex = 28;
            this.label3.Text = "Find Guest name:";
            // 
            // txtGuestName1
            // 
            this.txtGuestName1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGuestName1.Location = new System.Drawing.Point(172, 21);
            this.txtGuestName1.Name = "txtGuestName1";
            this.txtGuestName1.Size = new System.Drawing.Size(451, 38);
            this.txtGuestName1.TabIndex = 24;
            // 
            // btnSearch1
            // 
            this.btnSearch1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSearch1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch1.Location = new System.Drawing.Point(641, 21);
            this.btnSearch1.Name = "btnSearch1";
            this.btnSearch1.Size = new System.Drawing.Size(125, 38);
            this.btnSearch1.TabIndex = 20;
            this.btnSearch1.Text = "Search";
            this.btnSearch1.UseVisualStyleBackColor = false;
            this.btnSearch1.Click += new System.EventHandler(this.btnSearch1_Click);
            // 
            // FrontDesk_CheckIn_CheckOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(880, 627);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnBackReservation);
            this.Controls.Add(this.label2);
            this.Name = "FrontDesk_CheckIn_CheckOut";
            this.Text = "FrontDesk_CheckIn_CheckOut";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReservation)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBackReservation;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGuestName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button lblConfirm;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtGuestName1;
        private System.Windows.Forms.Button btnSearch1;
        private System.Windows.Forms.DataGridView dataGridViewReservation;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnGuestname;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnRoom;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCheckInDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnStatus;
        private System.Windows.Forms.Label lblStayOutput;
        private System.Windows.Forms.Label lblRoomOutput;
        private System.Windows.Forms.Label lblGuestnameOutput;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblOtherCharges;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblRoomCharges;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnCheckOut;
        private System.Windows.Forms.Label lblTotalBill;
    }
}