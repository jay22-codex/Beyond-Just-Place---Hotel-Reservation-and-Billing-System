
namespace Model
{
    public class Guest
    {
        public int GuestId { get; set; }

        public string GuestName { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }

        public string HomeAddress { get; set; }

        public string IdType { get; set; }

        public override string ToString()
        {
            return GuestName + " - " + ContactNumber;
        }
    }
}
