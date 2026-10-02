using System;
using BusinessLogic.Repository;
using Model;

namespace BusinessLogic
{
    public class BillingService
    {
        private BillingRepository repository =
            new BillingRepository();

        public BillingInfo SearchGuest(
            string guestName)
        {
            if (guestName.Trim() == "")
                throw new Exception(
                    "Please enter guest name.");

            return repository.SearchGuest(
                guestName);
        }

        public void ConfirmPayment(
            BillingInfo billing,
            string paymentMethod)
        {
            if (billing == null)
                throw new Exception(
                    "No billing information found.");

            if (paymentMethod == "")
                throw new Exception(
                    "Please select payment method.");

            if (repository.IsPaid(
                billing.ReservationId))
            {
                throw new Exception(
                    "This reservation is already paid.");
            }

            repository.SavePayment(
                billing.ReservationId,
                billing.TotalBill,
                paymentMethod);
        }
    }
}