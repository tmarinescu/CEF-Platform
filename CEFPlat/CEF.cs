using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CEFPlat
{
    public class CEF
    {
        private ChromiumWebBrowser _handle;

        public ChromiumWebBrowser Handle
        {
            get
            {
                return _handle;
            }
        }

        //Initialize the browser control
        public bool InitBrowser()
        {
            var settings = new CefSettings();
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "app",
                SchemeHandlerFactory = new CustomProtocolSchemeHandlerFactory(),
                IsSecure = true
            });
            Cef.Initialize(settings);
            return Debugger.DebugFunction(() =>
            {
                _handle = new ChromiumWebBrowser("app://local//index.html");
                _handle.Dock = DockStyle.Fill;
            }, "Creating browser...");
        }

        public class CustomProtocolSchemeHandler : ResourceHandler
        {
            private string _frontendFolderPath;

            public CustomProtocolSchemeHandler()
            {
                _frontendFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web");
            }

            public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
            {
                var uri = new Uri(request.Url);
                var fileName = uri.AbsolutePath;

                var requestedFilePath = _frontendFolderPath + fileName;

                if (File.Exists(requestedFilePath))
                {
                    byte[] bytes = File.ReadAllBytes(requestedFilePath);
                    Stream = new MemoryStream(bytes);

                    var fileExtension = Path.GetExtension(fileName);
                    MimeType = Cef.GetMimeType(fileExtension);

                    callback.Continue();
                    return CefReturnValue.Continue;
                }

                callback.Dispose();
                return CefReturnValue.Cancel;
            }
        }

        public class CustomProtocolSchemeHandlerFactory : ISchemeHandlerFactory
        {
            public const string SchemeName = "app";

            public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
            {
                return new CustomProtocolSchemeHandler();
            }
        }
    }
}
