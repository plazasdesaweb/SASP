
namespace Suma2Lealtad.Modules
{
    public class Tarjeta
    {
        public static string ConstruirTrackI(string NroTarjeta) 
        {
            return "B" + NroTarjeta + "^AUTOMERCADOS_PLAZAS^99120000"; //"B99999999999999999^AUTOMERCADOS_PLAZAS^99120000";
        }

        public static string ConstruirTrackII(string NroTarjeta) 
        {
            return NroTarjeta + "=99120000"; //"99999999999999999=99120000";
        }
    }
}