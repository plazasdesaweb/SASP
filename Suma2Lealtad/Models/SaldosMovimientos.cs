using System.Collections.Generic;

namespace Suma2Lealtad.Models
{
    public class SaldosMovimientos
    {
        public string DocId { set; get; }        
        public List<Saldo> Saldos { set; get; }
        public List<Movimiento> MovimientosSuma { set; get; }
        public List<Movimiento> MovimientosPrepago { set; get; }
    }
}