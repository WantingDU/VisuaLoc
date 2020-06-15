using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

public class MyPoint
{
    public string placeName;
    public Vector2d coordinate;

    public MyPoint()
    {
    }

    public MyPoint(string placeName, Vector2d coordinate)
    {
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
    public MyMapPoint()
    {
    }

    public MyMapPoint(string guid, string placeName, Vector2d coordinate, string UserID, bool IsPublic)
    {
        this.guid = guid;
        this.placeName = placeName;
        this.coordinate = coordinate;
        this.UserID = UserID;
        this.IsPublic = IsPublic;
    }
}


