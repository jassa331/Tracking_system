namespace WebApplication1.Models
{
    public class PatientAddress
    {
        public int Id { get; set; }

        public string HouseNo { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
