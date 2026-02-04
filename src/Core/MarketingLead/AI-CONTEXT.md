# Core/MarketingLead - AI Context

## Purpose

The **MarketingLead** module manages lead generation, lead source integration, lead qualification, and conversion tracking from various marketing channels (Google Ads, Facebook, HomeAdvisor, direct website, etc.).

## Key Entities (Domain/)

### Lead Management
- **MarketingLead**: Core lead entity from all sources
- **LeadSource**: Channel attribution (Google, Facebook, Referral, etc.)
- **LeadActivity**: Interaction history with leads
- **LeadScore**: Automated lead scoring and prioritization
- **LeadAssignment**: Routing leads to franchisees

### Lead Integration
- **HomeAdvisorLead**: HomeAdvisor-specific lead data
- **GoogleAdsLead**: Google Ads campaign leads
- **FacebookLead**: Facebook Lead Ads integration
- **WebFormLead**: Direct website submissions
- **ReferralLead**: Customer referral tracking

### Conversion Tracking
- **LeadConversion**: Conversion events and outcomes
- **ConversionPath**: Multi-touch attribution
- **LeadROI**: Marketing ROI calculations
- **CampaignPerformance**: Campaign-level metrics

### Communication
- **LeadCommunication**: Email/SMS/call logs
- **LeadNote**: Internal notes and follow-up reminders
- **LeadResponse**: Response templates and automation

## Service Interfaces

### Lead Services
- **IMarketingLeadFactory**: Lead creation from various sources
- **IMarketingLeadService**: Lead lifecycle management
- **ILeadScoreService**: Automated lead scoring
- **ILeadAssignmentService**: Routing and assignment logic
- **ILeadQualificationService**: Lead qualification workflow

### Integration Services
- **IHomeAdvisorParser**: Parse HomeAdvisor lead data
- **IHomeAdvisorFileParser**: Bulk lead import from files
- **IGetRoutingNumberService**: Dynamic phone number routing
- **IReviewPushLocationAPI**: Review platform integration
- **IUpdateMarketingLeadReportDataService**: Reporting data sync
- **IUpdateConvertedLeadsService**: Conversion tracking updates

### Analytics Services
- **ILeadReportService**: Lead performance metrics
- **IConversionTrackingService**: Attribution and conversion analytics
- **ICampaignROIService**: Campaign effectiveness
- **ILeadSourceAnalysisService**: Channel performance comparison

## Implementations (Impl/)

Business logic including:
- Lead deduplication algorithms
- Automated lead scoring models
- Geographic lead routing
- Real-time lead notification
- Multi-channel attribution
- Conversion funnel tracking

## Enumerations (Enum/)

- **LeadSourceType**: GoogleAds, Facebook, HomeAdvisor, Website, Referral, Phone, Email, Direct
- **LeadStatus**: New, Contacted, Qualified, Quoted, Converted, Lost, Invalid
- **LeadQuality**: Hot, Warm, Cold, Unqualified
- **LeadType**: Residential, Commercial, Emergency, Maintenance
- **ConversionType**: Customer, Quote, Appointment, NoSale
- **ContactMethod**: Phone, Email, SMS, WebChat
- **LeadPriority**: Low, Medium, High, Urgent

## ViewModels (ViewModel/)

- **MarketingLeadViewModel**: Complete lead data
- **LeadSummaryViewModel**: List view with key metrics
- **HomeAdvisorLeadViewModel**: HomeAdvisor-specific fields
- **LeadConversionViewModel**: Conversion event data
- **LeadReportViewModel**: Reporting and analytics
- **CampaignPerformanceViewModel**: Campaign metrics
- **LeadSourceROIViewModel**: Source-level ROI

## Business Rules

### Lead Capture
1. Leads automatically captured from integrated sources
2. Duplicate detection based on email, phone, and address
3. Leads auto-assigned to franchisee based on service area
4. Real-time notification to assigned franchisee
5. Lead response time tracked (SLA: respond within 1 hour)

### Lead Scoring
1. Automated scoring based on:
   - Lead source quality
   - Service type and project size
   - Geographic location
   - Time of inquiry
   - Historical conversion rates
2. Scores range 0-100
3. Hot leads (80+) trigger urgent notifications
4. Score updates based on interaction history

### Lead Assignment
1. Geographic-based routing (ZIP code matching)
2. Round-robin for multi-franchisee territories
3. Load balancing based on franchisee capacity
4. Skill-based routing for specialized services
5. Premium franchisees get priority for high-value leads

### Conversion Tracking
1. Track conversion at multiple stages: contact, quote, sale
2. Multi-touch attribution model
3. Time-to-conversion metrics
4. Lost lead reasons captured for analysis
5. ROI calculated per source and campaign

