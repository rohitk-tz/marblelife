# API/Areas/Review - AI Context

## Purpose

The **Review** area manages customer reviews, ratings, feedback collection, and integration with external review platforms (Google, Yelp, Facebook, etc.). It handles the review lifecycle from request through publication and response management.

## Key Functionality

### Review Collection
- Request reviews from customers
- Collect ratings (1-5 stars)
- Collect written feedback
- Photo/video uploads with reviews
- Review verification

### Review Management
- View all reviews
- Respond to reviews
- Flag inappropriate reviews
- Track review metrics
- Sentiment analysis

### Platform Integration
- Post reviews to Google My Business
- Sync with Yelp
- Facebook recommendations
- Third-party review sites
- Review aggregation

### Review Marketing
- Display best reviews
- Generate review widgets
- Share reviews on social media
- Review-based case studies
- Testimonial management

## Key Controllers

### ReviewController.cs
Primary review management operations.

**Endpoints**:
- `GET /Review/Review/{id}` - Get review details
- `GET /Review/Review/GetList` - Get review list with filters
- `POST /Review/Review/Submit` - Submit new review
- `POST /Review/Review/Request` - Request review from customer
- `POST /Review/Review/Respond` - Respond to review
- `GET /Review/Review/GetStats` - Get review statistics

### ReviewPlatformController.cs
External review platform integration.

**Endpoints**:
- `POST /Review/ReviewPlatform/SyncGoogle` - Sync Google reviews
- `POST /Review/ReviewPlatform/SyncYelp` - Sync Yelp reviews
- `POST /Review/ReviewPlatform/PublishReview` - Publish review to platforms
- `GET /Review/ReviewPlatform/GetPlatformStats` - Get platform-specific metrics

## Key ViewModels

```csharp
public class ReviewViewModel
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public long JobId { get; set; }
    public long FranchiseeId { get; set; }
    
    // Review Content
    public int Rating { get; set; }  // 1-5 stars
    public string Title { get; set; }
    public string Comment { get; set; }
    public List<string> PhotoUrls { get; set; }
    
    // Metadata
    public DateTime ReviewDate { get; set; }
    public ReviewSource Source { get; set; }
    public bool IsVerified { get; set; }
    public ReviewStatus Status { get; set; }
    
    // Response
    public string ResponseText { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string ResponseBy { get; set; }
    
    // Analytics
    public int HelpfulCount { get; set; }
    public double SentimentScore { get; set; }  // -1 to 1
}

public class ReviewRequestModel
{
    public long CustomerId { get; set; }
    public long JobId { get; set; }
    public string TemplateId { get; set; }
    public DateTime? ScheduledSendDate { get; set; }
    public ReviewRequestMethod Method { get; set; }  // Email, SMS, Both
}

public class ReviewStatsViewModel
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; }  // 1-5 stars: count
    public int ReviewsThisMonth { get; set; }
    public double MonthOverMonthChange { get; set; }
    public int ResponseRate { get; set; }
    public double AverageResponseTime { get; set; }  // Hours
}

public enum ReviewSource
{
    Internal = 1,
    Google = 2,
    Yelp = 3,
    Facebook = 4,
    Other = 5
}

public enum ReviewStatus
{
    Pending = 1,
    Published = 2,
    Flagged = 3,
    Removed = 4
}

public enum ReviewRequestMethod
{
    Email = 1,
    SMS = 2,
    Both = 3
}
```

## Review Request Process

### Automated Request Trigger
```csharp
// Triggered after job completion
public async Task SendReviewRequest(long jobId)
{
    var job = await _jobService.Get(jobId);
    
    // Wait configured delay (e.g., 24 hours) after job completion
    var sendDate = job.CompletionDate.AddHours(24);
    
    var request = new ReviewRequestModel
    {
        CustomerId = job.CustomerId,
        JobId = jobId,
        TemplateId = "PostJobReview",
        ScheduledSendDate = sendDate,
        Method = ReviewRequestMethod.Both
    };
    
    await _reviewService.RequestReview(request);
}
```

### Review Request Email Template
```
Subject: How was your experience with MarbleLife?

Hi {CustomerName},

Thank you for choosing MarbleLife for your recent {ServiceType} service.

We'd love to hear about your experience! Please take a moment to share your feedback:

[Rate Your Experience: ⭐⭐⭐⭐⭐]

Your review helps us improve and helps other customers make informed decisions.

Best regards,
{FranchiseeName}
```

## Review Response Best Practices

### Positive Review Response Template
```
Thank you so much for your kind words, {CustomerName}! 

We're thrilled to hear that you're happy with your {ServiceType}. 
{TechnicianName} will be delighted to hear your feedback.

We look forward to serving you again!

- {FranchiseeName} Team
```

### Negative Review Response Template
```
Thank you for sharing your feedback, {CustomerName}. 

We sincerely apologize that your experience didn't meet expectations. 
We take all feedback seriously and would like to make this right.

Please contact us at {ContactPhone} or {ContactEmail} so we can discuss 
how to resolve this matter.

- {FranchiseeName} Management
```

## Platform Integration

