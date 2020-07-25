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
    RawImage _image;
    void Start()
    {
        cg = transform.GetComponent<CanvasGroup>();
        _image = transform.GetComponentInChildren<RawImage>();
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(Screen.width,Screen.height);
        _image.texture = myTexture;
        if (StaticObject.myARmapName == "Default")
        {
            cg.alpha = 0;
            isShow = false;
            return;
        }
        StartCoroutine(firestore.LoadScreenshot(StaticObject.myARmapID + "/ARMapScreenshot.jpg", gameObject));
        isShow = true;
        
    }

    private void OnDestroy()
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        transform.GetComponentInChildren<RawImage>().texture = myTexture;
    }

    public void switchView()
    {
        if (isShow)
        {
            isShow = false;
            cg.alpha = 0;
        }
        else
        {
            isShow = true;
            cg.alpha = 1;
        }
    }
}
