using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPI.Data;
using TweetAPI.Services;

namespace TweetAPI.Installer
{
    public class DbInstaller : IInstaller
    {
        public void InstallSevices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<ApplicationDataContext>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDataContext>();

            services.AddScoped<IPostService, PostService>();
        }
    }
}
