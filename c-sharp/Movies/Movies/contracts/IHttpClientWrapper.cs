using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Movies.contracts
{
    /// <summary>
    /// Wrapper for HttpClient to assist in testing.
    /// Could use the mock HttpMessageHandler approach instead.
    /// </summary>
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(Uri requestUri);
    }
}
