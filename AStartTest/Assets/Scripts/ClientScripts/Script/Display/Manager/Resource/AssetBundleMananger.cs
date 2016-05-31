using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

// 管理加载assetbundle
public class AssetBundleMananger : Singleton<AssetBundleMananger> {
    public class BundleInfo
    {
        public string bundleName;            // bundle的路径名字，与服务器对应
        public AssetBundle assetBundle;      // 实际的assetbundle

        public float lastUseTime;       // 上次使用的时间
        public bool autoDelete;         // 是否自动清除
        public float autoDeleteTime;    // 自动清除的时间
        public int referenceCount;      // 此bundle的应用计数
    }

    public class ResourceInfo
    {
        public string file;
        public string hash;
        public List<string> deps = new List<string>();
    }

    public class WWWInfo
    {
        public string name = null;
        public WWW www = null;
        public bool isText = false;

        public AssetBundle assetBundle;
        public string text;

        public bool complete = false;

        // 只会在成功加载完毕时候调用一次
        public bool CheckComplete()
        {
            bool ret = !complete && www != null && www.isDone;
            if (ret && www != null) {
                if (isText) {
                    text = www.text;
                } else {
                    assetBundle = www.assetBundle;
                }

                www.Dispose();
                www = null;
                complete = true;
            }
            return ret;
        }

        // 加载是否结束 当不需要加载或者发生错误时均结束加载
        public bool IsComplete()
        {
            return complete || (www != null && (www.isDone || IsError()));
        }

        // 加载是否有错误
        public bool IsError()
        {
            return !string.IsNullOrEmpty(www.error);
        }
    }

    public class TaskInfo
    {
        public bool removeFlag = false;             // 当加载完后，设置为true，以便删除
        public WWWInfo task = null;                 // 主加载任务
        public WWWInfo subTask = null;              // 模型的纹理单独提出来，需要通知逻辑层
        public List<WWWInfo> depTask = new List<WWWInfo>();       // 依赖项 上层不需要关心

        public bool IsComplete()
        {
            // 主任务
            if (task != null && !task.IsComplete()) {
                return false;
            }
            // 子任务
            if (subTask != null && !subTask.IsComplete()) {
                return false;
            }
            // 依赖任务
            foreach (var item in depTask) {
                if (!item.IsComplete()) {
                    return false;
                }
            }
            return true;
        }

        public virtual void OnFinish()
        {
        }
    }

    public class AssetBundleTaskInfo : TaskInfo
    {
        public string bundlePath;
        public OnLoadAssetBundleOverHandler callback = null;
        public override void OnFinish()
        {
            if (task != null && task.assetBundle != null) {
                if (callback != null) {
                    callback(bundlePath, task.assetBundle);
                }
            }
        }
    }

    public class TextTaskInfo : TaskInfo
    {
        public OnLoadTextOverHandler callback = null;  // 逻辑层的回调
        public override void OnFinish()
        {
            if (task != null && task.text != null) {
                if (callback != null) {
                    callback(task.text);
                }
            }
        }
    }

    public class ModelTaskInfo : TaskInfo
    {
        public object userData;
        public bool instantiate = false;
        public OnLoadModelOverHandler callback = null;  // 逻辑层的回调
        public override void OnFinish()
        {
            GameObject goModel = null;
            if (task != null && task.assetBundle != null) {
                string[] names = task.assetBundle.GetAllAssetNames();
                if (names.Length > 0) {
                    goModel = task.assetBundle.LoadAsset<GameObject>(names[0]);
                
                    if (instantiate) {
                        goModel = Instantiate(goModel) as GameObject;
                    }
                }
            }

            Texture texture = null;
            if (subTask != null && subTask.assetBundle != null) {
                object[] allassets = subTask.assetBundle.LoadAllAssets();
                Texture[] allTextures = subTask.assetBundle.LoadAllAssets<Texture>();
                if (allTextures != null && allTextures.Length > 0) {
                    texture = allTextures[0];
                }
            }

            if (callback != null) {
                callback(goModel, texture, userData);
            }
        }
    }

    public class TextureTaskInfo : TaskInfo
    {
        public object userData;
        public GameObject goModel;
        public OnLoadModelOverHandler callback = null;  // 逻辑层的回调
        public override void OnFinish()
        {
            Texture texture = null;
            if (task != null && task.assetBundle != null) {
                string[] names = task.assetBundle.GetAllAssetNames();
                if (names.Length > 0) {
                    texture = task.assetBundle.LoadAsset<Texture>(names[0]);
                }
            }

            if (callback != null) {
                callback(goModel, texture, userData);
            }
        }
    }

