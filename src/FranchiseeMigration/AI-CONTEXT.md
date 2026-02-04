# FranchiseeMigration - Franchisee Data Initialization Service

## Overview
The FranchiseeMigration service is a one-time console application used to initialize franchisee data from hardcoded configuration. It creates franchisee organizations with their complete setup including contact information, addresses, phone numbers, service types, and fee profiles. This service was used during initial system setup to migrate franchisee data into the new system.

## Purpose
- Initialize franchisee organizations
- Configure franchisee contact information
- Set up service type availability
- Configure fee profiles
- Establish territory assignments
- Create user accounts for franchisees
- One-time data migration utility

## Technology Stack
- **.NET Framework**: C# Console Application
- **Database**: Entity Framework Core
- **Dependency Injection**: Custom DI container
- **Logging**: Core.Application.ILogService

## Project Structure
```
/FranchiseeMigration
├── FranchiseeMigration.csproj          # Project file
├── Program.cs                          # Entry point
├── FranchiseeFromFileCollection.cs     # Franchisee data definitions
├── AppContextStore.cs                  # Context management
├── WinJobSessionContext.cs             # Session handling
├── App.config                          # Configuration
└── /Properties
    └── AssemblyInfo.cs
```

## Main Implementation

### FranchiseeFromFileCollection.cs (Excerpt)
```csharp
using Core.Application;
using Core.Geo.Domain;
using Core.Organizations.ViewModel;
using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using EnumServiceType = Core.Organizations.Enum.ServiceType;

namespace FranchiseeMigration
{
    public class FranchiseeFromFileCollection
    {
        static IList<State> states;
        private IRepository<State> _stateRepository;

        public FranchiseeFromFileCollection(IUnitOfWork unitOfWork)
        {
            _stateRepository = unitOfWork.Repository<State>();
        }

        public void FetchAllStates()
        {
            states = _stateRepository.Table.ToArray();
        }

        public IList<FranchiseeDetailsModel> FranchiseesDetails()
        {
            FetchAllStates();
            
            return new List<FranchiseeDetailsModel>()
            {
                // SW Alabama Franchisee
                CreateModel(
                    name: "SW Alabama",
                    email: "swalabama@marblelife.com",
                    password: "Marblelife21",
                    firstName: "Darryl",
                    lastName: "Proctor",
                    address1: "2057 Tujaques Place",
                    address2: "",
                    city: "Pensacola",
                    zipCode: "32505",
                    state: "AL",
                    feeProfile: null,
                    services: Services(
                        new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                    ),
                    phones: Number(PhoneType.Office, "4848323333"),
                           Number(PhoneType.Cell, "4844728329")
                ),

                // Arizona Franchisee
                CreateModel(
                    name: "Arizona",
                    email: "arizona@marblelife.com",
                    password: "Marblelife22",
                    firstName: "Jim",
                    lastName: "Mannari",
                    address1: "1737 East Jackson St.",
                    address2: "",
                    city: "Phoenix",
                    zipCode: "85034",
                    state: "ARIZONA",
                    feeProfile: FeeProfile2(),
                    services: Services(
                        new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                    ),
                    phones: Number(PhoneType.Office, "4804833745"),
                           Number(PhoneType.Fax, "4804834615"),
                           Number(PhoneType.Cell, "4802259852")
                ),

                // Additional franchisees...
                // (Full list contains 50+ franchisees across North America)
            };
        }

        private FranchiseeDetailsModel CreateModel(
            string name,
            string email,
            string password,
            string firstName,
            string lastName,
            string address1,
            string address2,
            string city,
            string zipCode,
            string state,
            FeeProfileViewModel feeProfile,
            List<ServiceTypeViewModel> services,
            params PhoneViewModel[] phones)
        {
            var stateId = GetStateId(state);

            return new FranchiseeDetailsModel
            {
                Name = name,
                Email = email,
                Password = password,
                OwnerFirstName = firstName,
                OwnerLastName = lastName,
                Address = new AddressCreateEditModel
                {
                    Street1 = address1,
                    Street2 = address2,
                    City = city,
                    ZipCode = zipCode,
                    StateId = stateId
                },
                FeeProfile = feeProfile,
                ServiceTypes = services,
                Phones = phones.ToList(),
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
        }

        private long GetStateId(string stateName)
        {
            var state = states.FirstOrDefault(s =>
                s.Name.Equals(stateName, StringComparison.OrdinalIgnoreCase) ||
                s.Abbreviation.Equals(stateName, StringComparison.OrdinalIgnoreCase)
            );

            if (state == null)
            {
                throw new Exception($"State not found: {stateName}");
            }

            return state.Id;
        }

        private List<ServiceTypeViewModel> Services(params Tuple<EnumServiceType, bool>[] services)
        {
            return services.Select(s => new ServiceTypeViewModel
            {
                ServiceTypeId = (long)s.Item1,
                IsAvailable = s.Item2
            }).ToList();
        }

        private PhoneViewModel Number(PhoneType type, string number)
        {
            return new PhoneViewModel
            {
                Type = type,
                Number = FormatPhoneNumber(number)
            };
        }

        private string FormatPhoneNumber(string phone)
        {
            // Remove all non-digits
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            
            if (digits.Length == 10)
            {
                return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
            }
            
            return phone;
        }

        // Fee Profile Definitions
        private FeeProfileViewModel FeeProfile1()
        {
            return new FeeProfileViewModel
            {
                Name = "Standard Fee Profile",
                TechnologyFeePercent = 8.0m,
                MarketingFeePercent = 4.0m,
                RoyaltyFeePercent = 6.0m,
                MinimumMonthlyFee = 500.00m
            };
        }

        private FeeProfileViewModel FeeProfile2()
        {
            return new FeeProfileViewModel
            {
                Name = "Premium Fee Profile",
                TechnologyFeePercent = 10.0m,
                MarketingFeePercent = 5.0m,
                RoyaltyFeePercent = 7.0m,
                MinimumMonthlyFee = 750.00m
            };
        }
    }
}
```

