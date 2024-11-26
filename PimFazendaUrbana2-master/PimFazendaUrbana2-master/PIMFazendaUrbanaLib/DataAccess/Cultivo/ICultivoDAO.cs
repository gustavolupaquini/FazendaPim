namespace PIMFazendaUrbanaLib
{
    public interface ICultivoDAO
    {
        List<Cultivo> ListarCultivosComFiltros(string search);
        void CadastrarCultivo(Cultivo cultivo);
        void AlterarCultivo(Cultivo cultivo);
        void ExcluirCultivo(int cultivoId);
        List<Cultivo> ListarCultivosAtivos();
        List<Cultivo> ListarCultivosInativos();
        Cultivo ConsultarCultivoPorId(int cultivoId);
        Cultivo ConsultarCultivoPorNome(string cultivoNome);
        List<Cultivo> FiltrarCultivosPorNome(string cultivoNome);
        public List<string> ListarCategorias();
    }
}