    public delegate void OnInitOverHandler();
    public delegate void OnLoadModelOverHandler(GameObject model, Texture texture, object userData); // 一个模型加载完毕，通知上层逻辑
    public delegate void OnLoadAssetBundleOverHandler(string bundlePath, AssetBundle bundle);
    public delegate void OnLoadTextOverHandler(string text);
    public delegate void OnLoadOverHandler(WWW www);                              // 从一个AssetBundle加载一个物体
    public delegate void OnLoadBundleOverHandler(string bundleName, BundleInfo bundle);

    private Dictionary<string, ResourceInfo> _resourceInfos = new Dictionary<string, ResourceInfo>();    // 依赖列表

    // bundle加载队列
    private Dictionary<string, BundleInfo> _loadedBundle = new Dictionary<string, BundleInfo>();      // 已加载的配置文件列表
    private Dictionary<string, List<WWWInfo>> _loadingQueue = new Dictionary<string, List<WWWInfo>>();
    private List<TaskInfo> _taskQueue = new List<TaskInfo>();  // 正在加载的任务
    private bool _initOver = false;
    private bool _initStart = false;
    private OnInitOverHandler onInitOver;

    // 初始化，在游戏一开始的时候调用
    public void Init(OnInitOverHandler callback)
    {
        TextTaskInfo taskInfo = new TextTaskInfo();
        taskInfo.task = CreateWWW(GameConfig.ResourceListPath, true);
        taskInfo.callback = ParseManifest;
        _taskQueue.Add(taskInfo);

        foreach (var item in GameConfig.DefaultAssetBundle) {
            AssetBundleTaskInfo taskInfo2 = new AssetBundleTaskInfo();
            taskInfo2.task = CreateWWW(item, false, true);
            taskInfo2.bundlePath = item + GameConfig.ASSETBUNDLE;
            taskInfo2.callback = AddToBundleCache;
            _taskQueue.Add(taskInfo2);
        }
        onInitOver = callback;
        _initStart = true;
    }

    // 加载一个模型，如人物、怪物、武器模型，所有使用模型的地方都配置其id，在Model表中索引
    public void CreateModel(string modelPath, string texturePath, object userData, OnLoadModelOverHandler callback)
    {
        DoLoadModel(modelPath, texturePath, userData, callback, true);
    }

    // 加载模型但是不实例化，主要用于换装
    public void LoadModel(string modelPath, string texturePath, object userData, OnLoadModelOverHandler callback)
    {
        DoLoadModel(modelPath, texturePath, userData, callback, false);
    }

    public void DoLoadModel(string modelPath, string texturePath, object userData, OnLoadModelOverHandler callback, bool instantiate)
    {
        if (string.IsNullOrEmpty(modelPath)) {
            return;
        }

        if (callback == null) {
            Debug.LogError("Empty Callback when LoadResource");
            return;
        }

#if UNITY_EDITOR || UNITY_ANDROID
        // editor模式下可以优先加载Resources目录下的资源，方便测试。发布版本时要清掉不需要的资源
        GameObject obj = Resources.Load<GameObject>("model/" + modelPath);

        if (obj != null) {
            GameObject goModel;
            if (instantiate) {
                goModel = Instantiate(obj) as GameObject;
            } else {
                goModel = obj;
            }

            if (!string.IsNullOrEmpty(texturePath)) {
                // 额外加载纹理
                Texture tex = Resources.Load<Texture>("model/" + texturePath);
                callback(goModel, null, userData);
            } else {
                // 没有纹理，直接使用
                callback(goModel, null, userData);
            }
            return;
        }
#endif

        ModelTaskInfo taskInfo = new ModelTaskInfo();
        taskInfo.instantiate = instantiate;
        taskInfo.userData = userData;

        // 模型
        taskInfo.task = CreateWWW("model/" + modelPath);

        // 纹理
        if (!string.IsNullOrEmpty(texturePath)) {
            taskInfo.subTask = CreateWWW("texture/" + texturePath);
        }

        // 依赖项
        List<string> deps = new List<string>();
        string key = "model/" + modelPath + GameConfig.ASSETBUNDLE;
        CollectDependencies(key, ref deps);
        if (deps.Count > 0) {
            foreach (var dep in deps) {
                WWWInfo depTask = CreateWWW(dep);
                if (depTask != null) {
                    taskInfo.depTask.Add(depTask);
                }
            }
        }

        // 设置回调
        taskInfo.callback = callback;

        _taskQueue.Add(taskInfo);
    }

