<!-- AUTO-GENERATED: Header -->
# Core.Billing — Module Context
**Version**: 756207f800ce20f013d92c41cb12e0f8a8fbf48e
**Generated**: 2025-01-10T00:00:00Z
**Total Files**: 117 C# files
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Architecture -->
## Architectural Mental Model

### Core Responsibility
The Core.Billing module manages the complete franchisee billing lifecycle including invoice generation, payment processing through Authorize.Net, late fee calculations, royalty/ad-fund billing, and loan schedule management. It orchestrates payment workflows across multiple payment instruments (credit cards, eChecks, checks, account credits), maintains audit trails, and enforces business rules for franchisee financial obligations.

### Design Patterns
- **Factory Pattern**: Extensive use of factories (IInvoiceFactory, IPaymentFactory, IChargeCardFactory, etc.) to create domain entities with proper validation and initialization
- **Repository Pattern**: All data access through IRepository<T> via IUnitOfWork for transaction management
- **Service Layer**: Clear separation between service interfaces (root folder) and implementations (Impl folder)
- **Polling Agents**: Background services (IInvoiceLateFeePollingAgent, FranchiseeInvoiceGenerationPollingAgent) for scheduled tasks
- **Strategy Pattern**: Payment processing strategies for different instrument types (ChargeCard, ECheck, Check)
- **Audit Trail**: Comprehensive audit entities (Audit* prefix) tracking all financial transactions

### Data Flow

#### Invoice Generation Flow
1. **Input**: Sales data uploads trigger invoice generation via `FranchiseeInvoiceGenerationPollingAgent`
2. **Creation**: `IFranchiseeInvoiceFactory` creates `FranchiseeInvoice` with associated `Invoice` and `InvoiceItem` entities
3. **Item Types**: Generates multiple item types (Royalty, AdFund, ServiceFee, LateFee, InterestRate)
4. **Persistence**: Saved via `IInvoiceService.Save()` with currency exchange rate applied
5. **Status**: Invoice status tracked (Unpaid=82, PartialPaid=83, Paid=81, Canceled=84, ZeroDue=230)

#### Payment Processing Flow
1. **Entry**: Payment enters via `IPaymentService.MakePaymentByNewChargeCard()` or `MakePaymentByECheck()`
2. **Validation**: Franchisee and invoice validated, account credit applied first
3. **Processor**: For credit cards, `IChargeCardPaymentService.ChargeCardPayment()` calls Authorize.Net API
4. **Response**: `ProcessorResponse` returned with status (Accepted=131, Rejected=132, Fail=133, HeldForReview=135)
5. **Recording**: On success, creates `Payment` entity with `ChargeCardPayment` or `ECheckPayment` child
6. **Linking**: `IInvoicePaymentService` creates `InvoicePayment` junction entities linking payment to invoice
7. **Audit**: `AuditPayment` and `AuditInvoicePayment` records created for compliance
8. **Update**: Invoice status updated based on remaining balance

#### Late Fee Generation Flow
1. **Trigger**: `IInvoiceLateFeePollingAgent.LateFeeGenerator()` runs on schedule
2. **Detection**: Identifies overdue invoices past grace period
3. **Calculation**: Applies flat fee or percentage based on `LateFeeType` (Royalty=125, SalesData=126)
4. **Creation**: Generates `LateFeeInvoiceItem` linked to original invoice
5. **Notification**: Sends email notifications via `ILateFeeNotificationService`

#### Authorize.Net Integration Flow
1. **Configuration**: `AuthorizeNetApiMaster` stores API credentials per account type
2. **Payment Profile**: `FranchiseePaymentProfile` manages saved payment methods
3. **Transaction**: Uses Authorize.Net CIM (Customer Information Manager) for secure storage
4. **Response Mapping**: Transaction responses parsed into `ProcessorResponse` with codes
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: File Inventory -->
## Complete File Inventory

### Root Level (25 Files) - Service & Factory Interfaces

