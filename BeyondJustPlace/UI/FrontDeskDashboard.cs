using System;
using System.Windows.Forms;

namespace UI
{
    public partial class FrontDeskDashboard : Form
    {
        public FrontDeskDashboard()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}