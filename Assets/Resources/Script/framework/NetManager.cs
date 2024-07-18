using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Linq;


public class NetManager
{
    //定义套接字
    static Socket socket;
    //接受缓存区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;
    //事件
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }
    //事件委托类型
    public delegate void EventListener(string err);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener>
            eventListeners = new Dictionary<NetEvent, EventListener>();
    //消息委托类型
    public delegate void MsgListener(MsgBase msgBase);
    //消息监听列表
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    //是否正在连接
    static bool isConnecting = false;
    //是否正在关闭
    static bool isClosing = false;
    //消息列表
    static List<MsgBase> msgList = new List<MsgBase>();
    //消息列表长度
    static int msgCount = 0;
    //每一次update处理的消息量
    readonly static int MAX_MESSAGE_FIRE = 10;

    //是否启用心跳
    public static bool isUsePing = true;
    //心跳间隔时间
    public static int pingInterval = 30;
    //上一次发送Ping的时间
    static float lastPingTime = 0;
    //上一次收到PONG的时间
    static float lastPongTime = 0;


    //添加事件监听
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        //添加事件
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener; //同一个事件可以有多个监听函数
        }
        else
        {
            eventListeners[netEvent] = listener;
        }
    }
    //删除事件监听
    public static void RemoveListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
        }
    }
    //分发事件,一个参数是netevent的类型，另一个参数是传递给委托函数的参数
    private static void FireEvent(NetEvent netEvent, String err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);//执行回调函数
        }
    }

    //添加消息监听
    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        //添加
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        else
        {
            msgListeners[msgName] = listener;
        }
    }
    //删除消息监听
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
        }
    }
    //分发消息
    private static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }

    //连接
    public static void Connect(string ip, int port)
    {
        //状态判断
        if (socket != null && socket.Connected)
        {
            Debug.Log("Connect fail, already connected!");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("Connect fail, isConnecting");
            return;
        }
        //初始化成员
        InitState();
        //参数设置
        socket.NoDelay = true;
        //Connect
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    //初始化状态
    private static void InitState()
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //接收缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();
        //是否正在连接
        isConnecting = false;
        //是否正在关闭
        isClosing = false;
        //消息列表
        msgList = new List<MsgBase>();
        //消息列表长度
        msgCount = 0;
        //上一次发送ping时间
        lastPingTime = Time.time;
        //上一次收到Pong的时间
        lastPongTime = Time.time;
        //监听PONG协议
        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }

    //Connect回调
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
            //开始接收
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect Fail" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }

    //Receive回调
    public static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //获取接收数据长度
            int count = socket.EndReceive(ar);
            if (count == 0)
            {
                Close();
                return;
            }
            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData();
            //继续接收消息
            if (readBuff.remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }

    }

    //接收数据
    public static void OnReceiveData()
    {
        //消息长度
        if (readBuff.length <= 2)
            return;
        //获取消息长度
        Int16 bodyLength = readBuff.ReadInt16();
        //解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "")
        {
            Debug.Log("OnReceiveData MsgBase.DecodeName fail");
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //添加到消息队列
        lock (msgList)
        {
            msgList.Add(msgBase);
        }
        msgCount++;
        //继续接收消息
        if (readBuff.length > 2)
        {
            OnReceiveData();
        }
    }

    //关闭链接
    public static void Close()
    {
        //状态判断
        if (socket == null || !socket.Connected)
            return;
        if (isConnecting)
            return;
        //还有数据在发送
        if (writeQueue.Count > 0)
            isClosing = true;
        //没有数据发送了
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    //发送数据
    public static void Send(MsgBase msg)
    {
        //状态判断
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
            return;
        if (isClosing)
            return;
        //数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        //组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        //组装名字
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        //组装消息题
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
        //写入队列
        ByteArray ba = new ByteArray(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        //Send
        if (count == 1)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }
    }
    //Send回调
    public static void SendCallback(IAsyncResult ar)
    {
        //获取state、EndSend的处理
        Socket socket = (Socket)ar.AsyncState;
        //状态判断
        if (socket == null || !socket.Connected)
            return;
        //EndSend
        int count = socket.EndSend(ar);
        //获取写入队列的第一条数据
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        //完整发送
        ba.readIdx += count;
        if (ba.length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }
        //继续发送
        if (ba != null)
        {
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
        }
        //正在关闭
        else if (isClosing)
        {
            socket.Close();
        }
    }

    //update函数
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }
    public static void MsgUpdate()
    {
        //初步判断
        if (msgCount == 0)
            return;
        //重复处理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            //获取第一条消息
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            //分发消息
            if (msgBase != null)
            {
                FireMsg(msgBase.protoName, msgBase);
                Debug.Log("firemsg " + msgBase.protoName);
            }
            //没有消息了
            else
            {
                break;
            }
        }
    }
    //发送PING协议
    private static void PingUpdate()
    {
        //是否启用
        if (!isUsePing)
            return;
        //发送PING
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }
        //检测PONG时间
        if (Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }

    //监听PONG协议
    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }


    // static byte[] readBuff=new byte[1024];
    // //委托类型
    // public delegate void MsgListener(String str);
    // //监听列表
    // private static Dictionary<string,MsgListener> listeners=new Dictionary<string, MsgListener>();
    // //消息列表
    // static List<String> msgList=new List<String>();

    // //添加监听
    // public static void AddListener(string msgName,MsgListener listener)
    // {
    //     listeners[msgName]=listener;
    // }

    // //获取描述
    // public static string GetDesc()
    // {
    //     if(socket==null) return "";
    //     if(!socket.Connected) return "";
    //     return socket.LocalEndPoint.ToString();        
    // }

    // //连接
    // public static void Connect(string ip,int port)
    // {
    //     //Socket
    //     socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
    //     //Connect(使用同步的方法)
    //     socket.Connect(ip,port);
    //     //BeginReceive
    //     socket.BeginReceive(readBuff,0,1024,0,ReceiveCallback,socket);
    // }

    // //Receive回调
    // private static void ReceiveCallback(IAsyncResult ar)
    // {
    //     try
    //     {
    //         Socket socket=ar.AsyncState as Socket;
    //         int count=socket.EndReceive(ar);
    //         string recvStr=System.Text.Encoding.Default.GetString(readBuff,0,count);
    //         msgList.Add(recvStr);
    //         socket.BeginReceive(readBuff,0,1024,0,ReceiveCallback,socket);
    //     }
    //     catch(SocketException ex)
    //     {
    //         Console.WriteLine("Socket Receive fail"+ex.ToString());
    //     }
    // }

    // //发送
    // public static void Send(string sendStr)
    // {
    //     if(socket==null) return;
    //     if(!socket.Connected) return;
    //     byte[] sendBytes=System.Text.Encoding.Default.GetBytes(sendStr);
    //     socket.Send(sendBytes);
    // }

    // // Update is called once per frame
    // public static void Update()
    // {
    //     if(msgList.Count<=0)    return;
    //     String msgStr=msgList[0];
    //     msgList.RemoveAt(0);
    //     //目前的发送的消息是通过｜来分别的，前面的是协议的名称，后面的是内容
    //     //后续会进行更改
    //     string[] split=msgStr.Split('|');
    //     string msgName=split[0];
    //     string msgArgs=split[1];
    //     //监听回调
    //     if(listeners.ContainsKey(msgName))
    //     {
    //         //这一步就是执行当前协议所对应的回调函数
    //         listeners[msgName](msgArgs);
    //     }
    // }
}
