using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Suma2Lealtad.Models
{
    public class Afiliado
    {
        //ENTIDAD Affiliate
        public int id { get; set; }                     // id del Afiliado en SUMAPLAZAS
        public int customerid { get; set; }             // id del Afiliado en CARDS                	 
        public string docnumber { get; set; }           // Documento de Identificación del Afiliado
        public int clientid { get; set; }               // id de Cliente del Afiliado en WEBPLAZAS
        public int storeid { get; set; }                // id de Sucursal de Afiliación
        public int channelid { get; set; }              // id de Canal de Afiliación
        public int typeid { get; set; }                 // id de Tipo de Afiliación
        public string typedelivery { get; set; }        // Tipo de Envío de WEBPLAZAS
        public int? storeiddelivery { get; set; }       // id Sucursal de Envío DE WEBPLAZAS
        public int statusid { get; set; }               // id de Estatus de Afiliación
        public int? reasonsid { get; set; }             // id de Razon de Cambio de Estatus
        public string twitter_account { get; set; }     // cuenta de Twitter
        public string facebook_account { get; set; }    // cuenta de Facebook
        public string instagram_account { get; set; }   // cuenta de Instagram
        public string comments { get; set; }            // Coentarios
        //ENTIDAD CLIENTE
        public string nationality { get; set; }         // Nacionalidad
        public string name { get; set; }                // Primer Nombre
        public string name2 { get; set; }               // Segundo Nombre 
        public string lastname1 { get; set; }           // Primer Apellido
        public string lastname2 { get; set; }           // Segundo Apellido 
        public string birthdate { get; set; }           // Fecha de Nacimiento
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
        //ENTIDAD CustomerInterest
        public List<Interest> Intereses { get; set; }   // Lista de Intereses del Afiliado
        //ENTIDAD Type
        public string type { get; set; }                // Tipo de Afiliación (Suma, Prepago)
        //ENTIDAD Status
        public string estatus { get; set; }             // Estatus de Afiliación
        //ENTIDAD TARJETA
        public string pan { get; set; }                 // Número de Tarjeta
        public string estatustarjeta { get; set; }      // Estatus de la Tarjeta
        public string printed { get; set; }             // Fecha de Impresión de la Tarjeta
        public string trackI { get; set; }              // TrackI de la Tarjeta
        public string trackII { get; set; }             // TrackII de la Tarjeta
        //ENTIDAD Photos_Affiliate 
        public Photos_Affiliate picture { get; set; }   // imagen del Documento de Identificación del Afiliado
        //Entidad CompanyAffiliatte
        public int companyid { get; set; }              // id de compañia a que se asocia el afiliado
        //Campos extras que no se almacenan en Entidades
        public string WebType { get; set; }             // Type de Afiliado en WEBPLAZAS
        public int Monto { get; set; }    

        /* Excepciones */
        //public string exnumber { get; set; }
        //public string exdetail { get; set; }

        //ATRIBUTOS DE LISTAS DE VALORES (DINÁMICOS Y ESTÁTICOS).
        #region Listas_Datos_Geográficos
        public List<ESTADO> ListaEstados { get; set; }
        public List<CIUDAD> ListaCiudades { get; set; }
        public List<MUNICIPIO> ListaMunicipios { get; set; }
        public List<PARROQUIA> ListaParroquias { get; set; }
        public List<URBANIZACION> ListaUrbanizaciones { get; set; }
        #endregion

        #region Lista_Sexo
        public class Sexo
        {
            public int id { get; set; }
            public string sexo { get; set; }
        }


        public IEnumerable<Sexo> SexoOptions =
            new List<Sexo>
        {
              new Sexo { id = 0, sexo = ""          },
              new Sexo { id = 1, sexo = "Masculino" },
              new Sexo { id = 2, sexo = "Femenino"  }
        };
        #endregion

        #region Lista_MaritalStatus
        public class MaritalStatus
        {
            public int id { get; set; }
            public string maritalstatus { get; set; }
        }

        public IEnumerable<MaritalStatus> MaritalStatusOptions =
            new List<MaritalStatus>
        {
              new MaritalStatus { id = 0, maritalstatus = "" },
              new MaritalStatus { id = 1, maritalstatus = "Soltero"     },
              new MaritalStatus { id = 2, maritalstatus = "Casado"      },
              new MaritalStatus { id = 3, maritalstatus = "Divorciado"  },
              new MaritalStatus { id = 4, maritalstatus = "Viudo"       }
        };
        #endregion

        #region Lista_Surcursales
        public class Store
        {
            public string id { get; set; }
            public string sucursal { get; set; }
        }

        public IEnumerable<Store> StoreOptions =
            new List<Store>
        {
            new Store {id = null, sucursal = ""                 },
            new Store {id = "1002", sucursal = "Prados del Este"  },
            new Store {id = "1003", sucursal = "Cafetal"          },
            new Store {id = "1005", sucursal = "Los Samanes"      },
            new Store {id = "1006", sucursal = "Avila"            },
            new Store {id = "1007", sucursal = "Galerías"         },
            new Store {id = "1008", sucursal = "La Lagunita"      },
            new Store {id = "1009", sucursal = "Los Cedros"       },
            new Store {id = "1010", sucursal = "Centro Plaza"     },
            new Store {id = "1011", sucursal = "Vista Alegre"     },
            new Store {id = "1012", sucursal = "Los Naranjos"     },
            new Store {id = "1013", sucursal = "Valle Arriba"     },
            new Store {id = "1014", sucursal = "El Parral"        },
            new Store {id = "1016", sucursal = "Veracruz"         },
            new Store {id = "1017", sucursal = "Los Chaguaramos"  },
            new Store {id = "1018", sucursal = "Guarenas"         },
            new Store {id = "1019", sucursal = "Guatire"          }
        };
        #endregion

        #region Lista_Canales
        public class Channel
        {
            public int id { get; set; }
            public string channel { get; set; }
        }

        public IEnumerable<Channel> ChannelOptions =
            new List<Channel>
        {
              new Channel { id = 0, channel = ""           },
              new Channel { id = 1, channel = "Promotor"   },
              new Channel { id = 2, channel = "Página Web" },
              new Channel { id = 3, channel = "Eventos"    }
        };

        #endregion

        #region Lista_TipoDespacho
        public class TypeDelivery
        {
            public int id { get; set; }
            public string delivery { get; set; }
        }

        public IEnumerable<TypeDelivery> TypeDeliveryOptions =
            new List<TypeDelivery>
        {
              new TypeDelivery { id = 0, delivery = ""                },
              new TypeDelivery { id = 1, delivery = "Retirar en Sucursal" },
              new TypeDelivery { id = 2, delivery = "Envío a Casa"        },
              new TypeDelivery { id = 3, delivery = "Envío a Oficina"     }
        };
        #endregion

    }
}