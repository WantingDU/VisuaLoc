using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    float y;
    public static List<GameObject> radarList;
    public static List<GameObject> borderList;
    public static GameObject radarPrefab;
    public float switchDistance;
    public Transform helpTransform;
    private void OnDestroy()
    {
        radarList = null;
        borderList = null;

    }
    void Awake()
    {

        y = -GameObject.Find("PlayerPoint").transform.position.y;
        radarPrefab = Resources.Load<GameObject>("TargetPoint");
        radarList = new List<GameObject>();
        borderList = new List<GameObject>();

        firestore.PrecisARList = new List<GameObject>();
        /* Test  用
        GameObject cube = GameObject.Find("Cube1");
        firestore.PrecisARList.Add(cube);
        GameObject cube2 = GameObject.Find("Cube2");
        firestore.PrecisARList.Add(cube2);
        */

        InitTargets();


    }

    // Update is called once per frame
    void Update()
    {
        if (radarList.Count == 0 || borderList.Count == 0)
        {
            return;
        }
        for (int i = 0; i < radarList.Count; i++)
        {
            if (Vector3.Distance(radarList[i].transform.position, transform.position) > switchDistance)
            {
                helpTransform.LookAt(firestore.PrecisARList[i].transform);
                borderList[i].transform.position = Camera.main.transform.position + switchDistance * helpTransform.forward;

                borderList[i].layer = LayerMask.NameToLayer("Radar");
                radarList[i].layer = LayerMask.NameToLayer("Invisible");

            }
            else
            {
                borderList[i].layer = LayerMask.NameToLayer("Invisible");
                radarList[i].layer = LayerMask.NameToLayer("Radar");
                //switch back
            }
        }
    }

    public void InitTargets()
    {
       
        radarList = new List<GameObject>();
        borderList = new List<GameObject>();
        OnDestroyTarget();
        if (firestore.PrecisARListValid)
        {
            if (firestore.PrecisARList.Count != 0)
            {
                print("firestore.PrecisARList != null" + firestore.PrecisARList.Count);
                foreach (GameObject panel in firestore.PrecisARList)
                {
                    print("panel.transform.position" + panel.transform.position.x + "," + panel.transform.position.y + "," + panel.transform.position.z);
                    GameObject target = Instantiate(radarPrefab, panel.transform.position, Quaternion.identity);
                    print("TargetPoint.transform.position" + panel.transform.position.x + "," + panel.transform.position.z + 300 + "," + y);
                    target.tag = "Target";
                    target.name = panel.name + "Radar";
                    radarList.Add(target);
                    GameObject targetBorder = Instantiate(radarPrefab, panel.transform.position, Quaternion.identity);
                    borderList.Add(targetBorder);
                    targetBorder.tag = "Target";
                    targetBorder.name = panel.name + "Border";

                }
                print("panelexist is now available");
                return;
            }
        }
                
        else
        {
            if(StaticObject.myARmapName!= "Default")
            {
                Invoke("InitTargets",0.3f);
            }
            
        }

    }
    private void OnDestroyTarget()
    {
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }
        
    }
}
