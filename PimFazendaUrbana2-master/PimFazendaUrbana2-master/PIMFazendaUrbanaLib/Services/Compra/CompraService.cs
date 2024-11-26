﻿//using MySql.Data.MySqlClient;

namespace PIMFazendaUrbanaLib
{
    public class CompraService : ICompraService
    {
        private readonly ICompraDAO pedidoCompraDAO;
        //private readonly string connectionString;

        public CompraService(string connectionString)
        {
            //this.connectionString = connectionString;
            this.pedidoCompraDAO = new CompraDAO(connectionString);
        }

        // Método antigo para cadastrar um novo pedido de compra
        /*
        public void CadastrarPedidoCompraComItens(PedidoCompra pedidoCompra, List<PedidoCompraItem> compraItems)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        ValidarCompra(pedidoCompra, compraItems);

                        // Cadastrar PedidoCompra
                        pedidoCompraDAO.CadastrarPedidoCompra(pedidoCompra, transaction);

                        // Cadastrar Itens de Compra
                        foreach (var item in compraItems)
                        {
                            item.IdPedidoCompra = pedidoCompra.Id;
                            pedidoCompraDAO.CadastrarCompraItem(item, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro ao cadastrar pedido de compra: " + ex.Message);
                    }
                }
            }
        }
        */

        public List<PedidoCompraItem> ListarPedidoCompraItensComFiltros(string search)
        {
            try
            {
                List<PedidoCompraItem> compraitens = pedidoCompraDAO.ListarPedidoCompraItensComFiltros(search);
                return compraitens; // Retorna a lista filtrada de compras
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar compras filtradas: " + ex.Message); // Lança uma exceção indicando que ocorreu um erro ao listar compras filtradas
            }
        }

        // Método para cadastrar um novo pedido de compra
        public void CadastrarPedidoCompra(PedidoCompra pedidoCompra)
        {
            try
            {
                ValidarCompra(pedidoCompra);
                pedidoCompraDAO.CadastrarPedidoCompra(pedidoCompra);
            }
            catch (ValidationException ex) // Se ocorrer uma exceção do tipo ValidationException
            {
                throw; // Repassa a exceção de validação para o Controller manipular
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar pedido de compra: " + ex.Message);
            }

        }

        // Método para listar todos os pedidos de compra
        public List<PedidoCompra> ListarPedidosCompra()
        {
            try
            {
                return pedidoCompraDAO.ListarPedidosCompra();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar pedidos de compra: " + ex.Message);
            }
        }

        // Método para consultar um pedido de compra pelo ID
        public PedidoCompra ConsultarPedidoCompraPorId(int idPedidoCompra)
        {
            try
            {
                return pedidoCompraDAO.ConsultarPedidoCompraPorId(idPedidoCompra);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar pedido de compra: " + ex.Message);
            }
        }

        // Método para obter o último ID de pedido de compra
        public int? ObterUltimoIdPedidoCompra()
        {
            try
            {
                return pedidoCompraDAO.ObterUltimoIdPedidoCompra();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter último ID de pedido de compra: " + ex.Message);
            }
        }

        // Método para listar todos os itens de compra
        public List<PedidoCompraItem> ListarRegistrosDeCompra()
        {
            try
            {
                return pedidoCompraDAO.ListarRegistrosDeCompra();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar registros de compra: " + ex.Message);
            }
        }

        public List<PedidoCompraItem> FiltrarRegistrosDeCompraPorNome(string insumoNome)
        {
            try
            {
                List<PedidoCompraItem> compraItems = pedidoCompraDAO.FiltrarRegistrosDeCompraPorNome(insumoNome);
                return compraItems;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao filtrar registros de compra por nome de insumo: " + ex.Message);
            }
        }

        // Método para consultar um item de compra pelo ID
        public PedidoCompraItem ConsultarCompraItemPorId(int idCompraItem)
        {
            try
            {
                return pedidoCompraDAO.ConsultarCompraItemPorId(idCompraItem);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar item de compra: " + ex.Message);
            }
        }

        public List<PedidoCompraItem> FiltrarRegistrosDeCompraPorNomeEPeriodo(string insumoNome, DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                return pedidoCompraDAO.FiltrarRegistrosDeCompraPorNomeEPeriodo(insumoNome, dataInicio, dataFim);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao filtrar registros de compra por nome de insumo e período: " + ex.Message);
            }
        }

        /*
         * =-=-=-=-=-=-=-=-=-=-=-=- VALIDAÇÃO COMPRA =-=-=-=-=-=-=-=-=-=-=-=-
         */

        public void ValidarCompra(PedidoCompra pedidoCompra)
        {
            var erros = new List<ValidationError>();

            // validar quantidade, valor unitario, fornecedor e produto

            if (pedidoCompra.Itens.Count <= 0) // Verifica se a quantidade de itens é maior que 0
            {
                erros.Add((new ValidationError("Quantidade", "A compra deve conter pelo menos um item.")));
            }

            // valida fornecedor
            if (pedidoCompra.IdFornecedor <= 0)
            {
                erros.Add(new ValidationError("Fornecedor", "O fornecedor deve ser informado."));
            }

            // Valida cada item da compra
            foreach (var item in pedidoCompra.Itens)
            {
                // valida nome do produto
                if (string.IsNullOrWhiteSpace(item.NomeInsumo))
                {
                    erros.Add(new ValidationError("Insumo", "Insumo inválido."));
                }

                // Verifica se a quantidade do item de cada compra é menor ou igual a zero
                if (item.Qtd <= 0)
                {
                    erros.Add(new ValidationError("Quantidade", $"A quantidade do item '{item.NomeInsumo}' deve ser um número inteiro maior que zero."));
                }
                // Verifica se o valor unitário do item de cada compra é menor ou igual a zero
                if (item.Valor <= 0)
                {
                    erros.Add(new ValidationError("Valor", $"O valor unitário do item '{item.NomeInsumo}' deve ser um número decimal maior que zero."));
                }
            }

            if (erros.Any()) // se teve algum erro, lança exceção com a lista de erros
            {
                throw new ValidationException(erros);
            }

        }

    }
}
