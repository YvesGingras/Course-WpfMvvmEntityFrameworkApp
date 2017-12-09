using System.Windows;
using Autofac;
using FriendOrganizer.UI.Startup;

namespace FriendOrganizer.UI
{
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e) {
            var bootstrapper = new Bootstrapper();
            var constainer = bootstrapper.Bootstrap();

            var mainWindow = constainer.Resolve<MainWindow>();
            //var mainWindow = new MainWindow(new MainViewModel(new FriendDataService()));

            mainWindow.Show();
        }
    }
}
 