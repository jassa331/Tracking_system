using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
//jassa dhammi
    public DbSet<PatientAddress> PatientAddresses { get; set; }
    public DbSet<ApiLog>ApiLogs { get; set; }

}
