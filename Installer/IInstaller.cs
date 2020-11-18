using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Installer
{
    interface IInstaller
    {
        void InstallSevices(IConfiguration configuration, IServiceCollection services);
    }
}
