using System;
//注册
public class MsgRegister:MsgBase
{
    public MsgRegister() {protoName="MsgRegister";}
    //客户端发
    public string id="";
    public string pw="";
    //服务端返回 0代表成功 1代表失败
    public int result=0;
}

//登录
public class MsgLogin:MsgBase
{
    public MsgLogin(){protoName="MsgLogin";}
    public string id="";
    public string pw="";
    //服务端返回 0代表成功 1代表失败
    public int result=0;
}

//踢下线
public class MsgKick:MsgBase
{
    public MsgKick()
    {
        protoName="MsgKick";
    }
    //原因 0代表其他人登录同一账号
    public int reason=0;
}