#### Service Interfaces
- **IAuditFactory.cs**: Factory for creating audit trail entities (AuditInvoice, AuditPayment, AuditInvoicePayment)
- **ICalculateLoanScheduleService.cs**: Manages franchisee loan schedule calculations and overpayment detection
- **IChargeCardPaymentService.cs**: Processes credit card payments via Authorize.Net with rollback capability
- **IChargeCardProfileService.cs**: Manages stored credit card profiles in Authorize.Net CIM
- **IChargeCardService.cs**: CRUD operations for ChargeCard entities
- **ICheckService.cs**: Manages paper check payment records
- **IECheckPaymentService.cs**: Processes electronic check (ACH) payments via Authorize.Net
- **IEcheckProfileService.cs**: Manages stored eCheck profiles in Authorize.Net CIM
- **IEcheckService.cs**: CRUD operations for ECheck entities
- **IFranchhiseeInvoiceParseService.cs**: Parses franchisee sales data for invoice generation (note: typo in filename)
- **IFranchiseeSalesPaymentService.cs**: Processes payments from franchisee sales data uploads
- **IInvoiceItemService.cs**: Manages invoice line items (royalty, ad-fund, late fees, etc.)
- **IInvoiceLateFeePollingAgent.cs**: Background service that generates late fees for overdue invoices
- **IInvoicePaymentService.cs**: Links payments to invoices via InvoicePayment junction entities
- **IInvoiceService.cs**: Core invoice management - creation, details, listing, downloads, reconciliation
- **IPaymentService.cs**: Orchestrates payment processing across all instrument types

#### Factory Interfaces
- **IChargeCardFactory.cs**: Creates ChargeCard domain entities from edit models
- **IChargeCardPaymentProfileFactory.cs**: Creates payment profiles for Authorize.Net CIM
- **ICheckFactory.cs**: Creates Check domain entities
- **IECheckFactory.cs**: Creates ECheck domain entities
- **IFranchiseeInvoiceFactory.cs**: Creates FranchiseeInvoice with all associated items
- **IInvoiceFactory.cs**: Creates Invoice domain entities
- **IinvoiceItemFactory.cs**: Creates InvoiceItem entities (note: lowercase 'i' in filename)
- **IPaymentFactory.cs**: Creates Payment domain entities
- **IPaymentItemFactory.cs**: Creates PaymentItem entities linking payments to service types

### Constants/ (2 Files)
- **ServiceTypeAlias.cs**: Static string arrays defining service type groupings (StoneLife, Groutlife, Enduracrete, VinylGuard) for categorizing franchisee services

### Domain/ (31 Files) - Core Entities

#### Audit Entities (10 Files)
- **AuditAddress.cs**: Archived address records for compliance
- **AuditAddressDiscrepancy.cs**: Tracks address mismatches detected during audits
- **AuditCustomer.cs**: Archived customer records for financial audits
- **AuditFranchiseeSales.cs**: Archived sales records for royalty calculations
- **AuditInvoice.cs**: Archived invoice snapshots with QB invoice number tracking
- **AuditInvoiceItem.cs**: Archived invoice line items
- **AuditInvoicePayment.cs**: Archived payment-to-invoice linkages
- **AuditPayment.cs**: Archived payment records with full transaction details
- **AuditPaymentItem.cs**: Archived payment item allocations

#### Core Billing Entities (21 Files)
- **AccountCreditPayment.cs**: Records account credit usage against invoices
- **AuthorizeNetApiMaster.cs**: Stores Authorize.Net API credentials (ApiLoginID, ApiTransactionKey) per account type
- **ChargeCard.cs**: Credit card entity (NameOnCard, Number, ExpiryMonth, ExpiryYear, TypeId)
- **ChargeCardPayment.cs**: Links ChargeCard transactions to Payment parent
- **Check.cs**: Paper check entity with check number and bank details
- **CheckPayment.cs**: Links Check transactions to Payment parent
- **CurrencyExchangeRate.cs**: Exchange rates for multi-currency support
- **ECheck.cs**: Electronic check entity (RoutingNumber, AccountNumber, AccountTypeId, BankName)
- **ECheckPayment.cs**: Links ECheck transactions to Payment parent
- **FranchiseeInvoice.cs**: Junction entity linking Franchisee to Invoice, tracks download status
- **FranchiseePaymentProfile.cs**: Stored payment methods in Authorize.Net CIM per franchisee
- **InterestRateInvoiceItem.cs**: Interest charges on outstanding balances (loan-related)
- **Invoice.cs**: Core invoice entity (GeneratedOn, DueDate, StatusId, CustomerInvoiceId, QBInvoiceNumber, ReconciliationNotes)
- **InvoiceAddress.cs**: Billing address snapshot for invoice
- **InvoiceItem.cs**: Invoice line items with polymorphic children (RoyaltyInvoiceItem, AdFundInvoiceItem, LateFeeInvoiceItem, etc.)
- **InvoicePayment.cs**: Junction entity linking Invoice to Payment (many-to-many)
- **LateFeeInvoiceItem.cs**: Late fee charges (LateFeeTypeId, Amount, WaitPeriod, StartDate, EndDate, GeneratedOn)
- **Payment.cs**: Core payment entity (Date, Amount, InstrumentTypeId, CurrencyExchangeRateId) with polymorphic children
- **PaymentInstrument.cs**: Generic payment method descriptor
- **PaymentItem.cs**: Links Payment to ServiceType for allocation tracking
- **PaymentMailReminder.cs**: Tracks payment reminder emails sent
- **ServiceFeeInvoiceItem.cs**: Service fee charges for franchisee services

