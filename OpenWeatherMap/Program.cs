using Newtonsoft.Json.Linq;
using Prysm.AppVision.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OpenWeatherMap
{
    class Program
    {
        private static bool isClosed = false;
        private static AppServerForDriver appServerForDriver;

        static async Task Main(string[] args)
        {
            
            appServerForDriver = new AppServerForDriver();
            var arguments = Environment.GetCommandLineArgs();

            //ProtocolName@Hostaname

            var arg = arguments[1].Split('@');

            var protocolName = arg[0];
            var hostname = arg.Length > 1 ? arg[1] : "";

            appServerForDriver.Open(hostname);
            appServerForDriver.Login(protocolName);

            appServerForDriver.AddFilterNotifications("$V.OpenWeatherMap.Cmd.Ville");
            appServerForDriver.ProtocolSynchronized();
            appServerForDriver.StartNotifications(false);

            appServerForDriver.ControllerManager.Closed += ControllerManager_closed;
            appServerForDriver.VariableManager.StateChanged += VariableManager_StateChanged;

            Console.WriteLine("Is connected : " + appServerForDriver.IsConnected);
            Console.WriteLine($"Current protocol : {appServerForDriver.CurrentProtocol.Name}");

            var variables = appServerForDriver.GetVariablesByProtocol();
            foreach (var variable in variables)
            {
                Console.WriteLine("variable : " + variable.Name);
            }

            while (!isClosed)
            {
                Thread.Sleep(100);
            }

        }

        private static void VariableManager_StateChanged(Prysm.AppVision.Data.VariableState obj)
        {    
            Console.WriteLine($"variable : {obj.Name} | state : {obj.Value}");
            var variables = appServerForDriver.GetVariablesByProtocol();
            foreach (var variable in variables)
            {
                Console.WriteLine("variable : " + variable.Name);
            }
            _ = XmlChange(obj);
        }

        private static void ControllerManager_closed()
        {
            appServerForDriver.Close();
            isClosed = true;
        }
        private static async Task XmlChange(Prysm.AppVision.Data.VariableState obj)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/weather");
            var response = await httpClient.GetAsync("?q="+obj.Value+"&APPID=a054a8da56c098c57801d16fb50e3eae&mode=xml&units=metric");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var current = XmlExtension.Deserialize<Current>(content);
                appServerForDriver.VariableManager.Set("OpenWeatherMap.State.Temperature", current.Temperature.Value);
                appServerForDriver.VariableManager.Set("OpenWeatherMap.State.Vitesse", current.Wind.Speed.Value);
                appServerForDriver.VariableManager.Set("OpenWeatherMap.State.Pression", current.Pressure.Value);
                appServerForDriver.VariableManager.Set("OpenWeatherMap.State.Humidite", current.Humidity.Value);
                appServerForDriver.VariableManager.Set("OpenWeatherMap.State.Date_Heure", current.Lastupdate.Value);
            }

        }
       
    }
}
