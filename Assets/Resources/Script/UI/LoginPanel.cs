using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button regBtn;
    public override void OnInit()
    {
        skinPath="Prefab/LoginPanel";
        layer=PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] args)
    {
        idInput=skin.transform.Find("IdInput").GetComponent<InputField>();
        pwInput=skin.transform.Find("PwInput").GetComponent<InputField>();
        loginBtn=skin.transform.Find("LoginBtn").GetComponent<Button>();
        regBtn=skin.transform.Find("RegisterBtn").GetComponent<Button>();
        //监听
        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
        //网络协议监听
        
    }
    //关闭
    public override void OnClose()
    {
        
    }
}
