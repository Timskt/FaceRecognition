using System.Windows;
using CommunityToolkit.Mvvm.Input;
using FaceRecognition.Common.Router;
using FaceRecognition.ViewModel.BaseViewModel;

namespace FaceRecognition.ViewModel;

public partial class TestViewModel : ChildViewModelBase
{
    public TestViewModel()
    {
        var data = RouterHelper.GetRouterData();
        if (data != null && data.Count > 0)
            MessageBox.Show(data?["test"].ToString());
        MessageBox.Show("调用");
    }

    [RelayCommand]
    private void ChangeView()
    {
        RouterHelper.Push("login");
    }

    [RelayCommand]
    private void ChangeChildView()
    {
        RouterHelper.PushChildRouter("three");
    }

    protected override bool BeforeChangeChildView()
    {
        // var data = (Dictionary<string, object>)RouterHelper.GetRouterData();
        // if (data != null && data.Count > 0) MessageBox.Show(data?["test"].ToString());
        return true;
    }
}