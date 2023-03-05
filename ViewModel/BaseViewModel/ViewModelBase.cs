using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FaceRecognition.ViewModel.BaseViewModel;

public class ViewModelBase : ObservableRecipient
{
    protected Dictionary<string, object> _routerData = new();
}