using System;
using System.Collections.Generic;

public interface IMessage
{
    void RecvMsg(int cmd, params object[] data);
}

public class Message
{
    public static readonly Message Instance = new Message();

    // Message set
    protected Dictionary<int, List<IMessage>> msgDic;

    public Message()
    {
        msgDic = new Dictionary<int, List<IMessage>>();
    }

    /// <summary>
    /// send a message
    /// </summary>
    public void Send(int cmd, params object[] data)
    {
        ExecMsg(cmd, data);
    }

    /// <summary>
    /// Add message
    /// </summary>
    public void Add(int cmd, IMessage msg)
    {
        List<IMessage> msgList;
        if (!msgDic.TryGetValue(cmd, out msgList))
        {
            msgList = new List<IMessage>();
            msgDic.Add(cmd, msgList);
        }
        if (!msgList.Contains(msg))
        {
            msgList.Add(msg);
        }
    }

    /// <summary>
    /// Remove message
    /// </summary>
    public void Remove(int cmd, IMessage msg)
    {
        List<IMessage> msgList;
        if (msgDic.TryGetValue(cmd, out msgList))
        {
            int index = msgList.IndexOf(msg);
            if (index > -1)
            {
                msgList[index] = null;
            }
        }
    }

    /// <summary>
    /// Clear message
    /// </summary>
    public void Clear()
    {
        msgDic.Clear();
    }

    /// <summary>
    /// Execution message
    /// </summary>
    private void ExecMsg(int cmd, params object[] data)
    {
        List<IMessage> msgList;
        if (msgDic.TryGetValue(cmd, out msgList))
        {
            int len = msgList.Count;
            for (int i = 0; i < len;)
            {
                if (msgList[i] != null)
                {
                    msgList[i].RecvMsg(cmd, data);
                    i++;
                }
                else
                {
                    msgList.RemoveAt(i);
                    len--;
                }
            }
            if (len <= 0)
            {
                msgDic.Remove(cmd);
            }
        }
        object[] data2;
        if (data == null)
        {
            data2 = new object[1];
        }
        else
        {
            data2 = new object[data.Length + 1];
            data.CopyTo(data2, 1);
        }
        data2[0] = cmd;
    }

    internal void Add(int v, object callback)
    {
        throw new NotImplementedException();
    }
}