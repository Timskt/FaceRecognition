using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using FaceRecognition.Common.Message;
using FaceRecognition.ViewModel.BaseViewModel;
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

    public static void AddRouterByClassType(string name, Type type,bool isInit=false)
    {
        if (Container.Container.IsExist(name)) throw new Exception("不要重复注册同一个路由");

        if (isInit)
        {
            var control = type.GetType();
            var instance = Activator.CreateInstance(control);
            Container.Container.AddData(name, (UserControl)instance);
        }
        else
        {
            //未激活状态
            Container.Container.AddData(name+"RouterByClass", type);
            Container.Container.AddData(name+"RouterByClassFlag", false);
        }
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
        Check(name);
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
        Check(name);
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
    public static void PushChildRouter(ChildViewModelBase parentViewModel,string name, Dictionary<string, object> data = null)
    {
        Check(name);
        UserControl contentControl = null;
        contentControl = (UserControl)Container.Container.GetValue(name);
        if (null == contentControl) throw new Exception($"{name}路由不存在");
        Container.Container.AddData("routerData", data);
        ChildViewContentChange(contentControl, data,parentViewModel.GetClassName());
    }

    /**
     * 跳转非一级路由,生成新页面
     */
    public static void PushChildRouterNoHistory(ChildViewModelBase parentViewModel,string name, Dictionary<string, object> data = null)
    {
        Check(name);
        UserControl contentControl = null;
        contentControl = (UserControl)Container.Container.GetValue(name);
        if (null == contentControl) throw new Exception($"{name}路由不存在");
        Container.Container.AddData("routerData", data);
        var control = contentControl.GetType();
        var instance = Activator.CreateInstance(control);
        Container.Container.AddData(name, instance);
        ChildViewContentChange((UserControl)instance, data,parentViewModel.GetClassName());
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

    private static void ChildViewContentChange(UserControl control, Dictionary<string, object> data,string parentViewModelClassName)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            var messageList = new MessageList();
            messageList.NowMessageList["childView"] = control;
            messageList.NowMessageList["data"] = data;
            var messageModel = new MessageModel(messageList);
            WeakReferenceMessenger.Default.Send<MessageModel, string>(messageModel, "childViewChange"+parentViewModelClassName);
        });
    }

    /**
     * 主动发送数据给子页面
     */
    public static void SendDataToChild(Dictionary<string,object> data,ChildViewModelBase childViewModelBase)
    {
        try
        {
            var viewModel = (ViewModelBase) childViewModelBase.ChildView.DataContext;
            viewModel.SetParentSendData(data);
        }
        catch (Exception e)
        {
            var viewModel = (ChildViewModelBase) childViewModelBase.ChildView.DataContext;
            viewModel.SetParentSendData(data);
        }

    }
    
    /**
     * 子页面发送数据给父页面
     */
    public static void SendDataToParent(Dictionary<string, object> data, string routerName)
    {
        if (!Container.Container.IsExist(routerName)) throw new Exception($"{routerName}路由不存在");
        var parentView = (ChildViewModelBase) ((UserControl)Container.Container.GetValue(routerName)).DataContext;
        parentView.SetChildSendData(data);
    }

    private static void Check(string name)
    {
        if (!Container.Container.IsExist(name))
        {
            if (null == Container.Container.GetValue(name+"RouterByClass")) throw new Exception($"{name}路由不存在");
            if ((bool)Container.Container.GetValue(name+"RouterByClassFlag")==false)
            {
                Type type =(Type) Container.Container.GetValue(name + "RouterByClass");
                var control = type.GetType();
                var instance = Activator.CreateInstance(control);
                Container.Container.AddData(name, (UserControl)instance);
                Container.Container.Update(name + "RouterByClassFlag",true);
            }
        }
    }

    /// <summary>
    /// 注销消息
    /// </summary>
    /// <param name="name"></param>
    public static void UnRegisterMessage(string name)
    {
        Check(name);
        if (!Container.Container.IsExist(name)) throw new Exception($"{name}路由不存在");
        try
        {
           ViewModelBase viewModelBase =(ViewModelBase)Container.Container.GetValue(name);
           viewModelBase.UnRegister();
        }
        catch (Exception e)
        {

        }
        try
        {
            ChildViewModelBase childViewModelBase =(ChildViewModelBase)Container.Container.GetValue(name);
            childViewModelBase.UnRegister();
        }
        catch (Exception e)
        {

        }
    }

    /// <summary>
    /// 根据路由名称得到页面
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static UserControl GetViewByName(string name)
    {
        Check(name);
        if (!Container.Container.IsExist(name)) throw new Exception($"{name}路由不存在");
        return (UserControl) Container.Container.GetValue(name);
    }
}