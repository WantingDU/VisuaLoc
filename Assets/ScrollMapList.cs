using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScrollMapList : MonoBehaviour
{
    

    public static void initList(Dictionary<string,string> list)
    {
        GameObject prefabButton = Resources.Load<GameObject>("Button4List");
        double height = list.Count * (prefabButton.GetComponent<RectTransform>().rect.height+50);
        GameObject Container = GameObject.Find("ComponentContainer");
        Container.GetComponent<RectTransform>().sizeDelta = new Vector2(400,(float) Math.Round(height,0));
        foreach (KeyValuePair<string,string > map in list){
            GameObject mapbutton = Instantiate(prefabButton);
            mapbutton.transform.parent = Container.transform;
            mapbutton.GetComponentInChildren<Text>().text = map.Value;
            mapbutton.name = map.Key; //key is the guid of ARMap
            mapbutton.GetComponent<Button>().onClick.AddListener(() => { buttonClicked(map.Key, map.Value); });
        }
    }
    public static void buttonClicked(string id,string MapName)
    {
        StaticObject.myARmapID = id;
        StaticObject.myARmapName = MapName;
        GameObject.Find("SearchPanel").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("SearchPanel").GetComponent<CanvasGroup>().blocksRaycasts = false;
        //SceneManager.LoadSceneAsync("ARView");
        LoadingBehaviour.Show();
        LoadingBehaviour.Description.text = StaticObject.myARmapName;
        GameObject LoadPanel = GameObject.Find("LoadingInterface");
        StaticCoroutine.DoCoroutine(firestore.LoadScreenshot(id + "/ARMapScreenshot.jpg", LoadPanel));
        
    }
}
