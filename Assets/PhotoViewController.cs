using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoViewController : MonoBehaviour
{
    // Start is called before the first frame update
    CanvasGroup cg;
    public Texture2D myTexture;
    bool isShow;
    void Start()
    {
        cg = transform.GetComponent<CanvasGroup>();
        transform.GetComponentInChildren<RawImage>().texture = myTexture;
        StartCoroutine(firestore.LoadScreenshot(StaticObject.myARmapID + "/ARMapScreenshot.jpg", gameObject));

        isShow = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (StaticObject.isTracked)
        {
            Hide();
        }
    }
    private void OnDestroy()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        transform.GetComponentInChildren<RawImage>().texture = myTexture;
    }
    private void Hide()
    {
        isShow = false;
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
    private void Show()
    {
        isShow = true;
        cg.alpha = 1;
        cg.blocksRaycasts = true;
    }
    public void switchView()
    {
        if (isShow)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
}
