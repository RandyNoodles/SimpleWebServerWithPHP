using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server
{
    public class HTTP
    {

        //Request-specific
        public string Method { get; set; } //GET, POST, etc.
        public string Resource { get; set; }//E.g. /api/data
        public string QueryString { get; set; } //Everything after the ? => /api/data?user=123&sort=asc

        //Both
        public string Protocol { get; set; }//E.g. HTTP/1.1
        
        //Response-specific
        public int ResponseCode { get; set; } //E.g., 200
        public string ReasonPhrase { get; set; } //E.g., "OK"
        public string Server { get; set; } //E.g. Apache/1.3.29
        
        public string ContentType { get; set; } //E.g., "application/json"
        public int ContentLength { get; set; } //Length of body in bytes
        public DateTime Date { get; set; } //Date/Time of the response
        
        //E.g.
        //Cookie: session_id=abc123; theme=dark; user_token=xyz456
        public Dictionary<string, string> Cookies { get; set; }

        //Content
        public string Body { get; set; }

        public HTTP()
        {
            Method = string.Empty;
            Resource = string.Empty;
            QueryString = string.Empty;
            
            Protocol = string.Empty;
            
            ResponseCode = 0;
            ReasonPhrase = string.Empty;

            ContentType = string.Empty;
            ContentLength = 0;
            Date = DateTime.MinValue;

            Cookies = new Dictionary<string, string>();

            Body = string.Empty;
        }

        public void ParseRequest(string rawRequest)
        {
            string[] lines = rawRequest.Split(new[] { "\r\n" }, StringSplitOptions.None);

            //Parse Method, Resource, QueryString, Protocol from start line
            ParseStartLine(lines[0]);

            //Create dictionary for all headers.
            var headers = new Dictionary<string, string>();
            int i = 1;

            //Loop through until the blank line
            while (i < lines.Length && lines[i] != string.Empty)
            {
                string line = lines[i];
                int separatorIndex = line.IndexOf(':');
                if(separatorIndex > 0)
                {
                    string name = line.Substring(0, separatorIndex).Trim();
                    string value = line.Substring(separatorIndex + 1).Trim();
                    headers[name] = value;
                }
                i++;
            }

            //Concat all remaining lines into the body property
            Body = string.Join("\r\n", lines[(i + 1)..]);
        }

        private void ParseStartLine(string startLine)
        {
            //Check that it meets HTTP proper format
            string pattern = @"^[A-Z]+ [^\s]+ HTTP\/\d\.\d$";
            Regex regex = new Regex(pattern);

            if (!regex.IsMatch(startLine))
            {
                throw new InvalidDataException($"Invalid start line format for HTTP Request: {startLine}");
            }

            //Split into Method/Resource/Protocol
            string[] split = startLine.Split(' ');
            if(split.Length != 3)
            {
                throw new InvalidDataException($"Invalid start line format for HTTP Request: {startLine}");
            }


            Method = split[0];
            Protocol = split[2];

            //Split off query string if found
            if (split[1].Contains('?'))
            {
                string[] requestedResource = split[1].Split('?');
                Resource = requestedResource[0];
                QueryString = requestedResource[1];
            }
            else
            {
                Resource = split[1];
            }
        }
    }
}
