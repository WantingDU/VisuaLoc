using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using UnityEngine.UI;
using System;
using UnityEngine.XR.iOS;

public class StaticObject : MonoBehaviour
{

    public static FirebaseStorage storage;
    public static StorageReference storage_ref;
    public static StorageReference ARWorldMap_ref;
    public static StorageReference Bunkyou_ref;
    public static Dictionary<List<float>, List<string>> addedGO; //key:position+rotation, Value:prefabName+title+contents+guid+userid+ispublic+  order in scene +local scale;
    public static Dictionary<int,String> mapDict;
    public static string myARmapName;
    public static string myARmapID;
    public static Text debugger;
    public static bool photoAdded;
    public static List<string> listOfFiles;
    public static float tempSearchRange;
    public static int currentOrder;
    private void OnDestroy()
    {
        addedGO = null;
        mapDict = null;
        myARmapName = "Default";
        myARmapID = null;
        photoAdded = false;
        listOfFiles = null;
        currentOrder = 0;
    }
    public void Awake()
    {
        currentOrder = 0;
        StaticObject.storage = FirebaseStorage.DefaultInstance;
        StaticObject.storage_ref = StaticObject.storage.GetReferenceFromUrl("gs://shareardubidu.appspot.com");
        StaticObject.ARWorldMap_ref = StaticObject.storage_ref.Child("ARWorldMap");
        StaticObject.Bunkyou_ref = StaticObject.storage_ref.Child("Bunkyou");
        StaticObject.debugger = GameObject.Find("Debugger").GetComponent<Text>();
        debugger.text = "Debugger";
    }
    public static string getGUID()
    {
        
        System.Guid guid = new Guid();
        guid = Guid.NewGuid();
        string str = guid.ToString();
        return str;
    }
    //this method is for forcing a Texture2D to be readable
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

    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
    public static bool IsOnPlane(Vector3 p1, Vector3 planeCenter)
    {
        if (Math.Abs(p1.z - planeCenter.z) < 0.01 || Math.Abs(p1.y - planeCenter.y) < 0.01)
        {
            print("delta z="+(p1.z - planeCenter.z) + "delta y=" + (p1.y - planeCenter.y));
            return true;
        }
        return false;
    }
    public static Vector3 viewTo3D(ARPoint point)
    {

        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface
            .GetARSessionNativeInterface()
            .HitTest(point, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);

        // 平面とあたっていた場合
        if (hitResults.Count > 0)
        {
            return (UnityARMatrixOps.GetPosition(hitResults[0].worldTransform));

        }
        return Vector3.zero;
    }
}


