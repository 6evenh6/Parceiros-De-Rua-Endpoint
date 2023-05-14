using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Parceiros_Da_Rua.Model
{
    public class UsersPostViewModel
    {
        // idusers, nome, email, gestao, datacadastro FROM users
        [JsonIgnore]
        public int idUsers { get; set; }
        public string nome { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? password { get; set; }
        public string gestao { get; set; } = string.Empty;

        public DateTime dataCadastro { get; set; } = DateTime.Now;
    }
}
