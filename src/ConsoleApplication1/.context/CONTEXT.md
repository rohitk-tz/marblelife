<!-- AUTO-GENERATED: Header -->
# ConsoleApplication1 (CurrencyExchangeRateService) — Module Context
**Version**: 99bbd3bab4dd292938bce4f7e595bb3c94bf8366
**Generated**: 2025-02-10T11:26:00Z
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
ConsoleApplication1 (better named: CurrencyExchangeRateService) fetches historical and current currency exchange rates from an external API and populates the Marblelife database for international franchisee invoicing. It handles multi-currency conversions for Canada, Bahamas, Cayman Islands, South Africa, and UAE.

### Design Patterns
- **Date Range Iteration**: Fetches rates for specific historical periods
- **API Polling with Throttling**: 2-second delay between requests to avoid rate limiting
- **Transaction-per-Rate**: Each exchange rate saves in separate transaction
- **Retry Logic**: Continues processing on individual failures

### Data Flow
1. Query Country table for non-default countries (where IsDefault = false)
2. For each country:
   - **Phase 1**: Fetch monthly rates (Jan 2016 - Sep 2016) using end-of-month dates
   - **Phase 2**: Fetch daily rates (Oct 2016 - Jan 2017) for more granular data
3. For each date:
   - Call currency API with country's currency code
   - Parse JSON response
   - Extract exchange rate (bid/ask)
   - Calculate reciprocal (foreign currency → USD)
   - Save to CurrencyExchangeRate table
4. Log errors and continue to next country/date

### Supported Currencies
- CAD (Canadian Dollar)
- BSD (Bahamian Dollar)
- KYD (Cayman Islands Dollar)
- ZAR (South African Rand)
- AED (UAE Dirham)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Type Definitions / Models

### CurrencyExchangeRate (Domain Entity)
```csharp
public class CurrencyExchangeRate
{
    public long Id { get; set; }
    public long CountryId { get; set; }
    public decimal Rate { get; set; }      // Exchange rate (foreign → USD)
    public DateTime DateTime { get; set; }  // Rate effective date
    public bool IsNew { get; set; }
}
```

### API Response Models (Private)
```csharp
private class RootObject
{
    public string base_currency { get; set; }  // "USD"
    public Quotes quotes { get; set; }
}

private class Quotes
{
    public CAD CAD { get; set; }
    public BSD BSD { get; set; }
    public KYD KYD { get; set; }
    public ZAR ZAR { get; set; }
    public AED AED { get; set; }
}

private class CurrencyRate
{
    public string ask { get; set; }   // Sell price
    public string bid { get; set; }   // Buy price
    public string date { get; set; }  // "2016-01-31"
}
```
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API -->
## Public Interfaces

### `CurrencyExchangeRateService.GetAllCurrencyRate()`
- **Input**: None (queries database for countries)
- **Output**: void (console logging)
- **Behavior**:
  - Fetches exchange rates for all non-US countries
  - Handles two date ranges (monthly Jan-Sep 2016, daily Oct 2016-Jan 2017)
  - Saves each rate to database with 2-second delay between requests
  - Logs errors but continues processing
- **Side-effects**: Database writes, API calls, console logging

### `GetCurrencyExchangeRateFromApi(string currencyCode, DateTime date)`
- **Input**: Currency code (CAD, BSD, etc.), target date
- **Output**: Decimal exchange rate
- **Behavior**:
  - Constructs API URL with date and currency code
  - Makes HTTP GET request
  - Parses JSON response
  - Extracts ask price
  - Calculates reciprocal: `1 / rate` (converts foreign to USD)
  - Rounds to 4 decimal places
- **Side-effects**: HTTP request, logging

### `CreateDomain(long countryId, decimal exchangeRate, DateTime date)`
- **Input**: Country ID, rate value, effective date
- **Output**: CurrencyExchangeRate entity
- **Behavior**: Constructs domain object marked as new
- **Side-effects**: None (factory method)
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal
- **[Core.Application](../../Core/Application/.context/CONTEXT.md)** — IUnitOfWork, ILogService, IClock, ISettings
- **[Core.Billing](../../Core/Billing/.context/CONTEXT.md)** — CurrencyExchangeRate domain
- **[Core.Geo](../../Core/Geo/.context/CONTEXT.md)** — Country domain
- **[DependencyInjection](../../DependencyInjection/.context/CONTEXT.md)** — Service registration

