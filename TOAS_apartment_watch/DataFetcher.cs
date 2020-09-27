using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace TOAS_apartment_watch
{
    class DataFetcher
    {
        private static HttpClient _client;
        public DataFetcher()
        {
            _client = new HttpClient();
        }

        public async Task<string> FetchApartments()
        {
            var apartmentsHtml = await _client.GetStringAsync("https://toas.fi/nopeasti-saatavilla/");
            return apartmentsHtml;
        }
    }
}
