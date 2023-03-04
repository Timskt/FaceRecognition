using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FaceRecognition.ViewModel;

namespace FaceRecognition;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<MainViewModel>();
    }
}