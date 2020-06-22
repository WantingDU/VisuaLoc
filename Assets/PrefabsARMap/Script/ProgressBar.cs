using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置UI上image的进度条
/// </summary>
public class ProgressBar : MonoBehaviour
{
    private bool isInit = false;
    public static Image progressBar;
    public void Awake()
    {
        progressBar = transform.GetComponent<Image>();
        progressBar.type = Image.Type.Filled;
        progressBar.fillMethod = Image.FillMethod.Radial360;
        //progressBar.fillMethod = Image.FillMethod.Horizontal;
        //progressBar.fillOrigin = 0;
    }

    public static void SetProgressValue(float value)
    {
        if (progressBar != null)
        {
            if (value < 1)
            {
                progressBar.fillAmount = value;
                progressBar.color = Color.gray;
            }
            else
            {
                progressBar.fillAmount = 1;
                progressBar.color = new Color(118, 192, 117);
            }
        }
        else
        {
            print("no progressbar");
        }
    }
}
