using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const string AppId = "4b0db8c2-9f26-4417-8bde-3f0e3656f8e0";

        private IPublicClientApplication client;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            MyWebView.Close();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (client == null)
            {
                client = PublicClientApplicationBuilder.Create(clientId: AppId)
                    .WithAuthority(authorityUri: "https://login.microsoftonline.com/common")
                    .WithBroker(brokerOptions: new BrokerOptions(BrokerOptions.OperatingSystems.Windows))
                    .Build();
            }

            IntPtr handle = WinRT.Interop.WindowNative.GetWindowHandle(this);

            DispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    AcquireTokenInteractiveParameterBuilder tokenBuilder = client.AcquireTokenInteractive(["https://graph.microsoft.com/.default"])
                        .WithParentActivityOrWindow(handle)
                        .WithUseEmbeddedWebView(false);

                    AuthenticationResult result = await tokenBuilder.ExecuteAsync().ConfigureAwait(false);

                    Debug.WriteLine("Signed in.");
                }
                catch (MsalException ex) when (ex.ErrorCode == "authentication_canceled")
                {
                    Debug.WriteLine($"Sign in canceled.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Sign in failed: {ex.Message}.");
                    throw;
                }
            });
        }
    }
}
