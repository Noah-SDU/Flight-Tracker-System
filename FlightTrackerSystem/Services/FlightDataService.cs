using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.Services;

public class FlightDataService
{
    private const string DataFileName = "flights.json";
    private const string AssetRelativePath = "Assets/flights.json";
    private const string AvaloniaAssetUri = "avares://FlightTrackerSystem/Assets/flights.json";

    public async Task<FlightData> LoadFlightDataAsync()
    {
        try
        {
            var json = await ReadFlightDataJsonAsync();

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

    private static async Task<string> ReadFlightDataJsonAsync()
    {
        // Preferred source in Avalonia apps is the packaged asset.
        try
        {
            using var stream = AssetLoader.Open(new Uri(AvaloniaAssetUri));
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        catch
        {
            // Fallback for test/console contexts where assets are not mounted.
        }

        var candidatePaths = new List<string>
        {
            DataFileName,
            AssetRelativePath,
            Path.Combine(AppContext.BaseDirectory, DataFileName),
            Path.Combine(AppContext.BaseDirectory, AssetRelativePath)
        };

        var existingPath = candidatePaths.FirstOrDefault(File.Exists);
        if (existingPath == null)
            throw new FileNotFoundException($"Data file not found. Tried: {string.Join(", ", candidatePaths)}");

        return await File.ReadAllTextAsync(existingPath);
    }
}