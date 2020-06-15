using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using System.Globalization;


/*
        This Script is for retrieving data from DB and instantiating the appropriate Gameobjects .

 */

public class Place
{
    public string placeName;
    public Vector3 coordinate;

    public Place()
    {
    }

    public Place(string placeName, Vector3 coordinate)
    {
        this.placeName = placeName;
        this.coordinate = coordinate;
    }
}


public class FirebaseScript : MonoBehaviour
{
    
    public GameObject prefab;
    public static int counter2 = 0;
    const double lat2km = 111319.491;
    //DatabaseReference reference=generatePlaces.reference;
    public static DatabaseReference placesRef;
    public static bool stop = false;
    public static List<GameObject> placesExist = new List<GameObject> ();
    public static GameObject CounterObj ;
    public static bool ready = false;
    //GameObject ChangeObj;




    public void Start()
    {
        //CounterObj = GameObject.Find("Counter");
        counter2++;
       // CounterObj.GetComponent<Text>().text = "this is " + counter2.ToString() + "time this scene is started!";
        ready = true;
        //ChangeObj = GameObject.Find("d");
        placesRef = FirebaseDatabase.DefaultInstance.GetReference("places");
        placesRef.ChildAdded += HandleChildAdded;
        placesRef.ChildChanged += HandleChildChanged;
        Invoke("InstantiateFromDB", 0f);
        //InvokeRepeating("UpdatePosition", 30f, 30f);
        
        
    }

    public void UpdatePosition()
    {
        
        foreach (GameObject p in placesExist)
        {

            GPSdata GPSdataScript = p.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
            Vector3 positionOfP = new Vector3((float)GPSdataScript.longitude, (float)GPSdataScript.height, (float)GPSdataScript.latitude);
            p.transform.position = Camera.main.transform.position + CommonVariables.ConvertGPS2ARCoordinate(CommonVariables.location, positionOfP);
            p.transform.LookAt(Camera.main.transform);
            p.transform.Rotate(new Vector3(0, 180, 0));
        }

    }
    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (!stop)
        {
            placesRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    return;
                }
                else if (task.IsCompleted)
                {

                    string numero = args.Snapshot.Key;
                    print(numero);
                    string placeName = args.Snapshot.Child("placeName").Value.ToString();
                   
                    float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    Vector3 coordi1 = new Vector3(x, y, z);


                    Vector3 relatifPos = CommonVariables.ConvertGPS2ARCoordinate(CommonVariables.location, coordi1);
                    GameObject place = Instantiate(prefab, relatifPos, transform.rotation);
                    FirebaseScript.placesExist.Add(place);

                   
                    place.name = numero;
                    place.tag = "place";
                    
                    GPSdata GPSdataScript = place.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
                    GPSdataScript.place_name = placeName;
                    GPSdataScript.longitude =x;
                    GPSdataScript.latitude = z;
                    GPSdataScript.height = y;
                    GPSdataScript.GPSInfo = true;
                    

                }
                

            }

            );
        }

    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        GameObject changedObject= GameObject.Find(args.Snapshot.Key);

        string placeName = args.Snapshot.Child("placeName").Value.ToString();
        float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
     
        GPSdata GPSdataScript = changedObject.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
        GPSdataScript.place_name = placeName;
        GPSdataScript.longitude = x;
        GPSdataScript.latitude = z;
        GPSdataScript.height = y;
        

    }

    public void InstantiateFromDB()
    {
        if (!stop)
        {
            placesRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    int totalNum = (int)snapshot.ChildrenCount;
                    for (int i = 0; i < totalNum; i++)
                    {

                        string placeName1 = snapshot.Child(i.ToString()).Child("placeName").Value.ToString();
                        float x = float.Parse(snapshot.Child(i.ToString()).Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        float y = float.Parse(snapshot.Child(i.ToString()).Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        float z = float.Parse(snapshot.Child(i.ToString()).Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        Vector3 coordi1 = new Vector3(x, y, z);

                        Vector3 relatifPos = CommonVariables.ConvertGPS2ARCoordinate(CommonVariables.location, coordi1);
                        GameObject place = Instantiate(prefab, relatifPos, transform.rotation);
                        FirebaseScript.placesExist.Add(place);
                  
                        place.name = i.ToString();
                        place.tag = "place";
                        
                        GPSdata GPSdataScript = place.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
                        GPSdataScript.place_name = placeName1;
                        GPSdataScript.longitude = coordi1.x;
                        GPSdataScript.latitude = coordi1.z;
                        GPSdataScript.height = coordi1.y;
                        GPSdataScript.GPSInfo = true;
                        

                    }
                    
                }
            }

            );
        }
    }



}
