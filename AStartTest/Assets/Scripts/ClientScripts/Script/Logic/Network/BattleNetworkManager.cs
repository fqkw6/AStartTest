using System;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ProtoBuf;
using message;

// 网络模块 处理连接、收发数据
public class BattleNetworkManager
{
    public int HEADER_SIZE = 4;

    public static BattleNetworkManager Instance = new BattleNetworkManager();
    
    public Action<ConnectState> OnConnectCallback;
    public Action OnDisConnectCallback;

    private ConnectState _currentState = ConnectState.INVALID;

    public const int SEND_BUFFER_SIZE = 1024*8; // 发送数据缓冲区大小
    public const int RECIEVE_BUFFER_SIZE = 1024*64; // 接收数据缓冲区大小

    private Socket _socket = null;

    private int _sendSize = 0; // 发送缓冲区中需要发送的数据大小
    private int _recvSize = 0; // 接收缓冲区中数据的大小
    private int _recvStartIndex = 0; // 接收缓冲区中数据的起始位置
    private byte[] _sendBuffer = new byte[SEND_BUFFER_SIZE]; // 发送数据缓冲区
    private byte[] _recieveBuffer = new byte[RECIEVE_BUFFER_SIZE]; // 接收数据缓冲区 此为环形缓冲区

    private bool _isRecieving = false;
    private bool _notifyConnectState = false;
    
    // 连接服务器
    public void Connect(string ip, int port, Action<ConnectState> callback)
    {
        Log.Info("Connect:   {0}  {1}", ip, port);
        if (IsConnected()) {
            Log.Warning("重复连接: " + ip);
            Close();
        }

        // 检测是ip地址还是域名
        if (!(new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))")).IsMatch(ip)) {
            IPHostEntry ipHost = Dns.GetHostEntry(ip);
            ip = ipHost.AddressList[0].ToString();
        }

