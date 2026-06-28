using SQLite;

namespace FinTrack.Models
{
    [Table("Ativos")]
    public class Ativo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nome { get; set; }

        public double Valor { get; set; }
    }
}