
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
            this.ClientSize = new Size(1160, 710);
            this.AutoScroll = true;
            this.Text = "Front Desk Home";
            this.Name = "FrontDeskHome";

            this.label2.Name = "label2";
            this.label2.Text = "Dashboard - Front Desk";
            this.label2.Font = new Font("Arial", 16F, FontStyle.Bold);
            this.label2.AutoSize = true;
            this.label2.Location = new Point(430, 16);
            this.Controls.Add(this.label2);

            this.groupBoxTotalReservation.Name = "groupBoxTotalReservation";
            this.groupBoxTotalReservation.Text = "Today Reservation";
            this.groupBoxTotalReservation.Location = new Point(20, 65);
            this.groupBoxTotalReservation.Size = new Size(266, 140);
            this.groupBoxTotalReservation.BackColor =
                Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBoxTotalReservation);

            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Text = "Current Check-in Guest";
            this.groupBox1.Location = new Point(300, 65);
            this.groupBox1.Size = new Size(266, 140);
            this.groupBox1.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox1);

            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Text = "Expected Check-ins Today";
            this.groupBox2.Location = new Point(580, 65);
            this.groupBox2.Size = new Size(266, 140);
            this.groupBox2.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox2);

            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Text = "Pending Reservation";
            this.groupBox3.Location = new Point(860, 65);
            this.groupBox3.Size = new Size(266, 140);
            this.groupBox3.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox3);

            this.label3.Name = "label3";
            this.label3.Text = "0";
            this.label3.Font = new Font("Arial", 36F, FontStyle.Bold);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(105, 47);
            this.groupBoxTotalReservation.Controls.Add(this.label3);

            this.label4.Name = "label4";
            this.label4.Text = "0";
            this.label4.Font = new Font("Arial", 36F, FontStyle.Bold);
            this.label4.AutoSize = true;
            this.label4.Location = new Point(105, 47);
            this.groupBox1.Controls.Add(this.label4);

            this.label5.Name = "label5";
            this.label5.Text = "0";
            this.label5.Font = new Font("Arial", 36F, FontStyle.Bold);
            this.label5.AutoSize = true;
            this.label5.Location = new Point(105, 47);
            this.groupBox2.Controls.Add(this.label5);

            this.label6.Name = "label6";
            this.label6.Text = "0";
            this.label6.Font = new Font("Arial", 36F, FontStyle.Bold);
            this.label6.AutoSize = true;
            this.label6.Location = new Point(105, 47);
            this.groupBox3.Controls.Add(this.label6);

            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Text = "Latest Activity";
            this.groupBox4.Location = new Point(20, 220);
            this.groupBox4.Size = new Size(1106, 445);
            this.groupBox4.BackColor = Color.FromArgb(128, 128, 255);
            this.Controls.Add(this.groupBox4);

            this.listBox1.Name = "listBox1";
            this.listBox1.Location = new Point(20, 35);
            this.listBox1.Size = new Size(1065, 385);
            this.listBox1.Font = new Font("Arial", 11F);
            this.listBox1.BackColor = Color.FromArgb(240, 240, 240);
            this.groupBox4.Controls.Add(this.listBox1);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
