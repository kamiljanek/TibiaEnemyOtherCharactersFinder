using Microsoft.EntityFrameworkCore;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace CleanScrapSesionsDb
{
    public static class ClearTable
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }

    }
}
