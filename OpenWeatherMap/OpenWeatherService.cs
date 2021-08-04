using Prysm.AppVision.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OpenWeatherMap
{
    public class OpenWeatherService
    {

        public async Task<Current> GetCurrentWeather(String ville)
        {

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/weather");
            var response = await httpClient.GetAsync("?q=" + ville + "&APPID=a054a8da56c098c57801d16fb50e3eae&mode=xml&units=metric");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var current = XmlExtension.Deserialize<Current>(content);

                return current;
                
            }
            return null;

            

        }
    }
}
