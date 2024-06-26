using MenuMaster.Data;
using Microsoft.EntityFrameworkCore;

namespace MenuMaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Comando para adicionar meu contexto de banco de dados, pegando minha conex�o
            builder.Services.AddDbContext<RestauranteContext>(o => 
            o.UseSqlServer(builder.Configuration.GetConnectionString("DataBase")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Mesa}/{action=Index}/{id?}");

            app.Run();
        }
    }
}