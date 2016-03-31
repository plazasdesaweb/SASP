using Suma2Lealtad.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Suma2Lealtad.Modules
{
    public class AppSession
    {
        public class AppRols
        {
            public int roleid { get; set; }
        }

        public class AppUser
        {
            public int id { get; set; }
            public string login { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string status { get; set; }
            public int role { get; set; }
            public string type { get; set; }
            public List<AppRols> Roles { get; set; }
        }

        private IList<CMenu> _menu = new List<CMenu>();
        private string _username = "";
        private string _userlogin = "";
        private string _usertype = "";
        private int _userID;
        //se usa para determinar el nivel de usuario en el rol en prepago, las acciones definidas de momento son: Visualizar = 1, Editar = 2, Aprobar = 3
        private int _rolelevel;
        public int RoleLevel { get { return _rolelevel; } }

        public string Ambiente { get; set; }
        public string Version { get; set; }

        public IList<CMenu> MenuList { get { return _menu; } }
        public string UserName { get { return _username; } }
        public string UserLogin { get { return _userlogin; } }
        public string UserType { get { return _usertype; } }
        public int UserID { get { return _userID; } }        
        public string AppDate { get { var dt = DateTime.Now; return dt.ToString("D", new CultureInfo("es-ES")); } }

        public bool Login(string login, string password)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString();
                var user = db.Users.SingleOrDefault(u => u.login == login && u.passw == password);
                if (user != null)
                {
                    _username = "Usuario: " + user.lastname + ", " + user.firstname;
                    _userlogin = user.login;
                    _usertype = db.Types.SingleOrDefault(t => t.id == user.typeid).name;
                    _userID = user.id;                   
                    int[] roles = (from item in db.UserRols
                                   where item.userid == user.id
                                   select item.roleid
                                   ).ToArray();
                    //se busca el nivel maximo definido en los roles del usuario
                    //Ojo: Los roles están definidos uno por usuario.
                    _rolelevel = (from r in db.Roles
                                  where roles.Contains(r.id)
                                  select r.level).ToList().Max();
                    //Ojo: No se filtra el Menú por Tipo de Usuario, sino por Rol del Usuario. Hay que tener esto en cuenta al definir los roles que sean excluyentes.
                    _menu = (from mainMenu in db.Menus
                             join securityMenu in db.SecurityMenus on mainMenu.id equals securityMenu.menuid
                             where roles.Contains(securityMenu.roleid)
                             select new CMenu
                             {
                                 id = mainMenu.id,
                                 name = mainMenu.name,
                                 controller = mainMenu.controller,
                                 actions = mainMenu.actions,
                                 parentid = mainMenu.parentid,
                                 order_no = mainMenu.order_no
                             }).Distinct().OrderBy(m => m.parentid).ThenBy(m => m.order_no).ToList();
                    _menu.Add(new CMenu { id = 10000, name = "Salir", controller = "", actions = "", parentid = 0, order_no = 1 });
                    _menu.Add(new CMenu { id = 10001, name = "Cerrar Sesión", controller = "Home", actions = "Logout", parentid = 10000, order_no = 2 });
                }
                return (user != null);
            }
        }

    }
}