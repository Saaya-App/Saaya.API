namespace Saaya.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(x =>
            {
                x.UseStartup<Saaya>();

#if !DEBUG
                x.UseUrls("http://0.0.0.0:2251");
#endif
            });
    }
}