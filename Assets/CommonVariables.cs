using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Mapbox.Utils;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;

public class CommonVariables : MonoBehaviour
{
    public static LocationInfo location;
    public static DatabaseReference reference;
    const double lat2km = 111319.491;
    void Awake()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://shareardubidu.firebaseio.com/");
        if (Auth.UserSelfId == null) Auth.UserSelfId = "parDefaut";
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        //if(SceneManager.GetActiveScene().name=="LB-Test")
        
    }
    private void Start()
    {
        Input.location.Start();

        InvokeRepeating("UpdateGPS2", 0f, 1f);

    }
    public static void UpdateGPS2()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                location = Input.location.lastData;
            }
        }
    }// Update is called once per frame

public static void writeNewPlace2(string guID, string title, string contents, Vector3 coordi,string UserID,string IsPublic)
    {
        
        Place2 new_place = new Place2(guID, title,contents, coordi,UserID,IsPublic);
        string json = JsonUtility.ToJson(new_place);
        CommonVariables.reference.Child("places").Child(guID).SetRawJsonValueAsync(json);
        
    }
    public static void writeNewARMap(string guid, string title, Vector2d coordi,bool IsPublic,string UserID,string lastDate)
    {
        MyMapPoint new_ARMapPoint = new MyMapPoint(guid, title, coordi, UserID, IsPublic,lastDate);
        string json = JsonUtility.ToJson(new_ARMapPoint);
        CommonVariables.reference.Child("ARMap").Child(guid).SetRawJsonValueAsync(json);

    }

    public static Vector3 ConvertARCoordinate2GPS(Vector3 ARcoor)
    {
        Vector3 v;
        v = new Vector3(0f, 0f, 0f);
        while (true)
        {
            try
            {
                
                double dy = (ARcoor.y + location.altitude);
                double dz = -ARcoor.z / lat2km + location.latitude;
                double dx =-ARcoor.x / lat2km + location.longitude;
                v = new Vector3((float)dx, (float)dy, (float)dz);
                break; // Exit the loop. Could return from the method, depending
                       // on what it does...
            }
            catch
            {
                Debug.Log("error while Convert to GPS!");
            }
        }
        return v;
    }
    public static Vector3 ConvertGPS2ARCoordinate(LocationInfo location, Vector3 prefabLoc)
    {
        double dy = (prefabLoc.y - location.altitude);
        double dz = -(prefabLoc.z - location.latitude) * lat2km;
        double dx = -(prefabLoc.x - location.longitude) * lat2km;
        return new Vector3((float)dx, (float)dy, (float)dz);
    }
    public static string GetTimestamp(DateTime value)
    {
        return value.ToString("MM-dd HH:mm:ss");
    }
    public static string GetTimestampPrecise(DateTime value)
    {
        return value.ToString("MM/dd/HH/mm/ss/fff");// '/'causes children in database
    }
}


