using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointController : MonoBehaviour
{
    public GameObject panelPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Hit()
    {
        GameObject panel = Instantiate(panelPrefab, transform.position + new Vector3(0, 0.04f, 0), panelPrefab.transform.rotation);
        panel.tag = "little_panel";
        panel.GetComponentInChildren<Text>().text = transform.name;
        print(transform.name);
    }


}
