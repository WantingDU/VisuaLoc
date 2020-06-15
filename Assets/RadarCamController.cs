using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarCamController : MonoBehaviour
{

    // Start is called before the first frame update
    //GameObject target;
    //public Transform target;
    void Start()
    {
        //target = GameObject.Find("KeepVertical");
    }

    // Update is called once per frame
    void Update()
    {
        // print(Camera.main.transform.rotation);
        transform.position = Camera.main.transform.position+new Vector3(0,100,0);
        transform.rotation = Quaternion.Euler(90, 0,-Camera.main.transform.eulerAngles.y);

    }
}
