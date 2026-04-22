# Flight-Tracker-System
Home Assignment 3

## Overview
A Flight Tracker System developed for Sønderborg Airport to help air traffic controllers view flight routes and extract useful data while assisting airport administrators in managing flight operations.

## Project Description
This application provides real-time flight tracking, route visualization, and analytics for airport operations. It displays flight information from a JSON data file and visualizes meaningful insights through interactive charts and maps.

---

## Tech Stack
- **Framework:** .NET 10.0 with Avalonia UI
- **Architecture:** MVVM (Model-View-ViewModel)
- **Data Format:** JSON (flights.json)
- **Key Libraries:**
    - MVVM Toolkit - Data binding and observable properties
    - Mapsui - Map visualization
    - LiveCharts2 - Interactive charts
    - xUnit - Unit testing

---

## Project Structure

FlightTrackerSystem/
├── Models/
│   ├── Airport.cs              # Airport data model
│   ├── Flight.cs               # Flight data model
│   └── FlightData.cs           # Container for airports and flights
│
├── Services/
│   └── FlightDataService.cs    # JSON data loading service
│
├── ViewModels/
│   ├── MainWindowViewModel.cs  # Main window logic & tab navigation
│   ├── RouteViewModel.cs       # Route visualization & search logic
│   ├── AirportFlightInfoViewModel.cs  # Airport flight filtering
│   └── AnalyticsViewModel.cs   # Analytics data with LINQ queries
│
├── Views/
│   ├── MainWindow.axaml        # Main window with TabControl
│   ├── MainWindow.axaml.cs     # Code-behind
│   ├── RouteView.axaml         # Route search & map UI
│   ├── RouteView.axaml.cs      # Route code-behind
│   ├── AirportFlightInfoView.axaml  # Airport flights UI
│   ├── AirportFlightInfoView.axaml.cs     # Airport code-behind
│   ├── AnalyticsView.axaml     # Analytics charts UI
│   └── AnalyticsView.axaml.cs  # Analytics code-behind
│
├── Assets/                      # App icons and resources
├── flights.json                 # Flight data file
└── FlightTrackerSystem.csproj   # Project configuration

FlightTrackerSystem.Tests/       # Unit tests
├── ViewModels/
│   └── AnalyticsViewModelTests.cs
└── FlightTrackerSystem.Tests.csproj

---

## Features

### View 1: Route Visualization
- **Advanced Search:** Search flights by:
    - Flight number
    - Airline name or code
    - Departure or arrival airport (IATA code or full name)
    - Flight status
- **Interactive Map:** Displays departure and arrival airports on an interactive Mapsui map with OpenStreetMap tiles
- **Route Details:** Shows airport information for selected route
- **Clear Selection:** Reset search and map with a single command

### View 2: Airport Flight Information
- **Airport Selection:** Choose from a dropdown list of all airports
- **Flight Listing:** View all flights departing from the selected airport
- **Status Filtering:** Filter flights by status:
    - All (default)
    - On Time
    - Delayed
    - Cancelled
    - Custom statuses from data
- **Flight Details:** Display:
    - Flight Number
    - Airline Name & Code
    - Destination Airport
    - Scheduled Departure Time
    - Flight Status
    - Aircraft Type
- **Sorted Display:** Flights automatically sorted by departure time

### View 3: Analytics Dashboard
- **Busiest Routes by Time of Day:** Bar chart showing traffic for top routes, segmented by time buckets:
    - Morning (5:00 - 11:59)
    - Afternoon (12:00 - 16:59)
    - Evening (17:00 - 21:59)
    - Night (22:00 - 4:59)
- **Top Airlines by Traffic:** Bar chart ranking airlines by number of flights
- **Country Traffic Trends:** Monthly trend chart showing flight volume by departure country
- All data aggregated using LINQ queries on flight data

---

## Setup Instructions

### Prerequisites
- .NET 10.0 SDK or later
- Git

### Installation

1. **Clone the repository**
   bash
   git clone https://github.com/Noah-SDU/Flight-Tracker-System.git
   cd Flight-Tracker-System

2. **Restore dependencies**
   bash
   dotnet restore

3. **Build the project**
   bash
   dotnet build

4. **Run the application**
   bash
   dotnet run

---

## LINQ Queries Explained

### Query 1: Group Routes by Time of Day

