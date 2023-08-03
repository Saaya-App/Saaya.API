using Microsoft.EntityFrameworkCore;
using Saaya.API.Db;
using Saaya.API.Services;
using YoutubeExplode;

namespace Saaya.API
{
    public class Saaya
    {
        public IConfiguration Configuration { get; }

        public Saaya(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMemoryCache();

            services.AddDbContext<ApiContext>(options => options.UseSqlite("Data Source=saaya.db"));
            services.AddScoped<ApiContext>();
            services.AddScoped<LibraryService>();

            services.AddSingleton<YoutubeClient>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(x =>
            {
                x.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=index}/{id?}");
            });

            // Jank
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetRequiredService<ApiContext>();
                db.Database.EnsureCreated();
            }
        }
    }
}