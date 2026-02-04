# API/Templates - AI Context

## Purpose

The **Templates** folder contains Razor view templates (.cshtml files) used to generate formatted documents like PDF invoices, Excel reports, and HTML content. These templates combine static markup with dynamic data to produce professional, print-ready documents.

## Architecture

Templates are rendered server-side using the Razor view engine and can be converted to various formats:
- **PDF**: Using libraries like iTextSharp, wkhtmltopdf, or SelectPdf
- **Excel**: Rendered as HTML with Excel-specific markup, opened with Excel
- **HTML**: For email content, print previews, or web display
- **Images**: Converted from HTML/PDF for thumbnails or attachments

## Key Template Files

### Invoice Templates

#### cutomer_invoice.cshtml
**Purpose**: Standard customer invoice template without work details.

**Use Case**: Simple invoices showing totals, payment terms, and basic line items.

**Data Model**:
```csharp
public class InvoiceViewModel
{
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    
    // Customer Information
    public string CustomerName { get; set; }
    public string CustomerAddress { get; set; }
    public string CustomerPhone { get; set; }
    
    // Franchisee Information
    public string FranchiseeName { get; set; }
    public string FranchiseeAddress { get; set; }
    public string FranchiseePhone { get; set; }
    public string FranchiseeEmail { get; set; }
    
    // Line Items
    public List<InvoiceLineItem> LineItems { get; set; }
    
    // Totals
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }
    
    // Payment Terms
    public string PaymentTerms { get; set; }
    public string Notes { get; set; }
}
```

**Controller Usage**:
```csharp
[HttpGet]
public HttpResponseMessage GenerateInvoicePdf(long invoiceId)
{
    var invoice = _invoiceService.Get(invoiceId);
    var viewModel = _invoiceMapper.Map(invoice);
    
    // Render Razor template to HTML
    var html = RenderTemplate("cutomer_invoice.cshtml", viewModel);
    
    // Convert HTML to PDF
    var pdfBytes = _pdfConverter.ConvertHtmlToPdf(html);
    
    return FileDownloadHelper.CreateFileResponse(
        pdfBytes, 
        $"Invoice-{invoice.InvoiceNumber}.pdf", 
        "application/pdf"
    );
}
```

**Features**:
- Company logo and branding
- Invoice number and dates
- Bill to / Ship to addresses
- Itemized line items with descriptions, quantities, rates
- Subtotal, tax, and total calculations
- Payment terms and instructions
- Professional formatting for printing

#### cutomer_invoice_with_work.cshtml
**Purpose**: Detailed invoice template including work performed and job details.

**Use Case**: Comprehensive invoices showing before/after photos, detailed work descriptions, materials used, and labor hours.

**Additional Data**:
```csharp
public class DetailedInvoiceViewModel : InvoiceViewModel
{
    // Job Information
    public string JobNumber { get; set; }
    public DateTime ServiceDate { get; set; }
    public string ServiceLocation { get; set; }
    
    // Work Details
    public List<WorkPerformed> WorkItems { get; set; }
    public List<Material> MaterialsUsed { get; set; }
    public decimal LaborHours { get; set; }
    public decimal LaborRate { get; set; }
    
    // Photos
    public List<Photo> BeforePhotos { get; set; }
    public List<Photo> AfterPhotos { get; set; }
    
    // Technician Information
    public string TechnicianName { get; set; }
    public string TechnicianSignature { get; set; }
    
    // Customer Signature
    public string CustomerSignature { get; set; }
    public DateTime SignedDate { get; set; }
}
```

**Features**:
- All features from standard invoice
- Work description section
- Before/after photo gallery
- Materials and labor breakdown
- Technician details
- Customer signature capture
- Detailed job timeline

#### invoice-job-attacktment.cshtml
**Purpose**: Invoice attachment showing job-specific details and documentation.

**Use Case**: Supplementary document attached to invoice with technical details, warranties, care instructions.

