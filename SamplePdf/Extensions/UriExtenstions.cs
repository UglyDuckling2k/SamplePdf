using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace System
{
    public static class UriExtensions
    {
        /// <summary>
        /// Converts the provided app-relative path into an absolute Url containing the 
        /// full host name
        /// </summary>
        /// <param name="relativeUrl">App-Relative path</param>
        /// <returns>Provided relativeUrl parameter as fully qualified Url</returns>
        /// <example>~/path/to/foo to http://www.web.com/path/to/foo</example>
        public static Uri ToAbsolute(this Uri uri)
        {
            if (System.Web.HttpContext.Current == null)
                return uri;

            if (uri.IsAbsoluteUri)
                return uri;

            var relativeUrl = uri.ToString();
            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var domain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            var absoluteUrl = String.Format("{0}{1}", domain, VirtualPathUtility.ToAbsolute(relativeUrl));

            return new Uri(absoluteUrl, UriKind.Absolute);
        }

    }
}