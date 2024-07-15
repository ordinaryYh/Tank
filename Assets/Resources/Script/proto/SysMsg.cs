using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//心跳机制的协议
public class MsgPing:MsgBase
{
    public MsgPing()
    {
        protoName="MsgPing";
    }
}

public class MsgPong:MsgBase
{
    public MsgPong()
    {
        protoName="MsgPong";
    }
}
