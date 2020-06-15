using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Go2Next : MonoBehaviour
{
    // public static int currentObjectIndex;
    // Start is called before the first frame update
    void Start()
    {
        StaticObject.currentOrder = 0;
    
    }

    // Update is called once per frame
    public void go()
    {
        if (StaticObject.currentOrder < firestore.PrecisARList.Count-1)
        {
            StaticObject.currentOrder++;
            init();
        }
    }
    public static void init()
    {
        print("init "+ firestore.PrecisARList[StaticObject.currentOrder].name);
        StaticObject.SetLayerRecursively(firestore.PrecisARList[StaticObject.currentOrder], 0);
    }
}