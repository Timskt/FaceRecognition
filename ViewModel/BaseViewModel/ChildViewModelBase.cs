using System.Collections.Generic;
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

    /**
     * 接收到的界面
     */
    protected UserControl _receiveView;

    protected Dictionary<string, object> _routerData = new();


    public ChildViewModelBase()
    {
        RegisterRouter();
    }


    //注册路由
    protected void RegisterRouter()
    {
        WeakReferenceMessenger.Default.Register<ChildViewModelBase, MessageModel, string>(this, "childViewChange",
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
}