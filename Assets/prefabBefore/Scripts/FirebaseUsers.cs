using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Globalization;

using UnityEngine.UI;

public class User
{
    public string UserName, UserEmail, UserID;
    public Vector3 coordinate;
    public User()
    {
    }

    public User(string UserName, string UserEmail, string UserID, Vector3 coordinate)
    {
        this.UserName = UserName;
        this.UserEmail = UserEmail;
        this.UserID = UserID;
        this.coordinate = coordinate;
    }
}

public class FirebaseUsers : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseUser user;
    public static List<GameObject> UsersExist = new List<GameObject>();
    public GameObject prefab;
    //string Username, email, uid;
    System.Uri photo_url;
    DatabaseReference RootReference;
    public static int counterU;
   // public static DatabaseReference UsersRef;

    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://shareardubidu.firebaseio.com/");
        UsersExist.Clear();
        RootReference = FirebaseDatabase.DefaultInstance.RootReference;
        InitializeFirebase();
        Init_Instantiate();
        RootReference.Child("Users").ChildAdded += HandleUserAdded;
        RootReference.Child("Users").ChildChanged += HandleUserChanged;
    }

    void WriteUserInBD(string Username, string email, string uid)
    {
            User new_user = new User(Username, email, uid, altitudeText.locationVec);
            string json = JsonUtility.ToJson(new_user);
            RootReference.Child("Users").Child(new_user.UserID).SetRawJsonValueAsync(json);
            print("write in BD succes!");

    }
    void DeleteUserInBD(string uid)
    {
        RootReference.Child("Users").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                RootReference.Child("Users").Child(uid).RemoveValueAsync();
                print("Remove from BD succes!");

            }
        });

    }

    // Handle initialization of the necessary firebase modules:

    void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void Init_Instantiate()
    {
        RootReference.Child("Users").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int totalNum = (int)snapshot.ChildrenCount;
                for (int i = 0; i < totalNum; i++)
                {
                    string UserId = snapshot.Child(i.ToString()).Child("UserID").Value.ToString();
                    string UserName = snapshot.Child("UserName").Value.ToString();
                    float x = float.Parse(snapshot.Child(i.ToString()).Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float y = float.Parse(snapshot.Child(i.ToString()).Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    float z = float.Parse(snapshot.Child(i.ToString()).Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    Vector3 coordi1 = new Vector3(x, y, z);

                    Vector3 relatifPos = CommonVariables.ConvertGPS2ARCoordinate(altitudeText.location, coordi1);
                    if (UserId != Auth.UserSelfId)
                    {
                        print(UserId + "not identical" + Auth.UserSelfId);
                        GameObject u = Instantiate(prefab, relatifPos, transform.rotation);
                        UsersExist.Add(u);
                        u.name = UserId;
                        u.tag = "User";
                        GPSdata GPSdataScript = u.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
                        GPSdataScript.place_name = UserId;
                        GPSdataScript.longitude = coordi1.x;
                        GPSdataScript.latitude = coordi1.z;
                        GPSdataScript.height = coordi1.y;
                        GPSdataScript.GPSInfo = true;

                        counterU++;
                    }
                    


                    //if(FirebaseScript.ready)FirebaseScript.CounterObj.GetComponent<Text>().text = "There are " + FirebaseScript.counter2.ToString() + "Xmas trees and "+counterU+" Users next to you!";
                }

            }
        }

           );
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                //GameObject signOutUser = GameObject.Find(user.UserId);
                //UsersExist.Remove(signOutUser);
                //Destroy(signOutUser);
                //DeleteUserInBD(user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                WriteUserInBD(user.DisplayName, user.Email, user.UserId);
                //photo_url = user.PhotoUrl;

            }
        }
    }

    void HandleUserAdded(object sender, ChildChangedEventArgs args)
    {
        //print("user is added");
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        RootReference.Child("Users").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                string uid = args.Snapshot.Key;
                
                string UserName = args.Snapshot.Child("UserName").Value.ToString();

                float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
                Vector3 coordi1 = new Vector3(x, y, z);


                Vector3 relatifPos = CommonVariables.ConvertGPS2ARCoordinate(altitudeText.location, coordi1)+new Vector3(0.5f,0f,-0.5f);
                if (uid != Auth.UserSelfId)
                {
                    GameObject AddedUser = Instantiate(prefab, relatifPos, transform.rotation);
                    UsersExist.Add(AddedUser);


                    AddedUser.name = uid;
                    AddedUser.tag = "User";
                    GPSdata GPSdataScript = AddedUser.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
                    GPSdataScript.place_name = uid;
                    GPSdataScript.longitude = x;
                    GPSdataScript.latitude = z;
                    GPSdataScript.height = y;
                    GPSdataScript.GPSInfo = true;


                    print("user is added " + uid);
                    counterU++;
                }
                //if (FirebaseScript.ready) FirebaseScript.CounterObj.GetComponent<Text>().text = "There are " + FirebaseScript.counter2.ToString() + "Xmas trees and " + counterU + " Users next to you!";
            }


            }

            );
        }

    
    void HandleUserChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        GameObject changedObject = GameObject.Find(args.Snapshot.Key);

        string UserName = args.Snapshot.Child("UserName").Value.ToString();
        float x = float.Parse(args.Snapshot.Child("coordinate").Child("x").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        float y = float.Parse(args.Snapshot.Child("coordinate").Child("y").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        float z = float.Parse(args.Snapshot.Child("coordinate").Child("z").Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
        GPSdata GPSdataScript = changedObject.GetComponentInChildren<Transform>().gameObject.GetComponentInChildren<GPSdata>();
        GPSdataScript.longitude = x;
        GPSdataScript.latitude = z;
        GPSdataScript.height = y;
        print(changedObject.name.ToString() + "changed!");
        //ChangeObj.GetComponent<Text>().text = changedObject.name.ToString()+ "changed!";
        //CounterObj.GetComponent<Text>().text = changedObject.name.ToString() + "changed!";
        print("user's location should be changed!");
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
