namespace WebApplication1.Models
{
    public class IpApiResponse
    {
        public string? Query { get; set; }
        public string? City { get; set; }
        public string? RegionName { get; set; }
        public string? Country { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string? Isp { get; set; }
    }
}
