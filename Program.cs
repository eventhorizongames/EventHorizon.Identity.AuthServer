// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using System.Linq;

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

            var host = BuildWebHost(args);

            if (initDb)
            {
                AuthDatabase.InitializeDatabase(host.Services);
                return;
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}