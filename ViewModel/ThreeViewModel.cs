using System.Collections.Generic;
using FaceRecognition.Common.Router;
using FaceRecognition.ViewModel.BaseViewModel;
using HandyControl.Controls;

namespace FaceRecognition.ViewModel;

public class ThreeViewModel : ViewModelBase
{
    protected override void GetParentSendData(Dictionary<string, object> data)
    {
        base.GetParentSendData(data);
        MessageBox.Show($"接收到父组件传递的数据{data["test2"]}");
        Dictionary<string, object> data2 = new Dictionary<string, object>();
        data2["test3"] = "子组件";
        RouterHelper.SendDataToParent(data2,"test");
    }
}