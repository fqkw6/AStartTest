using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    public static T Instance
    {
        get
        {
            if (_applicationIsQuiting) {  // ��������˳���Ϸ
                return null;
            }

            lock (_lock) {
                if (_instance == null) {
                    _instance = (T)FindObjectOfType(typeof(T));  // ���ص�һ�����������Ϊ T �Ķ���
                    if (FindObjectsOfType(typeof(T)).Length > 1) {  // ����Type���͵����м���ļ��ص������б�
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
        _applicationIsQuiting = true;  // ��Ϸ�����˳�
    }
};