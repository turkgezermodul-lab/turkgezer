using System;
using UnityEngine;

namespace TurkGezer.Api
{
    [Serializable]
    public sealed class IssPosition
    {
        public double latitude;
        public double longitude;
        public double altitude;
        public double velocity;
        public long timestamp;
        public bool hasAltitudeAndVelocity;
    }

    public static class IssPositionParser
    {
        [Serializable] private sealed class Wtia
        {
            public string name;
            public int id;
            public double latitude;
            public double longitude;
            public double altitude;
            public double velocity;
            public long timestamp;
        }
        [Serializable] private sealed class Notify
        {
            public string message;
            public long timestamp;
            public Coordinates iss_position;
        }
        [Serializable] private sealed class Coordinates { public string latitude; public string longitude; }

        public static bool TryParse(string json, IssProvider provider, out IssPosition position)
        {
            position = null;
            if (string.IsNullOrWhiteSpace(json) || !json.Contains("\"latitude\"") || !json.Contains("\"longitude\"")) return false;
            try
            {
                if (provider == IssProvider.WhereTheIssAt)
                {
                    var data = JsonUtility.FromJson<Wtia>(json);
                    if (data == null || data.id != 25544 || data.name != "iss") return false;
                    position = new IssPosition { latitude = data.latitude, longitude = data.longitude,
                        altitude = data.altitude, velocity = data.velocity, timestamp = data.timestamp,
                        hasAltitudeAndVelocity = true };
                }
                else
                {
                    var data = JsonUtility.FromJson<Notify>(json);
                    if (data == null || data.message != "success" || data.iss_position == null) return false;
                    var culture = System.Globalization.CultureInfo.InvariantCulture;
                    if (!double.TryParse(data.iss_position.latitude, System.Globalization.NumberStyles.Float, culture, out var lat) ||
                        !double.TryParse(data.iss_position.longitude, System.Globalization.NumberStyles.Float, culture, out var lon)) return false;
                    position = new IssPosition { latitude = lat, longitude = lon, timestamp = data.timestamp };
                }
                if (!IsValid(position)) { position = null; return false; }
                return true;
            }
            catch (ArgumentException) { position = null; return false; }
        }
        private static bool IsValid(IssPosition p) => p != null && p.timestamp > 0 && p.timestamp <= 253402300799 &&
            Finite(p.latitude) && Finite(p.longitude) && Math.Abs(p.latitude) <= 90 && Math.Abs(p.longitude) <= 180 &&
            (!p.hasAltitudeAndVelocity || (Finite(p.altitude) && Finite(p.velocity) && p.altitude > 0 && p.velocity >= 0));
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
