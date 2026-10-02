namespace Model
{
    public class Guest
    {
        public int GuestId { get; set; }
        public string GuestName { get; set; }
        public string ContactNumber { get; set; }

        public override string ToString()
        {
            return GuestName + " - " + ContactNumber;
        }
    }
}