### Enum/ (13 Files) - Business Enumerations
- **AccountCreditType.cs**: Types of account credits
- **AccountType.cs**: Bank account types for eChecks
- **AuthorizeNetAccountType.cs**: Authorize.Net account configuration types
- **ChargeCardType.cs**: Credit card brands (Visa=51, MasterCard=52, Discover=53, AmericanExpress=54)
- **CvvResponseCode.cs**: Card verification value response codes from processor
- **InstrumentType.cs**: Payment method types (ChargeCard=41, ECheck=42, Cash=43, Check=44, ChargeCardOnFile=45, ECheckOnFile=46, AccountCredit=47)
- **InvoiceItemType.cs**: Line item categories (Service=91, RoyaltyFee=92, AdFund=93, Discount=94, LateFees=123, InterestRatePerAnnum=124, ServiceFee=95, NationalCharge=96, LoanServiceFee=208, etc.)
- **InvoiceStatus.cs**: Invoice lifecycle states (Paid=81, Unpaid=82, PartialPaid=83, Canceled=84, ZeroDue=230)
- **LateFeeType.cs**: Late fee categories (Royalty=125, SalesData=126)
- **PaymentStatus.cs**: Payment transaction states (Submitted=141, Approved=142, Processing=143, Rejected=144)
- **ProcessorResponseResult.cs**: Payment processor outcomes (Accepted=131, Rejected=132, Fail=133, ProcessorError=134, HeldForReview=135)
- **ServiceTypes.cs**: Franchisee service categories
- **TransactionResponseType.cs**: Authorize.Net transaction response type codes

### Impl/ (21 Files) - Service Implementations

#### Factory Implementations (8 Files)
- **AuditFactory.cs**: Creates audit trail entities with snapshot data from source entities
- **ChargeCardFactory.cs**: Maps ChargeCardEditModel to ChargeCard entity with validation
- **ChargeCardPaymentProfileFactory.cs**: Creates Authorize.Net payment profiles from card/eCheck data
- **CheckFactory.cs**: Creates Check entities from edit models
- **ECheckFactory.cs**: Maps ECheckEditModel to ECheck entity with bank account validation
- **FranchiseeInvoiceFactory.cs**: Orchestrates complex invoice creation with royalty/ad-fund calculations
- **InvoiceFactory.cs**: Creates Invoice with all associated items and currency conversion
- **InvoiceItemFactory.cs**: Creates InvoiceItem entities with proper type mapping
- **PaymentFactory.cs**: Creates Payment entities with currency exchange and instrument linking
- **PaymentItemFactory.cs**: Creates PaymentItem entities for payment allocation tracking

#### Service Implementations (11 Files)
- **CalculateLoanScheduleService.cs**: Calculates loan amortization schedules, detects overpayments
- **ChargeCardService.cs**: Basic CRUD for ChargeCard entities via factory
- **CheckService.cs**: Basic CRUD for Check entities via factory
- **ECheckService.cs**: Basic CRUD for ECheck entities via factory
- **FranchiseeInvoiceGenerationPollingAgent.cs**: Background job that generates monthly franchisee invoices from sales data
- **FranchiseeSalesPaymentService.cs**: Processes bulk payments from sales upload, creates payment instruments
- **InvoiceItemService.cs**: Manages invoice line item CRUD operations
- **InvoiceLateFeePollingAgent.cs**: **[CRITICAL]** Scheduled job that scans overdue invoices, calculates late fees (flat or percentage), generates LateFeeInvoiceItem records, sends notifications
- **InvoicePaymentService.cs**: Creates/manages InvoicePayment junction records, updates invoice status
- **InvoiceService.cs**: **[CORE]** Comprehensive invoice management - save, list with filters, details, reconciliation notes, Excel exports (ad-fund, royalty), download tracking
- **PaymentService.cs**: **[ORCHESTRATOR]** Main payment processing hub - coordinates ChargeCard/ECheck payment flows, applies account credits, handles overpayments, integrates with Authorize.Net, manages payment reminders