## Data Models

### FranchiseeDetailsModel
```csharp
public class FranchiseeDetailsModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string OwnerFirstName { get; set; }
    public string OwnerLastName { get; set; }
    public AddressCreateEditModel Address { get; set; }
    public FeeProfileViewModel FeeProfile { get; set; }
    public List<ServiceTypeViewModel> ServiceTypes { get; set; }
    public List<PhoneViewModel> Phones { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ServiceTypeViewModel
{
    public long ServiceTypeId { get; set; }
    public bool IsAvailable { get; set; }
}

public class PhoneViewModel
{
    public PhoneType Type { get; set; }
    public string Number { get; set; }
}

public enum PhoneType
{
    Office = 1,
    Cell = 2,
    Fax = 3,
    TollFree = 4
}
```

## Migration Service

### Program.cs
```csharp
using Core.Application;
using Core.Organizations;
using DependencyInjection;
using System;
using System.Linq;

namespace FranchiseeMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Franchisee Migration Service Starting...\n");
                
                // Setup dependency injection
                DependencyRegistrar.RegisterDependencies();
                ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
                ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
                DependencyRegistrar.SetupCurrentContextWinJob();
                
                var unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
                var franchiseeService = ApplicationManager.DependencyInjection.Resolve<IFranchiseeService>();
                var logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
                
                // Get franchisee data
                var collection = new FranchiseeFromFileCollection(unitOfWork);
                var franchisees = collection.FranchiseesDetails();
                
                Console.WriteLine($"Found {franchisees.Count} franchisees to migrate\n");
                
                int successCount = 0;
                int failureCount = 0;
                
                // Migrate each franchisee
                foreach (var franchisee in franchisees)
                {
                    try
                    {
                        Console.Write($"Migrating {franchisee.Name}... ");
                        
                        // Check if already exists
                        var existing = unitOfWork.Repository<Franchisee>()
                            .Get(f => f.Email == franchisee.Email);
                        
                        if (existing != null)
                        {
                            Console.WriteLine("SKIPPED (already exists)");
                            continue;
                        }
                        
                        unitOfWork.StartTransaction();
                        
                        // Create franchisee
                        var created = franchiseeService.Create(franchisee);
                        
                        unitOfWork.SaveChanges();
                        
                        Console.WriteLine($"SUCCESS (ID: {created.Id})");
                        logService.Info($"Created franchisee: {franchisee.Name} (ID: {created.Id})");
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        Console.WriteLine($"FAILED - {ex.Message}");
                        logService.Error($"Failed to create franchisee: {franchisee.Name}", ex);
                        failureCount++;
                    }
                }
                
                // Print summary
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("MIGRATION SUMMARY");
                Console.WriteLine(new string('=', 50));
                Console.WriteLine($"Total Franchisees: {franchisees.Count}");
                Console.WriteLine($"Successfully Migrated: {successCount}");
                Console.WriteLine($"Failed: {failureCount}");
                Console.WriteLine($"Skipped: {franchisees.Count - successCount - failureCount}");
                Console.WriteLine(new string('=', 50));
                
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                
                Environment.Exit(failureCount > 0 ? 1 : 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                
                Environment.Exit(1);
            }
        }
    }
}
```

## Service Type Configuration

