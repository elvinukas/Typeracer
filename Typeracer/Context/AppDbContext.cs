using Microsoft.EntityFrameworkCore;

namespace Typeracer.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    
    
   
}