using MySql.Data.MySqlClient;

namespace PIMFazendaUrbanaLib
{
    public class EstoqueProdutoDAO : IEstoqueProdutoDAO
    {
        private readonly string connectionString;

        public EstoqueProdutoDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Método para listar estoque de produtos com filtros, contendo objeto Producao
        public List<EstoqueProduto> ListarEstoqueProdutoComFiltros(string search)
        {
            List<EstoqueProduto> produtos = new List<EstoqueProduto>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT ep.id_estoqueproduto, ep.qtd_estoqueproduto, ep.unidqtd_estoqueproduto, 
                                ep.dataentrada_estoqueproduto, ep.ativo_estoqueproduto,

                                p.id_producao, p.qtd_producao, p.unidqtd_producao, p.ambientectrl_producao, 
                                p.finalizado_producao, p.data_producao, p.datacolheita_producao,

                                c.id_cultivo, c.nome_cultivo, c.variedade_cultivo, c.categoria_cultivo,
                                c.tempoprodtrad_cultivo, c.tempoprodctrl_cultivo, c.ativo_cultivo

                                FROM estoqueproduto ep
                                LEFT JOIN producao p ON ep.id_producao = p.id_producao 
                                LEFT JOIN cultivo c ON p.id_cultivo = c.id_cultivo 

                                WHERE 
                                c.id_cultivo LIKE @search
                                OR c.nome_cultivo LIKE @search 
                                OR c.variedade_cultivo LIKE @search 
                                OR ep.id_estoqueproduto LIKE @search
                                OR p.id_producao LIKE @search"
                                ;

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@search", "%" + search + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EstoqueProduto produto = new EstoqueProduto
                            {
                                Id = reader.GetInt32("id_estoqueproduto"),
                                Qtd = reader.GetInt32("qtd_estoqueproduto"),
                                Unidqtd = reader.GetString("unidqtd_estoqueproduto"),
                                DataEntrada = reader.GetDateTime("dataentrada_estoqueproduto"),
                                StatusAtivo = reader.GetBoolean("ativo_estoqueproduto"),

                                Producao = new Producao
                                {
                                    Id = reader.GetInt32("id_producao"),
                                    Qtd = reader.GetInt32("qtd_producao"),
                                    Unidqtd = reader.GetString("unidqtd_producao"),
                                    AmbienteControlado = reader.GetBoolean("ambientectrl_producao"),
                                    StatusFinalizado = reader.GetBoolean("finalizado_producao"),
                                    Data = reader.GetDateTime("data_producao"),
                                    DataColheita = reader.GetDateTime("datacolheita_producao"),

                                    Cultivo = new Cultivo
                                    {
                                        Id = reader.GetInt32("id_cultivo"),
                                        Nome = reader.GetString("nome_cultivo"),
                                        Variedade = reader.GetString("variedade_cultivo"),
                                        Categoria = reader.GetString("categoria_cultivo"),
                                        TempoProdTradicional = reader.GetInt32("tempoprodtrad_cultivo"),
                                        TempoProdControlado = reader.GetInt32("tempoprodctrl_cultivo"),
                                        StatusAtivo = reader.GetBoolean("ativo_cultivo")
                                    }

                                },
                            };
                            produtos.Add(produto);
                        }
                    }
                }
            }
            return produtos;
        }


        // Método para listar estoque de produtos (ativos)
        public List<EstoqueProduto> ListarEstoqueProdutoAtivos()
        {
            List<EstoqueProduto> produtos = new List<EstoqueProduto>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT ep.id_estoqueproduto, ep.qtd_estoqueproduto, ep.unidqtd_estoqueproduto, 
                                ep.dataentrada_estoqueproduto, ep.ativo_estoqueproduto,

                                p.id_producao, p.qtd_producao, p.unidqtd_producao, p.ambientectrl_producao, 
                                p.finalizado_producao, p.data_producao, p.datacolheita_producao,

                                c.id_cultivo, c.nome_cultivo, c.variedade_cultivo, c.categoria_cultivo,
                                c.tempoprodtrad_cultivo, c.tempoprodctrl_cultivo, c.ativo_cultivo

                                FROM estoqueproduto ep
                                LEFT JOIN producao p ON ep.id_producao = p.id_producao 
                                LEFT JOIN cultivo c ON p.id_cultivo = c.id_cultivo 

                                WHERE ep.ativo_estoqueproduto = true";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EstoqueProduto produto = new EstoqueProduto
                            {
                                Id = reader.GetInt32("id_estoqueproduto"),
                                Qtd = reader.GetInt32("qtd_estoqueproduto"),
                                Unidqtd = reader.GetString("unidqtd_estoqueproduto"),
                                DataEntrada = reader.GetDateTime("dataentrada_estoqueproduto"),
                                StatusAtivo = reader.GetBoolean("ativo_estoqueproduto"),

                                Producao = new Producao
                                {
                                    Id = reader.GetInt32("id_producao"),
                                    Qtd = reader.GetInt32("qtd_producao"),
                                    Unidqtd = reader.GetString("unidqtd_producao"),
                                    AmbienteControlado = reader.GetBoolean("ambientectrl_producao"),
                                    StatusFinalizado = reader.GetBoolean("finalizado_producao"),
                                    Data = reader.GetDateTime("data_producao"),
                                    DataColheita = reader.GetDateTime("datacolheita_producao"),

                                    Cultivo = new Cultivo
                                    {
                                        Id = reader.GetInt32("id_cultivo"),
                                        Nome = reader.GetString("nome_cultivo"),
                                        Variedade = reader.GetString("variedade_cultivo"),
                                        Categoria = reader.GetString("categoria_cultivo"),
                                        TempoProdTradicional = reader.GetInt32("tempoprodtrad_cultivo"),
                                        TempoProdControlado = reader.GetInt32("tempoprodctrl_cultivo"),
                                        StatusAtivo = reader.GetBoolean("ativo_cultivo")
                                    }

                                },
                            };
                            produtos.Add(produto);
                        }
                    }
                }
            }
            return produtos;
        }

        // Método para filtrar estoque de produtos (ativos) pelo nome
        public List<EstoqueProduto> FiltrarProdutosPorNome(string produtoNome)
        {
            List<EstoqueProduto> produtos = new List<EstoqueProduto>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT ep.id_estoqueproduto, ep.qtd_estoqueproduto, ep.unidqtd_estoqueproduto, 
                                ep.dataentrada_estoqueproduto, ep.ativo_estoqueproduto,

                                p.id_producao, p.qtd_producao, p.unidqtd_producao, p.ambientectrl_producao, 
                                p.finalizado_producao, p.data_producao, p.datacolheita_producao,

                                c.id_cultivo, c.nome_cultivo, c.variedade_cultivo, c.categoria_cultivo,
                                c.tempoprodtrad_cultivo, c.tempoprodctrl_cultivo, c.ativo_cultivo

                                FROM estoqueproduto ep
                                LEFT JOIN producao p ON ep.id_producao = p.id_producao 
                                LEFT JOIN cultivo c ON p.id_cultivo = c.id_cultivo 

                                WHERE ep.ativo_estoqueproduto = true
                                AND c.variedade_cultivo LIKE @nome";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nome", "%" + produtoNome + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EstoqueProduto produto = new EstoqueProduto
                            {
                                Id = reader.GetInt32("id_estoqueproduto"),
                                Qtd = reader.GetInt32("qtd_estoqueproduto"),
                                Unidqtd = reader.GetString("unidqtd_estoqueproduto"),
                                DataEntrada = reader.GetDateTime("dataentrada_estoqueproduto"),
                                StatusAtivo = reader.GetBoolean("ativo_estoqueproduto"),

                                Producao = new Producao
                                {
                                    Id = reader.GetInt32("id_producao"),
                                    Qtd = reader.GetInt32("qtd_producao"),
                                    Unidqtd = reader.GetString("unidqtd_producao"),
                                    AmbienteControlado = reader.GetBoolean("ambientectrl_producao"),
                                    StatusFinalizado = reader.GetBoolean("finalizado_producao"),
                                    Data = reader.GetDateTime("data_producao"),
                                    DataColheita = reader.GetDateTime("datacolheita_producao"),

                                    Cultivo = new Cultivo
                                    {
                                        Id = reader.GetInt32("id_cultivo"),
                                        Nome = reader.GetString("nome_cultivo"),
                                        Variedade = reader.GetString("variedade_cultivo"),
                                        Categoria = reader.GetString("categoria_cultivo"),
                                        TempoProdTradicional = reader.GetInt32("tempoprodtrad_cultivo"),
                                        TempoProdControlado = reader.GetInt32("tempoprodctrl_cultivo"),
                                        StatusAtivo = reader.GetBoolean("ativo_cultivo")
                                    }

                                },
                            };
                            produtos.Add(produto);
                        }
                    }
                }
            }
            return produtos;
        }

        // Método para filtrar produtos pela unidade
        public List<EstoqueProduto> FiltrarProdutosPorUnidade(string unidade)
        {
            List<EstoqueProduto> produtos = new List<EstoqueProduto>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT ep.id_estoqueproduto, ep.qtd_estoqueproduto, ep.unidqtd_estoqueproduto, 
                                ep.dataentrada_estoqueproduto, ep.ativo_estoqueproduto,

                                p.id_producao, p.qtd_producao, p.unidqtd_producao, p.ambientectrl_producao, 
                                p.finalizado_producao, p.data_producao, p.datacolheita_producao,

                                c.id_cultivo, c.nome_cultivo, c.variedade_cultivo, c.categoria_cultivo,
                                c.tempoprodtrad_cultivo, c.tempoprodctrl_cultivo, c.ativo_cultivo

                                FROM estoqueproduto ep
                                LEFT JOIN producao p ON ep.id_producao = p.id_producao 
                                LEFT JOIN cultivo c ON p.id_cultivo = c.id_cultivo 

                                WHERE ep.ativo_estoqueproduto = true
                                AND ep.unidqtd_estoqueproduto LIKE @unidade";

                MySqlCommand command = new MySqlCommand(query, connection);
                {
                    command.Parameters.AddWithValue("@unidade", "%" + unidade + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EstoqueProduto produto = new EstoqueProduto
                            {
                                Id = reader.GetInt32("id_estoqueproduto"),
                                Qtd = reader.GetInt32("qtd_estoqueproduto"),
                                Unidqtd = reader.GetString("unidqtd_estoqueproduto"),
                                DataEntrada = reader.GetDateTime("dataentrada_estoqueproduto"),
                                StatusAtivo = reader.GetBoolean("ativo_estoqueproduto"),

                                Producao = new Producao
                                {
                                    Id = reader.GetInt32("id_producao"),
                                    Qtd = reader.GetInt32("qtd_producao"),
                                    Unidqtd = reader.GetString("unidqtd_producao"),
                                    AmbienteControlado = reader.GetBoolean("ambientectrl_producao"),
                                    StatusFinalizado = reader.GetBoolean("finalizado_producao"),
                                    Data = reader.GetDateTime("data_producao"),
                                    DataColheita = reader.GetDateTime("datacolheita_producao"),

                                    Cultivo = new Cultivo
                                    {
                                        Id = reader.GetInt32("id_cultivo"),
                                        Nome = reader.GetString("nome_cultivo"),
                                        Variedade = reader.GetString("variedade_cultivo"),
                                        Categoria = reader.GetString("categoria_cultivo"),
                                        TempoProdTradicional = reader.GetInt32("tempoprodtrad_cultivo"),
                                        TempoProdControlado = reader.GetInt32("tempoprodctrl_cultivo"),
                                        StatusAtivo = reader.GetBoolean("ativo_cultivo")
                                    }

                                },
                            };
                            produtos.Add(produto);
                        }
                    }
                }
            }
            return produtos;
        }

    }
}
