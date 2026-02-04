# Core/Geo - AI Context

## Purpose

The **Geo** module manages geographic data, address validation, geocoding, territory management, and routing for the MarbleLife platform.

## Key Entities (Domain/)

### Geographic Data
- **Address**: Standardized address entity
- **Coordinate**: Latitude/longitude pairs
- **Territory**: Service area definitions
- **ZipCode**: ZIP code boundaries and metadata
- **ServiceArea**: Franchisee service territories

### Mapping
- **Route**: Optimized routes between locations
- **Distance**: Distance calculations and caching
- **GeoFence**: Geographic boundaries
- **Location**: General location entity

## Service Interfaces

### Core Geo Services
- **IAddressService**: Address validation and standardization
- **IGeocodingService**: Address to coordinates conversion
- **IReverseGeocodingService**: Coordinates to address
- **IDistanceService**: Distance calculations
- **IRoutingService**: Route optimization

### Territory Services
- **ITerritoryService**: Territory management
- **IServiceAreaService**: Service area assignment
- **IZipCodeService**: ZIP code lookups
- **IGeoFenceService**: Boundary checking

## Implementations (Impl/)

Business logic for:
- Google Maps API integration
- Address standardization (USPS)
- Distance matrix calculations
- Territory boundary checking
- Route optimization algorithms

## Enumerations (Enum/)

- **AddressType**: Residential, Commercial, PO Box
- **DistanceUnit**: Miles, Kilometers
- **TerritoryType**: Exclusive, Shared, Overflow

## ViewModels (ViewModel/)

- **AddressViewModel**: Address data
- **CoordinateViewModel**: GPS coordinates
- **RouteViewModel**: Route with waypoints
- **TerritoryViewModel**: Territory configuration

## Business Rules

1. **Address Validation**: Validate against USPS database
2. **Territory Assignment**: Based on ZIP code or coordinates
3. **Exclusive Territories**: No franchisee overlap
4. **Route Optimization**: Minimize drive time and distance
5. **Geocoding Cache**: Cache coordinates to reduce API calls

## Dependencies

- **Infrastructure**: Google Maps API, USPS API
- **Core/Organizations**: Franchisee territory assignment
- **Core/Scheduler**: Route planning for technicians

## For AI Agents

### Address Validation
```csharp
// Validate and standardize address
var validated = await _addressService.Validate(new AddressViewModel
{
    Street = "123 main st",
    City = "new york",
    State = "ny",
    ZipCode = "10001"
});

// Returns standardized: "123 Main St, New York, NY 10001-1234"
```

### Geocoding
```csharp
// Convert address to coordinates
var coordinates = await _geocodingService.Geocode("123 Main St, New York, NY 10001");

// Reverse geocoding
var address = await _reverseGeocodingService.ReverseGeocode(40.7128, -74.0060);
```

### Territory Assignment
```csharp
// Find franchisee by address
var franchisee = _serviceAreaService.FindByAddress(address);

// Check if address in territory
var inTerritory = _territoryService.IsInTerritory(franchiseeId, coordinates);
```

### Route Optimization
```csharp
// Optimize route for multiple stops
var optimizedRoute = await _routingService.OptimizeRoute(new RouteViewModel
{
    StartLocation = depotAddress,
    Stops = jobAddresses,
    EndLocation = depotAddress,
    OptimizeFor = OptimizeFor.Time
});

// Returns: ordered stops, distances, estimated times
```

## For Human Developers

### Best Practices
- Cache geocoding results to reduce API costs
- Use batch geocoding when possible
- Implement fallback for geocoding failures
- Validate addresses before geocoding
- Use territory boundaries for load balancing
- Monitor API usage and costs
- Implement rate limiting for external APIs
