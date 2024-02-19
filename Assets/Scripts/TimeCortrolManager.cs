using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.Events;

public class TimeControlManager : MonoBehaviour
{
    public TMP_Text textMeshPro, textMesh,friestext;
    public float fries=0;
    private string url = "https://worldtimeapi.org/api/ip";
    public  DateTime LastTime,CurrentTime;

    private const string LastTimeKey = "LastTime";
    private const string FriesKey = "Fries";
    void Start()
    {
       if (PlayerPrefs.HasKey(LastTimeKey))
        {
            LastTime = DateTime.Parse(PlayerPrefs.GetString(LastTimeKey));
        }

        fries = PlayerPrefs.GetFloat(FriesKey, 0f);
        friestext.text="fries :"+fries;
        StartCoroutine(GetDatas());
    }

    public void chechTimeEvent()
    {
        StartCoroutine(GetDatas());
        checktime();
    }
    IEnumerator GetDatas()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Data extraction error: " + www.error);
            }
            else
            {
                string jsonString = www.downloadHandler.text;
                Data data = JsonUtility.FromJson<Data>(jsonString);
                CurrentTime = DateTime.Parse(data.datetime);
                textMeshPro.text =  ""+CurrentTime;
                textMesh.text=""+LastTime;
            }
        }
    }
    public void checktime(){
        if(CurrentTime>LastTime.AddHours(24)|| LastTime==DateTime.MinValue){
            LastTime=CurrentTime;
            GiveReward();
            PlayerPrefs.SetString(LastTimeKey, LastTime.ToString());
            PlayerPrefs.SetFloat(FriesKey, fries);
        }
        else{
            Debug.Log("lasttime: " + LastTime);
        }
    }
    private void GiveReward()
        {
            fries += 1;
            friestext.text="fries :"+fries;
        }

}


    [System.Serializable]
    public class Data
    {
        public string datetime;
    }


