using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Movies.repositories;
using Movies.contracts;
using Movies.services;
using Movies.helpers;

namespace MoviesDriver
{
    class ContainerConfig
    {
        public static IContainer Container { get; set; }

        public static void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MoviesRepository>().As<IMoviesRepository>();
            builder.RegisterType<MoviesService>().As<IMoviesService>();
            builder.RegisterType<HttpClientWrapper>().As<IHttpClientWrapper>();
            builder.RegisterType<Configuration>().As<IConfiguration>();

            Container = builder.Build();
        }
    }
}
