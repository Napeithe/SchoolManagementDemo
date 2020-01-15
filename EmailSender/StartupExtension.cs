using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSender
{
    public static class StartupExtension
    {
        public static IServiceCollection AddEmailService(this IServiceCollection service, IConfiguration configuration)
        {

            service.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            service.AddScoped<ISmtpClient, MySmtpClient>();
            service.AddScoped<IEmailSender, EmailSender>();
            return service;
        }
    }
}
