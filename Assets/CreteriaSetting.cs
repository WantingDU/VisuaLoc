using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreteriaSetting : MonoBehaviour
{
    public static int CurrentFrame_cre = 200;
    public static int Total_cre=3000;
    public static int Weighted_cre=600;
    InputField CF_input, To_input, We_input;
    public static CanvasGroup cg;
    // Start is called before the first frame update
    void Start()
    {
        cg = transform.GetComponent<CanvasGroup>();
        CF_input = GameObject.Find("CF_cre").GetComponent<InputField>();
        To_input = GameObject.Find("To_cre").GetComponent<InputField>();
        We_input = GameObject.Find("We_cre").GetComponent<InputField>();
        CF_input.placeholder.GetComponent<Text>().text = "Current Frame :" + CurrentFrame_cre.ToString();
        To_input.placeholder.GetComponent<Text>().text = "Total :" + Total_cre.ToString();
        We_input.placeholder.GetComponent<Text>().text = "Weighted :" + Weighted_cre.ToString();
    }
    public void change_creteria()
    {
        if (CF_input.text.Length > 0)
        {
            CurrentFrame_cre = Int32.Parse(CF_input.text);
        }
        if (To_input.text.Length>0)
        {
            Total_cre = Int32.Parse(To_input.text);
        }
        if (We_input.text.Length > 0)
        {
            Weighted_cre = Int32.Parse(We_input.text);
        }
        //StaticCoroutine.DoCoroutine(EventScript.Go2ARTask());
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        StaticObject.myARmapID = StaticObject.getGUID();

    }
}
