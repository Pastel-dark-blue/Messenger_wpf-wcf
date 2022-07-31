using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Messenger.ChatDB
{
    public partial class ChatDBModel : DbContext
    {
        public ChatDBModel()
            : base("name=ChatDBModel")
        {
        }

        public virtual DbSet<ChatUser> ChatUser { get; set; }
        public virtual DbSet<Message> Message { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatUser>()
                .Property(e => e.Photo)
                .IsUnicode(false);

            modelBuilder.Entity<ChatUser>()
                .HasMany(e => e.Message)
                .WithRequired(e => e.ChatUser)
                .HasForeignKey(e => e.SenderUserId)
                .WillCascadeOnDelete(false);
        }
    }
}
