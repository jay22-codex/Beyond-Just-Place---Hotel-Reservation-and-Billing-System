namespace UI
{
    internal static class CurrentUserSession
    {
        public static string Username { get; set; }
        public static string Role { get; set; }

        public static bool IsFrontDesk
        {
            get { return string.Equals(Role, "FrontDesk", System.StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsAdmin
        {
            get { return string.Equals(Role, "Admin", System.StringComparison.OrdinalIgnoreCase); }
        }

        public static bool RequireFrontDesk(System.Windows.Forms.IWin32Window owner)
        {
            if (IsFrontDesk) return true;
            System.Windows.Forms.MessageBox.Show(owner,
                "Only Front Desk staff may verify guest IDs, reservation payments, " +
                "reservation status, and room-key handover.",
                "Front Desk Verification Required",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Warning);
            return false;
        }

        public static bool RequireAdmin(System.Windows.Forms.IWin32Window owner)
        {
            if (IsAdmin) return true;
            System.Windows.Forms.MessageBox.Show(owner,
                "This management feature is available to Admin only.",
                "Admin Access Required",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Warning);
            return false;
        }

        public static void Clear()
        {
            Username = null;
            Role = null;
        }
    }
}
