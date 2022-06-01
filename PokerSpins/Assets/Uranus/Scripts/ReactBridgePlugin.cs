using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ReactBridgePlugin : MonoBehaviour
{
    public static ReactBridgePlugin instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    [DllImport("__Internal")]
    private static extern void ReadyToPlay();
    [DllImport("__Internal")]
    private static extern void EndgameResult(string dataEndGame);

    DataAuth dataAuth;

    // Start is called before the first frame update
    void Start()
    {
        ReadyToPlay();
        //DataAuth temp = new DataAuth();
        //temp.authToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2MjgzMThhYjQ1NjA3MzI5YTVjZTk1MGYiLCJhZGRyZXNzIjoiMHg2NDFkQ2E4ODQzRDQ3NDYyZkJlODMxMzVFNGNmMDVhMmU3ZmM4YjYwIiwiaWF0IjoxNjU0MDU3ODMxLCJleHAiOjE2NTQxNDQyMzF9.nNSbjMFp8TA-N7k8VpfyAljtkti_RO1VcdgTCu9VaLM";
        //SetAuth(JsonUtility.ToJson(temp));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndgameResult(DataEndGame data)
    {
        EndgameResult(JsonUtility.ToJson(data));
    }

    public void SetAuth(string authJson)
    {
        dataAuth = JsonUtility.FromJson<DataAuth>(authJson);
    }

    public DataAuth GetAuth()
    {
        return dataAuth;
    }
}

[Serializable]
public class DataEndGame
{
    public int score;
    public DataEndGame(int score)
    {
        this.score = score;
    }
}

[Serializable]
public class DataAuth
{
    public string authToken;
}