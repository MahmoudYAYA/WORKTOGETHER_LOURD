using Microsoft.EntityFrameworkCore;
using WORKTOGETHER.DATA.Entities;
using MySql.Data.MySqlClient;

namespace WORKTOGETHER.DATA.Repositories
{
    public class Repository<T> where T : class
    {
        protected readonly WorktogetherContext context;
        protected readonly DbSet<T> table;

        public Repository()
        {
            context = new WorktogetherContext();
            table = context.Set<T>();
        }

        public void Create(T entity)
        {
            table.Add(entity);
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            table.Update(entity);
            context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var entity = table.Find(id);
            if (entity != null)
            {
                table.Remove(entity);
                context.SaveChanges();
            }
        }

        public T FindById(int id)
        {
            return table.Find(id);
        }

        public List<T> FindAll()
        {
            try
            {
                return table.AsNoTracking().ToList();
            }
            catch (MySqlException ex)
            {
                // ← Erreur de connexion MySQL
                throw new Exception("Impossible de se connecter à la base de données. Vérifiez que MySQL est démarré.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur : {ex.Message}");
            }
        }
    }
}