using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using FaceRecognition.ViewModel;

namespace FaceRecognition.View;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<LoginViewModel>();
    }
}