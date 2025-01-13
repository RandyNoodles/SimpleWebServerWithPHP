using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class PhpCgi
    {
        /* CGI Process
         * 1. Launch php-cgi.exe
         * 2. Set up environment variables for PHP
         * 3. Send HTTP request data via stdin
         * 4. Receive response data from stdout
         * 
         * 
         
         
         */


        public string RunPhP(string requestMethod, string contentType, int contentLength, string body)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @"PathToCGI",
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            startInfo.EnvironmentVariables["REQUEST_METHOD"] = requestMethod;
            startInfo.EnvironmentVariables["CONTENT_TYPE"] = contentType;

            Process cgi = new Process  };
        }
    }
}