**Data Model**:
```csharp
public class InvoiceAttachmentViewModel
{
    public string JobNumber { get; set; }
    public DateTime ServiceDate { get; set; }
    
    // Services Performed
    public List<ServiceDetail> Services { get; set; }
    
    // Products Used
    public List<Product> Products { get; set; }
    
    // Care Instructions
    public string CareInstructions { get; set; }
    
    // Warranty Information
    public string WarrantyDetails { get; set; }
    public DateTime WarrantyExpiration { get; set; }
    
    // Technical Notes
    public string TechnicalNotes { get; set; }
}
```

#### invoice-print-customer.cshtml
**Purpose**: Customer-facing print version of invoice.

**Use Case**: Clean, simplified invoice format optimized for printing by customers.

**Differences from Standard Invoice**:
- Larger fonts for readability
- Simplified layout
- No internal notes or technician details
- Print-friendly colors (black and white friendly)
- Larger margins for hole punching

### Report Templates

#### addin-alarm-history-list.excel.cshtml
**Purpose**: Excel export template for alarm history data.

**Use Case**: Generate downloadable Excel files with alarm/notification history.

**Data Model**:
```csharp
public class AlarmHistoryReportViewModel
{
    public string ReportTitle { get; set; }
    public DateTime GeneratedDate { get; set; }
    public List<AlarmHistoryItem> AlarmHistory { get; set; }
}

public class AlarmHistoryItem
{
    public DateTime Timestamp { get; set; }
    public string AlarmType { get; set; }
    public string Description { get; set; }
    public string FranchiseeName { get; set; }
    public string Status { get; set; }
    public string AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
}
```

**Controller Usage**:
```csharp
[HttpGet]
public HttpResponseMessage ExportAlarmHistory([FromUri] AlarmHistoryFilter filter)
{
    var data = _alarmService.GetHistory(filter);
    var viewModel = new AlarmHistoryReportViewModel
    {
        ReportTitle = "Alarm History Report",
        GeneratedDate = DateTime.Now,
        AlarmHistory = data
    };
    
    // Render template with Excel-specific markup
    var html = RenderTemplate("addin-alarm-history-list.excel.cshtml", viewModel);
    
    // Return as Excel file
    var bytes = Encoding.UTF8.GetBytes(html);
    return FileDownloadHelper.CreateFileResponse(
        bytes,
        $"AlarmHistory-{DateTime.Now:yyyyMMdd}.xls",
        "application/vnd.ms-excel"
    );
}
```

**Excel-Specific Markup**:
```html
<!-- Excel XML namespace for formatting -->
<html xmlns:o="urn:schemas-microsoft-com:office:office" 
      xmlns:x="urn:schemas-microsoft-com:office:excel">
<head>
    <xml>
        <x:ExcelWorkbook>
            <x:ExcelWorksheets>
                <x:ExcelWorksheet>
                    <x:Name>Alarm History</x:Name>
                </x:ExcelWorksheet>
            </x:ExcelWorksheets>
        </x:ExcelWorkbook>
    </xml>
</head>
```

### Before/After Templates

#### before-after-best-pair.cshtml
**Purpose**: Display before/after photo comparisons for jobs.

**Use Case**: Marketing materials, customer reports, quality documentation.

**Data Model**:
```csharp
public class BeforeAfterViewModel
{
    public string JobNumber { get; set; }
    public string CustomerName { get; set; }
    public DateTime ServiceDate { get; set; }
    public string ServiceDescription { get; set; }
    
    public List<PhotoPair> PhotoPairs { get; set; }
}

public class PhotoPair
{
    public string BeforeImageUrl { get; set; }
    public string AfterImageUrl { get; set; }
    public string Caption { get; set; }
    public string Location { get; set; }
}
```

**Features**:
- Side-by-side photo comparison
- Captions and descriptions
- Professional layout for presentations
- Print-ready formatting

### Graph Templates

#### local-office-graph.cshtml
**Purpose**: Graphical visualization of franchisee office performance.

**Use Case**: Dashboard visualizations, performance reports.

**Data Model**:
```csharp
public class OfficeGraphViewModel
{
    public string FranchiseeName { get; set; }
    public List<MetricDataPoint> SalesData { get; set; }
    public List<MetricDataPoint> JobsData { get; set; }
    public List<MetricDataPoint> RevenueData { get; set; }
}
```

