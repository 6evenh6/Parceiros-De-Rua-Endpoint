using API_Parceiros_Da_Rua.Controllers;
using System.ComponentModel.DataAnnotations;

namespace API_Parceiros_Da_Rua.Model
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


    }
}
