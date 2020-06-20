using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SaveResult
{
    public string guid;
    public string name;
    public string userID;
    public string type;
    public string CF;
    public string Total;
    public string We;

    public SaveResult()
    {
    }

    public SaveResult(string guid, string name, string userID, string type, string CF, string Total, string We)
    {
        this.guid = guid;
        this.name = name;
        this.userID = userID;
        this.type = type;
        this.CF = CF;
        this.Total = Total;
        this.We = We;
    }
}

public class RebuildResult
{
    public string guid;
    public string succes;
    public string time;

    public RebuildResult()
    {
    }

    public RebuildResult(string guid,string succes, string time)
    {
        this.guid = guid;
        this.succes = succes;
        this.time = time;
    }
}
public class PersistenceTest : MonoBehaviour
{
    public static void writeNewTest(string guid, string name, string userID, string type, string CF, string Total, string We)
    {

        SaveResult new_result = new SaveResult( guid,  name,  userID,  type,  CF,  Total,  We);
        string json = JsonUtility.ToJson(new_result);
        CommonVariables.reference.Child("SaveResults").Child(guid).SetRawJsonValueAsync(json);

    }
    public static void writeNewRebuild(string guid, string succes, string time)
    {

        RebuildResult new_result = new RebuildResult(guid,succes,time);
        string json = JsonUtility.ToJson(new_result);
        CommonVariables.reference.Child("RebuildResults").Child(guid).SetRawJsonValueAsync(json);

    }

}
