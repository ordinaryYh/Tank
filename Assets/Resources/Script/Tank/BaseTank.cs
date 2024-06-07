using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTank : MonoBehaviour
{
    //这个代表坦克模型
    private GameObject skin;
    //转向速度
    public float steer=20f;
    //移动速度
    public float speed=3f;
    //炮塔旋转速度
    public float turretSpeed=30f;
    //炮塔
    public Transform turret;
    //炮管
    public Transform gun;
    //发射点
    public Transform firePoint;
    protected Rigidbody rigidBody;

    //炮弹cd时间
    public float fireCd=0.5f;
    //上一次发射炮弹的时间
    public float lastFireTime=0;
    //tank生命值
    public float hp=100;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public virtual void Init(string skinPath)
    {
        GameObject skinRes=ResManager.LoadPrefab(skinPath);
        skin=Instantiate(skinRes);
        skin.transform.parent=this.transform;
        skin.transform.localPosition=Vector3.zero;
        //skin.transform.localEulerAngles代表的是相对父节点的旋转角度
        skin.transform.localEulerAngles=Vector3.zero; 
        turret=skin.transform.Find("Turret");
        gun=turret.transform.Find("Gun");
        firePoint=gun.transform.Find("FirePoint");

        //添加物理组件
        rigidBody=gameObject.AddComponent<Rigidbody>();
        BoxCollider boxCollider=gameObject.AddComponent<BoxCollider>();
        boxCollider.center=new Vector3(0,2.5f,1.47f);
        boxCollider.size=new Vector3(7,5,12);
    }
    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    public Bullet Fire()
    {
        if(IsDie())
        {
            return null;
        }
        GameObject bulletObj=new GameObject("bullet");
        Bullet bullet=bulletObj.AddComponent<Bullet>();
        bullet.Init();
        bullet.tank=this;
        //位置
        bullet.transform.position=firePoint.position;
        bullet.transform.rotation=firePoint.rotation;
        //更新时间
        lastFireTime=Time.time;
        return bullet;
    }
    public bool IsDie()
    {
        return hp<=0;
    }
    public void Attacked(float att)
    {
        if(IsDie())
        {
            return;
        }
        hp-=att;
        if(IsDie())
        {
            GameObject obj=Resources.Load("Prefab/explosion") as GameObject;
            GameObject explosion=Instantiate(obj,transform.position,transform.rotation);
            explosion.transform.SetParent(this.transform);
        }
    }
}
