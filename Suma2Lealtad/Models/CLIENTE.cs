//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Suma2Lealtad.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CLIENTE
    {
        public CLIENTE()
        {
            this.NOMBRE_CLIENTE2 = "(None)";
        }
    
        public string TIPO_DOCUMENTO { get; set; }
        public string NRO_DOCUMENTO { get; set; }
        public string NACIONALIDAD { get; set; }
        public string NOMBRE_CLIENTE1 { get; set; }
        public string NOMBRE_CLIENTE2 { get; set; }
        public string APELLIDO_CLIENTE1 { get; set; }
        public string APELLIDO_CLIENTE2 { get; set; }
        public Nullable<System.DateTime> FECHA_NACIMIENTO { get; set; }
        public string SEXO { get; set; }
        public string EDO_CIVIL { get; set; }
        public string OCUPACION { get; set; }
        public string TELEFONO_HAB { get; set; }
        public string TELEFONO_OFIC { get; set; }
        public string TELEFONO_CEL { get; set; }
        public string E_MAIL { get; set; }
        public Nullable<int> COD_SUCURSAL { get; set; }
        public string COD_ESTADO { get; set; }
        public string COD_CIUDAD { get; set; }
        public string COD_MUNICIPIO { get; set; }
        public string COD_PARROQUIA { get; set; }
        public string COD_URBANIZACION { get; set; }
        public Nullable<System.DateTime> FECHA_CREACION { get; set; }
        public Nullable<int> SEX_ID { get; set; }
        public Nullable<int> STORE_ID { get; set; }
        public Nullable<int> NACIONALITY_ID { get; set; }
        public Nullable<int> CIVIL_STATUS_ID { get; set; }
    
        public virtual Civil_Status Civil_Status { get; set; }
        public virtual Nacionality Nacionality { get; set; }
        public virtual Sex Sex { get; set; }
        public virtual Store Store { get; set; }
    }
}
