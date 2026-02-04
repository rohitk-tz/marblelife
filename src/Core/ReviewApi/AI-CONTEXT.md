# Core/ReviewApi - AI Context

## Purpose

The **ReviewApi** module handles integration with external review platforms like Google My Business, Facebook, Yelp, and other review aggregation services.

## Key Components

### External Integrations
- Google My Business API integration
- Facebook Graph API for reviews
- Yelp Fusion API
- Review aggregation service APIs

## Service Interfaces

- **IReviewPushLocationAPI**: Push reviews to external platforms
- **IReviewApiClient**: Generic review API client
- **IReviewImportService**: Import reviews from platforms
- **IReviewPushService**: Push internal reviews to platforms

## Implementations (Impl/)

Business logic for:
- OAuth authentication with review platforms
- API rate limiting and throttling
- Review data transformation
- Error handling and retry logic
- Webhook handling for review notifications

## ViewModels (ViewModel/)

- **ExternalReviewViewModel**: Normalized review data
- **ReviewApiConfigViewModel**: API connection settings
- **ReviewSyncViewModel**: Sync status and logs

## Business Rules

1. **Sync Frequency**: Import reviews every 6 hours
2. **API Limits**: Respect platform rate limits
3. **Data Privacy**: Comply with platform terms of service
4. **Deduplication**: Prevent duplicate review imports

## Dependencies

- **Core/Review**: Internal review management
- **Infrastructure**: HTTP clients, OAuth
- **Core/Organizations**: Platform credentials per franchisee

## For AI Agents

### Importing Reviews
```csharp
// Import Google reviews
var reviews = await _reviewImportService.ImportFromGoogle(franchiseeId);

// Process and store
foreach (var review in reviews)
{
    var existing = _reviewService.FindByExternalId(review.ExternalId);
    if (existing == null)
    {
        _reviewFactory.CreateFromExternal(review);
    }
}
```

### Pushing Reviews
```csharp
// Push review to external platform
await _reviewPushService.Push(new ReviewPushViewModel
{
    ReviewId = internalReviewId,
    Platform = ReviewPlatform.Google,
    FranchiseeId = franchiseeId
});
```

## For Human Developers

### Best Practices
- Use OAuth 2.0 for platform authentication
- Implement exponential backoff for retries
- Cache API responses where appropriate
- Monitor API quota usage
- Handle platform-specific requirements
- Validate data before pushing to platforms
- Log all API interactions for debugging
