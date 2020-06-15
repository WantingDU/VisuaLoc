using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;


/*
        This Script is for updating the present location and its UIText 

 */
public class altitudeText : MonoBehaviour
{
    public static LocationInfo location;
    public static Vector3 locationVec;
    int i;
    Text coordinate;
    public static bool locationChanged;
    void Start()
    {
        coordinate = GameObject.Find("altitude").GetComponent<Text>();
        Input.location.Start();
        //Thread thread = new Thread(new ThreadStart(UpdateGPS));
        //thread.Start();
        InvokeRepeating("UpdateGPS", 0f, 1f);
    }


    public void UpdateGPS()
    {
            if (Input.location.isEnabledByUser)
            {
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    locationChanged = !(location.Equals(Input.location.lastData));

                    location = Input.location.lastData;
                    locationVec = new Vector3(location.longitude, location.altitude, location.latitude);
                    i++;
                    coordinate.text = i + locationVec.ToString();
                    
                }
            }
        
    }
}