### Available Service Types
```csharp
public enum ServiceType
{
    StoneLife = 1,      // Marble and granite care
    ColorSeal = 2,      // Grout coloring/sealing
    Enduracrete = 3,    // Concrete polishing
    Tilelok = 4,        // Tile and grout sealing
    CleanShield = 5,    // Surface protection
    Vinylguard = 6,     // Vinyl floor care
    Wood = 7,           // Wood floor restoration
    CarpetLife = 8,     // Carpet care
    Fabricators = 9,    // Stone fabrication
    TileInstall = 10    // Tile installation
}
```

## Fee Profile Configuration

### Fee Structure
- **Technology Fee**: 6-10% of gross sales
- **Marketing Fee**: 3-5% of gross sales
- **Royalty Fee**: 5-8% of gross sales
- **Minimum Monthly Fee**: $250-$1,000

```csharp
private FeeProfileViewModel CreateFeeProfile(
    string name,
    decimal techFee,
    decimal marketingFee,
    decimal royaltyFee,
    decimal minimumFee)
{
    return new FeeProfileViewModel
    {
        Name = name,
        TechnologyFeePercent = techFee,
        MarketingFeePercent = marketingFee,
        RoyaltyFeePercent = royaltyFee,
        MinimumMonthlyFee = minimumFee,
        IsActive = true
    };
}
```

## User Account Creation

```csharp
private void CreateFranchiseeUser(Franchisee franchisee, FranchiseeDetailsModel model)
{
    var userService = ApplicationManager.DependencyInjection.Resolve<IUserService>();
    
    var user = new UserCreateEditModel
    {
        Email = model.Email,
        FirstName = model.OwnerFirstName,
        LastName = model.OwnerLastName,
        Password = model.Password,
        RoleId = (long)UserRole.FranchiseeOwner,
        FranchiseeId = franchisee.Id,
        IsActive = true
    };
    
    userService.Create(user);
}
```

## Validation

```csharp
private List<string> ValidateFranchisee(FranchiseeDetailsModel model)
{
    var errors = new List<string>();
    
    if (string.IsNullOrWhiteSpace(model.Name))
        errors.Add("Franchisee name is required");
        
    if (string.IsNullOrWhiteSpace(model.Email))
        errors.Add("Email is required");
    else if (!IsValidEmail(model.Email))
        errors.Add($"Invalid email format: {model.Email}");
        
    if (string.IsNullOrWhiteSpace(model.Password))
        errors.Add("Password is required");
        
    if (model.Address == null)
        errors.Add("Address is required");
    else
    {
        if (string.IsNullOrWhiteSpace(model.Address.Street1))
            errors.Add("Street address is required");
        if (string.IsNullOrWhiteSpace(model.Address.City))
            errors.Add("City is required");
        if (model.Address.StateId == 0)
            errors.Add("State is required");
        if (string.IsNullOrWhiteSpace(model.Address.ZipCode))
            errors.Add("Zip code is required");
    }
    
    if (!model.Phones.Any())
        errors.Add("At least one phone number is required");
        
    if (!model.ServiceTypes.Any())
        errors.Add("At least one service type must be enabled");
    
    return errors;
}

private bool IsValidEmail(string email)
{
    try
    {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
    }
    catch
    {
        return false;
    }
}
```

## Geographic Coverage

### Franchisees by Region
- **Southwest**: Arizona, New Mexico, Nevada
- **West Coast**: California (multiple territories), Oregon, Washington
- **Mountain**: Colorado, Utah
- **Midwest**: Illinois, Michigan, Minnesota, Wisconsin
- **Southeast**: Florida (multiple territories), Georgia, Alabama
- **Northeast**: Connecticut, Massachusetts, New York, Pennsylvania
- **Mid-Atlantic**: Maryland, Virginia, DC
- **Canada**: Ontario, British Columbia

## Configuration

### App.config
```xml
<configuration>
  <appSettings>
    <add key="DefaultPassword" value="Marblelife123!" />
    <add key="CreateUserAccounts" value="true" />
    <add key="SendWelcomeEmails" value="false" />
  </appSettings>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=.;Database=MarbleLife;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## Best Practices

1. **Idempotency**: Check for existing records before creating
2. **Validation**: Validate all data before migration
3. **Transaction Management**: Use transactions for atomicity
4. **Error Handling**: Continue on non-critical errors
5. **Logging**: Comprehensive logging for audit trail
6. **Rollback Capability**: Ability to undo migration if needed
7. **Testing**: Test with subset before full migration

## Post-Migration Tasks

1. **Verify Data**: Check all franchisees were created correctly
2. **Test Logins**: Verify user accounts can authenticate
3. **Configure Territories**: Set up service territories
4. **Setup Integrations**: Configure email, payment gateways
5. **Training**: Provide access to franchisee owners

## Related Services
- See `/CustomerDataUpload/AI-CONTEXT.md` for customer data import
- See `/UpdateInvoiceItemInfo/AI-CONTEXT.md` for data corrections
- See Core.Organizations domain for franchisee models
