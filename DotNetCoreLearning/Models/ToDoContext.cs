using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreLearning.Models
{
    public class ToDoContext : IdentityDbContext<IdentityUser>
    {
        public ToDoContext(DbContextOptions<ToDoContext> options)
            : base(options)
        {
        }

        public DbSet<ToDo> TodoItems { get; set; } = null!;
        //public DbSet<User> Users { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(local)\\sqlexpress;Database=DotNetCoreDB;Trusted_Connection=True;Encrypt=False");
        //}
    }
}
