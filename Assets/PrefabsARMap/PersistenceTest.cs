using System;
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
    public string timeStamp;

    public RebuildResult()
    {
    }

    public RebuildResult(string guid,string succes, string time,string timeStamp)
    {
        this.guid = guid;
        this.succes = succes;
        this.time = time;
        this.timeStamp = timeStamp;
    }
}
public class RebuildResult_simple
{
    public string guid;
    public string name;
    public string succes;
    public string time;
    public string timeStamp;
    public string UserID;
    public RebuildResult_simple()
    {
    }

    public RebuildResult_simple(string guid, string succes, string time, string timeStamp,string name,string UserID)
    {
        this.guid = guid;
        this.succes = succes;
        this.time = time;
        this.timeStamp = timeStamp;
        this.name = name;
        this.UserID = UserID;
    }
}


public class PersistenceTest : MonoBehaviour
{
    public static void writeNewTest(string guid, string name, string userID, string type, string CF, string Total, string We)
    {

        SaveResult new_result = new SaveResult( guid,  name,  userID,  type,  CF,  Total,  We);
        string json = JsonUtility.ToJson(new_result);
        CommonVariables.reference.Child("SaveResults_test").Child(name).SetRawJsonValueAsync(json);

    }
    public static void writeNewRebuild(string guid, string succes, string time,string timeStamp)
    {

        RebuildResult new_result = new RebuildResult(guid,succes,time,timeStamp);
        string json = JsonUtility.ToJson(new_result);
        CommonVariables.reference.Child("RebuildResults").Child(guid).Child(timeStamp).SetRawJsonValueAsync(json);

    }
    public static void writeNewRebuild_simple(string guid, string succes, string time, string timeStamp, string name,string userID)
    {

        RebuildResult_simple new_result = new RebuildResult_simple(guid, succes, time, timeStamp,  name,userID);
        string json = JsonUtility.ToJson(new_result);
        CommonVariables.reference.Child("RebuildResults_Test").Child(name).Child(timeStamp).SetRawJsonValueAsync(json);

    }

}
