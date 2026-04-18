using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.DATA.Repositories
{
    public class UserRepository : Repository<User>
    {
        public User FindByEmail(string email )
        {
            return table.FirstOrDefault(u => u.Email == email);
        }


        public List<User> FindClientAdtifs()
        {
            return table
                .Where(u => u.Actif == 1 && u.Roles.Contains("ROLE_CLIENT"))
                .ToList();
        }

        // Recherche d'utilisateurs actifs par rôle 
        public List<User> FindByRole(string role)
        {
            return table
                .ToList()
                .Where(u => u.Actif == 1 && u.Roles.Contains(role))
                .ToList();
        }

        // lister tous les utilisateurs actifs
        public List<User> FindAll()
        {
            return table
                .ToList();
        }

        public void ToggleActif(int id)
        {
            using var ctx = new WorktogetherContext();
            var user = ctx.Users.Find(id);
            if (user != null)
            {
                user.Actif = user.Actif == 1 ? (sbyte)0 : (sbyte)1;
                ctx.SaveChanges();
            }
        }

    }

}