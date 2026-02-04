# ReviewSystemAPITest - Review Integration Testing Service

## Overview
The ReviewSystemAPITest is a console application designed for testing and debugging the integration with external review systems (such as Google Reviews, Yelp, Facebook, and custom review platforms). It provides a sandbox environment for testing API connections, webhook handlers, and review data synchronization without affecting production data.

## Purpose
- Test review system API integrations
- Validate API credentials and configuration
- Debug webhook handlers
- Test review data parsing and storage
- Simulate review notifications
- Verify customer feedback workflows
- Test review response automation

## Technology Stack
- **.NET Framework**: C# Console Application
- **HTTP Client**: System.Net.Http for API calls
- **JSON Processing**: Newtonsoft.Json
- **Database**: Entity Framework Core
- **Dependency Injection**: Custom DI container
- **Logging**: Core.Application.ILogService

## Project Structure
```
/ReviewSystemAPITest
├── ReviewSystemAPITest.csproj         # Project file
├── Program.cs                         # Entry point and test scenarios
├── AppContextStore.cs                 # Context management
├── WinJobSessionContext.cs            # Session handling
├── App.config                         # Configuration
└── /Properties
    └── AssemblyInfo.cs
```

## Main Implementation

### Program.cs
```csharp
using Core.Application;
using Core.Review;
using DependencyInjection;
using System;

namespace ReviewSystemAPITest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Review System API Test Starting...\n");
                
                // Setup dependency injection
                DependencyRegistrar.RegisterDependencies();
                ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
                ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
                DependencyRegistrar.SetupCurrentContextWinJob();

                // Test configuration
                string clientId = "5a475638898b9b51b8fa2f241d24bc241ab8603b";
                int businessId = 34700;
                int businessId_2 = 35346;
                long customerId = 28477443;

                var feedbackService = ApplicationManager.DependencyInjection
                    .Resolve<ICustomerFeedbackService>();

                // Run tests
                Console.WriteLine("Running Review System Tests...\n");
                
                // Test 1: Get Feedback
                TestGetFeedback(feedbackService, clientId, businessId);
                
                // Test 2: Send Review Request
                TestSendReviewRequest(feedbackService, customerId, businessId);
                
                // Test 3: Process Review Response
                TestProcessReviewResponse(feedbackService, customerId);
                
                // Test 4: Sync Reviews
                TestSyncReviews(feedbackService, businessId);
                
                Console.WriteLine("\nAll tests completed!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                
                Environment.Exit(1);
            }
        }

        static void TestGetFeedback(ICustomerFeedbackService service, string clientId, int businessId)
        {
            Console.WriteLine("TEST 1: Get Feedback from Review System");
            Console.WriteLine(new string('-', 50));
            
            try
            {
                var feedback = service.GetFeedback(clientId, businessId);
                
                if (feedback != null)
                {
                    Console.WriteLine($"✓ Successfully retrieved feedback");
                    Console.WriteLine($"  Business ID: {businessId}");
                    Console.WriteLine($"  Total Reviews: {feedback.TotalReviews}");
                    Console.WriteLine($"  Average Rating: {feedback.AverageRating:F2}");
                    Console.WriteLine($"  Recent Reviews: {feedback.RecentReviews?.Count ?? 0}");
                }
                else
                {
                    Console.WriteLine("✗ No feedback retrieved");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test failed: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        static void TestSendReviewRequest(ICustomerFeedbackService service, long customerId, int businessId)
        {
            Console.WriteLine("TEST 2: Send Review Request to Customer");
            Console.WriteLine(new string('-', 50));
            
            try
            {
                var result = service.SendReviewRequest(customerId, businessId);
                
                if (result)
                {
                    Console.WriteLine($"✓ Review request sent successfully");
                    Console.WriteLine($"  Customer ID: {customerId}");
                    Console.WriteLine($"  Business ID: {businessId}");
                }
                else
                {
                    Console.WriteLine("✗ Failed to send review request");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test failed: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        static void TestProcessReviewResponse(ICustomerFeedbackService service, long customerId)
        {
            Console.WriteLine("TEST 3: Process Review Response");
            Console.WriteLine(new string('-', 50));
            
            try
            {
                var mockReview = new CustomerReview
                {
                    CustomerId = customerId,
                    Rating = 5,
                    Comment = "Excellent service! Very professional and thorough.",
                    ReviewDate = DateTime.UtcNow,
                    Source = ReviewSource.Email
                };
                
                var result = service.ProcessReview(mockReview);
                
                if (result)
                {
                    Console.WriteLine($"✓ Review processed successfully");
                    Console.WriteLine($"  Customer ID: {customerId}");
                    Console.WriteLine($"  Rating: {mockReview.Rating}");
                    Console.WriteLine($"  Comment: {mockReview.Comment}");
                }
                else
                {
                    Console.WriteLine("✗ Failed to process review");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test failed: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        static void TestSyncReviews(ICustomerFeedbackService service, int businessId)
        {
            Console.WriteLine("TEST 4: Sync Reviews from External Systems");
            Console.WriteLine(new string('-', 50));
            
            try
            {
                var syncResult = service.SyncReviews(businessId);
                
                Console.WriteLine($"✓ Review sync completed");
                Console.WriteLine($"  Business ID: {businessId}");
                Console.WriteLine($"  Reviews Synced: {syncResult.SyncedCount}");
                Console.WriteLine($"  New Reviews: {syncResult.NewReviews}");
                Console.WriteLine($"  Updated Reviews: {syncResult.UpdatedReviews}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test failed: {ex.Message}");
            }
            
            Console.WriteLine();
        }
    }
}
```

