using System.ComponentModel.DataAnnotations;

namespace RotaViagemAPI.Models
{
    public class Rota
    {
        [Key]
        public int Id { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public decimal Valor { get; set; }
    }
}
