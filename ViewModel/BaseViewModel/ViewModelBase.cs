using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FaceRecognition.ViewModel.BaseViewModel;

public partial class ViewModelBase : ObservableRecipient
{
    protected Dictionary<string, object> _routerData = new Dictionary<string, object>();
}