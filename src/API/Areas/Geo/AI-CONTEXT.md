# API/Areas/Geo - AI Context

## Purpose

The **Geo** area manages geographic data, territory management, service area definitions, location-based operations, and mapping functionality. It provides geolocation services for franchisee territory assignments, customer address validation, and routing.

## Key Functionality

### Territory Management
- Define franchisee service territories
- Territory boundary definitions (polygons)
- Zip code-based territories
- City/county-based territories
- Territory overlap detection
- Territory assignment to franchisees

### Geolocation Services
- Address geocoding (address → lat/long)
- Reverse geocoding (lat/long → address)
- Distance calculations
- Proximity searches
- Service area validation

### Mapping
- Map visualization data
- Territory boundary rendering
- Customer location mapping
- Technician location tracking
- Route optimization

### Address Validation
- Address standardization
- Address verification
- Duplicate address detection
- Service area eligibility check

## Key Controllers

### TerritoryController.cs
Territory management operations.

**Endpoints**:
- `GET /Geo/Territory/{id}` - Get territory details
- `GET /Geo/Territory/GetByFranchisee?franchiseeId={id}` - Get franchisee territories
- `POST /Geo/Territory` - Create/update territory
- `DELETE /Geo/Territory/{id}` - Delete territory
- `GET /Geo/Territory/CheckOverlap` - Check for territory overlaps
- `GET /Geo/Territory/FindByLocation?lat={lat}&lng={lng}` - Find territory by coordinates
- `GET /Geo/Territory/FindByZipCode?zipCode={zip}` - Find territory by zip code

### GeocodingController.cs
Geocoding and address validation.

**Endpoints**:
- `POST /Geo/Geocoding/GeocodeAddress` - Convert address to coordinates
- `POST /Geo/Geocoding/ReverseGeocode` - Convert coordinates to address
- `POST /Geo/Geocoding/ValidateAddress` - Validate and standardize address
- `GET /Geo/Geocoding/GetDistance` - Calculate distance between two points

### MapController.cs
Map visualization and data.

**Endpoints**:
- `GET /Geo/Map/GetTerritoryData` - Get territory boundaries for map display
- `GET /Geo/Map/GetCustomerLocations` - Get customer markers for map
- `GET /Geo/Map/GetTechnicianLocations` - Get technician locations (real-time)

## Key ViewModels

```csharp
public class TerritoryViewModel
{
    public long Id { get; set; }
    public long FranchiseeId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    // Territory Definition
    public TerritoryType Type { get; set; }
    public List<string> ZipCodes { get; set; }
    public List<string> Cities { get; set; }
    public List<string> Counties { get; set; }
    
    // For polygon-based territories
    public List<GeoPoint> BoundaryPoints { get; set; }
    
    // Metadata
    public bool IsActive { get; set; }
    public int EstimatedPopulation { get; set; }
    public decimal SquareMiles { get; set; }
}

public class GeoPoint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class AddressViewModel
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    
    // Geocoded data
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Validation
    public bool IsValidated { get; set; }
    public string StandardizedAddress { get; set; }
}

public class GeocodeRequest
{
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
}

public class GeocodeResponse
{
    public bool Success { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string FormattedAddress { get; set; }
    public AddressComponent[] AddressComponents { get; set; }
}

public enum TerritoryType
{
    ZipCode = 1,
    City = 2,
    County = 3,
    Polygon = 4,
    Radius = 5
}
```

## Territory Assignment Logic

### Find Franchisee by Address
```csharp
public async Task<long?> FindFranchiseeByAddress(AddressViewModel address)
{
    // 1. Geocode address if not already geocoded
    if (!address.Latitude.HasValue || !address.Longitude.HasValue)
    {
        var geocoded = await _geocodingService.GeocodeAddress(address);
        address.Latitude = geocoded.Latitude;
        address.Longitude = geocoded.Longitude;
    }
    
    // 2. Check polygon territories first (most specific)
    var polygonTerritory = await FindPolygonTerritory(address.Latitude.Value, address.Longitude.Value);
    if (polygonTerritory != null)
        return polygonTerritory.FranchiseeId;
    
    // 3. Check zip code territories
    var zipTerritory = await FindZipCodeTerritory(address.ZipCode);
    if (zipTerritory != null)
        return zipTerritory.FranchiseeId;
    
    // 4. Check city territories
    var cityTerritory = await FindCityTerritory(address.City, address.State);
    if (cityTerritory != null)
        return cityTerritory.FranchiseeId;
    
    // No territory found
    return null;
}
```

### Point-in-Polygon Algorithm
```csharp
public bool IsPointInPolygon(GeoPoint point, List<GeoPoint> polygon)
{
    bool inside = false;
    int j = polygon.Count - 1;
    
    for (int i = 0; i < polygon.Count; i++)
    {
        if ((polygon[i].Longitude > point.Longitude) != (polygon[j].Longitude > point.Longitude) &&
            point.Latitude < (polygon[j].Latitude - polygon[i].Latitude) * 
                            (point.Longitude - polygon[i].Longitude) / 
                            (polygon[j].Longitude - polygon[i].Longitude) + 
                            polygon[i].Latitude)
        {
            inside = !inside;
        }
        j = i;
    }
    
    return inside;
}
```

