
using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using ProjetoLogin.Models;

namespace ProjetoLogin.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly string _connectionString;
        private readonly PasswordHasher<UsuarioModel> _passwordHasher;

        
        public UsuarioRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Connection") ?? string.Empty;
            _passwordHasher = new PasswordHasher<UsuarioModel>();
        }

        // Método assíncrono para cadastrar um novo usuário no banco de dados, retornando verdadeiro se bem-sucedido
        public async Task<bool> Cadastrar(UsuarioModel usuario, string senhaPura)
        {
            try
            {
                // Gera o hash seguro da senha pura informada e o atribui à propriedade SenhaHash do usuário
                usuario.SenhaHash = _passwordHasher.HashPassword(usuario, senhaPura);

                // Cria uma nova conexão com o MySQL utilizando a string de conexão (o 'using' fecha a conexão automaticamente ao fim do bloco)
                using var conexao = new MySqlConnection(_connectionString);
                // Abre a conexão com o banco de dados de forma assíncrona
                await conexao.OpenAsync();

                // Define a instrução SQL de inserção (INSERT) com parâmetros para evitar SQL Injection
                var query = "INSERT INTO usuarios (Nome, Email, SenhaHash) VALUES (@Nome, @Email, @SenhaHash)";
                // Cria o comando SQL associando a query e a conexão aberta
                using var comando = new MySqlCommand(query, conexao);

                // Adiciona os valores dos parâmetros de forma segura aos comandos SQL
                comando.Parameters.AddWithValue("@Nome", usuario.Nome);
                comando.Parameters.AddWithValue("@Email", usuario.Email);
                comando.Parameters.AddWithValue("@SenhaHash", usuario.SenhaHash);

                // Executa o comando no banco e retorna a quantidade de linhas afetadas pela operação
                var linhasAfetadas = await comando.ExecuteNonQueryAsync();
                // Retorna true se pelo menos uma linha foi inserida (cadastro realizado com sucesso), caso contrário false
                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                // Se der erro, ele vai parar aqui e mostrar exatamente o motivo
                throw new Exception($"Erro detalhado no MySQL: {ex.Message}");
            }
        }
        // Método assíncrono para buscar um usuário no banco de dados através do seu e-mail
        public async Task<UsuarioModel?> BuscarEmail(string email)
        {
            // Cria e abre a conexão com o MySQL para realizar a consulta
            using var conexao = new MySqlConnection(_connectionString);
            await conexao.OpenAsync();

            // Define a query SQL para selecionar os dados do usuário com base no e-mail fornecido
            var query = "SELECT Id, Nome, Email, SenhaHash FROM usuarios WHERE Email = @Email";
            // Cria o comando SQL para executar a consulta
            using var comando = new MySqlCommand(query, conexao);
            // Adiciona o parâmetro de e-mail de forma segura
            comando.Parameters.AddWithValue("@Email", email);

            // Executa a leitura dos dados retornados pelo banco de dados
            using var reader = await comando.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // Retorna um novo objeto UsuarioModel populado com os dados vindos do banco
                return new UsuarioModel
                {
                    Id = Convert.ToInt32(reader["Id"]), // Converte com segurança o ID para inteiro
                    Nome = reader["Nome"].ToString(), // Obtém o nome como string
                    Email = reader["Email"].ToString(), // Obtém o e-mail como string
                    SenhaHash = reader["SenhaHash"].ToString() // Obtém o hash da senha como string
                };
            }
            // Se nenhum usuário for encontrado com esse e-mail, retorna null
            return null;
        }
    }
}
