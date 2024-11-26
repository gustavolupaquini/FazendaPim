﻿namespace PIMFazendaUrbanaAPI.DTOs
{
    public class PedidoCompraDTO  // atualizado com composição
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int IdFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public List<PedidoCompraItemDTO> Itens { get; set; }
        public decimal ValorTotal => Itens.Sum(i => i.ValorTotal);
    }
}

