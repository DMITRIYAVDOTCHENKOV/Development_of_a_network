
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace seminar5
{
    internal class ChatContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=Localhost; Database=myDataBase; Trusted_Connection=True").UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entite =>
            {
                entite.HasKey(x => x.Id).HasName("user_pkey");

                entite.ToTable("users");

                entite.HasIndex(x => x.FullName).IsUnique();

                entite.Property(e => e.FullName)
                .HasColumnName("FullName")
                .HasMaxLength(255)
                .IsRequired();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.MessageId).HasName("message_pk");
                entity.ToTable("message");

                entity.Property(e => e.Text).HasColumnName("message_text");
                entity.Property(e => e.DateSend).HasColumnName("message_date");
                entity.Property(e => e.IsSent).HasColumnName("is_isent");
                entity.Property(e => e.MessageId).HasColumnName("id");

                entity.HasOne(x => x.UserTo).WithMany(m => m.MessagesTo).HasForeignKey(x => x.UserToId).HasConstraintName("message_To_User_FK ");

                entity.HasOne(x => x.UserFrom).WithMany(m => m.MessagesFrom).HasForeignKey(x => x.UserFromId).HasConstraintName("message_From_User_FK ");




            });
        }

        public ChatContext()
        {

        }
        public ChatContext(DbContextOptions dbc) : base(dbc)
        {
            
        }
    }
}
