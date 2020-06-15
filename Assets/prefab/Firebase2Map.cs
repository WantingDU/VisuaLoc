using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using System.Globalization;
using Mapbox.Utils;
using UnityEngine.UI;
using Mapbox.Examples;
using UnityEngine.SceneManagement;


public class Firebase2Map : MonoBehaviour
{
    public GameObject prefab;
    public GameObject PrivatePrefab;
    GameObject ARMapPoint;
    GameObject PrivateARMapPoint;
    public static float SearchRange;
    public static DatabaseReference placesRef;
    public static DatabaseReference MapPointsRef;
    public  List<GameObject> placesExist = new List<GameObject>();
    public  Dictionary<GameObject,MyPoint> myPointExist = new Dictionary<GameObject,MyPoint>();
    public List<GameObject> MapExist = new List<GameObject>();
    public Dictionary<GameObject, MyMapPoint> myMapExist = new Dictionary<GameObject, MyMapPoint>();
    AbstractMap map;


    private void OnDestroy()
    {
        MapPointsRef.ChildAdded -= HandleChildAdded2;
        MapPointsRef.ChildChanged -= HandleChildChanged2;
        MapPointsRef.ChildRemoved -= HandleChildRemoved2;
    }
    public void ChangeSearchRange()
    {
        SearchRange = FindObjectOfType<Slider>().value;
        print("changed slider value=" + Firebase2Map.SearchRange);
        SceneManager.LoadSceneAsync("MapView");
    }

    public void Start()
    {

        FindObjectOfType<Slider>().value = SearchRange;
        SearchRange = FindObjectOfType<Slider>().value;
        //prefab = Resources.Load<GameObject>("Sphere");
        //PrivatePrefab = Resources.Load<GameObject>("SphereRed");
        StaticObject.myARmapID = null;
        StaticObject.myARmapName = null;
        ARMapPoint = Resources.Load<GameObject>("ARMapPoint");
        PrivateARMapPoint= Resources.Load<GameObject>("PrivateARMapPoint");
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://shareardubidu.firebaseio.com/");
        map = LocationProviderFactory.Instance.mapManager;
        placesRef = FirebaseDatabase.DefaultInstance.GetReference("places");
        MapPointsRef = FirebaseDatabase.DefaultInstance.GetReference("ARMap");
        print("My UserId" + Auth.UserSelfId+"Search range="+SearchRange);
        InstantiateFromDB();
        placesRef.OrderByChild("coordinate/z").StartAt(StaticThings.location.latitude - SearchRange).EndAt(StaticThings.location.latitude + SearchRange).ChildAdded += HandleChildAdded;
        //placesRef.ChildAdded += HandleChildAdded;
        placesRef.OrderByChild("coordinate/z").StartAt(StaticThings.location.latitude - SearchRange).EndAt(StaticThings.location.latitude + SearchRange).ChildChanged += HandleChildChanged;
        placesRef.ChildRemoved += HandleChildRemoved;
        MapPointsRef.OrderByChild("coordinate/x").StartAt(StaticThings.location.latitude - SearchRange).EndAt(StaticThings.location.latitude + SearchRange).ChildAdded += HandleChildAdded2;
        MapPointsRef.OrderByChild("coordinate/x").StartAt(StaticThings.location.latitude - SearchRange).EndAt(StaticThings.location.latitude + SearchRange).ChildChanged += HandleChildChanged2;
        MapPointsRef.ChildRemoved += HandleChildRemoved2;
    }


    // Update is called once per frame