        OnConnectCallback = callback;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, RECIEVE_BUFFER_SIZE);

        try {
            _currentState = ConnectState.CONNECTING;
            _socket.BeginConnect(IPAddress.Parse(ip), port, (ar) =>
            {
                // 处理连接结果
                Socket socket = (Socket) ar.AsyncState;
                socket.EndConnect(ar);

                _currentState = IsConnected() ? ConnectState.CONNECTED : ConnectState.INVALID;
                
                if (_currentState == ConnectState.CONNECTED) {
                    Log.Info("连接成功: " + ip);
                } else {
                    Log.Error("连接失败: " + ip);
                }

                _notifyConnectState = true;
            }, _socket);
        } catch (Exception e) {
            Log.Exception(e);
        }
    }

    // 关闭网络连接
    public void Close()
    {
        if (IsConnected()) {
            _socket.Close(0);
        }
        _isRecieving = false;
        _socket = null;
        _currentState = ConnectState.INVALID;

        // TODO 这里不应该清除，否则数据会错乱掉
        //        _recvStartIndex = 0;
        //        _recvSize = 0;
    }

    // 测试用，强行关闭socket连接
    public void TestDisConnect()
    {
        _socket.Close(0);
    }

    // 网络连接是否可用
    public bool IsConnected()
    {
        return ((_socket != null) && (_socket.Connected == true));
    }

    public ConnectState GetState()
    {
        return _currentState;
    }

    // 外部调用，每帧处理网络数据
    public void OnUpdate(float delta)
    {
        // 通知外部连接状态
        if (_notifyConnectState) {
            _notifyConnectState = false;
            if (OnConnectCallback != null) {
                OnConnectCallback(_currentState);
            }
        }

        if (_currentState != ConnectState.CONNECTED) {
            return;
        }

        if (!IsConnected()) {
            Close();
            OnConnectLost();
            Log.Error("socket连接已断开");
            return;
        }

        // 向服务器发送数据
        Flush();

        // 从服务器接收数据，尽可能多的接收数据，当没有消息时，beginrevieve没有返回，此时相当于挂起
        DoRecvFromSocket();

        // 接收消息（取得socket缓冲区中的所有消息，直到缓冲区为空）
        while (true) {
            if (!Recv()) {
                break;
            }
        }
    }


    private void OnConnectLost()
    {
        _currentState = ConnectState.INVALID;
        if (OnDisConnectCallback != null) {
            OnDisConnectCallback();
        }
    }

    private bool Send(byte[] data)
    {
        if (!IsConnected()) {
            return false;
        }

        int length = data.Length;
        if (_sendSize + length > SEND_BUFFER_SIZE) {
            // 数据超出缓冲区，立即发送数据以清空缓冲区
            // 一般情况不会发生这种情况，因为每帧都会将缓冲区的数据发送出去
            Flush();

            if (_sendSize + length > SEND_BUFFER_SIZE) {
                // 出错了，数据没有发送出去
                Close();
                return false;
            }
        }

        Array.Copy(data, 0, _sendBuffer, _sendSize, length);
        _sendSize += length;
        return true;
    }

    private void Flush()
    {
        // 没有数据需要发送
        if (_sendSize <= 0) {
            return;
        }

        // 检查网络连接
        if (!IsConnected()) {
            return;
        }

        // 实际发送数据
        try {
            _socket.BeginSend(_sendBuffer, 0, _sendSize, SocketFlags.None, (IAsyncResult ar) =>
            {
                Socket socket = (Socket) ar.AsyncState;
                int num = socket.EndSend(ar);
                if (num > 0) {
                    if (num < _sendSize) {
                        // 没有发送完，只删除已发送的部分
                        Array.Copy(_sendBuffer, num, _sendBuffer, 0, _sendSize - num);
                    }

                    _sendSize -= num;
                    if (_sendSize < 0) {
                        // 走到这里可能发生了错误
                        _sendSize = 0;
                    }
                }
            }, _socket);
        } catch (Exception e) {
            Log.Exception(e);
        }
    }

    private bool Recv()
    {
        if (!IsConnected()) {
            return false;
        }

        // 检查消息头（小于HEADER_SIZE，则无法获取到实际消息数据）
        if (_recvSize < HEADER_SIZE) {
            return false;
        }

        // 计算要拷贝的消息的消息头
        int startIndex = _recvStartIndex;
        int msgLength = NetUtil.ReadIntFromRing(_recieveBuffer, startIndex, RECIEVE_BUFFER_SIZE);
        startIndex = (startIndex + sizeof (int))%RECIEVE_BUFFER_SIZE;

        // 检查是否有一个消息（小于实际消息长度，则无法获取到实际消息数据）
        if (_recvSize < msgLength) {
            return false;
        }

        // 读取实际消息数据(protobuf数据)
        byte[] buffer = new byte[msgLength];
        NetUtil.ReadByteFromRing(_recieveBuffer, startIndex, RECIEVE_BUFFER_SIZE, buffer, msgLength);

        // 当成功接收完一个消息，则修改缓冲区头的位置
        _recvStartIndex = (startIndex + msgLength)%RECIEVE_BUFFER_SIZE;
        _recvSize -= (HEADER_SIZE + msgLength);
        
        // 解析消息为rpc结构
        MemoryStream stream = new MemoryStream(buffer);
        RPCResponse msgResponse = ProtoBuf.Serializer.Deserialize<RPCResponse>(stream);

        try {
            // 消息信息提示
            Net.CheckErrorCode(msgResponse.ErrCode, msgResponse.MsgID);

            ProcessMsg(msgResponse);

            Action<RPCResponse> callback;
            if (_messageResponseCallback.TryGetValue(msgResponse.MsgIdentifier, out callback)) {
                if (callback != null) {
                    callback(msgResponse);
                }

                _messageResponseCallback.Remove(msgResponse.MsgIdentifier);
            }
        } catch (Exception e) {
            Log.Exception(e);
        }

        return true;
    }

    // 从网络中读取尽可能多的数据，实际向服务器请求数据的地方
    private bool DoRecvFromSocket()
    {
        if (_isRecieving) {
            return false;
        }

        if (_recvSize >= RECIEVE_BUFFER_SIZE || !IsConnected()) {
            // 出异常了，缓冲区积压了很多数据没有处理
            return false;
        }

        // 此次接收消息的最大长度
        int saveLen = 0;
        if (_recvStartIndex + _recvSize < RECIEVE_BUFFER_SIZE) {
            // 尾部还有剩余空间盛放接收的消息，则直接放在尾部就可以了，此时缓冲区头部还有剩余空间，但是因为环形缓冲区头尾分离，所以只能先获取一部分数据填满尾部，然后再获取数据从头部开始填充
            saveLen = RECIEVE_BUFFER_SIZE - (_recvStartIndex + _recvSize);
        } else {
            // 现有消息已经有回卷了，则接收的时候肯定不会回卷（否则会有数据覆盖），此时所有的空余空间都是可以存放数据的
            saveLen = RECIEVE_BUFFER_SIZE - _recvSize;
        }

        // 接收消息的缓冲区位置
        int savePos = (_recvStartIndex + _recvSize)%RECIEVE_BUFFER_SIZE;

        try {
            _isRecieving = true;
            _socket.BeginReceive(_recieveBuffer, savePos, saveLen, SocketFlags.None, (IAsyncResult ar) =>
            {
                Socket socket = (Socket) ar.AsyncState;
                int num = socket.EndReceive(ar);
                _isRecieving = false;
                if (num > 0) {
                    // 有数据
                    _recvSize += num;
                }
            }, _socket);
        } catch (Exception e) {
            Log.Exception(e);
            return false;
        }

        return true;
    }

    private Dictionary<long, Action<RPCResponse>> _messageResponseCallback = new Dictionary<long, Action<RPCResponse>>();

    //  发送一个消息
    private static int s_currentId = 0;

    public long Send(MSG cmd, byte[] buffer, int bufferLen, Action<RPCResponse> callback = null)
    {
        Log.Info("发送消息: {0}", cmd);

        // 构造消息
        RPCRequest reqBuilder = new RPCRequest();
        reqBuilder.MsgID = cmd;
        reqBuilder.MsgIdentifier = ++s_currentId;

        if (buffer != null) {
            reqBuilder.MsgData = new byte[bufferLen];
            Array.Copy(buffer, 0, reqBuilder.MsgData, 0, bufferLen);
        }

        MemoryStream stream = new MemoryStream();
        Serializer.Serialize(stream, reqBuilder);

        int length = (int) stream.Length;
        byte[] data = new byte[length + HEADER_SIZE];

        // 写入消息长度
        NetUtil.WriteInt(data, 0, length);
        // 写入消息体
        Array.Copy(stream.GetBuffer(), 0, data, HEADER_SIZE, length);

        if (callback != null) {
            _messageResponseCallback[s_currentId] = callback;
        }

        // 把要发送的数据写入缓存，每帧发送缓存中的消息
        Send(data);

        // 外部逻辑可以根据返回的sid来判断服务器消息的回应情况
        return s_currentId;
    }

    public long Send<T>(MSG cmd, T data, Action<RPCResponse> callback = null)
    {
        MemoryStream stream = new MemoryStream();
        Serializer.Serialize(stream, data);
        return Send(cmd, stream.GetBuffer(), (int)stream.Length, callback);
    }

    public long Send(MSG cmd, Action<RPCResponse> callback = null)
    {
        return Send(cmd, null, 0, callback);
    }

    // 消息路由(注意，虽然支持同一个命令由多个管理类去注册管理，但是这样可能会造成混乱，尽量一个消息一个管理类去处理)
    private readonly Dictionary<MSG, Delegate> _router = new Dictionary<MSG, Delegate>();
    public void Register(MSG cmd, Action<RPCResponse> handler)
    {
        if (!_router.ContainsKey(cmd)) {
            _router.Add(cmd, null);
        }

        _router[cmd] = Delegate.Combine(_router[cmd], handler);
    }

    // 触发消息
    private void ProcessMsg(RPCResponse ret)
    {
        Delegate handler;

        //UnityEngine.Debug.Log("------------->" + ret.MsgID);

        if (_router.TryGetValue(ret.MsgID, out handler)) {
            var invocationList = handler.GetInvocationList();
            for (var i = 0; i < invocationList.Length; i++) {
                var action = invocationList[i] as Action<RPCResponse>;
                if (action == null) {
                    continue;
                }

                try {
                    action(ret);
                } catch (Exception ex) {
                    Log.Exception(ex);
                }
            }
        }
    }
}