## Test Scenarios

### 1. API Connection Test
```csharp
public class ApiConnectionTest
{
    public void TestConnection(string apiUrl, string apiKey)
    {
        Console.WriteLine($"Testing connection to: {apiUrl}");
        
        try
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.Timeout = TimeSpan.FromSeconds(30);
                
                var response = client.GetAsync($"{apiUrl}/health").Result;
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✓ Connection successful");
                    Console.WriteLine($"  Status: {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine("✗ Connection failed");
                    Console.WriteLine($"  Status: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Connection error: {ex.Message}");
        }
    }
}
```

### 2. Review Data Parsing Test
```csharp
public class ReviewParsingTest
{
    public void TestParseGoogleReview(string jsonResponse)
    {
        Console.WriteLine("Testing Google Review parsing...");
        
        try
        {
            var review = JsonConvert.DeserializeObject<GoogleReviewResponse>(jsonResponse);
            
            Console.WriteLine($"✓ Review parsed successfully");
            Console.WriteLine($"  Author: {review.AuthorName}");
            Console.WriteLine($"  Rating: {review.Rating}");
            Console.WriteLine($"  Text: {review.Text}");
            Console.WriteLine($"  Time: {review.Time}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Parsing failed: {ex.Message}");
        }
    }
}
```

### 3. Webhook Handler Test
```csharp
public class WebhookTest
{
    public void TestWebhookPayload(string webhookUrl, object payload)
    {
        Console.WriteLine($"Testing webhook: {webhookUrl}");
        
        try
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = client.PostAsync(webhookUrl, content).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✓ Webhook processed successfully");
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"  Response: {responseBody}");
                }
                else
                {
                    Console.WriteLine("✗ Webhook processing failed");
                    Console.WriteLine($"  Status: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Webhook error: {ex.Message}");
        }
    }
}
```

### 4. Review Notification Test
```csharp
public class NotificationTest
{
    public void TestReviewNotification(long customerId, string email)
    {
        Console.WriteLine($"Testing review notification for customer: {customerId}");
        
        try
        {
            var emailService = ApplicationManager.DependencyInjection
                .Resolve<IEmailService>();
            
            var template = new ReviewRequestEmailTemplate
            {
                CustomerEmail = email,
                ReviewUrl = $"https://reviews.marblelife.com/leave-review?customer={customerId}",
                FranchiseeName = "Marble Life Test"
            };
            
            var result = emailService.SendReviewRequest(template);
            
            if (result)
            {
                Console.WriteLine("✓ Review notification sent successfully");
            }
            else
            {
                Console.WriteLine("✗ Failed to send notification");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Notification error: {ex.Message}");
        }
    }
}
```

## Review System Integration Models

### Google Reviews Integration
```csharp
public class GoogleReviewResponse
{
    [JsonProperty("author_name")]
    public string AuthorName { get; set; }
    
    [JsonProperty("author_url")]
    public string AuthorUrl { get; set; }
    
    [JsonProperty("profile_photo_url")]
    public string ProfilePhotoUrl { get; set; }
    
    [JsonProperty("rating")]
    public int Rating { get; set; }
    
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("time")]
    public long Time { get; set; }
    
    [JsonProperty("relative_time_description")]
    public string RelativeTimeDescription { get; set; }
}
```

### Yelp Integration
```csharp
public class YelpReviewResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("rating")]
    public int Rating { get; set; }
    
    [JsonProperty("text")]
    public string Text { get; set; }
    
    [JsonProperty("time_created")]
    public DateTime TimeCreated { get; set; }
    
    [JsonProperty("user")]
    public YelpUser User { get; set; }
}

public class YelpUser
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }
}
```

