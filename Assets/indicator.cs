using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
public class indicator : MonoBehaviour
{
    public GameObject placementIndicator;
    private bool placementPoseIsValid = false;
    public GameObject infoPanelPrefab;

    public static bool addedNewPanel=false;
    public static string postInfo;
    public static bool token;
    public ARPoint centerpoint;
    public static GameObject infoPanel;
    public  static GameObject indicator2;
    // Start is called before the first frame update
    void Start()
    {
        Input.location.Start();
        indicator2 = Instantiate(placementIndicator);
        centerpoint = new ARPoint
        {
            x = 0.5,
            y = 0.5
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!addedNewPanel)
        {
            UpdatePlacementPose();

        }
    }
    
    public void UpdatePlacementPose()
    {


        /*place the indicator on the hited plane from screen center*/

       
        List<ARHitTestResult> hits = UnityARSessionNativeInterface
            .GetARSessionNativeInterface()
            .HitTest(centerpoint, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);

        placementPoseIsValid = (hits.Count > 0);

        /*if plane is found, show the indicator on it   */
        if (placementPoseIsValid)
        {

            indicator2.SetActive(true);
            indicator2.transform.position = UnityARMatrixOps.GetPosition(hits[0].worldTransform);
            indicator2.transform.rotation = UnityARMatrixOps.GetRotation(hits[0].worldTransform);
            
            print("indicator position is" + indicator2.transform.position.x + "," + indicator2.transform.position.y + "," + indicator2.transform.position.z);

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                infoPanel = Instantiate(infoPanelPrefab);
                infoPanel.transform.position = UnityARMatrixOps.GetPosition(hits[0].worldTransform);
                infoPanel.transform.LookAt(Camera.main.transform);
                infoPanel.transform.Rotate(new Vector3(0, 180, 0));



                addedNewPanel = !addedNewPanel;
            }
        }
        else
        {
            indicator2.SetActive(false);
        }

    }
    
}
