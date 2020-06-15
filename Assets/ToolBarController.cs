using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        int childNumber = gameObject.transform.childCount;
        float width = childNumber * transform.GetChild(0).GetComponent<Image>().rectTransform.rect.width;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, transform.GetChild(0).GetComponent<Image>().rectTransform.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
