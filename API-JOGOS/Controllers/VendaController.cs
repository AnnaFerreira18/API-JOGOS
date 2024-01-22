using API_JOGOS.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace API_JOGOS.Controllers
{
    [Route("api/vendas")]
    [ApiController]
    public class VendaController : ControllerBase
    {
        private static string conString = "Host=10.1.1.215;Port=15432;Database=Anna;Username=postgres;Password=formacao;";

        [HttpGet]
        public IActionResult ListarTodos()
        {
            List<Venda> retorno = new List<Venda>();

            using (var conexao = new NpgsqlConnection(conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand("SELECT * FROM venda", conexao))
                {
                    var leitor = comando.ExecuteReader();
                    while (leitor.Read())
                    {
                        var venda = new Venda()
                        {
                            DataCompra = leitor.GetDateTime(0),
                            CodigoJogoComprado = leitor.GetInt32(1),
                            NomeJogoComprado = leitor.GetString(2),
                            NomeComprador = leitor.GetString(3),
                            CpfComprador = leitor.GetString(4)
                        };
                        retorno.Add(venda);
                    }
                    leitor.Close();
                }

                conexao.Close();
            }

            return Ok(retorno);
        }

        [HttpPost]
        public IActionResult InserirVenda([FromBody] Venda venda)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("INSERT INTO venda (data_compra, codigo_jogo_comprado, nome_jogo_comprado, nome_comprador, cpf_comprador) " +
                        "VALUES (@dataCompra, @codigoJogoComprado, @nomeJogoComprado, @nomeComprador, @cpfComprador)", conexao))
                    {
                        comando.Parameters.AddWithValue("dataCompra", venda.DataCompra);
                        comando.Parameters.AddWithValue("codigoJogoComprado", venda.CodigoJogoComprado);
                        comando.Parameters.AddWithValue("nomeJogoComprado", venda.NomeJogoComprado);
                        comando.Parameters.AddWithValue("nomeComprador", venda.NomeComprador);
                        comando.Parameters.AddWithValue("cpfComprador", venda.CpfComprador);

                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok(venda);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpGet("{cpfComprador}")]
        public IActionResult ListarVendasPorUsuario(string cpfComprador)
        {
            try
            {
                List<Venda> vendas = new List<Venda>();

                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand($"SELECT * FROM venda WHERE cpf_comprador = '{cpfComprador}'", conexao))
                    {
                        var leitor = comando.ExecuteReader();
                        while (leitor.Read())
                        {
                            var venda = new Venda()
                            {
                                DataCompra = leitor.GetDateTime(0),
                                CodigoJogoComprado = leitor.GetInt32(1),
                                NomeJogoComprado = leitor.GetString(2),
                                NomeComprador = leitor.GetString(3),
                                CpfComprador = leitor.GetString(4)
                            };
                            vendas.Add(venda);
                        }
                        leitor.Close();
                    }

                    conexao.Close();
                }

                return Ok(vendas);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpGet("{cpfComprador}/info")]
        public IActionResult ListarInfoVendasPorUsuario(string cpfComprador)
        {
            try
            {
                List<object> infoVendas = new List<object>();

                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand($"SELECT nome_jogo_comprado, SUM(preco) AS valor_total FROM venda" +
                        $"JOIN jogo ON venda.codigo_jogo_comprado = jogo.id WHERE cpf_comprador = '{cpfComprador}' GROUP BY nome_jogo_comprado", conexao))
                    {
                        var leitor = comando.ExecuteReader();
                        while (leitor.Read())
                        {
                            var infoVenda = new
                            {
                                NomeJogoComprado = leitor.GetString(0),
                                ValorTotal = leitor.GetDecimal(1)
                            };
                            infoVendas.Add(infoVenda);
                        }
                        leitor.Close();
                    }

                    conexao.Close();
                }

                return Ok(infoVendas);
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }

        [HttpDelete("{cpfComprador}")]
        public IActionResult DeletarVendasPorUsuario(string cpfComprador)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand($"DELETE FROM venda WHERE cpf_comprador = '{cpfComprador}'", conexao))
                    {
                        comando.ExecuteNonQuery();
                    }

                    conexao.Close();
                }

                return Ok($"Vendas do usuário com CPF {cpfComprador} excluídas");
            }
            catch (Exception e)
            {
                return BadRequest(new { erro = true, mensagem = e.Message });
            }
        }
    }
}
