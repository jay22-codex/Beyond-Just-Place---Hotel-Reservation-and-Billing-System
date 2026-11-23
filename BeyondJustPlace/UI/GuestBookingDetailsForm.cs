using System;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class GuestBookingDetailsForm : Form
    {
        private readonly GuestBookingSelection selection;
        private readonly ReservationDetailsService reservations = new ReservationDetailsService();

        internal GuestBookingDetailsForm(GuestBookingSelection selection)
        {
            if (selection == null || selection.Rooms == null || selection.Rooms.Count == 0)
                throw new ArgumentException("A room selection is required.", "selection");
            this.selection = selection;
            InitializeComponent();
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            btnBack.Click += (s, e) => Close();
            btnSubmit.Click += Submit_Click;
            Load += (s, e) => ShowSummary();
        }

        private void ShowSummary()
        {
            string[] types = selection.Rooms.Select(r => r.RoomType).Distinct().ToArray();
            lblRoomTitle.Text = types.Length == 1 ? types[0] + " Room" : "Your selected rooms";
            photo.Image = GuestBookingLook.Photo(selection.Rooms[0].RoomType);
            lblSpecs.Text = selection.Rooms.Count + " room(s)  •  " +
                selection.Capacity + " guest capacity  •  " +
                selection.Guests + " guest(s)";
            string desc = string.Join("  /  ",
                selection.Rooms.Select(r => r.Description).Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct().ToArray());
            lblDescription.Text = desc.Length == 0 ? "Room details are managed by the hotel." : desc;
            lblDates.Text = selection.CheckIn.ToString("MMM dd, yyyy") + "  →  " +
                selection.CheckOut.ToString("MMM dd, yyyy") +
                "  •  " + selection.Nights + " night(s)";
            lbRooms.Items.Clear();
            foreach (AvailableRoomInfo room in selection.Rooms)
            {
                lbRooms.Items.Add("Room " + room.RoomNumber + "  •  " + room.RoomType +
                    "  •  " + GuestBookingLook.Money(room.RoomRate) + "/night");
            }
            lblCost.Text = "Estimated room total: " + GuestBookingLook.Money(selection.Total);
            lblDeposit.Text = "Reservation fee/deposit (payable later): " +
                GuestBookingLook.Money(selection.DepositEstimate);
            lblBeds.Text = "Beds across rooms: " +
                selection.Rooms.Sum(r => r.BedCount) + "  •  Beds and room size as configured by Admin";
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            string guestName = txtName.Text.Trim();
            string contact = txtContact.Text.Trim();
            string email = txtEmail.Text.Trim();
            string idNumber = txtIdNumber.Text.Trim();
            if (guestName.Length < 2 || contact.Length < 5)
            {
                MessageBox.Show("Enter the guest's full name and valid contact number.",
                    "Guest information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                MailAddress address = new MailAddress(email);
                if (!string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase))
                    throw new FormatException();
            }
            catch
            {
                MessageBox.Show("Enter a valid email address.", "Guest information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbIdType.SelectedIndex < 0 || idNumber.Length < 3)
            {
                MessageBox.Show("Choose an ID type and enter the ID number. Hotel staff will verify it at check-in.",
                    "Guest identification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!chkConsent.Checked)
            {
                MessageBox.Show("Please acknowledge the Pending reservation status.",
                    "Reservation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                btnSubmit.Enabled = false;
                Cursor = Cursors.WaitCursor;
                Guest guest = reservations.SaveGuest(new Guest
                {
                    GuestName = guestName,
                    ContactNumber = contact,
                    Email = email,
                    HomeAddress = txtAddress.Text.Trim(),
                    IdType = cmbIdType.Text,
                    IdNumber = idNumber
                });
                int number = reservations.CreateReservation(
                    guest.GuestId,
                    selection.Rooms,
                    selection.CheckIn,
                    selection.CheckOut,
                    selection.Guests,
                    selection.Adults,
                    selection.Children,
                    selection.Rooms.Count,
                    txtRequest.Text.Trim(),
                    null);
                MessageBox.Show(
                    "Reservation request submitted successfully!\n\n" +
                    "Reference: BJP-R" + number.ToString("D6") + "\n" +
                    "Guest: " + guestName + "\n" +
                    "Dates: " + selection.CheckIn.ToString("MMM dd") + " - " +
                        selection.CheckOut.ToString("MMM dd, yyyy") + "\n" +
                    "Rooms: " + selection.Rooms.Count + "\n" +
                    "Estimated room total: " + GuestBookingLook.Money(selection.Total) + "\n\n" +
                    "Status: PENDING\nThe hotel must confirm your request. No payment was collected.",
                    "Beyond Just Place - Booking reference",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reservation was not completed:\n" + ex.Message +
                    "\n\nRoom inventory may have changed; go back and check availability again.",
                    "Reservation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                if (!IsDisposed) btnSubmit.Enabled = true;
            }
        }
    }
}
