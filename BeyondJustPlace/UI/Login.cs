using System;
using System.Windows.Forms;
using BusinessLogic.Repository;

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
            if (!cbAdmin.Checked && !cbFrontDesk.Checked)
            {
                MessageBox.Show("Please select Admin or Front Desk.");
                return;
            }

            UserRepository user = new UserRepository();

            string role = user.Login(txtUsername.Text, txtPassword.Text);

            if (role == "Admin" && cbAdmin.Checked)
            {
                AdminDashboard admin = new AdminDashboard();
                admin.Show();
                this.Hide();
            }
            else if (role == "FrontDesk" && cbFrontDesk.Checked)
            {
                FrontDeskDashboard frontDesk = new FrontDeskDashboard();
                frontDesk.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username, password, or role.");
            }
        }

        private void cbAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAdmin.Checked)
            {
                cbFrontDesk.Checked = false;
            }
        }

        private void cbFrontDesk_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFrontDesk.Checked)
            {
                cbAdmin.Checked = false;
            }
        }
    }
}