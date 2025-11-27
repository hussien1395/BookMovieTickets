using BookMovieTickets.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using BookMovieTickets.Utilities;

namespace BookMovieTickets
{
    public static class AppConfiguration 
    {
        public static void Config(this IServiceCollection service,string connectionString)
        {
          
            service.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            service.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            service.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            service.AddTransient<IEmailSender, EmailSender>();
            service.AddScoped<IRepository<Category>, Repository<Category>>();
            //service.AddSingleton<IRepository<Category>, Repository<Category>>();
            //service.AddTransient<IRepository<Category>, Repository<Category>>();
            service.AddTransient<IEmailSender, EmailSender>();
            service.AddScoped<IRepository<Category>, Repository<Category>>();
            service.AddScoped<IRepository<Actor>, Repository<Actor>>();
            service.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            service.AddScoped<IRepository<Movie>, Repository<Movie>>();
            service.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();
            service.AddScoped<IRepository<MovieSubImage>, Repository<MovieSubImage>>();
            service.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            service.AddScoped<IDBInitializer, DBInitializer>();
        }
    }
}
