using UnityEngine;

public class GPSController : MonoBehaviour
{
    public double height;

    public double latitude ;   // 緯度
    public double longitude;  // 経度

    const double lat2km = 111319.491;
    public Vector3 ConvertGPS2ARCoordinate(LocationInfo location)
    {
        double dz = (latitude - location.latitude) * lat2km;   // -zが南方向
        double dx = (longitude - location.longitude) * lat2km; // +xが東方向
        return new Vector3((float)dx, (float)height, (float)dz);
    }

    void Start()
    {
        Input.location.Start();

        InvokeRepeating("UpdateGPS", 0.2f,0.5f);

    }

    public void UpdateGPS()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                LocationInfo location = Input.location.lastData;

                transform.position = ConvertGPS2ARCoordinate(location);
                //print(this.name);
                print(location);
               
            }
        }
    }
}