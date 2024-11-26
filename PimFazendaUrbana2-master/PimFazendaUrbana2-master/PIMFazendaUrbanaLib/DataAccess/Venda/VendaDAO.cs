using MySql.Data.MySqlClient;

namespace PIMFazendaUrbanaLib
{
    public class VendaDAO : IVendaDAO
    {
        private readonly string connectionString;

        public VendaDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // NOVO método para filtrar pedidos de venda e seus itens por uma query de busca
        public List<PedidoVendaItem> ListarPedidoVendaItensComFiltros(string search)
        {
            List<PedidoVendaItem> pedidosVendaItem = new List<PedidoVendaItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT vi.id_vendaitem, vi.qtd_vendaitem, vi.unidqtd_vendaitem, vi.valor_vendaitem, vi.id_pedidovenda, vi.id_estoqueproduto, 
                                cul.variedade_cultivo, pv.data_pedidovenda, cli.nome_cliente
                                FROM vendaitem vi
                                LEFT JOIN estoqueproduto ep ON vi.id_estoqueproduto = ep.id_estoqueproduto
                                LEFT JOIN producao pr ON ep.id_producao = pr.id_producao
                                LEFT JOIN cultivo cul ON pr.id_cultivo = cul.id_cultivo
                                LEFT JOIN pedidovenda pv ON vi.id_pedidovenda = pv.id_pedidovenda
                                LEFT JOIN cliente cli ON pv.id_cliente = cli.id_cliente
                                WHERE cul.variedade_cultivo LIKE @search OR cli.nome_cliente LIKE @search
                                OR pv.id_pedidovenda LIKE @search OR vi.id_vendaitem LIKE @search
                                OR pv.data_pedidovenda LIKE @search";
;
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@search", "%" + search + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PedidoVendaItem vendaItem = new PedidoVendaItem
                            {
                                Id = reader.GetInt32("id_vendaitem"),
                                Qtd = reader.GetInt32("qtd_vendaitem"),
                                UnidQtd = reader.GetString("unidqtd_vendaitem"),
                                Valor = reader.GetDecimal("valor_vendaitem"),
                                IdPedidoVenda = reader.GetInt32("id_pedidovenda"),
                                IdProduto = reader.GetInt32("id_estoqueproduto"),
                                NomeProduto = reader.GetString("variedade_cultivo"),
                                Data = reader.GetDateTime("data_pedidovenda"),
                                NomeCliente = reader.GetString("nome_cliente")
                            };
                            pedidosVendaItem.Add(vendaItem);
                        }
                    }
                }
            }

            return pedidosVendaItem;
        }

        // NOVO Método para obter todos os pedidos de venda e seus itens
        public List<PedidoVenda> ListarPedidosVendaComItems()
        {
            // cada PedidoVenda possui um List<PedidoVendaItem> Itens
            List<PedidoVenda> pedidosVenda = new List<PedidoVenda>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT pv.id_pedidovenda, pv.data_pedidovenda, pv.id_cliente, c.nome_cliente 
                                FROM pedidovenda pv
                                LEFT JOIN cliente c ON pv.id_cliente = c.id_cliente";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PedidoVenda pedidoVenda = new PedidoVenda
                            {
                                Id = reader.GetInt32("id_pedidovenda"),
                                Data = reader.GetDateTime("data_pedidovenda"),
                                IdCliente = reader.GetInt32("id_cliente"),
                                NomeCliente = reader.GetString("nome_cliente")
                            };

                            pedidoVenda.Itens = ListarItensPedidoVendaPorId(pedidoVenda.Id);
                            pedidosVenda.Add(pedidoVenda);
                        }
                    }
                }
            }
            return pedidosVenda;
        }

        // NOVO Método para listar os itens de venda de um pedido de venda por id
        public List<PedidoVendaItem> ListarItensPedidoVendaPorId(int idPedidoVenda)
        {
            List<PedidoVendaItem> vendaItens = new List<PedidoVendaItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT vi.id_vendaitem, vi.qtd_vendaitem, vi.unidqtd_vendaitem, vi.valor_vendaitem, vi.id_pedidovenda, vi.id_estoqueproduto, 
                                cul.variedade_cultivo
                                FROM vendaitem vi
                                LEFT JOIN estoqueproduto ep ON vi.id_estoqueproduto = ep.id_estoqueproduto
                                LEFT JOIN producao pr ON ep.id_producao = pr.id_producao
                                LEFT JOIN cultivo cul ON pr.id_cultivo = cul.id_cultivo
                                WHERE vi.id_pedidovenda = @idPedidoVenda";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idPedidoVenda", idPedidoVenda);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PedidoVendaItem vendaItem = new PedidoVendaItem
                            {
                                Id = reader.GetInt32("id_vendaitem"),
                                Qtd = reader.GetInt32("qtd_vendaitem"),
                                UnidQtd = reader.GetString("unidqtd_vendaitem"),
                                Valor = reader.GetDecimal("valor_vendaitem"),
                                IdPedidoVenda = reader.GetInt32("id_pedidovenda"),
                                IdProduto = reader.GetInt32("id_estoqueproduto"),
                                NomeProduto = reader.GetString("variedade_cultivo")
                            };
                            vendaItens.Add(vendaItem);
                        }
                    }
                }

            }
            return vendaItens;
        }


        // Método antigo para cadastrar um novo pedido de venda
        /*
        public void CadastrarPedidoVenda(PedidoVenda pedidoVenda, MySqlTransaction transaction)
        {
            string insertPedidoQuery = @"INSERT INTO pedidovenda (data_pedidovenda, id_cliente) 
                                 VALUES (@dataPedidoVenda, @idCliente);
                                 SELECT LAST_INSERT_ID();";

            using (MySqlCommand insertPedidoCommand = new MySqlCommand(insertPedidoQuery, transaction.Connection, transaction))
            {
                insertPedidoCommand.Parameters.AddWithValue("@dataPedidoVenda", pedidoVenda.Data);
                insertPedidoCommand.Parameters.AddWithValue("@idCliente", pedidoVenda.IdCliente);
                pedidoVenda.Id = Convert.ToInt32(insertPedidoCommand.ExecuteScalar());
            }
        }

        // Método antigo para cadastrar um novo item de venda
        public void CadastrarVendaItem(PedidoVendaItem vendaItem, MySqlTransaction transaction)
        {
            string insertItemQuery = @"INSERT INTO vendaitem (qtd_vendaitem, unidqtd_vendaitem, valor_vendaitem, id_pedidovenda, id_estoqueproduto) 
                               VALUES (@qtdVendaItem, @unidQtdVendaItem, @valorVendaItem, @idPedidoVenda, @idEstoqueProduto)";
            using (MySqlCommand insertItemCommand = new MySqlCommand(insertItemQuery, transaction.Connection, transaction))
            {
                insertItemCommand.Parameters.AddWithValue("@qtdVendaItem", vendaItem.Qtd);
                insertItemCommand.Parameters.AddWithValue("@unidQtdVendaItem", vendaItem.UnidQtd);
                insertItemCommand.Parameters.AddWithValue("@valorVendaItem", vendaItem.Valor);
                insertItemCommand.Parameters.AddWithValue("@idPedidoVenda", vendaItem.IdPedidoVenda);
                insertItemCommand.Parameters.AddWithValue("@idEstoqueProduto", vendaItem.IdProduto);
                insertItemCommand.ExecuteNonQuery();
            }
        }
        */

        // Método para cadastrar um novo pedido de venda com itens
        public void CadastrarPedidoVenda(PedidoVenda pedidoVenda)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Cadastrar PedidoVenda
                        string insertPedidoQuery = @"INSERT INTO pedidovenda (data_pedidovenda, id_cliente) 
                                 VALUES (@dataPedidoVenda, @idCliente);
                                 SELECT LAST_INSERT_ID();";

                        using (MySqlCommand insertPedidoCommand = new MySqlCommand(insertPedidoQuery, connection))
                        {
                            insertPedidoCommand.Parameters.AddWithValue("@dataPedidoVenda", pedidoVenda.Data);
                            insertPedidoCommand.Parameters.AddWithValue("@idCliente", pedidoVenda.IdCliente);
                            pedidoVenda.Id = Convert.ToInt32(insertPedidoCommand.ExecuteScalar());
                        }

                        // Cadastrar Itens de Venda
                        foreach (var item in pedidoVenda.Itens)
                        {
                            item.IdPedidoVenda = pedidoVenda.Id;
                            string insertItemQuery = @"INSERT INTO vendaitem (qtd_vendaitem, unidqtd_vendaitem, valor_vendaitem, id_pedidovenda, id_estoqueproduto) 
                                                     VALUES (@qtdVendaItem, @unidQtdVendaItem, @valorVendaItem, @idPedidoVenda, @idEstoqueProduto)";
                            using (MySqlCommand insertItemCommand = new MySqlCommand(insertItemQuery, connection))
                            {
                                insertItemCommand.Parameters.AddWithValue("@qtdVendaItem", item.Qtd);
                                insertItemCommand.Parameters.AddWithValue("@unidQtdVendaItem", item.UnidQtd);
                                insertItemCommand.Parameters.AddWithValue("@valorVendaItem", item.Valor);
                                insertItemCommand.Parameters.AddWithValue("@idPedidoVenda", pedidoVenda.Id);
                                insertItemCommand.Parameters.AddWithValue("@idEstoqueProduto", item.IdProduto);
                                insertItemCommand.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Erro ao cadastrar pedido de venda: " + ex.Message);
                    }
                }
            }
        }


        // Método para listar todos os pedidos de venda
        public List<PedidoVenda> ListarPedidosVenda()
        {
            List<PedidoVenda> pedidosVenda = new List<PedidoVenda>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id_pedidovenda, data_pedidovenda, id_cliente FROM pedidovenda";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PedidoVenda pedidoVenda = new PedidoVenda
                            {
                                Id = reader.GetInt32("id_pedidovenda"),
                                Data = reader.GetDateTime("data_pedidovenda"),
                                IdCliente = reader.GetInt32("id_cliente")
                            };
                            pedidosVenda.Add(pedidoVenda);
                        }
                    }
                }
            }
            return pedidosVenda;
        }

        // Método para consultar um pedido de venda pelo ID
        public PedidoVenda ConsultarPedidoVendaPorId(int idPedidoVenda)
        {
            PedidoVenda pedidoVenda = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT id_pedidovenda, data_pedidovenda, id_cliente FROM pedidovenda WHERE id_pedidovenda = @idPedidoVenda";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@idPedidoVenda", idPedidoVenda);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pedidoVenda = new PedidoVenda
                        {
                            Id = reader.GetInt32("id_pedidovenda"),
                            Data = reader.GetDateTime("data_pedidovenda"),
                            IdCliente = reader.GetInt32("id_cliente")
                        };
                    }
                }
            }
            return pedidoVenda;
        }

        // Método para obter o ID do último pedido de venda cadastrado
        public int? ObterUltimoIdPedidoVenda()
        {
            int? idPedidoVenda = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MAX(id_pedidovenda) FROM pedidovenda";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            idPedidoVenda = reader.GetInt32(0);
                        }
                    }
                }
            }
            return idPedidoVenda;
        }

        // Método para listar todos os itens de venda
        public List<PedidoVendaItem> ListarRegistrosDeVenda()
        {
            List<PedidoVendaItem> vendaItens = new List<PedidoVendaItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT vi.id_vendaitem, vi.qtd_vendaitem, vi.unidqtd_vendaitem, vi.valor_vendaitem, vi.id_pedidovenda, vi.id_estoqueproduto, 
                                cul.variedade_cultivo, pv.data_pedidovenda, cli.nome_cliente
                                FROM vendaitem vi
                                LEFT JOIN estoqueproduto ep ON vi.id_estoqueproduto = ep.id_estoqueproduto
                                LEFT JOIN producao pr ON ep.id_producao = pr.id_producao
                                LEFT JOIN cultivo cul ON pr.id_cultivo = cul.id_cultivo
                                LEFT JOIN pedidovenda pv ON vi.id_pedidovenda = pv.id_pedidovenda
                                LEFT JOIN cliente cli ON pv.id_cliente = cli.id_cliente";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PedidoVendaItem vendaItem = new PedidoVendaItem
                            {
                                Id = reader.GetInt32("id_vendaitem"),
                                Qtd = reader.GetInt32("qtd_vendaitem"),
                                UnidQtd = reader.GetString("unidqtd_vendaitem"),
                                Valor = reader.GetDecimal("valor_vendaitem"),
                                IdPedidoVenda = reader.GetInt32("id_pedidovenda"),
                                IdProduto = reader.GetInt32("id_estoqueproduto"),
                                NomeProduto = reader.GetString("variedade_cultivo"),
                                Data = reader.GetDateTime("data_pedidovenda"),
                                NomeCliente = reader.GetString("nome_cliente")
                            };
                            vendaItens.Add(vendaItem);
                        }
                    }
                }
            }
            return vendaItens;
        }

        // Método para consultar um item de venda pelo ID
        public PedidoVendaItem ConsultarVendaItemPorId(int idVendaItem)
        {
            PedidoVendaItem vendaItem = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT id_vendaitem, qtd_vendaitem, unidqtd_vendaitem, valor_vendaitem, id_pedidovenda, id_produto FROM vendaitem WHERE id_vendaitem = @idVendaItem";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@idVendaItem", idVendaItem);

                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vendaItem = new PedidoVendaItem
                        {
                            Id = reader.GetInt32("id_vendaitem"),
                            Qtd = reader.GetInt32("qtd_vendaitem"),
                            UnidQtd = reader.GetString("unidqtd_vendaitem"),
                            Valor = reader.GetDecimal("valor_vendaitem"),
                            IdPedidoVenda = reader.GetInt32("id_pedidovenda"),
                            IdProduto = reader.GetInt32("id_estoqueproduto"),
                            NomeProduto = reader.GetString("variedade_cultivo"),

                        };
                    }
                }
            }
            return vendaItem;
        }

        public List<PedidoVendaItem> FiltrarRegistrosDeVendaPorNome(string produtoNome)
        {
            List<PedidoVendaItem> vendaItens = new List<PedidoVendaItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT vi.id_vendaitem, vi.qtd_vendaitem, vi.unidqtd_vendaitem, vi.valor_vendaitem, vi.id_pedidovenda, vi.id_estoqueproduto, 
                                cul.variedade_cultivo, pv.data_pedidovenda, cli.nome_cliente
                                FROM vendaitem vi
                                LEFT JOIN estoqueproduto ep ON vi.id_estoqueproduto = ep.id_estoqueproduto
                                LEFT JOIN producao pr ON ep.id_producao = pr.id_producao
                                LEFT JOIN cultivo cul ON pr.id_cultivo = cul.id_cultivo
                                LEFT JOIN pedidovenda pv ON vi.id_pedidovenda = pv.id_pedidovenda
                                LEFT JOIN cliente cli ON pv.id_cliente = cli.id_cliente
                                WHERE cul.variedade_cultivo LIKE @nome";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nome", "%" + produtoNome + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PedidoVendaItem vendaItem = new PedidoVendaItem
                            {
                                Id = reader.GetInt32("id_vendaitem"),
                                Qtd = reader.GetInt32("qtd_vendaitem"),
                                UnidQtd = reader.GetString("unidqtd_vendaitem"),
                                Valor = reader.GetDecimal("valor_vendaitem"),
                                IdPedidoVenda = reader.GetInt32("id_pedidovenda"),
                                IdProduto = reader.GetInt32("id_estoqueproduto"),
                                NomeProduto = reader.GetString("variedade_cultivo"),
                                Data = reader.GetDateTime("data_pedidovenda"),
                                //NomeCliente = reader.GetString("nome_cliente")
                            };
                            vendaItens.Add(vendaItem);
                        }
                    }
                }
            }
            return vendaItens;
        }

        public List<PedidoVendaItem> FiltrarRegistrosDeVendaPorNomeEPeriodo(string produtoNome, DateTime dataInicio, DateTime dataFim)
        {
            List<PedidoVendaItem> vendaItens = new List<PedidoVendaItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT vi.id_vendaitem, vi.qtd_vendaitem, vi.unidqtd_vendaitem, vi.valor_vendaitem, vi.id_pedidovenda, vi.id_estoqueproduto, 
                                cul.variedade_cultivo, pv.data_pedidovenda, cli.nome_cliente
                                FROM vendaitem vi
                                LEFT JOIN estoqueproduto ep ON vi.id_estoqueproduto = ep.id_estoqueproduto
                                LEFT JOIN producao pr ON ep.id_producao = pr.id_producao
                                LEFT JOIN cultivo cul ON pr.id_cultivo = cul.id_cultivo
                                LEFT JOIN pedidovenda pv ON vi.id_pedidovenda = pv.id_pedidovenda
                                LEFT JOIN cliente cli ON pv.id_cliente = cli.id_cliente
                                WHERE cul.variedade_cultivo LIKE @nome AND pv.data_pedidovenda BETWEEN @dataInicio AND @dataFim";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nome", "%" + produtoNome + "%");
                    command.Parameters.AddWithValue("@dataInicio", dataInicio.ToString("yyyy-MM-dd 00:00:00"));
                    command.Parameters.AddWithValue("@dataFim", dataFim.ToString("yyyy-MM-dd 23:59:59"));

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PedidoVendaItem vendaItem = new PedidoVendaItem
                            {
                                Id = reader.GetInt32("id_vendaitem"),
                                Qtd = reader.GetInt32("qtd_vendaitem"),
                                UnidQtd = reader.GetString("unidqtd_vendaitem"),
                                Valor = reader.GetDecimal("valor_vendaitem"),
                                IdPedidoVenda = reader.GetInt32("id_pedidovenda"),
                                IdProduto = reader.GetInt32("id_estoqueproduto"),
                                NomeProduto = reader.GetString("variedade_cultivo"),
                                Data = reader.GetDateTime("data_pedidovenda"),
                                //NomeCliente = reader.GetString("nome_cliente")
                            };
                            vendaItens.Add(vendaItem);
                        }
                    }
                }
            }
            return vendaItens;
        }


    }
}
