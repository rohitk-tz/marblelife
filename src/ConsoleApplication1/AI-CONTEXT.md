# ConsoleApplication1 (CurrencyExchangeRateService) - Exchange Rate Import Service

## Overview
The CurrencyExchangeRateService (initially named ConsoleApplication1) is a console application that fetches currency exchange rates from external APIs and stores them in the database. This service enables multi-currency support for international franchisees by maintaining historical exchange rate data for accurate financial reporting and currency conversion.

## Purpose
- Fetch current currency exchange rates from external API
- Store historical exchange rate data
- Support multi-currency franchisee operations
- Enable accurate financial reporting across currencies
- Maintain exchange rate history for audit purposes

## Technology Stack
- **.NET Framework**: C# Console Application
- **HTTP Client**: System.Net.WebClient for API calls
- **JSON Parsing**: System.Web.Script.Serialization.JavaScriptSerializer
- **Database**: Entity Framework Core
- **Dependency Injection**: Custom DI container
- **Logging**: Core.Application.ILogService

## Project Structure
```
/ConsoleApplication1
├── CurrencyExchangeRateService.csproj    # Project file
├── Program.cs                            # Entry point
├── CurrencyExchangeRateService.cs        # Main service implementation
├── ICurrencyExchangeRateService.cs       # Service interface
├── AppContextStore.cs                    # Context management
├── WinJobSessionContext.cs               # Session handling
├── App.config                            # Configuration
└── /Properties
    └── AssemblyInfo.cs
```

## Main Service Implementation

### CurrencyExchangeRateService.cs
```csharp
using Core.Application;
using Core.Billing.Domain;
using Core.Geo.Domain;
using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    [DefaultImplementation]
    public class CurrencyExchangeRateService : ICurrencyExchangeRateService
    {
        private readonly string _defaultCurrencyCode = "USD";
        private readonly ILogService _logService;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;

        public CurrencyExchangeRateService(
            IUnitOfWork unitOfWork,
            ILogService logService,
            IClock clock,
            ISettings setting)
        {
            _unitOfWork = unitOfWork;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _countryRepository = unitOfWork.Repository<Country>();
            _logService = logService;
            _clock = clock;
            _setting = setting;
        }

        public void GetAllCurrencyRate()
        {
            var countryList = _countryRepository.Table.ToList();
            
            foreach (var item in countryList.Where(x => !x.IsDefault).ToList())
            {
                try
                {
                    // Historical data - monthly rates for 2016
                    var startDate = new DateTime(2016, 01, 01);
                    var endDate = new DateTime(2016, 09, 30);

                    for (DateTime date = startDate; date <= endDate; date = date.AddMonths(1))
                    {
                        Thread.Sleep(2000); // Rate limiting
                        var endOfTheMonth = date.AddMonths(1).AddDays(-1);
                        GetRateFromAPIAndSaveAgainstTheDate(item, endOfTheMonth);
                    }

                    // Daily rates from October 2016 onwards
                    startDate = new DateTime(2016, 10, 01);
                    endDate = new DateTime(2017, 01, 13);

                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        GetRateFromAPIAndSaveAgainstTheDate(item, date);
                    }
                }
                catch (Exception e)
                {
                    _logService.Error($"Exception in save Currency Exchange Rate of {item.Name}", e);
                }
            }
        }

        private void GetRateFromAPIAndSaveAgainstTheDate(Country item, DateTime date)
        {
            decimal exchangeRate = GetCurrencyExchangeRateFromApi(item.CurrencyCode, date);

            _unitOfWork.StartTransaction();
            try
            {
                var currencyExchangeRateDomain = CreateDomain(item.Id, exchangeRate, date);
                _currencyExchangeRateRepository.Save(currencyExchangeRateDomain);
                _unitOfWork.SaveChanges();
                
                _logService.Info($"Saved exchange rate for {item.CurrencyCode} on {date:yyyy-MM-dd}: {exchangeRate}");
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _logService.Error($"Exception in save Currency Exchange Rate of {item.Name} for date - {date.ToShortDateString()}", e);
            }
        }

        private decimal GetCurrencyExchangeRateFromApi(string currencyCode, DateTime date)
        {
            try
            {
                _logService.Debug($"Getting Currency Exchange Rate from API for {date:yyyy-MM-dd} for {currencyCode}");
                
                string currencyDate = $"{date.Year}-{date.Month}-{date.Day}";
                var apiKey = _setting.CurrencyExchangeRateApiKey;
                string urlPattern = _setting.CurrencyExchangeRateApi + 
                                   $"?date={currencyDate}&quote={currencyCode}&api_key={apiKey}";

                string url = string.Format(urlPattern, currencyCode.ToUpper(), _defaultCurrencyCode);

                string json = "";
                using (var webClient = new WebClient())
                {
                    json = webClient.DownloadString(url);
                }

                if (string.IsNullOrEmpty(json))
                {
                    _logService.Warning($"Empty response from currency API for {currencyCode} on {date:yyyy-MM-dd}");
                    return 1.0m;
                }

                // Parse JSON response
                var serializer = new JavaScriptSerializer();
                var result = serializer.Deserialize<Dictionary<string, object>>(json);

                if (result.ContainsKey("quotes"))
                {
                    var quotes = result["quotes"] as Dictionary<string, object>;
                    var rateKey = $"{_defaultCurrencyCode}{currencyCode}";
                    
                    if (quotes.ContainsKey(rateKey))
                    {
                        var rate = Convert.ToDecimal(quotes[rateKey]);
                        _logService.Debug($"Exchange rate retrieved: {rate}");
                        return rate;
                    }
                }

                _logService.Warning($"Rate not found in API response for {currencyCode}");
                return 1.0m;
            }
            catch (Exception ex)
            {
                _logService.Error($"Error getting exchange rate for {currencyCode} on {date:yyyy-MM-dd}", ex);
                return 1.0m; // Default to 1:1 on error
            }
        }

        private CurrencyExchangeRate CreateDomain(long countryId, decimal rate, DateTime date)
        {
            return new CurrencyExchangeRate
            {
                CountryId = countryId,
                Rate = rate,
                Date = date,
                CreatedDate = _clock.UtcNow,
                IsActive = true
            };
        }
        
        public void GetCurrentRates()
        {
            var countryList = _countryRepository.Table.Where(x => !x.IsDefault).ToList();
            var today = _clock.UtcNow.Date;
            
            foreach (var country in countryList)
            {
                try
                {
                    // Check if rate already exists for today
                    var existingRate = _currencyExchangeRateRepository
                        .Get(x => x.CountryId == country.Id && x.Date == today);
                    
                    if (existingRate != null)
                    {
                        _logService.Debug($"Rate already exists for {country.CurrencyCode} on {today:yyyy-MM-dd}");
                        continue;
                    }
                    
                    GetRateFromAPIAndSaveAgainstTheDate(country, today);
                    Thread.Sleep(1000); // Rate limiting
                }
                catch (Exception ex)
                {
                    _logService.Error($"Failed to get current rate for {country.Name}", ex);
                }
            }
        }
    }
}
```

