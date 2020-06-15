using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;


/*
        This Script is for generating randomly gameObejects in Database of firebase.

 */

public class generatePlaces : MonoBehaviour
{

    ILocationProvider _locationProvider;
    ILocationProvider LocationProvider
    
    {
        get
        {
            if (_locationProvider == null)
            {
                _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            }

            return _locationProvider;
        }
    }

    //public static DatabaseReference reference;
    LocationInfo location;
    //public static int counter;
    // Start is called before the first frame update
    void Start()
    {
        /* Input.location.Start();
         FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://shareardubidu.firebaseio.com/");

         reference = FirebaseDatabase.DefaultInstance.RootReference;
         reference.Child("places").GetValueAsync().ContinueWith(task =>
         {
             if (task.IsFaulted)
             {
                 return;
             }
             else if (task.IsCompleted)
             {
                 DataSnapshot snapshot = task.Result;
                 counter = (int)snapshot.ChildrenCount;
             }
         });
         */
        InvokeRepeating("randomInstanciate", 0.2f, 4f);
    }

    /*
    public void randomInstanciate()
    {

            float x = (float)LocationProvider.CurrentLocation.LatitudeLongitude.y + Random.Range(-0.0003f, 0.0003f);
            float z = (float)LocationProvider.CurrentLocation.LatitudeLongitude.x + Random.Range(-0.0003f, 0.0003f);
            float y = (float)Input.location.lastData.altitude+Random.Range(-5f,5f);
            Vector3 randomPos = new Vector3(x, y, z);

            CommonVariables.writeNewPlace(CommonVariables.counter.ToString(), randomPos);
            CommonVariables.counter++;
            

    }*/
}
