using UnityEngine;
using UnityEditor;
public class MyEditorWindow: EditorWindow
{
    GameObject Object2Convert;
    [MenuItem("GameObject/MyWindow")]
    static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 200, 200);
        MyEditorWindow window = (MyEditorWindow)EditorWindow.GetWindowWithRect(typeof(MyEditorWindow), wr, true, "Object2Image");
        window.Show();
        
    }
    private void OnGUI()
    {

        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        if (Event.current.type == EventType.DragExited)
        {
            Debug.Log("DragExit event, mousePos:" + Event.current.mousePosition + "window pos:" + position);

            foreach (Object obj in DragAndDrop.objectReferences)
            {
                if (obj.GetType() == typeof(GameObject))
                {
                    Object2Convert = (GameObject)obj;
                    string path = "Assets/PrefabsARMap/Images/PrefabsPhoto/" + obj.name.ToString() + ".png";
                    generateObjectImage.SaveTextureToPNG(generateObjectImage.GetAssetPreview(Object2Convert), path);
                    Debug.Log("yesso!");
                }
                    
            }
        }

        if (GUILayout.Button("myButton"))  //在窗口上创建一个按钮
        {
            Debug.Log("hello");
            generateObjectImage.changeTextureType("Assets/PrefabsARMap/Images/PrefabsPhoto");
        }


        //Object2Convert = EditorGUILayout.ObjectField("add GameObject", Object2Convert, typeof(Texture), true) as GameObject;
    }

}