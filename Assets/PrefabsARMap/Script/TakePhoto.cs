using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TakePhoto : MonoBehaviour
{
    WebCamTexture webCamTexture;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webCamTexture; //Add Mesh Renderer to the GameObject to which this script is attached to
        webCamTexture.Play();
    }
    public void startTake()
    {
        StartCoroutine("take");
    }

    private IEnumerator take()  // Start this Coroutine on some button click
    {

        // NOTE - you almost certainly have to do this here:
        yield return new WaitForEndOfFrame();



        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();
        NativeGallery.SaveImageToGallery(photo, "VisuaLoc", StaticObject.getGUID()+".png");

        //Encode to a PNG
        //byte[] bytes = photo.EncodeToPNG();
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        //File.WriteAllBytes(path + "photo.png", bytes);
    }
}