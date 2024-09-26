using UnityEngine;
using System;
using System.Collections;

public class GameUtils
{
    public static GameObject CreateObj(Transform parent, string name, float modelScale = 1, Action<GameObject> callback = null)
    {
        GameObject obj = Resources.Load(name) as GameObject;
        obj = GameObject.Instantiate(obj);

        if (obj != null)
        {
            ChangeObjValue(obj, parent, modelScale);
            if (callback != null)
                callback(obj);
        }
        return obj;
    }

    public static void CreateObj<T>(Transform parent, string name, Action<GameObject, T> callback = null)
    {
        GameObject obj = Resources.Load(name) as GameObject;
        if (obj != null)
        {
            T tScript;
            ChangeObjValue(obj, parent);
            tScript = obj.GetComponent<T>();
            if (callback != null)
                callback(obj, tScript);
        }
    }


    static void SetObjParent(Transform parent, string scriptName, GameObject flowerCtrlObj, Action<GameObject, Component> callback = null)
    {
        if (flowerCtrlObj != null)
        {
            Component script = null;
            ChangeObjValue(flowerCtrlObj, parent);
            if (scriptName != null)
                script = flowerCtrlObj.GetComponent(scriptName);
            if (callback != null)
                callback(flowerCtrlObj, script);
        }
    }

