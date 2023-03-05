using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using FaceRecognition.Common.Message;
using HandyControl.Tools;

namespace FaceRecognition.Common.Router;

public static class RouterHelper
{
    /**
     * 得到初始化界面
     */
    public static UserControl GetInitView()
    {
        return (UserControl)Container.Container.GetValue("initView");
    }


    /**
     * 加入路由
     */
    public static void AddRouter(string name, UserControl control)
    {
        //   Ioc.Default.GetRequiredService<>()
        Container.Container.AddData(name, control);
    }

    /**
     * 设置开始界面
     */
    public static void IniView(UserControl control)
    {
        if (Container.Container.GetValue("initView") == null)
        {
            Container.Container.AddData("initView", control);
            Container.Container.AddData("routerData", new Dictionary<string, object>());
        }
    }

    /**
     * 跳转一级路由
     */
    public static void Push(string name, Dictionary<string, object> data = null)
    {
        UserControl contentControl = null;
        contentControl = (UserControl)Container.Container.GetValue(name);
        if (null == contentControl) throw new Exception($"{name}一级路由不存在");
        Container.Container.AddData("routerData", data);
        ViewContentChange(contentControl, data);
    }

    /**
     * 跳转一级路由，生成新页面
     */
    public static void PushNoHistory(string name, Dictionary<string, object> data = null)
    {
        UserControl contentControl = null;
        contentControl = (UserControl)Container.Container.GetValue(name);
        if (null == contentControl) throw new Exception($"{name}一级路由不存在");
        // contentControl.GetType().GetMethod("SetDataContext").Invoke()
        Container.Container.AddData("routerData", data);
        var control = contentControl.GetType();
        var instance = Activator.CreateInstance(control);
        Container.Container.AddData(name, instance);
        ViewContentChange((UserControl)instance, data);
    }

    /**
     * 跳转非一级路由
     */
    public static void PushChildRouter(string name, Dictionary<string, object> data = null)
    {
        UserControl contentControl = null;
        contentControl = (UserControl)Container.Container.GetValue(name);
        if (null == contentControl) throw new Exception($"{name}路由不存在");
        Container.Container.AddData("routerData", data);
        ChildViewContentChange(contentControl, data);
    }

    /**
     * 跳转非一级路由,生成新页面
     */
    public static void PushChildRouterNoHistory(string name, Dictionary<string, object> data = null)
    {
        UserControl contentControl = null;
        contentControl = (UserControl)Container.Container.GetValue(name);
        if (null == contentControl) throw new Exception($"{name}路由不存在");
        Container.Container.AddData("routerData", data);
        var control = contentControl.GetType();
        var instance = Activator.CreateInstance(control);
        Container.Container.AddData(name, instance);
        ChildViewContentChange((UserControl)instance, data);
    }

    /**
     * 得到路由跳转传递的参数
     */
    public static Dictionary<string, object> GetRouterData()
    {
        return (Dictionary<string, object>)Container.Container.GetValue("routerData");
    }

    private static void ViewContentChange(UserControl control, Dictionary<string, object> data)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            var messageList = new MessageList();
            messageList.NowMessageList["nowView"] = control;
            messageList.NowMessageList["data"] = data;
            var messageModel = new MessageModel(messageList);
            WeakReferenceMessenger.Default.Send<MessageModel, string>(messageModel, "viewChange");
        });
    }

    private static void ChildViewContentChange(UserControl control, Dictionary<string, object> data)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            var messageList = new MessageList();
            messageList.NowMessageList["childView"] = control;
            messageList.NowMessageList["data"] = data;
            var messageModel = new MessageModel(messageList);
            WeakReferenceMessenger.Default.Send<MessageModel, string>(messageModel, "childViewChange");
        });
    }
}