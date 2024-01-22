using API_JOGOS.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace API_JOGOS.Controllers
{
    [Route("api/jogos")]
    [ApiController]
    public class JogoController : ControllerBase
    {
        private static string conString = "Host=10.1.1.215;Port=15432;Database=Anna;Username=postgres;Password=formacao;";

        [HttpPost]
        public IActionResult InserirJogo([FromBody] Jogo jogo)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("INSERT INTO jogo (nome, preco, categoriaId) VALUES (@nome, @preco, @categoriaId)", conexao))
                    {
                        comando.Parameters.AddWithValue("nome", jogo.Nome);
                        comando.Parameters.AddWithValue("preco", jogo.Preco);
                        comando.Parameters.AddWithValue("categoriaId", jogo.CategoriaId);

                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok(jogo);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarJogo(int id)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("DELETE FROM jogo WHERE id = @id", conexao))
                    {
                        comando.Parameters.AddWithValue("id", id);

                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok("Jogo excluído");
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }
    }

}

