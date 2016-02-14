using System.Collections.Generic;
using System.Linq;

namespace Suma2Lealtad.Models
{

    public class UserRols
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public List<CheckBoxes> Roles { get; set; }

        public UserRols() { }

        public UserRols( int id ) {

            using (LealtadEntities db = new LealtadEntities())
            {

                User user = db.Users.Find(id);

                UserID = user.id;
                UserName = user.lastname + ", " + user.firstname;

                Roles = new List<CheckBoxes>();

                foreach (var r in db.Roles)
                {

                    CheckBoxes item = new CheckBoxes();

                    item.RolID = r.id;
                    item.RolName = r.name;
                    item.Checked = db.UserRols.Where(x => x.userid == UserID && x.roleid == r.id).Count() > 0;

                    Roles.Add( item );

                }

            }

        }

        public void Update() 
        {

            using (LealtadEntities db = new LealtadEntities())
            {

                foreach (var m in db.UserRols.Where(f => f.userid == UserID))
                {
                    db.UserRols.Remove(m);
                }

                foreach (var u in Roles.Where(x => x.Checked == true))
                {

                    UserRol ur = new UserRol();

                    ur.userid = UserID;
                    ur.roleid = u.RolID;

                    db.UserRols.Add(ur);

                }

                db.SaveChanges();

            }

        }

    }

    public class CheckBoxes
    {
        public int RolID { get; set; }
        public string RolName { get; set; }
        public bool Checked { get; set; }
    }

}