### Custom Review System
```csharp
public class CustomerReview
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime ReviewDate { get; set; }
    public ReviewSource Source { get; set; }
    public string ExternalId { get; set; }
    public bool IsPublished { get; set; }
    public string ResponseText { get; set; }
    public DateTime? ResponseDate { get; set; }
}

public enum ReviewSource
{
    Google = 1,
    Yelp = 2,
    Facebook = 3,
    Email = 4,
    Website = 5
}
```

## Mock Data Generation

```csharp
public class MockDataGenerator
{
    public static List<CustomerReview> GenerateMockReviews(int count)
    {
        var reviews = new List<CustomerReview>();
        var random = new Random();
        
        var comments = new[]
        {
            "Excellent service! Very professional.",
            "Great work on our marble floors.",
            "Highly recommend Marble Life!",
            "Outstanding results, very satisfied.",
            "Professional team, quality work."
        };
        
        for (int i = 0; i < count; i++)
        {
            reviews.Add(new CustomerReview
            {
                Id = i + 1,
                CustomerId = 10000 + i,
                Rating = random.Next(4, 6), // 4-5 stars
                Comment = comments[random.Next(comments.Length)],
                ReviewDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                Source = (ReviewSource)random.Next(1, 6),
                IsPublished = true
            });
        }
        
        return reviews;
    }
}
```

## Configuration

### App.config
```xml
<configuration>
  <appSettings>
    <!-- Google Reviews -->
    <add key="GoogleApiKey" value="your_google_api_key" />
    <add key="GooglePlaceId" value="ChIJN1t_tDeuEmsRUsoyG83frY4" />
    
    <!-- Yelp -->
    <add key="YelpApiKey" value="your_yelp_api_key" />
    <add key="YelpBusinessId" value="marble-life-anytown" />
    
    <!-- Custom Review System -->
    <add key="ReviewSystemApiUrl" value="https://reviews.marblelife.com/api" />
    <add key="ReviewSystemApiKey" value="your_api_key" />
    
    <!-- Webhook -->
    <add key="WebhookUrl" value="https://app.marblelife.com/webhooks/reviews" />
    <add key="WebhookSecret" value="your_webhook_secret" />
    
    <!-- Test Mode -->
    <add key="TestMode" value="true" />
    <add key="MockDataCount" value="10" />
  </appSettings>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=.;Database=MarbleLife;Integrated Security=true;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

## Test Results Reporting

```csharp
public class TestReport
{
    public List<TestResult> Results { get; set; } = new List<TestResult>();
    
    public void AddResult(string testName, bool passed, string message = null)
    {
        Results.Add(new TestResult
        {
            TestName = testName,
            Passed = passed,
            Message = message,
            Timestamp = DateTime.Now
        });
    }
    
    public void PrintSummary()
    {
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("TEST SUMMARY");
        Console.WriteLine(new string('=', 70));
        
        int passed = Results.Count(r => r.Passed);
        int failed = Results.Count - passed;
        
        Console.WriteLine($"Total Tests: {Results.Count}");
        Console.WriteLine($"Passed: {passed} ({(double)passed / Results.Count * 100:F1}%)");
        Console.WriteLine($"Failed: {failed}");
        Console.WriteLine();
        
        foreach (var result in Results)
        {
            string status = result.Passed ? "✓ PASS" : "✗ FAIL";
            Console.WriteLine($"{status} - {result.TestName}");
            if (!string.IsNullOrEmpty(result.Message))
            {
                Console.WriteLine($"       {result.Message}");
            }
        }
        
        Console.WriteLine(new string('=', 70));
    }
}

public class TestResult
{
    public string TestName { get; set; }
    public bool Passed { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## Best Practices

1. **Isolation**: Test against sandbox/test accounts, not production
2. **Mock Data**: Use realistic mock data for testing
3. **Error Scenarios**: Test both success and failure cases
4. **Rate Limits**: Respect API rate limits during testing
5. **Logging**: Comprehensive logging of all test activities
6. **Validation**: Validate data formats and responses
7. **Cleanup**: Clean up test data after completion

## Common Test Scenarios

1. **API Authentication** - Verify credentials work
2. **Data Retrieval** - Test fetching reviews from external systems
3. **Data Transformation** - Test parsing external formats
4. **Webhook Processing** - Test incoming webhook payloads
5. **Email Notifications** - Test review request emails
6. **Error Handling** - Test API errors, timeouts, invalid data
7. **Rate Limiting** - Test throttling and backoff
8. **Data Sync** - Test bidirectional synchronization

## Related Services
- See `/NotificationService/AI-CONTEXT.md` for email notifications
- See Core.Review domain for review models and services
- See Web.UI for customer-facing review interface
