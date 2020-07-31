using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

public class MyPoint
{
    public string guid;
    public string placeName;
    public Vector2d coordinate;

    public MyPoint()
    {
    }

    public MyPoint(string guid,string placeName, Vector2d coordinate)
    {
        this.guid = guid;
        this.placeName = placeName;
        this.coordinate = coordinate;
    }
}
public class MyMapPoint
{
    public string guid;
    public string placeName;
    public Vector2d coordinate;
    public string UserID;
    public bool IsPublic;
    public string lastUpdateTime;
    public MyMapPoint()
    {
    }

    public MyMapPoint(string guid, string placeName, Vector2d coordinate, string UserID, bool IsPublic,string lastUpdateTime)
    {
        this.guid = guid;
        this.placeName = placeName;
        this.coordinate = coordinate;
        this.UserID = UserID;
        this.IsPublic = IsPublic;
        this.lastUpdateTime = lastUpdateTime;
    }
}