### ViewModel/ (27 Files) - DTOs for API Layer

#### Input Models (13 Files)
- **AccountCreditPaymentEditModel.cs**: Model for applying account credits to invoices
- **ChargeCardEditModel.cs**: Credit card input (NameOnCard, Number, ExpiryMonth/Year, TypeId, CVV)
- **ChargeCardPaymentEditModel.cs**: Extends EPaymentEditModel with ChargeCardEditModel and ProfileTypeId
- **CheckPaymentEditModel.cs**: Check payment details (CheckNumber, BankName, Date)
- **ECheckEditModel.cs**: Extends EPaymentEditModel for ACH payments (BankName, AccountNumber, RoutingNumber)
- **EPaymentEditModel.cs**: Base class for electronic payments (Amount, InvoiceId, IsLoanOverPayment, OverPaymentAmount)
- **FranchiseeSalesPaymentEditModel.cs**: Bulk payment entry from sales data uploads
- **InstrumentOnFileEditModel.cs**: Uses saved payment profile (CustomerProfileId, PaymentProfileId)
- **InvoiceEditModel.cs**: Invoice creation/update input
- **InvoiceItemEditModel.cs**: Individual invoice line item input
- **InvoiceListFilter.cs**: Filter criteria for invoice queries (date ranges, status, franchisee)
- **PaymentEditModel.cs**: Generic payment input model
- **PaymentItemEditModel.cs**: Payment allocation input

#### Output/Response Models (14 Files)
- **CurrencyExchangeRateViewModel.cs**: Exchange rate display data
- **CurrencyRates.cs**: Collection of currency rates
- **DownloadInvoiceModel.cs**: Invoice data for Excel export
- **DownloadPaymentModel.cs**: Payment data for Excel export
- **FranchiseePaymentInstrumentViewModel.cs**: Saved payment methods per franchisee
- **InvoiceDetailsForAttactmentViewModel.cs**: Invoice data for email attachments
- **InvoiceDetailsViewModel.cs**: Full invoice details with items, payments, franchisee info
- **InvoiceListModel.cs**: Paginated invoice list with summary data
- **InvoiceViewModel.cs**: Lightweight invoice display model
- **PaymentInstrumentViewModel.cs**: Display model for payment methods
- **PaymentModeDetailViewModel.cs**: Payment method details for display
- **ProcessorResponse.cs**: **[CRITICAL]** Payment processor result (ProcessorResult, Message, RawResponse, InstrumentId, CustomerProfileId)
- **StartEndDateViewModel.cs**: Date range selector
- **DeleteInvoiceResponseModel.cs**: Response for invoice item deletion
- **InvoiceReconciliationNotesModel.cs**: Model for saving QB reconciliation notes

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Type System -->
## Critical Type Definitions

### Core Domain Types

