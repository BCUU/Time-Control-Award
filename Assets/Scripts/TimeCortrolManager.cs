using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.Events;

public class TimeControlManager : MonoBehaviour
{
    public TMP_Text DayText, Day2Text,friestext,explanationtext;
    public float fries=0;
    public float FriesAmount=1;
    private string url = "https://worldtimeapi.org/api/ip";
    public  int LastDay,CurrentDay;

    private const string LastTimeKey = "LastTime";
    private const string FriesKey = "Fries";
    void Start()
    {
        LastDay = PlayerPrefs.GetInt(LastTimeKey,0);
        fries = PlayerPrefs.GetFloat(FriesKey, 0f);
        friestext.text="Fries :"+fries;
        Debug.Log("lasttime: " + LastDay);
        StartCoroutine(GetDatas());
        Debug.Log("lasttime: " + LastDay);
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
                CurrentDay = data.day_of_year;
                DayText.text = "day of year :"+CurrentDay;
                Day2Text.text="Last Day"+LastDay;
            }
        }
    }
    public void checktime(){
        if(CurrentDay>LastDay){
            LastDay=CurrentDay;
            GiveReward();
            PlayerPrefs.SetInt(LastTimeKey, LastDay);
            PlayerPrefs.SetFloat(FriesKey, fries);
        }
        else{
            explanationtext.text = "Are you sure you didn't eat fries yesterday? :)"; 
            friestext.text="Fries :"+fries;
            
        }
    }
    private void GiveReward()
        {
            fries += FriesAmount;
            friestext.text="More Fries :"+fries;
        }

}


    [System.Serializable]
    public class Data
    {
        public int day_of_year;
    }


