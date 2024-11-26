namespace PIMFazendaUrbanaAPI.DTOs
{
    public class FuncionarioSessionDTO // DTO com os dados básicos do funcionário para a sessão
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public string Cargo { get; set; }
        public string Usuario { get; set; }
    }
}
