using Suma2Lealtad.Modules;
using System.Collections.Generic;
using System.Linq;

namespace Suma2Lealtad.Models
{

    public class RoleMenus
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public List<CheckMenus> Menus { get; set; }

        public RoleMenus() { }

        public RoleMenus(int id)
        {

            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                Role role = db.Roles.Find(id);

                RoleID = role.id;
                RoleName = role.name;

                Menus = new List<CheckMenus>();

                foreach (var r in db.Menus)
                {

                    CheckMenus item = new CheckMenus();

                    item.MenuId = r.id;
                    item.MenuName = r.name;
                    item.Checked = db.SecurityMenus.Where(x => x.roleid == RoleID && x.menuid == r.id).Count() > 0;

                    Menus.Add(item);

                }

            }

        }

        public void Update()
        {

            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                foreach (var m in db.SecurityMenus.Where(f => f.roleid == RoleID))
                {
                    db.SecurityMenus.Remove(m);
                }

                foreach (var a in Menus.Where(x => x.Checked == true))
                {

                    SecurityMenu item = new SecurityMenu();

                    item.roleid = RoleID;
                    item.menuid = a.MenuId;
                    item.securitylevelid = 1;

                    db.SecurityMenus.Add(item);

                }

                db.SaveChanges();

            }

        }

    }

    public class CheckMenus
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public bool Checked { get; set; }
    }

}