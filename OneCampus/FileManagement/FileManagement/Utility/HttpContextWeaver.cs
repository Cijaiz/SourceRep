using System;
using System.IO;
using System.Web;

namespace FileManagement.Utility
{
    /// <summary>
    /// THIS IS A HACK
    /// There is a bug in ASP.NET 4.0 in HttpEncoder.Current, which prevents some calls to HttpUtiliy.Decode/Encode
    /// from Application_Start, on IIS or Azure. This hack will be removed when the bug is corrected.
    /// This is fired by the assembly Microsoft.WindowsAzure.StorageClient. Should be corrected in .NET4 SP1
    /// </summary>
    public sealed class HttpContextWeaver : IDisposable
    {
        private readonly HttpContext _current;

        public HttpContextWeaver()
        {
            _current = HttpContext.Current;
            HttpContext.Current = new HttpContext(new HttpRequest(String.Empty, "http://localhost", String.Empty), new HttpResponse(new StringWriter()));
        }

        public void Dispose()
        {
            HttpContext.Current = _current;
        }
    }
}

