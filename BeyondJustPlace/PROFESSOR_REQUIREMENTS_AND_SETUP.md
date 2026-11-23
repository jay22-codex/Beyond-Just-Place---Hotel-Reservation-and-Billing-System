# Beyond Just Place — Professor Requirements Update

This is the **updated C# WinForms/.NET Framework 4.7.2 source project**. It uses **SQL Server Express LocalDB on each PC**, with automatic database creation/migration at application startup. It is **not a prebuilt or runtime-tested EXE**; Visual Studio/Windows and LocalDB are needed for final build and testing.

## What changed

| Requirement | Implementation |
| --- | --- |
| Login, Admin and Front Desk roles | Database-backed authentication, active/inactive accounts, basic input validation, improved purple/gray Login UI and actual app logo. Legacy demo passwords upgrade to salted PBKDF2 hashes on successful login. |
| Room Types and rate maintenance | New **Admin > Room Type Setup** with rate, beds, capacity, room area, description, fee, and active/inactive state. |
| Room maintenance | Room number uniqueness checks, beds/size/description and active toggle instead of hard delete. Internal Room ID is hidden in the management table. |
| Availability | **Admin > Availability View** and the Front Desk date-scoped availability table. |
| Guest maintenance | Search/view/add/edit, ID number/type, manually checked ID, active/inactive toggle, and optional button creating 50 clearly marked **DEMO** records. Existing reservations are not deleted on deactivation. |
| Reservation | Room Type shown before Guest Information, separate Available/Selected Room tables, existing four embedded room photos, one reservation can hold multiple rooms, and room capacity must cover guest count. Adults + children must equal guest count. |
| Reservation summary | Grand total = room charges across selected rooms for booked nights + reservation fee; includes status and room list. |
| Reservation status | Pending after creation; Confirmed when a staff member verifies deposit or Admin confirms a zero-deposit booking; Cancelled through Admin. Paid-deposit cancellations are prevented until a manual refund process exists. |
| Check-In/Check-Out | Detailed guest/room/ID/deposit screen; physical-ID verification and key-handover confirmation; check-in requires Confirmed status and checked ID. Check-out leads to Billing. |
| Food/Laundry | Add-on quantity and price stored in `ReservationServices` and included in billing. |
| Billing | Combined multi-room charges plus Food/Laundry, less a verified reservation-fee deposit; Cash, Card, GCash. Staff-confirmed payment updates the reservation to Checked Out and stores receipt data. |
| GCash QR and reference | After choosing GCash, show the configured **real hotel QR**, then `OK - Payment Sent`, then prompt for the guest's reference and require staff to confirm it; unique reference saved to SQL/receipt. Both reservation deposit and checkout use the flow. |
| Receipt/email | Existing HTML printable receipt + EmailJS remain; receipt contains GCash reference and shows paid deposit adjustment. Email delivery depends on a properly configured EmailJS account/network. |
| Dates | The Reservation UI shows **standard check-in 2 PM / check-out 12 PM** as informational hotel times. Date fields store date-only values; actual time-enforcement is not implemented. |
| Installer | `BeyondJustPlaceSetup` source project included; **installer output must be rebuilt**. Old MSI/EXE binaries are deliberately not packaged. |

## GCash setup — required before testing GCash

**The user's QR image was not included in the upload.** Never make up a GCash destination or ask a guest to pay a placeholder. To enable the feature:

1. Obtain the authorized hotel/merchant GCash QR picture (PNG).
2. Save it as `UI/PaymentAssets/GCashQR.png` in the solution.
3. Reopen/reload the UI project if necessary, then **Rebuild UI and Setup Project**. The `.csproj` automatically copies this image to the installed app when the file exists.
4. The QR path can be changed with the `GCashQrImage` entry in `UI/App.config`.
5. Selecting GCash displays the QR. The staff must verify actual receipt in the GCash account before entering and approving a reference. This is **not** an automated GCash gateway or payment confirmation service.

Without that actual image, a clear message is displayed and GCash payments are *not* recorded. Cash/Card are manual staff-confirmed payments.

## LocalDB migration

Run `UI` after a successful rebuild. The initializer adds fields/tables to each computer's existing `BeyondJustPlaceDB`; it does **not** drop reservation history. New tables: `RoomTypes` and `ReservationServices`. Existing `ReservationRooms` is retained. Make a backup of any important local DB first.

## Test checklist on Windows + LocalDB

1. Rebuild the solution in **Release** and verify **0 compilation errors**.
2. Log in as Admin, verify Room Type Setup, active/disabled types, room duplicate-number validation, room details and availability.
3. Open Guest Maintenance. Test add/edit/search/deactivate/reactivate, ID checker. Use **Create 50 DEMO Guests** only for demonstration if required.
4. Log in as Front Desk, save a guest with valid ID info; select multiple rooms; deliberately exceed total capacity and confirm it is blocked. Then make a valid reservation and check Grand Total.
5. Verify reservation fee payment. A Pending reservation should not Check-In until confirmed, and guest ID must be marked verified. Confirm physical ID and key issuance.
6. Add Food/Laundry charges during stay. Check room and guest details; confirm deposit is deducted in Billing.
7. Test Cash checkout and printable receipt. With actual merchant QR configured, test GCash and ensure QR > OK > reference > staff verification is required. Check the Admin Billing Records list for reference.
8. Test receipt email using your own guest email and check EmailJS History if it fails. EmailJS may require non-browser access and account-specific private-key security; do **not** commit secret private keys to GitHub.
9. Rebuild `BeyondJustPlaceSetup` and copy freshly built `setup.exe` + `.msi` to your Git-tracked `Installer/` folder if you want them included when teammates pull.
10. Test setup on another Windows computer with .NET Framework and SQL Express LocalDB; its database should be created when it first launches the app.

## Limitations and security

- No Windows/.NET Framework/SQL Server LocalDB runtime is available in this build environment; **actual GUI/runtime/transaction testing is outstanding**. Static source, XML, merge-marker, referenced-file, and event-structure checks were performed.
- ID verification and GCash receipt checking are **manual staff attestations**, not government-ID validation or automatic bank verification.
- “50 records” is available as explicitly labelled demo data; it does not imply 50 real hotel guests.
- Deactivation preserves history. Guest ID numbers are sensitive; restrict database backups and protect machines/accounts before using real customer data.
- Default school/demo accounts should be changed or disabled before deployment outside a controlled demo. GCash Private Keys must never be placed in version control or bundled desktop source/distribution.
