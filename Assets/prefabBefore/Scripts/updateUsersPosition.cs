using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateUsersPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateUserPosition", 0f, 1f);
    }

    // Update is called once per frame
    public static void UpdateUserPosition()
    {
        if (FirebaseUsers.UsersExist.Count > 0)
        {
            if (altitudeText.locationChanged) {
                foreach (GameObject u in FirebaseUsers.UsersExist)
                {
                    //print("location of user should be updated");
                    GPSdata GPSdataScript = u.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
                    Vector3 positionOfU = new Vector3((float)GPSdataScript.longitude, (float)GPSdataScript.height, (float)GPSdataScript.latitude);
                    //u.transform.position = FirebaseScript.ConvertGPS2ARCoordinate(altitudeText.location, positionOfU);
                    u.transform.position = Camera.main.transform.position + CommonVariables.ConvertGPS2ARCoordinate(altitudeText.location, positionOfU) + new Vector3(0f, 0.5f, 0f);
                    //GetComponent<Camera>().transform.position
                    u.transform.LookAt(Camera.main.transform);
                    u.transform.Rotate(new Vector3(0, 180, 0));
                }
            }
        }
        
    }
}
