using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum POST_API
{
    game_end,
    game_start
}

public enum GET_API
{
    info,
    bet,
    claim
}

[RequireComponent(typeof(GameManager))]
public class ApiManager : MonoBehaviour
{

    public static ApiManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private string urlBase = "https://script.google.com/macros/s/AKfycbxHXrz_jqsY_3kmNMj6Fb0NhkjhDZIau0-BDGIFbJwjqojksxA/exec";

    //public void GameStart(Action<object> callback = null)
    //{
    //    byte[] bytePostData = null;
    //    if (dataAccess.pad._id != null && dataAccess.pad._id != string.Empty)
    //    {
    //        ReqGameStart paramStart = new ReqGameStart();
    //        paramStart.padId = dataAccess.pad._id;
    //        bytePostData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(paramStart));
    //    }
    //    UnityWebRequest webRequest = UnityWebRequest.Put($"{urlBase}/game/start", bytePostData);
    //    webRequest.method = "POST";
    //    webRequest.SetRequestHeader("Content-Type", "application/json");
    //    webRequest.SetRequestHeader("x-access-token", dataAccess.accessToken);
    //    webRequest.SetRequestHeader("x-api-key", dataAccess.apiKey);
    //    StartCoroutine(Post(POST_API.game_start, webRequest, callback));
    //}

    //public void GameEnd(int score, Action<object> callback = null)
    //{

    //    ReqGameEnd paramScore = new ReqGameEnd();
    //    paramScore.score = score;
    //    byte[] bytePostData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(paramScore));
    //    UnityWebRequest webRequest = UnityWebRequest.Put($"{urlBase}/game/end", bytePostData);
    //    webRequest.method = "POST";
    //    webRequest.SetRequestHeader("Content-Type", "application/json");
    //    webRequest.SetRequestHeader("x-access-token", dataAccess.accessToken);
    //    webRequest.SetRequestHeader("x-api-key", dataAccess.apiKey);
    //    StartCoroutine(Post(POST_API.game_end, webRequest, callback));
    //}

    private IEnumerator Post(POST_API api, UnityWebRequest webRequest, Action<object> callback = null)
    {
        Debug.Log(api + " ==== " + webRequest.url);
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(": Error: " + webRequest.error);
        }
        else
        {
            switch (api)
            {
                case POST_API.game_end:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    if (callback != null)
                    {
                        callback(webRequest.downloadHandler.text);
                    }
                    break;
                case POST_API.game_start:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    ResGameStart resStart = JsonUtility.FromJson<ResGameStart>(webRequest.downloadHandler.text);
                    if (callback != null)
                    {
                        callback(resStart.data);
                    }
                    break;
                default:
                    break;
            }

        }
    }

    public void GetInfo(Action<object> callback = null)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{urlBase}?api={GET_API.info.ToString()}&authToken={ReactBridgePlugin.instance.GetAuth().authToken}");
        webRequest.method = "GET";
        StartCoroutine(Get(GET_API.info, webRequest, callback));
    }

    public void GetResult(double betAmount, Action<object> callback = null)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{urlBase}?api={GET_API.bet.ToString()}&authToken={ReactBridgePlugin.instance.GetAuth().authToken}&betAmount={betAmount}");
        webRequest.method = "GET";
        StartCoroutine(Get(GET_API.bet, webRequest, callback));
    }

    public void GetClaim(Action<object> callback = null)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{urlBase}?api={GET_API.claim.ToString()}&authToken={ReactBridgePlugin.instance.GetAuth().authToken}");
        webRequest.method = "GET";
        StartCoroutine(Get(GET_API.claim, webRequest, callback));
    }

    private IEnumerator Get(GET_API api, UnityWebRequest webRequest, Action<object> callback = null)
    {
        Debug.Log(api + " ==== " + webRequest.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(": Error: " + webRequest.error);
        }
        else
        {
            Debug.Log("Received: " + webRequest.downloadHandler.text);
            switch (api)
            {
                case GET_API.info:
                    ResInfo resInfo = JsonUtility.FromJson<ResInfo>(webRequest.downloadHandler.text);
                    if (callback != null)
                    {
                        callback(resInfo);
                    }
                    break;
                case GET_API.bet:
                    ResResult resResult = JsonUtility.FromJson<ResResult>(webRequest.downloadHandler.text);
                    if (callback != null)
                    {
                        callback(resResult);
                    }
                    break;
                case GET_API.claim:
                    ResClaim resClaim = JsonUtility.FromJson<ResClaim>(webRequest.downloadHandler.text);
                    if (callback != null)
                    {
                        callback(resClaim);
                    }
                    break;
                default:
                    break;
            }

        }
    }
}

[Serializable]
public class ResClaim
{
    public int code;
    public string data;
}

[Serializable]
public class ResInfo 
{
    public double credit;
    public double collar;
}

[Serializable]
public class ResResult
{
    public int code;
    public DataGame data;
    public string msg;
}

[Serializable]
public class DataGame
{
    public PAYOUT_TYPE payout;
    public double reward;
    public double collar;
    public System.Collections.Generic.List<CardData> cards;
    public double credit;
}

[Serializable]
public class ResGameStart
{
    public int code;
    public DataPlayGame data;
    public string msg;
}

[Serializable]
public class DataPlayGame
{
    public DataGameInfo gameInfo;
    public DataUserInfo userInfo;
    public DataProject project;

    [Serializable]
    public class DataGameInfo
    {
        public string turnId;
        public string padId;
        public double creditSpend;
        public string gameType;
    }
}

[Serializable]
public class DataUserInfo
{
    public double creditBalance;
    public double freeCreditBalance;
    public double tokenBalance;
    public string userId;
    public string address;
}

[Serializable]
public class ResGameEnd
{
    public int code;
    public DataReceiveScore data;
    public string msg;
}

[Serializable]
public class DataReceiveScore
{
    public DataGameInfo gameInfo;
    public DataUserInfo userInfo;
    public DataTicketInfo ticketInfo;
    public DataProject project;

    [Serializable]
    public class DataGameInfo
    {
        public string turnId;
        public string padId;
        public int score;
        public long playtime;
        public double creditSpend;
        public double tokenReward;
        public string gameType;
    }
}

[Serializable]
public class DataTicketInfo
{
    public int gold;
    public int sliver;
    public int bronze;
}

[Serializable]
public class DataProject
{
    public string projectId;
    public int creditType;
    public int creditRate;
    public float earnRate;
}

[Serializable]
public class ReqGameEnd
{
    public int score;
}

[Serializable]
public class ReqGameStart
{
    public string padId;
}

[Serializable]
public class DataAccess
{
    public string accessToken;
    public string apiKey;
    public DataPad pad;
}

[Serializable]
public class DataPad
{
    public string _id;
    public string name;
    public int padClass;
    public int classBaseMint;
    public int rarity;
    public int rarityValue;
    public int power;
    public int luck;
    public int durability;
    public int level;
    public string imgUrl;
    public string userId;
    public string createdAt;
    public string updatedAt;
}