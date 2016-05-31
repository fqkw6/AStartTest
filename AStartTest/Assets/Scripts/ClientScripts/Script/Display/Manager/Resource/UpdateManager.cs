using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 版本更新管理
public class UpdateManager : Singleton<UpdateManager>
{
    public Dictionary<string, string> serverFileList = new Dictionary<string, string>();// 服务器文件列表，当需要更新的时候，从服务器获取这个配置
    public Dictionary<string, string> localFileList = new Dictionary<string, string>(); // 本地文件列表，存放的是本地bundle对应的md5，通过配置文件读取

    private int serverVersion = 0;          // 服务器最新版本号
    private int localVersion = 0;           // 本地文件列表的版本号 如果版本号不一致才需要更新

    public List<string> _downloadingList = new List<string>();                // 请求的下载列表

    public delegate void UpdateProgressHandler(int cur, int count);          // 当前更新的进度
    public delegate void UpdateOverHandler();                                // 更新完毕，进入游戏
    public delegate void UpdateResultHandler(bool needUpdate);                // 是否需要更新

    public UpdateOverHandler onUpdateOver;                                   // 主包更新的回调
    public UpdateProgressHandler onUpdateProgress;                           // 更新包下载进度

    // 检测app是否需要更新。可能不需要处理
    public void CheckAppUpdate(UpdateResultHandler callback)
    {
        // 判断app是否需要更新。如91之类的平台可以用其sdk完成
    }

    // 在登录界面显示一个小loading，检测自动更新 在回调函数里面处理需要更新或者不需要更新的情况
    public void CheckUpdate(UpdateResultHandler callback)
    {
        int versionInPackage = 0;       // 在包内的版本号
        int versionInData = 0;          // 缓存目录的版本号

        // 读取程序包内的版本号文件
        TextAsset ta = Resources.Load(GameConfig.VERSION_FILE) as TextAsset;
        if (ta != null) {
            versionInPackage = Convert.ToInt32(ta.text);
        }

        // 读取dataPath下的文件（上次更新后的）
        versionInData = Convert.ToInt32(ReadLocalFile(GameConfig.VERSION_FILE, false));

        // 如果dataPath中的版本更低的话，那就是说安装了新的程序包。这个时候需要清理已下载的旧的资源文件
        if (versionInData > 0 && versionInData < versionInPackage) {
            CleanOldFiles();
        }

        // 本地版本号是程序包和dataPath中较新的那个
        localVersion = Mathf.Max(versionInPackage, versionInData);

        // 下载服务器的版本号文件，判定是否需要更新
        StartCoroutine(DownloadFile(GetFileServerURL() + GameConfig.VERSION_FILE, (WWW w) => {
            if (w != null && (w.error == null || string.IsNullOrEmpty(w.error)) && w.text != null) {
                // 从服务器获取一个版本号
                serverVersion = Convert.ToInt32(w.text);
                if (callback != null) {
                    callback(localVersion < serverVersion);
                }
            } else {
                Debug.Log("没有检查到更新服务器版本信息");
                // 连不上更新，暂时就直接进吧
                if (callback != null) {
                    callback(false);
                }
            }
        }));
    }

    // 读取本地文本文件，如果dataPath中不存在的话（更新后的包），就读取Resources目录下的同名文件（随程序包）
    private string ReadLocalFile(string name, bool readInPackage = true)
    {
        // 读取dataPath下的文件（上次更新后的）
        try {
            StreamReader reader = null;
            reader = File.OpenText(Path.Combine(GameConfig.LOCAL_PATH, name));
            string content = reader.ReadToEnd();
            reader.Close();
            return content;
        } catch (Exception) {
            // 文件不存在
        }

        if (!readInPackage) {
            return "";
        }

        // 读取程序包内的版本号
        TextAsset asset = Resources.Load(name) as TextAsset;
        if (asset) {
            return asset.text;
        } else {
            return "";
        }
    }

    private void WriteFile(string fileName, string content)
    {
        FileStream version = new FileStream(Path.Combine(GameConfig.LOCAL_PATH, fileName), FileMode.OpenOrCreate);
        byte[] data2 = Encoding.UTF8.GetBytes(content);
        version.Write(data2, 0, data2.Length);
        version.Flush();
        version.Close();
    }

    // 删除旧的资源包
    private void CleanOldFiles()
    {
        if (Directory.Exists(GameConfig.LOCAL_PATH)) {
            Directory.Delete(GameConfig.LOCAL_PATH, true);
        }
    }

