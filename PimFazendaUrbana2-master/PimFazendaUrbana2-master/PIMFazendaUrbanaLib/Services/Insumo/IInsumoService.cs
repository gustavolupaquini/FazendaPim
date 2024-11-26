namespace PIMFazendaUrbanaLib
{
    public interface IInsumoService
    {
        List<Insumo> ListarInsumosComFiltros(string search);
        void CadastrarInsumo(Insumo insumo);
        void AlterarInsumo(Insumo insumo);
        Insumo ConsultarInsumoPorId(int insumoID);
        List<Insumo> ListarInsumosAtivos();
        List<Insumo> ListarInsumosEmEstoque();
        List<Insumo> ListarInsumosInativos();
        void DesativarInsumo(int insumoID);
        List<Insumo> FiltrarInsumosPorNome(string insumoNome);
        List<SaidaInsumo> ListarSaidaInsumos();
        List<SaidaInsumo> ListarSaidaInsumosComFiltros(string search);
        List<SaidaInsumo> FiltrarSaidaInsumosPorNome(string insumoNome);
        List<Insumo> FiltrarInsumosPorUnidade(string unidade);
        string ObterCategoriaPorNomeInsumo(string nomeInsumo);
        void CadastrarSaidaInsumo(SaidaInsumo saidainsumo);
        bool AumentarQtdInsumo(Insumo insumo, int qtd);
        List<SaidaInsumo> FiltrarSaidaInsumosPorNomeEPeriodo(string insumoNome, DateTime dataInicio, DateTime dataFim);
        void ValidarInsumo(Insumo insumo);
        void ValidarSaidaInsumo(SaidaInsumo saidainsumo);
    }
}