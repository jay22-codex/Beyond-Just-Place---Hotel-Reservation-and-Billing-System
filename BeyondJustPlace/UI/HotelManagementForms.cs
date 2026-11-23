using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BusinessLogic;
using Model;

namespace UI
{
    internal static class HotelFormStyle
    {
        public static readonly Color Purple=Color.FromArgb(128,128,255);
        public static readonly Color Pale=Color.FromArgb(241,239,249);
        public static Button Button(string text,int width=140)
        {
            return new Button{Text=text,Width=width,Height=38,Font=new Font("Arial",10,FontStyle.Bold),
                BackColor=Color.FromArgb(224,224,224),UseVisualStyleBackColor=false,Margin=new Padding(5)};
        }
        public static Label Label(string text)
        {
            return new Label{Text=text,AutoSize=false,Height=26,Dock=DockStyle.Fill,
                TextAlign=ContentAlignment.MiddleLeft,Font=new Font("Arial",10,FontStyle.Bold)};
        }
        public static void ConfigureDialog(Form form,string text,int width,int height)
        {
            form.Text="Beyond Just Place - "+text;
            form.ClientSize=new Size(width,height);
            form.StartPosition=FormStartPosition.CenterParent;
            form.BackColor=Pale;
            form.Font=new Font("Arial",10);
            form.MinimumSize=new Size(780,510);
        }
        public static NumericUpDown Number(int maximum=10000,int decimals=0)
        {
            return new NumericUpDown{Maximum=maximum,Minimum=0,DecimalPlaces=decimals,
                ThousandsSeparator=decimals>0,Dock=DockStyle.Fill,Font=new Font("Arial",11)};
        }
        public static DataGridView Grid()
        {
            return new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,MultiSelect=false,
                AutoGenerateColumns=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,
                SelectionMode=DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor=Color.White,RowHeadersVisible=false,RowTemplate={Height=30},
                ColumnHeadersHeight=38,AlternatingRowsDefaultCellStyle={BackColor=Color.FromArgb(247,246,252)}};
        }
    }

    public sealed class RoomTypeManagerForm:Form
    {
        private readonly HotelEnhancementService service=new HotelEnhancementService();
        private readonly DataGridView grid=HotelFormStyle.Grid();
        private readonly TextBox name=new TextBox{Dock=DockStyle.Fill,MaxLength=100};
        private readonly NumericUpDown rate=HotelFormStyle.Number(10000000,2);
        private readonly NumericUpDown capacity=HotelFormStyle.Number(50);
        private readonly NumericUpDown beds=HotelFormStyle.Number(30);
        private readonly NumericUpDown size=HotelFormStyle.Number(1000);
        private readonly NumericUpDown fee=HotelFormStyle.Number(1000000,2);
        private readonly TextBox description=new TextBox{Dock=DockStyle.Fill,MaxLength=500,Multiline=true,ScrollBars=ScrollBars.Vertical};
        private readonly CheckBox active=new CheckBox{Text="Active / bookable",Checked=true,AutoSize=true};
        private int selectedId;
        public RoomTypeManagerForm()
        {
            HotelFormStyle.ConfigureDialog(this,"Room Types / Rate Setup",1320,690);
            SplitContainer split=new SplitContainer{Dock=DockStyle.Fill,SplitterDistance=770};
            grid.SelectionChanged+=(s,e)=>LoadSelection();
            split.Panel1.Controls.Add(grid);
            TableLayoutPanel table=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=8,Padding=new Padding(15)};
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,145));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            for(int i=0;i<8;i++) table.RowStyles.Add(new RowStyle(SizeType.Percent,12.5f));
            Row(table,0,"Room Type",name);
            Row(table,1,"Rate / Night (PHP)",rate);
            Row(table,2,"Guest Capacity",capacity);
            Row(table,3,"Number of Beds",beds);
            Row(table,4,"Size (m²)",size);
            Row(table,5,"Reservation Fee",fee);
            Row(table,6,"Description",description);
            Row(table,7,"Status",active);
            split.Panel2.Controls.Add(table);
            FlowLayoutPanel actions=new FlowLayoutPanel{Dock=DockStyle.Bottom,Height=60,BackColor=HotelFormStyle.Purple,Padding=new Padding(7)};
            Button add=HotelFormStyle.Button("New Type");
            Button save=HotelFormStyle.Button("Save Type");
            Button disable=HotelFormStyle.Button("Activate / Deactivate",190);
            Button close=HotelFormStyle.Button("Close");
            actions.Controls.AddRange(new Control[]{add,save,disable,close});
            Controls.Add(split);Controls.Add(actions);
            add.Click+=(s,e)=>ClearEditor();
            save.Click+=(s,e)=>TryAction(()=>{
                service.SaveRoomType(selectedId,name.Text,rate.Value,(int)capacity.Value,
                    (int)beds.Value,(int)size.Value,fee.Value,description.Text,active.Checked);
                RefreshGrid();
            });
            disable.Click+=(s,e)=>TryAction(()=>{
                if(selectedId<=0)throw new InvalidOperationException("Select a room type first.");
                service.SetRoomTypeActive(selectedId,!active.Checked);
                RefreshGrid();
            });
            close.Click+=(s,e)=>Close();
            Shown+=(s,e)=>TryAction(()=>RefreshGrid());
            ClearEditor();
        }
        private static void Row(TableLayoutPanel p,int i,string text,Control control)
        {p.Controls.Add(HotelFormStyle.Label(text),0,i);p.Controls.Add(control,1,i);}
        private void ClearEditor()
        {selectedId=0;name.Clear();rate.Value=0;capacity.Value=2;beds.Value=1;size.Value=24;fee.Value=0;description.Clear();active.Checked=true;grid.ClearSelection();}
        private void RefreshGrid()
        {
            grid.DataSource=service.GetRoomTypes(true);
            if(grid.Columns.Contains("RoomTypeId"))grid.Columns["RoomTypeId"].Visible=false;
            if(grid.Columns.Contains("Description"))grid.Columns["Description"].Visible=false;
            grid.ClearSelection();
        }
        private void LoadSelection()
        {
            DataRowView row=grid.CurrentRow==null?null:grid.CurrentRow.DataBoundItem as DataRowView;
            if(row==null || grid.SelectedRows.Count==0)return;
            selectedId=Convert.ToInt32(row["RoomTypeId"]);
            name.Text=Convert.ToString(row["TypeName"]);
            rate.Value=Convert.ToDecimal(row["Rate"]);
            capacity.Value=Convert.ToDecimal(row["Capacity"]);
            beds.Value=Convert.ToDecimal(row["BedCount"]);
            size.Value=Convert.ToDecimal(row["SizeSqm"]);
            fee.Value=Convert.ToDecimal(row["ReservationFee"]);
            description.Text=Convert.ToString(row["Description"]);
            active.Checked=Convert.ToBoolean(row["IsActive"]);
        }
        private void TryAction(Action action)
        {try{action();}catch(Exception ex){MessageBox.Show(ex.Message,"Room Type Setup",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
    }

    public sealed class GuestMaintenanceForm:Form
    {
        private readonly HotelEnhancementService service=new HotelEnhancementService();
        private readonly ReservationDetailsService details=new ReservationDetailsService();
        private readonly DataGridView grid=HotelFormStyle.Grid();
        private readonly TextBox search=new TextBox{Width=280};
        private readonly TextBox name=new TextBox{Dock=DockStyle.Fill};
        private readonly TextBox contact=new TextBox{Dock=DockStyle.Fill};
        private readonly TextBox email=new TextBox{Dock=DockStyle.Fill};
        private readonly TextBox address=new TextBox{Dock=DockStyle.Fill};
        private readonly TextBox idNumber=new TextBox{Dock=DockStyle.Fill};
        private readonly ComboBox idType=new ComboBox{Dock=DockStyle.Fill,DropDownStyle=ComboBoxStyle.DropDownList};
        private readonly TextBox idVerified=new TextBox{Dock=DockStyle.Fill,ReadOnly=true,
            Text="Not verified - Front Desk only",BackColor=Color.FromArgb(235,234,246)};
        private int selectedId;
        public GuestMaintenanceForm()
        {
            HotelFormStyle.ConfigureDialog(this,"Guest Maintenance / 50 Records",1330,750);
            FlowLayoutPanel searchbar=new FlowLayoutPanel{Dock=DockStyle.Top,Height=53,Padding=new Padding(8),BackColor=HotelFormStyle.Purple};
            searchbar.Controls.Add(new Label{Text="Search Guest:",AutoSize=true,Margin=new Padding(8,9,8,4),Font=new Font("Arial",11,FontStyle.Bold)});
            searchbar.Controls.Add(search);
            Button find=HotelFormStyle.Button("Search");searchbar.Controls.Add(find);
            Button refresh=HotelFormStyle.Button("Refresh");searchbar.Controls.Add(refresh);
            SplitContainer split=new SplitContainer{Dock=DockStyle.Fill,SplitterDistance=760};
            split.Panel1.Controls.Add(grid);
            TableLayoutPanel editor=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=7,Padding=new Padding(12)};
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,135));
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            for(int i=0;i<7;i++) editor.RowStyles.Add(new RowStyle(SizeType.Percent,14.285f));
            idType.Items.AddRange(new object[]{"National ID","Passport","Driver's License","Student ID","Other"});
            EditorRow(editor,0,"Guest Name",name);
            EditorRow(editor,1,"Contact",contact);
            EditorRow(editor,2,"Email",email);
            EditorRow(editor,3,"Address",address);
            EditorRow(editor,4,"ID Type",idType);
            EditorRow(editor,5,"ID Number",idNumber);
            EditorRow(editor,6,"ID Status",idVerified);
            split.Panel2.Controls.Add(editor);
            FlowLayoutPanel actions=new FlowLayoutPanel{Dock=DockStyle.Bottom,Height=62,Padding=new Padding(6),BackColor=HotelFormStyle.Purple};
            Button add=HotelFormStyle.Button("New Guest");
            Button save=HotelFormStyle.Button("Save / Update",150);
            Button active=HotelFormStyle.Button("Activate / Deactivate",190);
            Button demo=HotelFormStyle.Button("Create 50 DEMO Guests",190);
            Button close=HotelFormStyle.Button("Close");
            actions.Controls.AddRange(new Control[]{add,save,active,demo,close});
            Controls.Add(split);Controls.Add(actions);Controls.Add(searchbar);
            grid.SelectionChanged+=(s,e)=>LoadSelection();
            find.Click+=(s,e)=>TryAction(RefreshGrid);
            refresh.Click+=(s,e)=>{search.Clear();TryAction(RefreshGrid);};
            add.Click+=(s,e)=>ClearEditor();
            save.Click+=(s,e)=>TryAction(()=>{
                Guest guest=new Guest{GuestId=selectedId,GuestName=name.Text,ContactNumber=contact.Text,
                    Email=email.Text,HomeAddress=address.Text,IdType=idType.Text,IdNumber=idNumber.Text};
                details.SaveGuest(guest);
                RefreshGrid();
                MessageBox.Show("Guest record saved.","Guest Maintenance",MessageBoxButtons.OK,MessageBoxIcon.Information);
            });
            active.Click+=(s,e)=>TryAction(()=>{
                if(selectedId<=0)throw new InvalidOperationException("Select a guest first.");
                DataRowView row=grid.CurrentRow.DataBoundItem as DataRowView;
                if(row==null)throw new InvalidOperationException("Select a record.");
                bool isActive=Convert.ToBoolean(row["IsActive"]);
                service.SetGuestActive(selectedId,!isActive);
                RefreshGrid();
            });
            demo.Click+=(s,e)=>{
                if(MessageBox.Show("Create 50 clearly marked DEMO records for testing?\nThese are NOT real guests.","School Demo Records",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
                TryAction(()=>{int n=service.CreateDemoGuestsForSchool();RefreshGrid();MessageBox.Show(n+" demo records created.","Demo Data",MessageBoxButtons.OK,MessageBoxIcon.Information);});
            };
            close.Click+=(s,e)=>Close();
            Shown+=(s,e)=>TryAction(RefreshGrid);
            ClearEditor();
        }
        private static void EditorRow(TableLayoutPanel p,int i,string caption,Control ctl)
        {p.Controls.Add(HotelFormStyle.Label(caption),0,i);p.Controls.Add(ctl,1,i);}
        private void ClearEditor()
        {selectedId=0;name.Clear();contact.Clear();email.Clear();address.Clear();idNumber.Clear();idType.SelectedIndex=-1;idVerified.Text="Not verified - Front Desk only";grid.ClearSelection();}
        private void RefreshGrid()
        {
            grid.DataSource=service.GetGuests(search.Text,true);
            if(grid.Columns.Contains("GuestId"))grid.Columns["GuestId"].Visible=false;
            if(grid.Columns.Contains("HomeAddress"))grid.Columns["HomeAddress"].Visible=false;
            if(grid.Columns.Contains("IdNumber"))grid.Columns["IdNumber"].Visible=false;
            if(grid.Columns.Contains("CreatedAt"))grid.Columns["CreatedAt"].Visible=false;
            grid.ClearSelection();
        }
        private void LoadSelection()
        {
            if(grid.SelectedRows.Count==0 || grid.CurrentRow==null)return;
            DataRowView r=grid.CurrentRow.DataBoundItem as DataRowView;
            if(r==null)return;
            selectedId=Convert.ToInt32(r["GuestId"]);
            name.Text=Convert.ToString(r["GuestName"]);
            contact.Text=Convert.ToString(r["ContactNumber"]);
            email.Text=Convert.ToString(r["Email"]);
            address.Text=Convert.ToString(r["HomeAddress"]);
            idType.SelectedItem=Convert.ToString(r["IdType"]);
            idNumber.Text=Convert.ToString(r["IdNumber"]);
            idVerified.Text=Convert.ToBoolean(r["IdVerified"]) ? "Verified by Front Desk" : "Awaiting Front Desk verification";
        }
        private void TryAction(Action action)
        {try{action();}catch(Exception ex){MessageBox.Show(ex.Message,"Guest Maintenance",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
    }
}
