using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BusinessLogic;

namespace UI
{
    internal sealed class GuestBookingSelection
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public List<AvailableRoomInfo> Rooms { get; set; }
        public int Guests { get { return Adults + Children; } }
        public int Nights { get { return Math.Max(1, (CheckOut.Date - CheckIn.Date).Days); } }
        public int Capacity { get { return Rooms.Sum(r => r.RoomCapacity.GetValueOrDefault()); } }
        public decimal NightlyRate { get { return Rooms.Sum(r => r.RoomRate); } }
        public decimal Total { get { return NightlyRate * Nights; } }
        public decimal DepositEstimate { get { return Rooms.Sum(r => r.ReservationFee.GetValueOrDefault()); } }
    }

    internal static class GuestBookingLook
    {
        public static readonly Color Purple = Color.FromArgb(128, 128, 255);
        public static readonly Color DarkPurple = Color.FromArgb(78, 65, 142);
        public static readonly Color LightPurple = Color.FromArgb(235, 233, 253);
        public static readonly Color Canvas = Color.FromArgb(246, 246, 250);
        public static readonly Color Border = Color.FromArgb(218, 215, 231);
        public static readonly Color Ink = Color.FromArgb(39, 37, 55);
        public static readonly Color Muted = Color.FromArgb(100, 99, 117);
        private static readonly Dictionary<string, Image> CachedPhotos =
            new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        public static Label Label(string text, float size = 11, bool bold = false)
        {
            return new Label
            {
                Text = text,
                ForeColor = Ink,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        public static Button Button(string text, bool primary = false)
        {
            return new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = primary ? DarkPurple : Color.White,
                ForeColor = primary ? Color.White : DarkPurple,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Height = 42
            };
        }

        public static Image Photo(string roomType)
        {
            string type = (roomType ?? "").Trim();
            if (type.EndsWith(" Room", StringComparison.OrdinalIgnoreCase))
                type = type.Substring(0, type.Length - 5).Trim();
            string name = type.Equals("Standard", StringComparison.OrdinalIgnoreCase) ? "StandardRoom.png" :
                type.Equals("Deluxe", StringComparison.OrdinalIgnoreCase) ? "DeluxeRoom.png" :
                type.Equals("Family", StringComparison.OrdinalIgnoreCase) ? "FamilyRoom.png" :
                type.Equals("Suite", StringComparison.OrdinalIgnoreCase) ? "SuiteRoom.png" : null;
            if (name == null) return null;
            if (CachedPhotos.ContainsKey(name)) return CachedPhotos[name];
            string resourceName = "UI.RoomImages." + name;
            try
            {
                using (Stream stream = typeof(GuestBookingLook).Assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        string location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RoomImages", name);
                        if (!File.Exists(location)) return null;
                        using (Image fileImage = Image.FromFile(location))
                        {
                            CachedPhotos[name] = new Bitmap(fileImage);
                            return CachedPhotos[name];
                        }
                    }
                    using (Image source = Image.FromStream(stream))
                    {
                        Bitmap bitmap = new Bitmap(600, 410);
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.Clear(Color.FromArgb(242, 241, 248));
                            float scale = Math.Min((float)bitmap.Width / source.Width, (float)bitmap.Height / source.Height);
                            int w = Math.Max(1, (int)(source.Width * scale));
                            int h = Math.Max(1, (int)(source.Height * scale));
                            g.DrawImage(source, new Rectangle((bitmap.Width-w)/2,(bitmap.Height-h)/2,w,h));
                        }
                        CachedPhotos[name] = bitmap;
                        return bitmap;
                    }
                }
            }
            catch { return null; }
        }

        public static string Money(decimal value)
        {
            return "₱" + value.ToString("N2");
        }

        public static string RoomDetails(AvailableRoomInfo room)
        {
            return (room.BedCount > 0 ? room.BedCount.ToString() : "?") + " bed(s)   •   " +
                (room.RoomCapacity.HasValue ? room.RoomCapacity.Value.ToString() : "?") + " guest capacity   •   " +
                (room.SizeSqm > 0 ? room.SizeSqm.ToString() + " m²" : "Size unavailable");
        }
    }
}
