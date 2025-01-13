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
         * 1. Set up environment variables for PHP
         * . Launch php process
         * . Send body data via stdin
         * . Get output via stdout
         * . Send HTTP request data via stdin
         * . Receive response data from stdout
         */


        public static string RunPhP(string phpFilepath, string queryString, 
            string requestMethod, string contentType, int contentLength, string body)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @"PathToCGI",
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            startInfo.EnvironmentVariables["SCRIPT_FILENAME"] = phpFilepath;
            startInfo.EnvironmentVariables["REQUEST_METHOD"] = requestMethod;
            if(contentType != string.Empty)
            {
            startInfo.EnvironmentVariables["CONTENT_TYPE"] = contentType;
            }
            if(contentLength != 0)
            {
                startInfo.EnvironmentVariables["CONTENT_LENGTH"] = contentLength.ToString();
            }
            if(queryString != string.Empty)
            {
                startInfo.EnvironmentVariables["QUERY_STRING"] = queryString;
            }

            string response = string.Empty;

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();


                using (StreamWriter writer = process.StandardInput)
                {
                    writer.Write(body);
                }

                using (StreamReader reader = process.StandardOutput)
                {
                     response = reader.ReadToEnd();
                }

                process.WaitForExit();

                return response;

            }
        }
    }
}
