using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    public partial class AdminDashboard : Form
    {
        private readonly AdminDataService adminService = new AdminDataService();
        private readonly Timer refreshTimer = new Timer();

        public AdminDashboard()
        {
            InitializeComponent();

            WindowState = FormWindowState.Maximized;
            refreshTimer.Interval = 5000;
            refreshTimer.Tick += (s, e) => RefreshDashboard(false);

            btnDashboard.Click -= BtnDashboard_Click;
            btnDashboard.Click += BtnDashboard_Click;

            button1.Click -= ManageRooms_Click;
            button1.Click += ManageRooms_Click;
            button2.Click -= ManageUsers_Click;
            button2.Click += ManageUsers_Click;
            button3.Click -= ViewReservations_Click;
            button3.Click += ViewReservations_Click;
            button4.Click -= ViewBillingRecords_Click;
            button4.Click += ViewBillingRecords_Click;
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            RefreshDashboard(true);
            refreshTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            refreshTimer.Stop();
            refreshTimer.Dispose();
            base.OnFormClosed(e);
        }

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            RefreshDashboard(true);
        }

        private void RefreshDashboard(bool showError)
        {
            try
            {
                AdminDashboardSnapshot snapshot = adminService.GetDashboardSnapshot();
                label3.Text = snapshot.TotalReservations.ToString();
                label4.Text = snapshot.CheckedIn.ToString();
                label5.Text = snapshot.PendingReservations.ToString();
                label6.Text = "PHP " + snapshot.TotalRevenue.ToString("N2");

                listBoxLatestActivity.BeginUpdate();
                try
                {
                    listBoxLatestActivity.Items.Clear();
                    foreach (DataRow row in snapshot.LatestActivity.Rows)
                    {
                        listBoxLatestActivity.Items.Add(
                            "Reservation #" + row["Reservation ID"] +
                            " - " + row["Guest"] +
                            " - Room " + row["Room"] +
                            " - " + row["Status"]);
                    }
                }
                finally
                {
                    listBoxLatestActivity.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                if (showError)
                    MessageBox.Show(ex.Message, "Admin Dashboard Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ManageRooms_Click(object sender, EventArgs e)
        {
            ShowRoomManager();
        }

        private void ManageUsers_Click(object sender, EventArgs e)
        {
            ShowUserManager();
        }

        private void ViewReservations_Click(object sender, EventArgs e)
        {
            ShowReadOnlyTable("Reservation Records", adminService.GetReservations());
        }

        private void ViewBillingRecords_Click(object sender, EventArgs e)
        {
            ShowReadOnlyTable("Billing Records", adminService.GetPayments());
        }

        private void ShowRoomManager()
        {
            Form form = CreateDialog("Manage Rooms", 1050, 620);
            DataGridView grid = CreateGrid();
            grid.Dock = DockStyle.Fill;

            FlowLayoutPanel actions = CreateActionBar();
            Button add = CreateActionButton("Add Room");
            Button edit = CreateActionButton("Edit Selected");
            Button delete = CreateActionButton("Delete Selected");
            Button refresh = CreateActionButton("Refresh");
            Button close = CreateActionButton("Close");
            actions.Controls.AddRange(new Control[] { add, edit, delete, refresh, close });

            form.Controls.Add(grid);
            form.Controls.Add(actions);

            Action load = () => grid.DataSource = adminService.GetRooms();
            add.Click += (s, e) =>
            {
                RoomEditorData data = ShowRoomEditor(null);
                if (data == null) return;
                try
                {
                    adminService.AddRoom(data.RoomNumber, data.RoomType, data.Rate,
                        data.Capacity, data.ReservationFee, data.Status);
                    load(); RefreshDashboard(false);
                }
                catch (Exception ex) { ShowAdminError(ex); }
            };
            edit.Click += (s, e) =>
            {
                if (grid.CurrentRow == null) { MessageBox.Show("Select a room first."); return; }
                DataRowView view = grid.CurrentRow.DataBoundItem as DataRowView;
                if (view == null) return;
                RoomEditorData existing = new RoomEditorData
                {
                    RoomId = Convert.ToInt32(view["RoomId"]),
                    RoomNumber = Convert.ToString(view["RoomNumber"]),
                    RoomType = Convert.ToString(view["RoomType"]),
                    Rate = Convert.ToDecimal(view["RoomRate"]),
                    Capacity = view["RoomCapacity"] == DBNull.Value ? 1 : Convert.ToInt32(view["RoomCapacity"]),
                    ReservationFee = view["ReservationFee"] == DBNull.Value ? 0m : Convert.ToDecimal(view["ReservationFee"]),
                    Status = Convert.ToString(view["RoomStatus"])
                };
                RoomEditorData data = ShowRoomEditor(existing);
                if (data == null) return;
                try
                {
                    adminService.UpdateRoom(existing.RoomId, data.RoomNumber, data.RoomType,
                        data.Rate, data.Capacity, data.ReservationFee, data.Status);
                    load(); RefreshDashboard(false);
                }
                catch (Exception ex) { ShowAdminError(ex); }
            };
            delete.Click += (s, e) =>
            {
                if (grid.CurrentRow == null) { MessageBox.Show("Select a room first."); return; }
                DataRowView view = grid.CurrentRow.DataBoundItem as DataRowView;
                if (view == null) return;
                if (MessageBox.Show("Delete room " + view["RoomNumber"] + "?", "Delete Room",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                try
                {
                    adminService.DeleteRoom(Convert.ToInt32(view["RoomId"]));
                    load(); RefreshDashboard(false);
                }
                catch (Exception ex) { ShowAdminError(ex); }
            };
            refresh.Click += (s, e) => load();
            close.Click += (s, e) => form.Close();
            load();
            form.ShowDialog(this);
        }

        private void ShowUserManager()
        {
            Form form = CreateDialog("Manage Users", 850, 560);
            DataGridView grid = CreateGrid();
            grid.Dock = DockStyle.Fill;

            FlowLayoutPanel actions = CreateActionBar();
            Button add = CreateActionButton("Add User");
            Button edit = CreateActionButton("Edit Selected");
            Button delete = CreateActionButton("Delete Selected");
            Button refresh = CreateActionButton("Refresh");
            Button close = CreateActionButton("Close");
            actions.Controls.AddRange(new Control[] { add, edit, delete, refresh, close });
            form.Controls.Add(grid); form.Controls.Add(actions);

            Action load = () => grid.DataSource = adminService.GetUsers();
            add.Click += (s, e) =>
            {
                UserEditorData data = ShowUserEditor(null);
                if (data == null) return;
                try { adminService.AddUser(data.Username, data.Password, data.Role); load(); }
                catch (Exception ex) { ShowAdminError(ex); }
            };
            edit.Click += (s, e) =>
            {
                if (grid.CurrentRow == null) { MessageBox.Show("Select a user first."); return; }
                DataRowView view = grid.CurrentRow.DataBoundItem as DataRowView;
                if (view == null) return;
                UserEditorData existing = new UserEditorData
                {
                    UserId = Convert.ToInt32(view["UserId"]),
                    Username = Convert.ToString(view["Username"]),
                    Role = Convert.ToString(view["Role"])
                };
                UserEditorData data = ShowUserEditor(existing);
                if (data == null) return;
                try { adminService.UpdateUser(existing.UserId, data.Username, data.Password, data.Role); load(); }
                catch (Exception ex) { ShowAdminError(ex); }
            };
            delete.Click += (s, e) =>
            {
                if (grid.CurrentRow == null) { MessageBox.Show("Select a user first."); return; }
                DataRowView view = grid.CurrentRow.DataBoundItem as DataRowView;
                if (view == null) return;
                if (MessageBox.Show("Delete user " + view["Username"] + "?", "Delete User",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                try { adminService.DeleteUser(Convert.ToInt32(view["UserId"])); load(); }
                catch (Exception ex) { ShowAdminError(ex); }
            };
            refresh.Click += (s, e) => load();
            close.Click += (s, e) => form.Close();
            load();
            form.ShowDialog(this);
        }

        private RoomEditorData ShowRoomEditor(RoomEditorData existing)
        {
            Form form = CreateDialog(existing == null ? "Add Room" : "Edit Room", 500, 420);
            TableLayoutPanel table = CreateEditorTable(6);
            TextBox number = AddEditorText(table, 0, "Room Number:", existing == null ? "" : existing.RoomNumber);
            TextBox type = AddEditorText(table, 1, "Room Type:", existing == null ? "" : existing.RoomType);
            NumericUpDown rate = AddEditorMoney(table, 2, "Room Rate:", existing == null ? 0m : existing.Rate);
            NumericUpDown capacity = AddEditorNumber(table, 3, "Capacity:", existing == null ? 2 : existing.Capacity);
            NumericUpDown fee = AddEditorMoney(table, 4, "Reservation Fee:", existing == null ? 0m : existing.ReservationFee);
            ComboBox status = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 11F) };
            status.Items.AddRange(new object[] { "Available", "Maintenance", "Unavailable" });
            status.SelectedItem = existing == null ? "Available" : existing.Status;
            AddEditorControl(table, 5, "Status:", status);

            FlowLayoutPanel actions = CreateActionBar();
            Button save = CreateActionButton("Save");
            Button cancel = CreateActionButton("Cancel");
            actions.Controls.AddRange(new Control[] { save, cancel });
            form.Controls.Add(table); form.Controls.Add(actions);
            RoomEditorData result = null;
            save.Click += (s, e) =>
            {
                result = new RoomEditorData
                {
                    RoomNumber = number.Text.Trim(), RoomType = type.Text.Trim(),
                    Rate = rate.Value, Capacity = Convert.ToInt32(capacity.Value),
                    ReservationFee = fee.Value, Status = Convert.ToString(status.SelectedItem)
                };
                form.DialogResult = DialogResult.OK; form.Close();
            };
            cancel.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };
            return form.ShowDialog(this) == DialogResult.OK ? result : null;
        }

        private UserEditorData ShowUserEditor(UserEditorData existing)
        {
            Form form = CreateDialog(existing == null ? "Add User" : "Edit User", 470, 300);
            TableLayoutPanel table = CreateEditorTable(3);
            TextBox username = AddEditorText(table, 0, "Username:", existing == null ? "" : existing.Username);
            TextBox password = AddEditorText(table, 1,
                existing == null ? "Password:" : "New Password:", "");
            password.UseSystemPasswordChar = true;
            ComboBox role = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 11F) };
            role.Items.AddRange(new object[] { "Admin", "FrontDesk" });
            role.SelectedItem = existing == null ? "FrontDesk" : existing.Role;
            AddEditorControl(table, 2, "Role:", role);

            FlowLayoutPanel actions = CreateActionBar();
            Button save = CreateActionButton("Save"); Button cancel = CreateActionButton("Cancel");
            actions.Controls.AddRange(new Control[] { save, cancel });
            form.Controls.Add(table); form.Controls.Add(actions);
            UserEditorData result = null;
            save.Click += (s, e) =>
            {
                result = new UserEditorData
                {
                    Username = username.Text.Trim(), Password = password.Text,
                    Role = Convert.ToString(role.SelectedItem)
                };
                form.DialogResult = DialogResult.OK; form.Close();
            };
            cancel.Click += (s, e) => { form.DialogResult = DialogResult.Cancel; form.Close(); };
            return form.ShowDialog(this) == DialogResult.OK ? result : null;
        }

        private void ShowReadOnlyTable(string title, DataTable table)
        {
            Form form = CreateDialog(title, 1200, 680);
            DataGridView grid = CreateGrid();
            grid.ReadOnly = true; grid.DataSource = table; grid.Dock = DockStyle.Fill;
            FlowLayoutPanel actions = CreateActionBar();
            Button close = CreateActionButton("Close");
            actions.Controls.Add(close); close.Click += (s, e) => form.Close();
            form.Controls.Add(grid); form.Controls.Add(actions);
            form.ShowDialog(this);
        }

        private static Form CreateDialog(string title, int width, int height)
        {
            return new Form
            {
                Text = title, Width = width, Height = height,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(224, 224, 224),
                Font = new Font("Arial", 10F), MinimizeBox = false, MaximizeBox = false
            };
        }

        private static DataGridView CreateGrid()
        {
            return new DataGridView
            {
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(224, 224, 224),
                Dock = DockStyle.Fill
            };
        }

        private static FlowLayoutPanel CreateActionBar()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(128, 128, 255)
            };
        }

        private static Button CreateActionButton(string text)
        {
            return new Button
            {
                Text = text, Width = 145, Height = 36,
                Font = new Font("Arial", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(224, 224, 224), UseVisualStyleBackColor = false
            };
        }

        private static TableLayoutPanel CreateEditorTable(int rows)
        {
            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, Padding = new Padding(18), ColumnCount = 2, RowCount = rows
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < rows; i++) table.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            return table;
        }

        private static TextBox AddEditorText(TableLayoutPanel table, int row, string caption, string value)
        {
            TextBox control = new TextBox { Dock = DockStyle.Fill, Text = value, Font = new Font("Arial", 11F) };
            AddEditorControl(table, row, caption, control); return control;
        }

        private static NumericUpDown AddEditorMoney(TableLayoutPanel table, int row, string caption, decimal value)
        {
            NumericUpDown control = new NumericUpDown
            {
                Dock = DockStyle.Fill, DecimalPlaces = 2, Maximum = 1000000, Value = Math.Max(0, value),
                ThousandsSeparator = true, Font = new Font("Arial", 11F)
            };
            AddEditorControl(table, row, caption, control); return control;
        }

        private static NumericUpDown AddEditorNumber(TableLayoutPanel table, int row, string caption, int value)
        {
            NumericUpDown control = new NumericUpDown
            {
                Dock = DockStyle.Fill, Minimum = 1, Maximum = 50, Value = Math.Max(1, value),
                Font = new Font("Arial", 11F)
            };
            AddEditorControl(table, row, caption, control); return control;
        }

        private static void AddEditorControl(TableLayoutPanel table, int row, string caption, Control control)
        {
            Label label = new Label
            {
                Text = caption, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 10F, FontStyle.Bold)
            };
            table.Controls.Add(label, 0, row); table.Controls.Add(control, 1, row);
        }

        private static void ShowAdminError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Admin Operation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private sealed class RoomEditorData
        {
            public int RoomId { get; set; }
            public string RoomNumber { get; set; }
            public string RoomType { get; set; }
            public decimal Rate { get; set; }
            public int Capacity { get; set; }
            public decimal ReservationFee { get; set; }
            public string Status { get; set; }
        }

        private sealed class UserEditorData
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }
    }
}
