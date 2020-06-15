using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class HolePlacer : MonoBehaviour
{
    private bool isGenerated = false;
    public GameObject holesPrefab;
    public GameObject placementIndicator;
    public GameObject molePrefab;
    private bool placementPoseIsValid = false;



    public void GenerateMoles(GameObject holes)
    {
        //
        foreach (Transform t in holes.transform)
        {
            GameObject child = t.gameObject;
            if (child.tag == "Hole") //in unity we had already tagged the hole prefab as "Hole", so we can track those hole's position.
            {
                Vector3 pos = child.transform.position;
                Instantiate(molePrefab, pos, molePrefab.transform.rotation);
            }
        }
    }


    public void UpdatePlacementPose()
    {
        

        /*place the indicator on the hited plane from screen center*/
      
        ARPoint centerpoint = new ARPoint
        {
            x = 0.5,
            y = 0.5
        };
        //viewportpoint !
 

        List<ARHitTestResult> hits = UnityARSessionNativeInterface
            .GetARSessionNativeInterface()
            .HitTest(centerpoint, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
    
        placementPoseIsValid = (hits.Count > 0);
        
        /*if plane is found, show the indicator on it   */
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            
            placementIndicator.transform.position= UnityARMatrixOps.GetPosition(hits[0].worldTransform);
            placementIndicator.transform.rotation = UnityARMatrixOps.GetRotation(hits[0].worldTransform);

            /*if tap the screen instantiate holes where the indicator is, and destroy the indicator */
            if (Input.touchCount > 0&&Input.GetTouch(0).phase==TouchPhase.Began)
            {
                    GameObject holes = Instantiate(holesPrefab);
                    holes.transform.position = UnityARMatrixOps.GetPosition(hits[0].worldTransform);
                    holes.transform.rotation = UnityARMatrixOps.GetRotation(hits[0].worldTransform);
                    GenerateMoles(holes);
                    Destroy(placementIndicator);
                    this.isGenerated = true;
                    
                
            }
            
        }
        else
        {
            placementIndicator.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
       /*if holes are not placed */
        if (!isGenerated)
        {
            UpdatePlacementPose();
            
        }
    }
}