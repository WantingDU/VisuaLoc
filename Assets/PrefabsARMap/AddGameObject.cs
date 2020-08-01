using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;

public class AddGameObject : MonoBehaviour
{
    public static GameObject Prefab2Instantiate;
    public static bool alreadyAdd = false;
    private void OnDestroy()
    {
        alreadyAdd = false;
    }
    void Start()
	{
        Prefab2Instantiate = Resources.Load<GameObject>("InfoPanel");
    }
    public void chooseObject()
    {
        StaticObject.AddClicked = true;
        string ButtonnName = EventSystem.current.currentSelectedGameObject.name.Replace("Button", ""); // to replace the specific text with blank;
        Prefab2Instantiate = Resources.Load<GameObject>(ButtonnName);
    }
    void HitTest(ARPoint point)
    {
        
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface
            .GetARSessionNativeInterface()
            .HitTest(point, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
        
        // 平面とあたっていた場合
        if (hitResults.Count > 0&&StaticObject.AddClicked)
        {
            
            alreadyAdd = true;
            GameObject g;
            g = Instantiate(Prefab2Instantiate);
            g.name = Prefab2Instantiate.name;
            //g.AddComponent<DontDestroyOnLoad>();
            g.transform.position = UnityARMatrixOps.GetPosition(hitResults[0].worldTransform);
            g.transform.rotation = UnityARMatrixOps.GetRotation(hitResults[0].worldTransform);
            
            

        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!alreadyAdd) {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    StaticObject.photoAdded = false;
                    var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                    ARPoint point = new ARPoint
                    {
                        x = screenPosition.x,
                        y = screenPosition.y
                    };

                    HitTest(point);
                }
            }

        }
    }
    
}