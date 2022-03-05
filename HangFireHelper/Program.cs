using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Text;
using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;
using Hangfire.Storage;

namespace HangFireHelper
{
    internal class Program
    {
        static void Main(string[] args)
        {

            GlobalConfiguration.Configuration
                         .UseSqlServerStorage(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=HangFireTibia; Integrated Security=True");

            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            var options = new BackgroundJobServerOptions
            {
                SchedulePollingInterval = TimeSpan.FromMilliseconds(1000)
            };

            RecurringJob.AddOrUpdate(() => Run(), "*/2 50-59 * * * *");

            using (var server = new BackgroundJobServer(options))
            {
                Console.WriteLine("Press");
                Console.ReadLine();
            }


        }

        public static void RunRun()
        {
            Console.WriteLine(DateTime.Now);
        }
        public static void Run()
        {
            Decompressor decompressor = new Decompressor();
            decompressor.Decompress();
            StringBuilder builder = new StringBuilder();

            using TibiaArchiveContext context = new TibiaArchiveContext();
            var urls = context.Urls;
            foreach (var item in urls)
            {
                decompressor.Names(item.Adrress, builder);
            }
            var scrapSesion = builder.ToString();
            ScrapSesion scrap = new ScrapSesion()
            {
                DatePublished = DateTime.Now,
                Names = scrapSesion
            };
            context.ScrapSesions.Add(scrap);
            context.SaveChanges();
            Console.WriteLine(DateTime.Now);
        }
    }
}
