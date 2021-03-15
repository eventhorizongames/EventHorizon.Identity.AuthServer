// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Elasticsearch;

namespace EventHorizon.Identity.AuthServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "EventHorizon.Identity.AuthServer";

            var initDb = args.Contains("/initdb");
            if (initDb)
            {
                args = args.Except(new[] { "/initdb" }).ToArray();
            }

            var host = CreateHostBuilder(args).Build();

            if (initDb)
            {
                AuthDatabase.InitializeDatabase(host.Services, true);
                return;
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.UseSerilog((ctx, cfg) => cfg
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("ApplicationVersion", ctx.Configuration["APPLICATION_VERSION"])
                        .ReadFrom.Configuration(ctx.Configuration)
                        .ConfigureElasticsearchLogging(ctx)
                    );
                }
            );
    }

    public static class SerilogElasticsearchExtensions
    {
        public static LoggerConfiguration ConfigureElasticsearchLogging(
            this LoggerConfiguration loggerConfig,
            WebHostBuilderContext context
        )
        {
            if (context.Configuration.GetValue<bool>("Serilog:Elasticsearch:Enabled"))
            {
                var sinkOptions = new ElasticsearchSinkOptions(
                    new Uri(
                        context.Configuration["Elasticsearch:Uri"]
                    )
                )
                {
                    ModifyConnectionSettings = conn =>
                    {
                        conn.BasicAuthentication(
                            context.Configuration["Elasticsearch:Username"],
                            context.Configuration["Elasticsearch:Password"]
                        );
                        return conn;
                    }

                };
                context.Configuration.GetSection(
                    "Serilog:Elasticsearch"
                ).Bind(
                    sinkOptions
                );
                return loggerConfig.WriteTo.Elasticsearch(
                    sinkOptions
                );
            }

            return loggerConfig;
        }
    }
}