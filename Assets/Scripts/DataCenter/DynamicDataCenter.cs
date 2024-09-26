using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DynamicDataCenter
{
    public delegate void DataReceiveDelegate(params object[] paras);//callback delegate
                                                                    //collection of callback functions
    public static Dictionary<EmDataType, DataReceiveDelegate> onUpdateDataEvents = new Dictionary<EmDataType, DataReceiveDelegate>();

    internal static void AddMessage(EmDataType artifactAutoRefine, object v)
    {
        throw new NotImplementedException();
    }

    //data list that exists in datacenter
    public static List<EmDataType> dataStorage = new List<EmDataType>();


    public static void RemoveAllData()
    {
        onUpdateDataEvents.Clear();
        dataStorage.Clear();
    }

    /// <summary>
    /// Request data from datacenter.
    /// </summary>
    /// <param name="dataType">Data type.</param>
    /// <param name="func">callback function.</param>
    /// <param name="paras">variable parameters.</param>
    public static void AddMessage(EmDataType dataType, DataReceiveDelegate func, bool sendToServer = true)
    {
        if (!onUpdateDataEvents.ContainsKey(dataType))
            onUpdateDataEvents.Add(dataType, null);
        if (!ContainsEvent(onUpdateDataEvents[dataType], func))
            onUpdateDataEvents[dataType] += func;
        if (dataStorage.Contains(dataType))
            func.Invoke();
    }

    public static void AddMessage(int dataType, DataReceiveDelegate func, bool sendToServer = true)
    {
        AddMessage((EmDataType)dataType, func, sendToServer);
    }

    /// <summary>
    /// Removes callback.
    /// </summary>
    /// <param name="dataType">Data type.</param>
    /// <param name="func">variable parameters.</param>
    public static void RemoveCallBack(EmDataType dataType, DataReceiveDelegate func)
    {
        if (onUpdateDataEvents.ContainsKey(dataType))
        {
            while (ContainsEvent(onUpdateDataEvents[dataType], func))
                onUpdateDataEvents[dataType] -= func;
        }
    }
    public static void RemoveCallBack(int dataType, DataReceiveDelegate func)
    {
        RemoveCallBack((EmDataType)dataType, func);
    }

    /// <summary>
    ///  receive data, but not mark this type of data as existed in datacenter .
    /// </summary>
    /// <param name="dataType">Data type.</param>
    /// <param name="paras">variable parameters.</param>
    public static void SendMessage(EmDataType dataType, params object[] paras)
    {
        if (!onUpdateDataEvents.ContainsKey(dataType))
            onUpdateDataEvents.Add(dataType, null);
        else if (onUpdateDataEvents[dataType] != null)
            onUpdateDataEvents[dataType].Invoke(paras);
    }
    public static void SendMessage(int dataType, params object[] paras)
    {
        SendMessage((EmDataType)dataType, paras);
    }

    /// <summary>
    /// Removes data status.
    /// </summary>
    /// <param name="dataType">Data type.</param>
    private static void RemoveDataMark(EmDataType dataType)
    {
        if (dataStorage.Contains(dataType))
            dataStorage.Remove(dataType);
    }
    /// <summary>
    /// Removes data status.
    /// </summary>
    /// <param name="dataType">Data type.</param>
    public static void RemoveDataAndRelatedCallBack(EmDataType dataType, params object[] paras)
    {
        if (onUpdateDataEvents.ContainsKey(dataType))
        {
            //			if(onUpdateDataEvents[dataType]!=null)
            //				onUpdateDataEvents [dataType].Invoke (paras);
            onUpdateDataEvents.Remove(dataType);
        }
        RemoveDataMark(dataType);
    }
    public static void RemoveDataAndRelatedCallBack(int dataType, params object[] paras)
    {
        RemoveDataAndRelatedCallBack((EmDataType)dataType, paras);
    }
    /// <summary>
    /// check if callback have existed in callback List.
    /// </summary>
    /// <returns><c>true</c>, if callback have existed in callback List, <c>false</c> otherwise.</returns>
    /// <param name="delLink">callback function list</param>
    /// <param name="func">callback function.</param>
    private static bool ContainsEvent(DataReceiveDelegate delLink, DataReceiveDelegate func)
    {
        if (delLink == null)
            return false;
        foreach (DataReceiveDelegate del in delLink.GetInvocationList())
        {
            if (del == func)
                return true;
        }
        return false;
    }
}


