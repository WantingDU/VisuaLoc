using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBehaviour : MonoBehaviour
{
    public static Text Description;
    public static GameObject OkButton;
    public static CanvasGroup cg;
    // Start is called before the first frame update
    void Start()
    {
        Description = transform.GetChild(0).Find("Description").GetComponent<Text>();
        OkButton = transform.GetChild(0).Find("Button").gameObject;
        cg = transform.GetComponentInChildren<CanvasGroup>();
        Hide();
    }
    public void Go2AR()
    {
        SceneManager.LoadSceneAsync("ARView");
    }
    public static void Show()
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
    }
    public void Hide()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
}
