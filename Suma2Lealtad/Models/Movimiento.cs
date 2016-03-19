
using System;
namespace Suma2Lealtad.Models
{
    public class Movimiento
    {
        public int batchid { get; set; }
        public string fecha { get; set; }
        public string isodescription { get; set; }
        public string pan { get; set; }
        public string puntos { get; set; }
        public decimal saldo { get; set; }
        public string transcode { get; set; }
        public string transname { get; set; }
        public string b037 { get; set; }
        public string batchtime { get; set; }
        public DateTime fechahora { get; set; }
    }
}
