namespace PIMFazendaUrbanaLib
{
    public interface IVendaService
    {
        List<PedidoVendaItem> ListarPedidoVendaItensComFiltros(string search);

        //void CadastrarPedidoVendaComItens(PedidoVenda pedidoVenda, List<PedidoVendaItem> vendaItems);
        void CadastrarPedidoVenda(PedidoVenda pedidoVenda);
        List<PedidoVenda> ListarPedidosVenda();
        PedidoVenda ConsultarPedidoVendaPorId(int idPedidoVenda);
        int? ObterUltimoIdPedidoVenda();
        List<PedidoVendaItem> ListarRegistrosDeVenda();
        PedidoVendaItem ConsultarVendaItemPorId(int idVendaItem);
        List<PedidoVendaItem> FiltrarRegistrosDeVendaPorNomeEPeriodo(string produtoNome, DateTime dataInicio, DateTime dataFim);
        List<PedidoVendaItem> FiltrarRegistrosDeVendaPorNome(string produtoNome);

        //void ValidarVenda(PedidoVenda pedidoVenda, List<PedidoVendaItem> vendaItems);
        void ValidarVenda(PedidoVenda pedidoVenda);
    }
}