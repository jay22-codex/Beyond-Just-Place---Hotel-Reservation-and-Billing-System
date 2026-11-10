namespace UI
{
    partial class FrontDesk_Billing_PaymentUI
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtSelectGuest = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnSearchGuest = new System.Windows.Forms.Button();
            this.lblBillingNotice = new System.Windows.Forms.Label();
            this.lblReservationID = new System.Windows.Forms.Label();
            this.lblReservationIDOutput = new System.Windows.Forms.Label();
            this.lblGuestName = new System.Windows.Forms.Label();
            this.lblGuestNameOutput = new System.Windows.Forms.Label();
            this.lblRoom = new System.Windows.Forms.Label();
            this.lblRoomOutput = new System.Windows.Forms.Label();
            this.lblStay = new System.Windows.Forms.Label();
            this.lblStayOutput = new System.Windows.Forms.Label();
            this.lblCheckIn = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCheckout = new System.Windows.Forms.Label();
            this.lblCheckOutOutput = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblRoomChargers = new System.Windows.Forms.Label();
            this.OtherCharges = new System.Windows.Forms.Label();
            this.lblOtherCharges = new System.Windows.Forms.Label();
            this.lblRoomChargesRate = new System.Windows.Forms.Label();
            this.lblRoomChargesQty = new System.Windows.Forms.Label();
            this.lblRoomChargesAmount = new System.Windows.Forms.Label();
            this.lblOtherChargesRate = new System.Windows.Forms.Label();
            this.lblOtherChargesQty = new System.Windows.Forms.Label();
            this.lblOtherChargesAmount = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblTotalBill = new System.Windows.Forms.Label();
            this.lblPaymentMethod = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.lblPaymentAmount = new System.Windows.Forms.Label();
            this.lblPaymentAmountOutput = new System.Windows.Forms.Label();
            this.lblAmountReceived = new System.Windows.Forms.Label();
            this.txtAmountReceived = new System.Windows.Forms.TextBox();
            this.lblChange = new System.Windows.Forms.Label();
            this.lblChangeOutput = new System.Windows.Forms.Label();
            this.btnConfirmPayment = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.txtSelectGuest.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();

            // txtSelectGuest
            this.txtSelectGuest.Name = "txtSelectGuest";
            this.txtSelectGuest.Location = new System.Drawing.Point(20, 78);
            this.txtSelectGuest.Size = new System.Drawing.Size(1620, 240);
            this.txtSelectGuest.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular);
            this.txtSelectGuest.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.txtSelectGuest.TabStop = false;
            this.txtSelectGuest.Text = "Select Guest";
            this.txtSelectGuest.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);

            // groupBox1
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Location = new System.Drawing.Point(20, 335);
            this.groupBox1.Size = new System.Drawing.Size(1620, 352);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular);
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Charges";
            this.groupBox1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);

            // groupBox2
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Location = new System.Drawing.Point(18, 38);
            this.groupBox2.Size = new System.Drawing.Size(1584, 295);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular);
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Final Bill";
            this.groupBox2.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);

            // groupBox4
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Location = new System.Drawing.Point(20, 706);
            this.groupBox4.Size = new System.Drawing.Size(1620, 200);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular);
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Payment";
            this.groupBox4.Anchor = (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom);

            // label2
            this.label2.Name = "label2";
            this.label2.Location = new System.Drawing.Point(30, 18);
            this.label2.Size = new System.Drawing.Size(620, 44);
            this.label2.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.label2.AutoSize = false;
            this.label2.AutoEllipsis = true;
            this.label2.Text = "Billing and Payment";

            // textBox1
            this.textBox1.Name = "textBox1";
            this.textBox1.Location = new System.Drawing.Point(212, 37);
            this.textBox1.Size = new System.Drawing.Size(680, 32);
            this.textBox1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular);
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Text = "";
            this.textBox1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);

            // btnSearchGuest
            this.btnSearchGuest.Name = "btnSearchGuest";
            this.btnSearchGuest.Location = new System.Drawing.Point(910, 37);
            this.btnSearchGuest.Size = new System.Drawing.Size(160, 35);
            this.btnSearchGuest.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnSearchGuest.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this.btnSearchGuest.UseVisualStyleBackColor = false;
            this.btnSearchGuest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchGuest.Text = "Search Guest";

            // lblBillingNotice
            this.lblBillingNotice.Name = "lblBillingNotice";
            this.lblBillingNotice.Location = new System.Drawing.Point(1092, 43);
            this.lblBillingNotice.Size = new System.Drawing.Size(480, 25);
            this.lblBillingNotice.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Regular);
            this.lblBillingNotice.AutoSize = false;
            this.lblBillingNotice.AutoEllipsis = true;
            this.lblBillingNotice.Text = "Search a checked-in guest or open from Check-Out.";

            // lblReservationID
            this.lblReservationID.Name = "lblReservationID";
            this.lblReservationID.Location = new System.Drawing.Point(30, 108);
            this.lblReservationID.Size = new System.Drawing.Size(155, 30);
            this.lblReservationID.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblReservationID.AutoSize = false;
            this.lblReservationID.AutoEllipsis = true;
            this.lblReservationID.Text = "Reservation ID:";

            // lblReservationIDOutput
            this.lblReservationIDOutput.Name = "lblReservationIDOutput";
            this.lblReservationIDOutput.Location = new System.Drawing.Point(187, 108);
            this.lblReservationIDOutput.Size = new System.Drawing.Size(153, 30);
            this.lblReservationIDOutput.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblReservationIDOutput.AutoSize = false;
            this.lblReservationIDOutput.AutoEllipsis = true;
            this.lblReservationIDOutput.Text = "Output";

            // lblGuestName
            this.lblGuestName.Name = "lblGuestName";
            this.lblGuestName.Location = new System.Drawing.Point(368, 108);
            this.lblGuestName.Size = new System.Drawing.Size(128, 30);
            this.lblGuestName.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblGuestName.AutoSize = false;
            this.lblGuestName.AutoEllipsis = true;
            this.lblGuestName.Text = "Guest name:";

            // lblGuestNameOutput
            this.lblGuestNameOutput.Name = "lblGuestNameOutput";
            this.lblGuestNameOutput.Location = new System.Drawing.Point(500, 108);
            this.lblGuestNameOutput.Size = new System.Drawing.Size(330, 30);
            this.lblGuestNameOutput.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblGuestNameOutput.AutoSize = false;
            this.lblGuestNameOutput.AutoEllipsis = true;
            this.lblGuestNameOutput.Text = "Guest Name Output";

            // lblRoom
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Location = new System.Drawing.Point(860, 108);
            this.lblRoom.Size = new System.Drawing.Size(78, 30);
            this.lblRoom.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblRoom.AutoSize = false;
            this.lblRoom.AutoEllipsis = true;
            this.lblRoom.Text = "Room:";

            // lblRoomOutput
            this.lblRoomOutput.Name = "lblRoomOutput";
            this.lblRoomOutput.Location = new System.Drawing.Point(945, 108);
            this.lblRoomOutput.Size = new System.Drawing.Size(210, 30);
            this.lblRoomOutput.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblRoomOutput.AutoSize = false;
            this.lblRoomOutput.AutoEllipsis = true;
            this.lblRoomOutput.Text = "Room Output";

            // lblStay
            this.lblStay.Name = "lblStay";
            this.lblStay.Location = new System.Drawing.Point(1195, 108);
            this.lblStay.Size = new System.Drawing.Size(62, 30);
            this.lblStay.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblStay.AutoSize = false;
            this.lblStay.AutoEllipsis = true;
            this.lblStay.Text = "Stay:";

            // lblStayOutput
            this.lblStayOutput.Name = "lblStayOutput";
            this.lblStayOutput.Location = new System.Drawing.Point(1265, 108);
            this.lblStayOutput.Size = new System.Drawing.Size(290, 30);
            this.lblStayOutput.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblStayOutput.AutoSize = false;
            this.lblStayOutput.AutoEllipsis = true;
            this.lblStayOutput.Text = "Stay Output";

            // lblCheckIn
            this.lblCheckIn.Name = "lblCheckIn";
            this.lblCheckIn.Location = new System.Drawing.Point(30, 167);
            this.lblCheckIn.Size = new System.Drawing.Size(110, 30);
            this.lblCheckIn.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblCheckIn.AutoSize = false;
            this.lblCheckIn.AutoEllipsis = true;
            this.lblCheckIn.Text = "Check-In:";

            // label3
            this.label3.Name = "label3";
            this.label3.Location = new System.Drawing.Point(148, 167);
            this.label3.Size = new System.Drawing.Size(188, 30);
            this.label3.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label3.AutoSize = false;
            this.label3.AutoEllipsis = true;
            this.label3.Text = "Output";

            // lblCheckout
            this.lblCheckout.Name = "lblCheckout";
            this.lblCheckout.Location = new System.Drawing.Point(368, 167);
            this.lblCheckout.Size = new System.Drawing.Size(120, 30);
            this.lblCheckout.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblCheckout.AutoSize = false;
            this.lblCheckout.AutoEllipsis = true;
            this.lblCheckout.Text = "Check-Out:";

            // lblCheckOutOutput
            this.lblCheckOutOutput.Name = "lblCheckOutOutput";
            this.lblCheckOutOutput.Location = new System.Drawing.Point(500, 167);
            this.lblCheckOutOutput.Size = new System.Drawing.Size(250, 30);
            this.lblCheckOutOutput.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblCheckOutOutput.AutoSize = false;
            this.lblCheckOutOutput.AutoEllipsis = true;
            this.lblCheckOutOutput.Text = "Output";

            // label4
            this.label4.Name = "label4";
            this.label4.Location = new System.Drawing.Point(860, 167);
            this.label4.Size = new System.Drawing.Size(80, 30);
            this.label4.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.label4.AutoSize = false;
            this.label4.AutoEllipsis = true;
            this.label4.Text = "Status:";

            // label5
            this.label5.Name = "label5";
            this.label5.Location = new System.Drawing.Point(945, 167);
            this.label5.Size = new System.Drawing.Size(350, 30);
            this.label5.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label5.AutoSize = false;
            this.label5.AutoEllipsis = true;
            this.label5.Text = "Output";

            // label6
            this.label6.Name = "label6";
            this.label6.Location = new System.Drawing.Point(40, 39);
            this.label6.Size = new System.Drawing.Size(560, 31);
            this.label6.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label6.AutoSize = false;
            this.label6.AutoEllipsis = true;
            this.label6.Text = "Description";

            // label7
            this.label7.Name = "label7";
            this.label7.Location = new System.Drawing.Point(800, 39);
            this.label7.Size = new System.Drawing.Size(195, 31);
            this.label7.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label7.AutoSize = false;
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label7.AutoEllipsis = true;
            this.label7.Text = "Rate (PHP)";
            this.label7.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // label8
            this.label8.Name = "label8";
            this.label8.Location = new System.Drawing.Point(1050, 39);
            this.label8.Size = new System.Drawing.Size(110, 31);
            this.label8.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label8.AutoSize = false;
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.AutoEllipsis = true;
            this.label8.Text = "Qty";
            this.label8.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // label11
            this.label11.Name = "label11";
            this.label11.Location = new System.Drawing.Point(1260, 39);
            this.label11.Size = new System.Drawing.Size(245, 31);
            this.label11.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label11.AutoSize = false;
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label11.AutoEllipsis = true;
            this.label11.Text = "Amount (PHP)";
            this.label11.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblRoomChargers
            this.lblRoomChargers.Name = "lblRoomChargers";
            this.lblRoomChargers.Location = new System.Drawing.Point(40, 89);
            this.lblRoomChargers.Size = new System.Drawing.Size(550, 31);
            this.lblRoomChargers.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblRoomChargers.AutoSize = false;
            this.lblRoomChargers.AutoEllipsis = true;
            this.lblRoomChargers.Text = "Room Charges";

            // OtherCharges
            this.OtherCharges.Name = "OtherCharges";
            this.OtherCharges.Location = new System.Drawing.Point(40, 142);
            this.OtherCharges.Size = new System.Drawing.Size(156, 31);
            this.OtherCharges.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.OtherCharges.AutoSize = false;
            this.OtherCharges.AutoEllipsis = true;
            this.OtherCharges.Text = "Other Charges";

            // lblOtherCharges
            this.lblOtherCharges.Name = "lblOtherCharges";
            this.lblOtherCharges.Location = new System.Drawing.Point(196, 145);
            this.lblOtherCharges.Size = new System.Drawing.Size(440, 27);
            this.lblOtherCharges.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Regular);
            this.lblOtherCharges.AutoSize = false;
            this.lblOtherCharges.AutoEllipsis = true;
            this.lblOtherCharges.Text = "(Extra bed, late check-out, etc.)";

            // lblRoomChargesRate
            this.lblRoomChargesRate.Name = "lblRoomChargesRate";
            this.lblRoomChargesRate.Location = new System.Drawing.Point(800, 89);
            this.lblRoomChargesRate.Size = new System.Drawing.Size(195, 32);
            this.lblRoomChargesRate.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblRoomChargesRate.AutoSize = false;
            this.lblRoomChargesRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRoomChargesRate.AutoEllipsis = true;
            this.lblRoomChargesRate.Text = "0.00";
            this.lblRoomChargesRate.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblRoomChargesQty
            this.lblRoomChargesQty.Name = "lblRoomChargesQty";
            this.lblRoomChargesQty.Location = new System.Drawing.Point(1050, 89);
            this.lblRoomChargesQty.Size = new System.Drawing.Size(110, 32);
            this.lblRoomChargesQty.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblRoomChargesQty.AutoSize = false;
            this.lblRoomChargesQty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRoomChargesQty.AutoEllipsis = true;
            this.lblRoomChargesQty.Text = "0";
            this.lblRoomChargesQty.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblRoomChargesAmount
            this.lblRoomChargesAmount.Name = "lblRoomChargesAmount";
            this.lblRoomChargesAmount.Location = new System.Drawing.Point(1260, 89);
            this.lblRoomChargesAmount.Size = new System.Drawing.Size(245, 32);
            this.lblRoomChargesAmount.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblRoomChargesAmount.AutoSize = false;
            this.lblRoomChargesAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRoomChargesAmount.AutoEllipsis = true;
            this.lblRoomChargesAmount.Text = "0.00";
            this.lblRoomChargesAmount.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblOtherChargesRate
            this.lblOtherChargesRate.Name = "lblOtherChargesRate";
            this.lblOtherChargesRate.Location = new System.Drawing.Point(800, 142);
            this.lblOtherChargesRate.Size = new System.Drawing.Size(195, 32);
            this.lblOtherChargesRate.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblOtherChargesRate.AutoSize = false;
            this.lblOtherChargesRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOtherChargesRate.AutoEllipsis = true;
            this.lblOtherChargesRate.Text = "0.00";
            this.lblOtherChargesRate.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblOtherChargesQty
            this.lblOtherChargesQty.Name = "lblOtherChargesQty";
            this.lblOtherChargesQty.Location = new System.Drawing.Point(1050, 142);
            this.lblOtherChargesQty.Size = new System.Drawing.Size(110, 32);
            this.lblOtherChargesQty.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblOtherChargesQty.AutoSize = false;
            this.lblOtherChargesQty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOtherChargesQty.AutoEllipsis = true;
            this.lblOtherChargesQty.Text = "0";
            this.lblOtherChargesQty.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblOtherChargesAmount
            this.lblOtherChargesAmount.Name = "lblOtherChargesAmount";
            this.lblOtherChargesAmount.Location = new System.Drawing.Point(1260, 142);
            this.lblOtherChargesAmount.Size = new System.Drawing.Size(245, 32);
            this.lblOtherChargesAmount.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblOtherChargesAmount.AutoSize = false;
            this.lblOtherChargesAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblOtherChargesAmount.AutoEllipsis = true;
            this.lblOtherChargesAmount.Text = "0.00";
            this.lblOtherChargesAmount.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // label9
            this.label9.Name = "label9";
            this.label9.Location = new System.Drawing.Point(40, 200);
            this.label9.Size = new System.Drawing.Size(1465, 2);
            this.label9.AutoSize = false;
            this.label9.AutoEllipsis = true;
            this.label9.Text = "";
            this.label9.Anchor = (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            // label10
            this.label10.Name = "label10";
            this.label10.Location = new System.Drawing.Point(40, 225);
            this.label10.Size = new System.Drawing.Size(385, 42);
            this.label10.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.label10.AutoSize = false;
            this.label10.AutoEllipsis = true;
            this.label10.Text = "Total Bill:";

            // lblTotalBill
            this.lblTotalBill.Name = "lblTotalBill";
            this.lblTotalBill.Location = new System.Drawing.Point(1130, 220);
            this.lblTotalBill.Size = new System.Drawing.Size(375, 51);
            this.lblTotalBill.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalBill.AutoSize = false;
            this.lblTotalBill.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTotalBill.AutoEllipsis = true;
            this.lblTotalBill.Text = "PHP 0.00";
            this.lblTotalBill.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblPaymentMethod
            this.lblPaymentMethod.Name = "lblPaymentMethod";
            this.lblPaymentMethod.Location = new System.Drawing.Point(30, 51);
            this.lblPaymentMethod.Size = new System.Drawing.Size(163, 31);
            this.lblPaymentMethod.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblPaymentMethod.AutoSize = false;
            this.lblPaymentMethod.AutoEllipsis = true;
            this.lblPaymentMethod.Text = "Payment Method:";

            // cmbPaymentMethod
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Location = new System.Drawing.Point(195, 51);
            this.cmbPaymentMethod.Size = new System.Drawing.Size(240, 32);
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // lblPaymentAmount
            this.lblPaymentAmount.Name = "lblPaymentAmount";
            this.lblPaymentAmount.Location = new System.Drawing.Point(470, 51);
            this.lblPaymentAmount.Size = new System.Drawing.Size(145, 31);
            this.lblPaymentAmount.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblPaymentAmount.AutoSize = false;
            this.lblPaymentAmount.AutoEllipsis = true;
            this.lblPaymentAmount.Text = "Amount Due:";

            // lblPaymentAmountOutput
            this.lblPaymentAmountOutput.Name = "lblPaymentAmountOutput";
            this.lblPaymentAmountOutput.Location = new System.Drawing.Point(617, 51);
            this.lblPaymentAmountOutput.Size = new System.Drawing.Size(200, 31);
            this.lblPaymentAmountOutput.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblPaymentAmountOutput.AutoSize = false;
            this.lblPaymentAmountOutput.AutoEllipsis = true;
            this.lblPaymentAmountOutput.Text = "PHP 0.00";

            // lblAmountReceived
            this.lblAmountReceived.Name = "lblAmountReceived";
            this.lblAmountReceived.Location = new System.Drawing.Point(850, 51);
            this.lblAmountReceived.Size = new System.Drawing.Size(168, 31);
            this.lblAmountReceived.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblAmountReceived.AutoSize = false;
            this.lblAmountReceived.AutoEllipsis = true;
            this.lblAmountReceived.Text = "Amount Received:";

            // txtAmountReceived
            this.txtAmountReceived.Name = "txtAmountReceived";
            this.txtAmountReceived.Location = new System.Drawing.Point(1020, 51);
            this.txtAmountReceived.Size = new System.Drawing.Size(175, 32);
            this.txtAmountReceived.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.txtAmountReceived.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAmountReceived.Text = "0.00";
            this.txtAmountReceived.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblChange
            this.lblChange.Name = "lblChange";
            this.lblChange.Location = new System.Drawing.Point(1220, 51);
            this.lblChange.Size = new System.Drawing.Size(90, 31);
            this.lblChange.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular);
            this.lblChange.AutoSize = false;
            this.lblChange.AutoEllipsis = true;
            this.lblChange.Text = "Change:";
            this.lblChange.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // lblChangeOutput
            this.lblChangeOutput.Name = "lblChangeOutput";
            this.lblChangeOutput.Location = new System.Drawing.Point(1310, 51);
            this.lblChangeOutput.Size = new System.Drawing.Size(200, 31);
            this.lblChangeOutput.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblChangeOutput.AutoSize = false;
            this.lblChangeOutput.AutoEllipsis = true;
            this.lblChangeOutput.Text = "PHP 0.00";
            this.lblChangeOutput.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);

            // btnConfirmPayment
            this.btnConfirmPayment.Name = "btnConfirmPayment";
            this.btnConfirmPayment.Location = new System.Drawing.Point(1116, 115);
            this.btnConfirmPayment.Size = new System.Drawing.Size(220, 51);
            this.btnConfirmPayment.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnConfirmPayment.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this.btnConfirmPayment.UseVisualStyleBackColor = false;
            this.btnConfirmPayment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmPayment.Text = "Confirm Payment";
            this.btnConfirmPayment.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);

            // btnCancel
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Location = new System.Drawing.Point(1354, 115);
            this.btnCancel.Size = new System.Drawing.Size(190, 51);
            this.btnCancel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);

            this.groupBox1.Controls.Add(this.groupBox2);
            this.txtSelectGuest.Controls.Add(this.textBox1);
            this.txtSelectGuest.Controls.Add(this.btnSearchGuest);
            this.txtSelectGuest.Controls.Add(this.lblBillingNotice);
            this.txtSelectGuest.Controls.Add(this.lblReservationID);
            this.txtSelectGuest.Controls.Add(this.lblReservationIDOutput);
            this.txtSelectGuest.Controls.Add(this.lblGuestName);
            this.txtSelectGuest.Controls.Add(this.lblGuestNameOutput);
            this.txtSelectGuest.Controls.Add(this.lblRoom);
            this.txtSelectGuest.Controls.Add(this.lblRoomOutput);
            this.txtSelectGuest.Controls.Add(this.lblStay);
            this.txtSelectGuest.Controls.Add(this.lblStayOutput);
            this.txtSelectGuest.Controls.Add(this.lblCheckIn);
            this.txtSelectGuest.Controls.Add(this.label3);
            this.txtSelectGuest.Controls.Add(this.lblCheckout);
            this.txtSelectGuest.Controls.Add(this.lblCheckOutOutput);
            this.txtSelectGuest.Controls.Add(this.label4);
            this.txtSelectGuest.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.lblRoomChargers);
            this.groupBox2.Controls.Add(this.OtherCharges);
            this.groupBox2.Controls.Add(this.lblOtherCharges);
            this.groupBox2.Controls.Add(this.lblRoomChargesRate);
            this.groupBox2.Controls.Add(this.lblRoomChargesQty);
            this.groupBox2.Controls.Add(this.lblRoomChargesAmount);
            this.groupBox2.Controls.Add(this.lblOtherChargesRate);
            this.groupBox2.Controls.Add(this.lblOtherChargesQty);
            this.groupBox2.Controls.Add(this.lblOtherChargesAmount);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.lblTotalBill);
            this.groupBox4.Controls.Add(this.lblPaymentMethod);
            this.groupBox4.Controls.Add(this.cmbPaymentMethod);
            this.groupBox4.Controls.Add(this.lblPaymentAmount);
            this.groupBox4.Controls.Add(this.lblPaymentAmountOutput);
            this.groupBox4.Controls.Add(this.lblAmountReceived);
            this.groupBox4.Controls.Add(this.txtAmountReceived);
            this.groupBox4.Controls.Add(this.lblChange);
            this.groupBox4.Controls.Add(this.lblChangeOutput);
            this.groupBox4.Controls.Add(this.btnConfirmPayment);
            this.groupBox4.Controls.Add(this.btnCancel);

            this.Controls.Add(this.txtSelectGuest);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label2);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this.ClientSize = new System.Drawing.Size(1660, 930);
            this.MinimumSize = new System.Drawing.Size(980, 700);
            this.Name = "FrontDesk_Billing_PaymentUI";
            this.Text = "Billing and Payment";

            this.txtSelectGuest.ResumeLayout(false);
            this.txtSelectGuest.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Existing control names are kept unchanged.
        private System.Windows.Forms.GroupBox txtSelectGuest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnSearchGuest;
        private System.Windows.Forms.Label lblBillingNotice;
        private System.Windows.Forms.Label lblReservationID;
        private System.Windows.Forms.Label lblReservationIDOutput;
        private System.Windows.Forms.Label lblGuestName;
        private System.Windows.Forms.Label lblGuestNameOutput;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.Label lblRoomOutput;
        private System.Windows.Forms.Label lblStay;
        private System.Windows.Forms.Label lblStayOutput;
        private System.Windows.Forms.Label lblCheckIn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCheckout;
        private System.Windows.Forms.Label lblCheckOutOutput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblRoomChargers;
        private System.Windows.Forms.Label OtherCharges;
        private System.Windows.Forms.Label lblOtherCharges;
        private System.Windows.Forms.Label lblRoomChargesRate;
        private System.Windows.Forms.Label lblRoomChargesQty;
        private System.Windows.Forms.Label lblRoomChargesAmount;
        private System.Windows.Forms.Label lblOtherChargesRate;
        private System.Windows.Forms.Label lblOtherChargesQty;
        private System.Windows.Forms.Label lblOtherChargesAmount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblTotalBill;
        private System.Windows.Forms.Label lblPaymentMethod;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label lblPaymentAmount;
        private System.Windows.Forms.Label lblPaymentAmountOutput;
        private System.Windows.Forms.Label lblAmountReceived;
        private System.Windows.Forms.TextBox txtAmountReceived;
        private System.Windows.Forms.Label lblChange;
        private System.Windows.Forms.Label lblChangeOutput;
        private System.Windows.Forms.Button btnConfirmPayment;
        private System.Windows.Forms.Button btnCancel;
    }
}