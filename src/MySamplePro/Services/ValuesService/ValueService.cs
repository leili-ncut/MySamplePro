using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MySamplePro.Services
{
    public class ValueService : IValueService
    {
        private readonly HttpClient  _httpClient;

        public ValueService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
