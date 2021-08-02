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

        static void Main(string[] args)
        {
            DriverMinimal DM = new DriverMinimal();
            DM.ConnectToTheServer().Wait();
        }

    }
}
