using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.Events;

public class TimeControlManager : MonoBehaviour
{
    [Header("TMP Inputs")]
    public TMP_Text DayText;
    public TMP_Text Day2Text;
    public TMP_Text friestext;
    public TMP_Text explanationtext;
     [Header("Float Inputs")]
    public float fries=0;
    public float FriesAmount=1;
    public  int LastDay,CurrentDay;
    public DateTime LastTime;
    [SerializeField]
    public int addHoursValue,addMinuteValue,addDayValue;
    private string url = "https://worldtimeapi.org/api/timezone/America/New_York";

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
                CurrentDay = data.day_of_year+addDayValue;
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
        LastTime= LastTime.AddHours(addHoursValue);
        LastTime= LastTime.AddMinutes(addMinuteValue);
        TimeSpan difference = endOfDay - LastTime;
        if (difference.TotalSeconds < 0)
        {
            Debug.Log("The day has ended!");
            explanationtext.text = "The day has ended...";
            if(addHoursValue!=0||addMinuteValue!=0)
            {
                addDayValue+=1;
                addHoursValue=0;
                addMinuteValue=0;
            }
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


