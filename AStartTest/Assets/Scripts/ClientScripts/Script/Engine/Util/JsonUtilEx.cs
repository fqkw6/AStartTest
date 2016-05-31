using UnityEngine;
using System.Collections.Generic;
using LitJson;

// 补充的json处理函数
public class JsonUtilEx
{
    public static List<int> Json2IntList(JsonData jdata, string key)
    {
        if (!JsonUtil.HasJsonKey(jdata, key)) {
            return null;
        }

        JsonData data = jdata[key];
        int count = data.Count;
        List<int> ret = new List<int>();
        for (int i = 0; i < count; ++i) {
            ret.Add(int.Parse(data[i].ToString()));
        }

        return ret;
    }

    public static Vector3 Json2Vector(JsonData jdata, string key)
    {
        if (!JsonUtil.HasJsonKey(jdata, key)) {
            return Vector3.zero;
        }

        JsonData data = jdata[key];
        int count = data.Count;
        
        Vector3 ret = new Vector3(float.Parse(data[0].ToString()), float.Parse(data[1].ToString()), float.Parse(data[2].ToString()));
        return ret;
    }
}
