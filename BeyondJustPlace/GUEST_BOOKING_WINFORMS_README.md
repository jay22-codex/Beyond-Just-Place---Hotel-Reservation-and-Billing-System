# Beyond Just Place — Guest Booking (Windows Forms, NOT a website)

This ZIP contains the **desktop WinForms** solution only. There is no HTML, ASP.NET,
IIS, web server, hosted database, or browser-based application.

## How to run

1. Open `BeyondJustPlace.sln` in Visual Studio 2022 with **.NET desktop development** installed.
2. The app targets **.NET Framework 4.7.2** and needs **SQL Server Express LocalDB** on the PC.
3. Select the **UI** project as Startup Project, then **Build → Rebuild Solution** and press Start.
4. On the existing Staff Login form click **Browse Rooms & Make a Reservation**.
5. Choose check-in/check-out dates, adults/children and desired number of rooms.
6. Click **Check availability**. You will see photo cards, rates, beds, capacity,
   area, descriptions, and real inventory for the selected dates.
7. Click **+ Add room** to pick individual available rooms. Mixed types are allowed.
8. A separate panel lists selected rooms and shows the total estimate.
9. Click **Continue to guest details →**. This second WinForms screen deliberately
   does NOT contain an availability checker. It shows the selected room details,
   stay dates, and estimated total.
10. Enter guest name, contact, email, ID type, ID number, and optional notes.
    Acknowledge Pending status and submit. Save/capacity/date checks occur again in
    LocalDB to avoid overbooking.
11. The booking displays a reference `BJP-R######` and is saved as **Pending**.

## What changed

- `UI/GuestRoomAvailabilityForm.cs` + `.Designer.cs` — first-step availability screen
- `UI/GuestBookingDetailsForm.cs` + `.Designer.cs` — second-step guest details only
- `UI/GuestBookingShared.cs` — fonts, colors, room image resources, selection model
- `UI/Login.cs` — extra *guest booking* entry button (staff login still works)
- `UI/UI.csproj` — register new WinForms source files, fix System.Configuration reference

All four original room photos remain **embedded in the EXE**. The existing Front
Desk Reservation UI and its Designer files were **not edited**.

## Important limitations

- The booking search uses the **local SQL Server LocalDB instance on this PC**.
  It is not publicly accessible online or shared between different computers.
- A guest booking only creates a **Pending reservation** and does not charge a
  card or GCash. Staff can manage the reservation in the same local desktop app.
- No room amenity is invented. Room descriptions, bed counts, and rates come
  from the Admin-maintained `RoomTypes`/`Rooms` data.
- The room availability screen is date-specific; the reservation service rechecks
  conflicts inside its database transaction when finalizing the request.
- Guest details are stored as a separate guest record before the reservation
  is finalized. A failed reservation can therefore leave an unused guest record.
- A real WinForms/LocalDB runtime build and installation must be tested on Windows.

## Rebuild the installer

The prior installer is outdated; rebuild `BeyondJustPlaceSetup` **after** rebuilding
`UI` in **Release** configuration. Then copy the fresh `setup.exe` and MSI into your
repository's `Installer` folder before committing and pushing.