### Assets

#### logo.png
**Purpose**: MarbleLife company logo for invoices and documents.

**Specifications**:
- Format: PNG with transparency
- Size: High resolution for print quality
- Usage: Header of all customer-facing documents

#### marblelife-logo.svg
**Purpose**: Scalable vector logo for high-quality rendering.

**Advantages**:
- Scales to any size without quality loss
- Smaller file size
- Editable in design tools

## Template Rendering Process

### 1. Data Preparation
```csharp
var invoice = _invoiceService.Get(invoiceId);
var viewModel = MapToViewModel(invoice);
```

### 2. Template Rendering
```csharp
public string RenderTemplate(string templateName, object model)
{
    var templatePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        "Templates", 
        templateName
    );
    
    var templateContent = File.ReadAllText(templatePath);
    
    // Render Razor template
    var razorEngine = RazorEngineService.Create();
    var html = razorEngine.RunCompile(templateContent, templateName, null, model);
    
    return html;
}
```

### 3. Format Conversion
```csharp
// To PDF
var pdfBytes = ConvertHtmlToPdf(html);

// To Excel
var excelBytes = Encoding.UTF8.GetBytes(html); // HTML with Excel markup

// To Image
var imageBytes = ConvertHtmlToImage(html);
```

### 4. Response Generation
```csharp
return FileDownloadHelper.CreateFileResponse(
    pdfBytes, 
    "Invoice-12345.pdf", 
    "application/pdf"
);
```

## Razor Template Syntax

### Basic Data Binding
```cshtml
<h1>Invoice #@Model.InvoiceNumber</h1>
<p>Date: @Model.InvoiceDate.ToString("MM/dd/yyyy")</p>
<p>Customer: @Model.CustomerName</p>
```

### Conditional Rendering
```cshtml
@if (Model.Balance > 0)
{
    <div class="alert">Balance Due: $@Model.Balance.ToString("N2")</div>
}
else
{
    <div class="success">Paid in Full</div>
}
```

### Loops
```cshtml
<table>
    @foreach (var item in Model.LineItems)
    {
        <tr>
            <td>@item.Description</td>
            <td>@item.Quantity</td>
            <td>$@item.UnitPrice.ToString("N2")</td>
            <td>$@item.Total.ToString("N2")</td>
        </tr>
    }
</table>
```

### Formatting
```cshtml
<!-- Currency -->
@Model.Total.ToString("C2")

<!-- Date -->
@Model.InvoiceDate.ToString("MMMM dd, yyyy")

<!-- Percentage -->
@Model.TaxRate.ToString("P1")

<!-- Custom Format -->
@string.Format("Invoice #{0:000000}", Model.InvoiceNumber)
```

## For AI Agents

### Creating New Template

1. **Create .cshtml file** in Templates folder:
```cshtml
@model MyNewViewModel

<!DOCTYPE html>
<html>
<head>
    <title>@Model.Title</title>
    <style>
        /* CSS for document styling */
        body { font-family: Arial, sans-serif; }
        .header { background: #003366; color: white; }
    </style>
</head>
<body>
    <div class="header">
        <h1>@Model.Title</h1>
    </div>
    <div class="content">
        @foreach (var item in Model.Items)
        {
            <p>@item.Description</p>
        }
    </div>
</body>
</html>
```

2. **Create ViewModel**:
```csharp
public class MyNewViewModel
{
    public string Title { get; set; }
    public List<Item> Items { get; set; }
}
```

3. **Render in Controller**:
```csharp
[HttpGet]
public HttpResponseMessage GenerateDocument(long id)
{
    var data = _service.Get(id);
    var viewModel = MapToViewModel(data);
    var html = RenderTemplate("my-new-template.cshtml", viewModel);
    var pdf = ConvertHtmlToPdf(html);
    
    return FileDownloadHelper.CreateFileResponse(pdf, "document.pdf", "application/pdf");
}
```

### Modifying Existing Template

