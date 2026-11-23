using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class WalkInCheckInForm : Form
    {
        private readonly ReservationDetailsService reservations = new ReservationDetailsService();
        private readonly HotelEnhancementService enhancements = new HotelEnhancementService();
        private readonly FrontDeskOperationsService checkin = new FrontDeskOperationsService();
        private readonly List<AvailableRoomInfo> availability = new List<AvailableRoomInfo>();
        private bool loading;
        public WalkInCheckInForm()
        {
            InitializeComponent();
            dtpIn.Value = DateTime.Today;
            dtpOut.Value = DateTime.Today.AddDays(1);
            cmbIdType.Items.AddRange(new object[]{"National ID","Passport","Driver's License","Student ID","Other"});
            cmbPayment.Items.AddRange(new object[]{"Cash","GCash","Card"});
            btnCheck.Click += (s,e) => LoadAvailable();
            btnCheckIn.Click += CheckInWalkIn;
            btnClose.Click += (s,e) => Close();
            dgvRooms.SelectionChanged += (s,e) => UpdateSelectionSummary();
            dtpIn.ValueChanged += (s,e) => ResetRoomSelection();
            dtpOut.ValueChanged += (s,e) => ResetRoomSelection();
            cmbRoomType.SelectedIndexChanged += (s,e) => ResetRoomSelection();
            numAdults.ValueChanged += (s,e) => UpdateSelectionSummary();
            numChildren.ValueChanged += (s,e) => UpdateSelectionSummary();
            numRoomCount.ValueChanged += (s,e) => UpdateSelectionSummary();
            Shown += (s,e) => LoadTypes();
        }
        private void LoadTypes()
        {
            try
            {
                loading=true;
                DataTable types=enhancements.GetRoomTypes(false);
                cmbRoomType.Items.Clear();
                foreach(DataRow row in types.Rows) cmbRoomType.Items.Add(Convert.ToString(row["TypeName"]));
                if(cmbRoomType.Items.Count>0)cmbRoomType.SelectedIndex=0;
            }
            catch(Exception ex){MessageBox.Show("Cannot load room types: "+ex.Message,"Walk-In");}
            finally{loading=false;}
            LoadAvailable();
        }
        private void ResetRoomSelection()
        {
            if(loading)return;
            availability.Clear();
            dgvRooms.Rows.Clear();
            lblSelection.Text="Room selection changed — click Check Available Rooms again.";
        }
        private void LoadAvailable()
        {
            if(cmbRoomType.SelectedIndex<0)return;
            if(dtpIn.Value.Date<DateTime.Today || dtpOut.Value.Date<=dtpIn.Value.Date)
            {MessageBox.Show("Check-in must be today or later; check-out must be after check-in.","Dates");return;}
            try
            {
                availability.Clear();
                dgvRooms.Rows.Clear();
                availability.AddRange(reservations.GetAvailableRooms(dtpIn.Value.Date,dtpOut.Value.Date,cmbRoomType.Text));
                foreach(AvailableRoomInfo room in availability)
                {
                    int i=dgvRooms.Rows.Add(room.RoomNumber,room.RoomCapacity.GetValueOrDefault(),
                        room.BedCount,room.RoomRate.ToString("N2"),room.Description);
                    dgvRooms.Rows[i].Tag=room;
                }
                dgvRooms.ClearSelection();
                UpdateSelectionSummary();
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"Room availability",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }
        private List<AvailableRoomInfo> SelectedRooms()
        {
            List<AvailableRoomInfo> chosen=new List<AvailableRoomInfo>();
            foreach(DataGridViewRow row in dgvRooms.SelectedRows)
                if(row.Tag is AvailableRoomInfo)chosen.Add((AvailableRoomInfo)row.Tag);
            return chosen.OrderBy(r=>r.RoomNumber).ToList();
        }
        private void UpdateSelectionSummary()
        {
            List<AvailableRoomInfo> rooms=SelectedRooms();
            int guests=(int)numAdults.Value+(int)numChildren.Value;
            int capacity=rooms.Sum(r=>r.RoomCapacity.GetValueOrDefault());
            decimal nightly=rooms.Sum(r=>r.RoomRate);
            int nights=Math.Max(1,(dtpOut.Value.Date-dtpIn.Value.Date).Days);
            lblSelection.Text="Selected: "+rooms.Count+" / "+numRoomCount.Value+
                " room(s)  •  Guest capacity: "+capacity+" / "+guests+
                "  •  Total: PHP "+(nightly*nights).ToString("N2")+
                (rooms.Count>0 && capacity<guests?"\r\nWARNING: Not enough room capacity for all guests.":"");
            lblSelection.ForeColor=rooms.Count>0 && capacity<guests?Color.Firebrick:Color.FromArgb(48,45,83);
        }
        private void CheckInWalkIn(object sender,EventArgs e)
        {
            if(!CurrentUserSession.RequireFrontDesk(this))return;
            List<AvailableRoomInfo> rooms=SelectedRooms();
            int count=(int)numRoomCount.Value;
            int adults=(int)numAdults.Value, children=(int)numChildren.Value, guests=adults+children;
            if(rooms.Count!=count)
            {MessageBox.Show("Select exactly "+count+" available room(s) from the table (hold Ctrl for multiple).","Rooms");return;}
            if(rooms.Any(r=>r.RoomCapacity.GetValueOrDefault()<1) || rooms.Sum(r=>r.RoomCapacity.GetValueOrDefault())<guests)
            {MessageBox.Show("The selected rooms cannot accommodate "+guests+" guests. Select larger/more rooms.","Guest capacity");return;}
            if(txtName.Text.Trim().Length<2 || txtContact.Text.Trim().Length<5)
            {MessageBox.Show("Enter a full name and valid contact number.","Guest details");return;}
            try { MailAddress parsed=new MailAddress(txtEmail.Text.Trim());if(parsed.Address!=txtEmail.Text.Trim())throw new FormatException(); }
            catch {MessageBox.Show("Enter a valid email address.","Guest email");return;}
            if(cmbIdType.SelectedIndex<0 || txtIdNumber.Text.Trim().Length<3 || !chkPhysicalId.Checked)
            {MessageBox.Show("Compare the physical ID with the entered ID number and tick the verification checkbox.","ID checker");return;}
            decimal fee=rooms.Sum(r=>r.ReservationFee.GetValueOrDefault());
            string method=null,reference=null;
            if(fee>0m)
            {
                if(cmbPayment.SelectedIndex<0)
                {MessageBox.Show("Choose the deposit payment method before checking in.","Reservation fee");return;}
                method=cmbPayment.Text;
                if(method=="GCash")
                {
                    reference=PaymentUiDialogs.RequestGcCashReference(this,fee,"Walk-in reservation fee");
                    if(string.IsNullOrWhiteSpace(reference))return;
                }
                else if(MessageBox.Show("Have you physically received and verified the PHP "+fee.ToString("N2")+
                    " reservation fee using "+method+"?", "Staff payment verification",
                    MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            }
            if(MessageBox.Show("Confirm the walk-in guest's ID was checked and you are ready to hand over the keys?\n"+
                "Room(s): "+string.Join(", ",rooms.Select(r=>r.RoomNumber).ToArray())+
                "\nGuests: "+guests+"\nCheck-out: "+dtpOut.Value.ToString("MMM dd, yyyy")+" (12 PM)",
                "Walk-In Check-In",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            int bookingId=0;
            try
            {
                btnCheckIn.Enabled=false;
                Cursor=Cursors.WaitCursor;
                Guest guest=reservations.SaveGuest(new Guest
                {
                    GuestName=txtName.Text.Trim(),ContactNumber=txtContact.Text.Trim(),Email=txtEmail.Text.Trim(),
                    HomeAddress=txtAddress.Text.Trim(),IdType=cmbIdType.Text,IdNumber=txtIdNumber.Text.Trim()
                });
                bookingId=reservations.CreateReservation(guest.GuestId,rooms,dtpIn.Value.Date,
                    dtpOut.Value.Date,guests,adults,children,count,txtNotes.Text.Trim(),null);
                enhancements.SaveGuestIdentity(guest.GuestId,txtIdNumber.Text.Trim(),true);
                if(fee>0m)enhancements.ConfirmReservationFee(bookingId,method,reference,true);
                else enhancements.UpdateReservationStatus(bookingId,"Confirmed");
                if(!checkin.CheckIn(bookingId))
                    throw new InvalidOperationException("The booking was created but could not be checked in automatically.");
                MessageBox.Show("Walk-in checked in. Keys issued to verified guest.\n"+
                    "Booking reference: BJP-R"+bookingId.ToString("D6")+"\nRooms: "+
                    string.Join(", ",rooms.Select(r=>r.RoomNumber).ToArray()),
                    "Walk-In Check-In Complete",MessageBoxButtons.OK,MessageBoxIcon.Information);
                DialogResult=DialogResult.OK;
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Walk-In could not be fully completed:\n"+ex.Message+
                    (bookingId>0?"\n\nBooking BJP-R"+bookingId.ToString("D6")+
                        " was already saved. Find it in Reservations and complete it there; do not create a duplicate.":""),
                    "Walk-In Check-In Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally {Cursor=Cursors.Default;if(!IsDisposed)btnCheckIn.Enabled=true;}
        }
    }
}
