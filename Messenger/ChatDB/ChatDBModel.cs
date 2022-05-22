using System.Data.Entity;

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
                .HasMany(e => e.Message)
                .WithOptional(e => e.ChatUser)
                .HasForeignKey(e => e.SenderUserId);
        }
    }
}
