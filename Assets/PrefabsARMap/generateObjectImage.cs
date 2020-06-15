#if (UNITY_EDITOR)


using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;




public class generateObjectImage : MonoBehaviour
{

    public static string  save_file_name;

    public static Texture GetAssetPreview(GameObject obj)
    {
        GameObject canvas_obj = null;
        GameObject clone = Instantiate(obj);
        Transform cloneTransform = clone.transform;
        bool isUINode = false;
        if (clone.transform is RectTransform)
        {
            //如果是UGUI节点的话就要把它们放在Canvas下了
            canvas_obj = new GameObject("render canvas", typeof(Canvas));
            Canvas canvas = canvas_obj.GetComponent<Canvas>();
            clone.transform.parent = canvas_obj.transform;
            clone.transform.localPosition = Vector3.zero;

            canvas_obj.transform.position = new Vector3(-1000, -1000, -1000);
            canvas_obj.layer = 21;//放在21层，摄像机也只渲染此层的，避免混入了奇怪的东西
            isUINode = true;
        }
        else
            clone.transform.position = new Vector3(-1000, -1000, -1000);

        Transform[] all = clone.GetComponentsInChildren<Transform>();
        foreach (Transform trans in all)
        {
            //print(trans.name);
            trans.gameObject.layer = 21;
        }

        Bounds bounds = GetBounds(clone);
        Vector3 Min = bounds.min;
        Vector3 Max = bounds.max;
        GameObject cameraObj = new GameObject("render camera");

        Camera renderCamera = cameraObj.AddComponent<Camera>();
        //renderCamera.backgroundColor =  new Color(0.8f, 0.8f, 0.8f, 1f) ;
        renderCamera.backgroundColor = Color.clear;// new Color(0.8f, 0.8f, 0.8f, 1f) ;
        renderCamera.clearFlags = CameraClearFlags.Color;
        renderCamera.cameraType = CameraType.Preview;
        renderCamera.cullingMask = 1 << 21;
        renderCamera.nearClipPlane = 0.01f;

        //renderCamera.renderingPath = RenderingPath.Forward;


        if (isUINode)
        {
            Debug.Log("this is ui node!");
            //cameraObj.transform.position = new Vector3((Max.x + Min.x) / 2f, (Max.y + Min.y) / 2f, clone.transform.position.z - 100);
            cameraObj.transform.position = new Vector3(-990, -1000, -1100);
            Vector3 center = new Vector3(clone.transform.position.x, (Max.y + Min.y) / 2f, clone.transform.position.z);
            cameraObj.transform.LookAt(new Vector3(-1000,-1000,-1000));

            renderCamera.orthographic = true;
            float width = Max.x - Min.x;
            float height = Max.y - Min.y;
            float max_camera_size = width > height ? width : height;
            renderCamera.orthographicSize = max_camera_size / 2;//预览图要尽量少点空白
        }
        else
        {
            print("hey!"+Max.z+","+Min.z);
            cameraObj.transform.position = new Vector3((Max.x + Min.x) / 2f, (Max.y + Min.y) / 2f, Max.z + (Max.z - Min.z)+1f);
            Vector3 center;
            if (clone.transform.childCount > 0)
            {
                center = new Vector3(clone.transform.GetChild(0).position.x, (Max.y + Min.y) / 2f, clone.transform.position.z);
            }
            else
            {
                center = new Vector3(clone.transform.position.x, (Max.y + Min.y) / 2f, clone.transform.position.z);
            }
            print(center);
            cameraObj.transform.LookAt(center);

            int angle = (int)(Mathf.Atan2((Max.y - Min.y) / 2, (Max.z - Min.z)) * 180 / 3.1415f * 2);
            renderCamera.fieldOfView = angle;
        }
        RenderTexture texture = new RenderTexture(512, 512, 0, RenderTextureFormat.Default);
        renderCamera.targetTexture = texture;
        renderCamera.Render();

        Undo.DestroyObjectImmediate(cameraObj);
        Undo.PerformUndo();//不知道为什么要删掉再Undo回来后才Render得出来UI的节点，3D节点是没这个问题的，估计是Canvas创建后没那么快有效？
        renderCamera.RenderDontRestore();
        RenderTexture tex = new RenderTexture(512, 512, 0, RenderTextureFormat.Default);
        Graphics.Blit(texture, tex);

        Object.DestroyImmediate(canvas_obj);
        Object.DestroyImmediate(cameraObj);
        Object.DestroyImmediate(clone);
        return tex;
    }

        

    public static Bounds GetBounds(GameObject obj)
    {
        Vector3 Min = new Vector3(99999, 99999, 99999);
        Vector3 Max = new Vector3(-99999, -99999, -99999);
        MeshRenderer[] renders = obj.GetComponentsInChildren<MeshRenderer>();
        if (renders.Length > 0)
        {
            for (int i = 0; i < renders.Length; i++)
            {
                if (renders[i].bounds.min.x < Min.x)
                    Min.x = renders[i].bounds.min.x;
                if (renders[i].bounds.min.y < Min.y)
                    Min.y = renders[i].bounds.min.y;
                if (renders[i].bounds.min.z < Min.z)
                    Min.z = renders[i].bounds.min.z;

                if (renders[i].bounds.max.x > Max.x)
                    Max.x = renders[i].bounds.max.x;
                if (renders[i].bounds.max.y > Max.y)
                    Max.y = renders[i].bounds.max.y;
                if (renders[i].bounds.max.z > Max.z)
                    Max.z = renders[i].bounds.max.z;
              
            }
        }
        else
        {
            RectTransform[] rectTrans = obj.GetComponentsInChildren<RectTransform>();
            Vector3[] corner = new Vector3[4];
            for (int i = 0; i < rectTrans.Length; i++)
            {
                //获取节点的四个角的世界坐标，分别按顺序为左下左上，右上右下
                rectTrans[i].GetWorldCorners(corner);
                if (corner[0].x < Min.x)
                    Min.x = corner[0].x;
                if (corner[0].y < Min.y)
                    Min.y = corner[0].y;
                if (corner[0].z < Min.z)
                    Min.z = corner[0].z;

                if (corner[2].x > Max.x)
                    Max.x = corner[2].x;
                if (corner[2].y > Max.y)
                    Max.y = corner[2].y;
                if (corner[2].z > Max.z)
                    Max.z = corner[2].z;
               
            }
        }

        Vector3 center = (Min + Max) / 2;
        Vector3 size = new Vector3(Max.x - Min.x, Max.y - Min.y, Max.z - Min.z);
        return new Bounds(center, size);

    }
    public static bool SaveTextureToPNG(Texture inputTex, string save_file_name)
    {
        RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(inputTex, temp);
        bool ret = SaveRenderTextureToPNG(temp, save_file_name);
        RenderTexture.ReleaseTemporary(temp);
        return ret;
    }
    public static void changeTextureType(string startPath) {
        DirectoryInfo d = new DirectoryInfo(startPath);

        foreach (var file in d.GetFiles("*.png"))
        {
            Debug.Log(file.Name);
            TextureImporter importer = AssetImporter.GetAtPath(startPath + "/" + file.Name) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            AssetDatabase.WriteImportSettingsIfDirty(save_file_name);
        }   
    }

    //将RenderTexture保存成一张png图片  
    public static bool SaveRenderTextureToPNG(RenderTexture rt, string save_file_name)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();
        string directory = Path.GetDirectoryName(save_file_name);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        FileStream file = File.Open(save_file_name, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
        Texture2D.DestroyImmediate(png);
        png = null;
        RenderTexture.active = prev;
        //changeTextureType(save_file_name);
        return true;
    }




}
#endif