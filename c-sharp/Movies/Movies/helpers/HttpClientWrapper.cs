using Movies.contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace Movies.helpers
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private HttpClient HttpClient = new HttpClient();

        public Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return HttpClient.GetAsync(requestUri);
        }
    }
}