## API Integration

### API Response Format
```json
{
  "success": true,
  "terms": "https://currencylayer.com/terms",
  "privacy": "https://currencylayer.com/privacy",
  "historical": true,
  "date": "2024-02-15",
  "timestamp": 1708041599,
  "source": "USD",
  "quotes": {
    "USDCAD": 1.35025,
    "USDEUR": 0.92345,
    "USDGBP": 0.78945,
    "USDMXN": 17.125,
    "USDJPY": 149.875
  }
}
```

### Supported Currencies
- **CAD**: Canadian Dollar
- **EUR**: Euro
- **GBP**: British Pound
- **MXN**: Mexican Peso
- **JPY**: Japanese Yen
- **AUD**: Australian Dollar
- **CHF**: Swiss Franc
- **CNY**: Chinese Yuan

## Database Schema

### CurrencyExchangeRate Table
```sql
CREATE TABLE CurrencyExchangeRate (
    Id BIGINT PRIMARY KEY IDENTITY,
    CountryId BIGINT NOT NULL,
    Rate DECIMAL(18, 6) NOT NULL,
    Date DATE NOT NULL,
    CreatedDate DATETIME NOT NULL,
    ModifiedDate DATETIME,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_CurrencyExchangeRate_Country FOREIGN KEY (CountryId) 
        REFERENCES Country(Id),
    CONSTRAINT UQ_CurrencyExchangeRate_Country_Date UNIQUE (CountryId, Date)
);

CREATE INDEX IX_CurrencyExchangeRate_Date ON CurrencyExchangeRate(Date);
CREATE INDEX IX_CurrencyExchangeRate_CountryId ON CurrencyExchangeRate(CountryId);
```

### Domain Model
```csharp
public class CurrencyExchangeRate : IEntity
{
    public long Id { get; set; }
    public long CountryId { get; set; }
    public Country Country { get; set; }
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
}
```

## Usage Example

### Currency Conversion
```csharp
public class CurrencyConverter
{
    private readonly IRepository<CurrencyExchangeRate> _rateRepository;
    
    public decimal ConvertToUSD(decimal amount, long countryId, DateTime date)
    {
        var rate = _rateRepository.Table
            .Where(x => x.CountryId == countryId && x.Date <= date)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Rate)
            .FirstOrDefault();
        
        if (rate == 0)
        {
            return amount; // No conversion if rate not found
        }
        
        return amount / rate;
    }
    
    public decimal ConvertFromUSD(decimal amountUSD, long countryId, DateTime date)
    {
        var rate = _rateRepository.Table
            .Where(x => x.CountryId == countryId && x.Date <= date)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Rate)
            .FirstOrDefault();
        
        if (rate == 0)
        {
            return amountUSD; // No conversion if rate not found
        }
        
        return amountUSD * rate;
    }
}
```

