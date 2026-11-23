
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BusinessLogic.Repository;

namespace UI
{
    public partial class Login : Form
    {
        // Preserve the 1920x1040 layout but fit it to smaller PC monitors.
        private readonly Dictionary<Control, Rectangle> baseBounds =
            new Dictionary<Control, Rectangle>();
        private readonly Dictionary<Control, Font> baseFonts =
            new Dictionary<Control, Font>();

        public Login()
        {
            InitializeComponent();

            txtPassword.UseSystemPasswordChar = true;
            chkShowPassword.CheckedChanged += (s,e)=>txtPassword.UseSystemPasswordChar=!chkShowPassword.Checked;
            btnExit.Click += (s,e)=>Close();
            try
            {
                this.Icon=System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                pictureLogo.Image=this.Icon.ToBitmap();
            }
            catch { }

            btnLogin.Click -= btnLogin_Click;
            btnLogin.Click += btnLogin_Click;

            // Customer-facing booking flow uses the same Windows application and LocalDB.
            // It does not require a staff account and does not modify existing dashboards.
            Button browseRooms = new Button
            {
                Name = "btnBrowseGuestRooms",
                Text = "Browse Rooms & Make a Reservation",
                Font = new Font("Arial", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(73, 49, 130),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                TabIndex = 5,
                Location = new Point(843, 857),
                Size = new Size(670, 57)
            };
            browseRooms.Click += (s, e) =>
            {
                using (GuestRoomAvailabilityForm booking = new GuestRoomAvailabilityForm())
                {
                    booking.ShowDialog(this);
                }
            };
            Controls.Add(browseRooms);

            foreach (Control control in Controls)
            {
                baseBounds[control] = control.Bounds;
                baseFonts[control] = control.Font;
            }
            Resize += (s, e) => ApplyResponsiveLayout();
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            if (ClientSize.Width < 1 || ClientSize.Height < 1) return;

            float sx = ClientSize.Width / 1920f;
            float sy = ClientSize.Height / 1040f;
            float textScale = Math.Min(sx, sy);

            SuspendLayout();
            try
            {
                foreach (Control control in Controls)
                {
                    Rectangle original = baseBounds[control];
                    control.SetBounds(
                        (int)Math.Round(original.X * sx),
                        (int)Math.Round(original.Y * sy),
                        Math.Max(12, (int)Math.Round(original.Width * sx)),
                        Math.Max(12, (int)Math.Round(original.Height * sy)));

                    Font originalFont = baseFonts[control];
                    if (originalFont != null)
                    {
                        float newSize = Math.Max(8f, originalFont.Size * textScale);
                        if (Math.Abs(control.Font.Size - newSize) > 0.25f)
                            control.Font = new Font(originalFont.FontFamily, newSize,
                                originalFont.Style, originalFont.Unit);
                    }
                }
            }
            finally { ResumeLayout(true); }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (username == "" || password == "")
            {
                MessageBox.Show(
                    "Please enter your username and password.","Login Validation");
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
                        "Invalid username, password, or inactive account.","Login Denied");
                    return;
                }

                CurrentUserSession.Username = username;
                CurrentUserSession.Role = role;

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
            CurrentUserSession.Clear();

            txtUsername.Clear();
            txtPassword.Clear();

            Show();
            Activate();
            txtUsername.Focus();
        }
    }
}
