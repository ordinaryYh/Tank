using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //距离矢量
    public Vector3 distance = new Vector3(0, 8, -18);
    //相机
    public Camera myCamera;
    //偏移量
    public Vector3 offset = new Vector3(0, 5f, 0);
    //相机移动速度
    public float speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main;
        //相机初始位置
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        Vector3 initpos = pos - 30 * forward + Vector3.up * 10;
        myCamera.transform.position = initpos;
    }

    //会在所有组件的update之后发生
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        //相机目标位置
        Vector3 targetpos = pos;
        targetpos = pos + forward * distance.z;
        targetpos.y += distance.y;
        //相机位置
        Vector3 cameraPos = myCamera.transform.position;
        cameraPos = Vector3.MoveTowards(cameraPos, targetpos, Time.deltaTime * speed);
        myCamera.transform.position = cameraPos;
        //对准目标
        myCamera.transform.LookAt(pos + offset);
    }
}
