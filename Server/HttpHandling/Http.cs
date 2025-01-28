using Server.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server.Logging;


namespace Server.HttpHandling
{
    internal class Http
    {
        //Supported header types
        internal string Method;
        internal string Resource;
        internal string QueryString;
        internal string Protocol;
        internal int StatusCode;
        internal string ReasonPhrase;

        internal int ContentLength;
        internal string ContentType;
        internal string Cookie;
        internal string SetCookie;
        internal string Server;
        internal string Date;

        //Raw dictionary of headers - including unsupported ones
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }

        //Extension to MIME type converter
        private static readonly Dictionary<string, string> ExtensionMimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { ".html", "text/html" },
        { ".htm", "text/html" },
        { ".css", "text/css" },
        { ".js", "application/javascript" },
        { ".json", "application/json" },
        { ".xml", "application/xml" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".webp", "image/webp" },
        { ".txt", "text/plain" },
        { ".pdf", "application/pdf" },
        { ".zip", "application/zip" },
    };

        public Http()
        {
            Method = string.Empty;
            Resource = string.Empty;
            QueryString = string.Empty;
            Protocol = "HTTP/1.1";
            StatusCode = 500; //Default to server error
            ReasonPhrase = "Uninitialized Http object";

            ContentLength = 0;
            ContentType = string.Empty;
            Cookie = string.Empty;
            SetCookie = string.Empty;
            Server = string.Empty;
            Date = string.Empty;

            Headers = new Dictionary<string, string>();
            Body = string.Empty;
        }

        //Utility method to get MIME type from file extension
        public static string GetMimeType(string extension)
        {
            if (ExtensionMimeTypes.TryGetValue(extension, out var mimeType))
            {
                return mimeType;
            }

            //Default to "application/octet-stream" if the extension is not found
            return "application/octet-stream";
        }




    }

}
