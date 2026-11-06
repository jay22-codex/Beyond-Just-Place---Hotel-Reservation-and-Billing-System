using System;
using System.Windows.Forms;

namespace UI
{
    partial class FrontDeskDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnReservation;
        private System.Windows.Forms.Button btnCheckInCheckOut;
        private System.Windows.Forms.Button btnBilling;
        private System.Windows.Forms.Button btnLogin;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblRole = new System.Windows.Forms.Label();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnReservation = new System.Windows.Forms.Button();
            this.btnCheckInCheckOut = new System.Windows.Forms.Button();
            this.btnBilling = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.panelSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.Controls.Add(this.lblRole);
            this.panelSidebar.Controls.Add(this.btnDashboard);
            this.panelSidebar.Controls.Add(this.btnReservation);
            this.panelSidebar.Controls.Add(this.btnCheckInCheckOut);
            this.panelSidebar.Controls.Add(this.btnBilling);
            this.panelSidebar.Controls.Add(this.btnLogin);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(234, 1040);
            this.panelSidebar.TabIndex = 1;
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.Silver;
            this.btnLogout.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnLogout.Location = new System.Drawing.Point(15, 973);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(204, 45);
            this.btnLogout.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.btnLogout.TabIndex = 6;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            // 
            // lblRole
            // 
            this.lblRole.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblRole.Location = new System.Drawing.Point(17, 28);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(205, 30);
            this.lblRole.TabIndex = 0;
            this.lblRole.Text = "ROLE: FRONT DESK";
            // 
            // btnDashboard
            // 
            this.btnDashboard.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnDashboard.Location = new System.Drawing.Point(15, 86);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(204, 46);
            this.btnDashboard.TabIndex = 1;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = true;
            // 
            // btnReservation
            // 
            this.btnReservation.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnReservation.Location = new System.Drawing.Point(15, 144);
            this.btnReservation.Name = "btnReservation";
            this.btnReservation.Size = new System.Drawing.Size(204, 46);
            this.btnReservation.TabIndex = 2;
            this.btnReservation.Text = "Reservation";
            this.btnReservation.UseVisualStyleBackColor = true;
            // 
            // btnCheckInCheckOut
            // 
            this.btnCheckInCheckOut.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnCheckInCheckOut.Location = new System.Drawing.Point(15, 202);
            this.btnCheckInCheckOut.Name = "btnCheckInCheckOut";
            this.btnCheckInCheckOut.Size = new System.Drawing.Size(204, 46);
            this.btnCheckInCheckOut.TabIndex = 3;
            this.btnCheckInCheckOut.Text = "Check-In / Check-Out";
            this.btnCheckInCheckOut.UseVisualStyleBackColor = true;
            // 
            // btnBilling
            // 
            this.btnBilling.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnBilling.Location = new System.Drawing.Point(15, 260);
            this.btnBilling.Name = "btnBilling";
            this.btnBilling.Size = new System.Drawing.Size(204, 46);
            this.btnBilling.TabIndex = 4;
            this.btnBilling.Text = "Billing";
            this.btnBilling.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.btnLogin.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.btnLogin.Location = new System.Drawing.Point(15, 973);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(204, 45);
            this.btnLogin.Visible = false;
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Logout";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // FrontDeskDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1920, 1040);
            this.Controls.Add(this.panelSidebar);
            this.IsMdiContainer = true;
            this.Name = "FrontDeskDashboard";
            this.Text = "Beyond Just Place - Front Desk";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrontDeskDashboard_Load);
            this.panelSidebar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Button btnLogout;
    }
}


