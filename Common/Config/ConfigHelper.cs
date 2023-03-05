using System;
using System.Configuration;

namespace FaceRecognition.Common.Config;

public class ConfigHelper
{
    public static Action<string> SettingChanged;

    /// <summary>
    ///     保存appSetting
    /// </summary>
    /// <param name="key">appSetting的KEY值</param>
    /// <param name="value">appSetting的Value值</param>
    public static void SetAppSetting(string key, object value)
    {
        //静态类的获取方法
        if (typeof(ConfigHelper).GetProperty(key).GetValue(typeof(ConfigHelper)) == value)
            return;

        // 创建配置文件对象
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        if (config.AppSettings.Settings[key] != null)
        {
            // 修改
            config.AppSettings.Settings[key].Value = value.ToString();
        }
        else
        {
            // 添加
            var ass = (AppSettingsSection)config.GetSection("appSettings");
            ass.Settings.Add(key, value.ToString());
        }

        // 保存修改
        config.Save(ConfigurationSaveMode.Modified);

        // 强制重新载入配置文件的连接配置节
        ConfigurationManager.RefreshSection("appSettings");

        //静态类的设置方法
        typeof(ConfigHelper).GetProperty(key).SetValue(typeof(ConfigHelper), value);

        if (SettingChanged != null) SettingChanged(key);
    }

    public static Configuration GetWriteSection(string key, ConfigurationSection section)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        var lastsection = config.Sections[key];
        if (lastsection == null) config.Sections.Add(key, section);

        return config;
    }

    public static void SaveSection(Configuration config)
    {
        config.Save();
    }

    public static ConfigurationSection GetSection(string key)
    {
        try
        {
            return ConfigurationManager.GetSection(key) as ConfigurationSection;
        }
        catch
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.Sections.Remove(key);
            config.Save();
            return null;
        }
    }
}