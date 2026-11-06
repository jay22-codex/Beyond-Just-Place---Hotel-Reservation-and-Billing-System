
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class FrontDeskHome
    {
        private System.ComponentModel.IContainer components = null;

        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;

        private GroupBox groupBoxTotalReservation;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;

        private ListBox listBox1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.label5 = new Label();
            this.label6 = new Label();

            this.groupBoxTotalReservation = new GroupBox();
            this.groupBox1 = new GroupBox();
            this.groupBox2 = new GroupBox();
            this.groupBox3 = new GroupBox();
            this.groupBox4 = new GroupBox();

            this.listBox1 = new ListBox();

            this.SuspendLayout();

            this.BackColor = Color.FromArgb(224, 224, 224);
            this.ClientSize = new System.Drawing.Size(1660, 930);
            this.AutoScroll = true;
            this.Text = "Front Desk Home";
            this.Name = "FrontDeskHome";

            this.label2.Name = "label2";
            this.label2.Text = "Dashboard - Front Desk";
            this.label2.Font = new Font("Arial", 16F, FontStyle.Bold);
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 22);
            this.Controls.Add(this.label2);

            this.groupBoxTotalReservation.Name = "groupBoxTotalReservation";
            this.groupBoxTotalReservation.Text = "Today Reservation";
            this.groupBoxTotalReservation.Font = new Font("Arial", 11F, FontStyle.Bold);
            this.groupBoxTotalReservation.Location = new System.Drawing.Point(26, 82);
            this.groupBoxTotalReservation.Size = new System.Drawing.Size(385, 178);
            this.groupBoxTotalReservation.BackColor =
                Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBoxTotalReservation);

            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Text = "Current Check-in Guest";
            this.groupBox1.Font = new Font("Arial", 11F, FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(430, 82);
            this.groupBox1.Size = new System.Drawing.Size(385, 178);
            this.groupBox1.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox1);

            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Text = "Expected Check-ins Today";
            this.groupBox2.Font = new Font("Arial", 11F, FontStyle.Bold);
            this.groupBox2.Location = new System.Drawing.Point(834, 82);
            this.groupBox2.Size = new System.Drawing.Size(385, 178);
            this.groupBox2.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox2);

            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Text = "Pending Reservation";
            this.groupBox3.Font = new Font("Arial", 11F, FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(1238, 82);
            this.groupBox3.Size = new System.Drawing.Size(385, 178);
            this.groupBox3.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox3);

            this.label3.Name = "label3";
            this.label3.Text = "0";
            this.label3.Font = new Font("Arial", 32F, FontStyle.Bold);
            this.label3.AutoSize = false;
            this.label3.Location = new System.Drawing.Point(23, 52);
            this.label3.Size = new System.Drawing.Size(335, 90);
            this.label3.TextAlign = ContentAlignment.MiddleCenter;
            this.groupBoxTotalReservation.Controls.Add(this.label3);

            this.label4.Name = "label4";
            this.label4.Text = "0";
            this.label4.Font = new Font("Arial", 32F, FontStyle.Bold);
            this.label4.AutoSize = false;
            this.label4.Location = new System.Drawing.Point(23, 52);
            this.label4.Size = new System.Drawing.Size(335, 90);
            this.label4.TextAlign = ContentAlignment.MiddleCenter;
            this.groupBox1.Controls.Add(this.label4);

            this.label5.Name = "label5";
            this.label5.Text = "0";
            this.label5.Font = new Font("Arial", 32F, FontStyle.Bold);
            this.label5.AutoSize = false;
            this.label5.Location = new System.Drawing.Point(23, 52);
            this.label5.Size = new System.Drawing.Size(335, 90);
            this.label5.TextAlign = ContentAlignment.MiddleCenter;
            this.groupBox2.Controls.Add(this.label5);

            this.label6.Name = "label6";
            this.label6.Text = "0";
            this.label6.Font = new Font("Arial", 32F, FontStyle.Bold);
            this.label6.AutoSize = false;
            this.label6.Location = new System.Drawing.Point(23, 52);
            this.label6.Size = new System.Drawing.Size(335, 90);
            this.label6.TextAlign = ContentAlignment.MiddleCenter;
            this.groupBox3.Controls.Add(this.label6);

            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Text = "Latest Activity";
            this.groupBox4.Font = new Font("Arial", 11F, FontStyle.Bold);
            this.groupBox4.Location = new System.Drawing.Point(26, 280);
            this.groupBox4.Size = new System.Drawing.Size(1597, 624);
            this.groupBox4.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.groupBox4.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox4);

            this.listBox1.Name = "listBox1";
            this.listBox1.Location = new System.Drawing.Point(23, 38);
            this.listBox1.Size = new System.Drawing.Size(1547, 560);
            this.listBox1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
            this.listBox1.Font = new Font("Arial", 11F);
            this.listBox1.BackColor = Color.FromArgb(240, 240, 240);
            this.groupBox4.Controls.Add(this.listBox1);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
