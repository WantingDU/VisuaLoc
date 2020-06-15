using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hammer : MonoBehaviour
{
    int count = 0;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Mole"))
                {
                    hit.transform.gameObject.GetComponent<MoleController>().Hit();
                    bool hitBad=hit.transform.gameObject.GetComponent<MoleController>().badMole;
                    if (hitBad)
                    {
                        count++;
                    }
                    else
                    {
                        count--;
                    }
                   
                    GameObject.Find("Score").GetComponent<Text>().text = "Score: " + count.ToString();
                }
            }
        }
    }
}