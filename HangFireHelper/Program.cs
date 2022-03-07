using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Text;
using EnemyCharsFinder;
using EnemyCharsFinder.Data;
using EnemyCharsFinder.Models;
using Hangfire.Storage;
using AddScrapSesionToDb;

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

            var addScrapSesion = new AddScrapSesion();


            RecurringJob.AddOrUpdate(() => addScrapSesion.Run(), " * * * * *");

            using (var server = new BackgroundJobServer(options))
            {
                Console.WriteLine("Press");
                Console.ReadLine();
            }


        }

        public static void RunRun()
        {
            Decompressor decompressor = new Decompressor();
            decompressor.Decompress();
            StringBuilder builder = new StringBuilder();

            using TibiaArchiveContext context = new TibiaArchiveContext();
        
                decompressor.Names("https://www.tibia.com/community/?subtopic=worlds&world=Vunira", builder);
            
            var scrapSesion = builder.ToString();
            ScrapSesion scrap = new ScrapSesion()
            {
                DatePublished = DateTime.Now,
                //OnlineCharacterNamesNames = scrapSesion
            };
            context.ScrapSesions.Add(scrap);
            context.SaveChanges();
            Console.WriteLine(DateTime.Now);
        }
  
        
    }
}