### External
- **Currency Exchange Rate API** (configured in ISettings.CurrencyExchangeRateApi)
- **System.Net** — HTTP client for API calls
- **System.Web.Script.Serialization** — JSON parsing

### Configuration
Requires in App.config:
```xml
<add key="CurrencyExchangeRateApi" value="https://api.exchangerate.host/convert" />
<add key="CurrencyExchengeRateApiKey" value="your_api_key_here" />
```
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Hardcoded Date Ranges
The service uses **fixed historical date ranges**:
- **Monthly**: Jan 1, 2016 → Sep 30, 2016 (end-of-month rates)
- **Daily**: Oct 1, 2016 → Jan 13, 2017 (daily rates)

**Why?** Initial database population. For current rates, use NotificationService (Jobs) scheduled daily updates.

### API Rate Limiting
```csharp
Thread.Sleep(2000);  // 2-second delay between requests
```
Prevents API rate limiting. Many currency APIs restrict to 30 requests/minute.

### Exchange Rate Calculation
The API returns USD → foreign currency rates, but Marblelife needs foreign → USD:
```csharp
decimal apiRate = 1.35;  // 1 USD = 1.35 CAD
decimal dbRate = 1 / apiRate;  // 1 CAD = 0.74 USD (stored in DB)
```

This allows direct multiplication for invoice calculations:
```csharp
decimal cadAmount = 1000;
decimal usdAmount = cadAmount * exchangeRate;  // 1000 * 0.74 = 740 USD
```

### Currency Detection Logic
The code has a peculiar fallback chain:
```csharp
var value = obj.quotes.CAD != null ? obj.quotes.CAD.ask : null;
if (string.IsNullOrEmpty(value))
    value = obj.quotes.AED != null ? obj.quotes.AED.ask : null;
// ... continues for BSD, KYD, ZAR
```

**Why?** API response structure varies by request. The service checks which currency object is populated.

### Error Handling Strategy
Individual rate failures don't stop processing:
```csharp
try {
    GetRateFromAPIAndSaveAgainstTheDate(country, date);
} catch (Exception e) {
    _logService.Error("Failed for " + country.Name, e);
    // Continue to next date/country
}
```

This ensures partial completion even with API hiccups.

### Performance Characteristics
**For 5 countries × 105 days = 525 requests**:
- At 2 seconds per request: **17.5 minutes total**
- Longer with retries or API delays

Run during off-hours or use async/parallel processing for better performance.

### Modern Alternatives
Consider these improvements:
1. **Async/Await**: Non-blocking HTTP requests
2. **Parallel Processing**: Fetch multiple currencies simultaneously
3. **Batch API Calls**: Some APIs support date ranges in single request
4. **Scheduled Updates**: Move to NotificationService for ongoing updates

### Currency API Alternatives
If current API deprecated, consider:
- **Open Exchange Rates**: https://openexchangerates.org/
- **Fixer.io**: https://fixer.io/
- **CurrencyLayer**: https://currencylayer.com/
- **European Central Bank**: Free, no API key required

### Testing Recommendations
**Before production run**:
1. Test with single country: `if (item.Name == "Canada")`
2. Verify rate format in database (should be < 1.0 for most currencies vs. USD)
3. Check invoice calculations use rates correctly
4. Validate against manual currency converter (Google, XE.com)

### Ongoing Maintenance
**Daily Updates** (recommended):
Move to NotificationService with daily job:
```csharp
// Add to Jobs/Program.cs
var currencyRatePollingAgent = ApplicationManager.DependencyInjection.Resolve<ICurrencyRateService>();
currencyRatePollingAgent.GetTodaysRate();  // New method: fetch only today's rate
```

**Historical Backfill** (as needed):
Modify date ranges in code to fill gaps:
```csharp
var startDate = new DateTime(2024, 01, 01);
var endDate = new DateTime(2024, 12, 31);
```
<!-- END CUSTOM SECTION -->
