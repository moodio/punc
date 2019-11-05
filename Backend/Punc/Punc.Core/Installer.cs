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
            
            service.AddScoped<StripeService, StripeService>();
            service.AddScoped<ITimersService, TimersService>();
            service.AddScoped<IRefereeCodesService, RefereeCodesService>();
            service.AddScoped<IEmailService, AwsEmailService>();
            service.AddScoped<ITimersRepository, TimersRepository>();
            service.AddScoped<IRefereeCodesRepository, RefereeCodesRepository>();
            service.AddScoped<ITimerEmailsService, TimerEmailsService>();
            service.AddScoped<ISubscribersRepository, SubscribersRepository>();
            service.AddScoped<ISubscribersService, SubscribersService>();

            return service;
        }
    }
}
