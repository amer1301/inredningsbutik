# inredningsbutik
En ASP.NET Core MVC e-handelsapplikation byggd med tydlig lagerarkitektur, transaktionshantering och central felhantering.

Projektet är utvecklat med fokus på ren arkitektur, testbarhet och produktionsredo struktur.

---

## Funktioner

- Produktkatalog med kategorier
- Varukorg
- Checkout med lagerkontroll
- Orderhantering
- Rollbaserad autentisering (Customer/Admin)
- Central felhantering (404, 403, 500)
- Transaktionssäker orderhantering

## Arkitektur

Projektet är uppdelat i tre lager:
Inredningsbutik.Web → Controllers, Views
Inredningsbutik.Core → Entiteter + Interfaces
Inredningsbutik.Infrastructure → Services + DbContext

### Flöde:
Web → IOrderService (Core) → OrderService (Infrastructure) → DbContext

### Designprinciper:

- Interface-baserad service-arkitektur
- Dependency Injection
- Transaktionshantering i service-lagret
- Separation of concerns
- Central felhantering via ErrorController

---

## Tekniker

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- Dependency Injection
- Logging (ILogger)

---

## Orderhantering

Orderhantering sker i `OrderService` och inkluderar:

- Explicit databastransaktion
- Lagerkontroll
- Lageruppdatering
- Order + OrderItems sparas atomiskt
- Rollback vid fel

Detta säkerställer dataintegritet.

---

## Felhantering

Projektet använder:

- `UseExceptionHandler` (500)
- `UseStatusCodePagesWithReExecute` (404, 403, 401)
- Central `ErrorController`
- Loggning via ILogger
- TraceId för felsökning

---

## Köra projektet lokalt

### Klona repo

```bash
git clone https://github.com/amer1301/inredningsbutik.git
cd inredningsbutik
```

### Kör projektet
```bash
dotnet run --project src/Inredningsbutik.Web
```
Applikationen startar på: http://localhost:5162

---

## Databas
Projektet använder SQL Server.

Migrationer appliceras automatiskt vid start.

Vill du uppdatera manuellt:
```bash
dotnet ef database update --project src/Inredningsbutik.Infrastructure --startup-project src/Inredningsbutik.Web
```
---

## Testa felhantering

Testa:
- 404 → /abc123
- 403 → Försök nå skyddad route utan rätt roll
- 500 → Släng ett test-exception i en controller

## DOCKER - .....
