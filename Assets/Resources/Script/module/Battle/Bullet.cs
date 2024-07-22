using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //移动速度
    public float speed = 100f;
    //发射者
    public BaseTank tank;
    //炮弹模型
    GameObject skin;
    //物理
    Rigidbody rigidBody;

    public void Init()
    {
        GameObject skinRes = Resources.Load("Prefab/BulletPrefab") as GameObject;
        skin = Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;
        //物理
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    void OnCollisionEnter(Collision other)
    {
        GameObject collObj = other.gameObject;
        BaseTank hitTank = collObj.GetComponent<BaseTank>();
        if (hitTank == tank)
        {
            return;
        }
        if (hitTank != null)
        {
            //发送协议，这里不处理坦克的血量，由服务器来进行判断
            SendMsgHit(tank, hitTank);
            //hitTank.Attacked(35);
        }
        GameObject explode = Resources.Load("Prefab/fire") as GameObject;
        Instantiate(explode, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    //发送伤害协议
    void SendMsgHit(BaseTank tank, BaseTank hitTank)
    {
        if (hitTank == null || tank == null)
        {
            return;
        }
        //不是自己发出的炮弹
        if (tank.id != GameManager.id)
        {
            return;
        }
        MsgHit msg = new MsgHit
        {
            targetId = hitTank.id,
            id = tank.id,
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z
        };
        NetManager.Send(msg);
    }
}
