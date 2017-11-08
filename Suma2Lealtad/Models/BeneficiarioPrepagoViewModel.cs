using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Models
{
    public class BeneficiarioPrepagoViewModel 
    {

        public AfiliadoSuma Afiliado;
        public ClientePrepago Cliente;

        public string numdoc { get; set; }
        public string beneficiario { get; set; }

        [Required(ErrorMessage = "El campo Monto de Transacción es requerido.")]
        [RegularExpression(@"[\d]{1,6}([,][\d]{2})?", ErrorMessage = "El campo Monto de Transacción permite únicamente números en formato decimal : 999999,99")]
        public string monto { get; set; }

        public decimal saldo { get; set; }

        public string documento { get { return numdoc.Replace("V-", "").Replace("J-", "").Replace("E-", "").Replace("G-", "").Replace("P-", ""); } }
        public string montotrx { get { return monto.Replace(",", "").Replace(".", ""); } }

        public string saldoactual { get { return saldo.ToString(); } }

        public string observaciones { get; set;}

        public string storeid { get; set; }

        #region Lista_Surcursales
        public class Store
        {
            public string id { get; set; }
            public string sucursal { get; set; }
        }

        public IEnumerable<Store> StoreOptions =
            new List<Store>
        {
            new Store {id = "0", sucursal = ""                 },
            new Store {id = "1001", sucursal = "Sede Principal Baruta"  },
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


    }
}
