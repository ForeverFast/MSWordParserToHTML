using Microsoft.Extensions.DependencyInjection;
using MSWordParserToHTML.Services;
using MSWordParserToHTML.ViewModels;
using MSWordParserToHTML.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MSWordParserToHTML
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IParserService, ParserService>();
            services.AddSingleton<IDialogService, DialogService>();

            services.AddSingleton(typeof(MainWindowViewModel));
            services.AddSingleton(typeof(MainWindow));
           
        }
    }
}
