using Microsoft.EntityFrameworkCore;

namespace CleanDbTable
{
    public static class ClearTable
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }

    }
}
