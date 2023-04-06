using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FaceRecognition.Common.Message;
using FaceRecognition.Common.Router;
using HandyControl.Tools;

namespace FaceRecognition.ViewModel.BaseViewModel;

/**
 * 具有子界面的需要继承此类
 */
public partial class ChildViewModelBase : ObservableRecipient
{
    /**
     * 当前界面
     */
    [ObservableProperty] protected UserControl _childView;
    
    protected Dictionary<string, object> _childData = new();

    /**
     * 接收到的界面
     */
    protected UserControl _receiveView;

    protected Dictionary<string, object> _routerData = new();
    
    protected Dictionary<string, object> _parentData = new();


    public ChildViewModelBase()
    {
        RegisterRouter();
    }


    //注册路由
    protected void RegisterRouter()
    {
        WeakReferenceMessenger.Default.Register<ChildViewModelBase, MessageModel, string>(this, "childViewChange"+GetClassName(),
            (recipient, message) => { DispatcherHelper.RunOnMainThread(() => { ReceiveRouter(message); }); });
    }


    /**
     * 接收到路由信息
     */
    protected void ReceiveRouter(MessageModel message)
    {
        var data = RouterHelper.GetRouterData();
        var messagelist = message.Value;
        var view = (UserControl)messagelist.NowMessageList["childView"];
        _receiveView = view;
        _routerData = (Dictionary<string, object>)messagelist.NowMessageList["data"];
        if (BeforeChangeChildView())
            Application.Current.RunOnUIThread(() =>
            {
                // ChildView = null;
                ChildView = view;
            });
    }

    /**
     * 在改变子页面之前
     */
    protected virtual bool BeforeChangeChildView()
    {
        return true;
    }


    /**
     * 得到子主件发送的数据
     */
    protected virtual void GetChildSendData(Dictionary<string, object> data)
    {
        
    }

    /**
     * 设置子组件发送的数据
     */
    public void SetChildSendData(Dictionary<string, object> data)
    {
        _childData = data;
        GetChildSendData(data);
    }

    public string GetClassName()
    {
        return this.GetType().FullName.Split(".").LastOrDefault();
    }
    
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