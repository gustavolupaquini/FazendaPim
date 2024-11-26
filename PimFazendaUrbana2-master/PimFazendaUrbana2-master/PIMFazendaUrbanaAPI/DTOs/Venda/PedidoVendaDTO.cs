namespace PIMFazendaUrbanaAPI.DTOs
{
    public class PedidoVendaDTO
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public List<PedidoVendaItemDTO> Itens { get; set; }
        public decimal ValorTotal => Itens.Sum(i => i.ValorTotal);
    }
}

