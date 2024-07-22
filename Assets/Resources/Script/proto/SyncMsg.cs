using System;

//同步坦克信息
public class MsgSyncTank : MsgBase
{
    public MsgSyncTank() { protoName = "MsgSyncTank"; }
    //位置、旋转、炮塔旋转
    public float x = 0;
    public float y = 0;
    public float z = 0;
    public float ex = 0;
    public float ey = 0;
    public float ez = 0;
    public float turretY = 0;
    //服务端补充
    public string id = ""; //代表哪个坦克
}

//开火
public class MsgFire : MsgBase
{
    public MsgFire() { protoName = "MsgFire"; }
    //炮弹初始位置、旋转
    public float x = 0;
    public float y = 0;
    public float z = 0;
    public float ex = 0;
    public float ey = 0;
    public float ez = 0;
    //服务端补充
    public string id = "";  //代表哪个坦克
}

//击中协议
public class MsgHit : MsgBase
{
    public MsgHit() { protoName = "MsgHit"; }
    //击中谁
    public string targetId = "";
    //击中点 可以判断是否有作弊行为
    public float x = 0;
    public float y = 0;
    public float z = 0;
    //服务端补充
    public string id = "";  //哪个坦克
    public int hp = 0;    //被击中坦克的血量
    public int damage = 0;   //受到的伤害
}