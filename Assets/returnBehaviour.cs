using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental;
using UnityEngine.UI;

public class returnBehaviour : MonoBehaviour
{

    public void getPostContent()
    {
        
        indicator.infoPanel.name = GameObject.Find("Window/inputHere").GetComponent<InputField>().text;
        //string json = JsonUtility.ToJson(new_place);
        //print("hey 7" + CommonVariables.counter);
        //CommonVariables.writeNewPlace(indicator.infoPanel.name, CommonVariables.ConvertARCoordinate2GPS(indicator.infoPanel.transform.position));
        //CommonVariables.counter++;
        indicator.indicator2.SetActive(false);
        Destroy(indicator.infoPanel);
        //print("hey 8!!" + json);
        //CommonVariables.reference.Child("places").Child(indicator.infoPanel.name).SetRawJsonValueAsync(json);
        //generatePlaces.reference.Child("places").Child(generatePlaces.counter.ToString()).SetRawJsonValueAsync(json);
        //generatePlaces.writeNewPlace(indicator.infoPanel.name, indicator.ConvertARCoordinate2GPS(indicator.infoPanel.transform.position));

    }

}