    public void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("post"))
                {
                    hit.transform.gameObject.GetComponent<PointController>().Hit();
                }
                else if (hit.transform.gameObject.CompareTag("ARMapPoint"))
                {
                    //string mapGuid=myMapExist[hit.transform.gameObject].guid;
                    string mapID = myMapExist[hit.transform.root.gameObject].guid;
                    StaticObject.myARmapID = mapID;
                    StaticObject.myARmapName = myMapExist[hit.transform.root.gameObject].placeName;
                    StaticCoroutine.DoCoroutine(EventScript.Go2ARTask());
                    //SceneManager.LoadSceneAsync("ARView");

                }
            }

            else
            {
                Destroy(GameObject.FindWithTag("little_panel"));
            }
        }

            foreach (GameObject point in placesExist)
            {

                var location = myPointExist[point].coordinate;
                point.transform.localPosition = map.GeoToWorldPosition(location, true);
                //point.transform.localScale = new Vector3(SpawnOnMap._spawnScale, SpawnOnMap._spawnScale, SpawnOnMap. _spawnScale);

            }
        
        
        foreach (GameObject MapPoint in MapExist)
        {

            var location = myMapExist[MapPoint].coordinate;
            MapPoint.transform.localPosition = map.GeoToWorldPosition(location, true);
            //point.transform.localScale = new Vector3(SpawnOnMap._spawnScale, SpawnOnMap._spawnScale, SpawnOnMap. _spawnScale);

        }



    }

    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        bool XisInRange = (inRange(x, StaticThings.location.longitude - SearchRange, StaticThings.location.longitude + SearchRange));
        string IsPublic= args.Snapshot.Child("IsPublic").Value.ToString();
        string UserID= args.Snapshot.Child("UserID").Value.ToString();
        if (XisInRange&& (IsPublic.Equals("True")||UserID.Equals(Auth.UserSelfId)))
        {
            string guid = args.Snapshot.Child("guid").Value.ToString();
            if (GameObject.Find(guid) == null) 
            {
                string placeName = args.Snapshot.Child("title").Value.ToString();
                float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                Vector2d position_point = new Vector2d(z, x);
                MyPoint my_point = new MyPoint(guid, position_point);
                GameObject point;
                if (!UserID.Equals(Auth.UserSelfId))
                {
                    point = Instantiate(prefab, map.GeoToWorldPosition(position_point), transform.rotation);
                }
                else
                {
                    point = Instantiate(PrivatePrefab, map.GeoToWorldPosition(position_point), transform.rotation);
                }
                point.name = guid;
                point.tag = "post";
                placesExist.Add(point);
                myPointExist.Add(point, my_point);

                Debug.Log("SearchRange:" + SearchRange + " Added placeName: " + placeName);
            }
            
            
        }
            

    }


    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        placesRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {

                if (args.DatabaseError != null)
                {
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }
                
                float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                bool XisInRange = (inRange(x, StaticThings.location.longitude - SearchRange, StaticThings.location.longitude + SearchRange));
                if (XisInRange)
                {
                    GameObject changedObject = GameObject.Find(args.Snapshot.Child("guid").Value.ToString());
                    MyPoint changedPoint = myPointExist[changedObject];
                    string guid = args.Snapshot.Child("guid").Value.ToString();
                    string placeName = args.Snapshot.Child("title").Value.ToString();
                    float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    Vector2d position_point = new Vector2d(z, x);
                    changedObject.transform.position = map.GeoToWorldPosition(position_point);
                    changedObject.name = guid;
                    changedPoint.coordinate = position_point;
                    changedPoint.placeName = guid;
                    print(changedObject.name.ToString() + "changed!");
                }

            }
        },
         System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
        );
    }
    

    public void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        print("destroy a removed child");
        if (GameObject.Find(args.Snapshot.Child("guid").Value.ToString()) != null)
        {
            GameObject changedObject = GameObject.Find(args.Snapshot.Child("guid").Value.ToString());
            print("args previous child name:" + args.PreviousChildName);
            Destroy(changedObject);
            myPointExist.Remove(key: changedObject);
            placesExist.Remove(changedObject);
        } 
    }


    void HandleChildAdded2(object sender, ChildChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        MapPointsRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                bool YisInRange = (inRange(y, StaticThings.location.longitude - SearchRange, StaticThings.location.longitude + SearchRange));
                string IsPublic = args.Snapshot.Child("IsPublic").Value.ToString();
                bool isPublicBool = IsPublic.Equals("True");
                string UserID = args.Snapshot.Child("UserID").Value.ToString();
                if (YisInRange && (isPublicBool || UserID.Equals(Auth.UserSelfId)))
                {
                    string guid = args.Snapshot.Child("guid").Value.ToString();
                    if (GameObject.Find(guid) == null)
                    {
                        string placeName = args.Snapshot.Child("placeName").Value.ToString();
                        float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

                        Vector2d position_point = new Vector2d(x, y);
                        MyMapPoint my_point = new MyMapPoint(guid, placeName, position_point, Auth.UserSelfId, isPublicBool);

                        GameObject point;
                        if (!UserID.Equals(Auth.UserSelfId))
                        {
                            point = Instantiate(ARMapPoint, map.GeoToWorldPosition(position_point), transform.rotation);
                        }
                        else
                        {
                            point = Instantiate(PrivateARMapPoint, map.GeoToWorldPosition(position_point), transform.rotation);
                        }
                        point.GetComponentInChildren<Text>().text = placeName;
                        StartCoroutine(firestore.LoadScreenshot(guid + "/ARMapScreenshot.jpg", point));
                        point.name = guid;
                        point.tag = "ARMapPoint";
                        MapExist.Add(point);
                        myMapExist.Add(point, my_point);
                    }
                    
                }

            }
        },
     System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
    );




    }

    void HandleChildChanged2(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        MapPointsRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {

                if (args.DatabaseError != null)
                {
                    Debug.LogError(args.DatabaseError.Message);
                    return;
                }

                float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                bool YisInRange = (inRange(y, StaticThings.location.longitude - SearchRange, StaticThings.location.longitude + SearchRange));

                if (YisInRange)
                {
                    GameObject changedObject = GameObject.Find(args.Snapshot.Key);
                    MyMapPoint changedPoint = myMapExist[changedObject];
                    string guid = args.Snapshot.Child("guid").Value.ToString();
                    string placeName = args.Snapshot.Child("placeName").Value.ToString();
                    float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

                    Vector2d position_point = new Vector2d(x, y);
                    changedObject.transform.position = map.GeoToWorldPosition(position_point);
                    changedObject.name = guid;
                    changedPoint.coordinate = position_point;
                    changedPoint.placeName = placeName;
                    changedPoint.guid = guid;
                    changedObject.GetComponentInChildren<Text>().text = placeName;
                    print(changedObject.name.ToString() + "changed!");
                }
            }
        },
         System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
        );
    }


    public void HandleChildRemoved2(object sender, ChildChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        print("destroy a removed child");
        if (GameObject.Find(args.Snapshot.Child("placeName").Value.ToString()) != null)
        {
            GameObject changedObject = GameObject.Find(args.Snapshot.Child("placeName").Value.ToString());
            print("args previous child name:" + args.PreviousChildName);

            Destroy(changedObject);
            myMapExist.Remove(key: changedObject);
            MapExist.Remove(changedObject);
        }
        
    }

    bool inRange(float x, float min, float max) => ((x - max) * (x - min) <= 0);

    public void InstantiateFromDB()
    {
            placesRef.OrderByChild("coordinate/z").StartAt(StaticThings.location.latitude - SearchRange).EndAt(StaticThings.location.latitude + SearchRange).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError(message: "task.IsFaulted");
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot panel in snapshot.Children)
                    {
                        //double check for altitude
                        float x = float.Parse(panel.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        bool XisInRange = (inRange(x, StaticThings.location.longitude - SearchRange, StaticThings.location.longitude + SearchRange));
                        string IsPublic = panel.Child("IsPublic").Value.ToString();
                        string UserID = panel.Child("UserID").Value.ToString();
                        if (XisInRange && (IsPublic.Equals("True") || UserID.Equals(Auth.UserSelfId)))
                        {
                            string guid = panel.Child("guid").Value.ToString();
                            string placeName1 = panel.Child("title").Value.ToString();
                            float z = float.Parse(panel.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                           
                            Vector2d position_point = new Vector2d(z, x);
                            MyPoint my_point = new MyPoint(guid, position_point);
                            if (GameObject.Find(guid) == null)
                            {
                                GameObject point;
                                if (!UserID.Equals(Auth.UserSelfId))
                                {
                                    point = Instantiate(prefab, map.GeoToWorldPosition(position_point), transform.rotation);
                                }
                                else
                                {
                                    point = Instantiate(PrivatePrefab, map.GeoToWorldPosition(position_point), transform.rotation);
                                }                            
                                point.tag = "post";
                                point.name = guid;
                                placesExist.Add(point);
                                myPointExist.Add(point, my_point);
                            }
                        }
                    }

                }
            },
             System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
            );

            MapPointsRef.OrderByChild("coordinate/x").StartAt(StaticThings.location.latitude - SearchRange).EndAt(StaticThings.location.latitude + SearchRange).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    return;
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach(DataSnapshot panel in snapshot.Children)
                    {
                        float y = float.Parse(snapshot.Child(panel.Key).Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        bool YisInRange = (inRange(y, StaticThings.location.longitude - SearchRange, StaticThings.location.longitude + SearchRange));
                        string IsPublic = panel.Child("IsPublic").Value.ToString();
                        bool isPublicBool = IsPublic.Equals("True");
                        string UserID = panel.Child("UserID").Value.ToString();
                        if (YisInRange &&( isPublicBool || UserID.Equals(Auth.UserSelfId)))
                        {
                            string guid = panel.Child("guid").Value.ToString();
                            string placeName1 = snapshot.Child(panel.Key).Child("placeName").Value.ToString();
                            float x = float.Parse(snapshot.Child(panel.Key).Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                            Vector2d position_point = new Vector2d(x, y);
                            MyMapPoint my_point = new MyMapPoint(guid, placeName1, position_point, Auth.UserSelfId, isPublicBool);
                            GameObject point;
                            if (!UserID.Equals(Auth.UserSelfId))
                            {
                                point = Instantiate(ARMapPoint, map.GeoToWorldPosition(position_point), transform.rotation);
                            }
                            else
                            {
                                point = Instantiate(PrivateARMapPoint, map.GeoToWorldPosition(position_point), transform.rotation);
                            }

                            point.GetComponentInChildren<Text>().text = placeName1;
                            point.tag = "ARMapPoint";
                            point.name = guid;
                            StartCoroutine(firestore.LoadScreenshot(guid + "/ARMapScreenshot.jpg", point));
                            MapExist.Add(point);
                            myMapExist.Add(point, my_point);
                        }
                    }

                }
            },
             System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
            );
    }
    public void OnSearchMap()
    {
        Text output= GameObject.Find("SearchResult").GetComponent<Text>(); 
        string MapID = GameObject.Find("SearchMap").GetComponent<InputField>().text;
        Debug.Log("found mapid text" + MapID);
        Firebase2Map.MapPointsRef.Child(MapID).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                output.text = "ID is invalid, please check again";
                return;
            }
            else if (task.IsCompleted)
            {
                
                DataSnapshot snapshot = task.Result;
                if (snapshot.ChildrenCount==0) {
                    output.text = "ID is invalid, please check again";
                    return;
                }
                output.text = "ARMap found, now loading...";
                float y = float.Parse(snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                string IsPublic = snapshot.Child("IsPublic").Value.ToString();
                string UserID = snapshot.Child("UserID").Value.ToString();
                string guid = snapshot.Child("guid").Value.ToString();
                string placeName1 = snapshot.Child("placeName").Value.ToString();
                float x = float.Parse(snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

                Vector2d position_point = new Vector2d(x, y);
                MyMapPoint my_point = new MyMapPoint(guid, placeName1, position_point, Auth.UserSelfId, IsPublic.Equals("True"));
                GameObject point = Instantiate(PrivateARMapPoint, map.GeoToWorldPosition(position_point), transform.rotation);

                point.GetComponentInChildren<Text>().text = placeName1;


                point.tag = "ARMapPoint";
                point.name = guid;
                StartCoroutine(firestore.LoadScreenshot(guid + "/ARMapScreenshot.jpg", point));

                MapExist.Add(point);


                myMapExist.Add(point, my_point);


            }
        },
             System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
            );
    }

}
