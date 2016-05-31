using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Utils
{
    public static Transform FindChild(Transform t, string name)
    {
        if (t.name == name) {
            return t;
        }

        foreach (Transform child in t) {
            Transform ct = FindChild(child, name);
            if (ct != null) {
                return ct;
            }
        }
        return null;
    }

    public static void SetLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;

        foreach (Transform tran in root) {
            SetLayer(tran, layer);
        }
    }

    public static T GetComponent<T>(string name) where T : Component
    {
        GameObject go = GameObject.Find(name);
        return go ? go.GetComponent<T>() : null;
    }

    public static T GetComponent<T>(GameObject go) where T : Component
    {
        Transform tran = go.transform;
        do {
            T t = tran.GetComponent<T>();
            if (t != null) {
                return t;
            }
            tran = tran.parent;
        } while (tran != null);

        return null;
    }

    public static T CheckOrAddComponent<T>(GameObject go) where T : MonoBehaviour
    {
        T component = go.GetComponent<T>();
        if (component == null) {
            component = go.AddComponent<T>();
        }

        return component;
    }

    public static string GetGuid()
    {
        return SystemInfo.deviceUniqueIdentifier.ToString();
    }

    public static void DisplayTypeAndAddress()
    {
        var allNetworkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        Debug.Log(allNetworkInterfaces.Length + " nics");
        var array = allNetworkInterfaces;
        for (int i = 0; i < array.Length; i++) {
            var networkInterface = array[i];
            var iPProperties = networkInterface.GetIPProperties();
            Debug.Log(networkInterface.Description);
            Debug.Log(string.Empty.PadLeft(networkInterface.Description.Length, '='));
            Debug.Log("  Interface type .......................... : " + networkInterface.NetworkInterfaceType);
            Debug.Log("  Physical Address ........................ : " + networkInterface.GetPhysicalAddress().ToString());
            Debug.Log("  Is receive only.......................... : " + networkInterface.IsReceiveOnly);
            Debug.Log("  Multicast................................ : " + networkInterface.SupportsMulticast);
        }
    }

    public static string GetMacAddress()
    {
        string str = string.Empty;
        var allNetworkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        var array = allNetworkInterfaces;
        for (int i = 0; i < array.Length; i++) {
            var networkInterface = array[i];
            var physicalAddress = networkInterface.GetPhysicalAddress();
            if (physicalAddress.ToString() != string.Empty) {
                str = physicalAddress.ToString();
            }
        }
        Debug.Log("Mac Address: " + str);
        return string.Empty;
    }

    // 获取当前坐标下的地面坐标
    public static Vector3 GetGroundLandPos(Vector3 pos)
    {
        float distance = 1000f;
        RaycastHit[] array = Physics.RaycastAll(pos + new Vector3(0f, 100f, 0f), Vector3.down, distance, GameLayer.GroundMask);
        List<Vector3> ret = new List<Vector3>();
        foreach (RaycastHit raycastHit in array) {
            ret.Add(raycastHit.point);
        }

        if (ret.Count <= 0) {
            Debug.Log(">>> Can't Find GroundPos At: " + pos);
            return pos;
        }

        ret.Sort((Vector3 left, Vector3 right) =>
        {
            if (left.y < right.y) {
                return -1;
            } else if (left.y > right.y) {
                return 1;
            } else {
                return 0;
            }
        });

        return ret[ret.Count - 1];
    }

    public static List<T> RandomSortList<T>(List<T> ListT)
    {
        System.Random random = new System.Random();
        List<T> newList = new List<T>();
        foreach (T item in ListT) {
            newList.Insert(random.Next(newList.Count + 1), item);
        }
        return newList;
    }

    // 根据屏幕坐标获取点到地面上面的位置
    public static Vector3 GetPositionFromTouch(float x, float y)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, GameLayer.GroundMask)) {
            return hit.point;
        }

        return Vector3.zero;
    }

    // 将毫秒时间转换为秒
    public static int GetSeconds(long ms)
    {
        return Mathf.CeilToInt(ms / 1000.0f);
    }

    // 获取倒计时，格式为 00:00:00
    public static string GetCountDownTime(float time, bool forceShowHour = false)
    {
        int countdown = (int)Mathf.Max(time, 0);
        if (countdown >= 3600 || forceShowHour) {
            int minsec = countdown % 3600;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", countdown / 3600, minsec / 60, minsec % 60);
        } else {
            return string.Format("{0:D2}:{1:D2}", countdown / 60, countdown % 60);
        }
    }

    // 获取倒计时，格式为xx天xx时xx分xx秒，如果shortFormat为true，则只显示前两个有效时间单位
    public static string GetCountDownString(float time, bool shortFormat = true)
    {
        int countdown = (int)Mathf.Max(time, 0);
        int day = countdown / (3600 * 24);
        int hour = (countdown % (3600 * 24)) / 3600;
        int min = (countdown % 3600) / 60;
        int sec = (countdown % 3600) % 60;

        StringBuilder sb = new StringBuilder();

        // xx天
        if (day > 0) {
            sb.AppendFormat("{0}{1}", day, Str.Get("UI_TIME_FORMAT_DAY"));
        }
        
        // xx小时
        if (hour > 0) {
            sb.AppendFormat("{0}{1}", hour, Str.Get("UI_TIME_FORMAT_HOUR"));
            // 显示天和小时
            if (shortFormat && day > 0) return sb.ToString();
        }

        // xx分钟
        if (min > 0) {
            sb.AppendFormat("{0}{1}", min, Str.Get("UI_TIME_FORMAT_MINUTE"));

            // 显示小时和分钟
            if (shortFormat && hour > 0) return sb.ToString();
        }

        // xx秒
        if (sec > 0) {
            sb.AppendFormat("{0}{1}", sec, Str.Get("UI_TIME_FORMAT_SECOND"));
            if (shortFormat && min > 0) return sb.ToString();
        }
        return sb.ToString();
    }

    // 获取金钱格式化文字
    public static string GetMoneyString(long value)
    {
//        if (value > 10000) {
//            return string.Format(Str.Get("UI_MONEY_FORMAT"), Mathf.FloorToInt(value / 10000.0f));
//        } else {
//            return value.ToString();
//        }
        return value.ToString();
    }

    // 获取日期的时间
    public static string GetTimeString(long value)
    {
        return "";
    }


    // 按照权重选择序号
    public static int RandomByWeight(int[] weights)
    {
        // 空
        if (weights == null || weights.Length <= 0) {
            return -1;
        }

        // 只有一个元素
        if (weights.Length == 1) {
            return weights[0];
        }

        int low = 0;
        int total = 0;   // 最大掉落数字
        foreach (var item in weights) {
            total += item;
        }

        if (total <= 0) {
            // 权重和为0，错误情况
            return -1;
        }

        int ret = -1;
        int random = UnityEngine.Random.Range(0, total);

        // 直接遍历，不用二分查找，二分查找需要构建有序表
        for (int i = 0; i < weights.Length; ++i) {
            if (random >= low && random < low + weights[i]) {
                // 在某个掉落区间内，则进行相应的掉落判定
                ret = i;
                break;
            }
            low += weights[i];
        }

        return ret;
    }

    public static int RandomByWeight(List<int> weights)
    {
        // 空
        if (weights == null || weights.Count <= 0) {
            return -1;
        }

        // 只有一个元素
        if (weights.Count == 1) {
            return weights[0];
        }

        int low = 0;
        int total = 0;   // 最大掉落数字
        foreach (var item in weights) {
            total += item;
        }

        if (total <= 0) {
            // 权重和为0，错误情况
            return -1;
        }

        int ret = -1;
        int random = UnityEngine.Random.Range(0, total);

        // 直接遍历，不用二分查找，二分查找需要构建有序表
        for (int i = 0; i < weights.Count; ++i) {
            if (random >= low && random < low + weights[i]) {
                // 在某个掉落区间内，则进行相应的掉落判定
                ret = i;
                break;
            }
            low += weights[i];
        }

        return ret;
    }
}