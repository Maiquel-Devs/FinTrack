using SQLite;
using FinTrack.Models;

namespace FinTrack.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _connection;

        // Método para inicializar o banco se ele não existir
        private async Task InitAsync()
        {
            if (_connection != null)
                return;

            // Define o caminho físico do arquivo .db no sistema operacional do celular
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "fintrack.db3");

            // Abre a conexão com o arquivo
            _connection = new SQLiteAsyncConnection(dbPath);

            // Cria as tabelas de forma assíncrona se elas não existirem no arquivo
            await _connection.CreateTableAsync<Ativo>();
            await _connection.CreateTableAsync<Despesa>();
        }

        #region MÉTODOS DE ATIVOS

        public async Task<List<Ativo>> GetAtivosAsync()
        {
            await InitAsync();
            return await _connection.Table<Ativo>().ToListAsync();
        }

        public async Task<int> SalvarAtivoAsync(Ativo ativo)
        {
            await InitAsync();
            return await _connection.InsertAsync(ativo);
        }

        #endregion

        #region MÉTODOS DE DESPESAS

        public async Task<List<Despesa>> GetDespesasAsync()
        {
            await InitAsync();
            return await _connection.Table<Despesa>().ToListAsync();
        }

        public async Task<int> SalvarDespesaAsync(Despesa despesa)
        {
            await InitAsync();
            return await _connection.InsertAsync(despesa);
        }

        #endregion
    }
}