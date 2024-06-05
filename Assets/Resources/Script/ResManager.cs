using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager : MonoBehaviour
{
    public static GameObject LoadPrefab(string path)
    {
        return Resources.Load("Prefab/TankPrefab/"+path) as GameObject;
    }
}