```csharp
// Invoice - Central billing entity
public class Invoice : DomainBase
{
    public DateTime GeneratedOn { get; set; }
    public DateTime DueDate { get; set; }
    public long StatusId { get; set; }  // Maps to InvoiceStatus enum
    public long CustomerInvoiceId { get; set; }  // Internal tracking ID
    public long CustomerQbInvoiceId { get; set; }  // QuickBooks integration ID
    public string ReconciliationNotes { get; set; }
    public virtual ICollection<InvoicePayment> InvoicePayments { get; set; }
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }  // Cascade delete
    public virtual ICollection<PaymentMailReminder> PaymentMailReminders { get; set; }
}

// Payment - Polymorphic payment entity
public class Payment : DomainBase
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public long InstrumentTypeId { get; set; }  // Maps to InstrumentType enum
    public long CurrencyExchangeRateId { get; set; }
    public virtual CurrencyExchangeRate CurrencyExchangeRate { get; set; }
    public virtual ICollection<InvoicePayment> InvoicePayments { get; set; }
    public virtual ICollection<PaymentItem> PaymentItems { get; set; }  // Cascade delete
    // One of these will be populated based on InstrumentTypeId:
    public virtual ChargeCardPayment ChargeCardPayment { get; set; }
    public virtual ECheckPayment ECheckPayment { get; set; }
    public virtual CheckPayment CheckPayment { get; set; }
}

// InvoiceItem - Polymorphic invoice line item
public class InvoiceItem : DomainBase
{
    public long InvoiceId { get; set; }
    public long? ItemId { get; set; }  // Links to ServiceType
    public long ItemTypeId { get; set; }  // Maps to InvoiceItemType enum
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public long CurrencyExchangeRateId { get; set; }
    // One of these will be populated based on ItemTypeId:
    public virtual RoyaltyInvoiceItem RoyaltyInvoiceItem { get; set; }
    public virtual AdFundInvoiceItem AdFundInvoiceItem { get; set; }
    public virtual LateFeeInvoiceItem LateFeeInvoiceItem { get; set; }
    public virtual InterestRateInvoiceItem InterestRateInvoiceItem { get; set; }
    public virtual ServiceFeeInvoiceItem ServiceFeeInvoiceItem { get; set; }
}

// LateFeeInvoiceItem - Late fee calculation details
public class LateFeeInvoiceItem : DomainBase
{
    public long? LateFeeTypeId { get; set; }  // Royalty or SalesData
    public decimal Amount { get; set; }
    public int WaitPeriod { get; set; }  // Grace period days
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedOn { get; set; }
}

// ChargeCard - Credit card details
public class ChargeCard : DomainBase
{
    public string NameOnCard { get; set; }
    public long TypeId { get; set; }  // Visa, MasterCard, Discover, AmEx
    public string Number { get; set; }  // Encrypted in production
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
}

// ECheck - Electronic check details
public class ECheck : DomainBase
{
    public string RoutingNumber { get; set; }
    public long AccountTypeId { get; set; }  // Checking or Savings
    public string Name { get; set; }  // Account holder name
    public string AccountNumber { get; set; }  // Encrypted in production
    public string BankName { get; set; }
}

// AuthorizeNetApiMaster - Payment gateway credentials
public class AuthorizeNetApiMaster : DomainBase
{
    public string ApiLoginID { get; set; }
    public long AccountTypeId { get; set; }
    public string ApiTransactionKey { get; set; }
}

// FranchiseePaymentProfile - Saved payment methods in Authorize.Net CIM
public class FranchiseePaymentProfile : DomainBase
{
    public long FranchiseeId { get; set; }
    public string CustomerProfileId { get; set; }  // Authorize.Net customer ID
    public string PaymentProfileId { get; set; }  // Authorize.Net payment profile ID
    public long InstrumentTypeId { get; set; }
}
```

### Critical Enums

```csharp
// Payment processing result
public enum ProcessorResponseResult
{
    Accepted = 131,      // Payment successful
    Rejected = 132,      // Payment declined by bank
    Fail = 133,          // System failure
    ProcessorError = 134, // Authorize.Net error
    HeldForReview = 135  // Fraud detection hold
}

// Invoice lifecycle
public enum InvoiceStatus
{
    Paid = 81,         // Fully paid
    Unpaid = 82,       // No payments
    PartialPaid = 83,  // Partial payment
    Canceled = 84,     // Voided
    ZeroDue = 230      // No amount due
}

// Payment instrument types
public enum InstrumentType
{
    ChargeCard = 41,       // New credit card
    ECheck = 42,           // New ACH
    Cash = 43,             // Cash payment
    Check = 44,            // Paper check
    ChargeCardOnFile = 45, // Saved credit card
    ECheckOnFile = 46,     // Saved ACH
    AccountCredit = 47     // Account balance
}

// Invoice line item types
public enum InvoiceItemType
{
    Service = 91,              // Service charges
    RoyaltyFee = 92,          // Franchise royalty
    AdFund = 93,              // Advertising fund
    Discount = 94,            // Applied discounts
    LateFees = 123,           // Late payment penalty
    InterestRatePerAnnum = 124, // Interest charges
    ServiceFee = 95,          // Service fees
    NationalCharge = 96,      // National program fees
    LoanServiceFee = 208,     // Loan processing fee
    LoanServiceFeeInterestRatePerAnnum = 209,
    FranchiseeTechMail = 215  // Tech mail charges
}

// Late fee categories
public enum LateFeeType
{
    Royalty = 125,     // Late royalty payment
    SalesData = 126    // Late sales data submission
}

// Payment transaction status
public enum PaymentStatus
{
    Submitted = 141,   // Sent to processor
    Approved = 142,    // Approved by bank
    Processing = 143,  // In progress
    Rejected = 144     // Declined
}
```

