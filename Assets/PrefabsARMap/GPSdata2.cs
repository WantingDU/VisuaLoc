using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GPSdata2 : MonoBehaviour
{
    public double height;
    public bool GPSInfo;
    public string place_name;
    public double latitude;   // 緯度
    public double longitude;  // 経度
    const double lat2km = 111319.491;
    public int distBefore=0;

    public Vector3 ConvertGPS2ARCoordinate(LocationInfo location)
    {
        double dy = (height - location.altitude);
        double dz = -(latitude - location.latitude) * lat2km;   // ＋zが南方向
        double dx = -(longitude - location.longitude) * lat2km; // +xが東方向
        return new Vector3((float)dx, (float)dy, (float)dz); //
    }

    void Start()
    {
                Vector3 coor = ConvertGPS2ARCoordinate(CommonVariables.location);
                AddTextToScence(place_name+" ("+coor.x+","+coor.y+ "," + coor.z+")", this.gameObject);
                transform.root.LookAt(Camera.main.transform);
                transform.root.Rotate(new Vector3(0, 180, 0));
                UpdateGPS();
    }


    public void UpdateGPS()
    {
            Text txt = gameObject.GetComponent<Text>();
            Vector3 coor = ConvertGPS2ARCoordinate(CommonVariables.location);
            Vector3 nulCoor = new Vector3(0f, 0f, 0f);
            int dist =(int) Vector3.Distance(coor, nulCoor);
            if (distBefore!=0)
            {
                txt.fontSize = Convert.ToInt32(((float)dist / (float)distBefore)*50*(float)dist/20);
               
            }
            distBefore = (int)Vector3.Distance(coor, nulCoor);
            string x = dist.ToString();
            txt.text = place_name + " " + x;
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
        mytext.fontSize =50;
        canvasGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1600, 120);
        mytext.color = Color.blue;
        //mytext.transform.localScale = new Vector3(10,10,10);
    }
}