```csharp
var routeTimeData = flights
.GroupBy(f => new
{
Route = $"{f.DepartureAirport} -> {f.ArrivalAirport}",
TimeBucket = ToTimeOfDayBucket(f.ScheduledDeparture.Hour)
})
.Select(g => new AnalyticsMetric
{
Label = $"{g.Key.Route} ({g.Key.TimeBucket})",
Count = g.Count()
})
.OrderByDescending(x => x.Count)
.ThenBy(x => x.Label)
.Take(8)
.ToList();
```

#### What it does:

- Groups flights by route (departure → arrival) AND time of day
- Counts how many flights use each route in each time period
- Sorts by flight count (highest first), then alphabetically
- Returns top 8 results to prevent chart overcrowding

#### Example output:

```bash
CPH -> ARN (Morning): 12 flights
CPH -> AAL (Afternoon): 10 flights
BLL -> CPH (Evening): 8 flights
```

---

### Query 2: Group Airlines by Traffic

```csharp
var airlineData = flights
.GroupBy(f => string.IsNullOrWhiteSpace(f.AirlineName) ? "Unknown airline" : f.AirlineName)
.Select(g => new AnalyticsMetric
{
Label = g.Key,
Count = g.Count()
})
.OrderByDescending(x => x.Count)
.ThenBy(x => x.Label)
.Take(8)
.ToList();
```

#### What it does:

- Groups flights by airline name
- Handles missing airline names by labeling them "Unknown airline"
- Counts flights per airline
- Sorts by count (descending), then alphabetically
- Returns top 8 airlines

#### Example Output:

```bash
SAS: 42 flights
Ryanair: 28 flights
Lufthansa: 15 flights
```

---

### Query 3: Group Countries by Monthly Traffic

```csharp
var countryTrendData = flights
.Select(f => new
{
Month = f.ScheduledDeparture.ToString("yyyy-MM"),
Country = ResolveDepartureCountry(f.DepartureAirport)
})
.GroupBy(x => new { x.Month, x.Country })
.Select(g => new AnalyticsMetric
{
Label = $"{g.Key.Month} | {g.Key.Country}",
Count = g.Count()
})
.OrderByDescending(x => x.Count)
.ThenBy(x => x.Label)
.Take(10)
.ToList();
```

#### What it does:

- Extracts month and departure country for each flight
- Groups by both month AND country
- Counts flights per month-country combination
- Looks up country from airport data
- Sorts by count (descending), then label
- Returns top 10 results

#### Example output:

2024-01 | Denmark: 125 flights
2024-01 | Sweden: 98 flights
2024-02 | Denmark: 142 flights

---

## Application Architecture

### MVVM Pattern

The application follows the Model-View-ViewModel (MVVM) pattern:
- Models (Flight, Airport, FlightData) - Data structures from JSON
- Views (XAML files) - UI presentation layer
- ViewModels - Business logic, LINQ queries, filtering, and data binding

### Data Flow

1. **Application Start**
    - App.xaml.cs initializes MainWindow
    - MainWindow creates MainWindowViewModel

2. **Data Loading**
    - FlightDataService.LoadFlightsAsync() reads flights.json
    - Creates FlightData object with airports and flights
    - MainWindowViewModel.Initialize() receives the data

3. **Initialize Child ViewModels**
    - MainWindowViewModel calls .Initialize(flightData) on:
        - RouteViewModel
        - AirportFlightInfoViewModel
        - AnalyticsViewModel

4. **View 1: Route Visualization (RouteViewModel)**
    - User types in search box → RouteSearchText property changes
    - OnRouteSearchTextChanged() triggers → ApplyRouteFilter()
    - LINQ filters flights by: flight number, airline, airport code/name, status
    - Results populate FilteredRouteFlights collection
    - User selects a flight → SelectedFlight property changes
    - UpdateSelectedRoute() looks up airport details from FlightData.Airports
    - Map displays departure and arrival airport info

5. **View 2: Airport Flight Information (AirportFlightInfoViewModel)**
    - User selects airport → SelectedAirport property changes
    - OnSelectedAirportChanged() triggers → LoadFlightsForAirport()
    - LINQ filters flights where DepartureAirport == SelectedAirport.IataCode
    - Results stored in _allFlightsForAirport (internal cache)
    - User changes status filter → SelectedStatus property changes
    - OnSelectedStatusChanged() triggers → ApplyFilter()
    - If status is "All": show all flights; else filter by status
    - Results populate DisplayedFlights collection
    - DataGrid binds to DisplayedFlights and displays flights

