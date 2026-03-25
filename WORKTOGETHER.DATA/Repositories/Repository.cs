using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.DATA.Repositories
{
    // class générique pour les opérations CRUD de base
    public class Repository<T>  where T : class
    {
        // contexte de base de données et DbSet pour l'entité T
        protected WorktogetherContext context;
        protected DbSet<T> table;

        // constructeur qui initialise le contexte et le DbSet
        public Repository()
        {
            context = new WorktogetherContext();
            table = context.Set<T>();
        }

        public void Create(T entity)
        {
            using var ctx = new WorktogetherContext();
            ctx.Set<T>().Add(entity);
            ctx.SaveChanges();
        }

        public void Update(T entity)
        {
            using var ctx = new WorktogetherContext();
            ctx.Set<T>().Update(entity);
            ctx.SaveChanges();
        }

        public void Delete(int id)
        {
            using var ctx = new WorktogetherContext();
            var entity = ctx.Set<T>().Find(id);
            if (entity != null)
            {
                ctx.Set<T>().Remove(entity);
                ctx.SaveChanges();
            }
        }

        public T FindById(int id)
        {
            using var ctx = new WorktogetherContext();
            return ctx.Set<T>().Find(id);
        }

        public List<T> FindAll()
        {
            using var ctx = new WorktogetherContext();
            return ctx.Set<T>().ToList();
        }
    }
}