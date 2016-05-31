using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using comrt.comnet;
using message;

// 辅助函数，方便调用
public partial class Net
{
    public static void Register(eCommand cmd, Action<byte[]> handler)
    {
        NetworkManager.Instance.Register(cmd, handler);
    }

    public static long Send<T>(eCommand cmd, T data, Action<byte[]> handler = null)
    {
        return NetworkManager.Instance.Send<T>(cmd, data, handler);
    }

    public static long Send(eCommand cmd, Action<byte[]> handler = null)
    {
        return NetworkManager.Instance.Send((int)cmd, null, 0, handler);
    }

    public static T Deserialize<T>(byte[] buffer)
    {
        MemoryStream stream = new MemoryStream(buffer);
        return ProtoBuf.Serializer.Deserialize<T>(stream);
    }


    // 注册消息路由 (注意，虽然支持同一个命令由多个管理类去注册管理，但是这样可能会造成混乱，尽量一个消息一个管理类去处理)
    public static void Register(MSG cmd, Action<RPCResponse> handler)
    {
        BattleNetworkManager.Instance.Register(cmd, handler);
    }

    // 发送消息，带结构体(由于服务器暂时不支持返回id，所以暂时统一用注册消息的方式，不用回调的方式)
    public static long Send<T>(MSG cmd, T data)
    {
        return BattleNetworkManager.Instance.Send<T>(cmd, data, null);
    }

    // 发送消息，没有结构体，只有命令
    public static long Send(MSG cmd)
    {
        return BattleNetworkManager.Instance.Send(cmd, null, 0, null);
    }
}
