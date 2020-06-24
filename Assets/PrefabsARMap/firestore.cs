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
    private UnityARAnchorManager unityARAnchorManager;
    public static List<GameObject> PrecisARList;
    public static int checkFile;
    public static bool PrecisARListValid;
    public static ARWorldMap loadedMap;
    public ParticleSystem pointCloudParticlePrefab;
    private ParticleSystem totalPS;
    public static int FilesLoaded= 0;
    public static Stopwatch sw=new Stopwatch();

    bool view = true;
    private void OnDestroy()
    {
        PrecisARList = null;
        checkFile = 0;
        PrecisARListValid = false;
        FilesLoaded = 0;
        print("ondestroy");
        loadedMap = null;
        UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = false;
        
    }
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

//evaluate by viewpoint
    public void onSave()
    {
        if (StaticObject.myARmapName == "Default")
        {
            StaticObject.debugger.text = "Current ARMap doesn't have a name please click button 'new' to create one";
            return;
        }
        /*
        if (PointCloudParticleExampleVersionDu.CPNumber <= 150)
        {
            StaticObject.debugger.text = "Current scene is not able to be rebuilt, please retry";
            return;
        }
        */
        Vector3 pos = Camera.main.transform.position;
        int viewPoint = 0;
        WorldMapManager.session.GetCurrentWorldMapAsync(worldMap =>
        {
            foreach (Vector3 v in worldMap.pointCloud.Points)
            {
                Vector2 viewPos = Camera.main.WorldToViewportPoint(v); //viewport pos of point
                Vector3 dir = (v - pos).normalized;
                float dot = Vector3.Dot(Camera.main.transform.forward, dir);     //判断物体是否在相机前面
                float dist = Vector3.Distance(pos, v);
                if (dist <= 3 && dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                {
                    viewPoint++;
                }
            }
            GameObject.Find("CPTotal").GetComponent<Text>().text = "total: " + worldMap.pointCloud.Count.ToString() + " view: " + viewPoint;
            if (viewPoint <= 600)
            {
                StaticObject.debugger.text = "Current scene is not able to be rebuilt, please retry";
                return;
            }
            print("begin serializing");
            var worldMapInBytes = worldMap.SerializeToByteArray();
            StartCoroutine(WriteFile(worldMapInBytes, StaticObject.myARmapID + "/WorldData"));
            GalleryController.onScreenshotMap();
            StaticObject.debugger.text = "uploading...";
            //UploadCurrentWorldMap();
            saveObject();
            WriteARPoint2DB();


        });


    }

    //evaluate by checking current frame
    public void onSaveCF()
    {
        if (StaticObject.myARmapName =="Default")
        {
            //StaticObject.debugger.text = "Current ARMap doesn't have a name please click button 'new' to create one";
            String timeStamp = CommonVariables.GetTimestamp(DateTime.Now);
            StaticObject.myARmapName = "CF >=" + CreteriaSetting.CurrentFrame_cre + " (" + timeStamp + ")";
            //return;
        }
        float evalu = (float) PointCloudParticleExampleVersionDu.CPNumber/CreteriaSetting.CurrentFrame_cre;
        ProgressBar.SetProgressValue(evalu);
        if (PointCloudParticleExampleVersionDu.CPNumber <= CreteriaSetting.CurrentFrame_cre)
        {
            StaticObject.debugger.text = "Current scene is not able to be rebuilt, please retry";
            return;
        }
      
        WorldMapManager.session.GetCurrentWorldMapAsync (worldMap =>
        {
            print("begin serializing");
            var worldMapInBytes = worldMap.SerializeToByteArray();
            StartCoroutine(WriteFile(worldMapInBytes, StaticObject.myARmapID + "/WorldData"));
            GalleryController.onScreenshotMap();
            StaticObject.debugger.text = "uploading...";
            //UploadCurrentWorldMap();
            saveObject();
            WriteARPoint2DB();
            PersistenceTest.writeNewTest(StaticObject.myARmapID, StaticObject.myARmapName, Auth.UserSelfId, "Current Frame", PointCloudParticleExampleVersionDu.CPNumber.ToString(), worldMap.pointCloud.Count.ToString(), "Null");
            loadedMap = worldMap;
        });


    }

    //evaluate by total number
    public void onSaveTotal()
    {
        if (StaticObject.myARmapName == "Default")
        {
            //StaticObject.debugger.text = "Current ARMap doesn't have a name please click button 'new' to create one";
            String timeStamp = CommonVariables.GetTimestamp(DateTime.Now);
            StaticObject.myARmapName = "To >="+CreteriaSetting.Total_cre+" ("+timeStamp+")";
            //return;
        }
        WorldMapManager.session.GetCurrentWorldMapAsync(worldMap =>
        {
            print(worldMap.pointCloud.Count);
            float evalu = (float) worldMap.pointCloud.Count / CreteriaSetting.Total_cre;
            print(evalu);
            ProgressBar.SetProgressValue(evalu);
            if (worldMap.pointCloud.Count <= CreteriaSetting.Total_cre)
            {
                StaticObject.debugger.text = "Current scene is not able to be rebuilt, please retry";
                return;
            }
            print("begin serializing");
            var worldMapInBytes = worldMap.SerializeToByteArray();
            StartCoroutine(WriteFile(worldMapInBytes, StaticObject.myARmapID + "/WorldData"));
            GalleryController.onScreenshotMap();
            StaticObject.debugger.text = "uploading...";
            //UploadCurrentWorldMap();
            saveObject();
            WriteARPoint2DB();
            PersistenceTest.writeNewTest(StaticObject.myARmapID, StaticObject.myARmapName, Auth.UserSelfId, "Total", PointCloudParticleExampleVersionDu.CPNumber.ToString(), worldMap.pointCloud.Count.ToString(), "Null");
            loadedMap = worldMap;

        
        });

    }
    //evaluate by checking plane 
    public void onSavePlane()
    {
        if (StaticObject.myARmapName == "Default")
        {
            //StaticObject.debugger.text = "Current ARMap doesn't have a name please click button 'new' to create one";
            String timeStamp = CommonVariables.GetTimestamp(DateTime.Now);
            StaticObject.myARmapName = "We >= "+CreteriaSetting.Weighted_cre+"("+timeStamp+")";
            //return;
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
                if (dist <= 3 && dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                {
                    viewPoint+=0.3f;
                    if (StaticObject.IsOnPlane(v,p1)||StaticObject.IsOnPlane(v,p2)|| StaticObject.IsOnPlane(v, p3))//check if point in view port is also on detected plane in view port
                    {
                        viewPoint+=1f;
                    }

                }
                viewPoint =(float) Math.Round(viewPoint, 0);
            }
            GameObject.Find("CPTotal").GetComponent<Text>().text = "total: " + worldMap.pointCloud.Count.ToString() + " Plane: " + viewPoint;
            float evalu = viewPoint/ CreteriaSetting.Weighted_cre;
            ProgressBar.SetProgressValue(evalu);
            if (viewPoint <= CreteriaSetting.Weighted_cre)
            {
                StaticObject.debugger.text = "Current scene is not able to be rebuilt, please retry";
                return;
            }
            print("begin serializing");
            var worldMapInBytes = worldMap.SerializeToByteArray();
            StartCoroutine(WriteFile(worldMapInBytes, StaticObject.myARmapID + "/WorldData"));
            GalleryController.onScreenshotMap();
            StaticObject.debugger.text = "uploading...";
            //UploadCurrentWorldMap();
            saveObject();
            PersistenceTest.writeNewTest(StaticObject.myARmapID, StaticObject.myARmapName, Auth.UserSelfId, "Weighted", PointCloudParticleExampleVersionDu.CPNumber.ToString(), worldMap.pointCloud.Count.ToString(),viewPoint.ToString());
            WriteARPoint2DB();
            loadedMap = worldMap;

        });


    }
    void WriteARPoint2DB()
    {
        Vector2d m_coor=new Vector2d (CommonVariables.location.latitude, CommonVariables.location.longitude);
        CommonVariables.writeNewARMap(StaticObject.myARmapID, StaticObject.myARmapName,m_coor, ScenePublic,Auth.UserSelfId);
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
            StorageReference newMap = StaticObject.Bunkyou_ref.Child(pathName);
            newMap.PutBytesAsync(custom_bytes)
              .ContinueWith((Task<StorageMetadata> task) => {
                  if (task.IsFaulted || task.IsCanceled)
                  {
                      UnityEngine.Debug.Log(task.Exception.ToString());
                      StaticObject.debugger.text = "error while write"+pathName+" in db";
                  
                   }
                  else
                  {
                  // Metadata contains file metadata such as size, content-type, and download URL.
                  Firebase.Storage.StorageMetadata metadata = task.Result;

                      //string download_url = StaticObject.Bunkyou_ref.GetDownloadUrlAsync().ToString();
                      if (!StaticObject.listOfFiles.Contains(pathName))
                          StaticObject.listOfFiles.Add(pathName);
                      StaticObject.debugger.text = "Finished Upload" + pathName;
                      UnityEngine.Debug.Log("Finished uploading "+pathName);
                      firestore.checkFile++;
                      if (firestore.checkFile == 4)
                      {
                          StaticObject.debugger.text = "Save succesfully";
                          print("save succesfully");
                          firestore.checkFile=0;
                      }
                  }
                  
              });
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
        StaticObject.debugger.text = "Downloading...";
        const long maxAllowedSize = 100 * 1024 * 1024;
        StorageReference reference2Read = StaticObject.Bunkyou_ref.Child("Images").Child(Mypath);
        reference2Read.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                UnityEngine.Debug.Log(task.Exception.ToString());
                UnityEngine.Debug.Log("trouve pas de "+Mypath);
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
                UnityEngine.Debug.Log("trouve pas de " + Mypath);
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
        });
        yield return new WaitForEndOfFrame();
    }

    public void ReadFile(string Mypath)
    {
        print("reading"+Mypath);
        const long maxAllowedSize = 100 * 1024 * 1024;
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
                StaticObject.debugger.text = Mypath+"is ready";
                print(Mypath + "is ready");
                switch (Mypath)
                {
                    case "WorldData":
                        print("worldData found");
                        LoadWorldMap(fileContents);//！！！this method causes crash when is called for 2nd times(after scene change)
                        break;
                    case "ObjectInfo":
                        StaticObject.addedGO = byte2Dict(fileContents);
                        print("objectinfo found");
                        print("StaticObject.addedGO"+StaticObject.addedGO.Count);
                        FilesLoaded++;
                        StartCoroutine(reInstantiateGo(StaticObject.addedGO));
                        break;
                    case "FileList":
                        StaticObject.listOfFiles = byte2List(fileContents);
                        FilesLoaded++;
                        print("FileList found");
                        break;

                }

               
            }
        });


    }
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
   
    }

    private void LoadWorldMap(byte[] worldMapInBytes)
    {
        print("Loading WorldMap!");
        ARWorldMap worldMap = ARWorldMap.SerializeFromByteArray(worldMapInBytes);
        FilesLoaded++;
        print("FilesLoaded++ " + FilesLoaded);
        print("finished serialize");
        UnityEngine.Debug.LogFormat("Map loaded. Center: {0} Extent: {1}", worldMap.center, worldMap.extent);
        UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;
        var config = m_ARCameraManager.sessionConfiguration;
        config.worldMap = worldMap;
        UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;
        //StaticObject.debugger.text = "Restarting session with worldMap";
        print("Restarting session with worldMap");
        WorldMapManager.session.RunWithConfigAndOptions(config, runOption);//!!!!!!!!!!!!!这一部分会引起crash
        //StaticObject.debugger.text = "ARWorldMap is rebuilt";
        print("ARWorldMap is rebuilt");
        loadedMap = worldMap;

    }
    public static bool FilesAreReady()
    {
        return (FilesLoaded >= 3);

    }
    public IEnumerator reInstantiateGo(Dictionary<List<float>, List<string>> dict)
    {
        print("reinstatiateGo 1");
        yield return new WaitUntil(FilesAreReady);
        print("reinstatiateGo 2");

        //timer start after all files are ready
        sw.Reset();
        sw.Start();

        print("reInstantiateGo start PrecisARList count="+dict.Count);
        PrecisARList = new List<GameObject>(dict.Count);
        foreach (KeyValuePair<List<float>, List<string>> Go in dict)
        {
            GameObject m_InfoPanel;
            switch (Go.Value[0])
            {
                case "InfoPanelOut":
                    m_InfoPanel = Instantiate(Prefab, new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]), new Quaternion(Go.Key[3], Go.Key[4], Go.Key[5], Go.Key[6]));
                    print("Start loading image of panel");
                    StartCoroutine(LoadImage(Go.Value[3] + ".jpg", m_InfoPanel));
                    m_InfoPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = Go.Value[1];
                    print("Title="+Go.Value[1]);
                    m_InfoPanel.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = Go.Value[2];
                    print("Contents=" + Go.Value[2]);
                    break;

                case "InfoPanelNoImage":
                    m_InfoPanel = Instantiate(PrefabNoImage, new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]), new Quaternion(Go.Key[3], Go.Key[4], Go.Key[5], Go.Key[6]));
                    m_InfoPanel.transform.GetChild(0).GetChild(1).GetComponentInChildren<Text>().text = Go.Value[1];
                    m_InfoPanel.transform.GetChild(0).GetChild(2).GetComponentInChildren<Text>().text = Go.Value[2];
                    break;
                default:
                    GameObject SimplePrefab = Resources.Load<GameObject>(Go.Value[0]);
                    print("find"+ SimplePrefab.name);
                    m_InfoPanel = Instantiate(SimplePrefab, new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]), new Quaternion(Go.Key[3], Go.Key[4], Go.Key[5], Go.Key[6]));
                    m_InfoPanel.transform.localScale =StaticObject.StringToVector3(Go.Value[7]);
                    break;

            }
            print("Instantiated at position "+ new Vector3(Go.Key[0], Go.Key[1], Go.Key[2]).ToString()+" with lindex: "+Go.Value[6]);
            StaticObject.SetLayerRecursively(m_InfoPanel,15);
            m_InfoPanel.name = Go.Value[3];
            //PrecisARList.Add(m_InfoPanel);
            PrecisARList.Insert(Int32.Parse(Go.Value[6]),m_InfoPanel);
            print("reinstantiate "+m_InfoPanel.name);
        }
        print("PrecisARlist is Valid: "+PrecisARList.Count);
        if (PrecisARList.Count > 0)
        {
            Go2Next.init();
        }
        firestore.PrecisARListValid = true;
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
                });
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
        });
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
            });
        }





    }



}


    


