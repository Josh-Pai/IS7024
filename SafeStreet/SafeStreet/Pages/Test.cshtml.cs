using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SafeStreet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SafeStreet.Pages
{
    public class TestModel : PageModel
    {
        private readonly ILogger<TestModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public string GoogleMapApiKey { get; private set; }
        public List<Crime> Crimes { get; set; } = new List<Crime>();

        public TestModel(ILogger<TestModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public void OnGet()
        {
            // Fetch Google Maps API key from configuration
            GoogleMapApiKey = _configuration["GoogleMapApiKey"];
        }

        [HttpGet]
        public async Task<IActionResult> OnGetCrimeStatsNearbyAsync(double latitude, double longitude)
        {
            try
            {
                DateTime oneYearAgo = DateTime.UtcNow.AddYears(-1);

                // Fetch crimes from the API if not already loaded
                if (Crimes == null || !Crimes.Any())
                {
                    Crimes = await FetchCrimesFromApi(oneYearAgo);
                }

                const double SEARCH_RADIUS_KM = 1.0; // 1 km radius
                DateTime now = DateTime.UtcNow;

                // Calculate nearby crimes within the radius
                var nearbyCrimes = Crimes.Where(c =>
                    double.TryParse(c.LatitudeX, out var lat) &&
                    double.TryParse(c.LongitudeX, out var lng) &&
                    CalculateDistance(latitude, longitude, lat, lng) <= SEARCH_RADIUS_KM
                );

                // Summarize crime statistics
                var stats = new
                {
                    Last1Month = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 30),
                    Last3Months = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 90),
                    Last6Months = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 180),
                    Last1Year = nearbyCrimes.Count(c => (now - c.DateReported).TotalDays <= 365)
                };

                _logger.LogInformation($"Stats: {System.Text.Json.JsonSerializer.Serialize(stats)}");
                return new JsonResult(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching or processing nearby crime stats: {ex.Message}");
                return BadRequest("Error processing request.");
            }
        }

        private async Task<List<Crime>> FetchCrimesFromApi(DateTime since)
        {
            var crimes = new List<Crime>();
            int limit = 1000;
            int offset = 0;
            bool moreData = true;

            while (moreData)
            {
                string url = $"https://data.cincinnati-oh.gov/resource/k59e-2pvf.json?$limit={limit}&$offset={offset}&$where=date_reported >= '{since:yyyy-MM-ddT00:00:00}'";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string crimeJson = await response.Content.ReadAsStringAsync();
                    var crimesBatch = Crime.FromJson(crimeJson);

                    if (crimesBatch != null && crimesBatch.Any())
                    {
                        crimes.AddRange(crimesBatch);
                        offset += limit;
                        _logger.LogInformation($"Fetched {crimesBatch.Count} records. Total so far: {crimes.Count}");
                    }
                    else
                    {
                        moreData = false; // No more data available
                    }
                }
                else
                {
                    _logger.LogError($"Failed to fetch crimes: {response.StatusCode}");
                    moreData = false;
                }
            }

            return crimes;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
