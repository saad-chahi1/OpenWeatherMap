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
    class DriverMinimal
    {
        private static bool isClosed = false;
        private static Timer timer;
        private static AppServerForDriver appServerForDriver;

        public async Task ConnectToTheServer()
        {
            appServerForDriver = new AppServerForDriver();
            var arguments = Environment.GetCommandLineArgs();

            //ProtocolName@Hostaname

            var arg = arguments[1].Split('@');

            var protocolName = arg[0];
            var hostname = arg.Length > 1 ? arg[1] : "";

            appServerForDriver.Open(hostname);
            appServerForDriver.Login(protocolName);

            var protocolVar = await appServerForDriver.VariableManager.GetVariablesByProtocol(appServerForDriver.CurrentProtocol.Name);
            appServerForDriver.AddFilterNotifications("$V."+ protocolVar[0].Name+ ".Cmd.Ville");

            appServerForDriver.ProtocolSynchronized();
            appServerForDriver.StartNotifications(false);   

            appServerForDriver.ControllerManager.Closed += ControllerManager_closed;
            appServerForDriver.VariableManager.StateChanged += VariableManager_StateChanged;

            Console.WriteLine("Is connected : " + appServerForDriver.IsConnected);
            Console.WriteLine($"Current protocol : {appServerForDriver.CurrentProtocol.Name}");

            while (!isClosed)
            {
                Thread.Sleep(100);
            }
        }

        private static void VariableManager_StateChanged(Prysm.AppVision.Data.VariableState obj)
        {
            Console.WriteLine($"variable : {obj.Name} | state : {obj.Value}");

            ArrayList arguments = new ArrayList();
            arguments.Add(obj);
            arguments.Add(appServerForDriver);

            timer = new Timer(OpenWeatherService.XmlChange, arguments, 1000, Timeout.Infinite);        

        }

        private static void ControllerManager_closed()
        {
            appServerForDriver.Close();
            isClosed = true;
        }  

        
    }
}
