using System.Runtime.Serialization;

namespace src.Models
{
    public enum TypeOfTransaction
    {
        [EnumMember(Value = "c")]
        Credit,
        [EnumMember(Value = "d")]
        Debit
    }
}