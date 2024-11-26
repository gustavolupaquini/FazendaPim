namespace PIMFazendaUrbanaLib
{
    public interface ICultivoService
    {
        List<Cultivo> ListarCultivosComFiltros(string search);
        void CadastrarCultivo(Cultivo cultivo);
        void AlterarCultivo(Cultivo cultivo);
        void ExcluirCultivo(int cultivoId);
        List<Cultivo> ListarCultivosAtivos();
        List<Cultivo> ListarCultivosInativos();
        List<string> ListarCategorias();
        Cultivo ConsultarCultivoPorId(int cultivoId);
        List<Cultivo> FiltrarCultivosPorNome(string cultivoNome);
        void ValidarCultivo(Cultivo cultivo);
    }
}