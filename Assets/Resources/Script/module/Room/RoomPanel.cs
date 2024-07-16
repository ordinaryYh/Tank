using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    //开战按钮
    private Button startButton;
    //退出按钮
    private Button closeButton;
    //列表容器
    private Transform content;
    //玩家信息物体
    private GameObject playerObj;

    //初始化
    public override void OnInit()
    {
        skinPath="Prefab/RoomPanel";
        layer=PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] para)
    {
        startButton=skin.transform.Find("CtrlPanel/StartButton").GetComponent<Button>();
        closeButton=skin.transform.Find("CtrlPanel/CloseButton").GetComponent<Button>();
        content=skin.transform.Find("ListPanel/Scroll View/Viewport/Content");
        playerObj=skin.transform.Find("Player").gameObject;
        //不激活玩家信息
        playerObj.SetActive(false);
        //按钮事件
        startButton.onClick.AddListener(OnStartClick);
        closeButton.onClick.AddListener(OnCloseClick);
        //协议监听
        NetManager.AddMsgListener("MsgGetRoomInfo",OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom",OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgStartBattle",OnMsgStartBattle);
        //发送查询
        MsgGetRoomInfo msg=new MsgGetRoomInfo();
        NetManager.Send(msg);
    }

    //关闭
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetRoomInfo",OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgLeaveRoom",OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgStartBattle",OnMsgStartBattle);
    }

    //收到玩家列表信息协议
    public void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        MsgGetRoomInfo msg=(MsgGetRoomInfo)msgBase;
        //清除玩家列表
        for(int i=content.childCount-1;i>=0;i--)
        {
            GameObject o=content.GetChild(i).gameObject;
            Destroy(o);
        }
        //重新生成列表
        if(msg.players==null)
        {
            return;
        }
        for(int i=0;i<msg.players.Length;i++)
        {
            GeneratePlayerInfo(msg.players[i]);
        }
    }
    //创建一个玩家
    public void GeneratePlayerInfo(PlayerInfo playerInfo)
    {
        //创建物体
        GameObject o=Instantiate(playerObj);
        o.transform.SetParent(content);
        o.SetActive(true);
        o.transform.localScale=Vector3.one;
        //获取组件
        Text idText=o.transform.Find("IdText").GetComponent<Text>();
        Text campText=o.transform.Find("CampText").GetComponent<Text>();
        Text scoreText=o.transform.Find("ScoreText").GetComponent<Text>();
        //填充信息
        idText.text=playerInfo.id;
        if(playerInfo.camp==1)
        {
            campText.text="红";
        }
        else
        {
            campText.text="蓝";
        }
        if(playerInfo.isOwner==1)
        {
            campText.text=campText.text+"!";
        }
        scoreText.text=playerInfo.win+"胜"+playerInfo.lost+"负";
    }

    //玩家点击退出按钮
    public void OnCloseClick()
    {
        MsgLeaveRoom msg=new MsgLeaveRoom();
        NetManager.Send(msg);
    }
    //玩家收到退出协议
    public void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg=(MsgLeaveRoom)msgBase;
        //成功退出房间
        if(msg.result==0)
        {
            PanelManager.Open<TipPanel>("退出房间");
            PanelManager.Open<RoomListPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("退出房间失败");
        }
    }

    //玩家点击开战按钮
    public void OnStartClick()
    {
        MsgStartBattle msg=new MsgStartBattle();
        NetManager.Send(msg);
    }
    //玩家收到开战协议
    public void OnMsgStartBattle(MsgBase msgBase)
    {
        MsgStartBattle msg=(MsgStartBattle)msgBase;
        if(msg.result==0)
        {
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("开战失败！两队至少都需要一名玩家，只有对战可以开战");
        }
    }
}