### Key ViewModels

```csharp
// Authorize.Net payment response
public class ProcessorResponse
{
    public ProcessorResponseResult ProcessorResult { get; set; }
    public string Message { get; set; }  // User-facing error/success message
    public string RawResponse { get; set; }  // Full Authorize.Net XML/JSON
    public long InstrumentId { get; set; }  // Created payment instrument ID
    public string CustomerProfileId { get; set; }  // Authorize.Net profile ID
}

// Credit card payment input
public class ChargeCardPaymentEditModel : EPaymentEditModel
{
    public ChargeCardEditModel ChargeCardEditModel { get; set; }
    public long ProfileTypeId { get; set; }  // New or saved profile
}

// Base electronic payment model
public class EPaymentEditModel
{
    public long InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public bool IsLoanOverPayment { get; set; }
    public decimal OverPaymentAmount { get; set; }
}

// Saved payment method usage
public class InstrumentOnFileEditModel : EPaymentEditModel
{
    public string CustomerProfileId { get; set; }
    public string PaymentProfileId { get; set; }
}
```

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Public Interfaces -->
## Public Interface Reference

### IInvoiceService - Invoice Management
**Purpose**: Core invoice CRUD, listing, exports, reconciliation

#### `Invoice Save(InvoiceEditModel model)`
- **Input**: `InvoiceEditModel` with invoice details and items
- **Output**: Persisted `Invoice` entity
- **Behavior**: Creates/updates invoice, cascades to items, applies currency exchange
- **Side Effects**: Commits to database, updates invoice status

#### `InvoiceDetailsViewModel InvoiceDetails(long invoiceId)`
- **Input**: Invoice ID
- **Output**: Complete invoice with items, payments, franchisee data
- **Behavior**: Loads full object graph for display
- **Side Effects**: None (read-only)

#### `InvoiceListModel GetInvoiceList(InvoiceListFilter filter, int pageNumber, int pageSize)`
- **Input**: Filter criteria (date range, status, franchisee), pagination params
- **Output**: Paginated list with summary data
- **Behavior**: Applies filters, orders by GeneratedOn DESC
- **Side Effects**: None (read-only)

#### `bool DownloadInvoiceListFile(long[] invoiceIds, out string fileName)`
- **Input**: Array of invoice IDs
- **Output**: Boolean success, Excel filename via out param
- **Behavior**: Generates Excel file with invoice data
- **Side Effects**: Creates temp file on disk

#### `bool CreateExcelRoyality(long[] invoiceIds, out string fileName)`
- **Input**: Array of invoice IDs
- **Output**: Boolean success, Excel filename via out param
- **Behavior**: Exports royalty items to Excel for QuickBooks import
- **Side Effects**: Creates temp file on disk

#### `bool MarkInvoicesAsDownloaded(long[] invoiceIds)`
- **Input**: Array of invoice IDs
- **Output**: Boolean success
- **Behavior**: Sets IsDownloaded flag on FranchiseeInvoice records
- **Side Effects**: Updates database

### IPaymentService - Payment Orchestration
**Purpose**: Coordinates payment processing across all instrument types

#### `ProcessorResponse MakePaymentByNewChargeCard(ChargeCardPaymentEditModel model, long franchiseeId, long invoiceId)`
- **Input**: Card details, franchisee ID, invoice ID
- **Output**: `ProcessorResponse` with transaction result
- **Behavior**: 
  1. Validates franchisee and invoice exist
  2. Applies account credit first (reduces charge amount)
  3. Creates ChargeCard entity
  4. Calls Authorize.Net via `IChargeCardPaymentService`
  5. On success: Creates Payment, InvoicePayment, updates invoice status
  6. On failure: Rolls back transaction, returns error
- **Side Effects**: Creates Payment, ChargeCardPayment, InvoicePayment, updates Invoice.StatusId, commits transaction
- **Errors**: Returns Fail if franchisee/invoice not found, ProcessorError if Authorize.Net fails

