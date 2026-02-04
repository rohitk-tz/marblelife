# Core/Review - AI Context

## Purpose

The **Review** module manages customer reviews, ratings, feedback collection, and review moderation for the MarbleLife platform's reputation management system.

## Key Entities (Domain/)

### Review Management
- **Review**: Customer review entity
- **Rating**: Numeric rating (1-5 stars)
- **ReviewResponse**: Franchisee responses to reviews
- **ReviewRequest**: Review solicitation tracking
- **ReviewModeration**: Moderation queue and actions

### Review Sources
- **GoogleReview**: Google Business reviews
- **FacebookReview**: Facebook reviews
- **YelpReview**: Yelp reviews
- **InternalReview**: Platform-specific reviews

## Service Interfaces

### Core Review Services
- **IReviewFactory**: Review creation
- **IReviewService**: Review management
- **IReviewRequestService**: Review solicitation
- **IReviewResponseService**: Response management
- **IReviewModerationService**: Content moderation

### Integration Services
- **IReviewAggregationService**: Multi-platform aggregation
- **IReviewSyncService**: External platform sync
- **IReviewAnalyticsService**: Sentiment analysis

## Implementations (Impl/)

Business logic for:
- Automatic review requests post-service
- Multi-platform review aggregation
- Sentiment analysis
- Review moderation workflows
- Response templates

## Enumerations (Enum/)

- **ReviewSource**: Internal, Google, Facebook, Yelp, HomeAdvisor
- **ReviewStatus**: Pending, Published, Moderated, Removed
- **Rating**: OneStar, TwoStar, ThreeStar, FourStar, FiveStar
- **ReviewCategory**: Service, Quality, Pricing, Communication

## ViewModels (ViewModel/)

- **ReviewViewModel**: Complete review data
- **ReviewRequestViewModel**: Review solicitation
- **ReviewResponseViewModel**: Franchisee response
- **ReviewAnalyticsViewModel**: Review metrics

## Business Rules

1. **Automatic Requests**: Send review request 24 hours after job completion
2. **Moderation**: All reviews reviewed for inappropriate content
3. **Responses**: Franchisees can respond to reviews
4. **Aggregation**: Import reviews from external platforms daily
5. **Analytics**: Track average ratings and trends

## Dependencies

- **Core/Scheduler**: Job completion triggers review requests
- **Core/Organizations**: Franchisee review aggregation
- **Core/Notification**: Review request emails
- **Infrastructure/ReviewApi**: External API integrations

## For AI Agents

### Requesting Review
```csharp
// Send review request after job completion
var request = _reviewRequestService.Create(new ReviewRequestViewModel
{
    CustomerId = customerId,
    JobId = jobId,
    FranchiseeId = franchiseeId,
    RequestDate = DateTime.Now.AddHours(24)
});

// Email sent automatically via notification service
```

### Submitting Review
```csharp
// Customer submits review
var review = _reviewFactory.Create(new ReviewViewModel
{
    JobId = jobId,
    CustomerId = customerId,
    Rating = 5,
    Title = "Excellent service!",
    Comment = "Very professional and thorough work.",
    Categories = new[] { ReviewCategory.Service, ReviewCategory.Quality }
});

// Auto-moderation check
_reviewModerationService.CheckForInappropriateContent(review.Id);
```

### Responding to Review
```csharp
// Franchisee responds
var response = _reviewResponseService.Create(new ReviewResponseViewModel
{
    ReviewId = reviewId,
    FranchiseeId = franchiseeId,
    ResponseText = "Thank you for your feedback! We're glad you were satisfied.",
    RespondedBy = userId
});
```

## For Human Developers

### Best Practices
- Send review requests at optimal times (24-48 hours post-service)
- Make review process easy (one-click links)
- Respond to negative reviews promptly
- Monitor review trends for quality issues
- Incentivize reviews (within platform guidelines)
- Aggregate reviews from all platforms
- Implement sentiment analysis for early warning
- Train franchisees on review response best practices
