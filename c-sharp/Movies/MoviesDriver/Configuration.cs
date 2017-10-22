using Movies.contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesDriver
{
    class Configuration : IConfiguration
    {
        public Uri BaseUri => new Uri("https://alintacodingtest.azurewebsites.net");
    }
}
