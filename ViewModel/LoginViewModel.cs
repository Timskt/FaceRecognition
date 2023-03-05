using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using FaceRecognition.Common.Router;
using FaceRecognition.ViewModel.BaseViewModel;

namespace FaceRecognition.ViewModel;

public partial class LoginViewModel : ViewModelBase
{
    [RelayCommand]
    private void ChangeView()
    {
        var dictionary = new Dictionary<string, object>();
        dictionary["test"] = "哈哈哈";
        RouterHelper.PushNoHistory("test", dictionary);
    }
}