using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Server.ConfigHandling
{
    public class ConfigLoader
    {
        private ConfigurationManager configManager;        
        
        private TCPConfig? tcpSettings;

        private bool _tcpSuccess;
        public bool TcpSuccess { get { return _tcpSuccess; } }

        public TCPConfig? TcpSettings
        {
            get
            {
                return TcpSuccess ? tcpSettings : null;
            }
        }

        public bool LoadSuccessful { get; }


        private Logging.ILogger _logger;

        public ConfigLoader(Logging.ILogger logger)
        {
            _logger = logger;
            configManager = new ConfigurationManager();
            configManager.Sources.Clear();
        }

        public void LoadConfigSources()
        {
            if (configManager.Sources.Count == 0)
            {
                throw new Exception("Source list is empty.");
                return;
            }
            try
            {
                //Parse sources into objects
                _tcpSuccess = ParseTCPSettings();

                //PHP Stuff

                //Whatever else

            }
            catch (Exception e)
            {
                throw new Exception("Failed to load critical settings: " + e.Message);
            }
        }


        public bool AddJsonSource(string fileName)
        {
            try
            {
                configManager.SetBasePath(Directory.GetCurrentDirectory());
                configManager.AddJsonFile(fileName, optional: false, reloadOnChange: false);
            }
            catch(Exception e)
            {
                configManager.Sources.RemoveAt(configManager.Sources.Count - 1);
                _logger.Err(e.Message);
            }
            return true;
        }

        private bool ParseTCPSettings()
        {

            if (!IPAddress.TryParse(configManager["ListenerSettings:IPAddress"], out IPAddress? ipTemp))
            {
                throw new FormatException($"Invalid IP Address: {ipTemp}");
            }

            if (!int.TryParse(configManager["ListenerSettings:Port"], out int portTemp))
            {
                throw new InvalidCastException("Unable to parse valid integer from ListenerSettings:Port");
            }
            if (portTemp <= 0 || portTemp >= 65535)
            {
                throw new ArgumentOutOfRangeException("TcpSettings.Port", $"Port value of {portTemp} is out of range.");
            }

            return true;
        }
    }
}
