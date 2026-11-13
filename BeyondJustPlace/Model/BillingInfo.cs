namespace Model
{
    public class BillingInfo
    {
        public int ReservationId { get; set; }

        public string GuestName { get; set; }

        public string GuestEmail { get; set; }

        public string GuestContactNumber { get; set; }

        public string GuestAddress { get; set; }

        public string GuestIdType { get; set; }

        public string RoomNumber { get; set; }

        public string RoomType { get; set; }

        public int StayDays { get; set; }
        public int RoomCount { get; set; }

        public decimal RoomRate { get; set; }

        public decimal RoomCharges { get; set; }

        public decimal OtherCharges { get; set; }

        public decimal TotalBill { get; set; }

        public string ReservationStatus { get; set; }

        public System.DateTime? CheckInDate { get; set; }

        public System.DateTime? CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }

        public string SpecialRequest { get; set; }
    }
}