    private static void SetModelLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetModelLayer(child.gameObject, layer);
        }
    }

    private static GameObject CreateModel(UnityEngine.Object asset, GameObject parent, float modelScale)
    {
        string shadername = "";
        //firParent.name = asset.name + "_Parent";
        GameObject modelObj = GameObject.Instantiate(asset) as GameObject;

        ChangeObjValue(modelObj, parent.transform, modelScale);

        modelObj.name = asset.name;
        if (!string.IsNullOrEmpty(shadername))
        {
            SkinnedMeshRenderer[] renderers = modelObj.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; ++i)
                {
                    if (renderers[i].material != null)
                        renderers[i].material.shader = Shader.Find(shadername);

                    renderers[i].material.SetFloat("_RimWidth", 0.1f);
                }
            }
        }
        return modelObj;
    }

    public static void ChangeObjValue2(GameObject obj, Transform parent, float scale = 1)
    {
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one * scale;
        obj.SetActive(true);
    }

    public static void ChangeObjValue(GameObject obj, Transform parent, float scale = 1)
    {
        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one * scale;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.SetActive(true);
    }

    public static Color GetColorForHexColor(string hexColor)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hexColor, out color);
        return color;
    }

    public static void SetScaleTF(Transform trans, float x, float y, float z)
    {
        if (trans != null)
        {
            Vector3 scale;
            scale.x = x;
            scale.y = y;
            scale.z = z;
            trans.localScale = scale;
        }
    }

    public static IEnumerator SmoothSetLocalPosTF(Transform trans, float x, float y, float z = 0, float duration = 0.1f)
    {
        if (trans != null)
        {
            Vector3 startPosition = trans.localPosition;
            Vector3 targetPosition = new Vector3(x, y, z);
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration; // Calculate the interpolation factor
                trans.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null; // Wait until the next frame
            }

            trans.localPosition = targetPosition; // Ensure the final position is set
        }
    }

    public static void SetLocalPosTF(Transform parent, Transform trans, float x, float y, float z = 0)
    {
        if (parent != null)
            trans.parent = parent;

        if (trans != null)
        {
            Vector3 position;
            position.x = x;
            position.y = y;
            position.z = z;
            trans.localPosition = Vector3.Lerp(trans.localPosition, position, 0.5f);
        }
    }

    public static void SetLocalPosTFLerp(Transform trans, float x, float y, float z = 0)
    {
        if (trans != null)
        {
            Vector3 position;
            position.x = x;
            position.y = y;
            position.z = z;
            trans.localPosition = Vector3.Lerp(trans.localPosition, position, 0.5f);
        }
    }

    public static void SetLocalPosTF(Transform trans, float x, float y, float z = 0)
    {
        if (trans != null)
        {
            Vector3 position;
            position.x = x;
            position.y = y;
            position.z = z;
            trans.localPosition = position;
        }
    }

    public static void SetLocalPosTF(Transform trans, Vector3 position)
    {
        if (trans != null)
        {
            trans.localPosition = position;
        }
    }

    public static IEnumerator SmoothSetRotationTF(Transform trans, float x, float y, float z, float w, float duration = 0.1f)
    {
        if (trans != null)
        {
            Quaternion startRotation = trans.localRotation;
            Quaternion targetRotation = new Quaternion(x, y, z, w);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                Quaternion interpolatedRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                trans.localRotation = interpolatedRotation;
                yield return null; 
            }

            trans.localRotation = targetRotation; 
        }
    }

    public static void SetRotationTFLerp(Transform trans, float x, float y, float z, float w)
    {
        if (trans != null)
        {
            Quaternion quaternion = new Quaternion(x, y, z, w);
            trans.localRotation = Quaternion.Lerp(trans.localRotation, quaternion, Time.deltaTime * 30);
        }
    }

    public static void SetRotationTF(Transform trans, float x, float y, float z, float w)
    {
        if (trans != null)
        {
            trans.localRotation = new Quaternion(x, y, z, w);
        }
    }

    public static void SetRotationTF(Transform trans, Quaternion rotation)
    {
        if (trans != null)
        {
            trans.localRotation = rotation;
        }
    }
    public static void SetRotationTF(Transform trans, Vector3 eulerAngles)
    {
        if (trans != null)
        {
            trans.localEulerAngles = eulerAngles;
        }
    }
    public static void SetRotationTF(Transform trans, float x, float y, float z)
    {
        if (trans != null)
        {
            Vector3 eulerAngles = new Vector3(x, y, z);
            trans.localEulerAngles = eulerAngles;
        }
    }

    public static int GetNextDay()
    {
        int dateDiff = 0;
        DateTime DateTime1 = DateTime.Now;
        DateTime DateTime2 = DateTime.Now.AddDays(1).Date;
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        TimeSpan ts = ts1.Subtract(ts2).Duration();
        dateDiff = ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;
        return dateDiff;
    }

    public static int GetNextDay2(long nowSysTime)
    {
        int dateDiff;
        DateTime dt_time = new DateTime(1970, 1, 1, 8, 0, 0, 0);
        long time_1970 = dt_time.Ticks;
        long time_tick1 = time_1970 + nowSysTime * TimeSpan.TicksPerSecond;
        DateTime DateTime1 = new DateTime(time_tick1);
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        dateDiff = (24 - ts1.Hours - 1) * 3600 + (60 - ts1.Minutes - 1) * 60 + (60 - ts1.Seconds);
        return dateDiff;
    }

    public static int GetNextDay3(long nowSysTime)
    {
        int dateDiff;
        DateTime dt_time = new DateTime(1970, 1, 1, 8, 0, 0, 0);
        long time_1970 = dt_time.Ticks;
        long time_tick1 = time_1970 + nowSysTime * TimeSpan.TicksPerSecond;
        DateTime DateTime1 = new DateTime(time_tick1);
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        dateDiff = (24 - ts1.Hours - 1) * 3600 + (60 - ts1.Minutes - 1) * 60 + (60 - ts1.Seconds);
        return 86400 - dateDiff;
    }
    public static string GetTimeHM(long time)
    {
        DateTime dt_time = new DateTime(1970, 1, 1, 8, 0, 0, 0);
        long time_1970 = dt_time.Ticks;
        long time_tick1 = time_1970 + time * TimeSpan.TicksPerSecond;
        DateTime DateTime1 = new DateTime(time_tick1);
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        string hor = ts1.Hours < 10 ? "0" + ts1.Hours.ToString() : ts1.Hours.ToString();
        string min = ts1.Minutes < 10 ? "0" + ts1.Minutes.ToString() : ts1.Minutes.ToString();
        return string.Format("{0}:{1}", hor, min);
    }
    public static string GetTimeHMS(long startTime, long endTime)
    {
        int time = (int)(endTime - startTime);
        int hor = time / 3600;
        int min = (time % 3600) / 60;
        int sec = time % 3600 % 60;
        return string.Format("{0}时{1}分{2}秒", hor, min, sec);
    }
    public static string GetTimeHMS(long time)
    {
        string str;
        DateTime dt_time = new DateTime(1970, 1, 1, 8, 0, 0, 0);
        long time_1970 = dt_time.Ticks;
        long time_tick1 = time_1970 + time * TimeSpan.TicksPerSecond;
        DateTime DateTime1 = new DateTime(time_tick1);
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        string hor = ts1.Hours < 10 ? "0" + ts1.Hours.ToString() : ts1.Hours.ToString();
        string min = ts1.Minutes < 10 ? "0" + ts1.Minutes.ToString() : ts1.Minutes.ToString();
        string sec = ts1.Seconds < 10 ? "0" + ts1.Seconds.ToString() : ts1.Seconds.ToString();
        str = string.Format("{0}:{1}:{2}", hor, min, sec);
        return str;
    }
    public static string GetTimeYMD(long time)
    {
        DateTime dt_time = new DateTime(1970, 1, 1, 8, 0, 0, 0);
        long time_1970 = dt_time.Ticks;
        long time_tick1 = time_1970 + time * TimeSpan.TicksPerSecond;
        DateTime DateTime1 = new DateTime(time_tick1);
        return DateTime1.ToString("MM月dd日");
    }
    public static string GetTimeMDH(long time)
    {
        DateTime dt_time = new DateTime(1970, 1, 1, 8, 0, 0, 0);
        long time_1970 = dt_time.Ticks;
        long time_tick = time_1970 + time * TimeSpan.TicksPerSecond;
        DateTime dt = new DateTime(time_tick);
        return dt.ToString("MM/dd HH:mm");
    }

    public static string GetTimeMDHM(long time)
    {
        DateTime dt_time = new DateTime(1970, 1, 1, 8, 0, 0, 0);
        long time_1970 = dt_time.Ticks;
        long time_tick = time_1970 + time * TimeSpan.TicksPerSecond;
        DateTime dt = new DateTime(time_tick);
        return dt.ToString("MM/dd HH:mm:ss");
    }

    public static string GetTimeDHMS(long time)
    {
        string str;
        long day = 0;
        long seconds = time % 60;
        long minute = (time % 3600) / 60;
        long hour = time / 3600;
        day = hour / 24;
        hour -= 24 * day;
        string da = day < 10 ? "0" + day.ToString() : day.ToString();
        string hor = hour < 10 ? "0" + hour.ToString() : hour.ToString();
        string min = minute < 10 ? "0" + minute.ToString() : minute.ToString();
        string sec = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
        if (day > 0)
            str = string.Format("{0}天{1}:{2}:{3}", day, hor, min, sec);
        else
            str = string.Format("{0}:{1}:{2}", hor, min, sec);
        return str;
    }

    public static string GetTimeDH(long time)
    {
        string str;
        long day = 0;
        long hour = time / 3600;
        day = hour / 24;
        hour -= 24 * day;
        string hor = hour < 10 ? "0" + hour.ToString() : hour.ToString();
        if (day > 0)
            str = string.Format("{0}天{1}小时", day, hor);
        else
            str = string.Format("{0}小时", hor);
        return str;
    }
   
    public static string GetHour(long time)
    {
        time /= 1000;
        long hour = time / 3600;
        long minute = (time % 3600) / 60;
        string hor = hour < 10 ? "0" + hour.ToString() : hour.ToString();
        string min = minute < 10 ? "0" + minute.ToString() : minute.ToString();
        return string.Format("{0}:{1}", hor, min);
    }
    public static GameObject GetChild(Transform trans, string childName)
    {
        Transform child = trans.Find(childName);
        if (child != null)
        {
            return child.gameObject;
        }
        int count = trans.childCount;
        GameObject go = null;
        for (int i = 0; i < count; ++i)
        {
            child = trans.GetChild(i);
            go = GetChild(child, childName);
            if (go != null)
            {
                return go;
            }
        }
        return null;
    }

    public static T GetChild<T>(Transform trans, string childName) where T : Component
    {
        GameObject go = GetChild(trans, childName);
        if (go == null)
        {
            Debug.LogWarning("Select " + typeof(T).Name + " Object null");
            return null;
        }
        return go.GetComponent<T>();
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }
}
