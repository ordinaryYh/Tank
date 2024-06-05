using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CtrlTank : BaseTank
{
    public override void Update()
    {
        base.Update();
        MoveUpdate();
        //炮塔控制
        TurretUpdate();
    }
    public void MoveUpdate()
    {
        //旋转
        float x =Input.GetAxis("Horizontal");
        transform.Rotate(0,x*steer*Time.deltaTime,0);
        //前进后退
        float y=Input.GetAxis("Vertical");
        transform.position+=y*transform.forward*speed*Time.deltaTime;
    }
    public void TurretUpdate()
    {
        float axis=0;
        if(Input.GetKey(KeyCode.Q))
        {
            axis=-1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            axis=1;
        }
        //旋转角度
         Vector3 le=turret.localEulerAngles;
         le.y+=axis*Time.deltaTime*turretSpeed;
         turret.localEulerAngles=le;
    }
}
