using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreteriaSetting : MonoBehaviour
{
    public static int CurrentFrame_cri = 150;
    public static int Total_cri=10000;
    public static int Weighted_cri=300;
    InputField CF_input, To_input, We_input;
    public static CanvasGroup cg;
    // Start is called before the first frame update
    void Start()
    {
        cg = transform.GetComponent<CanvasGroup>();
        CF_input = GameObject.Find("CF_cri").GetComponent<InputField>();
        To_input = GameObject.Find("To_cri").GetComponent<InputField>();
        We_input = GameObject.Find("We_cri").GetComponent<InputField>();
        CF_input.placeholder.GetComponent<Text>().text = "Current Frame :" + CurrentFrame_cri.ToString();
        To_input.placeholder.GetComponent<Text>().text = "Total :" + Total_cri.ToString();
        We_input.placeholder.GetComponent<Text>().text = "Weighted :" + Weighted_cri.ToString();
    }
    public void change_criteria()
    {
        if (CF_input.text.Length > 0)
        {
            CurrentFrame_cri = Int32.Parse(CF_input.text);
        }
        if (To_input.text.Length>0)
        {
            Total_cri = Int32.Parse(To_input.text);
        }
        if (We_input.text.Length > 0)
        {
            Weighted_cri = Int32.Parse(We_input.text);
        }
        //StaticCoroutine.DoCoroutine(EventScript.Go2ARTask());
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        StaticObject.myARmapID = StaticObject.getGUID();

    }
}
