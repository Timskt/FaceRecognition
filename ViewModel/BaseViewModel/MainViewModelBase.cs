using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FaceRecognition.Common.Container;
using FaceRecognition.Common.Message;
using FaceRecognition.Common.Router;
using HandyControl.Tools;

namespace FaceRecognition.ViewModel.BaseViewModel;

/**
 * 主界面需要继承此类
 */
public partial class MainViewModelBase : ObservableRecipient
{
    /**
     * 当前界面
     */
    [ObservableProperty] protected UserControl _nowView;

    /**
     * 接收到的界面
     */
    protected UserControl _receiveView;
    
    protected Dictionary<string, object> _routerData = new Dictionary<string, object>();


    public MainViewModelBase()
    {
        RegisterRouter();
        _nowView = RouterHelper.GetInitView();
    }


    //注册路由
    protected void RegisterRouter()
    {
        WeakReferenceMessenger.Default.Register<MainViewModelBase, MessageModel, string>(this, "viewChange",
            (recipient, message) => { DispatcherHelper.RunOnMainThread(() => { ReceiveRouter(message); }); });
    }


    /**
     * 接收到路由信息
     */
    protected void ReceiveRouter(MessageModel message)
    {
        var messagelist = message.Value;
        var view = (UserControl)messagelist.NowMessageList["nowView"];
        _receiveView = view; 
        _routerData = (Dictionary<string, object>)messagelist.NowMessageList["data"];
        if (BeforeChangeView())
            Application.Current.RunOnUIThread(() => { NowView = view; });
    }

    /**
     * 在改变页面之前
     */
    protected virtual bool BeforeChangeView()
    {
        return true;
    }
}