    // 开始自动更新，这个时候显示的应该是自动更新界面
    public void StartUpdate(UpdateProgressHandler progress, UpdateOverHandler over)
    {
        // 读取本地资源文件列表
        string txt = ReadLocalFile(GameConfig.FILE_LIST);
        ParseFile(txt, localFileList);
        onUpdateProgress = progress;
        onUpdateOver = over;

        // 下载服务器的资源文件列表
        StartCoroutine(DownloadFile(GetFileServerURL() + GameConfig.FILE_LIST, (WWW w) => {
            if (w != null && (w.error == null || string.IsNullOrEmpty(w.error)) && w.text != null) {
                ParseFile(w.text, serverFileList);
                // 比较资源文件列表，如果需要更新的话，则进行更新
                if (CompareVersionList()) {
                    // 使用字符串调用协程以便可以随时停止下载
                    StartCoroutine("StartDownloading");
                } else {
                    // 如果不需要更新的话 直接结束
                    if (onUpdateOver != null) {
                        onUpdateOver();
                    }

                    // 没有发现需要更新的文件，把服务器版本号列表写入本地
                    WriteFile(GameConfig.VERSION_FILE, serverVersion.ToString());
                }
            } else {
                // 没有找到资源列表文件，直接开始游戏
                if (onUpdateOver != null) {
                    onUpdateOver();
                }
            }
        }));
    }

    // 比较文件差异
    private bool CompareVersionList()
    {
        // 遍历所有的服务器列表中的文件，查看本地是否需要下载或者更新
        foreach (var bundle in serverFileList) {
            if (!localFileList.ContainsKey(bundle.Key)) {
                // 新增文件
                _downloadingList.Add(bundle.Key);
                continue;
            }

            string localMd5 = localFileList[bundle.Key];
            if (localMd5 != bundle.Value) {
                // 更新文件
                _downloadingList.Add(bundle.Key);
                continue;
            }
        }
        return _downloadingList.Count > 0;
    }

    // 解析一个资源文件列表，并更新
    private void ParseFile(string text, Dictionary<string, string> dict)
    {
        dict.Clear();
        if (text == null || text.Length == 0) {
            return;
        }

        string[] list = text.Split('\n');
        foreach (var str in list) {
            int index = str.IndexOf('=');

            if (index < 0) {
                continue;
            }

            string key = str.Substring(0, index);
            string value = str.Substring(index + 1, str.Length - 1 - key.Length);

            key = key.Trim();
            value = value.Trim();
            dict[key] = value;
        }
    }

    // 下载一个文件，下载后执行一个回调
    public delegate void DownloadFinishHandler(WWW w);
    private IEnumerator DownloadFile(string url, DownloadFinishHandler callback)
    {
        WWW www = new WWW(url);
        yield return www;
        if (callback != null) {
            callback(www);
        }
        www.Dispose();
    }

    // 下载更新流程
    private IEnumerator StartDownloading()
    {
        int cur = 0;
        int count = _downloadingList.Count;
        foreach (var bundle in _downloadingList) {
            ++cur;
            WWW www = new WWW(GetFileServerURL() + bundle);
            yield return www;

            if (string.IsNullOrEmpty(www.error)) {
                Debug.LogError("url is " + www.url);
                if (onUpdateProgress != null) {
                    onUpdateProgress(cur, count);
                }
                continue;
            }

            Debug.Log(string.Format("save file {0}  {1}/{2}", bundle, cur, count));

            // 将文件保存在本地
            ReplaceLocalFile(bundle, www.bytes);

            // 释放资源
            www.Dispose();

            // 更新进度
            if (onUpdateProgress != null) {
                onUpdateProgress(cur, count);
            }
        }

        // 更新版本号
        StringBuilder builder = new StringBuilder();
        foreach (var item in localFileList) {
            builder.Append(item.Key).Append('=').Append(item.Value).Append('\n');
        }

        // 写入服务器的版本号版本号
        WriteFile(GameConfig.FILE_LIST, builder.ToString());

        _downloadingList.Clear();

        // 更新完毕
        if (onUpdateOver != null) {
            onUpdateOver();
        }
    }

    // 把一个文件写入到本地（下载后的文件写入）
    private void ReplaceLocalFile(string fileName, byte[] data)
    {
        string filePath = Path.Combine(GameConfig.LOCAL_PATH, fileName);
        try {
            string fileDir = filePath.Substring(0, filePath.LastIndexOf('/'));
            if (!System.IO.Directory.Exists(fileDir)) {
                System.IO.Directory.CreateDirectory(fileDir);
            }

            FileStream stream = new FileStream(filePath, FileMode.Create);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();

            // 更新本地列表中的数据
            localFileList[fileName] = serverFileList[fileName];
        } catch (Exception e) {
            Debug.LogException(e);
        }
    }

    // 停止下载（用户取消更新）
    public void StopDownloading()
    {
        StopCoroutine("StartDownloading");
    }

    // 服务器资源路径 包含所有资源和version.txt,不同版本或渠道的url不同
    private static string _fileServerURL = null;
    public static string GetFileServerURL()
    {
        if (_fileServerURL == null) {
#if UNITY_ANDROID
            //_fileServerURL = GetString("file_server_android");
#elif UNITY_IOS
	        //_fileServerURL = GetString("file_server_ios");
#elif UNITY_WP8
	        //_fileServerURL = GetString("file_server_wp8");
#elif UNITY_STANDALONE_WIN
            //_fileServerURL = GetString("file_server_windows");
#else
            //_fileServerURL = GetString("file_server_windows");
#endif
            if (!_fileServerURL.EndsWith("/")) {
                _fileServerURL += "/";
            }

        }

        return _fileServerURL;
    }
}

