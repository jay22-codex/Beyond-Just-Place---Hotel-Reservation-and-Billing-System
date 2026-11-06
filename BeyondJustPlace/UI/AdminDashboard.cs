using System;
using System.Windows.Forms;

namespace UI
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
            // Admin layout is preserved. Other Admin modules are not supplied yet.
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close(); // Original Login form is shown by Login.Dashboard_FormClosed.
            }
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
        }
    }
}