using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChristmasGame : MonoBehaviour
{
    public GameObject prefab;
    LocationInfo location;
    const double lat2km = 111319.491;
    int counter = 0;
    Dictionary<string, Vector3> dic = new Dictionary<string, Vector3>();
    public Vector3 ConvertGPS2ARCoordinate(LocationInfo location,Vector3 prefabLoc)
    {
        double dy = (prefabLoc.y - location.altitude);
        double dz = (prefabLoc.z - location.latitude) * lat2km;   // ＋zが南方向
        double dx = (prefabLoc.x - location.longitude) * lat2km; // +xが東方向
        return new Vector3((float)dx, (float)dy, (float)dz); // なんでy軸での表現がなさそう？下にいるはずなのに
    }

    void Start()
    {
        Input.location.Start();

        //Instantiate(mytext, this.transform.position, this.transform.rotation);

        InvokeRepeating("randomInstanciate", 0.2f, 5f);
        InvokeRepeating("UpdateGPS", 0.2f, 0.5f);
        InvokeRepeating("UpdateLocation", 0.2f, 0.5f);
 

    }
    void randomInstanciate()
    {
        Vector3 randomPos = new Vector3(location.longitude + Random.Range(-0.001f,0.001f), location.altitude + Random.Range(-20f, 20f), location.latitude + Random.Range(-0.001f, 0.001f));
        Vector3 relatifPos = ConvertGPS2ARCoordinate(location, randomPos);
        GameObject AddedTree=Instantiate(prefab,relatifPos, transform.rotation);

        counter++;
        GameObject.Find("Counter").GetComponent<Text>().text = "There are " + counter.ToString()+ "Xmas trees next to you!";
        print(counter);
        AddedTree.name = counter.ToString();
        dic.Add(AddedTree.name, randomPos);
        AddTextToScence(relatifPos.ToString(), AddedTree);
    }

    // Update is called once per frame

    public void UpdateGPS()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
          
                location = Input.location.lastData;
                foreach (KeyValuePair<string, Vector3> pair in dic)
                {
                    
                    GameObject obj = GameObject.Find(pair.Key);
                    obj.transform.position = ConvertGPS2ARCoordinate(location, pair.Value);

                    if (obj.GetComponent<Text>()!=null)
                    {
                        Text txt = obj.GetComponent<Text>();
                        string x = ConvertGPS2ARCoordinate(location,pair.Value).ToString();
                        txt.text =  x;
                        

                    }
                    else
                    {
                        AddTextToScence(pair.Value.ToString(), obj);

                    }

                }

            }
        }
    }

    public static void AddTextToScence(string textString, GameObject canvasGameObject)
    {
        print("jajoute" + textString);
        Text mytext = canvasGameObject.AddComponent<Text>();
        mytext.text = textString;

        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        mytext.font = ArialFont;
        mytext.material = ArialFont.material;
        mytext.fontSize = 16;
        mytext.color = Color.green;
    }
}