
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class GameScript : MonoBehaviour
{
    public GameObject prefab;
    int counter = 0;
    LocationInfo location;
    const double lat2km = 111319.491;
   
    public Vector3 ConvertGPS2ARCoordinate(LocationInfo location, Vector3 prefabLoc)
    {
        double dy = (prefabLoc.y - location.altitude);
        double dz = (prefabLoc.z - location.latitude) * lat2km;   // ＋zが南方向
        double dx = (prefabLoc.x - location.longitude) * lat2km; // +xが東方向
        return new Vector3((float)dx, (float)dy, (float)dz); // なんでy軸での表現がなさそう？下にいるはずなのに
    }

    void Start()
    {
        Input.location.Start();

        //Instantiate(mytext, this.transform.position, this.transform.rotation);

        InvokeRepeating("UpdateGPS", 0.2f, 0.5f);
        InvokeRepeating("randomInstanciate", 0.2f, 5f);
        InvokeRepeating("UpdatePosition", 10f, 10f);

    }
    public void UpdatePosition()
    {
        GameObject[] placeTotal = GameObject.FindGameObjectsWithTag(tag: "place");
        foreach (GameObject p in placeTotal){
          
            GPSdata GPSdataScript = p.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
            Vector3 positionOfP = new Vector3((float)GPSdataScript.longitude, (float)GPSdataScript.height, (float)GPSdataScript.latitude);
            p.transform.position = ConvertGPS2ARCoordinate(location,positionOfP);
            p.transform.LookAt(Camera.main.transform);
            p.transform.Rotate(new Vector3(0, 180, 0));
        } 

    }
    public void randomInstanciate()
    {
        float x = location.longitude + Random.Range(-0.0003f, 0.0003f);
        float y = location.altitude + Random.Range(-10f, 10f);
        float z = location.latitude + Random.Range(-0.0003f, 0.0003f);
        Vector3 randomPos = new Vector3(x,y,z);
        Vector3 relatifPos = ConvertGPS2ARCoordinate(location, randomPos);
        GameObject place=Instantiate(prefab, relatifPos, transform.rotation);
        place.name = counter.ToString();
        place.tag = "place";
        GPSdata GPSdataScript=place.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
        GPSdataScript.place_name = place.name;
        GPSdataScript.longitude = x;
        GPSdataScript.latitude = z;
        GPSdataScript.height = y;
        GPSdataScript.GPSInfo = true;

        counter++;
        GameObject.Find("Counter").GetComponent<Text>().text = "There are " + counter.ToString() + "Xmas trees next to you!";

        
    }

    // Update is called once per frame
  
        


    public void UpdateGPS()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {

                location = Input.location.lastData;

                GameObject.Find("altitude").GetComponent<Text>().text = "( "+location.longitude.ToString()+" , "+location.altitude.ToString()+ " , " + location.latitude.ToString()+" )";


            }
        }
    }

}