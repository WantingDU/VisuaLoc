using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventScript : MonoBehaviour
{
    GameObject panel2active;
    bool active;
    CanvasGroup canvasGroup;
    Coroutine SceneLoaderThread;
    void Awake()
    {
        panel2active = GameObject.Find("SearchPanel");
        canvasGroup = panel2active.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; 
        canvasGroup.blocksRaycasts = false; 
    }

    /*#THIS IS ONLY FOR DEVELOPER AKA DUBI#
    public void ClearAll()
    {
        FirebaseScript.placesRef.RemoveValueAsync().ContinueWith(
            (task) =>
            {
                if (task.IsCanceled || task.IsFaulted) Debug.Log("Fail");
                else if (task.IsCompleted) Debug.Log("Success");
            });
        foreach (GameObject p in FirebaseScript.placesExist)
            Destroy(p);

    }
    public void ClearGO()
    {

        foreach (GameObject p in FirebaseScript.placesExist)
            Destroy(p);
    }*/

    public void Go2Map()
    {
        SceneManager.LoadSceneAsync("MapView");
    }
    public void Go2AR()
    {
        StaticObject.myARmapID = StaticObject.getGUID();
        StaticCoroutine.DoCoroutine(Go2ARTask());
        //SceneManager.LoadSceneAsync("ARView");
    }
    public static IEnumerator Go2ARTask()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ARView");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }
    public void addInfo()
    {
        indicator.addedNewPanel = false;
    } 
    public void setActivePanel()
    {
        active = !active;
        if (active)
        {
            canvasGroup.alpha = canvasGroup.alpha = 1f; 
            canvasGroup.blocksRaycasts = true; 
        }
        else
        {
            canvasGroup.alpha = canvasGroup.alpha = 0f; //this makes everything transparent
            canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        }
        ScrollMapList.initList(Firebase2Map.DictOfARMap);
    }

}
