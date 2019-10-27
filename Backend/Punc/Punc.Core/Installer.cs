using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Punc.Data;

namespace Punc
{
    public static class ServicesInstaller
    {
        public static IServiceCollection AddPuncServices(this IServiceCollection service)
        {
            service.AddScoped<ITimersRepository, TimersRepository>();

            return service;
        }
    }
}