## Distance Calculations

### Haversine Formula
```csharp
public double CalculateDistance(GeoPoint point1, GeoPoint point2)
{
    const double R = 3959; // Earth's radius in miles
    
    var lat1Rad = DegreesToRadians(point1.Latitude);
    var lat2Rad = DegreesToRadians(point2.Latitude);
    var deltaLat = DegreesToRadians(point2.Latitude - point1.Latitude);
    var deltaLon = DegreesToRadians(point2.Longitude - point1.Longitude);
    
    var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
            Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
            Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
    
    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    
    return R * c; // Distance in miles
}
```

## Geocoding Integration

### Google Maps API Integration
```csharp
public async Task<GeocodeResponse> GeocodeAddress(GeocodeRequest request)
{
    var address = $"{request.Address}, {request.City}, {request.State} {request.ZipCode}";
    
    var httpClient = new HttpClient();
    var apiKey = _settings.GoogleMapsApiKey;
    var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";
    
    var response = await httpClient.GetAsync(url);
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonConvert.DeserializeObject<GoogleGeocodeResponse>(content);
    
    if (result.Status == "OK" && result.Results.Length > 0)
    {
        var location = result.Results[0].Geometry.Location;
        
        return new GeocodeResponse
        {
            Success = true,
            Latitude = location.Lat,
            Longitude = location.Lng,
            FormattedAddress = result.Results[0].FormattedAddress,
            AddressComponents = result.Results[0].AddressComponents
        };
    }
    
    return new GeocodeResponse { Success = false };
}
```

## Business Rules

- Franchisee territories cannot overlap (enforced at save)
- Each zip code can only belong to one franchisee
- Polygon territories take precedence over zip code territories
- Territory changes require approval (audit trail)
- Customers automatically assigned to franchisee based on service address
- Leads assigned to franchisee based on location
- Service area eligibility checked before booking appointments

## Authorization

- **Super Admin**: Manage all territories
- **Franchisee Admin**: View their territory, cannot modify
- **Public**: Check service availability by address (no auth required)

## Territory Overlap Detection

```csharp
public async Task<List<TerritoryOverlap>> DetectOverlaps()
{
    var overlaps = new List<TerritoryOverlap>();
    var territories = await _territoryService.GetAll();
    
    for (int i = 0; i < territories.Count; i++)
    {
        for (int j = i + 1; j < territories.Count; j++)
        {
            if (TerritoriesOverlap(territories[i], territories[j]))
            {
                overlaps.Add(new TerritoryOverlap
                {
                    Territory1 = territories[i],
                    Territory2 = territories[j],
                    OverlapType = DetermineOverlapType(territories[i], territories[j])
                });
            }
        }
    }
    
    return overlaps;
}

private bool TerritoriesOverlap(TerritoryViewModel t1, TerritoryViewModel t2)
{
    // Check zip code overlap
    if (t1.Type == TerritoryType.ZipCode && t2.Type == TerritoryType.ZipCode)
    {
        return t1.ZipCodes.Intersect(t2.ZipCodes).Any();
    }
    
    // Check polygon overlap (more complex)
    if (t1.Type == TerritoryType.Polygon && t2.Type == TerritoryType.Polygon)
    {
        return PolygonsOverlap(t1.BoundaryPoints, t2.BoundaryPoints);
    }
    
    // Mixed types - check common areas
    // ... additional logic
    
    return false;
}
```

## Testing

```csharp
[Test]
public void FindFranchiseeByZipCode_ValidZip_ReturnsFranchisee()
{
    var address = new AddressViewModel
    {
        ZipCode = "12345",
        City = "New York",
        State = "NY"
    };
    
    var franchiseeId = _geoService.FindFranchiseeByAddress(address).Result;
    
    Assert.IsNotNull(franchiseeId);
    Assert.AreEqual(1, franchiseeId.Value);
}

[Test]
public void CalculateDistance_TwoPoints_ReturnsCorrectDistance()
{
    var point1 = new GeoPoint { Latitude = 40.7128, Longitude = -74.0060 }; // NYC
    var point2 = new GeoPoint { Latitude = 34.0522, Longitude = -118.2437 }; // LA
    
    var distance = _geoService.CalculateDistance(point1, point2);
    
    Assert.IsTrue(distance > 2400 && distance < 2500); // ~2445 miles
}
```

## Integration Points

- **Organizations**: Franchisee territory assignments
- **MarketingLead**: Lead assignment by location
- **Sales**: Customer address validation
- **Scheduler**: Job assignment by location
- **Dashboard**: Geographic performance visualization

## Performance Optimization

- Cache territory data (changes infrequently)
- Index zip codes and city names for fast lookups
- Use spatial indexes for polygon queries
- Pre-calculate territory boundaries
- Batch geocoding for bulk operations

## Future Enhancements

- Real-time technician GPS tracking
- Route optimization for multiple stops
- Traffic-aware routing
- Service area heat maps
- Demographic data integration
- Mobile app with live maps