    private WWWInfo CreateWWW(string resPath, bool isText = false, bool forceNew = false)
    {
        string path = GetAssetBundlePath(resPath);
        if (string.IsNullOrEmpty(path)) {
            // 没有这个文件
            return null;
        }

        WWWInfo task = new WWWInfo();
        task.name = resPath;
        task.isText = isText;

        if (!task.name.EndsWith(GameConfig.ASSETBUNDLE)) {
            task.name += GameConfig.ASSETBUNDLE;
        }

        BundleInfo bundleInfo;
        if (_loadedBundle.TryGetValue(task.name, out bundleInfo)) {
            // 此包已经加载过了，可以直接使用
            bundleInfo.lastUseTime = Time.time;
            ++bundleInfo.referenceCount;
            task.assetBundle = bundleInfo.assetBundle;
            task.complete = true;
            task.www = null;
        } else {
            List<WWWInfo> list;
            if (!_loadingQueue.TryGetValue(task.name, out list)) {
                // 没有加载
                list = new List<WWWInfo>();
                _loadingQueue[task.name] = list;

                // TODO 检验两种方式的速度和内存占用 WWW.LoadFromCacheOrDownload("", Hash128.Parse(""));
                if (isText || forceNew) {
                    // 文本只能用这种方式加载
                    task.www = new WWW(GetFileURL(path));
                } else {
                    task.www = WWW.LoadFromCacheOrDownload(GetFileURL(path), Hash128.Parse(GetBundleHash(task.name)));
                }
            } else {
                // 正在加载中
                task.www = null;
                list.Add(task);
            }
        }

        return task;
    }

    void Update()
    {
        bool removeFlag = false;
        int count = _taskQueue.Count;
        for (int i = 0; i < count; ++i) {
            TaskInfo taskInfo = _taskQueue[i];
            if (taskInfo.task != null && taskInfo.task.CheckComplete()) {
                AddToBundleCache(taskInfo.task.name, taskInfo.task.assetBundle);
                OnLoadingFinish(taskInfo.task.name, taskInfo.task.assetBundle);
            }

            if (taskInfo.subTask != null && taskInfo.subTask.CheckComplete()) {
                AddToBundleCache(taskInfo.subTask.name, taskInfo.subTask.assetBundle);
                OnLoadingFinish(taskInfo.subTask.name, taskInfo.subTask.assetBundle);
            }

            foreach (var dep in taskInfo.depTask) {
                if (dep.CheckComplete()) {
                    AddToBundleCache(dep.name, dep.assetBundle);
                    OnLoadingFinish(dep.name, dep.assetBundle);
                }
            }

            if (taskInfo.IsComplete()) {
                try {
                    taskInfo.OnFinish();
                } catch (System.Exception e) {
                    Debug.LogException(e);
                } finally {
                    taskInfo.removeFlag = true;
                    removeFlag = true;
                }
            }
        }

        if (removeFlag) {
            _taskQueue.RemoveAll((item) => { return item.removeFlag == true; });
        }

        if (_initStart && !_initOver && _taskQueue.Count <= 0) {
            _initOver = true;
            if (onInitOver != null) {
                onInitOver();
            }
        }
    }

    // 缓存已经加载的bundle
    private void AddToBundleCache(string bundleName, AssetBundle bundle)
    {
        if (string.IsNullOrEmpty(bundleName) || bundle == null) {
            return;
        }

        BundleInfo bundleInfo;
        if (!_loadedBundle.TryGetValue(bundleName, out bundleInfo)) {
            bundleInfo = new BundleInfo();
        }

        bundleInfo.assetBundle = bundle;
        bundleInfo.autoDelete = false;
        bundleInfo.autoDeleteTime = 0;
        bundleInfo.bundleName = bundleName;
        bundleInfo.lastUseTime = Time.time;
        _loadedBundle[bundleName] = bundleInfo;
    }

    private void OnLoadingFinish(string bundleName, AssetBundle bundle)
    {
        List<WWWInfo> list;
        if (_loadingQueue.TryGetValue(bundleName, out list)) {
            foreach (var item in list) {
                if (item.www == null) {
                    item.complete = true;
                    item.assetBundle = bundle;
                }
            }
        }
        _loadingQueue.Remove(bundleName);
    }

