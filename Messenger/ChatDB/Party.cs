namespace Messenger.ChatDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Party")]
    public partial class Party
    {
        public long Id { get; set; }

        public long? ChatUser { get; set; }

        public long? Chat { get; set; }

        public virtual Chat Chat1 { get; set; }

        public virtual ChatUser ChatUser1 { get; set; }
    }
}
