using Microsoft.EntityFrameworkCore;
namespace BeltExam3.Models
{
	public class UsersContext : DbContext
	{
		public UsersContext(DbContextOptions<UsersContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
	}
}