    // 卸载bundle
    public void UnloadBundle(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName)) {
            return;
        }

        BundleInfo bundleInfo;
        if (_loadedBundle.TryGetValue(bundleName, out bundleInfo)) {
            --bundleInfo.referenceCount;
            if (bundleInfo.referenceCount <= 0) {
                bundleInfo.assetBundle.Unload(false);
                bundleInfo.assetBundle = null;
                _loadedBundle.Remove(bundleName);
            }
        }
    }

    private void ParseManifest(string text)
    {
        // 加载成功 解析资源列表
        try {
            JsonData jdata = JsonMapper.ToObject(text);

            Dictionary<string, ResourceInfo> cache = new Dictionary<string, ResourceInfo>();

            // 加载现有的资源列表
            int count = jdata.Count;
            for (int i = 0; i < count; ++i) {
                ResourceInfo info = new ResourceInfo();

                JsonData jd = jdata[i];
                info.file = jd["file"].ToString();
                info.hash = jd["hash"].ToString();

                IDictionary jdd = jd as IDictionary;
                if (jdd != null && jdd.Contains("dep")) {
                    JsonData depList = jd["dep"];

                    int depCount = depList.Count;
                    for (int j = 0; j < depCount; ++j) {
                        string dep = depList[j].ToString();
                        if (!info.deps.Contains(dep)) {
                            info.deps.Add(dep);
                        }
                    }
                }

                if (!cache.ContainsKey(info.file)) {
                    _resourceInfos[info.file] = info;
                } else {
                    Debug.LogError("重复的资源项: " + info.file);
                }
            }
        } catch (System.Exception e) {
            Debug.LogException(e);
        } finally {
        }
    }

    // 收集该bundle所有依赖包
    private void CollectDependencies(string bundleName, ref List<string> deps)
    {
        ResourceInfo resInfo = null;
        if (_resourceInfos.TryGetValue(bundleName, out resInfo)) {
            foreach (var item in resInfo.deps) {
                if (!deps.Contains(item)) {
                    deps.Add(item);
                }

                CollectDependencies(item, ref deps);
            }
        }
    }

    private string GetBundleHash(string bundleName)
    {
        ResourceInfo resInfo = null;
        if (_resourceInfos.TryGetValue(bundleName, out resInfo)) {
            return resInfo.hash;
        }
        Debug.LogError("BundleName not found: " + bundleName);
        return "";
    }

    private static string GetFileURL(string path)
    {
#if UNITY_EDITOR
        string url = "file://" + path;
#elif UNITY_ANDROID
		string url = "file://" + path;                  // 同上，url给www读取
#elif UNITY_IPHONE
        string url = path;                              // 同上，url给www读取
#elif UNITY_STANDALONE_WIN
        string url = "file://" + path;                  // 同上，url给www读取
#endif
        return url;
    }

    // 获取下载更新的bundle路径
    private string GetPathInDocument(string filePath)
    {
        return Path.Combine(GetAssetBundlePath(), filePath);
    }

    // 获取在streamasset文件夹下的bundle路径
    private string GetPathInPackage(string filePath)
    {
        return Path.Combine(Application.streamingAssetsPath, filePath);
    }

    // 获取assetbundle的路径
    private string GetAssetBundlePath(string filePath)
    {
        string ext = "";
        if (filePath.IndexOf(".") == -1) {
            ext = GameConfig.ASSETBUNDLE;
        }

        // 获取更新下载目录下的路径
        string path = GetPathInDocument(filePath);
        if (!string.IsNullOrEmpty(ext) && !path.EndsWith(ext)) {
            path += ext;
        }

        if (!File.Exists(path)) {
            // 如果没有则加载安装包内的bundle
            path = GetPathInPackage(filePath);
            if (!string.IsNullOrEmpty(ext) && !path.EndsWith(ext)) {
                path += ext;
            }

            // 都没有，返回空
            if (!File.Exists(path)) {
                return null;
            }
        }

        return path;
    }

    private static string _outputPath = null;
    // assetbundle的输出路径
    public static string GetAssetBundlePath()
    {
        if (_outputPath == null) {
            //_outputPath = GetString("bundle_path");
        }

        if (string.IsNullOrEmpty(_outputPath)) {
            _outputPath = Application.dataPath.Replace("\\", "/");
            _outputPath = _outputPath.Substring(0, _outputPath.IndexOf("/") + 1);
        }

        // 与输出资源的项目路径设置对应
#if UNITY_ANDROID
        string prefix = "AssetsBundle/android/";
#elif UNITY_IOS
        string prefix = "AssetsBundle/ios/";
#elif UNITY_STANDALONE_WIN
        string prefix = "AssetsBundle/windows/";

#else
        string prefix = "AssetsBundle/windows/";
#endif
        string fullPath;
        if (string.IsNullOrEmpty(_outputPath)) {
            fullPath = Path.Combine(GameConfig.LOCAL_PATH, prefix);
        } else {
#if UNITY_EDITOR
            // 编辑器模式下可以配置资源路径
            fullPath = Path.Combine(_outputPath, prefix);
#else
            fullPath = Path.Combine(GameConfig.LOCAL_PATH, prefix);
#endif
        }
        return fullPath;
    }
}
