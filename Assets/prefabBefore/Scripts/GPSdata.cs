using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GPSdata : MonoBehaviour
{
    public double height;
    public bool GPSInfo;
    public string place_name;
    public double latitude;   // 緯度
    public double longitude;  // 経度
    const double lat2km = 111319.491;

    public Vector3 ConvertGPS2ARCoordinate(LocationInfo location)
    {
        double dy = (height - location.altitude);
        double dz = (latitude - location.latitude) * lat2km;   // ＋zが南方向
        double dx = (longitude - location.longitude) * lat2km; // +xが東方向
        return new Vector3((float)dx, (float)dy, (float)dz); // なんでy軸での表現がなさそう？下にいるはずなのに
    }

    void Start()
    {
                AddTextToScence(place_name, this.gameObject);

                //transform.parent.parent.position = ConvertGPS2ARCoordinate(CommonVariables.location);
                transform.parent.parent.LookAt(Camera.main.transform);
                transform.parent.parent.Rotate(new Vector3(0, 180, 0));




        //Instantiate(mytext, this.transform.position, this.transform.rotation);
        /*
        if (GPSInfo)
        {
            InvokeRepeating("UpdateGPS", 0.2f, 1f);
        }*/

    }


    public void UpdateGPS()
    {
            Text txt = gameObject.GetComponent<Text>();
            Vector3 coor = ConvertGPS2ARCoordinate(CommonVariables.location);
            Vector3 nulCoor = new Vector3(0f, 0f, 0f);
        float dist = Vector3.Distance(coor, nulCoor);
        /*if (dist >= 100)
        {
            this.transform.parent.parent.gameObject.SetActive(false);
        }*/
        //else
        //{
        //this.transform.parent.parent.gameObject.SetActive(true);
        string x = coor.ToString()+"distance="+dist.ToString();
            txt.text = place_name + " " + x;
            // transform.parent.parent.LookAt(Camera.main.transform);
            //transform.parent.parent.Rotate(new Vector3(0, 180, 0));
            //transform.parent.parent.position = ConvertGPS2ARCoordinate(location);
        //}
            
    }
    



    public static void AddTextToScence(string textString, GameObject canvasGameObject)
    {
        
        Text mytext = canvasGameObject.AddComponent<Text>();
        mytext.text = textString;
        mytext.verticalOverflow = VerticalWrapMode.Overflow;
        mytext.horizontalOverflow = HorizontalWrapMode.Wrap;
        mytext.alignment = TextAnchor.MiddleCenter;
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        mytext.font = ArialFont;
        
        mytext.material = ArialFont.material;
        mytext.fontSize =300;
        canvasGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1600, 80);
        mytext.color = Color.red;
        //mytext.transform.localScale = new Vector3(10,10,10);
    }
}