BEYOND JUST PLACE - IMPORTANT

This version contains updated source code; the old setup.exe/MSI was built
BEFORE these changes. To distribute the new version:

1. Open BeyondJustPlace.sln in Visual Studio with the Microsoft Installer
   Projects extension installed.
2. Build the UI project in Release mode.
3. Right-click BeyondJustPlaceSetup -> Rebuild.
4. Copy the NEW setup.exe and BeyondJustPlaceSetup.msi from
   BeyondJustPlaceSetup\Release into this Installer folder.
5. Commit/push the updated installer files. Ask teammates to run setup.exe.

Requirements on another PC: .NET Framework 4.7.2 (or compatible higher)
and SQL Server Express LocalDB; the app auto-creates its local database on
first run. Sending email requires internet and a configured EmailJS account.
GCash payment requires an authentic hotel QR saved in
UI\PaymentAssets\GCashQR.png BEFORE rebuilding the installer.
