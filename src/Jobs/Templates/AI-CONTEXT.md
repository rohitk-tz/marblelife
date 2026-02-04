# Jobs/Templates - AI Context

## Purpose

This folder contains HTML and text templates used by background jobs for generating emails, PDFs, and other formatted output.

## Contents

Template files:
- **Email Templates**: HTML templates for notification emails
- **PDF Templates**: Razor templates for PDF generation
- **Report Templates**: Formatted report templates
- **SMS Templates**: Text message templates

## For AI Agents

**Template Usage Pattern**:
```csharp
public class EmailService
{
    private readonly ITemplateEngine _templateEngine;
    
    public async Task SendInvoiceEmail(Invoice invoice)
    {
        // Load template
        var template = File.ReadAllText(
            "Templates/InvoiceEmail.html"
        );
        
        // Render with data
        var html = _templateEngine.Render(template, new
        {
            CustomerName = invoice.Customer.Name,
            InvoiceNumber = invoice.Number,
            Amount = invoice.Total,
            DueDate = invoice.DueDate
        });
        
        // Send email
        await _emailService.Send(
            invoice.Customer.Email,
            "Invoice #" + invoice.Number,
            html
        );
    }
}
```

**Template Example** (InvoiceEmail.html):
```html
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; }
        .invoice { border: 1px solid #ccc; padding: 20px; }
        .total { font-weight: bold; font-size: 18px; }
    </style>
</head>
<body>
    <h2>Invoice #{{InvoiceNumber}}</h2>
    <p>Dear {{CustomerName}},</p>
    <p>Your invoice is ready:</p>
    <div class="invoice">
        <p class="total">Amount Due: ${{Amount}}</p>
        <p>Due Date: {{DueDate}}</p>
    </div>
    <p>Thank you for your business!</p>
</body>
</html>
```

## For Human Developers

Templates separate presentation from logic:

### Template Types:

1. **Email Templates**: HTML with inline CSS
2. **PDF Templates**: Razor views compiled to PDF
3. **SMS Templates**: Plain text with variable placeholders
4. **Report Templates**: Formatted reports with tables/charts

### Best Practices:
- Use placeholders for dynamic content ({{VariableName}})
- Include inline CSS for emails (external CSS often blocked)
- Test templates with sample data
- Keep templates responsive for mobile viewing
- Version templates when making significant changes
- Use template engine (Razor, Mustache, etc.) for complex logic
- Store templates as embedded resources or separate files
- Provide plain text alternative for HTML emails

### Template Engines:
- **RazorEngine**: For complex templates with C# logic
- **Mustache**: For simple variable substitution
- **Custom**: String.Replace for very simple templates
