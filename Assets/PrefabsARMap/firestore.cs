using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Firebase.Storage;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using System.Diagnostics;

public class firestore : MonoBehaviour {


    [SerializeField]
    UnityARCameraManager m_ARCameraManager;

    GameObject Prefab;
    GameObject PrefabNoImage;
    public static bool ScenePublic;
    //private UnityARAnchorManager unityARAnchorManager;
    public static List<GameObject> PrecisARList;
    public static int checkFile;
    public static bool PrecisARListValid;
    public static ARWorldMap loadedMap;
    public ParticleSystem pointCloudParticlePrefab;
    private ParticleSystem totalPS;
    public static int FilesLoaded= 0;
    public static Stopwatch sw=new Stopwatch();
    //GameObject Timer;
    bool view = true;
    private void OnDestroy()
    {
        PrecisARList = null;
        checkFile = 0;
        PrecisARListValid = false;
        FilesLoaded = 0;
        print("ondestroy");
        loadedMap = null;
        sw.Reset();
        
        UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = false;
        
    }

/*Commented for user version*/
/*
    private void Awake()
    {
        Timer = GameObject.Find("Timer");
    }
    private void Update()
    {

        //GameObject.Find("CPTotal").GetComponent<Text>().text = StaticObject.ARWorldMapTracked().ToString();
        if (FilesLoaded >= 3)
        {
            Timer.GetComponent<Text>().color = Color.blue;
        }
        else
        {
            Timer.GetComponent<Text>().color = Color.white;
        }
        if (sw.ElapsedMilliseconds > 0)
        {
            Timer.GetComponent<Text>().text = sw.ElapsedMilliseconds.ToString();
        }
        
       

    }
    */
    void Start() {
        
        checkFile = 0;
        FilesLoaded = 0;
        Prefab = Resources.Load<GameObject>("InfoPanelOut");
        PrefabNoImage = Resources.Load<GameObject>("InfoPanelNoImage");
        if (StaticObject.myARmapName ==null) StaticObject.myARmapName ="Default";
        if (StaticObject.myARmapID == null) StaticObject.myARmapID = StaticObject.getGUID();
        string mapinfo = "Map Name: " + StaticObject.myARmapName + "$Map ID: " + StaticObject.myARmapID;
        mapinfo = mapinfo.Replace('$', '\n');
        GameObject.Find("MapInfo").GetComponent<Text>().text = mapinfo;
        StaticObject.addedGO = new Dictionary<List<float>,List<string>>();
        StaticObject.listOfFiles = new List<string>();
        PrecisARList = new List<GameObject>();
        //unityARAnchorManager = new UnityARAnchorManager();
        print("start in firestore is finished");
        sw.Reset();
        onLoad();

    }
    public byte[] list2byte(List<string> myObjToSerialize)
    {
        var binFormatter = new BinaryFormatter();
        var mStream = new MemoryStream();
        binFormatter.Serialize(mStream, myObjToSerialize);
        return (mStream.ToArray());
    }

    public byte[] dict2byte(Dictionary<List<float>, List<string>> myObjToSerialize)
    {
        var binFormatter = new BinaryFormatter();
        var mStream = new MemoryStream();
        binFormatter.Serialize(mStream, myObjToSerialize);
        return (mStream.ToArray());
    }

    public Dictionary<List<float>,List<string>> byte2Dict(byte[] objectBytes)
    {
        var mStream = new MemoryStream();
        var binFormatter = new BinaryFormatter();

        // Where 'objectBytes' is your byte array.
        mStream.Write(objectBytes, 0, objectBytes.Length);
        mStream.Position = 0;

        var myDict = binFormatter.Deserialize(mStream) as Dictionary<List<float>,List<string>>;
        return myDict;
    }
    public List<string> byte2List(byte[] objectBytes)
    {
        var mStream = new MemoryStream();
        var binFormatter = new BinaryFormatter();
        mStream.Write(objectBytes, 0, objectBytes.Length);
        mStream.Position = 0;
        var myList = binFormatter.Deserialize(mStream) as List<string>;
        return myList;
    }


