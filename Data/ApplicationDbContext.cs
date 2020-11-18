using Microsoft.EntityFrameworkCore;
using TweetAPI.Domain;

namespace TweetAPI.Data
{
    public class ApplicationDataContext : DbContext
    {
        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
    }
}
