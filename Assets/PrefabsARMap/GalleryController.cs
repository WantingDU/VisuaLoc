using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GalleryController : MonoBehaviour
{
    public static CanvasGroup UICG;
    private void Awake()
    {
        UICG = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
    }
    
    public void onClickPanelMedia()
    {
        if(NativeGallery.IsMediaPickerBusy())

            return;
        else
        {
            // Pick a PNG image from Gallery/Photos
            // If the selected image's width and/or height is greater than 512px, down-scale the image
            PickImageForPanel(512);
        }
    }

    public void onScreenshot()
	{
		StartCoroutine(TakeScreenshotAndSave());
	}
    public static void onScreenshotMap()
    {
        StaticCoroutine.DoCoroutine(TakeScreenshot());
    }
    public void onChooseImage()
	{
		if (NativeGallery.IsMediaPickerBusy())
			return;

		else 
		{
			// Pick a PNG image from Gallery/Photos
			// If the selected image's width and/or height is greater than 512px, down-scale the image
			PickImage(512);
            

		}
		
	}
    public void onChooseVideo()
	{
		if (NativeGallery.IsMediaPickerBusy())
			return;

		else
		{
			// Pick a video from Gallery/Photos
			PickVideo();
		}
	}
    private static IEnumerator TakeScreenshot()
    {
        UICG.alpha = 0;
        UICG.blocksRaycasts = false;
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        ss = StaticObject.duplicateTexture(ss);
        byte[] mediaImageFile = ImageConversion.EncodeToJPG(ss);
        StaticCoroutine.DoCoroutine(firestore.WriteFile(mediaImageFile, StaticObject.myARmapID+"/ARMapScreenshot.jpg"));
        // Save the screenshot to Gallery/Photos
        //Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png"));
        // To avoid memory leaks
        Destroy(ss);
        UICG.alpha = 1;
        UICG.blocksRaycasts = true;
    }
    private IEnumerator TakeScreenshotAndSave()
	{
        UICG.alpha = 0;
        UICG.blocksRaycasts = false;
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();

		// Save the screenshot to Gallery/Photos
		Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "VisuaLoc", StaticObject.getGUID() + ".png"));
        
		// To avoid memory leaks
		Destroy(ss);
        UICG.alpha = 1;
        UICG.blocksRaycasts = true;
    }
    
    private void PickImageForPanel(int maxSize)
    {
        StaticObject.photoAdded = true;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                /*
                GameObject infoPanelFound = GameObject.Find("InfoPanel");
                Debug.Log("found infopanel");
                GameObject canvasFound = infoPanelFound.transform.Find("Canvas").gameObject;
                Debug.Log("found Canvas");
                GameObject MediaPanel = canvasFound.transform.Find("Media").gameObject;
                Debug.Log("found Media");
                */
                GameObject MediaPanel = GameObject.Find("InfoPanel/Canvas/Media");
                MediaPanel.GetComponent<CanvasRenderer>().SetTexture(texture);
                Debug.Log("set texture !");
                MediaPanel.GetComponent<CanvasRenderer>().GetMaterial().mainTexture = texture;
                Debug.Log("just set maintexture!"+ MediaPanel.GetComponent<CanvasRenderer>().GetMaterial().mainTexture.height);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }



	private void PickImage(int maxSize)
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				// Assign texture to a temporary quad and destroy it after 5 seconds
				GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
				quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
				quad.transform.forward = Camera.main.transform.forward;
				quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

				Material material = quad.GetComponent<Renderer>().material;
				if (!material.shader.isSupported) // happens when Standard shader is not included in the build
					material.shader = Shader.Find("Legacy Shaders/Diffuse");

				material.mainTexture = texture;

				//Destroy(quad, 5f);

				// If a procedural texture is not destroyed manually, 
				// it will only be freed after a scene change
				//Destroy(texture, 5f);
			}
		}, "Select a PNG image", "image/png");

		Debug.Log("Permission result: " + permission);
	}

	private void PickVideo()
	{
		NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
		{
			Debug.Log("Video path: " + path);
			if (path != null)
			{
                Debug.Log(NativeGallery.GetVideoProperties(path).rotation);
                GameObject myCarier = GameObject.CreatePrimitive(PrimitiveType.Cube);
                myCarier.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
 
                myCarier.transform.forward = -Camera.main.transform.forward;
                //myCarier.transform.localScale = new Vector3(1f, 1.2f, 1f);
                myCarier.transform.localScale=new Vector3(1f, (float)NativeGallery.GetVideoProperties(path).height / (float)NativeGallery.GetVideoProperties(path).width, 1f);
                Material material = myCarier.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                AudioSource audioSouce = myCarier.AddComponent<AudioSource>();
                VideoPlayer videoPlayer = myCarier.AddComponent<VideoPlayer>();
                videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
                videoPlayer.targetMaterialRenderer = myCarier.GetComponent<Renderer>();
                videoPlayer.targetMaterialProperty = "_MainTex";
                videoPlayer.url = path;
                videoPlayer.source = VideoSource.Url;
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                videoPlayer.EnableAudioTrack(0, true);
                videoPlayer.SetTargetAudioSource(0, audioSouce);
               
                //Output the current clip's length
                videoPlayer.Play();
                audioSouce.Play();
                //Debug.Log("Audio clip length : " + audioSouce.clip.length);
                Debug.Log("hey there is my volume of audio " + audioSouce.volume);
            }
		}, "Select a video");

		Debug.Log("Permission result: " + permission);
	}
}
