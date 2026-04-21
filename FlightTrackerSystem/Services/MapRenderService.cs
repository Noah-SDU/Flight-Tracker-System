using System;
using System.Collections.Generic;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.Services;

/// <summary>
/// Service responsible for rendering flight routes on a Mapsui map.
/// Encapsulates map feature creation and styling logic.
/// </summary>
public class MapRenderService
{
    /// <summary>
    /// Creates route features (departure point, arrival point, and connecting line) for the given flight.
    /// </summary>
    public IFeature[] CreateRouteFeatures(Airport? departure, Airport? arrival)
    {
        if (departure == null || arrival == null)
            return Array.Empty<IFeature>();

        var departureCoords = SphericalMercator.FromLonLat(departure.Longitude, departure.Latitude);
        var arrivalCoords = SphericalMercator.FromLonLat(arrival.Longitude, arrival.Latitude);

        var departurePoint = CreateDepartureMarker(departureCoords);
        var arrivalPoint = CreateArrivalMarker(arrivalCoords);
        var routeLine = CreateRouteLine(departureCoords, arrivalCoords);

        return new IFeature[] { departurePoint, arrivalPoint, routeLine };
    }

    /// <summary>
    /// Calculates the bounding box that encompasses both airports with padding.
    /// </summary>
    public MRect CalculateBoundingBox(Airport? departure, Airport? arrival, double padding = 200000)
    {
        if (departure == null || arrival == null)
            return new MRect(0, 0, 1, 1);

        var departureCoords = SphericalMercator.FromLonLat(departure.Longitude, departure.Latitude);
        var arrivalCoords = SphericalMercator.FromLonLat(arrival.Longitude, arrival.Latitude);

        var minX = Math.Min(departureCoords.x, arrivalCoords.x);
        var minY = Math.Min(departureCoords.y, arrivalCoords.y);
        var maxX = Math.Max(departureCoords.x, arrivalCoords.x);
        var maxY = Math.Max(departureCoords.y, arrivalCoords.y);

        return new MRect(minX - padding, minY - padding, maxX + padding, maxY + padding);
    }

    private GeometryFeature CreateDepartureMarker((double x, double y) coordinates)
    {
        return new GeometryFeature
        {
            Geometry = new Point(coordinates.x, coordinates.y),
            Styles = new List<IStyle>
            {
                new SymbolStyle
                {
                    SymbolScale = 0.75,
                    Fill = new Brush(Color.Red)
                }
            }
        };
    }

    private GeometryFeature CreateArrivalMarker((double x, double y) coordinates)
    {
        return new GeometryFeature
        {
            Geometry = new Point(coordinates.x, coordinates.y),
            Styles = new List<IStyle>
            {
                new SymbolStyle
                {
                    SymbolScale = 0.75,
                    Fill = new Brush(Color.Green)
                }
            }
        };
    }

    private GeometryFeature CreateRouteLine((double x, double y) departure, (double x, double y) arrival)
    {
        return new GeometryFeature
        {
            Geometry = new LineString(new[]
            {
                new Coordinate(departure.x, departure.y),
                new Coordinate(arrival.x, arrival.y)
            }),
            Styles = new List<IStyle>
            {
                new VectorStyle
                {
                    Line = new Pen(Color.DodgerBlue, 3)
                }
            }
        };
    }
}
