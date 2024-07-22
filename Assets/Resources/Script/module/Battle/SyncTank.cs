using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

//会使用预测算法来实现状态同步
public class SyncTank : BaseTank
{
    //预测信息，哪个时间到达哪个位置
    Vector3 lastPos;
    Vector3 lastRot;
    Vector3 forecastPos;  //预测
    Vector3 forecastRot;   //预测
    float forecastTime;   //代表最近一次收到位置同步协议的时间


    //重写Init函数
    public override void Init(string skinPath)
    {
        base.Init(skinPath);
        //不受物理运动影响
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        rigidBody.useGravity = false;
        //初始化预测信息
        lastPos = transform.position;
        lastRot = transform.eulerAngles;
        forecastPos = transform.position;
        forecastRot = transform.eulerAngles;
        forecastTime = Time.time;
    }

    new void Update()
    {
        base.Update();
        //更新位置，使用预测算法
        ForecastUpdate();
    }


    //更新位置
    public void ForecastUpdate()
    {
        //时间,将t归一化
        float t = (Time.time - forecastTime) / CtrlTank.syncInterval;
        t = Mathf.Clamp(t, 0f, 1f);
        //移动到预测地点
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos, t);
        transform.position = pos;
        //旋转到预测角度
        Quaternion quat = transform.rotation;
        Quaternion forecastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forecastQuat, t);
        transform.rotation = quat;
    }

    //位置预测算法的实现
    //移动同步的函数
    public void SyncPos(MsgSyncTank msg)
    {
        //预测位置
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        forecastPos = pos + 2 * (pos - lastPos);
        forecastRot = rot + 2 * (rot - lastRot);
        //更新
        lastPos = pos;
        lastRot = rot;
        forecastTime = Time.time;
        //炮塔
        Vector3 le = turret.localEulerAngles;
        le.y = msg.turretY;
        turret.localEulerAngles = le;
    }
    //炮弹的同步
    public void SyncFire(MsgFire msg)
    {
        Bullet bullet = Fire();
        //更新坐标
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        bullet.transform.position = pos;
        bullet.transform.eulerAngles = rot;
    }
}


