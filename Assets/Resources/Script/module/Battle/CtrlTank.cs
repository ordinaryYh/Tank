using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CtrlTank : BaseTank
{
    //上一次发送同步信息的时间
    private float lastSendSyncTime = 0;
    //同步帧率
    public static float syncInterval = 0.1f;


    public override void Update()
    {
        base.Update();
        MoveUpdate();
        //炮塔控制
        TurretUpdate();
        FireUpdate();
        //发送同步信息
        SyncUpdate();
    }
    public void MoveUpdate()
    {
        if (IsDie())
        {
            return;
        }
        //旋转
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0, x * steer * Time.deltaTime, 0);
        //前进后退
        float y = Input.GetAxis("Vertical");
        transform.position += y * transform.forward * speed * Time.deltaTime;
    }
    public void TurretUpdate()
    {
        if (IsDie())
        {
            return;
        }
        float axis = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            axis = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            axis = 1;
        }
        //旋转角度
        Vector3 le = turret.localEulerAngles;
        le.y += axis * Time.deltaTime * turretSpeed;
        turret.localEulerAngles = le;
    }
    public void FireUpdate()
    {
        if (IsDie())
        {
            return;
        }
        if (!Input.GetKey(KeyCode.Space))
        {
            return;
        }
        if (Time.time - lastFireTime < fireCd)
        {
            return;
        }
        Bullet bullet = Fire();
        //同时要发送协议到服务器
        MsgFire msg = new MsgFire
        {
            x = bullet.transform.position.x,
            y = bullet.transform.position.y,
            z = bullet.transform.position.z,
            ex = bullet.transform.eulerAngles.x,
            ey = bullet.transform.eulerAngles.y,
            ez = bullet.transform.eulerAngles.z
        };
        NetManager.Send(msg);
    }

    //发送同步信息
    public void SyncUpdate()
    {
        //时间间隔判断
        if (Time.time - lastSendSyncTime < syncInterval)
        {
            return;
        }
        lastSendSyncTime = Time.time;
        //发送同步协议
        MsgSyncTank msg = new MsgSyncTank
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z,
            ex = transform.eulerAngles.x,
            ey = transform.eulerAngles.y,
            ez = transform.eulerAngles.z
        };
        NetManager.Send(msg);
    }
}
