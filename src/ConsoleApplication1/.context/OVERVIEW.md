<!-- AUTO-GENERATED: Header -->
# ConsoleApplication1 (CurrencyExchangeRateService)
> Historical currency exchange rate import utility for international franchisees
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview
ConsoleApplication1 (internally named CurrencyExchangeRateService) populates the database with historical exchange rates for international franchisees in Canada, Bahamas, Cayman Islands, South Africa, and UAE. It fetches rates from a currency API for specific date ranges and stores them for royalty invoice calculations.

**Purpose**: One-time historical data import + manual updates when needed.

**Note**: For ongoing daily updates, use the NotificationService (Jobs) scheduled task instead.
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## Usage

### Configuration
```xml
<add key="CurrencyExchangeRateApi" value="https://api.exchangerate.host/convert" />
<add key="CurrencyExchengeRateApiKey" value="your_api_key_here" />
```

### Running
```bash
ConsoleApplication1.exe
# Fetches rates for Jan 2016 - Jan 2017
# ~17 minutes for 525 requests (2-second delay each)
```

### Expected Output
```
Get Currency Exchange Rate from API 1/31/2016 for CAD
Get Currency Exchange Rate from API 2/29/2016 for CAD
...
Get Currency Exchange Rate from API 10/1/2016 for CAD
...
```

### Supported Currencies
- **CAD**: Canadian Dollar
- **BSD**: Bahamian Dollar
- **KYD**: Cayman Islands Dollar
- **ZAR**: South African Rand
- **AED**: UAE Dirham
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: API Reference -->
## API Summary

| Method | Description |
|--------|-------------|
| `GetAllCurrencyRate()` | Fetches historical rates for all international countries |
| `GetCurrencyExchangeRateFromApi()` | Makes API call for single currency/date |
| `CreateDomain()` | Constructs CurrencyExchangeRate entity |
<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Troubleshooting -->
## Troubleshooting

### "Exception - in save Currency Exchange Rate"
**Cause**: API call failed, invalid response, or database error.  
**Solution**: Check logs for specific error, verify API key valid, ensure network connectivity.

### Rates seem inverted (e.g., CAD = 1.35 instead of 0.74)
**Cause**: Rate calculation not inverted (should be `1 / apiRate`).  
**Solution**: Verify `exchangeRate = Math.Round(1 / exchangeRate, 4)` in code.

### API rate limiting errors (429)
**Cause**: Too many requests too quickly.  
**Solution**: Increase `Thread.Sleep(2000)` delay to 3000-5000ms.

### Missing dates in database
**Cause**: API doesn't have data for weekends/holidays or individual request failed.  
**Solution**: Re-run for specific date range, or use linear interpolation for missing days.

### How to fetch current rates only
**Modify date range**:
```csharp
var startDate = DateTime.Now.Date;
var endDate = DateTime.Now.Date;
for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
{
    GetRateFromAPIAndSaveAgainstTheDate(item, date);
}
```
<!-- END CUSTOM SECTION -->
