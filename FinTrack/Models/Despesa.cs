using SQLite;

namespace FinTrack.Models
{
    [Table("Despesas")]
    public class Despesa
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Descricao { get; set; }

        public double Valor { get; set; }
    }
}