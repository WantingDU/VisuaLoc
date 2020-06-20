using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventScript2 : MonoBehaviour
{
    GameObject inputFieldPrefab;

    CanvasGroup CG;
    public void Awake()
    {
        inputFieldPrefab = Resources.Load<GameObject>("MapNameInput");
        
    }
    public void Start()
    {
        CG=GameObject.Find("ToolBar").GetComponent<CanvasGroup>();
        SetVisibleDeleteButton();
    }
    public void ClearAll()
    {
        CommonVariables.reference.RemoveValueAsync().ContinueWith(
            (task) =>
            {
                if (task.IsCanceled || task.IsFaulted) Debug.Log("Fail");
                else if (task.IsCompleted) Debug.Log("Success");
            },
            System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext()
            );
        foreach (GameObject p in Database2AR.PanelExist)
            Destroy(p);

    }
    public void ClearGO()
    {
        
        foreach (GameObject p in Database2AR.PanelExist)
            Destroy(p);
    }
    public void DisplayTools()
    {
        if (CG.alpha ==0)
        {
            CG.alpha = 1;
            CG.blocksRaycasts = true;
        }
        else
        {
            CG.alpha = 0;
            CG.blocksRaycasts = false;
        }

    }
    /*
    public void StopInstantiate()
    {
        Database2AR.stop = true;
    }
    public void restart()
    {
        Database2AR.stop = false;
    }*/
    public void Go2Map()
    {
        SceneManager.LoadSceneAsync("MapView");

    }
    void SetVisibleDeleteButton()
    {
        print("ARMapID=>?？？"+ StaticObject.myARmapID);
        print("UserId=>?"+Auth.UserSelfId);
        Database2AR.ARMapRef.Child(StaticObject.myARmapID).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("task.IsFaulted");
                Debug.Log(task.Exception);
                var canvasGroup = GameObject.Find("DeleteButton").GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string ARMapHolderID = snapshot.Child("UserID").Value.ToString();
                print("ARMapHolderID=" + ARMapHolderID);
                if (ARMapHolderID != Auth.UserSelfId)
                {
                    var canvasGroup = GameObject.Find("DeleteButton").GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = false;
                }
            }

        });
    }
    /*
    public void Go2AR()
    {
        SceneManager.LoadSceneAsync("ARView");

    }*/

    public void OnClickNew()
    {
        GameObject inputField = Instantiate(inputFieldPrefab, transform.position + transform.forward * 300f, transform.rotation);
        inputField.name = "InputField";
    }
    public void onInputMapName()
    {
        print("Clicked ok!");
        GameObject myInput = GameObject.Find("InputField");
        firestore.ScenePublic = GameObject.Find("SceneToggle").GetComponent<Toggle>().isOn;
        StaticObject.myARmapName = GameObject.Find("InputField").transform.GetChild(0).GetComponentInChildren<InputField>().text;
        if (StaticObject.myARmapName == "Default")
        {
            GameObject.Find("InputDebugger").GetComponent<Text>().text = "This name is not acceptable, please use another one";
            return;
        }
        StaticObject.myARmapID = StaticObject.getGUID();
        string mapinfo= "Map Name: " + StaticObject.myARmapName+"$Map ID: "+ StaticObject.myARmapID;
        mapinfo = mapinfo.Replace('$', '\n');
        GameObject.Find("MapInfo").GetComponent<Text>().text = mapinfo;
        Destroy(myInput);
    }
    public void onClose()
    {
        GameObject myInput = GameObject.Find("InputField");
        Destroy(myInput);
    }
    public void goSetting()
    {
        SceneManager.LoadSceneAsync("Setting");
    }
}
