namespace Messenger.ChatDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Message")]
    public partial class Message
    {
        public long Id { get; set; }

        [Required]
        [StringLength(3000)]
        public string Content { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime CreationDate { get; set; }

        public long SenderUserId { get; set; }

        public virtual ChatUser ChatUser { get; set; }
    }
}
