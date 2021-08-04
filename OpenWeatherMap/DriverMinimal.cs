using Prysm.AppVision.Data;
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
        private bool _isClosed = false;
        private Timer _timer;
        private AppServerForDriver _appServerForDriver;
        private OpenWeatherService _openWeatherService;
        String _nameVille = "Rabat";
        VariableRow _variableRow;

        public DriverMinimal()
        {
            _appServerForDriver = new AppServerForDriver();
            _openWeatherService = new OpenWeatherService();
        }

        public async Task ConnectToTheServer()
        {
            var arguments = Environment.GetCommandLineArgs();

            //ProtocolName@Hostaname

            var arg = arguments[1].Split('@');

            var protocolName = arg[0];
            var hostname = arg.Length > 1 ? arg[1] : "";

            _appServerForDriver.Open(hostname);
            _appServerForDriver.Login(protocolName);

            _variableRow = (await _appServerForDriver.VariableManager.GetVariablesByProtocol(_appServerForDriver.CurrentProtocol.Name)).First();
            _appServerForDriver.AddFilterNotifications("$V."+ _variableRow.Name+ ".Cmd.Ville");

            _appServerForDriver.ProtocolSynchronized();
            _appServerForDriver.StartNotifications(false);   

            _appServerForDriver.ControllerManager.Closed += ControllerManager_closed;
            _appServerForDriver.VariableManager.StateChanged += VariableManager_StateChanged;

            _timer = new Timer(SyncWeather, null, 0, Timeout.Infinite);

            Console.WriteLine("Is connected : " + _appServerForDriver.IsConnected);
            Console.WriteLine($"Current protocol : {_appServerForDriver.CurrentProtocol.Name}");

            while (!_isClosed)
            {
                Thread.Sleep(100);
            }
        }

        private void VariableManager_StateChanged(Prysm.AppVision.Data.VariableState obj)
        {
            Console.WriteLine($"variable : {obj.Name} | state : {obj.Value}");

             _nameVille = (String) obj.Value;
            SyncWeather(null);

        }

        private void ControllerManager_closed()
        {
            _appServerForDriver.Close();
            _isClosed = true;
        }
        
        public async void SyncWeather(Object obj)
        {
            var current = await _openWeatherService.GetCurrentWeather(_nameVille);

            if (current == null)
            {
                _appServerForDriver.AlarmManager.AddAlarm(50, "Ville Incorrect", "Ville incorrecte");
                
            }
            else
            {
                _appServerForDriver.VariableManager.Set(_variableRow.Name+".State.Temperature", current.Temperature.Value);
                _appServerForDriver.VariableManager.Set(_variableRow.Name+".State.Vitesse", current.Wind.Speed.Value);
                _appServerForDriver.VariableManager.Set(_variableRow.Name+".State.Pression", current.Pressure.Value);
                _appServerForDriver.VariableManager.Set(_variableRow.Name+".State.Humidite", current.Humidity.Value);
                _appServerForDriver.VariableManager.Set(_variableRow.Name+".State.Date_Heure", current.Lastupdate.Value);
            }
            
            _timer.Change(10000, Timeout.Infinite);
        }

        
    }
}
