using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine.SceneManagement;


/*
        This Script is for retrieving data from DB and instantiating the appropriate Gameobjects .

 */


public class Place2
{
    public string guid;
    public string title;
    public string contents;
    public Vector3 coordinate;
    public string UserID;
    public string IsPublic;

    public Place2()
    {
    }

    public Place2(string guid, string title, string contents, Vector3 coordinate,string UserID,string IsPublic)
    {
        this.guid = guid;
        this.title = title;
        this.contents = contents;
        this.coordinate = coordinate;
        this.UserID = UserID;
        this.IsPublic = IsPublic;
    }
}



public class Database2AR : MonoBehaviour
{
    
    GameObject prefab;
    public static DatabaseReference placesRef;
    public static DatabaseReference ARMapRef;
    public static bool stop = false;
    public static List<GameObject> PanelExist ;

    public static bool ready = false;

    bool inRange(float x, float min, float max) => ((x - max) * (x - min) <= 0);
    private void OnDestroy()
    {
        PanelExist = null;
        placesRef.ChildAdded -= HandleChildAdded;
        placesRef.ChildChanged -= HandleChildChanged;
        placesRef.ChildRemoved -= HandleChildRemoved;

    }
    public void Awake()
    {
        ready = true;
        placesRef = FirebaseDatabase.DefaultInstance.GetReference("places");
        ARMapRef = FirebaseDatabase.DefaultInstance.GetReference("ARMap");
    }
    public void Start()
    {
        StaticObject.tempSearchRange = Firebase2Map.SearchRange;
        FindObjectOfType<Slider>().value = Firebase2Map.SearchRange;
        print("changed slider value="+Firebase2Map.SearchRange);
        placesRef.OrderByChild("coordinate/z").StartAt(CommonVariables.location.latitude - Firebase2Map.SearchRange).EndAt(CommonVariables.location.latitude + Firebase2Map.SearchRange).ChildAdded += HandleChildAdded;
        placesRef.OrderByChild("coordinate/z").StartAt(CommonVariables.location.latitude - Firebase2Map.SearchRange).EndAt(CommonVariables.location.latitude + Firebase2Map.SearchRange).ChildChanged += HandleChildChanged;
        placesRef.ChildRemoved += HandleChildRemoved;
        prefab = Resources.Load<GameObject>("PanelWithGPS");
        Invoke("InstantiateFromDB", 0f);
        PanelExist = new List<GameObject>();


    }

    public void UpdatePosition()
    {
        Firebase2Map.SearchRange= FindObjectOfType<Slider>().value;
        float EPSILON = 0.001f;
        if (System.Math.Abs(Firebase2Map.SearchRange - StaticObject.tempSearchRange) > EPSILON)
        {
            print("SearchRange Changed!");
            foreach (GameObject p in PanelExist)
            {
                Destroy(p);
            }
            PanelExist.Clear();
            SceneManager.LoadSceneAsync("ARView"); 
        }
        else
        {
            foreach (GameObject p in PanelExist)
            {

                GPSdata2 GPSdataScript = p.GetComponentInChildren<GPSdata2>();

                Vector3 positionOfP = new Vector3((float)GPSdataScript.longitude, (float)GPSdataScript.height, (float)GPSdataScript.latitude);
                GPSdataScript.UpdateGPS();
                p.transform.position = Camera.main.transform.position + CommonVariables.ConvertGPS2ARCoordinate(CommonVariables.location, positionOfP);
                p.transform.LookAt(Camera.main.transform);
                p.transform.Rotate(new Vector3(0, 180, 0));

            }
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
        bool XisInRange = inRange(x, CommonVariables.location.longitude - Firebase2Map.SearchRange, CommonVariables.location.longitude + Firebase2Map.SearchRange);

        if (XisInRange)
        {
                    string guid = args.Snapshot.Key;
                    if (!GameObject.Find(guid))
                    {
                        GameObject place = Instantiate(prefab);
                        StartCoroutine(firestore.LoadImage(guid + ".jpg", place));
                        string title = args.Snapshot.Child("title").Value.ToString();
                        string contents = args.Snapshot.Child("contents").Value.ToString();
                        //Debug.Log(title + "added!");
                        place.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = title;
                        place.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = contents;
                       
                        float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        Vector3 coordi1 = new Vector3(x, y, z);
                        Vector3 relatifPos = CommonVariables.ConvertGPS2ARCoordinate(CommonVariables.location, coordi1);
                        place.transform.position = relatifPos;
                        place.transform.rotation = transform.rotation;
                        Database2AR.PanelExist.Add(place);
                        place.name = guid;
                        place.tag = "panel";
                        GPSdata2 GPSdataScript = place.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata2>();
                        GPSdataScript.place_name = title;
                        GPSdataScript.longitude = x;
                        GPSdataScript.latitude = z;
                        GPSdataScript.height = y;
                        GPSdataScript.GPSInfo = true;
                    }

         }
             

        //}

    }
    
    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        bool XisInRange = inRange(x, CommonVariables.location.longitude - Firebase2Map.SearchRange, CommonVariables.location.longitude + Firebase2Map.SearchRange);

        if (XisInRange)
        {
            GameObject changedObject = GameObject.Find(args.Snapshot.Key);
            string title = args.Snapshot.Child("title").Value.ToString();

            float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
            float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
            //Debug.Log(title + "changed!");
            GPSdata2 GPSdataScript = changedObject.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata2>();
            GPSdataScript.place_name = title;
            GPSdataScript.longitude = x;
            GPSdataScript.latitude = z;
            GPSdataScript.height = y;
            GPSdataScript.GPSInfo = true;
        }
    }


