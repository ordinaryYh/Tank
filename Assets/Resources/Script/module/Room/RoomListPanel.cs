using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    //账号文本
    private Text idText;
    //战绩文本
    private Text scoreText;
    //创建房间按钮
    private Button createButton;
    //刷新列表按钮
    private Button reflashButton;
    //列表容器
    private Transform content;
    //房间列表
    private GameObject roomObj;

    //初始化
    public override void OnInit()
    {
        skinPath="Prefab/RoomListPanel";
        layer=PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] para)
    {
        //寻找组件
        idText=skin.transform.Find("InfoPanel/IdText").GetComponent<Text>();
        scoreText=skin.transform.Find("InfoPanel/IdText").GetComponent<Text>();
        createButton=skin.transform.Find("CtrlPanel/CreateButton").GetComponent<Button>();
        reflashButton=skin.transform.Find("CtrlPanel/ReflashButton").GetComponent<Button>();
        content=skin.transform.Find("ListPanel/Scroll View/Viewport/Content");
        roomObj=skin.transform.Find("Room").gameObject;
        //按钮事件
        createButton.onClick.AddListener(OnCreateClick);
        reflashButton.onClick.AddListener(OnReflashClick);
        //不激活房间
        roomObj.SetActive(false);
        //显示id
        idText.text=GameManager.id;

        //相关协议的监听
        NetManager.AddMsgListener("MsgGetAchieve",OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgGetRoomList",OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgCreateRoom",OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgEnterRoom",OnMsgEnterRoom);
        //发送查询
        MsgGetAchieve msgGetAchieve=new MsgGetAchieve();
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgGetRoomList=new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }

    //关闭
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetAchieve",OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgGetRoomList",OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgCreateRoom",OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgEnterRoom",OnMsgEnterRoom);
    }


    //收到战绩查询协议
    public void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg=(MsgGetAchieve)msgBase;
        scoreText.text=msg.win+"胜"+msg.lost+"负";
    }
    //收到房间列表协议
    public void OnMsgGetRoomList(MsgBase msgBase)
    {
        MsgGetRoomList msg=(MsgGetRoomList)msgBase;
        //清除房间列表
        for(int i=content.childCount-1;i>=0;i--)
        {
            GameObject o=content.GetChild(i).gameObject;
            Destroy(o);
        }
        //如果没有房间，不需要进一步处理
        if(msg.rooms==null) return;
        for(int i=0;i<msg.rooms.Length;i++)
        {
            GenerateRoom(msg.rooms[i]);
        }
    }
    //创建一个房间
    public void GenerateRoom(RoomInfo roomInfo)
    {
        //创建物品
        GameObject o=Instantiate(roomObj);
        o.transform.SetParent(content);
        o.SetActive(true);
        o.transform.localScale=Vector3.one;
        //获取组件
        Text idText=o.transform.Find("IdText").GetComponent<Text>();
        Text countText=o.transform.Find("CountText").GetComponent<Text>();
        Text statusText=o.transform.Find("StatusText").GetComponent<Text>();
        Button btn=o.transform.Find("JoinButton").GetComponent<Button>();
        //填充信息
        idText.text=roomInfo.id.ToString();
        countText.text=roomInfo.count.ToString();
        if(roomInfo.status==0)
        {
            statusText.text="准备中";
        }
        else
        {
            statusText.text="战斗中";
        }
        //按钮事件
        btn.name=idText.text;
        btn.onClick.AddListener(delegate(){
            OnJoinClick(btn.name); //这个函数是点击加入房间按钮执行的函数
        });
    }
    //点击加入房间按钮
    public void OnJoinClick(string id)
    {
        MsgEnterRoom msg=new MsgEnterRoom();
        msg.id=int.Parse(id); //将字符串转化为整形
        NetManager.Send(msg);
    }

    //收到进入房间协议
    public void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msg=(MsgEnterRoom)msgBase;
        //成功进入房间
        if(msg.result==0)
        {
            PanelManager.Open<RoomPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("进入房间失败");
        }
    }

    //玩家点击创建房间按钮
    public void OnCreateClick()
    {
        MsgCreateRoom msg=new MsgCreateRoom();
        NetManager.Send(msg);
    }
    //收到新建房间协议
    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msg=(MsgCreateRoom)msgBase;
        //成功创建房间
        if(msg.result==0)
        {
            PanelManager.Open<TipPanel>("创建成功");
            PanelManager.Open<RoomPanel>();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("创建房间失败");
        }
    }

    //玩家点击刷新按钮
    public void OnReflashClick()
    {
        MsgGetRoomList msg=new MsgGetRoomList();
        NetManager.Send(msg);
    }
}
