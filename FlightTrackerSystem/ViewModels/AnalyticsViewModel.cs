using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public ObservableCollection<AnalyticsMetric> FlightsByAirline => TopAirlinesByTraffic;

    [ObservableProperty] private ObservableCollection<AnalyticsMetric> countryTrafficTrends = new();

    [ObservableProperty] private int busiestRoutesMax = 1;

    [ObservableProperty] private int topAirlinesMax = 1;

    [ObservableProperty] private int countryTrafficMax = 1;

    [ObservableProperty] private string exportResultMessage = string.Empty;

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

    [RelayCommand]
    private void ExportAnalyticsToTextFile()
    {
        try
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = $"flight_analytics_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var fullPath = Path.Combine(documentsPath, fileName);

            var builder = new StringBuilder();
            builder.AppendLine("Flight Tracker Analytics Export");
            builder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine();

            AppendSection(builder, "Busiest Routes By Time Of Day", BusiestRoutesByTimeOfDay);
            AppendSection(builder, "Top Airlines By Traffic Volume", TopAirlinesByTraffic);
            AppendSection(builder, "Country-Level Traffic Trends", CountryTrafficTrends);

            File.WriteAllText(fullPath, builder.ToString());
            ExportResultMessage = $"Exported analytics to: {fullPath}";
        }
        catch (Exception ex)
        {
            ExportResultMessage = $"Export failed: {ex.Message}";
        }
    }

    private static void AppendSection(StringBuilder builder, string title, ObservableCollection<AnalyticsMetric> items)
    {
        builder.AppendLine(title);
        builder.AppendLine(new string('-', title.Length));

        if (items.Count == 0)
        {
            builder.AppendLine("No data available.");
            builder.AppendLine();
            return;
        }

        foreach (var item in items)
        {
            builder.AppendLine($"- {item.Label}: {item.Count}");
        }

        builder.AppendLine();
    }
}