6. **View 3: Analytics (AnalyticsViewModel)**
    - GenerateAnalytics() runs LINQ queries on FlightData.Flights:
        - **Query 1:** Groups routes by (Departure → Arrival + Time of Day) → BusiestRoutesByTimeOfDay
        - **Query 2:** Groups by airline name → TopAirlinesByTraffic
        - **Query 3:** Groups by (Month + Country) → CountryTrafficTrends
    - Each result capped at 8-10 items to prevent overcrowding
    - BusiestRoutesMax, TopAirlinesMax, CountryTrafficMax set for chart scaling
    - LiveCharts2 charts bind to these collections and render automatically

7. **User Interaction Cycle**
    - User action (search, filter, select) → ViewModel property changes
    - Property change triggers partial void OnXxxChanged() method
    - Method calls filtering/querying logic
    - Results update observable collection
    - UI automatically re-renders via data binding

### Observable Collections & Data Binding

All properties are [ObservableProperty] from MVVM Toolkit:
- **RouteViewModel:** FilteredRouteFlights, SelectedFlight, DepartureAirportInfo, ArrivalAirportInfo
- **AirportFlightInfoViewModel:** AllAirports, SelectedAirport, DisplayedFlights, StatusOptions, SelectedStatus
- **AnalyticsViewModel:** BusiestRoutesByTimeOfDay, TopAirlinesByTraffic, CountryTrafficTrends, plus Max values for chart scaling

When a property changes, MVVM Toolkit automatically:
- Calls the partial void OnXxxChanged(Type value) method
- Raises PropertyChanged event
- UI updates automatically through bindings

---

## Running Tests

```bash
dotnet test
```

### Expected Outcome:

```bash
Test Run Successful.
Total tests: 7
Passed: 7
Failed: 0
```

---

## How to Use the Application

### Starting the App
1. Run dotnet run --project FlightTrackerSystem
2. The application loads flight data from flights.json automatically
3. Click tabs to navigate between the three views

### View 1: Route Visualization
1. Type in the search box to find flights by:
    - Flight number (e.g., "SK001")
    - Airline name (e.g., "SAS")
    - Airport code (e.g., "CPH")
    - Airport name (e.g., "Copenhagen")
2. Select a flight from the filtered list
3. The map displays the departure and arrival airports
4. Airport information is shown for the selected route
5. Click "Clear Selection" to reset the search

### View 2: Airport Flight Information
1. Select an airport from the dropdown (sorted by IATA code)
2. All departing flights are displayed in the table
3. Choose a status from the filter dropdown to show only flights with that status
4. Click "All" to see all flights again
5. Flights are sorted by departure time

### View 3: Analytics Dashboard
1. Three charts automatically display analytics:
    - **Left chart:** Routes by time of day
    - **Right chart:** Top airlines
    - **Bottom chart:** Monthly country traffic trends
2. Hover over charts to see exact values
3. Charts update automatically if flight data changes

---

## Technical Decisions

### Why MVVM?
- Separates business logic from UI layer
- Makes testing easier (can test ViewModels without UI)
- Enables data binding between Views and ViewModels
- Follows industry best practices for modern applications

### Why Avalonia?
- Cross-platform support (.NET Framework)
- XAML-based UI (similar to WPF)
- Good support for data binding and reactive properties
- Lightweight and performant

### Why LINQ for Analytics?
- Declarative query syntax (readable and maintainable)
- Efficient data filtering, grouping, and aggregation
- Built-in to .NET
- Easy to modify queries without changing data structures

### Data Storage
- JSON format chosen for simplicity and portability
- Easy to update and modify without a database
- Can be replaced with API or database later without changing ViewModel logic
- Loaded asynchronously to prevent UI freezing

---

## Future Enhancements

- Real-time flight data from OpenSky Network API
- Persistent user preferences (saved between sessions)
- Advanced filters (by aircraft type, date range)
- Dynamic chart management (add/remove charts)
- Navigation history (back/forward buttons)
- Flight status notifications
- Data export to CSV/PDF
- Dark/light theme toggle

---

## Contributors
- **Person A:** Frontend and QA
- **Person B:** Backend and Unit Tests
- **Person C:** Prototype and Bonus Exercise

---

## License
This project is part of an university assignment

---

## Contact & Support
For issues or questions, please contact the development team.

Last Updated: April 2026