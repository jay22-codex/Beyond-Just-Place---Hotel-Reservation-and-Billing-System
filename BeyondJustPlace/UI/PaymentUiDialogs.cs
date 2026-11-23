using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UI
{
    // GCash reference must be entered after seeing the actual hotel's GCash QR.
    // This is staff-attested payment, NOT a verified gateway transaction.
    internal static class PaymentUiDialogs
    {
        public static string RequestGcCashReference(IWin32Window owner, decimal amount, string purpose)
        {
            string setting=ConfigurationManager.AppSettings["GCashQrImage"];
            if(string.IsNullOrWhiteSpace(setting))setting="PaymentAssets\\GCashQR.png";
            string file=Path.IsPathRooted(setting)?setting:Path.Combine(AppDomain.CurrentDomain.BaseDirectory,setting);
            if(!File.Exists(file))
            {
                MessageBox.Show(owner,
                    "The hotel's actual GCash QR has not been configured.\n\n"+
                    "Place your authorized GCash QR image at:\n"+file+"\n\n"+
                    "No payment was recorded.",
                    "GCash QR Not Configured",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return null;
            }
            using(Form qr=new Form())
            {
                qr.Text="Beyond Just Place - GCash Payment";
                qr.StartPosition=FormStartPosition.CenterParent;
                qr.ClientSize=new Size(475,555);
                qr.FormBorderStyle=FormBorderStyle.FixedDialog;
                qr.MaximizeBox=false;
                qr.MinimizeBox=false;
                qr.BackColor=Color.FromArgb(242,241,250);
                Label heading=new Label{Text="GCash - Scan the hotel's QR",Font=new Font("Arial",16,FontStyle.Bold),
                    TextAlign=ContentAlignment.MiddleCenter,Dock=DockStyle.Top,Height=52,ForeColor=Color.FromArgb(66,45,120)};
                Label due=new Label{Text=purpose+"\nAmount due: PHP "+amount.ToString("N2"),
                    Font=new Font("Arial",12,FontStyle.Bold),Height=52,Dock=DockStyle.Top,TextAlign=ContentAlignment.MiddleCenter};
                PictureBox image=new PictureBox{Location=new Point(80,115),Size=new Size(315,315),BorderStyle=BorderStyle.FixedSingle,SizeMode=PictureBoxSizeMode.Zoom,BackColor=Color.White};
                using(Image source=Image.FromFile(file))image.Image=new Bitmap(source);
                Label note=new Label{Text="Check the recipient and amount in GCash before continuing.\nClick OK after the guest has sent payment.",
                    Location=new Point(25,440),Size=new Size(420,45),TextAlign=ContentAlignment.MiddleCenter};
                Button ok=new Button{Text="OK - Payment Sent",DialogResult=DialogResult.OK,Location=new Point(100,495),Size=new Size(155,38),BackColor=Color.FromArgb(128,128,255)};
                Button cancel=new Button{Text="Cancel",DialogResult=DialogResult.Cancel,Location=new Point(270,495),Size=new Size(105,38)};
                qr.Controls.AddRange(new Control[]{image,note,ok,cancel,due,heading});
                qr.AcceptButton=ok;qr.CancelButton=cancel;
                DialogResult response=qr.ShowDialog(owner);
                image.Image.Dispose();
                if(response!=DialogResult.OK)return null;
            }
            string reference=PromptReference(owner);
            if(string.IsNullOrWhiteSpace(reference))return null;
            if(MessageBox.Show(owner,
                "GCash Reference: "+reference+"\n\nDid you verify the transfer in the hotel's GCash account?\n"+
                "Click Yes ONLY after checking the transaction.",
                "Staff Payment Verification",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)
                return null;
            return reference;
        }

        private static string PromptReference(IWin32Window owner)
        {
            using(Form dialog=new Form())
            {
                dialog.Text="Enter GCash Receipt Reference Number";
                dialog.StartPosition=FormStartPosition.CenterParent;
                dialog.ClientSize=new Size(500,190);
                dialog.BackColor=Color.FromArgb(242,241,250);
                dialog.FormBorderStyle=FormBorderStyle.FixedDialog;
                dialog.MinimizeBox=false;dialog.MaximizeBox=false;
                Label label=new Label{Text="Enter the reference number shown on the GCash receipt:",
                    Font=new Font("Arial",11),Location=new Point(20,22),Size=new Size(460,30)};
                TextBox reference=new TextBox{Location=new Point(20,60),Size=new Size(455,32),MaxLength=100,Font=new Font("Arial",14)};
                Label note=new Label{Text="A unique reference is required to record this transaction.",
                    Location=new Point(20,100),Size=new Size(460,22)};
                Button ok=new Button{Text="OK",DialogResult=DialogResult.OK,Location=new Point(260,135),Size=new Size(100,37),BackColor=Color.FromArgb(128,128,255)};
                Button cancel=new Button{Text="Cancel",DialogResult=DialogResult.Cancel,Location=new Point(374,135),Size=new Size(100,37)};
                dialog.Controls.AddRange(new Control[]{label,reference,note,ok,cancel});
                dialog.AcceptButton=ok;dialog.CancelButton=cancel;
                while(dialog.ShowDialog(owner)==DialogResult.OK)
                {
                    string value=reference.Text.Trim();
                    if(value.Length>=6 && value.Length<=100 && System.Text.RegularExpressions.Regex.IsMatch(value,@"^[a-zA-Z0-9\-]+$"))
                        return value;
                    MessageBox.Show(owner,"Enter 6-100 letters/numbers from the GCash receipt.",
                        "Invalid GCash Reference",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                return null;
            }
        }
    }
}
