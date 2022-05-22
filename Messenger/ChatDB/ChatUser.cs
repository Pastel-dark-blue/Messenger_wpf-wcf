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

        #region ��������� ����� ������ ������ ChatUser
        // ��������� ���� �����
        static public string LoginValidation(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "���� \"�����\" �� ����� ���� ������";
            else if (value.Length > 30)
                return "���� \"�����\" �� ����� ��������� ����� 30 ��������";

            return null;
        }

        static public string EmailValidation(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "���� \"����������� �����\" �� ����� ���� ������";

            string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";

            //��� �������� ������������ ������ ������� ������������ ����� IsMatch: 
            //Regex.IsMatch(value, pattern, RegexOptions.Case).
            //��������� �������� ���������, ��� ������� ����� ������������.
            //� ���� ������ ������������� �������, �� ����� ���������� true.
            Match isMatch = Regex.Match(value, pattern, RegexOptions.IgnoreCase);

            if (!isMatch.Success)
                return "������������ �������� ����������� �����. " +
                    "����� ������ ��������� ��� ����� ����������� ������\"@\", " +
                    "������ ����� - ��� ��������� �����, ������ - �������� ��� ���� �������, �� ������� ���������� �������� ����." +
                    "��������: myEmail@mail.ru";

            return null;
        }

        static public string PasswordValidation(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "���� \"������\" �� ����� ���� ������";
            else if (value.Length > 50)
                return "���� \"������\" �� ����� ��������� ����� 50 ��������";
            else if (value.Length < 6)
                return "���� \"������\" �� ����� ��������� ����� 6 ��������";

            return null;
        }
        #endregion

        // wcf
        //[NotMapped]     // �� ������� � ������ ��
        //[IgnoreDataMember]
        //public OperationContext OperationContext { get; set; }
    }
}
