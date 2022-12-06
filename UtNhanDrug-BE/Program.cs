using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using UtNhanDrug_BE.Configurations;

namespace UtNhanDrug_BE
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            //ScheduleConfig.IntervalInSeconds(22, 1, 100
            //);

            ////Schedule check exp
            //ScheduleConfig.IntervalInDays(22, 44, 1);
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // Port for http & https
                    webBuilder.UseUrls("http://*;https://*");
                });

    }
}
