namespace Messenger.ChatDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text.RegularExpressions;

    [Table("ChatUser")]
    [DataContract]
    //[KnownType(typeof(ChatUser))]
    //[KnownType(typeof(Message))]
    public partial class ChatUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChatUser()
        {
            Message = new HashSet<Message>();
        }

        //[DataMember]
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        //[IgnoreDataMember]
        public string Login { get; set; }

        [Required]
        [StringLength(254)]
        //[IgnoreDataMember]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        //[IgnoreDataMember]
        public string Password { get; set; }

        //[IgnoreDataMember]
        public byte[] Photo { get; set; }

        [StringLength(2000)]
        //[IgnoreDataMember]
        public string Bio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //[IgnoreDataMember]
        public virtual ICollection<Message> Message { get; set; }

        #region Валидация через методы класса ChatUser
        // валидация всех полей
        static public string LoginValidation(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "Поле \"Логин\" не может быть пустым";
            else if (value.Length > 30)
                return "Поле \"Логин\" не может содержать более 30 символов";

            return null;
        }

        static public string EmailValidation(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "Поле \"Электронная почта\" не может быть пустым";

            string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";

            //Для проверки соответствия строки шаблону используется метод IsMatch: 
            //Regex.IsMatch(value, pattern, RegexOptions.Case).
            //Последний параметр указывает, что регистр можно игнорировать.
            //И если строка соответствует шаблону, то метод возвращает true.
            Match isMatch = Regex.Match(value, pattern, RegexOptions.IgnoreCase);

            if (!isMatch.Success)
                return "Неккоректное значение электронной почты. " +
                    "Адрес должен содержать две части разделенные знаком\"@\", " +
                    "первая часть - имя почтового ящика, вторая - доменное имя того сервера, на котором расположен почтовый ящик." +
                    "Например: myEmail@mail.ru";

            return null;
        }

        static public string PasswordValidation(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "Поле \"Пароль\" не может быть пустым";
            else if (value.Length > 50)
                return "Поле \"Пароль\" не может содержать более 50 символов";
            else if (value.Length < 6)
                return "Поле \"Пароль\" не может содержать менее 6 символов";

            return null;
        }
        #endregion

        // wcf
        //[NotMapped]     // не попадет в модель БД
        //[IgnoreDataMember]
        //public OperationContext OperationContext { get; set; }
    }
}