    //evaluate by checking plane 
    public void onSavePlane()
    {
        if (StaticObject.myARmapName == "Default")
        {
            StaticObject.debugger.text = "Please create a new map with a different name than Default";
            return;
        }
        //get two viewport point on top & buttom of screen 
        var screenPosition_buttom = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width / 2.0f, Screen.height / 4.0f, 100.0f));
        var screenPosition_top = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width / 2.0f, 3*Screen.height / 4.0f, 100.0f));
        var screenPosition_center = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 100.0f));
        ARPoint point_buttom = new ARPoint
        {
            x = screenPosition_buttom.x,
            y = screenPosition_buttom.y
        };
        ARPoint point_top = new ARPoint
        {
            x = screenPosition_top.x,
            y = screenPosition_top.y
        };
        ARPoint point_center = new ARPoint
        {
            x = screenPosition_center.x,
            y = screenPosition_center.y
        };
        //get one point on the plane by raycasting from view point
        Vector3 p1=StaticObject.viewTo3D(point_buttom);
        Vector3 p2 = StaticObject.viewTo3D(point_top);
        Vector3 p3 = StaticObject.viewTo3D(point_center);
        Vector3 pos = Camera.main.transform.position;
        float viewPoint = 0;
        WorldMapManager.session.GetCurrentWorldMapAsync(worldMap =>
        {
            foreach (Vector3 v in worldMap.pointCloud.Points)
            {
                Vector2 viewPos = Camera.main.WorldToViewportPoint(v); //viewport pos of point
                Vector3 dir = (v - pos).normalized;
                float dot = Vector3.Dot(Camera.main.transform.forward, dir);     //判断物体是否在相机前面
                float dist = Vector3.Distance(pos, v);
                if (dist <= 1.8 && dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                {
                    viewPoint+=0.2f;
                    if (StaticObject.IsOnPlane(v,p1)||StaticObject.IsOnPlane(v,p2)|| StaticObject.IsOnPlane(v, p3))//check if point in view port is also on detected plane in view port
                    {
                        viewPoint+=1f;
                    }

                }
                viewPoint =(float) Math.Round(viewPoint, 0);
            }
            GameObject.Find("PointsNumber").GetComponent<Text>().text =viewPoint.ToString();
            float evalu = viewPoint/ StaticObject.Weighted_cri;
            ProgressBar.SetProgressValue(evalu);
            if (viewPoint <= StaticObject.Weighted_cri)
            {
                StaticObject.debugger.text = "Current scene is not able to be rebuilt, please retry";
                return;
            }

            loadedMap = worldMap;
            GameObject saveW = GameObject.Find("SaveWindow");
            saveW.GetComponent<CanvasGroup>().alpha = 1;
            saveW.GetComponent<CanvasGroup>().blocksRaycasts = true;
            
            //worldMap.Save(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID));
            
            print("begin serializing");
            var worldMapInBytes = worldMap.SerializeToByteArray();
            StartCoroutine(WriteFile(worldMapInBytes, StaticObject.myARmapID + "/WorldData"));
            GalleryController.onScreenshotMap();
            saveObject();
            //PersistenceTest.writeNewTest(StaticObject.myARmapID, StaticObject.myARmapName, Auth.UserSelfId, "Weighted", PointCloudParticleExampleVersionDu.CPNumber.ToString(), worldMap.pointCloud.Count.ToString(),viewPoint.ToString());
            WriteARPoint2DB();
            loadedMap = worldMap;

        });


    }
    void WriteARPoint2DB()
    {
        Vector2d m_coor=new Vector2d (CommonVariables.location.latitude, CommonVariables.location.longitude);
        CommonVariables.writeNewARMap(StaticObject.myARmapID, StaticObject.myARmapName,m_coor, ScenePublic,Auth.UserSelfId, CommonVariables.GetTimestamp(DateTime.Now));
        StaticObject.debugger.text = "Finished Upload ARMap in database";
    }
    
    public void saveObject()
    {
        StaticObject.listOfFiles.Add(StaticObject.myARmapID + "/" + "ObjectInfo");
        byte[] objectInfoByte = dict2byte(StaticObject.addedGO);
        StartCoroutine(WriteFile(objectInfoByte, StaticObject.myARmapID + "/ObjectInfo"));
        byte[] FileInfoByte = list2byte(StaticObject.listOfFiles);
        StartCoroutine(WriteFile(FileInfoByte, StaticObject.myARmapID + "/FileList"));
        

    }


    public static IEnumerator WriteFile(byte[] custom_bytes,string pathName)
    {
            print("writing " + pathName);
            //SaveFileLocal(pathName, custom_bytes);  //problem :not found directory
        StorageReference newMap = StaticObject.Bunkyou_ref.Child(pathName);
            newMap.PutBytesAsync(custom_bytes)
              .ContinueWith((Task<StorageMetadata> task) => {
                  if (task.IsFaulted || task.IsCanceled)
                  {
                      UnityEngine.Debug.Log(task.Exception.ToString());
                      StaticObject.debugger.text = "error while writing"+pathName+" in database";
                  
                   }
                  else
                  {
                  // Metadata contains file metadata such as size, content-type, and download URL.
                  Firebase.Storage.StorageMetadata metadata = task.Result;

                      //string download_url = StaticObject.Bunkyou_ref.GetDownloadUrlAsync().ToString();
                      if (!StaticObject.listOfFiles.Contains(pathName))
                          StaticObject.listOfFiles.Add(pathName);
                      //StaticObject.debugger.text = "Finished Upload" + pathName;
                      print("Finished uploading "+pathName);
                      firestore.checkFile+=1;
                      if (checkFile > 0)
                      {
                          StaticObject.debugger.text = "Saving...:" + checkFile.ToString() + "/ 4";
                      }
                      if (firestore.checkFile == 4)
                      {
                          StaticObject.debugger.text = "Save succesfully";
                          print("save succesfully");
                          firestore.checkFile=0;
                      }
                  }
                  
              },System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
              )
        ;
        yield return new WaitForEndOfFrame();
    }


    public void onLoad()
    {
        print("onLoad start");
        ReadFile("ObjectInfo");
        ReadFile("FileList");
        ReadFile("WorldData");
    }
    public static IEnumerator  LoadImage(string Mypath,GameObject myGO)
    {
        int width= myGO.transform.GetComponentInChildren<RawImage>().texture.width;
        int height = myGO.transform.GetComponentInChildren<RawImage>().texture.height;
        print("downloading "+Mypath);
        const long maxAllowedSize = 100 * 1024 * 1024;
        StorageReference reference2Read = StaticObject.Bunkyou_ref.Child("Images").Child(Mypath);
        reference2Read.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                UnityEngine.Debug.Log(task.Exception.ToString());
                print("trouve pas de "+Mypath);
                StaticObject.debugger.text = task.Exception.ToString() + "length: " + task.Result.Length;
            }
            else
            {
                byte[] fileContents = task.Result;
                Texture2D myTexture = new Texture2D(width, height);
                myTexture.LoadImage(fileContents);
                myGO.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = myTexture;
                print(Mypath + " ready");
            }
        });
        yield return new WaitForEndOfFrame();
    }
    public static IEnumerator LoadScreenshot(string Mypath, GameObject myGO)
    {
        int width = myGO.transform.GetComponentInChildren<RawImage>().texture.width;
        int height = myGO.transform.GetComponentInChildren<RawImage>().texture.height;
        const long maxAllowedSize = 100 * 1024 * 1024;
        StorageReference reference2Read = StaticObject.Bunkyou_ref.Child(Mypath);
        reference2Read.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                UnityEngine.Debug.Log("error while read screenshot"+task.Exception.ToString());
                print("trouve pas de " + Mypath);
            }
            else
            {

                byte[] fileContents = task.Result;
                Texture2D myTexture = new Texture2D(width, height);
                myTexture.LoadImage(fileContents);
                myGO.transform.GetComponentInChildren<RawImage>().texture = myTexture;
                if (!StaticObject.listOfFiles.Contains(Mypath))
                    StaticObject.listOfFiles.Add(Mypath);
            }
        },
        System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        yield return new WaitForEndOfFrame();
    }
    /*
    public static void SaveFileLocal(string filename,byte[] FileBytes)
    {

        print("save to " + filename );
        File.WriteAllBytes(filename, FileBytes);
        print("save "+ filename + " in local storage");
         
    }*/

    public void onLoadWorldMapLocally()
    {
        StartCoroutine(LoadWorldMapLocal(loadedMap));
        GameObject LoadWindow = GameObject.Find("LoadWindow");
        LoadWindow.GetComponent<CanvasGroup>().alpha = 0;
        LoadWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
        return;
    }
    public void onNotLoadLocally()
    {
        GameObject LoadWindow = GameObject.Find("LoadWindow");
        LoadWindow.GetComponent<CanvasGroup>().alpha = 0;
        LoadWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
        const long maxAllowedSize = 1000 * 1024 * 1024;
        if (StaticObject.Bunkyou_ref.Child(StaticObject.myARmapID).Child("WorldData") == null)
        {
            print("there is no such reference");
            return;
        }
        StorageReference reference2Read = StaticObject.Bunkyou_ref.Child(StaticObject.myARmapID).Child("WorldData");
        reference2Read.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                UnityEngine.Debug.Log(task.Exception.ToString());
                StaticObject.debugger.text = task.Exception.ToString() + "length: " + task.Result.Length;
            }
            else
            {
                byte[] fileContents = task.Result;
                print("WorldData is downloaded");
                StartCoroutine(LoadWorldMap(fileContents));
                return;
            }
        },
        TaskScheduler.FromCurrentSynchronizationContext()
        );
    }
    public void ReadFile(string Mypath)
    {

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://
        //var path = Path.Combine(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID), Mypath + ".txt");
        //if current map exists in local storage
        if (Mypath == "WorldData")
        {
            var worldMap = ARWorldMap.Load(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID));
            if (worldMap != null)
            {
                loadedMap = worldMap;
                GameObject LoadWindow = GameObject.Find("LoadWindow");
                EventScript2.getUpdateTime();
                LoadWindow.GetComponentInChildren<Text>().text = "Would you like load this scene from local storage?  (Last update: " + StaticObject.lastUpdateTime +" )";
                LoadWindow.GetComponent<CanvasGroup>().alpha = 1;
                LoadWindow.GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }
        }

        /*
        if (Mypath == "ObjectInfo")
        {
            //var path = Path.Combine(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID), Mypath + ".txt");
            print(path);

            if (Directory.Exists(path))
            {
                print("Directory.Exists"+path);
                var ObjectInfo = File.ReadAllBytes(path);
                if (ObjectInfo != null)
                {
                    StaticObject.addedGO = byte2Dict(ObjectInfo);
                    print("objectinfo found in local");
                    print("StaticObject.addedGO " + StaticObject.addedGO.Count);
                    FilesLoaded++;
                    StaticObject.debugger.text = "Loading..." + FilesLoaded.ToString() + "/ 3";
                    StartCoroutine(reInstantiateGo(StaticObject.addedGO));
                    print("Tried to reinstantiateGo!");
                    return;
                }
            }
        }
        if (Mypath == "FileList")
        {
            //var path = Path.Combine(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID), Mypath + ".txt");
            print(path);
            if (Directory.Exists(path))
            {

                var FileList = File.ReadAllBytes(path);
                if (FileList != null)
                {
                    StaticObject.listOfFiles = byte2List(FileList);
                    FilesLoaded++;
                    StaticObject.debugger.text = "Loading..." + FilesLoaded.ToString() + "/ 3";
                    print("FileList found in local");
                    return;
                }
            }
             
        }
        */

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::://

        const long maxAllowedSize = 1000 * 1024 * 1024;
        if (StaticObject.Bunkyou_ref.Child(StaticObject.myARmapID).Child(Mypath) == null)
        {
            print("there is no such reference");
            return;
        }


        StorageReference reference2Read = StaticObject.Bunkyou_ref.Child(StaticObject.myARmapID).Child(Mypath);
        reference2Read.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                UnityEngine.Debug.Log(task.Exception.ToString());
                StaticObject.debugger.text = task.Exception.ToString()+ "length: " + task.Result.Length;
            }
            else
            {
                byte[] fileContents = task.Result;
                //StaticObject.debugger.text = Mypath+"is ready";
                print(Mypath + " is downloaded");
                switch (Mypath)
                {
                    case "WorldData":
                        print("worldData found");
                        print("Trying to load downloded WorldMap!");
                        StartCoroutine(LoadWorldMap(fileContents));
                        print("Tried to load downloded WorldMap!");
                        break;
                    case "ObjectInfo":
                        
                        StaticObject.addedGO = byte2Dict(fileContents);
                        print("objectinfo found");
                        print("StaticObject.addedGO "+StaticObject.addedGO.Count);
                        FilesLoaded++;
                        StaticObject.debugger.text = "Loading..." + FilesLoaded.ToString() + "/ 3";
                        StartCoroutine(reInstantiateGo(StaticObject.addedGO));
                        //SaveFileLocal(path, fileContents);
                        print("Tried to reinstantiateGo!");
                        break;
                    case "FileList":
                        
                        StaticObject.listOfFiles = byte2List(fileContents);
                        FilesLoaded++;
                        StaticObject.debugger.text = "Loading..." + FilesLoaded.ToString() + "/ 3";
                        print("FileList found");
                        //SaveFileLocal(path, fileContents);
                        break;

                }
            }
        },
        System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
        );


    }
    //View could point in ARMAP
    /*
    public void ViewCloud()
    {
        if (view)
        {
            ARWorldMap worldMap = loadedMap;
            print("point cloud count: " + worldMap.pointCloud.Count);
            GameObject.Find("CPTotal").GetComponent<Text>().text = worldMap.pointCloud.Count.ToString();
            int numParticles = 10000;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
            totalPS = Instantiate(pointCloudParticlePrefab);
            int index = 0;
            foreach (Vector3 v in worldMap.pointCloud.Points)
            {

                particles[index].position = v;
                particles[index].startColor = new Color(1.0f, 1.0f, 1.0f);
                particles[index].startSize = 0.01f;
                index++;
                if (index >= numParticles) break;
                totalPS.SetParticles(particles, numParticles);

            }
            view = !view;
        }
        else
        {
            Destroy(totalPS);
            print("just destroy PS!");
            view = !view;
        }
   
    }*/

    public IEnumerator LoadWorldMap(byte[] worldMapInBytes)
    {
        
        ARWorldMap worldMap = ARWorldMap.SerializeFromByteArray(worldMapInBytes);
        UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;
        var config = m_ARCameraManager.sessionConfiguration;
        config.worldMap = worldMap;
        UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;
        print("Restarting session with worldMap");
        yield return new WaitUntil(TwoFilesAreReady);
        print("Finished Loading all files");
        StaticObject.debugger.text = "Finished Loading all files";
        //show start button for timer after files are ready
        GameObject.Find("StartTracking").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("StartTracking").GetComponent<CanvasGroup>().blocksRaycasts = true;
        //active RestartTracking button
        GameObject.Find("RestartTracking").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("RestartTracking").GetComponent<CanvasGroup>().blocksRaycasts = true;
        yield return new WaitUntil(StaticObject.ClickedStart);
        WorldMapManager.session.RunWithConfigAndOptions(config, runOption);
        FilesLoaded++;
        print("FilesLoaded++ " + FilesLoaded);
        print("finished serialize");
        loadedMap = worldMap;
        if (!File.Exists(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID)))
        {
            GameObject saveW = GameObject.Find("SaveWindow");
            saveW.GetComponent<CanvasGroup>().alpha = 1;
            saveW.GetComponent<CanvasGroup>().blocksRaycasts = true;
           
        }

        yield return null;
    }
    public void OnSaveLocally()
    {
        GameObject LoadWindow = GameObject.Find("SaveWindow");
        LoadWindow.GetComponent<CanvasGroup>().alpha = 0;
        LoadWindow.GetComponent<CanvasGroup>().blocksRaycasts = false;
        loadedMap.Save(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID));
        UnityEngine.Debug.LogFormat("Online ARWorldMap saved to {0}", Path.Combine(Application.persistentDataPath, StaticObject.myARmapID));
        return;
    }
    public void OnNotSaveLocally()
    {
        GameObject saveW = GameObject.Find("SaveWindow");
        saveW.GetComponent<CanvasGroup>().alpha = 0;
        saveW.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public IEnumerator LoadWorldMapLocal(ARWorldMap worldMap)
    {
        print("Loading WorldMap in local storage!");
        UnityEngine.Debug.LogFormat("Map loaded. Center: {0} Extent: {1}", worldMap.center, worldMap.extent);
        /*
        UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;
        var config = m_ARCameraManager.sessionConfiguration;
        config.worldMap = worldMap;
        UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;
        */
        //StaticObject.debugger.text = "Restarting session with worldMap";
        print("Restarting session with worldMap");
        yield return new WaitUntil(TwoFilesAreReady);
        print("Finished Loading all files");
        StaticObject.debugger.text = "Finished Loading all files";
        //show start button for timer after files are ready
        GameObject.Find("StartTracking").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("StartTracking").GetComponent<CanvasGroup>().blocksRaycasts = true;
        //active restart button
        GameObject.Find("RestartTracking").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("RestartTracking").GetComponent<CanvasGroup>().blocksRaycasts = true;
        //
        yield return new WaitUntil(StaticObject.ClickedStart);
        //WorldMapManager.session.RunWithConfigAndOptions(config, runOption);//!!!!!!!!!!!!!这一部分会引起crash
        loadedMap = worldMap;
        startTracking();
        FilesLoaded++;
        print("FilesLoaded++ " + FilesLoaded);
        print("finished serialize");
        //loadedMap = worldMap;
        yield return null;
    }
    public void startTracking()
    {

        UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;
        var config = m_ARCameraManager.sessionConfiguration;
        config.worldMap = loadedMap;
        UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;
        WorldMapManager.session.RunWithConfigAndOptions(config, runOption);//!!!!!!!!!!!!!这一部分会引起crash
        sw.Reset();
        FilesLoaded = 3;
        sw.Start();
    }
    public static bool TwoFilesAreReady()
    {
        return (FilesLoaded >= 2); //FileList+GameObjectInfo

    }
    //All ready after click timer starter button and the AR tracking session is running
    public static bool AllFilesReady()
    {
        return (FilesLoaded >= 3);
    }

    public IEnumerator reInstantiateGo(Dictionary<List<float>, List<string>> dict)
    {

        //start rebuild after arworldmap is tracked succesful
        yield return new WaitUntil(StaticObject.ARWorldMapTracked);
        GameObject.Find("PhotoView").GetComponent<CanvasGroup>().alpha = 0;
        print("reInstantiateGo start PrecisARList count="+dict.Count);
        PrecisARList = new List<GameObject>(dict.Count);
        foreach (KeyValuePair<List<float>, List<string>> Go in dict)
        {
            GameObject VirtualGO;
            switch (Go.Value[0])
            {
                case "InfoPanelOut":
                    VirtualGO = Instantiate(Prefab, new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]), new Quaternion(Go.Key[3], Go.Key[4], Go.Key[5], Go.Key[6]));
                    print("Start loading image of panel");
                    StartCoroutine(LoadImage(Go.Value[3] + ".jpg", VirtualGO));
                    VirtualGO.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = Go.Value[1];
                    print("Title="+Go.Value[1]);
                    VirtualGO.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = Go.Value[2];
                    print("Contents=" + Go.Value[2]);
                    break;

                case "InfoPanelNoImage":
                    VirtualGO = Instantiate(PrefabNoImage, new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]), new Quaternion(Go.Key[3], Go.Key[4], Go.Key[5], Go.Key[6]));
                    VirtualGO.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = Go.Value[1];
                    VirtualGO.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = Go.Value[2];
                    break;
                default:
                    GameObject SimplePrefab = Resources.Load<GameObject>(Go.Value[0]);
                    print("find"+ SimplePrefab.name);
                    VirtualGO = Instantiate(SimplePrefab, new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]), new Quaternion(Go.Key[3], Go.Key[4], Go.Key[5], Go.Key[6]));
                    VirtualGO.transform.localScale =StaticObject.StringToVector3(Go.Value[7]);
                    break;

            }
            print("Instantiated at position "+ new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]).ToString()+" with lindex: "+Go.Value[6]);
            StaticObject.SetLayerRecursively(VirtualGO,15);
            VirtualGO.name = Go.Value[3];
            VirtualGO.tag = "VirtualObject";
            //PrecisARList.Add(VirtualGO);
            PrecisARList.Insert(Int32.Parse(Go.Value[6]),VirtualGO);
            print("reinstantiate "+VirtualGO.name);
        }
        PrecisARList.Reverse();

        print("PrecisARlist is Valid: "+PrecisARList.Count);
        if (PrecisARList.Count > 0)
        {
            Go2Next.init();
        }
        firestore.PrecisARListValid = true;
    }
    
    public void deleteLocalMap()
    {
        try
        {
            // Check if file exists 
            if (File.Exists(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID)))
            {
                File.Delete(Path.Combine(Application.persistentDataPath, StaticObject.myARmapID));
                StaticObject.debugger.text = "Map " + StaticObject.myARmapID + " is deleted succesfully in local storage";
            }
        }
        catch (IOException ioExp)
        {
            StaticObject.debugger.text = "Map " + StaticObject.myARmapID + " couldn't be deleted properly";
            Console.WriteLine(ioExp.Message);
        }

    }

    public void delete()
    {
        // Create a reference to the file to delete.
        // Delete the file
      StaticCoroutine.DoCoroutine(Database2AR.onRemoveMap());
      foreach(string path in StaticObject.listOfFiles)
        {
            //UnityEngine.Debug.Log(path);
            if (path != StaticObject.myARmapID + "/FileList")
            {
                StaticObject.Bunkyou_ref.Child(path).DeleteAsync().ContinueWith(task => {
                    if (task.IsCompleted)
                    {
                        UnityEngine.Debug.Log("File (" + path + ") deleted successfully.");
                        StaticObject.debugger.text = path + "File deleted successfully.";
                    }
                    else
                    {
                        UnityEngine.Debug.Log(path + "fail to delete");
                        UnityEngine.Debug.Log(task.Exception.ToString());
                        StaticObject.debugger.text = "File deleted fail";
                    }
                },
                System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        StaticObject.Bunkyou_ref.Child(StaticObject.myARmapID + "/FileList").DeleteAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                UnityEngine.Debug.Log("File (" + StaticObject.myARmapID + "/FileList" + ") deleted successfully.");
                StaticObject.debugger.text = StaticObject.myARmapID + "/FileList" + "File deleted successfully.";
            }
            else
            {
                UnityEngine.Debug.Log(StaticObject.myARmapID + "/FileList" + "fail to delete");
                UnityEngine.Debug.Log(task.Exception.ToString());
                StaticObject.debugger.text = "File deleted fail";
            }
        }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        foreach (KeyValuePair<List<float>, List<string>> Go in StaticObject.addedGO)
        {
            StaticObject.Bunkyou_ref.Child("Images/"+Go.Value[3]+"jpg").DeleteAsync().ContinueWith(task => {
                if (task.IsCompleted)
                {
                    UnityEngine.Debug.Log("File (" + Go.Value[3] + "jpg" + ") deleted successfully.");
                    StaticObject.debugger.text = Go.Value[3] + "jpg" + "File deleted successfully.";
                }
                else
                {
                    UnityEngine.Debug.Log(Go.Value[3] + "jpg" + "fail to delete");
                    UnityEngine.Debug.Log(task.Exception.ToString());
                    StaticObject.debugger.text = "File deleted fail";
                }
            },
            System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }





    }



}


    


