using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using UnityEditor;

[System.Serializable]
internal class GamObjectInfomation
{
    public string gamObjectName;
    //public string prefabName;
    public Vector3 gameObjectPosition;
    public Quaternion gamObjectRotation;
    public Vector3 gameObjectScale;
}

class QuaternionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion q = (Quaternion)obj;
        info.AddValue("x", q.x);
        info.AddValue("y", q.y);
        info.AddValue("z", q.z);
        info.AddValue("w", q.w);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion q = (Quaternion)obj;
        q.x = info.GetSingle("x");
        q.y = info.GetSingle("y");
        q.z = info.GetSingle("z");
        q.w = info.GetSingle("w");
        obj = q;
        return obj;
    }
}

public sealed class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = info.GetSingle("x");
        v3.y = info.GetSingle("y");
        v3.z = info.GetSingle("z");
        obj = v3;
        return obj;
    }
}

public class ObjSave : MonoBehaviour
{

    //所有添加对象的根
    public GameObject gameObjectRoot;
    internal List<GamObjectInfomation> gameObjects=new List<GamObjectInfomation>();

    public void Save()
    {
        SaveJson();
    }

    public void Load()
    {
        LoadJson();
    }

    #region Json序列化对象
    private void SaveJson()
    {
        FileStream file = new FileStream(pathJson, FileMode.Create);
        for (int i = 0; i < gameObjectRoot.transform.childCount; i++)
        {
            var temp = new GamObjectInfomation();
            var tempObj = gameObjectRoot.transform.GetChild(i);
            temp.gamObjectName = tempObj.name;
            temp.gamObjectName = temp.gamObjectName.IndexOf('(') == -1 ? temp.gamObjectName : temp.gamObjectName.Substring(0, temp.gamObjectName.IndexOf('('));
            temp.gamObjectName = temp.gamObjectName.IndexOf(' ') == -1 ? temp.gamObjectName : temp.gamObjectName.Substring(0, temp.gamObjectName.IndexOf(' '));
#if UNITY_EDITOR
            Debug.Log(temp.gamObjectName);
#endif
            temp.gameObjectPosition = tempObj.position;
            temp.gamObjectRotation = tempObj.rotation;
            temp.gameObjectScale = tempObj.localScale;
            var jsonStr = JsonUtility.ToJson(temp)+"\n";
            file.Write(System.Text.Encoding.Default.GetBytes(jsonStr),0,System.Text.Encoding.Default.GetBytes(jsonStr).Length);
            file.Flush();
        }
        file.Close();
    }

    private void LoadJson()
    {
        var content = File.ReadAllBytes(pathJson);
        var str = System.Text.Encoding.Default.GetString(content);
        var objs = str.Split('\n');
        foreach (var obj in objs)
        {
            GamObjectInfomation temp = JsonUtility.FromJson<GamObjectInfomation>(obj);
            var path = "Model/" + temp.gamObjectName + "/" + temp.gamObjectName;
#if UNITY_EDITOR
            Debug.Log(path);
#endif
            var tempRes = Resources.Load(path) as GameObject;
            var tempObj = GameObjectPool.instance.CreateObject(temp.gamObjectName, tempRes, temp.gameObjectPosition, temp.gamObjectRotation);
            tempObj.transform.position = temp.gameObjectPosition;
            tempObj.transform.rotation = temp.gamObjectRotation;
            tempObj.transform.localScale = temp.gameObjectScale;
        }
    }
    #endregion

    #region C#接口序列化对象
    private void SaveObj()
    {
        for (int i = 0; i < gameObjectRoot.transform.childCount; i++)
        {
            var temp = new GamObjectInfomation();
            var tempObj = gameObjectRoot.transform.GetChild(i);
            temp.gamObjectName = tempObj.name;
            temp.gamObjectName = temp.gamObjectName.IndexOf('(') == -1 ? temp.gamObjectName : temp.gamObjectName.Substring(0, temp.gamObjectName.IndexOf('('));
            temp.gamObjectName = temp.gamObjectName.IndexOf(' ') == -1 ? temp.gamObjectName : temp.gamObjectName.Substring(0, temp.gamObjectName.IndexOf(' '));
#if UNITY_EDITOR
            Debug.Log(temp.gamObjectName);
#endif
            temp.gameObjectPosition = tempObj.position;
            temp.gamObjectRotation = tempObj.rotation;
            temp.gameObjectScale = tempObj.localScale;
            gameObjects.Add(temp);
        }

        SaveBinary();
    }

    private void LoadObj()
    {
        LoadBinary();

        foreach(var obj in gameObjects)
        {
            var path = "Model/" + obj.gamObjectName+"/"+ obj.gamObjectName;
#if UNITY_EDITOR
            Debug.Log(path);
#endif
            var tempRes = Resources.Load(path) as GameObject;
            var tempObj = GameObjectPool.instance.CreateObject(obj.gamObjectName,tempRes, obj.gameObjectPosition, obj.gamObjectRotation);
            tempObj.transform.position = obj.gameObjectPosition;
            tempObj.transform.rotation = obj.gamObjectRotation;
            tempObj.transform.localScale = obj.gameObjectScale;
        }
    }

    void SaveBinary()
    {
        try
        {
            FileStream file = null;
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            var streamingContext = new StreamingContext(StreamingContextStates.All);
            ss.AddSurrogate(typeof(Vector3), streamingContext, new Vector3SerializationSurrogate());
            ss.AddSurrogate(typeof(Quaternion), streamingContext, new QuaternionSerializationSurrogate());
            bf.SurrogateSelector = ss;
            file = File.Open(pathObj, FileMode.Create);
            bf.Serialize(file, gameObjects);
            file.Close();
            print("成功存储");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("存储失败----" + ex.Message);
        }
    }

    void LoadBinary()
    {
        try
        {
            FileStream file = File.Open(pathObj, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            var streamingContext = new StreamingContext(StreamingContextStates.All);
            ss.AddSurrogate(typeof(Vector3), streamingContext, new Vector3SerializationSurrogate());
            ss.AddSurrogate(typeof(Quaternion), streamingContext, new QuaternionSerializationSurrogate());
            bf.SurrogateSelector = ss;
            gameObjects = (List<GamObjectInfomation>)bf.Deserialize(file);
            file.Close();
            print("成功读取");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("读取失败----" + ex.Message);
        }
    }

    #endregion

    public void DestroyObj()
    {
        GameObjectPool.instance.ClaerAll();
    }


    string pathObj
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, "my_session_Obj.worldmap");
#elif UNITY_IOS
            return Path.Combine(Application.persistentDataPath, "my_session_Obj.worldmap");
#endif
        }
    }

    string pathJson
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, "my_session_Json.txt");
#elif UNITY_IOS
            return Path.Combine(Application.persistentDataPath, "my_session_Json.txt");
#endif
        }
    }
}
