using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    //提示文本
    private Text text;
    //确定按钮
    private Button okBtn;

    public override void OnInit()
    {
        skinPath = "Prefab/TipPanel";
        layer = PanelManager.Layer.Tip;
    }
    //显示
    public override void OnShow(params object[] para)
    {
        text = skin.transform.Find("Text").GetComponent<Text>();
        okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
        okBtn.onClick.AddListener(OnOkClick);
        //提示语
        if (para.Length == 1)
        {
            text.text = (string)para[0];
        }
    }

    public override void OnClose()
    {

    }

    //按下确定按钮
    public void OnOkClick()
    {
        Close();
    }
}
