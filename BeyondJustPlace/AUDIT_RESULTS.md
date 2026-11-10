# Beyond Just Place - Source Audit

Checked on 2026-11-10.

## Passed static checks
- No unresolved Git merge markers.
- UI/App.config is valid XML and contains LocalDB + EmailJS settings.
- UI, BusinessLogic, Model and DB project XML files parse successfully.
- All WinForms partial class names match their Designer classes.
- Designer-declared event handlers have matching methods.
- Billing code references controls that exist in its Designer.
- Reservation guest email is saved to dbo.Guests and loaded into BillingInfo.
- Checkout saves SQL payment first, then creates receipt, then attempts EmailJS.
- LocalDB initializer creates/updates the local database at application startup.
- Admin and Front Desk dashboard refresh timers are present.

## Fixed in this audited copy
1. EmailJsReceiptService now uses TLS 1.2, a 30-second timeout, and detailed HTTP/network errors.
2. DB/Table/Payments.sql now matches the runtime Payments schema (AmountReceived, ChangeAmount, ProcessedBy).
3. Added .gitignore for Visual Studio/build/database-generated files.

## Functional limitations still present
- Walk-in check-in workflow is not implemented in the current Check-In/Check-Out form.
- Reservation fee payment method is recorded, but the reservation fee is not actually posted as a payment/deposit or deducted from final checkout balance.
- Other charges are currently always 0.00; there is no service/extra-charge entry workflow.
- Login passwords are stored as plain text. This is acceptable only for a classroom/demo project, not production.
- BillingService/BillingRepository are legacy/unused; current Billing UI uses FrontDeskOperationsService.

## Runtime test still required on Windows
This environment cannot run .NET Framework WinForms + SQL Server LocalDB. In Visual Studio run:
1. Build > Clean Solution
2. Build > Rebuild Solution
3. Use a new guest with a real email
4. Create reservation > Check-In > Billing > Confirm Payment
5. Check EmailJS Email History if the email does not arrive
