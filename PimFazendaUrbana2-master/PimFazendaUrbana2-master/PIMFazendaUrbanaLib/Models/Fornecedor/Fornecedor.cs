﻿namespace PIMFazendaUrbanaLib

{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CNPJ { get; set; }
        public bool StatusAtivo { get; set; }
        public Endereco Endereco { get; set; }
        public Telefone Telefone { get; set; }
    }
}