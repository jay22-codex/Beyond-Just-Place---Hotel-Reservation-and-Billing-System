using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    public partial class FrontDesk_ReservationUI : Form
    {
        private readonly ReservationOverviewService overview = new ReservationOverviewService();
        private readonly HotelEnhancementService hotel = new HotelEnhancementService();
        private DataRow selected;
        public FrontDesk_ReservationUI()
        {
            InitializeComponent();
            btnSearch.Click += (s,e) => RefreshReservations();
            btnRefresh.Click += (s,e) => RefreshReservations();
            txtSearch.KeyDown += (s,e) => { if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;RefreshReservations();} };
            dgvReservations.SelectionChanged += (s,e) => DisplaySelection();
            btnVerify.Click += VerifyId;
            btnVerifyDeposit.Click += VerifyDeposit;
            dgvReservations.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            dgvReservations.RowHeadersVisible=false;
            dgvReservations.RowTemplate.Height=30;
            dgvReservations.ColumnHeadersHeight=38;
            dgvReservations.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(246,245,252);
            btnConfirm.Click += (s,e) => ChangeStatus("Confirmed");
            btnCancelBooking.Click += (s,e) => ChangeStatus("Cancelled");
            btnBrowse.Click += (s,e) =>
            {
                using(GuestRoomAvailabilityForm form=new GuestRoomAvailabilityForm()) form.ShowDialog(this);
                RefreshReservations();
            };
            UiPolish.Apply(this);
            Shown += (s,e) => RefreshReservations();
        }
        private void RefreshReservations()
        {
            int selectedId=selected==null?0:Convert.ToInt32(selected["ReservationId"]);
            try
            {
                DataTable data=overview.GetReservations(txtSearch.Text);
                dgvReservations.DataSource=data;
                string[] visible={"ReservationId","GuestName","RoomNumbers","CheckInDate","CheckOutDate",
                    "NumberOfGuests","BedCount","ContactNumber","IdVerified","ReservationStatus","DepositStatus"};
                foreach(DataGridViewColumn c in dgvReservations.Columns)
                    c.Visible=Array.IndexOf(visible,c.Name)>=0;
                Rename("ReservationId","Booking #"); Rename("RoomNumbers","Room(s)");
                Rename("NumberOfGuests","Guests");Rename("BedCount","Beds"); Rename("GuestName","Guest");
                Rename("IdVerified","ID Checked"); Rename("DepositStatus","Deposit");
                Rename("ReservationStatus","Status");
                Rename("ContactNumber","Contact");
                CompactColumns();
                Rename("CheckInDate","Check-in"); Rename("CheckOutDate","Check-out");
                dgvReservations.ClearSelection();
                foreach(DataGridViewRow row in dgvReservations.Rows)
                    if(Convert.ToInt32(((DataRowView)row.DataBoundItem).Row["ReservationId"])==selectedId)
                    {row.Selected=true; dgvReservations.CurrentCell=row.Cells["GuestName"];break;}
                if(selectedId==0) {selected=null;DisplaySelection();}
                lblCount.Text=data.Rows.Count+" reservation(s) shown • Guest details are read-only";
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"Reservation records",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }
        private void CompactColumns()
        {
            foreach(DataGridViewColumn column in dgvReservations.Columns)
            {
                if(!column.Visible)continue;
                column.MinimumWidth=58;
                column.FillWeight=75;
            }
            Weight("GuestName",155,130);
            Weight("RoomNumbers",120,95);
            Weight("ContactNumber",115,100);
            Weight("CheckInDate",95,90);
            Weight("CheckOutDate",95,90);
            Weight("ReservationStatus",92,85);
        }
        private void Weight(string name,float weight,int minWidth)
        {
            if(!dgvReservations.Columns.Contains(name))return;
            dgvReservations.Columns[name].FillWeight=weight;
            dgvReservations.Columns[name].MinimumWidth=minWidth;
        }

        private void Rename(string col,string header)
        {
            if(dgvReservations.Columns.Contains(col)) dgvReservations.Columns[col].HeaderText=header;
        }
        private void DisplaySelection()
        {
            selected=null;
            if(dgvReservations.SelectedRows.Count>0 && dgvReservations.SelectedRows[0].DataBoundItem is DataRowView)
                selected=((DataRowView)dgvReservations.SelectedRows[0].DataBoundItem).Row;
            if(selected==null)
            {
                txtDetails.Text="Select a reservation above to review guest information, dates, rooms, and status.";
                txtIdNumber.Text="";
                lblStatus.Text="No reservation selected";
                btnVerify.Enabled=btnConfirm.Enabled=btnCancelBooking.Enabled=false;
                btnVerifyDeposit.Enabled=false;
                chkChecked.Enabled=false;
                return;
            }
            DataRow r=selected;
            int id=Convert.ToInt32(r["ReservationId"]);
            txtDetails.Text="BOOKING BJP-R"+id.ToString("D6")+"  |  "+r["ReservationStatus"]+"\r\n"+
                "Guest: "+r["GuestName"]+"  •  Contact: "+r["ContactNumber"]+"\r\n"+
                "Email: "+r["Email"]+"  •  Address: "+r["HomeAddress"]+"\r\n"+
                "ID Type: "+r["IdType"]+"  •  ID verified: "+(Convert.ToBoolean(r["IdVerified"])?"YES":"NO")+"\r\n"+
                "Room(s): "+r["RoomNumbers"]+"  •  Count: "+r["RoomCount"]+"\r\n"+
                "Stay: "+Convert.ToDateTime(r["CheckInDate"]).ToString("MMM dd, yyyy")+" (2 PM) to "+
                Convert.ToDateTime(r["CheckOutDate"]).ToString("MMM dd, yyyy")+" (12 PM)\r\n"+
                "Guests: "+r["NumberOfGuests"]+" (Adults "+r["Adults"]+", Children "+r["Children"]+")\r\n"+
                "Deposit: PHP "+Convert.ToDecimal(r["DepositAmount"]).ToString("N2")+"  •  "+r["DepositStatus"]+"\r\n"+
                "Requests: "+r["SpecialRequest"];
            txtIdNumber.Text="";
            chkChecked.Checked=false;
            lblStatus.Text="ID is verified by comparing the guest's physical ID with the saved ID number.";
            bool idChecked=Convert.ToBoolean(r["IdVerified"]);
            string status=Convert.ToString(r["ReservationStatus"]);
            bool pending=status=="Pending";
            bool editable=pending || status=="Confirmed";
            decimal deposit=Convert.ToDecimal(r["DepositAmount"]);
            bool feePaid=Convert.ToString(r["DepositStatus"])=="Paid";
            btnVerify.Enabled=!idChecked && editable;
            chkChecked.Enabled=btnVerify.Enabled;
            btnVerifyDeposit.Enabled=pending && deposit>0m && !feePaid;
            btnConfirm.Enabled=pending && (deposit==0m || feePaid);
            btnCancelBooking.Enabled=editable;
            lblStatus.Text=idChecked
                ? "ID VERIFIED - checked by Front Desk."
                : "ID PENDING - compare physical ID, enter the ID number, tick the box, then click VERIFY GUEST ID.";
            if(btnVerifyDeposit.Enabled)
                lblStatus.Text="DEPOSIT PENDING - click Verify Reservation Fee before confirming booking.";
        }
        private void VerifyId(object sender,EventArgs e)
        {
            if(!CurrentUserSession.RequireFrontDesk(this))return;
            if(selected==null)return;
            string stored=Convert.ToString(selected["IdNumber"]).Trim();
            string typed=txtIdNumber.Text.Trim();
            if(stored.Length<3 || !string.Equals(stored,typed,StringComparison.OrdinalIgnoreCase) || !chkChecked.Checked)
            {MessageBox.Show("Check the actual physical ID. Enter the same ID number saved during booking and tick the confirmation box.",
                "ID verification",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            if(MessageBox.Show("I confirm I checked the guest's physical ID and its number matches the booking.",
                "Verify identity",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            try{hotel.SaveGuestIdentity(Convert.ToInt32(selected["GuestId"]),stored,true);RefreshReservations();
                MessageBox.Show("Guest ID verified successfully.","ID verified");}
            catch(Exception ex){MessageBox.Show(ex.Message,"ID verification error");}
        }
        private void VerifyDeposit(object sender,EventArgs e)
        {
            if(!CurrentUserSession.RequireFrontDesk(this))return;
            if(selected==null)return;
            int reservationId=Convert.ToInt32(selected["ReservationId"]);
            decimal fee=Convert.ToDecimal(selected["DepositAmount"]);
            if(fee<=0m)
            {
                MessageBox.Show("No reservation fee is required.","Reservation Fee");
                return;
            }
            if(Convert.ToString(selected["ReservationStatus"])!="Pending")
            {
                MessageBox.Show("Only pending bookings can have a fee verified here.","Reservation Fee");
                return;
            }
            string method=ChooseDepositMethod(Convert.ToString(selected["ReservationFeePaymentMethod"]));
            if(string.IsNullOrWhiteSpace(method))return;
            string reference=null;
            if(method=="GCash")
            {
                reference=PaymentUiDialogs.RequestGcCashReference(this,fee,"Reservation Fee");
                if(reference==null)return;
            }
            else
            {
                if(MessageBox.Show("Did you personally confirm PHP "+fee.ToString("N2")+
                    " received through "+method+"?", "Confirm Reservation Fee",
                    MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            }
            try
            {
                hotel.ConfirmReservationFee(reservationId,method,reference,true);
                RefreshReservations();
                MessageBox.Show("Reservation fee recorded as paid. Booking is now Confirmed.",
                    "Payment Verified",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Fee verification error",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private string ChooseDepositMethod(string suggested)
        {
            using(Form dialog=new Form())
            {
                dialog.Text="Front Desk - Reservation Fee Payment Method";
                dialog.ClientSize=new Size(400,170);
                dialog.StartPosition=FormStartPosition.CenterParent;
                dialog.FormBorderStyle=FormBorderStyle.FixedDialog;
                dialog.MaximizeBox=false;dialog.MinimizeBox=false;
                dialog.BackColor=Color.FromArgb(240,238,249);
                Label label=new Label{Text="Select how the guest actually paid:",
                    Location=new Point(18,16),Size=new Size(360,26)};
                ComboBox cmb=new ComboBox{Location=new Point(18,48),Width=355,
                    DropDownStyle=ComboBoxStyle.DropDownList};
                cmb.Items.AddRange(new object[]{"Cash","GCash","Card"});
                cmb.SelectedItem=cmb.Items.Contains(suggested)?suggested:"Cash";
                Button ok=new Button{Text="Continue",Location=new Point(167,103),
                    Size=new Size(98,34),DialogResult=DialogResult.OK,
                    BackColor=Color.FromArgb(128,128,255)};
                Button cancel=new Button{Text="Cancel",Location=new Point(278,103),
                    Size=new Size(98,34),DialogResult=DialogResult.Cancel};
                dialog.Controls.AddRange(new Control[]{label,cmb,ok,cancel});
                dialog.AcceptButton=ok;dialog.CancelButton=cancel;
                return dialog.ShowDialog(this)==DialogResult.OK?cmb.Text:null;
            }
        }

        private void ChangeStatus(string status)
        {
            if(!CurrentUserSession.RequireFrontDesk(this))return;
            if(selected==null)return;
            int id=Convert.ToInt32(selected["ReservationId"]);
            if(MessageBox.Show("Change Booking #"+id+" to "+status+"?", "Confirm status",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            try{hotel.UpdateReservationStatus(id,status);RefreshReservations();}
            catch(Exception ex){MessageBox.Show(ex.Message,"Status change failed",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }
    }
}
