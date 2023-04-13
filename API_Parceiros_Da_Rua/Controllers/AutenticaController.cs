using API_Parceiros_Da_Rua.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace API_Parceiros_Da_Rua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticaController : ControllerBase
    {
        private readonly string connectionString;

        public AutenticaController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("MySqlConnectionString");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                        command.Parameters.AddWithValue("@Email", model.Email);

                        int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        if (count == 0)
                        {
                            return BadRequest("E-mail não encontrado.");
                        }

                        command.Parameters.Clear();
                        command.CommandText = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND Password = @Password";
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Password", model.Password);

                        count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        if (count == 0)
                        {
                            return BadRequest("Senha incorreta.");
                        }

                        return Ok();
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}

