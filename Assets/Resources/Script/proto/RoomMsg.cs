using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//查询战绩协议
public class MsgGetAchieve : MsgBase
{
    public MsgGetAchieve()
    {
        protoName = "MsgGetAchieve";
    }
    //服务端返回
    public int win = 0;
    public int lost = 0;
}

//房间信息
[System.Serializable]
public class RoomInfo
{
    public int id = 0;  //房间id
    public int count = 0;  //人数
    public int status = 0;  //状态0-准备中 1-战斗中
}

//请求查询房间列表协议
public class MsgGetRoomList : MsgBase
{
    public MsgGetRoomList()
    {
        protoName = "MsgGetRoomList";
    }
    //服务端返回
    public RoomInfo[] rooms;
}

//创建房间协议
public class MsgCreateRoom : MsgBase
{
    public MsgCreateRoom()
    {
        protoName = "MsgCreateRoom";
    }
    //服务端返回
    public int result = 0;
}

//进入房间协议
public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        protoName = "MsgEnterRoom";
    }
    //客户端发
    public int id = 0;
    //服务端返回
    public int result = 0;
}

//玩家信息
[System.Serializable]

public class PlayerInfo
{
    public string id = "yh";    //账号
    public int camp = 0;  //阵营
    public int win = 0;   //胜利数
    public int lost = 0;  //失败数
    public int isOwner = 0;  //是否是房主
}

//获取房间信息
public class MsgGetRoomInfo : MsgBase
{
    public MsgGetRoomInfo() { protoName = "MsgGetRoomInfo"; }
    //服务端返回
    public PlayerInfo[] players;
}

//退出房间协议
public class MsgLeaveRoom : MsgBase
{
    public MsgLeaveRoom() { protoName = "MsgLeaveRoom"; }
    //服务端返回
    public int result = 0;
}

//点击开战协议
public class MsgStartBattle : MsgBase
{
    public MsgStartBattle() { protoName = "MsgStartBattle"; }
    //服务端返回
    public int result = 0;
}