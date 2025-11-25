# ManResort

ASP.NET Core 8 Razor Pages site for Mangam Resort & Guesthouse. Visitors can browse accommodations, submit contact requests, buy day-pass tickets, pay with Yoco, and receive QR-coded tickets by email. A simple API endpoint lets staff validate and redeem tickets at the gate.

## Features
- Ticket purchase flow with per-guest pricing (weekend/holiday rates, cooler boxes, business entrances).
- Yoco card checkout popup (tokenized) and server-side charge request.
- QR code generation (QRCoder) for each ticket; PNG saved to `wwwroot/qrCodes` and attached to the confirmation email.
- Azure SQL-backed ticket storage via EF Core; initial migration creates the `Tickets` table.
- Contact form sends inquiries via SMTP.
- Accommodation page with rate cards; marketing-heavy home page with gallery/CTA; success/failure pages for payments.
- API `POST api/TicketValidation/ValidateQRCode` to mark tickets redeemed.

## Tech Stack
- .NET 8, Razor Pages
- EF Core 9 + SQL Server
- Yoco Web SDK (client) + Yoco charge API (server)
- QRCoder for QR images
- SMTP email (System.Net.Mail); Azure Blob SDK available for QR uploads (helper stub present)

## Prerequisites
- .NET 8 SDK
- SQL Server instance (local or Azure) reachable from the app
- Yoco public and secret keys
- SMTP credentials for outbound mail
- (Optional) Azure Blob Storage account/container for QR uploads

## Configuration
Keep secrets out of source control. Use [dotnet user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables instead of committing `appsettings.json`.

Required keys (matching current code paths):
```jsonc
{
  "ConnectionStrings": {
    "AzureTicketDb": "Server=...;Database=...;User Id=...;Password=...;Encrypt=True;TrustServerCertificate=False;",
    "AzureBlobStorage": "DefaultEndpointsProtocol=...;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net"
  },
  "EmailSettings": {
    "SMTPHost": "...",
    "SMTPPort": 587,
    "FromEmail": "...",
    "Password": "..."
  },
  "Yoco": {
    "PublicKey": "pk_test_or_live",
    "SecretKey": "sk_test_or_live"
  }
}
```

Example user-secrets setup (from repo root):
```powershell
cd ManResort
dotnet user-secrets init --project ManResort/ManResort.csproj
dotnet user-secrets set "ConnectionStrings:AzureTicketDb" "<connection-string>"
dotnet user-secrets set "ConnectionStrings:AzureBlobStorage" "<blob-connection-string>"
dotnet user-secrets set "EmailSettings:SMTPHost" "<smtp-host>"
dotnet user-secrets set "EmailSettings:SMTPPort" "587"
dotnet user-secrets set "EmailSettings:FromEmail" "<from-email>"
dotnet user-secrets set "EmailSettings:Password" "<smtp-password>"
dotnet user-secrets set "Yoco:PublicKey" "<yoco-public-key>"
dotnet user-secrets set "Yoco:SecretKey" "<yoco-secret-key>"
```

Notes:
- `Tickets.cshtml.cs` currently reads `_configuration["Yoco:SecretKey"]` but `appsettings.json` ships with `YocoSecretKey`. Align keys in config (recommended) or update the code to match.
- `TicketConfirmation.cshtml` has a hardcoded Yoco public key string; swap it for your key via config or a view model to avoid leaking secrets.
- Rotate the credentials present in the committed `appsettings.json` before deploying anywhere sensitive.

## Running Locally
```powershell
cd ManResort
dotnet restore
dotnet ef database update --project ManResort/ManResort.csproj   # creates Tickets table
dotnet run --project ManResort/ManResort.csproj
```
Browse to the indicated `http://localhost` / `https://localhost` URL.

## Ticket Flow
1) `/Tickets` collects guest counts and contact info; price is calculated as  
   `Adults*80 + Kids*50 + HolidayAdults*100 + HolidayKids*70 + CoolerBoxes*20 + BusinessEntrance*120 + BusinessElectricEntrance*220`.
2) `/TicketConfirmation` shows a summary and opens the Yoco popup; amount sent in cents.
3) On success, `Tickets/ProcessPayment` charges the card, stamps `PaymentStatus`/`PaymentReference`, stores the ticket, generates a QR PNG, and emails the buyer.
4) `/PaymentSuccess` or `/PaymentFailed` informs the guest.

## Data & Validation API
- Database schema lives in the `Migrations/` folder; initial migration creates the `Tickets` table with redemption, payment, and QR fields.
- Validation endpoint for gate staff:  
  `POST /api/TicketValidation/ValidateQRCode` with JSON body `123` (ticketId as an integer).  
  Returns 400 if missing/duplicate; marks `IsRedeemed=true` on success.

## Email & QR Codes
- Emails send via `System.Net.Mail.SmtpClient` using `EmailSettings` values; the QR PNG is attached if present.
- QR codes are saved under `wwwroot/qrCodes`. A helper `SaveToBlobStorage` exists if you want to push QR images to Azure Blob Storage—call it after generation if desired.

## Project Structure
- `Pages/` Razor Pages for Home, Tickets, TicketConfirmation, PaymentSuccess/Failed, Accommodation, Contact, Privacy.
- `Controller/TicketValidationController.cs` API for redemption.
- `Data/TicketDBContext.cs`, `Model/Ticket.cs`, `Migrations/` for persistence.
- `wwwroot/` static assets (CSS/JS/images, generated QR codes).

## Development Tips
- Run `dotnet watch run` during UI iteration.
- Update prices or add new ticket options in `Pages/Tickets.cshtml` and the `CalculateTotalPrice` method in `Tickets.cshtml.cs`.
- No automated tests are present; consider adding coverage around payment processing and ticket validation before production use.

