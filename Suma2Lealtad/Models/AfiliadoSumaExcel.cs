using System;
using System.Collections.Generic;

namespace Suma2Lealtad.Models
{
    public class AfiliadoSumaExcel
    {
        //ENTIDAD Type
        public string type { get; set; }                // Tipo de Afiliación (Suma, Prepago)
        //ENTIDAD Status
        public string estatus { get; set; }             // Estatus de Afiliación        
        //ENTIDAD Affiliate
        public int id { get; set; }                     // id del Afiliado en SUMAPLAZAS
        public string docnumber { get; set; }           // Documento de Identificación del Afiliado
        public int clientid { get; set; }               // id de Cliente del Afiliado en WEBPLAZAS
        public string storeid { get; set; }                // id de Sucursal de Afiliación
        public string channelid { get; set; }              // id de Canal de Afiliación
        public string typedelivery { get; set; }        // Tipo de Envío de WEBPLAZAS
        public string storeiddelivery { get; set; }       // id Sucursal de Envío DE WEBPLAZAS
        public string twitter_account { get; set; }     // cuenta de Twitter
        public string facebook_account { get; set; }    // cuenta de Facebook
        public string instagram_account { get; set; }   // cuenta de Instagram
        //ENTIDAD CLIENTE
        public string nationality { get; set; }         // Nacionalidad
        public string name { get; set; }                // Primer Nombre
        public string name2 { get; set; }               // Segundo Nombre 
        public string lastname1 { get; set; }           // Primer Apellido
        public string lastname2 { get; set; }           // Segundo Apellido 
        public string birthdate { get; set; }           // Fecha de Nacimiento
        public int edad { get; set; }
        public string gender { get; set; }              // Sexo
        public string maritalstatus { get; set; }       // Estado Civil
        public string occupation { get; set; }          // Ocupación
        public string phone1 { get; set; }              // Teléfono Habitación
        public string phone2 { get; set; }              // Teléfono Oficina
        public string phone3 { get; set; }              // Teléfono Celular
        public string email { get; set; }               // Email
        public string cod_estado { get; set; }          // Dirección Codigo Estado
        public string cod_ciudad { get; set; }          // Dirección Codigo Ciudad
        public string cod_municipio { get; set; }       // Dirección Codigo Municipio
        public string cod_parroquia { get; set; }       // Dirección Codigo Parroquia
        public string cod_urbanizacion { get; set; }    // Dirección Codigo Urbanización    
        //ENTIDAD TARJETA
        public string pan { get; set; }                 // Número de Tarjeta
        public string estatustarjeta { get; set; }      // Estatus de la Tarjeta
        public string printed { get; set; }             // Fecha de Impresión de la Tarjeta
        
        public string fechaAfiliacion { get; set; }
        public string usuarioAfiliacion { get; set; }

        //ENTIDAD CustomerInterest
        //public List<Interest> Intereses { get; set; }   // Lista de Intereses del Afiliado
        
        
    }
}