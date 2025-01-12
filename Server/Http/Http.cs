using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http
{
    internal class Http
    {
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
        public Dictionary<HeaderType, string> Headers { get; set; }
        public string Body { get; set; }


        public Http()
        {
            Headers = new Dictionary<HeaderType, string>();
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
            parsedRequest = null;
            try
            {
                //Try to parse start line
                string[] lines = rawRequest.Split(new[] { "\r\n" }, StringSplitOptions.None);
                if(!TryParseRequestStartLine(lines[0], 
                    out string method, out string resource, out string protocol, out string queryString){
                    return false;
                }
                else
                {
                    parsedRequest.Headers.Add(HeaderType.Method, method);
                    parsedRequest.Headers.Add(HeaderType.Protocol, protocol);
                    parsedRequest.Headers.Add(HeaderType.Resource, resource);
                    if(queryString != null)
                    {
                        parsedRequest.Headers.Add(HeaderType.QueryString, queryString);
                    }
                }

                int i = 1;
                int contentLength = int.MinValue;
                //Loop through until the blank line
                while (i < lines.Length && lines[i] != string.Empty)
                {
                    string line = lines[i];
                    
                    int separatorIndex = line.IndexOf(':');
                    if (separatorIndex > 0)
                    {
                        string name = line.Substring(0, separatorIndex).Trim();
                        string value = line.Substring(separatorIndex + 1).Trim();

                        if(!HttpValidation.ValidateHeaderName(name) || !HttpValidation.ValidateHeaderValue(value))
                        {
                            return false;
                        }
                        if(!GetHeaderType(name, out HeaderType headerType))
                        {
                            return false;
                        }
                        if (headerType == HeaderType.ContentLength)
                        {
                            if (!int.TryParse(value, out contentLength))
                            {
                                return false;
                            }
                        }
                        //Date must parse to valid DateTime
                        if (headerType == HeaderType.Date)
                        {
                            if (!DateTime.TryParse(value, out _))
                            {
                                return false;
                            }
                        }

                        parsedRequest.Headers.Add(headerType, value);
                    }
                    else
                    {
                        return false;
                    }

                    i++;
                }

                //Concat all remaining lines into the body property
                parsedRequest.Body = string.Join("\r\n", lines[(i + 1)..]);



                //Final checks for required headers && accurate Content-Length
                if (!RequiredRequestHeaders(parsedRequest.Headers))
                {
                    return false;
                }
                if (parsedRequest.Headers[HeaderType.Method] != "GET")
                {
                    if(contentLength != parsedRequest.Body.Length)
                    {
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
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
            if (!HttpValidation.ValidateRequestStartLineFormat(startLine))
            {
                return false;
            }

            //Split into Method/Resource/Protocol
            string[] split = startLine.Split(' ');
            if (split.Length != 3)
            {
                return false;
            }

            if (!HttpValidation.ValidateMethod(split[0])) {  return false; }
            if (!HttpValidation.ValidateProtocol(split[2])) {  return false; }

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
        #endregion
        #region Responses


        #endregion

    }
}
