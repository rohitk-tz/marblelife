# Core/Sales - AI Context

## Purpose

The **Sales** module manages the complete customer relationship management (CRM) system, including customers, leads, quotes, opportunities, and sales pipeline tracking for the MarbleLife platform.

## Key Entities (Domain/)

### Customer Management
- **Customer**: Core customer entity with contact and service history
- **CustomerContact**: Multiple contacts per customer
- **CustomerLocation**: Service addresses for customers
- **CustomerNote**: Internal notes and communication history

### Sales Pipeline
- **Lead**: Potential customers (from MarketingLead or direct)
- **Opportunity**: Qualified leads with sales potential
- **Quote**: Price estimates and proposals
- **SalesOrder**: Confirmed orders ready for scheduling

### Relationship Management
- **CustomerInteraction**: Call logs, emails, meetings
- **FollowUp**: Scheduled follow-up activities
- **ReferralSource**: How customer found the service
- **CustomerTag**: Categorization and segmentation

## Service Interfaces

### Customer Services
- **ICustomerFactory**: Customer entity creation and validation
- **ICustomerService**: CRUD operations for customers
- **ICustomerContactFactory**: Contact management
- **ICustomerLocationService**: Service address management
- **ICustomerNoteFactory**: Note creation and retrieval

### Sales Operations
- **ILeadFactory**: Lead creation from various sources
- **ILeadService**: Lead qualification and conversion
- **IOpportunityFactory**: Opportunity tracking
- **IQuoteFactory**: Quote generation and management
- **IQuoteService**: Quote approval and conversion workflow
- **ISalesOrderFactory**: Order creation from quotes

### Analytics & Reporting
- **ISalesReportService**: Sales performance metrics
- **ICustomerLifetimeValueService**: CLV calculations
- **IConversionRateService**: Pipeline conversion analytics
- **ICustomerRetentionService**: Retention metrics

## Implementations (Impl/)

Concrete implementations with business logic including:
- Customer validation and deduplication
- Lead scoring and prioritization
- Quote pricing calculation
- Sales pipeline stage transitions
- Conversion tracking and reporting

## Enumerations (Enum/)

- **LeadStatus**: New, Contacted, Qualified, Converted, Lost
- **OpportunityStage**: Prospecting, Qualification, Proposal, Negotiation, ClosedWon, ClosedLost
- **QuoteStatus**: Draft, Sent, Viewed, Accepted, Declined, Expired
- **CustomerType**: Residential, Commercial, Government, Property Management
- **ServiceFrequency**: OneTime, Recurring, AsNeeded
- **ReferralSourceType**: Website, GoogleAds, Facebook, Referral, Direct

## ViewModels (ViewModel/)

- **CustomerViewModel**: Complete customer data
- **CustomerSummaryViewModel**: List view data
- **LeadViewModel**: Lead information
- **OpportunityViewModel**: Sales opportunity details
- **QuoteViewModel**: Quote with line items
- **SalesOrderViewModel**: Order confirmation data
- **SalesPipelineViewModel**: Pipeline stage metrics

## Business Rules

### Customer Management
1. Customers must have unique email or phone number
2. Multiple service locations allowed per customer
3. Customer merge capability for duplicate detection
4. Soft delete customers (maintain history)

### Lead-to-Customer Conversion
1. Leads must be qualified before converting
2. Converted leads become customers automatically
3. Lead source tracking maintained throughout lifecycle
4. Conversion triggers notification to franchisee

### Quote Process
1. Quotes valid for configurable period (default 30 days)
2. Quote approval workflow for high-value deals
3. Accepted quotes auto-convert to sales orders
4. Quote versions tracked for modifications

### Sales Pipeline
1. Opportunities progress through defined stages
2. Stage transitions logged for reporting
3. Win/loss reasons captured for analytics
4. Pipeline forecasting based on stage probability

## Dependencies

- **Core/Organizations**: Franchisee assignment
- **Core/Scheduler**: Job scheduling from sales orders
- **Core/Billing**: Invoice generation from orders
- **Core/MarketingLead**: Lead import and integration
- **Core/Notification**: Customer communication
- **Core/Geo**: Location and territory management

## For AI Agents

### Creating a New Customer
```csharp
// Validate and create customer
var customer = _customerFactory.Create(new CustomerViewModel
{
    FirstName = "John",
    LastName = "Smith",
    Email = "john.smith@example.com",
    Phone = "(555) 123-4567",
    CustomerType = CustomerType.Residential,
    FranchiseeId = franchiseeId
});

// Add service location
var location = _customerLocationService.AddLocation(customer.Id, new LocationViewModel
{
    Address = "123 Main St",
    City = "New York",
    State = "NY",
    ZipCode = "10001"
});
```

### Lead to Customer Conversion
```csharp
// Qualify lead
_leadService.Qualify(leadId, qualificationNotes);

// Convert to customer
var customer = _leadService.ConvertToCustomer(leadId, new CustomerViewModel
{
    // Customer details from lead
});

// Create initial opportunity
var opportunity = _opportunityFactory.Create(new OpportunityViewModel
{
    CustomerId = customer.Id,
    Description = "Initial restoration service",
    EstimatedValue = 5000m,
    Stage = OpportunityStage.Prospecting
});
```

### Quote Generation
```csharp
// Create quote from opportunity
var quote = _quoteFactory.Create(new QuoteViewModel
{
    OpportunityId = opportunityId,
    CustomerId = customerId,
    ValidUntil = DateTime.Now.AddDays(30),
    LineItems = new[]
    {
        new QuoteLineItemViewModel
        {
            Description = "Marble Floor Restoration",
            Quantity = 500,
            UnitPrice = 8.50m,
            Unit = "sq ft"
        }
    }
});

// Send quote to customer
_quoteService.Send(quote.Id, sendEmail: true);
```

## For Human Developers

### Common Workflows

#### 1. New Customer Onboarding
```csharp
// Create customer → Add locations → Initial assessment → Quote → Order → Schedule
var customer = _customerFactory.Create(customerData);
var location = _customerLocationService.AddLocation(customer.Id, locationData);
var quote = _quoteFactory.CreateFromAssessment(assessmentData);
var order = _salesOrderFactory.CreateFromQuote(quote.Id);
```

#### 2. Sales Pipeline Management
```csharp
// Move opportunity through stages
_opportunityService.MoveToStage(opportunityId, OpportunityStage.Qualification);
_opportunityService.MoveToStage(opportunityId, OpportunityStage.Proposal);

// Close won/lost
_opportunityService.CloseWon(opportunityId, actualAmount, notes);
_opportunityService.CloseLost(opportunityId, lossReason, competitorInfo);
```

#### 3. Customer Analytics
```csharp
// Customer lifetime value
var clv = _customerLifetimeValueService.Calculate(customerId);

// Sales metrics
var conversionRate = _conversionRateService.GetLeadToCustomerRate(startDate, endDate);
var pipelineValue = _salesReportService.GetPipelineValue(franchiseeId);
```

### Best Practices
- Always validate customer data before creation
- Track all customer interactions for history
- Use async operations for external communications
- Implement duplicate detection before creating customers
- Log all pipeline stage transitions
- Calculate quote totals server-side (never trust client)
- Maintain audit trail for all customer data changes
- Implement soft deletes to preserve sales history
