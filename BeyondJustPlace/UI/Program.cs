using System;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The local Beyond Just Place database could not be prepared.\n\n" +
                    ex.Message,
                    "Local Database Setup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Login());
        }
    }
}
