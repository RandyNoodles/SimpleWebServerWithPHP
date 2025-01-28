using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.HttpHandling
{

    //Static class to take in raw string request and return parsed Http object.
    //Throws an exception with details if error occurs


    internal class RequestParser
    {


        //Overload to accept byte array
        public static Http ParseRequest(byte[] rawRequest)
        {
            string strRequest = Encoding.UTF8.GetString(rawRequest);
            return ParseRequest(strRequest);
        }


        //Main parser function
        public static Http ParseRequest(string rawRequest)
        {

            Http parsedRequest = new Http();
            
            string[] lines = rawRequest.Split(new[] { "\r\n" }, StringSplitOptions.None);

            //Parse start line
            if (!TryParseRequestStartLine(lines[0], parsedRequest))
            {
                throw new Exception($"Invalid start line. {lines[0]}");
            }


            //Check for blank line
            if (!ValidateBlankLine(lines, out int endIndex))
            {
                throw new Exception("Missing blank line in request header.");
            }


            //Grab all headers that match "Name: Value" format
            ParseValidHeaders(lines, endIndex, parsedRequest);

            //Grab content-length, content-type, cookies, etc.
            ParseSupportedHeaders(parsedRequest);


            //Check Content-Length matches body
            if (parsedRequest.Method != "GET")
            {
                parsedRequest.Body = string.Join("\r\n", lines[(endIndex + 1)..]);

                if (parsedRequest.ContentLength != parsedRequest.Body.Length)
                {
                    throw new Exception("Content-Length does not match body of request.");
                }
            }

            return parsedRequest;
        }

        private static void ParseSupportedHeaders(Http parsedRequest)
        {
            if (parsedRequest.Headers.ContainsKey("Content-Length"))
            {
                if(int.TryParse(parsedRequest.Headers["Content-Length"], out int cl))
                {
                    parsedRequest.ContentLength = cl;
                }
            }

            parsedRequest.ContentType = parsedRequest.Headers.TryGetValue("Content-Type", out string contentType)
                ? contentType
                : string.Empty;

            parsedRequest.Cookie = parsedRequest.Headers.TryGetValue("Cookie", out string cookie)
                ? cookie
                : string.Empty;

            parsedRequest.SetCookie = parsedRequest.Headers.TryGetValue("SetCookie", out string sCookie)
                ? sCookie
                : string.Empty;

            parsedRequest.Date = parsedRequest.Headers.TryGetValue("Date", out string Date)
                ? Date
                : string.Empty;
        }
   

        private static void ParseValidHeaders(string[] lines, int endIndex, Http parsedRequest)
        {
            int i = 1; //Start after the start line

            while (i < endIndex)
            {
                string line = lines[i];
                int separatorIndex = line.IndexOf(':');

                //Check for malformed headers -> Silently skip malformed ones
                if (separatorIndex <= 0)
                {
                    i++;
                    continue;
                }

                //Extract name and value
                string name = line.Substring(0, separatorIndex).Trim();
                string value = line.Substring(separatorIndex + 1).Trim();

                //Add to Headers if it fits the Name: Value format
                parsedRequest.Headers[name] = value;

                i++;
            }
        }
        private static bool TryParseRequestStartLine(string startLine, Http parsedRequest)
        {


            //Check that it meets HTTP proper format
            if (!ValidateRequestStartLineFormat(startLine))
            {
                return false;
            }

            //Split into Method/Resource/Protocol
            string[] split = startLine.Split(' ');
            if (split.Length != 3)
            {
                return false;
            }

            if (!ValidateMethod(split[0])) { return false; }
            if (!ValidateProtocol(split[2])) { return false; }

            parsedRequest.Method = split[0];
            parsedRequest.Protocol = split[2];

            //Split off query string if found
            if (split[1].Contains('?'))
            {
                string[] requestedResource = split[1].Split('?');
                parsedRequest.Resource = requestedResource[0];
                parsedRequest.QueryString = requestedResource[1];
            }
            else
            {
                parsedRequest.QueryString = split[1];
            }
            return true;
        }
        private static bool ValidateBlankLine(string[] lines, out int endIndex)
        {
            endIndex = -1;
            for(int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "\r\n")
                {
                    endIndex = i;
                    return true;
                }
            }
            return false;
        }
        public static bool ValidateHeaderValue(string headerValue)
        {
            //Check for blank value
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return false;
            }
            //Check for control chars - e.g. \n
            foreach (char c in headerValue)
            {
                if (char.IsControl(c) && c != '\t')
                {
                    return false;
                }
            }
            return true;
        }
        public static bool ValidateMethod(string method)
        {
            if (method != "GET" && method != "PUT" && method != "POST" && method != "DELETE")
            {
                return false;
            }
            return true;
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
    }
}
