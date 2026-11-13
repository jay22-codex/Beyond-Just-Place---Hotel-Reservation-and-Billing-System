# Beyond Just Place - Multi-Room Reservation Update

## What changed
- One ReservationId can now contain multiple actual rooms.
- The existing Rooms selector controls how many rooms the guest wants.
- In the availability table, hold Ctrl and select multiple room rows, then click Add Room(s).
- Selected rooms may come from different room types. Switch room type, check availability, and add more until the requested room count is reached.
- The existing hidden button is reused as Clear Selected.
- The existing PictureBox now displays the four supplied room photos: Standard, Deluxe, Family, and Suite. The photos are embedded in the UI assembly, so they are available offline and after installation.
- Reservation Summary shows all room numbers, combined nightly rate, and combined reservation fee.
- Capacity validation uses the combined capacity of all selected rooms when capacities are configured.
- Check-In shows all rooms belonging to the reservation.
- Billing totals all reserved room rates for every night.
- Receipt/email lists all rooms and uses the combined room total.
- Admin reservation/payment/latest-activity lists display the multi-room booking correctly.

## LocalDB migration
The app automatically creates dbo.ReservationRooms and backfills existing one-room reservations. Existing records are preserved.

## Important installer note
The installer EXE/MSI from before this update does not contain the new compiled code. Rebuild the UI in Release, then rebuild BeyondJustPlaceSetup and replace the Installer folder outputs before pushing the installer.
