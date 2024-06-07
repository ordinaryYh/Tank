using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public string skinPath;
    public GameObject skin;
    //层级
    public PanelManager.Layer layer=PanelManager.Layer.Panel;
    public void Init()
    {
        GameObject skinPrefab=Resources.Load(skinPath) as GameObject;
        skin=Instantiate(skinPrefab);
    }
    //关闭
    public void Close()
    {
        //name就是当前的类的名字，当其他xxxpanel集成BasePanel时，name就是xxxpanel
        string name=this.GetType().ToString();
        PanelManager.Close(name);
    }
    
    //初始化时
    public virtual void OnInit()
    {
        
    }
    //显示时
    //params object[] para这个代表任意长度的参数
    public virtual void OnShow(params object[] para){

    }
    //关闭时
    public virtual void OnClose(){

    }
}