#### `ProcessorResponse MakePaymentOnFileChargeCard(InstrumentOnFileEditModel model, long franchiseeId, long invoiceId)`
- **Input**: Saved profile IDs, franchisee ID, invoice ID
- **Output**: `ProcessorResponse` with transaction result
- **Behavior**: Uses Authorize.Net CIM to charge saved card
- **Side Effects**: Creates Payment, InvoicePayment, updates invoice status

#### `ProcessorResponse MakePaymentByECheck(ECheckEditModel model, long franchiseeId, long invoiceId)`
- **Input**: Bank account details, franchisee ID, invoice ID
- **Output**: `ProcessorResponse` with transaction result
- **Behavior**: Processes ACH payment via Authorize.Net eCheck API
- **Side Effects**: Creates Payment, ECheckPayment, InvoicePayment

#### `decimal AccountCreditPayment(decimal amount, Invoice invoice, long franchiseeId)`
- **Input**: Payment amount, invoice, franchisee ID
- **Output**: Remaining amount after credit applied
- **Behavior**: Applies available account credit to reduce payment amount
- **Side Effects**: Creates AccountCreditPayment, updates franchisee credit balance

#### `void CreateOverPaymentInvoiceItem(decimal amount, long franchiseeId, long invoiceId)`
- **Input**: Overpayment amount, franchisee ID, invoice ID
- **Output**: Void
- **Behavior**: Creates credit memo as negative invoice item for overpayment
- **Side Effects**: Adds InvoiceItem with negative amount

### IChargeCardPaymentService - Credit Card Processing
**Purpose**: Authorize.Net credit card integration

#### `ProcessorResponse ChargeCardPayment(ChargeCardPaymentEditModel model, long franchiseeId, out decimal creditCardCharge, out decimal chargedAmount)`
- **Input**: Card details, franchisee ID
- **Output**: `ProcessorResponse`, charged amounts via out params
- **Behavior**: 
  1. Retrieves Authorize.Net credentials from AuthorizeNetApiMaster
  2. Calls Authorize.Net CIM CreateCustomerProfileTransaction API
  3. Parses response for approval/decline codes
  4. Maps to ProcessorResponseResult enum
- **Side Effects**: Charges credit card, creates Authorize.Net customer profile if ProfileTypeId indicates save
- **Errors**: Returns ProcessorError on API failure, Rejected on card decline

#### `bool RollbackPayment(long accountTypeId, string transactionId)`
- **Input**: Account type ID, Authorize.Net transaction ID
- **Output**: Boolean success
- **Behavior**: Voids or refunds transaction in Authorize.Net
- **Side Effects**: Reverses charge in payment gateway

### IECheckPaymentService - ACH Processing
**Purpose**: Authorize.Net eCheck (ACH) integration

#### `ProcessorResponse MakeECheckPayment(ECheckEditModel model, long franchiseeId)`
- **Input**: Bank account details, franchisee ID
- **Output**: `ProcessorResponse` with transaction result
- **Behavior**: Processes ACH payment via Authorize.Net eCheck API
- **Side Effects**: Initiates ACH transaction (settles in 2-3 business days)

### IInvoiceLateFeePollingAgent - Late Fee Automation
**Purpose**: Scheduled background service for late fee generation

#### `void LateFeeGenerator()`
- **Input**: None (triggered by scheduler)
- **Output**: Void
- **Behavior**: 
  1. Queries all Unpaid and PartialPaid invoices
  2. Filters invoices past DueDate + grace period (configured in settings)
  3. Calculates late fee (percentage or flat amount based on LateFeeType)
  4. Creates LateFeeInvoiceItem for each overdue invoice
  5. Sends email notification via ILateFeeNotificationService
  6. Updates invoice amount and status
- **Side Effects**: Creates InvoiceItems, sends emails, updates invoice totals

#### `void SaveRoyalityLateFee(FranchiseeInvoice item, DateTime currentDate)`
- **Input**: Franchisee invoice, current date
- **Output**: Void
- **Behavior**: Calculates and saves late fee for royalty invoices specifically
- **Side Effects**: Creates LateFeeInvoiceItem with LateFeeType.Royalty

### IFranchiseeSalesPaymentService - Sales Data Payment Processing
**Purpose**: Bulk payment processing from sales uploads

#### `Payment Save(FranchiseeSalesPaymentEditModel model)`
- **Input**: Payment model from sales data upload
- **Output**: Created Payment entity
- **Behavior**: Creates payment with items allocated to service types
- **Side Effects**: Creates Payment, PaymentItems, links to invoice

