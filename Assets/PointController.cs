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
        GameObject panel = Instantiate(panelPrefab, transform.position + new Vector3(0, 10f, 0), panelPrefab.transform.rotation);
        panel.transform.SetParent(transform);
        panel.tag = "little_panel";
        panel.GetComponentInChildren<Text>().text = Firebase2Map.myPointExist[transform.gameObject].placeName;
    }


}
