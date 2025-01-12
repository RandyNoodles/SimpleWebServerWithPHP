using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;

namespace Server.Http
{
    
    internal static class HttpValidation
    {

        private static readonly HashSet<string> ValidHeaders = new HashSet<string>
        {
            "Cookie",
            "Server",
            "SetCookie",
            "Protocol",
            "Content-Type",
            "Content-Length",
            "Date",
            "INVALID"
        };
        private static readonly HashSet<string> ValidMethods = new HashSet<string>
        {
            "GET",
            "POST",
            "PUT",
            "DELETE"
        };
        public static bool ValidateHeaderName(string headerName)
        {
            return ValidHeaders.Contains(headerName);
        }
        public static bool ValidateHeaderValue(string headerValue)
        {
            //Check for blank value
            if(string.IsNullOrWhiteSpace(headerValue))
            {
                return false;
            }
            //Check for control chars - e.g. \n
            foreach(char c in headerValue)
            {
                if(char.IsControl(c) && c != '\t')
                {
                    return false;
                }
            }
            return true;
        }
        public static bool ValidateMethod(string method)
        {
            return ValidMethods.Contains(method);
        }
        public static bool ValidateProtocol(string protocol)
        {
            return protocol == "HTTP/1.1";
        }
        public static bool ValidateRequestStartLineFormat(string startLine)
        {
            string pattern = @"^[A-Z]+ [^\s]+ HTTP\/\d\.\d$";
            Regex regex = new Regex(pattern);

            if (!regex.IsMatch(startLine))
            {
                return false;
            }
            return true;
        }

        public static bool Validate 
        
    }
}
