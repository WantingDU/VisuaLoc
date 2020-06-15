using　System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class StaticMap : MonoBehaviour
{
    //Google Maps Static API URL
    private const string STATIC_MAP_URL = "https://maps.googleapis.com/maps/api/staticmap?key=AIzaSyAb53a8Q9yhukg2JtaYwbQZxmP9wQKwUls&zoom=15&size=640x640&scale=12&maptype=terrain&style=element:labels|visibility:off";
    //private const string STATIC_MAP_URL = "https://maps.googleapis.com/maps/api/staticmap?center=Brooklyn+Bridge,New+York,NY&zoom=13&size=600x300&maptype=roadmap&markers=color:blue%7Clabel:S%7C40.702147,-74.015794&markers=color:green%7Clabel:G%7C40.711614,-74.012318&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=AIzaSyAb53a8Q9yhukg2JtaYwbQZxmP9wQKwUls";
    private int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 非同期処理
        Input.location.Start();
        StartCoroutine(getStaticMap());
    }

    // Update is called once per frame
    void Update()
    {
        // 5秒に一度の実行
        if (frame >= 300)
        {
            StartCoroutine(getStaticMap());
            frame = 0;
        }
        frame++;
    }

    IEnumerator getStaticMap()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                var query = "";

                // centerで取得するミニマップの中央座標を設定
                query += "&center=" + UnityWebRequest.UnEscapeURL(string.Format("{0},{1}", Input.location.lastData.latitude, Input.location.lastData.longitude));
                // markersで渡した座標(=現在位置)にマーカーを立てる
                query += "&markers=" + UnityWebRequest.UnEscapeURL(string.Format("{0},{1}", Input.location.lastData.latitude, Input.location.lastData.longitude));

                // リクエストの定義
                var req = UnityWebRequestTexture.GetTexture(STATIC_MAP_URL + query);
                // リクエスト実行
                yield return req.SendWebRequest();

                if (req.error == null)
                {
                    // すでに表示しているマップを更新
                    
                    Texture2D tex=((DownloadHandlerTexture)req.downloadHandler).texture;
                    GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    print("material changed!!!");
                }
                else
                {
                    print(req.error);
                    print("error!!!");
                }
            }
        }
    }
}