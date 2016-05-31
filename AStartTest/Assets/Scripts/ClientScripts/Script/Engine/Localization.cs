using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 语言
public enum Language
{
    none = 0,
    en = 1,     // 英文
    zhCN = 2,   // 中文
    zhTW = 3,   // 繁体
}

// 翻译文本
public class Translation
{
    public static readonly Translation EMPTY = new Translation();

    string _key;

    public Translation()
    {
        _key = "";
    }

    public Translation(string key)
    {
        _key = key;
    }

    public override string ToString()
    {
        return LocalizationManager.Instance.GetText(_key);
    }

    public string ToString(Language lan)
    {
        return LocalizationManager.Instance.GetText(_key, lan);
    }
}

// 本地化管理
public class LocalizationManager
{
    public static readonly LocalizationManager Instance = new LocalizationManager();
    
    public LocalizationManager()
    {
    }

    Language _language = Language.none;
    public Language Language
    {
        get
        {
            if (_language == Language.none) {
                _language = (Language)PlayerPrefs.GetInt("Language", (int)Language.zhCN);
            }
            return _language;
        }

        set
        {
            _language = value;
            if (value != Language.none) {
                PlayerPrefs.SetInt("Language", (int)_language);
                PlayerPrefs.Save();
            }
        }
    }

    // 获取翻译的文本(主要是ui、提示等)
    public string GetText(string key)
    {
        var cfg = TranslationConfigLoader.GetConfig(key);
        if (cfg == null) {
            // 没有配置对应的文本
            return "";
        }

        switch (Language) {
            case Language.en:
                return string.IsNullOrEmpty(cfg.Text_en) ? key : cfg.Text_en;
            case Language.zhCN:
                return string.IsNullOrEmpty(cfg.Text_zhCN) ? key : cfg.Text_zhCN;
            case Language.zhTW:
                return string.IsNullOrEmpty(cfg.Text_zhCN) ? key : cfg.Text_zhCN;
        }

        return "";
    }

    public string GetText(string key, Language language)
    {
        var cfg = TranslationConfigLoader.GetConfig(key);
        if (cfg == null) {
            // 没有配置对应的文本
            return "";
        }

        switch (language) {
            case Language.en:
                return string.IsNullOrEmpty(cfg.Text_en) ? key : cfg.Text_en;
            case Language.zhCN:
                return string.IsNullOrEmpty(cfg.Text_zhCN) ? key : cfg.Text_zhCN;
            case Language.zhTW:
                return string.IsNullOrEmpty(cfg.Text_zhCN) ? key : cfg.Text_zhCN;
        }

        return "";
    }
}

public class Str
{
    public static string Get(string key)
    {
        return LocalizationManager.Instance.GetText(key);
    }

    public static bool Has(string key)
    {
        var cfg = TranslationConfigLoader.GetConfig(key, false);
        return cfg != null;
    }


    public static string Format(string key, params object[] param)
    {
        string text = LocalizationManager.Instance.GetText(key);
        return string.Format(text, param);
    }
}