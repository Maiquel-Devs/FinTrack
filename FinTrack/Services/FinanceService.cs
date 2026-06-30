using SQLite;
using FinTrack.Models;

namespace FinTrack.Services;

public class FinanceService
{
    private readonly SQLiteConnection _db;

    public FinanceService()
    {
        // Cria o arquivo do banco de dados direto no armazenamento local do aparelho
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "fintrack.db3");
        _db = new SQLiteConnection(dbPath);
        _db.CreateTable<Transaction>();
    }

    public List<Transaction> GetAll() => _db.Table<Transaction>().ToList();

    public void Add(string title, decimal amount, bool isExpense)
    {
        _db.Insert(new Transaction { Title = title, Amount = amount, IsExpense = isExpense });
    }

    public void Delete(int id)
    {
        _db.Delete<Transaction>(id);
    }

    public decimal GetTotal(bool isExpense) => _db.Table<Transaction>().Where(t => t.IsExpense == isExpense).ToList().Sum(t => t.Amount);

    public decimal GetBalance() => GetTotal(false) - GetTotal(true);
}