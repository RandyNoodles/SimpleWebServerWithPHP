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
        private readonly Logging.ILogger _logger;
        private readonly IConfigurationRoot _config;

        public TCPConfig TcpSettings { get; private set; }


        public ConfigLoader(Logging.ILogger logger, string configFilePath)
        {
            _logger = logger;
            try
            {
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile(configFilePath, optional: false, reloadOnChange: false);

                _config = configBuilder.Build();
                LoadConfig();
            }
            catch (Exception e)
            {
                _logger.Err($"Failed to load configuration: {e.Message}");
                throw;
            }

        }
        private void LoadConfig()
        {
            try
            {
                ParseTCPSettings();
            }
            catch (Exception e)
            {
                _logger.Err($"Error parsing configuration: {e.Message}");
                throw;
            }
        }

        private void ParseTCPSettings()
        {

            if (!IPAddress.TryParse(_config["ListenerSettings:IPAddress"], out IPAddress? ipTemp))
            {
                throw new FormatException($"Invalid IP Address: {ipTemp}");
            }

            if (!int.TryParse(_config["ListenerSettings:Port"], out int portTemp))
            {
                throw new InvalidCastException("Unable to parse valid integer from ListenerSettings:Port");
            }
            if (portTemp <= 0 || portTemp >= 65535)
            {
                throw new ArgumentOutOfRangeException("TcpSettings.Port", $"Port value of {portTemp} is out of range.");
            }
        }
    }
}
