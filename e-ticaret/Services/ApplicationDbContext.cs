using e_ticaret.Models;
using Microsoft.EntityFrameworkCore;

namespace e_ticaret.Services
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Product> Products {  get; set; }   
    } 
}
