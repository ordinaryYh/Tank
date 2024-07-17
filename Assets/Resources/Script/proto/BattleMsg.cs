using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//坦克信息
[System.Serializable]
public class TankInfo
{
    public string id=""; //玩家id
    public int camp=0;  //阵营
    public int hp=0;   //生命值

    public float x=0;   //位置
    public float y=0;
    public float z=0;
    public float ex=0;   //旋转
    public float ey=0;
    public float ez=0;
}

//进入战场协议(服务端推送)
public class MsgEnterBattle:MsgBase
{
    public MsgEnterBattle()
    {
        protoName="MsgEnterBattle";
    }
    //服务端回
    public TankInfo[] tanks;
    public int mapId=1;
}

//战斗结果（服务端推送） 
public class MsgBattleResult:MsgBase
{
    public MsgBattleResult(){protoName="MsgBattleResult";}
    //服务端回
    public int winCamp=0; //获胜的阵营
}

//退出战斗（服务端推送）
public class MsgLeaveBattle:MsgBase
{
    public MsgLeaveBattle(){protoName="MsgLeaveBattle";}
    //服务端回
    public string id=""; //玩家id
}

