using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        testtank();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void testtank()
    {
        GameObject tankObj=new GameObject("myTank");
        CtrlTank ctrlTank=tankObj.AddComponent<CtrlTank>();
        ctrlTank.Init("tankPrefab");
        ctrlTank.AddComponent<CameraFollow>();
    }
}
