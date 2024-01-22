using API_JOGOS.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace API_JOGOS.Controllers
{
    [Route("api/categorias")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private static string conString = "Host=10.1.1.215;Port=15432;Database=Anna;Username=postgres;Password=formacao;";

        [HttpGet]
        public IActionResult ListarTodos()
        {
            List<Categoria> retorno = new List<Categoria>();

            using (var conexao = new NpgsqlConnection(conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand("SELECT * FROM categoria", conexao))
                {
                    var leitor = comando.ExecuteReader();
                    while (leitor.Read())
                    {
                        var categoria = new Categoria()
                        {
                            Id = leitor.GetInt32(0),
                            Nome = leitor.GetString(1)
                        };
                        retorno.Add(categoria);
                    }
                    leitor.Close();
                }

                conexao.Close();
            }

            return Ok(retorno);
        }

        [HttpPost]
        public IActionResult InserirCategoria([FromBody] Categoria categoria)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("INSERT INTO categoria (nome) VALUES (@nome)", conexao))
                    {
                        comando.Parameters.AddWithValue("nome", categoria.Nome);

                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok(categoria);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarCategoria(int id)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("DELETE FROM categoria WHERE id = @id", conexao))
                    {
                        comando.Parameters.AddWithValue("id", id);

                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok("Categoria excluída");
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpGet("{id}/jogos")]
        public IActionResult ListarJogosPorCategoria(int id)
        {
            try
            {
                List<Jogo> jogos = new List<Jogo>();

                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand($"SELECT * FROM jogo WHERE categoriaId = {id}", conexao))
                    {
                        var leitor = comando.ExecuteReader();
                        while (leitor.Read())
                        {
                            var jogo = new Jogo()
                            {
                                Id = leitor.GetInt32(0),
                                Nome = leitor.GetString(1),
                                Preco = leitor.GetDecimal(2),
                                CategoriaId = leitor.GetInt32(3)
                            };
                            jogos.Add(jogo);
                        }
                        leitor.Close();
                    }

                    conexao.Close();
                }

                return Ok(jogos);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }
    }
}
