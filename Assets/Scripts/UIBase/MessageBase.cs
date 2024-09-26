using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
[Serializable]
public class MessageBase
{
    public int feedback; // 1000 succeed
    public MessageType messageId;
    public object data;


    // Serialize the object into an array of bytes
    public byte[] Serialize()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        formatter.Serialize(memoryStream, this);
        return memoryStream.ToArray();
    }

    // Deserialize an array of bytes into an object
    public static MessageBase Deserialize(byte[] byteArray)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(byteArray);
        return formatter.Deserialize(memoryStream) as MessageBase;
    }
}
