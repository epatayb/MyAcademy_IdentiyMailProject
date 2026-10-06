using IdentiyMail.Web.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentiyMail.Web.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().HasMany(message => message.SentMessages)
                                        .WithOne(s => s.Sender).HasForeignKey
                                            (x => x.SenderId)
                                        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>().HasMany(message => message.ReceivedMessages)
                                        .WithOne(s => s.Receiver).HasForeignKey
                                            (x => x.ReceiverId)
                                        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>().HasMany(x => x.DraftMessages)
                                        .WithOne(x => x.Sender).HasForeignKey
                                            (x => x.SenderId)
                                        .OnDelete(DeleteBehavior.Restrict);

            #region Kategori ilişkileri
            builder.Entity<Category>().HasOne(x => x.User)
                                        .WithMany(x => x.Categories)
                                        .HasForeignKey(x => x.UserId)
                                        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>().HasIndex(x => new { x.UserId, x.Name })
                                        .IsUnique();

            builder.Entity<Category>().Property(x => x.Name)
                                        .HasMaxLength(50);

            builder.Entity<Category>().Property(x => x.Description)
                                        .HasMaxLength(250);

            builder.Entity<Category>().Property(x => x.ColorHex)
                                        .HasMaxLength(7);

            builder.Entity<MessageCategoryAssignment>()
                                        .HasOne(x => x.User)
                                        .WithMany(x => x.CategoryAssignments)
                                        .HasForeignKey(x => x.UserId)
                                        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageCategoryAssignment>()
                                        .HasOne(x => x.Message)
                                        .WithMany(x => x.CategoryAssignments)
                                        .HasForeignKey(x => x.MessageId)
                                        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MessageCategoryAssignment>()
                                        .HasOne(x => x.Category)
                                        .WithMany(x => x.Assignments)
                                        .HasForeignKey(x => x.CategoryId)
                                        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MessageCategoryAssignment>()
                                        .HasIndex(x => new { x.UserId, x.MessageId })
                                        .IsUnique();


            builder.Entity<DraftMessage>().HasOne(x => x.Category)
                                        .WithMany(x => x.DraftMessages)
                                        .HasForeignKey(x => x.CategoryId)
                                        .OnDelete(DeleteBehavior.SetNull);
            #endregion


            base.OnModelCreating(builder);
        }

        public DbSet<UserMessage> UserMessages { get; set; }

        public DbSet<DraftMessage> DraftMessages { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<MessageCategoryAssignment> MessageCategoryAssignments { get; set; }
    }
}
