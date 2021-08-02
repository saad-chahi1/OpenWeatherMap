using Prysm.AppVision.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenWeatherMap
{
    public class OpenWeatherService
    {
        static int FREQ = 10000;

        public static async void XmlChange(Object argument)
        {
            var arguments = (ArrayList)argument;

            var x = (Prysm.AppVision.Data.VariableState)arguments[0];
            var y = (AppServerForDriver)arguments[1];
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/weather");
            var response = await httpClient.GetAsync("?q=" + x.Value + "&APPID=a054a8da56c098c57801d16fb50e3eae&mode=xml&units=metric");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var current = XmlExtension.Deserialize<Current>(content);
                y.VariableManager.Set("OpenWeatherMap.State.Temperature", current.Temperature.Value);
                y.VariableManager.Set("OpenWeatherMap.State.Vitesse", current.Wind.Speed.Value);
                y.VariableManager.Set("OpenWeatherMap.State.Pression", current.Pressure.Value);
                y.VariableManager.Set("OpenWeatherMap.State.Humidite", current.Humidity.Value);
                y.VariableManager.Set("OpenWeatherMap.State.Date_Heure", current.Lastupdate.Value);
            }
            else
            {
                y.AlarmManager.AddAlarm(50, "Ville Incorrect", "Ville incorrecte");
            }

            Timer.Change(FREQ, Timeout.Infinite);

        }
    }
}
