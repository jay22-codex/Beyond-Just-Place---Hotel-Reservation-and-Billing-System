using Model;
namespace Model
{
    public class Room
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }

        public override string ToString()
        {
            return "Room " + RoomNumber + " - " + RoomType;
        }
    }
}