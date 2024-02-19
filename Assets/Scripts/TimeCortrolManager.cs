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
    private string url = "https://worldtimeapi.org/api/timezone/America/New_York";
    public  int LastDay,CurrentDay;
    public DateTime LastTime;

    private const string LastDayKey = "LastDay";
    private const string FriesKey = "Fries";
    private const string LastTimeKey = "LastTime";
    void Start()
    {
         if (PlayerPrefs.HasKey(LastTimeKey))
        {
            LastTime = DateTime.Parse(PlayerPrefs.GetString(LastTimeKey));
        }
        LastDay = PlayerPrefs.GetInt(LastDayKey,0);
        fries = PlayerPrefs.GetFloat(FriesKey, 0f);

        friestext.text="Fries :"+fries;
        Debug.Log("lastDay: " + LastDay);
        StartCoroutine(GetDatas());
        Debug.Log("lastDay: " + LastDay);
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
                LastTime=DateTime.Parse(data.datetime);

                DayText.text = "day of year :"+CurrentDay;
                Day2Text.text="Last Day"+LastDay;
            }
        }
    }
    public void checktime(){
        if(CurrentDay>LastDay){
            LastDay=CurrentDay;
            GiveReward();
            PlayerPrefs.SetInt(LastDayKey, LastDay);
            PlayerPrefs.SetFloat(FriesKey, fries);
            PlayerPrefs.SetString(LastTimeKey, LastTime.ToString());
        }
        else{
            //explanationtext.text = "Are you sure you didn't eat fries yesterday? :)"; 
            friestext.text="Fries :"+fries;
            CalculateHoursLeft();
            
        }
    }
    private void GiveReward()
        {
            fries += FriesAmount;
            friestext.text="More Fries :"+fries;
        }
    private void CalculateHoursLeft(){
        DateTime endOfDay  = new DateTime(LastTime.Year, LastTime.Month, LastTime.Day, 23, 59, 59);
        TimeSpan difference = endOfDay - LastTime;
        if (difference.TotalSeconds < 0)
        {
            Debug.Log("The day has ended!");
        }
        else
        {
            int remainingHours = (int)difference.TotalHours;
            int remainingMinutes = (int)difference.TotalMinutes % 60;
            int remainingSeconds = (int)difference.TotalSeconds % 60;
            explanationtext.text="Remaining time until the end of the day: " + remainingHours + " hours, " + remainingMinutes + " minutes, " + remainingSeconds + " seconds";
            Debug.Log("Remaining time until the end of the day: " + remainingHours + " hours, " + remainingMinutes + " minutes, " + remainingSeconds + " seconds");
        }

    }

}


    [System.Serializable]
    public class Data
    {
        public int day_of_year;
        public string datetime;
    }


