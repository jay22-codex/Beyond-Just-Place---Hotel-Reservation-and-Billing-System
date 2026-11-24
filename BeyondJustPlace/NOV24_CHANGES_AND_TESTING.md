# Beyond Just Place – November 24 WinForms update

## What changed
- Admin Add/Edit User dialogs now include **Confirm Password** with masked input, equality check, at least 8 characters for changed passwords, and blank-both-to-keep on Edit.
- Admin Dashboard no longer uses the sparse Latest Activity list: it shows a full read-only, searchable reservation DataGridView including guest/contact, email, room numbers, beds, guest counts, dates, ID checked, deposit and status.
- Admin Dashboard responsive arrangement: four compact KPI tiles, compact Quick Links row, full-width reservation table. Same purple/gray palette and navigation buttons.
- Guest Browse Rooms now automatically refreshes capacity-filtered room types when guest/room counts or dates change. It considers the combined capacity of the requested rooms (including mixed room types). If no possible combination exists, it shows a warning and disables continuation.
- LocalDB initial seed fills each default room type (Standard, Deluxe, Family, Suite) to **20 rooms total each** (at least 80 rooms overall) using collision-safe room numbers (e.g. 11001...): existing rooms/reservations remain untouched, no duplicate additions on restarts.
- Common WinForms table styles improved across Admin/Front Desk while preserving button colors and names.
- Fixed reported `Math` CS0103 in WalkInCheckInForm.Designer.cs using `System.Math`.

## Required Windows tests (this environment cannot run WinForms + LocalDB)
1. Backup your existing LocalDB database.
2. Open solution in Visual Studio 2022, **Build > Rebuild Solution**. Ensure 0 errors.
3. Start app with a fresh local database: confirm 20 Standard, 20 Deluxe, 20 Family, and 20 Suite rooms; rerun and confirm counts do not increase.
4. On existing DB: confirm no existing room/guest/reservation was deleted; count should be at least 20 per default type after first launch.
5. Admin > Manage User: mismatched password confirmation should block Save; matching passwords should save. Edit with both password fields blank should preserve password.
6. Admin Dashboard: verify read-only guest reservation grid, search, responsive layout at 1920x1080 and smaller widths.
7. Browse Rooms: Adults=3, Children=4, Rooms wanted=1 should warn (default max capacity 5). Rooms wanted=2 should offer feasible combined rooms.
8. Front Desk: verify room ID checks, booking, check-in/out, billing and receipt still work.
9. After tests, rebuild the setup project in Release and replace the installer binaries before committing to GitHub.

**Note:** The rooms are sample inventory, and their availability depends on existing reservations and room activation status. Room number prefixes for newly seeded rooms are `11`, `21`, `31`, `41` respectively. Existing room numbers are unchanged.
