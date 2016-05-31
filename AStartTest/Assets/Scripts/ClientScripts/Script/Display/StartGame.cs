using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// unity入口 (额外分一个场景，防止StartGame重复创建)
public class StartGame : MonoBehaviour
{
    public bool _isOffline = false;
    public bool _isDevelop = false;  // 是否是内网
    public string _testRouteServer;

    void Start()
    {
        DontDestroyOnLoad(gameObject);  // 场景切换时 UI 层保留

        Game.Instance.IsOffline = _isOffline;

#if UNITY_EDITOR  // 开发模式
        Game.Instance.IsDevelop = _isDevelop;
#endif

        Game.Instance.TestRouteServer = _testRouteServer;  // IP 地址

        // 初始化游戏逻辑
        Game.Instance.Init();

        // 加载主场景
        SceneManager.LoadScene("new_zc", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.MainCity == null) {
            Game.Instance.ShowMainCity(false);
        }
    }
}
