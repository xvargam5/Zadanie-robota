Pouzite technologie

- .NET 8 
- ASP.NET Core Web API
- Entity Framework Core
- SQLite - pre jednoduchost
- Swagger
- Bonus - Sluzba na pozadi (IHostedService): Implementovana je aj bonusova funkcionalita, ktora kazdy den kontroluje pozicky a vypise do konzoly pripomienku, ak sa blizi termin vratenia.

Spustenie projektu:

Poziadavky:
- .NET 8 SDK

Postup:
1. Obnovenie zavislosti:
   Spustite v terminali v priečinku LibraryApi:
   dotnet restore

2. Vytvorenie a aktualizacia databazy:
   Tento prikaz vytvori subor library.db a pripravi tabulky:
   dotnet ef database update

3. Spustenie aplikacie:
   dotnet run

4. Pristup k API:
   - Aplikacia bezi na adrese http://localhost:5149.
   - Interaktivna Swagger dokumentacia je dostupna na http://localhost:5149/swagger.
