using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using UnityEngine.UI;
using Firebase.Unity.Editor;
using Mapbox.Utils;
using Firebase;
using Firebase.Database;
using System;

public class StaticThings : MonoBehaviour
{
    const double lat2km = 111319.491;
    public static LocationInfo location;
    public static DatabaseReference reference;

    void Awake()
    {
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://shareardubidu.firebaseio.com/");
        if (Auth.UserSelfId == null) Auth.UserSelfId = "parDefaut";
        reference = FirebaseDatabase.DefaultInstance.RootReference;

         StaticObject.storage = FirebaseStorage.DefaultInstance;
         StaticObject.storage_ref = StaticObject.storage.GetReferenceFromUrl("gs://shareardubidu.appspot.com");
         StaticObject.ARWorldMap_ref = StaticObject.storage_ref.Child("ARWorldMap");
         StaticObject.Bunkyou_ref = StaticObject.storage_ref.Child("Bunkyou");
        

    }
    private void Start()
    {
        Input.location.Start();
        InvokeRepeating("UpdateGPS2", 0f, 1f);
    }

    public void UpdateGPS2()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                location = Input.location.lastData;
            }
        }
        else
        {
            print("Input.location.isNotEnabledByUser");
        }
    }
}

    //this method is for forcing a Texture2D to be readable
    /*
    public static Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
    public static string getGUID()
    {
        Guid guid = new Guid();
        string str = guid.ToString();
        return str;
    }
        /*
    public static void writeNewPlace2(string guID, string title, string contents, Vector3 coordi, string UserID, string IsPublic)
    {

        Place2 new_place = new Place2(guID, title, contents, coordi, UserID, IsPublic);
        string json = JsonUtility.ToJson(new_place);
        CommonVariables.reference.Child("places").Child(guID).SetRawJsonValueAsync(json);

    }
    public static void writeNewARMap(string guid, string title, Vector2d coordi, bool IsPublic, string UserID)
    {
        MyMapPoint new_ARMapPoint = new MyMapPoint(guid, title, coordi, UserID, IsPublic);
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
                double dx = -ARcoor.x / lat2km + location.longitude;
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
    }*/


