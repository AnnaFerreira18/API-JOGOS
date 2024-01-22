namespace API_JOGOS.Models
{
    public class Venda
    {
        public DateTime DataCompra { get; set; }
        public int CodigoJogoComprado { get; set; }
        public string NomeJogoComprado { get; set; }
        public string NomeComprador { get; set; }
        public string CpfComprador { get; set; }
        public decimal ValorVenda { get; set; }

    }
}
