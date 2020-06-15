using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Auth : MonoBehaviour
{
    FirebaseAuth auth;
    string email;
    string password;
    public static string UserSelfId;

    Coroutine SignInThread;
    Text output;
    private void Awake()
    {
        GameObject.Find("Version").GetComponent<Text>().text = "Version: "+Application.version;
    }
    public void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        UserSelfId = "parDefaut";
        output = GameObject.Find("Output").GetComponent<Text>();
    }

    public void CreateAccount()
    {
        email = GameObject.Find("Email").GetComponent<InputField>().text;
        password = GameObject.Find("Password").GetComponent<InputField>().text;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                output.text = "CreateUserWithEmailAndPasswordAsync was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                output.text = "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            output.text = "user created successfully";
        });
    }
    public void onSignInButtonClicked()
    {
        SignInThread = StartCoroutine("SignInAsync");
    }
    private IEnumerator SignInAsync()
    {
        //for simplifying program debuggin
        if (GameObject.Find("Email").GetComponent<InputField>().text.Length==0)
        {
            UserSelfId = "DefaultID";
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MapView");
            output.text = "Sign in successfully, loading the map...";
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            email = GameObject.Find("Email").GetComponent<InputField>().text;
            print("email.Length"+email.Length);
            password = GameObject.Find("Password").GetComponent<InputField>().text;
            var signInTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => signInTask.IsCompleted);

            if (signInTask.Exception != null)
            {
                Debug.LogWarning(message: $"failed to sign in task with{signInTask.Exception}");
                output.text = "failed to sign in, please be sure that your password and email are correct";
            }
            else
            {
                FirebaseUser newUser = signInTask.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                                newUser.DisplayName, newUser.UserId);
                UserSelfId = newUser.UserId;
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MapView");
                output.text = "Sign in successfully, loading the map...";
                // Wait until the asynchronous scene fully loads
                while (!asyncLoad.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        

    }

        
}




