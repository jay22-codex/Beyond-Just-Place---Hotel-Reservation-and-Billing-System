
using System;
using System.Data;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    public partial class FrontDesk_CheckIn_CheckOut : Form
    {
        private readonly FrontDeskOperationsService operations =
            new FrontDeskOperationsService();

        private BillingInfo selectedBilling;
        private int lastSelectedCheckInId;
        private readonly HotelEnhancementService enhancements=new HotelEnhancementService();

        public FrontDesk_CheckIn_CheckOut()
        {
            InitializeComponent();
            UiPolish.Apply(this);

            btnSearchCheckIn.Click += btnSearch_Click;
            button1.Click += btnConfirmCheckIn_Click;
            btnVerifySelectedId.Click += VerifySelectedGuestId;
            button3.Click += btnSearchCheckout_Click;
            btnCheckOut.Click += btnCheckOut_Click;
            btnWalkIn.Click += (s,e) =>
            {
                if(!CurrentUserSession.RequireFrontDesk(this))return;
                using (WalkInCheckInForm form = new WalkInCheckInForm())
                    if (form.ShowDialog(this) == DialogResult.OK)
                        btnSearch_Click(this, EventArgs.Empty);
            };
            dgvCheckInReservations.SelectionChanged += (s,e) => ShowSelectionInDetailsPanel();
            Shown += (s,e) => btnSearch_Click(this,EventArgs.Empty);

            dgvCheckInReservations.AllowUserToAddRows = false;
            dgvCheckInReservations.ReadOnly = true;
            dgvCheckInReservations.MultiSelect = false;

            dgvCheckInReservations.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCheckInReservations.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvCheckInReservations.RowHeadersVisible=false;
            dgvCheckInReservations.ColumnHeadersHeight=38;
            dgvCheckInReservations.RowTemplate.Height=30;
            dgvCheckInReservations.AlternatingRowsDefaultCellStyle.BackColor=
                System.Drawing.Color.FromArgb(246,245,252);
            ClearBill();
        }

        private int SelectedReservationId()
        {
            if (dgvCheckInReservations.CurrentRow == null ||
                dgvCheckInReservations.CurrentRow.DataBoundItem == null)
                return 0;
            DataRowView item = dgvCheckInReservations.CurrentRow.DataBoundItem as DataRowView;
            if (item == null) return 0;
            return Convert.ToInt32(item.Row["ReservationId"]);
        }

        private void ShowSelectionInDetailsPanel()
        {
            int id = SelectedReservationId();
            if (id!=lastSelectedCheckInId)
            {
                lastSelectedCheckInId=id;
                txtPhysicalId.Clear();
                chkPhysicalIdChecked.Checked=false;
                chkKeysIssued.Checked=false;
            }
            btnVerifySelectedId.Enabled=id>0;
            button1.Enabled=id>0;
            if (id == 0)
            {
                btnVerifySelectedId.Enabled=false;
                button1.Enabled=false;
                txtSelectedGuestDetails.Text =
                    "Select a reservation above. Guest details will appear automatically here.";
                return;
            }
            try
            {
                DataTable table = enhancements.GetReservationDetails(id);
                if (table.Rows.Count > 0)
                {
                    DataRow info=table.Rows[0];
                    txtSelectedGuestDetails.Text=Describe(info);
                    bool alreadyChecked=Convert.ToBoolean(info["IdVerified"]);
                    btnVerifySelectedId.Enabled=!alreadyChecked;
                    button1.Enabled=Convert.ToString(info["ReservationStatus"])=="Confirmed";
                    if(alreadyChecked)
                        txtSelectedGuestDetails.Text+="\nID verified by Front Desk - room-key handover still required.";
                }
                else
                {
                    txtSelectedGuestDetails.Text = "Reservation no longer exists.";
                    btnVerifySelectedId.Enabled=false;
                    button1.Enabled=false;
                }
            }
            catch (Exception ex)
            { txtSelectedGuestDetails.Text = "Cannot load selected guest details: " + ex.Message; }
        }

        private static string Describe(DataRow r)
        {
            return "Reservation #"+r["ReservationId"]+" - "+r["ReservationStatus"]+"\n"+
                "Guest: "+r["GuestName"]+" | Contact: "+r["ContactNumber"]+"\n"+
                "Email: "+r["Email"]+" | Address: "+r["HomeAddress"]+"\n"+
                "ID Type: "+r["IdType"]+" | ID verified: "+(Convert.ToBoolean(r["IdVerified"])?"YES":"NO")+"\n"+
                "Rooms ("+r["RoomCount"]+"): "+r["Rooms"]+"\n"+
                "Guests: "+r["NumberOfGuests"]+" | Adults: "+r["Adults"]+" | Children: "+r["Children"]+"\n"+
                "Check-in: "+Convert.ToDateTime(r["CheckInDate"]).ToString("MMM dd, yyyy")+" (2:00 PM)\n"+
                "Check-out: "+Convert.ToDateTime(r["CheckOutDate"]).ToString("MMM dd, yyyy")+" (12:00 PM)\n"+
                "Special requests: "+r["SpecialRequest"]+"\n"+
                "Deposit status: "+r["ReservationFeeStatus"]+"\n"+
                "Keys issued: "+(Convert.ToBoolean(r["KeysIssued"])?"YES":"NO");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ReservationOverviewService overview = new ReservationOverviewService();
                DataTable matches = overview.GetReservations(txtCheckInGuest.Text.Trim(), true);
                dgvCheckInReservations.AutoGenerateColumns = true;
                dgvCheckInReservations.Columns.Clear();
                dgvCheckInReservations.DataSource = matches;
                string[] visible={"ReservationId","GuestName","RoomNumbers","NumberOfGuests",
                    "BedCount","CheckInDate","CheckOutDate","ContactNumber","IdVerified","ReservationStatus"};
                foreach(DataGridViewColumn c in dgvCheckInReservations.Columns)
                    c.Visible=Array.IndexOf(visible,c.Name)>=0;
                string[,] headers = {
                    {"ReservationId","Booking #"},{"GuestName","Guest"},{"RoomNumbers","Rooms"},
                    {"NumberOfGuests","Guests"},{"BedCount","Beds"},{"CheckInDate","Check-in"},{"CheckOutDate","Check-out"},
                    {"ContactNumber","Contact"},{"IdVerified","ID checked"},{"ReservationStatus","Status"}};
                for(int i=0;i<headers.GetLength(0);i++)
                    if(dgvCheckInReservations.Columns.Contains(headers[i,0]))
                        dgvCheckInReservations.Columns[headers[i,0]].HeaderText=headers[i,1];
                FitCheckInColumns();
                ShowSelectionInDetailsPanel();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void FitCheckInColumns()
        {
            foreach(DataGridViewColumn c in dgvCheckInReservations.Columns)
            {
                if(!c.Visible)continue;
                c.MinimumWidth=62;
                c.FillWeight=75;
            }
            SetCheckInWeight("GuestName",155,135);
            SetCheckInWeight("RoomNumbers",125,95);
            SetCheckInWeight("ContactNumber",125,103);
            SetCheckInWeight("ReservationStatus",100,90);
            SetCheckInWeight("CheckInDate",95,91);
            SetCheckInWeight("CheckOutDate",95,91);
        }
        private void SetCheckInWeight(string name,float weight,int minWidth)
        {
            if(!dgvCheckInReservations.Columns.Contains(name))return;
            dgvCheckInReservations.Columns[name].FillWeight=weight;
            dgvCheckInReservations.Columns[name].MinimumWidth=minWidth;
        }

        private void VerifySelectedGuestId(object sender,EventArgs e)
        {
            if(!CurrentUserSession.RequireFrontDesk(this))return;
            int reservationId=SelectedReservationId();
            if(reservationId<=0)
            {
                MessageBox.Show("Select a guest row first.","ID Verification");
                return;
            }
            if(!chkPhysicalIdChecked.Checked)
            {
                MessageBox.Show("Check the guest's physical ID, then tick Physical ID checked.",
                    "ID Verification",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            try
            {
                DataTable details=enhancements.GetReservationDetails(reservationId);
                if(details.Rows.Count==0)throw new InvalidOperationException("Reservation no longer exists.");
                DataRow row=details.Rows[0];
                string recorded=Convert.ToString(row["IdNumber"]).Trim();
                if(recorded.Length<3 ||
                    !string.Equals(recorded,txtPhysicalId.Text.Trim(),StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("The entered number does not match the booking record. " +
                        "Compare the original physical ID and try again.",
                        "ID Mismatch",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return;
                }
                if(MessageBox.Show("Confirm the guest's physical ID and recorded ID number match?",
                    "Verify Guest ID",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)
                    return;
                enhancements.SaveGuestIdentity(Convert.ToInt32(row["GuestId"]),recorded,true);
                ShowSelectionInDetailsPanel();
                MessageBox.Show("Guest ID verified. You may now complete check-in after " +
                    "the reservation is confirmed and the room keys are handed over.",
                    "ID Verified",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch(Exception ex){ShowError(ex);}
        }

        private void btnConfirmCheckIn_Click(
            object sender, EventArgs e)
        {
            if(!CurrentUserSession.RequireFrontDesk(this))return;
            int id = SelectedReservationId();
            if (id <= 0)
            {
                MessageBox.Show("Select a guest reservation first.", "Check-In");
                return;
            }

            DataTable details;
            try{details=enhancements.GetReservationDetails(id);}
            catch(Exception ex){ShowError(ex);return;}
            if(details.Rows.Count==0){MessageBox.Show("Reservation not found.","Check-In");return;}
            DataRow row=details.Rows[0];
            if(!Convert.ToBoolean(row["IdVerified"]))
            {MessageBox.Show("The guest ID is not verified. Enter the physical ID number and click Verify Selected ID above.",
                "ID Checker",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            if(Convert.ToString(row["ReservationStatus"])!="Confirmed")
            {MessageBox.Show("Reservation is Pending. Verify the reservation fee and confirm the booking in Front Desk Reservation first.",
                "Reservation Status",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
            if(!chkPhysicalIdChecked.Checked || !chkKeysIssued.Checked)
            {
                MessageBox.Show("Tick both Physical ID checked and Room key(s) handed over " +
                    "before confirming check-in.","Check-In Checklist",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            DialogResult answer = MessageBox.Show(
                Describe(row)+"\n\nHave you checked the physical ID and handed over all room keys?",
                "Confirm ID and Key Issuance",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            try
            {
                bool updated = operations.CheckIn(id);

                MessageBox.Show(updated
                    ? "Guest checked in successfully. Room key(s) handed over and recorded."
                    : "Check-in blocked. Confirm the reservation and verify the guest ID first.",
                    "Check-In and Key Issuance");

                btnSearch_Click(sender, e);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }


        private void btnSearchCheckout_Click(
            object sender, EventArgs e)
        {
            selectedBilling = null;
            ClearBill();

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show(
                    "Enter the guest's name in the Check-Out section.", "Beyond Just Place");
                return;
            }

            try
            {
                selectedBilling =
                    operations.GetCheckedInBilling(
                        textBox1.Text.Trim());

                if (selectedBilling == null)
                {
                    MessageBox.Show(
                        "No checked-in reservation found.", "Beyond Just Place");
                    return;
                }

                lblGuestnameOutput.Text =
                    selectedBilling.GuestName;

                lblRoomOutput.Text =
                    selectedBilling.RoomNumber;

                lblStayOutput.Text =
                    selectedBilling.StayDays + " night(s)";

                lblRoomCharges.Text =
                    "PHP " +
                    selectedBilling.RoomCharges.ToString("N2");

                lblOtherCharges.Text =
                    "PHP " +
                    selectedBilling.OtherCharges.ToString("N2");

                lblTotalBill.Text =
                    "PHP " +
                    selectedBilling.TotalBill.ToString("N2");

                // Checkout details remain visible in the form's existing billing labels.
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }


        private void btnCheckOut_Click(
            object sender, EventArgs e)
        {
            if(!CurrentUserSession.RequireFrontDesk(this))return;
            if (selectedBilling == null)
            {
                MessageBox.Show(
                    "Search a checked-in guest in the Check-Out section first.", "Beyond Just Place");
                return;
            }

            try
            {
                FrontDeskDashboard parent =
                    MdiParent as FrontDeskDashboard;

                if (parent != null)
                {
                    parent.OpenBilling(selectedBilling);
                }
                else
                {
                    using (FrontDesk_Billing_PaymentUI form =
                        new FrontDesk_Billing_PaymentUI(
                            selectedBilling))
                    {
                        form.ShowDialog(this);

                        if (form.PaymentCompleted)
                        {
                            textBox1.Clear();
                            selectedBilling = null;
                            ClearBill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }


        private void ClearBill()
        {
            lblGuestnameOutput.Text = "Output";
            lblRoomOutput.Text = "Output";
            lblStayOutput.Text = "Output";

            lblRoomCharges.Text = "PHP 0.00";
            lblOtherCharges.Text = "PHP 0.00";
            lblTotalBill.Text = "PHP 0.00";
        }

        private void btnBackReservation_Click(
            object sender, EventArgs e)
        {
            FrontDeskDashboard parent =
                MdiParent as FrontDeskDashboard;

            if (parent != null)
                parent.ShowDashboard();
            else
                Close();
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Check-In / Check-Out Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