## Dependencies

- **Core/Organizations**: Franchisee assignment and territories
- **Core/Sales**: Lead-to-customer conversion
- **Core/Geo**: Geographic routing and territory matching
- **Core/Notification**: Real-time lead notifications
- **Core/Scheduler**: Appointment scheduling from leads
- **Infrastructure/AWS**: S3 for lead file storage

## For AI Agents

### Processing Incoming Lead
```csharp
// Capture lead from web form
var lead = _marketingLeadFactory.Create(new MarketingLeadViewModel
{
    FirstName = "Jane",
    LastName = "Doe",
    Email = "jane@example.com",
    Phone = "(555) 987-6543",
    Source = LeadSourceType.Website,
    ServiceType = "Floor Restoration",
    ProjectDescription = "Need marble floor restored in foyer",
    Address = "456 Elm St, Chicago, IL 60601"
});

// Auto-assign to franchisee
var franchisee = _leadAssignmentService.AssignLeadToFranchisee(lead);

// Calculate lead score
var score = _leadScoreService.ScoreLead(lead.Id);

// Send notification if high-priority
if (score >= 80)
{
    _notificationService.SendUrgentLeadAlert(franchisee.Id, lead.Id);
}
```

### HomeAdvisor Integration
```csharp
// Parse HomeAdvisor leads from email or file
var parser = _homeAdvisorFileParser;
var leads = parser.ParseLeads(fileContent);

foreach (var leadData in leads)
{
    // Check for duplicates
    var existing = _marketingLeadService.FindDuplicate(leadData.Email, leadData.Phone);
    
    if (existing == null)
    {
        var lead = _marketingLeadFactory.CreateFromHomeAdvisor(leadData);
        _leadAssignmentService.AssignLeadToFranchisee(lead);
    }
}
```

### Lead Qualification and Conversion
```csharp
// Qualify lead
_leadQualificationService.Qualify(leadId, new LeadQualificationViewModel
{
    IsValidContact = true,
    HasBudget = true,
    HasTimeline = true,
    HasAuthority = true,
    NeedsService = true,
    QualityScore = LeadQuality.Hot
});

// Convert to customer and opportunity
var conversion = _updateConvertedLeadsService.Convert(leadId, new ConversionViewModel
{
    ConversionType = ConversionType.Customer,
    ConversionDate = DateTime.Now,
    QuoteValue = 7500m,
    Notes = "Ready to proceed with full restoration"
});

// Track in sales pipeline
_conversionTrackingService.TrackConversion(leadId, conversion);
```

## For Human Developers

### Common Workflows

#### 1. Lead Import from External Source
```csharp
// Bulk import from CSV/Excel
var importResult = _marketingLeadService.BulkImport(fileStream, new ImportOptions
{
    Source = LeadSourceType.HomeAdvisor,
    AutoAssign = true,
    SendNotifications = true,
    SkipDuplicates = true
});

// Returns: successCount, duplicateCount, errorCount, errors[]
```

#### 2. Lead Analytics
```csharp
// Campaign performance
var performance = _campaignROIService.GetCampaignMetrics(campaignId, startDate, endDate);

// Lead source comparison
var sourceAnalysis = _leadSourceAnalysisService.Compare(
    new[] { LeadSourceType.GoogleAds, LeadSourceType.Facebook, LeadSourceType.HomeAdvisor },
    startDate,
    endDate
);

// Conversion funnel
var funnel = _conversionTrackingService.GetConversionFunnel(franchiseeId, dateRange);
```

#### 3. Lead Routing Configuration
```csharp
// Configure routing rules
_leadAssignmentService.ConfigureRules(franchiseeId, new RoutingRules
{
    ServiceArea = new[] { "60601", "60602", "60603" }, // ZIP codes
    ServiceTypes = new[] { "Restoration", "Cleaning", "Polishing" },
    MaxLeadsPerDay = 10,
    Priority = FranchiseePriority.Premium,
    SpecialtyServices = new[] { "Terrazzo", "Travertine" }
});
```

### Best Practices
- Respond to leads within 1 hour (industry standard)
- Implement webhook receivers for real-time lead capture
- Use distributed locking for lead assignment to prevent race conditions
- Log all lead interactions for compliance and analytics
- Implement retry logic for external API integrations
- Cache lead scores to reduce computation overhead
- Validate phone numbers and emails before creating leads
- Track partial conversions (quote requested, appointment scheduled)
- Implement A/B testing for lead response strategies
- Monitor lead source quality and adjust scoring models accordingly
