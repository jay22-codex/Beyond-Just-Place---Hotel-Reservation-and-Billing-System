# Beyond Just Place — Roles, verification and DataGridView update

## What changed

- **Front Desk only:** guest physical-ID verification, reservation-fee verification (including GCash QR + reference), booking confirmation and cancellation, check-in / key handover, walk-in verification, and final billing confirmation.
- **Admin:** manages room types/rates, rooms, user accounts and guest records; checks availability and views reservation/payment tables **read-only**. There are no buttons in Admin for verifying IDs or fees or changing bookings.
- **Guest Maintenance:** editing a guest does not verify the guest's ID. Changing the ID number automatically clears any prior verification through `ReservationDetailsService.SaveGuest`.
- **Reservation review:** clear `Verify Guest ID` button and physical-ID checkbox, plus a `Verify Reservation Fee` button. Front Desk enters a GCash receipt reference after viewing the actual QR image.
- **Check-In:** visible `Physical ID checked` checkbox, ID-number entry, `Verify Selected ID` button, and `Room key(s) handed over` checkbox; details are shown automatically when a row is selected. Check-In requires a confirmed booking and verified guest.
- **Tables:** DataGridViews use sensible column filling, compact row height, no row headers, and smaller visible column sets; detail sections retain the hidden information.
- **Designer repair:** includes the regular Designer-compatible `FrontDesk-ReservationUI.Designer.cs` file from the latest fix. The original purple/gray palette and naming are retained.

## Installing the update

1. Back up the current solution and LocalDB database.
2. Extract the full source ZIP. Open `BeyondJustPlace.sln`.
3. Set **UI** as the Startup Project and **Build → Rebuild Solution**.
4. Test Admin login: Room Type Setup / Manage Rooms / Manage Users / Guest Maintenance; View Reservations must be read-only.
5. Test Front Desk login: Reservation → select a pending booking → check physical ID → enter its number → click Verify Guest ID.
6. On a booking with an unpaid reservation fee, use **Verify Reservation Fee**; for GCash, the real hotel QR must be configured and the reference must be verified by staff.
7. Open Check-In → select a guest in the table → review details automatically → verify ID if needed → tick key handover → Confirm Check-In.
8. Confirm Billing still creates receipts and sends EmailJS receipts when configured.
9. Rebuild `BeyondJustPlaceSetup` after testing. Previous setup binaries do not contain this update.

**Security limitation:** Role separation is applied in the WinForms UI through `CurrentUserSession`; it is not a separate hardened authorization service. The local SQL database still has Windows-user-level access. This is suitable for a school demo but not a production security boundary.

**Testing limitation:** These files were structurally checked here, not compiled or run on Windows with .NET Framework and LocalDB.
