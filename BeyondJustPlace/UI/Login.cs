using System;
using System.Windows.Forms;

namespace UI
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter username and password.");
            }
            else if (cbAdmin.Checked &&
                     username == "Admin" &&
                     password == "admin123")
            {
                MessageBox.Show("Login successful!");

                AdminDashboard admin = new AdminDashboard();
                admin.Show();
                this.Hide();
            }
            else if (cbFrontDesk.Checked &&
                     username == "FrontDesk" &&
                     password == "front123")
            {
                MessageBox.Show("Login successful!");

                FrontDeskDashboard frontDesk = new FrontDeskDashboard();
                frontDesk.Show();
                this.Hide();
            }
            else if (!cbAdmin.Checked && !cbFrontDesk.Checked)
            {
                MessageBox.Show("Please select Admin or Front Desk.");
            }
            else
            {
                MessageBox.Show("Username or password is incorrect.");
            }
        }

        private void chkAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAdmin.Checked)
            {
                cbFrontDesk.Checked = false;
            }
        }

        private void chkFrontDesk_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFrontDesk.Checked)
            {
                cbAdmin.Checked = false;
            }
        }
    }
}