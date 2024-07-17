using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string id=""; //当前客户端玩家的id
    
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.Close,OnConnectClose);
        NetManager.AddMsgListener("MsgKick",OnMsgKick);
        //初始化
        PanelManager.Init();
        BattleManager.Init();
        //打开登录
        PanelManager.Open<LoginPanel>();
    }

    void Update()
    {
        NetManager.Update();
    }

    //关闭连接
    void OnConnectClose(string err)
    {
        Debug.Log("断开连接");
    }

    //被踢下线
    void OnMsgKick(MsgBase msgBase)
    {
        PanelManager.Open<TipPanel>("被踢下线");
    }
}
