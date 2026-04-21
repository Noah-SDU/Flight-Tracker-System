using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.ViewModels;

public partial class AnalyticsViewModel : ViewModelBase
{
    public sealed class AnalyticsMetric
    {
        public required string Label { get; init; }
        public int Count { get; init; }
    }

    [ObservableProperty]
    private FlightData? flightData;

    [ObservableProperty] private ObservableCollection<AnalyticsMetric> busiestRoutesByTimeOfDay = new();

    [ObservableProperty] private ObservableCollection<AnalyticsMetric> topAirlinesByTraffic = new();

    [ObservableProperty] private ObservableCollection<AnalyticsMetric> countryTrafficTrends = new();

    [ObservableProperty] private int busiestRoutesMax = 1;

    [ObservableProperty] private int topAirlinesMax = 1;

    [ObservableProperty] private int countryTrafficMax = 1;

    public void Initialize(FlightData? flightData)
    {
        FlightData = flightData;
        GenerateAnalytics();
    }

    private void GenerateAnalytics()
    {
        if (FlightData?.Flights == null || FlightData.Flights.Count == 0)
        {
            BusiestRoutesByTimeOfDay = new ObservableCollection<AnalyticsMetric>();
            TopAirlinesByTraffic = new ObservableCollection<AnalyticsMetric>();
            CountryTrafficTrends = new ObservableCollection<AnalyticsMetric>();
            BusiestRoutesMax = 1;
            TopAirlinesMax = 1;
            CountryTrafficMax = 1;
            return;
        }

        var flights = FlightData.Flights;

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

        var airportCountryLookup = (FlightData.Airports ?? new())
            .GroupBy(a => a.IataCode)
            .ToDictionary(
                g => g.Key,
                g => string.IsNullOrWhiteSpace(g.First().Country) ? "Unknown country" : g.First().Country);

        string ResolveDepartureCountry(string departureAirport)
        {
            return airportCountryLookup.TryGetValue(departureAirport, out var country)
                ? country
                : "Unknown country";
        }

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

        BusiestRoutesByTimeOfDay = new ObservableCollection<AnalyticsMetric>(routeTimeData);
        TopAirlinesByTraffic = new ObservableCollection<AnalyticsMetric>(airlineData);
        CountryTrafficTrends = new ObservableCollection<AnalyticsMetric>(countryTrendData);

        BusiestRoutesMax = routeTimeData.Count == 0 ? 1 : routeTimeData.Max(x => x.Count);
        TopAirlinesMax = airlineData.Count == 0 ? 1 : airlineData.Max(x => x.Count);
        CountryTrafficMax = countryTrendData.Count == 0 ? 1 : countryTrendData.Max(x => x.Count);
    }

    private static string ToTimeOfDayBucket(int hour)
    {
        return hour switch
        {
            >= 5 and < 12 => "Morning",
            >= 12 and < 17 => "Afternoon",
            >= 17 and < 22 => "Evening",
            _ => "Night"
        };
    }
}