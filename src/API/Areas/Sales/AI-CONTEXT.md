# API/Areas/Sales - AI Context

## Purpose

The **Sales** area manages customer relationships, sales pipeline, invoicing, and revenue operations. It exposes endpoints for customer management, invoice generation, sales tracking, and batch processing operations.

## Key Controllers

### CustomerController.cs
Manages customer entities, contact information, and customer history.

**Endpoints**:
- `GET /Sales/Customer/{id}` - Get customer details
- `GET /Sales/Customer` - Get paginated customer list with filters
- `POST /Sales/Customer` - Create/update customer
- `DELETE /Sales/Customer/{id}` - Delete customer
- `GET /Sales/Customer/Search?term={query}` - Search customers

**Key Features**:
- Customer profile management
- Contact information tracking
- Customer history and notes
- Search and filtering
- Customer segmentation

### InvoiceController.cs
Handles invoice creation, management, and PDF generation.

**Endpoints**:
- `GET /Sales/Invoice/{id}` - Get invoice
- `POST /Sales/Invoice` - Create invoice
- `POST /Sales/Invoice/Generate` - Generate PDF invoice
- `GET /Sales/Invoice/Send/{id}` - Email invoice to customer
- `GET /Sales/Invoice/GetList` - Get invoice list with filters

**Key Features**:
- Invoice creation from jobs
- PDF generation using Templates
- Email delivery
- Payment tracking
- Invoice status management (Draft, Sent, Paid, Overdue)

### SalesController.cs
Manages sales pipeline, opportunities, and deal tracking.

**Endpoints**:
- `GET /Sales/Sales/GetPipeline` - Get sales pipeline data
- `GET /Sales/Sales/GetOpportunities` - Get active opportunities
- `POST /Sales/Sales/UpdateStage` - Move opportunity through pipeline
- `GET /Sales/Sales/GetForecast` - Get revenue forecast

**Key Features**:
- Pipeline visualization data
- Opportunity management
- Stage progression tracking
- Revenue forecasting
- Win/loss analysis

### SalesFunnelController.cs
Provides sales funnel analytics and conversion metrics.

**Endpoints**:
- `GET /Sales/SalesFunnel/GetFunnelData` - Get funnel metrics
- `GET /Sales/SalesFunnel/GetConversionRates` - Get conversion rates by stage
- `GET /Sales/SalesFunnel/GetTrendData` - Get historical trends

### BatchController.cs
Handles batch operations for sales data processing.

**Endpoints**:
- `POST /Sales/Batch/ProcessInvoices` - Batch invoice processing
- `POST /Sales/Batch/SendReminders` - Send payment reminders
- `POST /Sales/Batch/GenerateReports` - Generate bulk reports

### AnnualBatchController.cs
Manages annual/recurring batch operations and reporting.

### AccountCreditController.cs
Manages customer account credits, refunds, and adjustments.

### SalesInvoiceController.cs
Specialized invoice operations (likely for specific invoice types).

## Key ViewModels

```csharp
public class CustomerViewModel
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public long FranchiseeId { get; set; }
    public CustomerType Type { get; set; }  // Residential/Commercial
    public DateTime CreatedDate { get; set; }
    public decimal LifetimeValue { get; set; }
}

public class InvoiceViewModel
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; }
    public long CustomerId { get; set; }
    public long JobId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public InvoiceStatus Status { get; set; }
    public List<InvoiceLineItem> LineItems { get; set; }
}

public class SalesPipelineViewModel
{
    public List<OpportunityStage> Stages { get; set; }
    public decimal TotalPipelineValue { get; set; }
    public int TotalOpportunities { get; set; }
}
```

## Authorization

- **Franchisee Users**: Can only access their franchisee's customers/invoices
- **Super Admin**: Full access to all franchisees
- **Technicians**: Read-only access to assigned jobs' customers

## Business Rules

- Customers must have unique email per franchisee
- Invoices cannot be deleted once paid
- Invoice numbers are auto-generated and sequential
- Tax calculation based on customer location
- Payment terms default to franchisee settings
- Late fees apply after grace period

## Related Areas

- **Scheduler**: Links jobs to customers and invoices
- **Payment**: Processes invoice payments
- **Organizations**: Franchisee-specific sales data
- **Reports**: Sales analytics and reporting
