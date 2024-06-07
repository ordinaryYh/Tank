using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public override void OnInit()
    {
        skinPath="Prefab/LoginPanel";
        layer=PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] args)
    {
        
    }
    //关闭
    public override void OnClose()
    {
        
    }
}
