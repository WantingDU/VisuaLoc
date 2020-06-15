using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ObjectScaler : MonoBehaviour
{

    private Touch oldTouch1;  //上次触摸点1(手指1)
    private Touch oldTouch2;  //上次触摸点2(手指2)

    private float _prevouseClick;
    // Start is called before the first frame update
    void Start()
    {
        _prevouseClick = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Touch newTouch1 = Input.GetTouch(0);
            Touch newTouch2 = Input.GetTouch(1);
            //第2点刚开始接触屏幕, 只记录，不做处理
            if (newTouch2.phase == TouchPhase.Began)
            {
                oldTouch2 = newTouch2;
                oldTouch1 = newTouch1;
                return;
            }
            //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型
            float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
            float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);
            //两个距离之差，为正表示放大手势， 为负表示缩小手势
            float offset = newDistance - oldDistance;
            //放大因子， 一个像素按 0.01倍来算(100可调整)
            float scaleFactor = offset / 50f;
            Vector3 localScale = transform.localScale;
            Vector3 scale = new Vector3(localScale.x + scaleFactor,
                localScale.y + scaleFactor,
                localScale.z + scaleFactor);
            //在什么情况下进行缩放
            if (scale.x >= 0.05f && scale.y >= 0.05f && scale.z >= 0.05f)
            {
                transform.localScale = scale;
            }
            //记住最新的触摸点，下次使用
            oldTouch1 = newTouch1;
            oldTouch2 = newTouch2;
        }/*
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform == transform)
            {
                if ((Time.realtimeSinceStartup - _prevouseClick) < 0.1f)
                {
                    //注意：要双击的物体上一定要有碰撞器，并且碰撞器和本脚本挂在同一个物体上
                    Debug.Log("double click on object");
                    if (transform.name != "InfoPanel")
                    {
                        AddGameObject.alreadyAdd = false;
                        GameObject g=Instantiate(Resources.Load<GameObject>(gameObject.name+"Simple"),transform.position,transform.rotation);
                        string g_GUID = StaticObject.getGUID();
                        g.name = g_GUID;
                        firestore.PrecisARList.Add(g);
                        List<float> myPosition = new List<float>() { g.transform.position.x, g.transform.position.y, g.transform.position.z,
                     g.transform.rotation.x, g.transform.rotation.y, g.transform.rotation.z,g.transform.rotation.w };
                        StaticObject.addedGO.Add(myPosition, new List<string>() { gameObject.name + "Simple", null, null, g_GUID, Auth.UserSelfId, "true"});
                        //StaticObject.addedGO.Add(myPosition, new List<string>() { InfoPanel2Instantiate.name, title, contents, g_GUID, Auth.UserSelfId, Ispublic.ToString() });
                        GameObject target = Instantiate(CompassController.radarPrefab, g.transform.position, Quaternion.identity);
                        target.tag = "Target";
                        target.name = g.name + "Radar";
                        CompassController.radarList.Add(target);
                        GameObject targetBorder = Instantiate(CompassController.radarPrefab, g.transform.position, Quaternion.identity);
                        CompassController.borderList.Add(targetBorder);
                        targetBorder.tag = "Target";
                        targetBorder.name = g.name + "Border";
                        print("just destroyed!");
                        Destroy(gameObject);
                    }
                }
            
                else
                {
                    _prevouseClick = Time.realtimeSinceStartup;
                }
            }
        }
        */
    }


}

