using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnLogin;
        private Button btnExit;
        private CheckBox chkShowPassword;
        private Splitter splitter1;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label lblWelcome;
        private Label lblSubtitle;
        private Label lblHelpful;
        private PictureBox pictureLogo;
        private TextBox txtUsername;
        private TextBox txtPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components!=null)components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnLogin=new Button();
            this.btnExit=new Button();
            this.chkShowPassword=new CheckBox();
            this.splitter1=new Splitter();
            this.label1=new Label();
            this.label2=new Label();
            this.label4=new Label();
            this.label5=new Label();
            this.label6=new Label();
            this.lblWelcome=new Label();
            this.lblSubtitle=new Label();
            this.lblHelpful=new Label();
            this.pictureLogo=new PictureBox();
            this.txtUsername=new TextBox();
            this.txtPassword=new TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).BeginInit();
            this.SuspendLayout();

            // Purple hotel branding panel.
            this.splitter1.BackColor=Color.FromArgb(104,88,180);
            this.splitter1.Dock=DockStyle.Left;
            this.splitter1.Location=new Point(0,0);
            this.splitter1.Size=new Size(575,1040);
            this.splitter1.TabStop=false;
            this.splitter1.Name="splitter1";

            this.pictureLogo.BackColor=Color.FromArgb(104,88,180);
            this.pictureLogo.Location=new Point(215,220);
            this.pictureLogo.Name="pictureLogo";
            this.pictureLogo.Size=new Size(145,145);
            this.pictureLogo.SizeMode=PictureBoxSizeMode.Zoom;

            this.label4.BackColor=Color.FromArgb(104,88,180);
            this.label4.ForeColor=Color.White;
            this.label4.Font=new Font("Arial",32F,FontStyle.Bold);
            this.label4.TextAlign=ContentAlignment.MiddleCenter;
            this.label4.Location=new Point(30,410);
            this.label4.Size=new Size(515,66);
            this.label4.Name="label4";
            this.label4.Text="BEYOND JUST";

            this.label5.BackColor=Color.FromArgb(104,88,180);
            this.label5.ForeColor=Color.White;
            this.label5.Font=new Font("Arial",39F,FontStyle.Bold);
            this.label5.TextAlign=ContentAlignment.MiddleCenter;
            this.label5.Location=new Point(30,480);
            this.label5.Size=new Size(515,82);
            this.label5.Name="label5";
            this.label5.Text="PLACE";

            this.label6.BackColor=Color.FromArgb(104,88,180);
            this.label6.ForeColor=Color.White;
            this.label6.Font=new Font("Arial",13F,FontStyle.Regular);
            this.label6.TextAlign=ContentAlignment.MiddleCenter;
            this.label6.Location=new Point(45,578);
            this.label6.Size=new Size(485,68);
            this.label6.Name="label6";
            this.label6.Text="Hotel Reservation and Billing System\nManage every stay with care.";

            this.lblWelcome.Font=new Font("Arial",27F,FontStyle.Bold);
            this.lblWelcome.ForeColor=Color.FromArgb(73,49,130);
            this.lblWelcome.Location=new Point(835,275);
            this.lblWelcome.Size=new Size(790,60);
            this.lblWelcome.Text="Welcome back";
            this.lblWelcome.Name="lblWelcome";

            this.lblSubtitle.Font=new Font("Arial",14F);
            this.lblSubtitle.ForeColor=Color.FromArgb(89,84,101);
            this.lblSubtitle.Location=new Point(839,346);
            this.lblSubtitle.Size=new Size(700,55);
            this.lblSubtitle.Text="Sign in to keep your guests comfortable and your hotel running smoothly.";
            this.lblSubtitle.Name="lblSubtitle";

            this.label1.Font=new Font("Arial",12F,FontStyle.Bold);
            this.label1.Location=new Point(843,434);
            this.label1.Size=new Size(600,30);
            this.label1.Text="Username";
            this.label1.Name="label1";
            this.txtUsername.Font=new Font("Arial",17F);
            this.txtUsername.Location=new Point(843,468);
            this.txtUsername.Size=new Size(670,41);
            this.txtUsername.Name="txtUsername";
            this.txtUsername.TabIndex=0;

            this.label2.Font=new Font("Arial",12F,FontStyle.Bold);
            this.label2.Location=new Point(843,535);
            this.label2.Size=new Size(600,30);
            this.label2.Text="Password";
            this.label2.Name="label2";
            this.txtPassword.Font=new Font("Arial",17F);
            this.txtPassword.Location=new Point(843,569);
            this.txtPassword.Size=new Size(670,41);
            this.txtPassword.Name="txtPassword";
            this.txtPassword.UseSystemPasswordChar=true;
            this.txtPassword.TabIndex=1;

            this.chkShowPassword.Location=new Point(843,626);
            this.chkShowPassword.Size=new Size(190,30);
            this.chkShowPassword.Text="Show password";
            this.chkShowPassword.Name="chkShowPassword";
            this.chkShowPassword.Font=new Font("Arial",11F);
            this.chkShowPassword.TabIndex=2;

            this.btnLogin.BackColor=Color.FromArgb(128,128,255);
            this.btnLogin.ForeColor=Color.Black;
            this.btnLogin.Font=new Font("Arial",14F,FontStyle.Bold);
            this.btnLogin.Location=new Point(843,703);
            this.btnLogin.Size=new Size(350,51);
            this.btnLogin.Text="Login";
            this.btnLogin.Name="btnLogin";
            this.btnLogin.UseVisualStyleBackColor=false;
            this.btnLogin.TabIndex=3;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            this.btnExit.BackColor=Color.FromArgb(224,224,224);
            this.btnExit.Font=new Font("Arial",13F,FontStyle.Bold);
            this.btnExit.Location=new Point(1210,703);
            this.btnExit.Size=new Size(303,51);
            this.btnExit.Name="btnExit";
            this.btnExit.Text="Exit";
            this.btnExit.UseVisualStyleBackColor=false;
            this.btnExit.TabIndex=4;

            this.lblHelpful.Font=new Font("Arial",10F);
            this.lblHelpful.ForeColor=Color.FromArgb(99,95,108);
            this.lblHelpful.Location=new Point(843,773);
            this.lblHelpful.Size=new Size(650,57);
            this.lblHelpful.Text="Authorized hotel staff only. Your access level is determined by your account.";
            this.lblHelpful.Name="lblHelpful";

            this.AutoScaleMode=AutoScaleMode.Font;
            this.BackColor=Color.FromArgb(239,237,245);
            this.ClientSize=new Size(1920,1040);
            this.StartPosition=FormStartPosition.CenterScreen;
            this.WindowState=FormWindowState.Maximized;
            this.Text="Beyond Just Place - Staff Login";
            this.Name="Login";
            this.Controls.Add(this.pictureLogo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lblHelpful);
            this.Controls.Add(this.splitter1);
            this.AcceptButton=this.btnLogin;
            this.CancelButton=this.btnExit;
            ((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
