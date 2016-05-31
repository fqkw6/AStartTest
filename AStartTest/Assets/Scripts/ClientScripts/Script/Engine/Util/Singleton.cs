using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    public static T Instance
    {
        get
        {
            if (_applicationIsQuiting) {  // 如果正在退出游戏
                return null;
            }

            lock (_lock) {
                if (_instance == null) {
                    _instance = (T)FindObjectOfType(typeof(T));  // 返回第一个激活的类型为 T 的对象
                    if (FindObjectsOfType(typeof(T)).Length > 1) {  // 返回Type类型的所有激活的加载的物体列表
                        return _instance;
                    }

                    if (_instance == null) {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton)" + typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
                    } else {
                    }
                }

                return _instance;
            }
        }
    }

    private static bool _applicationIsQuiting = false;
    public void OnDestroy()
    {
        _applicationIsQuiting = true;  // 游戏正在退出
    }
};