    public void InstantiateFromDB()
    {
        int i = 0;
        //if (!stop)
        //{
            placesRef.OrderByChild("coordinate/z").StartAt(CommonVariables.location.latitude - Firebase2Map.SearchRange).EndAt(CommonVariables.location.latitude + Firebase2Map.SearchRange).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("task.IsFaulted");
                    Debug.Log(task.Exception);
                    return;
                }
                else if (task.IsCompleted)
                {
                    print("task.IsCompleted in instantiateDB");
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot panel in snapshot.Children)
                    {
                        float x = float.Parse(snapshot.Child(panel.Key).Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                        bool XisInRange = inRange(x, CommonVariables.location.longitude - Firebase2Map.SearchRange, CommonVariables.location.longitude + Firebase2Map.SearchRange);

                        if (XisInRange)
                        {
                            string guid = snapshot.Child(panel.Key).Child("guid").Value.ToString();
                            GameObject place = Instantiate(prefab);
                            StartCoroutine(firestore.LoadImage(guid + ".jpg", place));
                            string title = snapshot.Child(panel.Key).Child("title").Value.ToString();
                            string contents = snapshot.Child(panel.Key).Child("contents").Value.ToString();
                            
                            float y = float.Parse(snapshot.Child(panel.Key).Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                            float z = float.Parse(snapshot.Child(panel.Key).Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

                            Vector3 coordi1 = new Vector3(x, y, z);
                            Vector3 relatifPos = CommonVariables.ConvertGPS2ARCoordinate(CommonVariables.location, coordi1);
                            place.transform.position = relatifPos;
                            place.transform.rotation = transform.rotation;
                            place.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = title;
                            place.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = contents;

                            Database2AR.PanelExist.Add(place);

                            place.name = guid;
                            place.tag = "panel";

                            GPSdata2 GPSdataScript = place.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata2>();
                            GPSdataScript.place_name = title;
                            GPSdataScript.longitude = coordi1.x;
                            GPSdataScript.latitude = coordi1.z;
                            GPSdataScript.height = coordi1.y;
                            GPSdataScript.GPSInfo = true;

                            //print("just added:" + PanelExist[i].name);
                            i++;
                        }
                    }

                }
            },
            System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
            );
        //}
        Debug.Log("total number:"+Database2AR.PanelExist.Count);

    }
    public static IEnumerator onRemoveMap()
    {
        DatabaseReference mPostReference1 = CommonVariables.reference.Child("ARMap")
                       .Child(StaticObject.myARmapID);
        mPostReference1.RemoveValueAsync();
        foreach (KeyValuePair<List<float>, List<string>> Go in StaticObject.addedGO)
        {
            
            DatabaseReference mPostReference = CommonVariables.reference.Child("places")
                                    .Child(Go.Value[3]);
            mPostReference.RemoveValueAsync();
            
        }
        yield return new WaitForEndOfFrame();
    }
    public void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        if (GameObject.Find(args.Snapshot.Child("guid").Value.ToString()) != null)
        {
            GameObject changedObject = GameObject.Find(args.Snapshot.Child("guid").Value.ToString());
            print("args previous child name:" + args.PreviousChildName);

            Destroy(changedObject);
            //myPointExist.Remove(key:changedObject);
            PanelExist.Remove(changedObject);
        }
        
    }

    }
