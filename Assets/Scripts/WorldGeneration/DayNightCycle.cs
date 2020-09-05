using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public bool isRunning;
    public bool isDay;

    public Color nightColor, dayColor;
    public float dayBrightness, nightBrightness;
    public int lightingChangeDuration = 1; // Seconds It Takes for Lighting To Lerp Between Brightness Values

    public int dayLength, dayMorning, dayNight;
    public float cycleSpeed = 1;

    [Space]
    public Light2D sun;
    public GameObject player;
    public int day, currentTime;

    private Vector3 sunPos;
    private bool loaded;

    void Start()
    {
        sun = GameObject.Find("GlobalLight").GetComponent<Light2D>();

        // Set Sun Position based on World Size (Middle x, 100 Tiles over Level Top
        Level level = GetComponent<WorldGenerator>().GetLevelInstance();
        sun.transform.position = new Vector3((level.Width / 2) * level.Scale, (level.Height * level.Scale));

        isRunning = true;
        StartCoroutine(TimeOfDayClock());

        if (!loaded)
        {
            currentTime = 200;
            day = 0;
        }

        if (isDay)
        {
            sun.intensity = dayBrightness;
            sun.color = dayColor;
        }
        else if (!isDay)
        {
            sun.intensity = nightBrightness;
            sun.color = nightColor;
        }
    }

    void Update()
    {
        if (isRunning)
        {
            if (currentTime > 0 && currentTime < dayMorning) // If It's Early (Before dayMorning)
            {
                isDay = false;
            }
            else if (currentTime >= dayMorning && currentTime < dayNight) // If It's Day (After dayMorning and Before dayNight)
            {
                if (!isDay)
                {
                    UnityEngine.Debug.Log("It is Day now! Good Morning!");
                    StartCoroutine(SmoothLightingToDay());
                    isDay = true;
                }
            }
            else if (currentTime >= dayNight && currentTime < dayLength) // If It's Night (After dayNight)
            {
                if (isDay)
                {
                    UnityEngine.Debug.Log("It is Night now! Get some Rest!");
                    StartCoroutine(SmoothLightingToNight());
                    isDay = false;
                }
            }
            else if (currentTime >= dayLength) // It's A Fresh New Day
            {
                currentTime = 0;
                day++;

                GameObject.Find("UICanvas").GetComponent<UIHandler>().SendNotif("A New Day! Full of possibilites :?", Color.black, 20f);
            }
        }
    }

    public IEnumerator TimeOfDayClock()
    {
        while (isRunning)
        {
            currentTime += 1;
            int minutes = Mathf.RoundToInt(currentTime / 60);
            int seconds = currentTime % 60;

            yield return new WaitForSeconds(1f / cycleSpeed);
        }
    }



    public IEnumerator SmoothLightingToDay() 
    {
        float t = 0;
        while (t <= lightingChangeDuration)
        {
            t += Time.deltaTime;
            float perc = t / lightingChangeDuration;
            sun.intensity = Mathf.Lerp(nightBrightness, dayBrightness, perc);
            sun.color = Color.Lerp(nightColor, dayColor, perc);
            yield return null;
        }
        if (t > lightingChangeDuration)
        {
            sun.intensity = dayBrightness;
            sun.color = dayColor;
            yield break;
        }
    }

    public IEnumerator SmoothLightingToNight()
    {
        float t = 0;
        while (t <= lightingChangeDuration)
        {
            t += Time.deltaTime;
            float perc = t / lightingChangeDuration;
            sun.intensity = Mathf.Lerp(dayBrightness, nightBrightness, perc);
            sun.color = Color.Lerp(dayColor, nightColor, perc);
            yield return null;
        }
        if (t > lightingChangeDuration)
        {
            sun.intensity = nightBrightness;
            sun.color = nightColor;
            yield break;
        }
    }



    public int GetDate() { return day; }
    public int GetTime() { return currentTime; }

    public void LoadDateAndTime()
    {
        SaveManager saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        day = saveManager.loadedData.levelsSaved[0].day;
        currentTime = saveManager.loadedData.levelsSaved[0].time;
        loaded = true;
    }
}
