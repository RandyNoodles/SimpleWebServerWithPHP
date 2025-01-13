using NUnit.Framework.Internal;
using Server.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Http
{
    
    internal class Http
    {
        struct RequestStartLine
        {
            public RequestStartLine(string method, string resource, string queryString, string protocol)
            {
                Method = method;
                Resource = resource;
                QueryString = queryString;
                Protocol = protocol;
            }
            string Method;
            string Resource;
            string QueryString;
            string Protocol;
        }
        struct ResponseStartLine
        {
            public ResponseStartLine(string protocol, int responseCode, string reasonPhrase, string reasonPhraseExpanded)
            {
                Protocol = protocol;
                ResponseCode = responseCode;
                ReasonPhrase = reasonPhrase;
                ReasonPhraseExpanded = reasonPhraseExpanded;
            }
            string Protocol;
            int ResponseCode;
            string ReasonPhrase;
            string ReasonPhraseExpanded;
        }
        static private ConsoleLogger _logger = new ConsoleLogger();
        public Dictionary<HeaderType, string> Headers { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<string> RejectedHeaders { get; set; }
        public string Body { get; set; }

        private static readonly HashSet<string> SupportedHeaders = new HashSet<string>
        {
            "Content-Length",
            "Content-Type",
            "Cookie",
            "Protocol",
            "SetCookie",
            "Server",
            "Date"
        };
        private static readonly HashSet<string> ValidMethods = new HashSet<string>
        {
            "GET",
            "POST",
            "PUT",
            "DELETE"
        };
        public enum HeaderType
        {
            //Request-Specific
            Method,
            Resource,
            QueryString,
            Cookie,
            //Response-Specific
            ResponseCode,
            ReasonPhrase,
            Server,
            SetCookie,
            //Both
            Protocol,
            ContentType,
            ContentLength,
            Date
        }
     


        public Http()
        {
            Headers = new Dictionary<HeaderType, string>();
            RawHeaders = new Dictionary<string, string>();
            RejectedHeaders = new List<string>();
            Body = string.Empty;
        }

        public static bool GetHeaderType(string headerName, out Http.HeaderType header)
        {
            return Enum.TryParse<HeaderType>(headerName, out header);
        }

        #region Requests

        public static bool TryParseRequest(byte[] rawRequest, out Http parsedRequest)
        {
            string strRequest = Encoding.UTF8.GetString(rawRequest);
            return TryParseRequest(strRequest, out parsedRequest);
        }
        public static bool TryParseRequest(string rawRequest, out Http parsedRequest)
        {
            //Parse StartLine -> If malformed, reject it.
            //Convert %20s to spaces, etc. Check for invalid chars.

            //Check for blank line at the end
            //Parse headers into Name/Value
            //Filter header list by supported headers
            //Check all required headers are present

            //IF NOT GET
            //Check for 



            parsedRequest = null;
            try
            {
                parsedRequest = new Http();

                //Try to parse start line
                string[] lines = rawRequest.Split(new[] { "\r\n" }, StringSplitOptions.None);
                if (!TryParseRequestStartLine(lines[0],
                    out string method, out string resource, out string protocol, out string queryString))
                {
                    _logger.Err($"Invalid start line. {lines[0]}");
                    return false;
                }

                parsedRequest.Headers.Add(HeaderType.Method, method);
                parsedRequest.Headers.Add(HeaderType.Protocol, protocol);
                parsedRequest.Headers.Add(HeaderType.Resource, resource);
                if (!string.IsNullOrEmpty(queryString))
                {
                    parsedRequest.Headers.Add(HeaderType.QueryString, queryString);
                }

                int headerEndIndex = ParseRequestHeaders(lines, parsedRequest);
                if (headerEndIndex == -1)
                {
                    _logger.Err("Header parsing failed.");
                    return false;
                }

                parsedRequest.Body = string.Join("\r\n", lines[(headerEndIndex + 1)..]);

                if (parsedRequest.Headers[HeaderType.Method] != "GET" &&
                int.TryParse(parsedRequest.Headers.GetValueOrDefault(HeaderType.ContentLength, "0"), out int contentLength))
                {
                    if (contentLength != parsedRequest.Body.Length)
                    {
                        _logger.Warn("Content-Length mismatch.");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Err($"Exception during request parsing: {e.Message}");
                return false;
            }
            return true;
        }



        private static int ParseRequestHeaders(string[] lines, Http parsedRequest)
        {
            int i = 1; // Start after the start line

            while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
            {
                string line = lines[i];
                int separatorIndex = line.IndexOf(':');

                // Check for malformed headers
                if (separatorIndex <= 0)
                {
                    _logger.Err($"Malformed header: {line}");
                    parsedRequest.RejectedHeaders.Add(line);
                    i++;
                    continue;
                }

                // Extract name and value
                string name = line.Substring(0, separatorIndex).Trim();
                string value = line.Substring(separatorIndex + 1).Trim();

                // Add to RawHeaders regardless of validity
                parsedRequest.RawHeaders[name] = value;

                // Validate header name and value
                if (!CheckIfHeaderSupported(name) || !ValidateHeaderValue(value))
                {
                    _logger.Err($"Invalid header: {name}");
                    parsedRequest.RejectedHeaders.Add(line);
                }
                else if (GetHeaderType(name, out HeaderType headerType))
                {
                    // Add supported headers to Headers dictionary
                    parsedRequest.Headers[headerType] = value;
                }
                else
                {
                    // Unsupported headers are logged but not added to Headers
                    _logger.Warn($"Unsupported header: {name}");
                }

                i++;
            }

            return i;
        }

        private static bool RequiredRequestHeaders(Dictionary<HeaderType, string> request)
        {
            if(request == null)
            {
                return false;
            }
            if(!request.ContainsKey(HeaderType.Method) || 
                !request.ContainsKey(HeaderType.Protocol) ||
                !request.ContainsKey(HeaderType.Resource))
            {
                return false;
            }
            if (request[HeaderType.Method] != "GET")
            {
                if(!request.ContainsKey(HeaderType.ContentLength)||
                    !request.ContainsKey(HeaderType.ContentType))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool TryParseRequestStartLine(string startLine, 
            out string method, out string resource, out string protocol, out string queryString)
        {
            method = string.Empty;
            resource = string.Empty;
            protocol = string.Empty;
            queryString = string.Empty;

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

            if (!ValidateMethod(split[0])) {  return false; }
            if (!ValidateProtocol(split[2])) {  return false; }

            method = split[0];
            protocol = split[2];

            //Split off query string if found
            if (split[1].Contains('?'))
            {
                string[] requestedResource = split[1].Split('?');
                resource = requestedResource[0];
                queryString = requestedResource[1];
            }
            else
            {
                queryString = split[1];
            }
            return true;
        }

        public static Dictionary<string, string> FilterSupportedHeaders(Dictionary<string, string> headers)
        {
            var filteredHeaders = new Dictionary<string, string>();

            foreach (var header in headers)
            {
                string name = header.Key;
                string value = header.Value;

                //Check if the header is supported
                if (SupportedHeaders.Contains(name))
                {
                    //Validate before adding to filtered list
                    if (ValidateHeaderValue(name))
                    {
                        filteredHeaders[name] = value;
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid header ignored: {name}");
                }
            }

            return filteredHeaders;
        }
        public static bool CheckIfHeaderSupported(string headerName)
        {
            return SupportedHeaders.Contains(headerName);
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




        #endregion
        #region Responses


        #endregion

    }
}
