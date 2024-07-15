using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    float timer=0;
    GameObject tankObj;
    // Start is called before the first frame update
    void Start()
    {
        testtank();
        PanelManager.Init();
        PanelManager.Open<LoginPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if(timer>=2f)
        {
            tankObj.GetComponent<CtrlTank>().Attacked(100);
            timer=0;
        }
    }

    public void testtank()
    {
        tankObj=new GameObject("myTank");
        CtrlTank ctrlTank=tankObj.AddComponent<CtrlTank>();
        ctrlTank.Init("tankPrefab");
        ctrlTank.AddComponent<CameraFollow>();
    }
}