1. **Locate template** in Templates folder
2. **Update markup/styling** as needed
3. **Test rendering** with sample data
4. **Verify PDF/Excel conversion** works correctly

## For Human Developers

### Best Practices

#### Template Design
- Use inline CSS (external stylesheets may not work in PDF conversion)
- Test with various data sizes (empty lists, long text, etc.)
- Include print media queries for optimal printing
- Use web-safe fonts or embed custom fonts
- Design for A4/Letter paper sizes

#### Data Preparation
- Always validate data before passing to template
- Handle null/missing values gracefully
- Format dates/numbers in code or template consistently
- Provide default values for optional fields

#### Performance
- Cache compiled templates
- Render asynchronously for large documents
- Stream PDFs directly to response (don't buffer)
- Consider background jobs for complex reports

#### Localization
- Support multiple languages/currencies
- Use culture-aware formatting
- Include timezone conversions
- Provide translated text resources

### Common Patterns

#### Header/Footer
```cshtml
<div class="header">
    <img src="data:image/png;base64,@Model.LogoBase64" />
    <h1>@Model.CompanyName</h1>
</div>

<div class="content">
    @* Main content *@
</div>

<div class="footer">
    <p>Page @Model.PageNumber of @Model.TotalPages</p>
    <p>Generated: @DateTime.Now.ToString("g")</p>
</div>
```

#### Responsive Tables
```cshtml
<table style="width: 100%; border-collapse: collapse;">
    <thead style="background: #f0f0f0;">
        <tr>
            <th style="padding: 8px; text-align: left;">Item</th>
            <th style="padding: 8px; text-align: right;">Amount</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model.Items)
        {
            <tr style="border-bottom: 1px solid #ddd;">
                <td style="padding: 8px;">@item.Name</td>
                <td style="padding: 8px; text-align: right;">$@item.Amount.ToString("N2")</td>
            </tr>
        }
    </tbody>
</table>
```

#### Embedded Images
```cshtml
<!-- Base64 embedded image -->
<img src="data:image/png;base64,@Model.ImageBase64" alt="Photo" style="max-width: 300px;" />

<!-- URL (if accessible during PDF conversion) -->
<img src="@Model.ImageUrl" alt="Photo" style="max-width: 300px;" />
```

### Testing Templates

```csharp
[Test]
public void InvoiceTemplate_RendersCorrectly()
{
    var viewModel = new InvoiceViewModel
    {
        InvoiceNumber = "INV-001",
        InvoiceDate = DateTime.Now,
        CustomerName = "Test Customer",
        Total = 1000m
    };
    
    var html = RenderTemplate("cutomer_invoice.cshtml", viewModel);
    
    Assert.IsTrue(html.Contains("INV-001"));
    Assert.IsTrue(html.Contains("Test Customer"));
    Assert.IsTrue(html.Contains("$1,000.00"));
}
```

## PDF Conversion Libraries

### SelectPdf / wkhtmltopdf
- Renders HTML/CSS to PDF with high fidelity
- Supports JavaScript execution
- Handles complex layouts

### iTextSharp
- Programmatic PDF generation
- More control but more complex
- Better for dynamic layouts

## Excel Generation

Templates with Excel-specific markup can be opened directly in Excel:
- Use HTML tables with Excel XML namespaces
- Set content type to `application/vnd.ms-excel`
- Excel will parse and format automatically

## Security Considerations

- **Input Sanitization**: Escape HTML in user-provided content
- **File Access**: Validate file paths for logo/image includes
- **Data Privacy**: Don't include sensitive data in templates unintentionally
- **Authorization**: Verify user can access document data before generating

## Related Files

- **Controllers/**: Call template rendering and conversion
- **Core/*/ViewModel/**: Data models passed to templates
- **Impl/FileDownloadHelper.cs**: Creates HTTP responses for generated files
- **Web.config**: May contain PDF converter settings

## Future Enhancements

- **Template versioning**: Support multiple template versions
- **Dynamic templates**: Allow franchisees to customize templates
- **Template preview**: Web-based preview before generating PDF
- **Template library**: Repository of reusable template components
