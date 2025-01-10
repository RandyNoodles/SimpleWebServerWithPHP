using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.ConfigHandling
{
    internal class ConfigManager
    {
        private bool loadSuccessful;
        private IConfiguration? config;
        private TCPConfig? tcpSettings;

        internal TCPConfig? TcpSettings
        {
            get
            {
                return loadSuccessful ? tcpSettings : null;
            }
        }

        internal bool LoadSuccessful { get; }

        internal event Action<string>? WriteLog;
        private void Log(string s)
        {
            WriteLog?.Invoke(s);
        }

        private ConfigManager(string configFilename)
        {
            loadSuccessful = false;

            ConfigurationBuilder b = new ConfigurationBuilder();
            b.SetBasePath(Directory.GetCurrentDirectory());
            b.AddJsonFile(configFilename, optional: false, reloadOnChange: true);
            config = b.Build();
        }

        internal void LoadAppSettings(string configFilename)
        {
            try
            {
                //Parse sources into objects
                tcpSettings = ParseTCPSettings();

                //PHP Stuff

                //Whatever else

            }
            catch (Exception e)
            {
                Log("Failed to load app settings: " + e.Message);
                return;
            }

            loadSuccessful = true;
        }


        internal TCPConfig ParseTCPSettings()
        {

            if (!IPAddress.TryParse(config["ListenerSettings:IPAddress"], out IPAddress? ipTemp))
            {
                throw new FormatException($"Invalid IP Address: {ipTemp}");
            }

            if (!int.TryParse(config["ListenerSettings:Port"], out int portTemp))
            {
                throw new InvalidCastException("Unable to parse valid integer from ListenerSettings:Port");
            }
            if (portTemp <= 0 || portTemp >= 65535)
            {
                throw new ArgumentOutOfRangeException("TcpSettings.Port", $"Port value of {portTemp} is out of range.");
            }

            return new TCPConfig(ipTemp, portTemp);
        }



    }
}
