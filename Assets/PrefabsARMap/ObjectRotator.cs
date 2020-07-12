using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ObjectRotator : MonoBehaviour
{

    private float _sensitivity;
    private Vector3 _rotation;


    //Vector3  defaultScale= Vector3.one;
    //Vector3 newScale  = new Vector3(2,2,2); //Twice the size.
    //bool scaled;

    private float width;
    private float height;
    void Start()
    {
        _sensitivity = 3f;
        _rotation = Vector3.zero;
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;

    }

    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)//>0
        {
            Vector2 pos = Input.GetTouch(0).position;
            pos.x = (pos.x - width) / width;
            
            // apply rotation
            _rotation.y = -pos.x * _sensitivity;

            // rotate
            transform.Rotate(_rotation);


            //change z
            pos.y = (pos.y - height)/height/200f;
            transform.position+= new Vector3(0,pos.y,0);

        }
    }
    
}