### Google My Business Integration
```csharp
public async Task SyncGoogleReviews(long franchiseeId)
{
    var franchisee = await _franchiseeService.Get(franchiseeId);
    var googlePlaceId = franchisee.GooglePlaceId;
    
    // Fetch reviews from Google API
    var googleReviews = await _googleMyBusinessAPI.GetReviews(googlePlaceId);
    
    foreach (var review in googleReviews)
    {
        // Check if review already exists
        var existing = await _reviewService.GetByExternalId(review.ReviewId, ReviewSource.Google);
        
        if (existing == null)
        {
            // Import new review
            await _reviewService.ImportReview(new ReviewViewModel
            {
                CustomerId = FindOrCreateCustomer(review.ReviewerName),
                FranchiseeId = franchiseeId,
                Rating = review.StarRating,
                Comment = review.Comment,
                ReviewDate = review.CreateTime,
                Source = ReviewSource.Google,
                ExternalId = review.ReviewId,
                IsVerified = true
            });
        }
    }
}
```

### Publishing Reviews
```csharp
public async Task PublishReviewToPlatforms(long reviewId, List<ReviewSource> platforms)
{
    var review = await _reviewService.Get(reviewId);
    
    // Only publish 4-5 star reviews
    if (review.Rating < 4)
        return;
    
    foreach (var platform in platforms)
    {
        try
        {
            switch (platform)
            {
                case ReviewSource.Google:
                    await _googleAPI.PostReview(review);
                    break;
                
                case ReviewSource.Facebook:
                    await _facebookAPI.PostReview(review);
                    break;
                
                // Add other platforms
            }
            
            await _reviewService.MarkPublished(reviewId, platform);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to publish review {reviewId} to {platform}", ex);
        }
    }
}
```

## Authorization

- **Public**: Submit reviews (with verification)
- **Franchisee Users**: View and respond to their franchisee's reviews
- **Super Admin**: View and manage all reviews
- **Customers**: View and submit their own reviews

## Business Rules

- Reviews can only be submitted for completed jobs
- One review per customer per job
- Review requests sent 24 hours after job completion
- Reviews with profanity automatically flagged for moderation
- Franchisees must respond to reviews within 48 hours (best practice)
- Low ratings (1-2 stars) trigger immediate notification to management
- Reviews cannot be deleted, only hidden if inappropriate
- Verified reviews have higher weight in analytics

## Sentiment Analysis

```csharp
public async Task<double> AnalyzeSentiment(string reviewText)
{
    // Use AI service (Azure Cognitive Services, AWS Comprehend, etc.)
    var sentiment = await _sentimentAnalysisService.Analyze(reviewText);
    
    // Returns score from -1 (negative) to 1 (positive)
    return sentiment.Score;
}

public ReviewSentiment ClassifySentiment(double score)
{
    if (score > 0.5) return ReviewSentiment.Positive;
    if (score < -0.5) return ReviewSentiment.Negative;
    return ReviewSentiment.Neutral;
}
```

## Metrics and Analytics

### Key Metrics
- Average rating (overall and by franchisee)
- Review volume trends
- Response rate
- Response time
- Sentiment trends
- Review source distribution
- Photo/video attachment rate
- Conversion rate (review requests → submitted reviews)

### Dashboard Endpoint
```
GET /Review/Review/GetDashboard?franchiseeId={id}

Response:
{
  "averageRating": 4.7,
  "totalReviews": 1234,
  "ratingDistribution": {
    "5": 800,
    "4": 300,
    "3": 80,
    "2": 30,
    "1": 24
  },
  "reviewsThisMonth": 45,
  "responseRate": 92,
  "averageResponseTimeHours": 6.5,
  "sentimentTrend": "improving"
}
```

## Testing

```csharp
[Test]
public void SubmitReview_ValidReview_Success()
{
    var review = new ReviewViewModel
    {
        CustomerId = 123,
        JobId = 456,
        Rating = 5,
        Comment = "Excellent service!",
        Source = ReviewSource.Internal
    };
    
    var result = _reviewController.Submit(review).Result;
    
    Assert.IsTrue(result);
}

[Test]
public void SubmitReview_Profanity_Flagged()
{
    var review = new ReviewViewModel
    {
        CustomerId = 123,
        JobId = 456,
        Rating = 1,
        Comment = "Contains profanity here...",
        Source = ReviewSource.Internal
    };
    
    var result = _reviewController.Submit(review).Result;
    var stored = _reviewService.Get(review.Id).Result;
    
    Assert.AreEqual(ReviewStatus.Flagged, stored.Status);
}
```

## Integration Points

- **Sales**: Link reviews to customers and invoices
- **Scheduler**: Link reviews to completed jobs
- **Organizations**: Franchisee review management
- **Notification**: Review request emails and alerts
- **Dashboard**: Review metrics display

## Best Practices

- Respond to all reviews within 24-48 hours
- Personalize responses using customer and job details
- Address negative reviews promptly and professionally
- Thank customers for positive reviews
- Use reviews in marketing materials (with permission)
- Monitor review trends for quality issues
- Incentivize reviews but never pay for positive reviews
- Make review process easy (one-click rating)
