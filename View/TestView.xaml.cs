using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using FaceRecognition.ViewModel;

namespace FaceRecognition.View;

public partial class TestView : UserControl
{
    public TestView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<TestViewModel>();
    }
}