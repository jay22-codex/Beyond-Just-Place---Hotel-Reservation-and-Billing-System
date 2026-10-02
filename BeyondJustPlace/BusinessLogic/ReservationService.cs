using System;
using System.Collections.Generic;
using BusinessLogic.Repository;
using Model;

namespace BusinessLogic
{
    public class ReservationService
    {
        private ReservationRepository repository =
            new ReservationRepository();

        public List<Guest> SearchGuest(string keyword)
        {
            return repository.SearchGuest(keyword);
        }

        public Guest CreateGuest(string guestName, string contactNumber)
        {
            if (guestName.Trim() == "")
                throw new Exception("Please enter guest name.");

            if (contactNumber.Trim() == "")
                throw new Exception("Please enter contact number.");

            int guestId = repository.CreateGuest(
                guestName,
                contactNumber);

            Guest guest = new Guest();

            guest.GuestId = guestId;
            guest.GuestName = guestName;
            guest.ContactNumber = contactNumber;

            return guest;
        }

        public List<string> GetRoomTypes()
        {
            return repository.GetRoomTypes();
        }

        public List<Room> GetAvailableRooms(
            DateTime checkIn,
            DateTime checkOut,
            string roomType)
        {
            if (checkOut.Date <= checkIn.Date)
                throw new Exception(
                    "Check-out date must be later than check-in date.");

            if (roomType == "")
                throw new Exception(
                    "Please select a room type.");

            return repository.GetAvailableRooms(
                checkIn,
                checkOut,
                roomType);
        }

        public int CreateReservation(
            Guest guest,
            Room room,
            DateTime checkIn,
            DateTime checkOut,
            string status)
        {
            if (guest == null)
                throw new Exception(
                    "Please select a guest.");

            if (room == null)
                throw new Exception(
                    "Please select a room.");

            if (checkOut.Date <= checkIn.Date)
                throw new Exception(
                    "Check-out date must be later than check-in date.");

            if (status == "")
                throw new Exception(
                    "Please select reservation status.");

            return repository.CreateReservation(
                guest.GuestId,
                room.RoomId,
                checkIn,
                checkOut,
                status);
        }

        public void CheckInGuest(int reservationId)
        {
            if (reservationId <= 0)
            {
                throw new Exception("Invalid reservation.");
            }

            repository.CheckInGuest(reservationId);
        }

        public bool CancelReservation(Guest guest)
        {
            if (guest == null)
            {
                throw new Exception("Please select a guest.");
            }

            int result =
                repository.CancelReservation(guest.GuestId);

            return result > 0;
        }

        public int GetTodayReservationCount()
        {
            return repository.GetTodayReservationCount();
        }

        public int GetCurrentCheckedInCount()
        {
            return repository.GetCurrentCheckedInCount();
        }

        public int GetExpectedCheckInsTodayCount()
        {
            return repository.GetExpectedCheckInsTodayCount();
        }

        public int GetPendingReservationCount()
        {
            return repository.GetPendingReservationCount();
        }

        public List<string> GetLatestActivity()
        {
            return repository.GetLatestActivity();
        }
    }
}