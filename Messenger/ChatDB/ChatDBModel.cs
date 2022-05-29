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

        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<ChatUser> ChatUser { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Party> Party { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Chat>()
                .HasMany(e => e.Party)
                .WithOptional(e => e.Chat1)
                .HasForeignKey(e => e.Chat);

            modelBuilder.Entity<ChatUser>()
                .HasMany(e => e.Chat)
                .WithOptional(e => e.ChatUser)
                .HasForeignKey(e => e.Admin);

            modelBuilder.Entity<ChatUser>()
                .HasMany(e => e.Message)
                .WithOptional(e => e.ChatUser)
                .HasForeignKey(e => e.SenderUserId);

            modelBuilder.Entity<ChatUser>()
                .HasMany(e => e.Party)
                .WithOptional(e => e.ChatUser1)
                .HasForeignKey(e => e.ChatUser);
        }
    }
}
