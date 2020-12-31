using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class BaiduAI : MonoBehaviour
{
    public string url= "https://aip.baidubce.com/rpc/2.0/ai_custom/v1/detection/ImageDect";
    public string accessToken;
    public string imagePath=Application.streamingAssetsPath+"/Image.jpg";
    public string resString;
    public string clientId= "DIxTqqm7GlqUwkri0INwD1zn";
    public string clientSecret= "51bqd29eHqNOiTWuAaVdkzn5oWNFbBra";

    Camera main;
    LineRenderer r;

    private Texture2D screenShot;
    private WebRequest webRequest;
    public Image image1;

    string pathJson
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, "my_Image_Json.txt");
#elif UNITY_IOS
            return Path.Combine(Application.persistentDataPath, "my_Image_Json.txt");
#endif
        }
    }

    string pathImage
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, "my_Image.jpg");
#elif UNITY_IOS
            return Path.Combine(Application.persistentDataPath, "my_Image.jpg");
#endif
        }
    }

    private void Start()
    {
        main = this.GetComponent<Camera>();
        r = this.GetComponent<LineRenderer>();
        screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        //string urlForToken = "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=wBf3aGDCPcvX2kH89FeOOZgy&client_secret=sO2kBrGy2cLRLr9KHqtcHYfYWGZs6gHZ&";

        string authHost = "https://aip.baidubce.com/oauth/2.0/token";
        HttpClient client = new HttpClient();
        List<KeyValuePair<string, string>> paraList = new List<KeyValuePair<string, string>>();
        paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
        paraList.Add(new KeyValuePair<string, string>("client_id", clientId));
        paraList.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
        HttpContent content = new FormUrlEncodedContent(paraList);
        HttpResponseMessage tokenResponse = client.PostAsync(authHost, content).Result;

        accessToken = tokenResponse.Content.ReadAsStringAsync().Result;

        var tempArray = accessToken.Split(',');
        tempArray = tempArray[3].Split(':');
        accessToken = "[" + tempArray[1].Substring(1, tempArray[1].Length - 2) + "]";
        

        url = url + "?access_token=" + accessToken;
        Debug.Log(url);

        //#region 测试
        //webRequest = (HttpWebRequest)WebRequest.Create(url);
        //webRequest.Method = "POST";
        //webRequest.ContentType = "application/json;charset=utf-8";

        ////image1.rectTransform.rect.Set(928, 629, 1303, 1069);

        //FileStream fileStream = File.Open(imagePath, FileMode.Open);
        //int fileLength = (int)fileStream.Length;

        //byte[] image = new byte[fileLength];
        //fileStream.Read(image, 0, fileLength);

        //var detail = System.Convert.ToBase64String(image);

        //string str = $"{{\"image\":\"{detail}\",\"scenes\":[\"animal\",\"plant\",\"ingredient\",\"dishes\", \"red_wine\",\"currency\",\"landmark\"]}}";
        //byte[] buffer = Encoding.Default.GetBytes(str);
        //webRequest.ContentLength = buffer.Length;
        //webRequest.GetRequestStream().Write(buffer, 0, buffer.Length);

        //using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
        //{
        //    Stream stream = response.GetResponseStream();
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        resString = reader.ReadToEnd().ToString();
        //    }
        //}

        //JsonReader jsonReader = new JsonReader(resString);
        //JsonData res = JsonMapper.ToObject(jsonReader);
        //Debug.Log(resString);
        //res = res["results"];
        
        //res = res[0];
        //res = res["location"];
        //Rect rect = new Rect(new Vector2(int.Parse(res["left"].ToString()), int.Parse(res["top"].ToString())), new Vector2(int.Parse(res["width"].ToString()), int.Parse(res["height"].ToString())));

        ////Debug.Log(rect.ToString());

        ////Vector3 top_Left = main.ScreenToWorldPoint(new Vector3(rect.position.x, rect.position.y, 0));
        ////Vector3 top_Right = main.ScreenToWorldPoint(new Vector3(rect.position.x + rect.width, rect.position.y, 0));
        ////Vector3 bottom_Right = main.ScreenToWorldPoint(new Vector3(rect.position.x + rect.width, rect.position.y - rect.height, 0));
        ////Vector3 bottom_Left = main.ScreenToWorldPoint(new Vector3(rect.position.x, rect.position.y - rect.height, 0));
        ////Debug.Log(top_Left);
        ////r.SetPositions(new Vector3[] { top_Left, top_Right, bottom_Right, bottom_Left, top_Left });

        //#endregion
    }

    private void OnPostRender()
    {
        screenShot.ReadPixels(new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height)), 0, 0);
        screenShot.Apply();
    }

    private void Update()
    {
        byte[] image = screenShot.EncodeToJPG();
        using (FileStream file = new FileStream(pathImage, FileMode.OpenOrCreate))
        {
            file.Write(image, 0, image.Length);
            file.Flush();
        }
        var detail = System.Convert.ToBase64String(image);
        string str = $"{{\"image\":\"{detail}\",\"scenes\":[\"animal\",\"plant\",\"ingredient\",\"dishes\", \"red_wine\",\"currency\",\"landmark\"]}}";
        using (FileStream file = new FileStream(pathJson, FileMode.OpenOrCreate))
        {
            file.Write(System.Text.Encoding.Default.GetBytes(str), 0, System.Text.Encoding.Default.GetBytes(str).Length);
            file.Flush();
        }

        byte[] buffer = Encoding.Default.GetBytes(str);

        webRequest = (HttpWebRequest)WebRequest.Create(url);
        webRequest.Method = "POST";
        webRequest.ContentType = "application/json;charset=utf-8";
        webRequest.ContentLength = buffer.Length;

        using (Stream reqStream = webRequest.GetRequestStream())
        {
            reqStream.Write(buffer, 0, buffer.Length);
        }

        using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
        {
            Stream stream = response.GetResponseStream();
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                resString = reader.ReadToEnd().ToString();
            }
        }

        JsonReader jsonReader = new JsonReader(resString);
        JsonData res = JsonMapper.ToObject(jsonReader);
        Debug.Log(resString);
        res = res["results"];
        if (res.Count == 0)
        {
            return;
        }
        res = res[0];
        res = res["location"];

        Rect rect = new Rect(new Vector2(int.Parse(res["left"].ToString()), int.Parse(res["top"].ToString())), new Vector2(int.Parse(res["width"].ToString()), int.Parse(res["height"].ToString())));

        //Vector3 top_Left = main.ScreenToWorldPoint(new Vector3(rect.position.x, rect.position.y, 10));
        //Vector3 top_Right = main.ScreenToWorldPoint(new Vector3(rect.position.x + rect.width, rect.position.y, 10));
        //Vector3 bottom_Right = main.ScreenToWorldPoint(new Vector3(rect.position.x + rect.width, rect.position.y - rect.height, 10));
        //Vector3 bottom_Left = main.ScreenToWorldPoint(new Vector3(rect.position.x, rect.position.y - rect.height, 10));
        //r.SetPositions(new Vector3[] { top_Left, top_Right, bottom_Right, bottom_Left, top_Left });

        //r.SetPosition(0, top_Left);
        //r.SetPosition(1, top_Right);
        //r.SetPosition(2, bottom_Right);
        //r.SetPosition(3, bottom_Left);
        //r.SetPosition(4, top_Left);
        image1.rectTransform.offsetMax = new Vector2(rect.x + rect.width, -rect.y);
        image1.rectTransform.offsetMin = new Vector2(res["left"].ValueAsInt(), -res["top"].ValueAsInt() - res["height"].ValueAsInt());

    }

    private void Draw(Rect rect)
    {
        GL.PushMatrix();
        Color color = Color.green;
        color.a = 0.1f;
        GL.LoadPixelMatrix();

        GL.Begin(GL.QUADS);
        GL.Color(color);
        GL.Vertex3(rect.position.x,rect.position.y,0);
        GL.Vertex3(rect.position.x + rect.width, rect.position.y, 0);
        GL.Vertex3(rect.position.x + rect.width, rect.position.y - rect.height,0);
        GL.Vertex3(rect.position.x, rect.position.y - rect.height, 0);
    }
}
