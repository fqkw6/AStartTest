using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class JsonUtil
{
    public static bool HasJsonKey(JsonData jdata, string key)
    {
        if (jdata == null) {
            return false;
        }

        IDictionary dict = jdata as IDictionary;
        if (dict == null || !dict.Contains(key)) {
            return false;
        }

        return true;
    }

    public static bool IsValueEmpty(JsonData jdata)
    {
        if (jdata == null) {
            return true;
        }

        if (jdata.ToString().Length <= 0) {
            return true;
        }
        return false;
    }

    public static float Json2Float(JsonData jdata, string key, float defaultValue = 0)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return defaultValue;
            }

            return (float)System.Convert.ToDouble(jdata[key].ToString());
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return defaultValue;
    }

    public static int Json2Int(JsonData jdata, string key, int defaultValue = 0)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return defaultValue;
            }

            return System.Convert.ToInt32(jdata[key].ToString());
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return defaultValue;
    }

    public static long Json2Long(JsonData jdata, string key, long defaultValue = 0)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return defaultValue;
            }

            return System.Convert.ToInt64(jdata[key].ToString());
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return defaultValue;
    }

    public static string Json2String(JsonData jdata, string key, string defaultValue = "")
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return defaultValue;
            }

            return jdata[key].ToString();
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return defaultValue;
    }

    public static Translation Json2Translation(JsonData jdata, string key)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return Translation.EMPTY;
            }

            return new Translation(jdata[key].ToString());
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return Translation.EMPTY;
    }

    public static bool Json2Boolean(JsonData jdata, string key, bool defaultValue = false)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return defaultValue;
            }

            string temp = jdata[key].ToString();
            if (temp.Equals("TRUE") || temp.Equals("True") || temp.Equals("true") || temp.Equals("yes") || temp.Equals("1")) {
                return true;
            } else {
                return false;
            }
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return defaultValue;
    }

    public static Vector3 Json2Vector(JsonData jdata, string key)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return Vector3.zero;
            }

            string[] list = jdata[key].ToString().Split(',');
            if (list.Length < 3) {
                return Vector3.zero;
            }

            float x = (float)System.Convert.ToDouble(list[0]);
            float y = (float)System.Convert.ToDouble(list[1]);
            float z = (float)System.Convert.ToDouble(list[2]);

            return new Vector3(x, y, z);
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return Vector3.zero;
    }

    public static Color Json2Color(JsonData jdata, string key, Color defaultValue)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return defaultValue;
            }

            string[] list = jdata[key].ToString().Split(',');
            if (list.Length < 3) {
                return defaultValue;
            }

            int r = System.Convert.ToInt32(list[0]);
            int g = System.Convert.ToInt32(list[1]);
            int b = System.Convert.ToInt32(list[2]);
            int a = 255;

            if (list.Length == 4) {
                a = System.Convert.ToInt32(list[3]);
            }
            return new Color(1.0f * r / 255, 1.0f * g / 255, 1.0f * b / 255, 1.0f * a / 255);
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return defaultValue;
    }

    public static Color Json2ColorF(JsonData jdata, string key, Color defaultValue)
    {
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return defaultValue;
            }

            string[] list = jdata[key].ToString().Split(',');
            if (list.Length < 3) {
                return defaultValue;
            }

            float r = (float)System.Convert.ToDouble(list[0]);
            float g = (float)System.Convert.ToDouble(list[1]);
            float b = (float)System.Convert.ToDouble(list[2]);
            float a = (float)System.Convert.ToDouble(list[3]);

            return new Color(r, g, b, a);
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return defaultValue;
    }

    public static List<int> Json2IntList(JsonData jdata, string key)
    {
        List<int> ret = new List<int>();
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return ret;
            }

            JsonData list = jdata[key];
            int count = list.Count;
            for (int i = 0; i < count; ++i) {
                ret.Add(System.Convert.ToInt32(list[i].ToString()));
            }
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return ret;
    }

    public static List<float> Json2FloatList(JsonData jdata, string key)
    {
        List<float> ret = new List<float>();
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return ret;
            }

            JsonData list = jdata[key];
            int count = list.Count;
            for (int i = 0; i < count; ++i) {
                ret.Add((float)System.Convert.ToDouble(list[i].ToString()));
            }
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return ret;
    }

    public static List<string> Json2StringList(JsonData jdata, string key)
    {
        List<string> ret = new List<string>();
        try {
            if (!HasJsonKey(jdata, key) || IsValueEmpty(jdata[key])) {
                return ret;
            }

            JsonData list = jdata[key];
            int count = list.Count;
            for (int i = 0; i < count; ++i) {
                ret.Add(list[i].ToString());
            }
        } catch (System.Exception ex) {
            Debug.LogException(ex);
        }

        return ret;
    }
}