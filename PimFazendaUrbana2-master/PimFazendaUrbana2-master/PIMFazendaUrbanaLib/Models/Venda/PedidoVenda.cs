namespace PIMFazendaUrbanaLib
{
    public class PedidoVenda // atualizado com composição
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public List<PedidoVendaItem> Itens { get; set; }
        public decimal ValorTotal => Itens.Sum(i => i.ValorTotal);
    }
}

