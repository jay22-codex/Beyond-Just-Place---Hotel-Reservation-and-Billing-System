using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    internal static class UiPolish
    {
        // Styling only: does not change SQL logic, names, locations or event handlers.
        public static void Apply(Control root)
        {
            foreach (Control child in root.Controls)
            {
                DataGridView grid = child as DataGridView;
                if (grid != null)
                {
                    grid.BackgroundColor = Color.White;
                    grid.GridColor = Color.FromArgb(218, 213, 233);
                    grid.BorderStyle = BorderStyle.FixedSingle;
                    grid.RowHeadersVisible = false;
                    grid.AllowUserToResizeRows = false;
                    grid.RowTemplate.Height = 30;
                    grid.ColumnHeadersHeight = 38;
                    grid.EnableHeadersVisualStyles = false;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(82, 66, 140);
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.ColumnHeadersDefaultCellStyle.Font =
                        new Font("Arial", 9.5F, FontStyle.Bold);
                    grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 245, 251);
                    grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 205, 247);
                    grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 28, 71);
                    grid.DefaultCellStyle.Font = new Font("Arial", 9.5F);
                    grid.DefaultCellStyle.Padding = new Padding(3, 1, 3, 1);
                }
                Button button = child as Button;
                if (button != null)
                {
                    button.Cursor = Cursors.Hand;
                    button.FlatStyle = FlatStyle.Standard;
                }
                if (child.HasChildren) Apply(child);
            }
        }
    }
}