## Configuration

### App.config
```xml
<configuration>
  <appSettings>
    <add key="CurrencyExchangeRateApi" value="https://api.currencylayer.com/historical" />
    <add key="CurrencyExchangeRateApiKey" value="your_api_key_here" />
    <add key="UpdateIntervalHours" value="24" />
    <add key="RateLimitDelayMs" value="1000" />
  </appSettings>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=.;Database=MarbleLife;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## Deployment

### Program.cs
```csharp
using Core.Application;
using DependencyInjection;
using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Currency Exchange Rate Service Starting...\n");
                
                // Setup dependency injection
                DependencyRegistrar.RegisterDependencies();
                ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
                ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
                DependencyRegistrar.SetupCurrentContextWinJob();
                
                var service = ApplicationManager.DependencyInjection
                    .Resolve<ICurrencyExchangeRateService>();
                
                // Get current rates
                Console.WriteLine("Fetching current exchange rates...");
                service.GetCurrentRates();
                
                Console.WriteLine("\nExchange rates updated successfully!");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}
```

### Scheduled Task
```powershell
# Run daily at midnight
$action = New-ScheduledTaskAction -Execute "C:\Services\CurrencyExchangeRateService\CurrencyExchangeRateService.exe"
$trigger = New-ScheduledTaskTrigger -Daily -At "00:00"
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount

Register-ScheduledTask -TaskName "MarbleLife Currency Rate Update" `
    -Action $action -Trigger $trigger -Principal $principal
```

## Error Handling

```csharp
private void ProcessWithRetry(Country country, DateTime date, int maxRetries = 3)
{
    int attempt = 0;
    Exception lastException = null;
    
    while (attempt < maxRetries)
    {
        try
        {
            GetRateFromAPIAndSaveAgainstTheDate(country, date);
            return; // Success
        }
        catch (WebException webEx)
        {
            lastException = webEx;
            attempt++;
            
            if (webEx.Response is HttpWebResponse response)
            {
                if ((int)response.StatusCode == 429) // Rate limit
                {
                    _logService.Warning($"Rate limit hit, waiting 60 seconds...");
                    Thread.Sleep(60000);
                }
                else if ((int)response.StatusCode >= 500) // Server error
                {
                    _logService.Warning($"Server error, retrying in 10 seconds...");
                    Thread.Sleep(10000);
                }
            }
        }
        catch (Exception ex)
        {
            lastException = ex;
            attempt++;
            Thread.Sleep(5000); // Wait before retry
        }
    }
    
    _logService.Error($"Failed after {maxRetries} attempts for {country.CurrencyCode} on {date:yyyy-MM-dd}", lastException);
}
```

## Rate Limit Management

```csharp
private class RateLimiter
{
    private DateTime _lastCall = DateTime.MinValue;
    private readonly int _minDelayMs;
    
    public RateLimiter(int minDelayMs)
    {
        _minDelayMs = minDelayMs;
    }
    
    public void WaitIfNeeded()
    {
        var timeSinceLastCall = DateTime.Now - _lastCall;
        var requiredDelay = TimeSpan.FromMilliseconds(_minDelayMs);
        
        if (timeSinceLastCall < requiredDelay)
        {
            var waitTime = requiredDelay - timeSinceLastCall;
            Thread.Sleep((int)waitTime.TotalMilliseconds);
        }
        
        _lastCall = DateTime.Now;
    }
}
```

## Monitoring

```csharp
private class ExchangeRateStats
{
    public int TotalCountries { get; set; }
    public int SuccessfulUpdates { get; set; }
    public int FailedUpdates { get; set; }
    public TimeSpan Duration { get; set; }
    
    public void PrintSummary()
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("EXCHANGE RATE UPDATE SUMMARY");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Total Countries: {TotalCountries}");
        Console.WriteLine($"Successful: {SuccessfulUpdates}");
        Console.WriteLine($"Failed: {FailedUpdates}");
        Console.WriteLine($"Duration: {Duration.TotalSeconds:F2} seconds");
        Console.WriteLine(new string('=', 50) + "\n");
    }
}
```

## Best Practices

1. **Rate Limiting**: Respect API rate limits with delays
2. **Error Handling**: Retry on transient failures
3. **Caching**: Don't fetch existing rates
4. **Historical Data**: Maintain complete history
5. **Validation**: Validate rate values are reasonable
6. **Logging**: Comprehensive logging for audit trail
7. **Monitoring**: Track API availability and success rate

## Related Services
- See `/CustomerDataUpload/AI-CONTEXT.md` for data import with currency conversion
- See Core.Billing domain for invoice currency handling
- See Core.Geo domain for country and currency models
