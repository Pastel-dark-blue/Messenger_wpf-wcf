namespace Messenger.ChatDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Chat")]
    public partial class Chat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Chat()
        {
            Party = new HashSet<Party>();
        }

        public long Id { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

        public long? Admin { get; set; }

        public virtual ChatUser ChatUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Party> Party { get; set; }
    }
}
