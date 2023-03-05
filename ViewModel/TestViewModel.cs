using System.Collections.Generic;
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
    }

    [RelayCommand]
    private void ChangeView()
    {
        RouterHelper.Push("login");
    }

    [RelayCommand]
    private void ChangeChildView()
    {
        var data = (Dictionary<string, object>)RouterHelper.GetRouterData();
        if (data != null && data.Count > 0) MessageBox.Show(data?["test"].ToString());
        RouterHelper.PushChildRouter("three");
        Dictionary<string, object> data2 = new Dictionary<string, object>();
        data2["test2"] = "测认识";
        RouterHelper.SendDataToChild(data2,this);
    }

    protected override bool BeforeChangeChildView()
    {
        // var data = (Dictionary<string, object>)RouterHelper.GetRouterData();
        // if (data != null && data.Count > 0) MessageBox.Show(data?["test"].ToString());
        return true;
    }

    protected override void GetChildSendData(Dictionary<string, object> data)
    {
        base.GetChildSendData(data);
        HandyControl.Controls.MessageBox.Show($"接收到子组件传值{data["test3"]}");
    }
}