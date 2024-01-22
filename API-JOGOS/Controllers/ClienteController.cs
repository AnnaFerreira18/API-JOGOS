using API_JOGOS.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace API_JOGOS.Controllers
{
    [Route("api/clientes")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private static string conString = "Host=10.1.1.215;Port=15432;Database=Anna;Username=postgres;Password=formacao;";

        [HttpGet]
        public IActionResult ListarTodos()
        {
        
            using (var conexao = new NpgsqlConnection(conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand("select * from usuario", conexao))
                {
                    var clientes = new List<Usuario>();

                    using (var leitor = comando.ExecuteReader())
                    {

                        while (leitor.Read())
                        {
                            clientes.Add(new Usuario
                            {
                                Codigo = leitor.GetInt32(0),
                                Cpf = leitor.GetString(1),
                                Nome = leitor["nome"].ToString(),
                                Idade = leitor.GetInt32(3)
                            });
                        }
                    }
                    return Ok(clientes);
                }
            }

        }

        [HttpGet("{cpf}")]
        public IActionResult BuscarPorCpf(string cpf)
        {
            try
            {
                Usuario cliente = null;

                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand($"SELECT * FROM usuario WHERE cpf = '{cpf}'", conexao))
                    {
                        var leitor = comando.ExecuteReader();
                        while (leitor.Read())
                        {
                            cliente = new Usuario()
                            {
                                Codigo = leitor.GetInt32(0),
                                Cpf = leitor.GetString(1),
                                Nome = leitor["nome"].ToString(),
                                Idade = leitor.GetInt32(3)
                            };
                        }
                        leitor.Close();
                    }

                    conexao.Close();
                }

                if (cliente != null)
                {
                    return Ok(cliente);
                }
                else
                {
                    return NotFound("Cliente não encontrado");
                }
            }
            catch (Exception)
            {
                return BadRequest("Ocorreu uma falha");
            }
        }

        [HttpPost]
        public IActionResult CadastrarCliente([FromBody] Usuario cliente)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("INSERT INTO usuario (cpf, nome, idade) VALUES (@cpf, @nome, @idade)", conexao))
                    {
                        comando.Parameters.AddWithValue("@cpf", cliente.Cpf);
                        comando.Parameters.AddWithValue("@nome", cliente.Nome);
                        comando.Parameters.AddWithValue("@idade", cliente.Idade);

                        var codigo = comando.ExecuteScalar();
                        cliente.Codigo = Convert.ToInt32(codigo);
                    }

                }

                return Ok(cliente);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarCliente(int id, [FromBody] Usuario cliente)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("UPDATE usuario SET cpf = @cpf, nome = @nome, idade = @idade WHERE codigo = @codigo", conexao))
                    {
                        comando.Parameters.AddWithValue("cpf", cliente.Cpf);
                        comando.Parameters.AddWithValue("nome", cliente.Nome);
                        comando.Parameters.AddWithValue("idade", cliente.Idade);
                        comando.Parameters.AddWithValue("codigo", id);

                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok(cliente);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarCliente(int id)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("DELETE FROM usuario WHERE codigo = @codigo", conexao))
                    {
                        comando.Parameters.AddWithValue("codigo", id);

                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok("Cliente excluído");
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }
    }
}
