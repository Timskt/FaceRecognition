using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using FaceRecognition.ViewModel;

namespace FaceRecognition.View;

public partial class ThreeView : UserControl
{
    public ThreeView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<ThreeViewModel>();
    }
}