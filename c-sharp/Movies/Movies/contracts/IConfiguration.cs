using System;

namespace Movies.contracts
{
    public interface IConfiguration
    {
        Uri BaseUri { get; }
    }
}