# Beyond Just Place - Local Database

The application uses SQL Server Express LocalDB only. It does not use Azure SQL.

## One-time requirement on each Windows computer
Install **SQL Server Express LocalDB**. If Visual Studio is installed, open Visual Studio Installer > Modify > Individual components and install SQL Server Express LocalDB / the SQL data tools component.

## Automatic setup
Run the UI project. Before Login opens, `DatabaseInitializer.Initialize()`:
1. starts/connects to `(localdb)\MSSQLLocalDB`;
2. creates `BeyondJustPlaceDB` if missing;
3. creates/upgrades Users, Guests, Rooms, Reservations and Payments;
4. seeds default users and starter rooms.

No MDF/database file needs to be committed to GitHub. Each teammate gets a private local database.

## Default local accounts
- Admin: `admin` / `admin123`
- Front Desk: `frontdesk` / `frontdesk123`

Change these accounts from Admin Dashboard > Manage User after first login.
