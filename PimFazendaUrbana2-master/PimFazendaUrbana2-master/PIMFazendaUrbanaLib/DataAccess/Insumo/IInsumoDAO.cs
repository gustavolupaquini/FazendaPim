namespace PIMFazendaUrbanaLib
{
    public interface IInsumoDAO
    {
        List<Insumo> ListarInsumosComFiltros(string search);
        void CadastrarInsumo(Insumo insumo);
        void AlterarInsumo(Insumo insumo);
        void DesativarInsumo(int idInsumo);
        List<Insumo> ListarInsumosAtivos();
        List<Insumo> ListarInsumosEmEstoque();
        List<SaidaInsumo> ListarSaidaInsumos();
        List<SaidaInsumo> ListarSaidaInsumosComFiltros(string search);
        List<SaidaInsumo> FiltrarSaidaInsumosPorNome(string insumoNome);
        List<Insumo> ListarInsumosInativos();
        Insumo ConsultarInsumoPorId(int idInsumo);
        List<Insumo> FiltrarInsumosPorNome(string insumoNome);
        List<Insumo> FiltrarInsumosPorUnidade(string unidade);
        string ObterCategoriaPorNomeInsumo(string nomeInsumo);
        void CadastrarSaidaInsumo(SaidaInsumo saidainsumo);
        void AumentarQtdInsumo(Insumo insumo, int qtd);
        List<SaidaInsumo> FiltrarSaidaInsumosPorNomeEPeriodo(string insumoNome, DateTime dataInicio, DateTime dataFim);

    }
}