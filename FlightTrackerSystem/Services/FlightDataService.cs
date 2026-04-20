using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.Services;

public class FlightDataService
{
    private const string DataFileName = "flights.json";

    public async Task<FlightData> LoadFlightDataAsync()
    {
        try
        {
            if (!File.Exists(DataFileName))
            {
                throw new FileNotFoundException($"Data file '{DataFileName}' not found.");
            }

            var json = await File.ReadAllTextAsync(DataFileName);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return JsonSerializer.Deserialize<FlightData>(json, options) ?? throw new InvalidOperationException("Failed to deserialize flight data.");
            

        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading flight data: {ex.Message}");
        }
    }
}