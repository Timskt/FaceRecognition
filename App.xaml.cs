using System;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FaceRecognition.Common.Router;
using FaceRecognition.View;
using FaceRecognition.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace FaceRecognition;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(ConfigureServiceProvider());
        RegisterRouter();
        base.OnStartup(e);
    }


    //依赖注入
    private IServiceProvider ConfigureServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddTransient<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<TestViewModel>();
        services.AddTransient<ThreeViewModel>();
        return services.BuildServiceProvider();
    }

    private void RegisterRouter()
    {
        var loginView = new LoginView();
        RouterHelper.IniView(loginView);
        RouterHelper.AddRouter("login", loginView);
        RouterHelper.AddRouter("test", new TestView());
        RouterHelper.AddRouter("three",new ThreeView());
        // RouterHelper.AddRouter("login",new LoginView());
    }
}