#### `string GetPaymentInstrument(ICollection<InvoicePayment> list)`
- **Input**: Collection of invoice payments
- **Output**: Comma-separated string of payment instrument names
- **Behavior**: Extracts instrument types from payments for display
- **Side Effects**: None (read-only)

### ICalculateLoanScheduleService - Loan Management
**Purpose**: Manages franchisee loan amortization schedules

#### `void CalculateSchedule()`
- **Input**: None (triggered by scheduler)
- **Output**: Void
- **Behavior**: Recalculates loan payment schedules for all active loans
- **Side Effects**: Updates FranchiseeLoanSchedule records

#### `void CheckingForOverPaidLoan(List<FranchiseeLoanSchedule> loanScheduleList)`
- **Input**: List of loan schedule entries
- **Output**: Void
- **Behavior**: Detects overpayments, adjusts schedule, refunds if necessary
- **Side Effects**: May create refund transactions

### Factory Interfaces
All factories follow consistent pattern:

#### `IInvoiceFactory.Create(InvoiceEditModel model)`
- Creates Invoice entity from ViewModel
- Applies validation, sets defaults
- Does NOT persist (caller must save)

#### `IPaymentFactory.Create(PaymentEditModel model)`
- Creates Payment with proper InstrumentType
- Links to CurrencyExchangeRate
- Does NOT persist

#### `IChargeCardFactory.CreateChargeCard(ChargeCardEditModel model)`
- Maps ChargeCardEditModel to ChargeCard
- Validates card number format
- Does NOT persist

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Dependencies -->
## Dependencies

### Internal Dependencies
- **Core.Application**: Base domain classes (DomainBase), repositories (IRepository, IUnitOfWork), validation attributes, settings
- **Core.Organizations**: Franchisee entities, FranchiseeAccountCredit, FranchiseeLoan, FranchiseeLoanSchedule
- **Core.Sales**: SalesDataUpload, ServiceType, RoyaltyInvoiceItem, AdFundInvoiceItem
- **Core.Notification**: Email services (ILateFeeNotificationService, IPaymentReminderPollingAgent), notification view models

### External Dependencies
- **Authorize.Net SDK**: Payment processing gateway integration for credit cards and eChecks
- **Entity Framework**: ORM for database persistence
- **System.ComponentModel.DataAnnotations**: Model validation attributes

### Cross-Module Workflows
1. **Sales → Billing**: SalesDataUpload triggers FranchiseeInvoice generation
2. **Billing → Notification**: Late fees and payment reminders trigger email notifications
3. **Organizations → Billing**: Franchisee data used for invoice generation, account credits applied to payments
4. **Billing → Organizations**: Loan payments update FranchiseeLoan and FranchiseeLoanSchedule

<!-- END AUTO-GENERATED -->

<!-- CUSTOM SECTION: Insights -->
## Developer Insights

### Payment Processing Best Practices
- Always apply account credits BEFORE charging payment instruments to minimize transaction fees
- Check ProcessorResponseResult before creating Payment entities - rollback on failure
- Use InstrumentOnFile types for recurring payments to reduce PCI scope
- Late fee generation runs nightly - ensure grace periods configured properly

### Common Gotchas
- Invoice status calculation depends on sum of InvoiceItems vs sum of InvoicePayments - must keep in sync
- ChargeCard.Number and ECheck.AccountNumber should be encrypted before persistence (not shown in this layer)
- Authorize.Net transaction IDs must be stored for refunds/voids - keep in AuditPayment
- Currency exchange rates applied at invoice AND payment creation - rates can drift for partial payments

### Testing Considerations
- Mock Authorize.Net API calls in unit tests - use test credentials in integration tests
- Late fee polling agent should run in isolation - test date-based logic with mocked IClock
- Payment rollback must be tested for both same-day voids and next-day refunds
- Invoice status transitions require testing all paths: Unpaid → PartialPaid → Paid

### Performance Notes
- GetInvoiceList with large date ranges can be slow - ensure indexes on Invoice.GeneratedOn and Invoice.StatusId
- Excel export operations block thread - consider async implementation for large datasets
- Late fee polling agent should batch process to avoid memory issues with large franchisee counts
- Authorize.Net API has rate limits - implement retry logic with exponential backoff

<!-- END CUSTOM SECTION -->
