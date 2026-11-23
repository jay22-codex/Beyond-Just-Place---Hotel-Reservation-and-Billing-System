BEYOND JUST PLACE — FRONT DESK / GUEST BOOKING WORKFLOW UPDATE
============================================================

Based on the latest complete WinForms guest-booking project available in this chat:
BeyondJustPlace_WINFORMS_GUEST_BOOKING_SOURCE.zip

WHAT CHANGED
------------
1. Browse Rooms remains the ONLY guest-facing availability-and-rate selection page.
   Its labels no longer overlap. Four Standard/Deluxe/Family/Suite photos remain
   embedded, with a safer fit/preview and fallback to RoomImages folder.

2. Guest Booking Details displays selected rooms/dates/rates and asks for guest
   identity/contact and ID number. Submit saves a Pending reservation. Staff
   must inspect the physical ID to verify identity later. The Guest Details
   footer/form field layout adapts to the actual form width.

3. Front Desk > Reservation is now a reservation-record review UI. No duplicate
   room-type selection or availability grid. Staff search Pending/Confirmed/etc.
   bookings, review room/guest details, compare ID number to the physical ID,
   then mark identity verified. Status changes respect existing DB rules.

4. Front Desk > Dashboard now displays a searchable DATA TABLE of bookings,
   guests, contact, rooms, beds, guest count, dates, deposit/ID/status. It still
   refreshes periodically.

5. Front Desk > Check-In now displays detailed guest/room/dates/ID/key details
   automatically when a row is selected; there is no View Details button.
   The normal Check-In must be Confirmed and ID-verified. It now includes a
   separate 'New Walk-In Check-In' button.

6. WalkInCheckInForm is a NEW WinForms form. It checks availability for walk-in
   dates, allows multiple room selections (Ctrl+Click), validates combined room
   capacity, saves guest/contact/ID info, requires staff physical ID checking,
   records any required reservation fee, and checks in the guest. GCash uses
   your pre-existing QR -> OK -> reference flow, contingent on an authorized QR.

7. Billing's Food/Laundry buttons have been moved beneath the charges box so
   they do not cover the Total Bill values.

WHAT DID NOT CHANGE
-------------------
* LocalDB engine/name and automatic database initializer.
* Existing reservation/billing/payment/receipt logic, except layout and display.
* Original four room photo files.
* Staff Login, Admin Dashboard, and sidebar navigation.

HOW TO RUN
----------
1. BACK UP your current solution and LocalDB database.
2. Extract the FULL SOURCE ZIP to a new folder. Open BeyondJustPlace.sln.
3. Set 'UI' as startup project. Ensure SQL Server Express LocalDB is installed.
4. Build > Rebuild Solution.
5. Test Browse Rooms -> Guest Booking -> Pending -> Staff Reservation Review.
6. Verify physical ID using exact ID number entered during booking.
7. Confirm deposit in the existing payment workflow and Confirm reservation.
8. Open Check-In. Select row; details should appear automatically.
9. Open New Walk-In Check-In and test one room then a multi-room group.
10. Open Billing; confirm buttons no longer cover Total Bill.
11. Rebuild BeyondJustPlaceSetup in Release to package updated EXE.

CAUTIONS / RUNTIME TEST STATUS
------------------------------
* This environment cannot compile or run .NET Framework 4.7.2 WinForms and
  SQL Server LocalDB, so this is SOURCE-CHECKED, not Windows-runtime-certified.
* The automatic LocalDB initialization still requires installed LocalDB.
* GCash QR must be the HOTEL'S REAL QR, configured in PaymentAssets; this does
  not connect to GCash's transaction-verification API.
* During walk-in, if a DB failure happens AFTER booking creation, that Pending
  booking is retained; find/complete it instead of creating a duplicate.
* Do not use your previous compiled MSI; rebuild the installer from source.
* A guest's self-entered ID number is not proof of identity; staff must compare
  it against the original physical document before checking in.
