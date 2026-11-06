
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

            txtPassword.UseSystemPasswordChar = true;

            btnLogin.Click -= btnLogin_Click;
            btnLogin.Click += btnLogin_Click;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (username == "" || password == "")
            {
                MessageBox.Show(
                    "Please enter your username and password.");
                return;
            }

            try
            {
                btnLogin.Enabled = false;

                UserRepository repository = new UserRepository();
                string role = repository.Login(username, password);

                role = (role ?? "")
                    .Replace(" ", "")
                    .Trim();

                Form dashboard;

                if (role.Equals("Admin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    dashboard = new AdminDashboard();
                }
                else if (role.Equals("FrontDesk",
                    StringComparison.OrdinalIgnoreCase))
                {
                    dashboard = new FrontDeskDashboard();
                }
                else
                {
                    MessageBox.Show(
                        "Invalid username or password.");
                    return;
                }

                dashboard.FormClosed += Dashboard_FormClosed;

                dashboard.Show();
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to log in:\n" + ex.Message,
                    "Login Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void Dashboard_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();

            Show();
            Activate();
            txtUsername.Focus();
        }
    }
}
