using System.Net.Http.Json;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class IpLocationService
    {
        private readonly HttpClient _http;

        public IpLocationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> GetCityFromIp(string ip)
        {
            var result = await _http.GetFromJsonAsync<IpApiResponse>(
                $"http://ip-api.com/json/{ip}");

            return result?.City;
        }
        public async Task<IpApiResponse?> GetLocationAsync(string ip)
        {
            return await _http.GetFromJsonAsync<IpApiResponse>(
                $"http://ip-api.com/json/{ip}");
        }
        public async Task<string> GetPublicIpAsync()
        {
            return await _http.GetStringAsync("https://api.ipify.org");
        }

    }


}
