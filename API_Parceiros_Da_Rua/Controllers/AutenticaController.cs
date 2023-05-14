using API_Parceiros_Da_Rua.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace API_Parceiros_Da_Rua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParceirosController : ControllerBase
    {
        private readonly string connectionString;

        public ParceirosController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("MySqlConnectionString");
        }

        //Post que autentica se o usuario está logado ou não
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

        //Get de todos os usuarios da tabela de usuario
        [HttpGet("Users")]
        public async Task<IEnumerable<UsersViewModel>> UsersAsync()
        {
            var users = new List<UsersViewModel>();
            var queryString = "SELECT idusers, nome, email, gestao, datacadastro FROM heroku_ab73d035cef131f.users;";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryString;
                        
                        using(var reader = await command.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string nome = reader.GetString(1);
                                string email = reader.GetString(2);
                                string gestao = reader.GetString(3).ToString();
                                DateTime data = reader.GetDateTime(4).ToLocalTime();
                                UsersViewModel user = new UsersViewModel { idUsersGet = id, nome = nome, email = email, gestao = gestao, dataCadastro = data };
                                users.Add(user);
                                
                            }
                        }
                    }
                }

                return users;
            
            
        }

        //Get de todos os moradores parceiros cadastrados
        [HttpGet("Parceiros")]
        public async Task <IEnumerable<ParceirosViewModel>> GetParceirosAsync()
        {
            var parceiros = new List<ParceirosViewModel>();
            var queryString = "SELECT idparceiros, parceirosnome, parceirosidade, parceirossexo, parceiroscpf, parceirosdatenow FROM heroku_ab73d035cef131f.parceiros;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = queryString;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string nome = reader.GetString(1);
                            int idade = reader.GetInt32(2);
                            string sexo = reader.GetString(3).ToString();
                            double cpf = reader.GetDouble(4);
                            DateTime data = reader.GetDateTime(5);

                            ParceirosViewModel parceiro = new ParceirosViewModel { idparceiros=id,parceirosnome=nome,parceirosidade=idade,parceirossexo=sexo,parceiroscpf=cpf,parceirosdatenow=data};
                            parceiros.Add(parceiro);

                        }
                    }
                }
            }

            return parceiros;
        }
        //Get para o grafico no dashboard
        [HttpGet("Dashboard")]
        public async Task<IEnumerable<DashBoardViewModel>> DashBoardsAsync()
        {
            var recursos = new List<DashBoardViewModel>();
            var queryString = "SELECT (SELECT COUNT(*) FROM alimentos) AS alimentos,(SELECT COUNT(*) FROM vestuarios) AS vestuarios,(SELECT COUNT(*) FROM higiene) AS higiene;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = queryString;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int alimento = reader.GetInt32(0);
                            int vestuario = reader.GetInt32(1);
                            int higiene = reader.GetInt32(2);
                            

                            DashBoardViewModel recurso = new DashBoardViewModel { alimento = alimento, vestuario = vestuario, higiene = higiene };
                                recursos.Add(recurso);

                        }
                    }
                }
            }

            return recursos;

        }

        //Get para os alimentos
        [HttpGet("Alimentos")]
        public async Task <IEnumerable<AlimentosViewModel>> AlimentosAsync()
        {

            var alimentos = new List<AlimentosViewModel>();
            var queryString = "SELECT * FROM heroku_ab73d035cef131f.alimentos;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = queryString;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string nome = reader.GetString(1);
                            string category = reader.GetString(2);
                            string origem = reader.GetString(3).ToString();
                            int qntd = reader.GetInt32(4);

                            AlimentosViewModel alimento = new AlimentosViewModel { idalimentos = id, alimentosnome = nome, alimentoscategory = category, alimentosorigem = origem, alimentosqntd = qntd };
                            alimentos.Add(alimento);

                        }
                    }
                }
            }

            return alimentos;

        }
        //Get para os vestuarios
        [HttpGet("Vestuarios")]
        public async Task<IEnumerable<VestuariosViewModel>> VestuariosAsync()
        {

            var vestuarios = new List<VestuariosViewModel>();
            var queryString = "SELECT * FROM heroku_ab73d035cef131f.vestuarios;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = queryString;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string nome = reader.GetString(1);
                            string category = reader.GetString(2);
                            string origem = reader.GetString(3).ToString();
                            int qntd = reader.GetInt32(4);

                            VestuariosViewModel vestuario = new VestuariosViewModel { idvestuarios = id, vestuariosnome = nome, vestuarioscategory = category, vestuariosorigem = origem, vestuariosqntd = qntd };
                            vestuarios.Add(vestuario);

                        }
                    }
                }
            }

            return vestuarios;

        }
        //Get para os higiene
        [HttpGet("Higiene")]
        public async Task<IEnumerable<HigieneViewModel>> HigieneAsync()
        {

            var higiene = new List<HigieneViewModel>();
            var queryString = "SELECT * FROM heroku_ab73d035cef131f.higiene;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = queryString;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string nome = reader.GetString(1);
                            string category = reader.GetString(2);
                            string origem = reader.GetString(3).ToString();
                            int qntd = reader.GetInt32(4);

                            HigieneViewModel higi = new HigieneViewModel { idhigiene = id, higienenome = nome, higienecategory = category, higieneorigem = origem, higieneqntd = qntd };
                            higiene.Add(higi);

                        }
                    }
                }
            }

            return higiene;

        }


        //Post para cadastrar usuario
        [HttpPost("AddUser")]
        public async Task<IActionResult> PostUsuarioAsync([FromBody] UsersPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryString = "INSERT INTO users (nome, Email, Password, gestao, datacadastro)" +
                            " VALUES ('" + model.nome + "', " + model.email + ",'" + model.password + "','" + model.gestao + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');";
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {
                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro no cadastro do usuario.");
                        }

                        return Ok("Registro incluido!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        //Post para cadastrar os moradores
        [HttpPost("AddParceiro")]
        public async Task <IActionResult> PostParceirosAsync(ParceirosViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryString = "INSERT INTO parceiros (parceirosnome, parceirosidade, parceirossexo, parceiroscpf, parceirosdatenow)" +
                            " VALUES ('"+ model.parceirosnome +"', "+ model.parceirosidade +",'"+ model.parceirossexo +"','"+ model.parceiroscpf +"', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');";
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString,connection))
                    {
                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro no cadastro do parceiro."); 
                        }

                        return Ok("Registro incluido!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //Post para cadastrar alimento
        [HttpPost("AddAlimento")]
        public async Task<IActionResult> PostAlimentosAsync(AlimentosViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryString = "INSERT INTO alimentos (alimentosnome, alimentoscategory, alimentosorigem, alimentosqntd)" +
                        " VALUES ('" + model.alimentosnome + "', '" + model.alimentoscategory + "','" + model.alimentosorigem + "'," + model.alimentosqntd + ");";
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {
                        
                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro no cadastro do alimento.");
                        }

                        return Ok("Registro incluido!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //Post para cadastrar Vestuario
        [HttpPost("AddVestuario")]
        public async Task<IActionResult> PostVestuarioAsync(VestuariosViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryString = "INSERT INTO vestuarios (vestuariosnome, vestuarioscategory, vestuariosorigem, vestuariosqntd)" +
                        " VALUES ('" + model.vestuariosnome + "', '" + model.vestuarioscategory + "','" + model.vestuariosorigem + "'," + model.vestuariosqntd + ");";
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {

                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro no cadastro do alimento.");
                        }

                        return Ok("Registro incluido!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //Post para cadastrar higiene
        [HttpPost("AddHigiene")]
        public async Task<IActionResult> PostHigieneAsync(HigieneViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryString = "INSERT INTO higiene (higienenome, higienecategory, higieneorigem, higieneqntd)" +
                        " VALUES ('" + model.higienenome + "', '" + model.higienecategory + "','" + model.higieneorigem + "'," + model.higieneqntd + ");";
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {

                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro no cadastro do produto de higiene.");
                        }

                        return Ok("Registro incluido!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }



        //Delete para o morador
        [HttpDelete("DelParceiro")]
        public async Task <IActionResult> DelParceirosAsync(int idparceiros)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string queryString = "DELETE FROM parceiros WHERE idparceiros = " + idparceiros + "";
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {
                        
                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro ao excluir o parceiro.");
                        }

                        return Ok("Registro deletado!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }

        }



        //HttpPut para atualizar a quantidade de alimentos pelo id
        [HttpPut("UpdateAlimento")]
        public async Task <IActionResult> PutAlimentoAsyc(int idAlimento, int quantidadeAlimento)
        {
            string queryString;
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (quantidadeAlimento == 0)
                    {
                        queryString = "DELETE FROM alimentos WHERE idalimentos = " + idAlimento + "";
                    }
                    else
                    {
                        queryString = "UPDATE alimentos SET alimentosqntd = " + quantidadeAlimento + " WHERE idalimentos = " + idAlimento + "";
                    }
                    
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {

                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro ao atualizar o alimento.");
                        }

                        return Ok("Registro atualizado!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //HttpPut para atualizar a quantidade de vestuarios pelo id
        [HttpPut("UpdateVestuario")]
        public async Task<IActionResult> PutVestuarioAsyc(int idVestuario, int quantidadeVestuario)
        {
            string queryString;
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (quantidadeVestuario == 0)
                    {
                        queryString = "DELETE FROM vestuarios WHERE idvestuarios = " + idVestuario + "";
                    }
                    else
                    {
                        queryString = "UPDATE vestuarios SET vestuariosqntd = " + quantidadeVestuario + " WHERE idvestuarios = " + idVestuario + "";
                    }
                    
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {

                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro ao atualizar o vestuario.");
                        }

                        return Ok("Registro atualizado!");
                    }
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //HttpPut para atualizar a quantidade de Higiene pelo id
        [HttpPut("UpdateHigiene")]
        public async Task<IActionResult> PutHigieneAsyc(int idHigiene, int quantidadeHigiene)
        {
            string queryString;
            if (ModelState.IsValid)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (quantidadeHigiene == 0)
                    {
                        queryString = "DELETE FROM higiene WHERE idhigiene = " + idHigiene + "";
                    }
                    else
                    {
                        queryString = "UPDATE higiene SET higieneqntd = " + quantidadeHigiene + " WHERE idhigiene = " + idHigiene + "";
                    }
                        
                    await connection.OpenAsync();
                    using (MySqlCommand command = new MySqlCommand(queryString, connection))
                    {

                        int count = Convert.ToInt32(await command.ExecuteNonQueryAsync());
                        if (count == 0)
                        {
                            return BadRequest("Houve um erro ao atualizar a Higiene.");
                        }

                        return Ok("Registro atualizado!");
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

