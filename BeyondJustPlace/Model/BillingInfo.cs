namespace Model
{
    public class BillingInfo
    {
        public int ReservationId { get; set; }
        public string GuestName { get; set; }
        public string RoomNumber { get; set; }

        public int StayDays { get; set; }

        public decimal RoomRate { get; set; }
        public decimal RoomCharges { get; set; }
        public decimal OtherCharges { get; set; }
        public decimal TotalBill { get; set; }

        public string ReservationStatus { get; set; }
    }
}