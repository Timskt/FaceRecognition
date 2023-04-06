using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace FaceRecognition.ViewModel.BaseViewModel;

public partial class ViewModelBase : ObservableRecipient
{
    protected Dictionary<string, object> _parentData = new();
    
    

    /**
     * 得到父主件发送的数据
     */
    protected virtual void GetParentSendData(Dictionary<string, object> data)
    {
        
    }

    /**
     * 设置父组件发送的数据
     */
    public void SetParentSendData(Dictionary<string, object> data)
    {
        _parentData = data;
        GetParentSendData(data);
    }
    
    public void UnRegister()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}