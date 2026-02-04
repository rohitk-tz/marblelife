# API/Areas/MarketingLead - AI Context

## Purpose

The **MarketingLead** area manages lead generation, lead tracking, lead qualification, and conversion to customers. It handles the top of the sales funnel from initial contact through qualification and handoff to sales.

## Key Functionality

### Lead Capture
- Web form submissions
- Phone call tracking
- Referral tracking
- Marketing campaign attribution
- Lead import from external sources

### Lead Management
- Lead assignment to franchisees
- Lead scoring and qualification
- Follow-up task creation
- Lead status tracking
- Duplicate detection

### Lead Conversion
- Convert lead to customer
- Conversion rate tracking
- Source attribution
- ROI calculation by source

### Lead Nurturing
- Automated follow-up sequences
- Email campaigns
- SMS reminders
- Lead warming workflows

## Key Controllers

### LeadController.cs
Primary lead management operations.

**Endpoints**:
- `POST /MarketingLead/Lead/Submit` - Submit new lead (public endpoint)
- `GET /MarketingLead/Lead/{id}` - Get lead details
- `GET /MarketingLead/Lead/GetList` - Get lead list with filters
- `POST /MarketingLead/Lead/Assign` - Assign lead to franchisee
- `POST /MarketingLead/Lead/UpdateStatus` - Update lead status
- `POST /MarketingLead/Lead/Convert` - Convert lead to customer
- `POST /MarketingLead/Lead/Import` - Bulk import leads

### LeadSourceController.cs
Lead source tracking and analytics.

**Endpoints**:
- `GET /MarketingLead/LeadSource/GetSources` - Get lead sources
- `GET /MarketingLead/LeadSource/GetPerformance` - Get source performance metrics
- `POST /MarketingLead/LeadSource/Create` - Create lead source

## Key ViewModels

```csharp
public class LeadViewModel
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    
    // Lead Information
    public string Source { get; set; }
    public string Campaign { get; set; }
    public string Medium { get; set; }
    public string ReferralCode { get; set; }
    
    // Interest
    public List<string> InterestedServices { get; set; }
    public string Message { get; set; }
    public DateTime? PreferredContactDate { get; set; }
    
    // Tracking
    public LeadStatus Status { get; set; }
    public int LeadScore { get; set; }
    public long? AssignedFranchiseeId { get; set; }
    public long? AssignedToUserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastContactedDate { get; set; }
}

public class LeadSourcePerformanceViewModel
{
    public string Source { get; set; }
    public int TotalLeads { get; set; }
    public int QualifiedLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageLeadValue { get; set; }
    public decimal ROI { get; set; }
}

public enum LeadStatus
{
    New = 1,
    Contacted = 2,
    Qualified = 3,
    Unqualified = 4,
    Converted = 5,
    Lost = 6
}
```

## Lead Assignment Rules

### Geographic Assignment
Leads automatically assigned to franchisee based on service territory (zip code/city match).

### Load Balancing
If multiple franchisees serve the same area, distribute leads based on:
- Current workload
- Response time history
- Capacity settings
- Round-robin

### Manual Override
Super admins can manually reassign leads.

## Lead Scoring

Leads scored 0-100 based on:
- **Completeness** (20 points): Full contact information provided
- **Intent** (30 points): Specific service interest, preferred date set
- **Value** (25 points): Service type value, property size
- **Source** (15 points): High-converting sources score higher
- **Engagement** (10 points): Email opens, website visits

## Business Rules

- New leads must be contacted within 1 hour (configurable)
- Leads not contacted within 24 hours escalated to management
- Duplicate detection based on email/phone/address
- Leads older than 90 days auto-archived
- Lead sources tracked for marketing ROI
- GDPR/privacy compliance for lead data

## Authorization

- **Public**: Can submit leads (no authentication)
- **Franchisee Users**: View and manage assigned leads
- **Super Admin**: View and manage all leads
- **Read-Only**: View leads, cannot modify

## Integration Points

- **Sales**: Convert lead to customer
- **Organizations**: Franchisee territory assignments
- **Scheduler**: Schedule estimate appointments
- **Notification**: Lead notifications and follow-ups
- **Review**: Track review-based referrals

## API Usage Example

### Submit Lead (Public)
```javascript
fetch('/MarketingLead/Lead/Submit', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        firstName: 'John',
        lastName: 'Doe',
        email: 'john@example.com',
        phone: '555-1234',
        zipCode: '12345',
        interestedServices: ['Marble Restoration'],
        source: 'Website',
        campaign: 'Spring2024',
        message: 'Interested in marble floor restoration'
    })
})
```

### Get Assigned Leads
```javascript
fetch('/MarketingLead/Lead/GetList?status=1&assignedToMe=true', {
    headers: { 'token': userToken }
})
```

## Metrics Tracked

- Lead volume by source
- Response time
- Contact rate
- Qualification rate
- Conversion rate
- Time to conversion
- Lead value
- Cost per lead (if cost data available)
- ROI by source
