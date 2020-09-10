using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;

    public bool isRunning;
    public bool isDay;

    [SerializeField]
    private Color nightColor, dayColor;
    [SerializeField]
    private float dayBrightness, nightBrightness;
    [SerializeField]
    private int lightingChangeDuration = 10; // Seconds It Takes for Lighting To Lerp Between Brightness Values
    [SerializeField]
    private int dayLength, dayMorning, dayNight;
    [SerializeField]
    private float cycleSpeed = 1;

    [Space]
    public Light2D sun;
    public GameObject player;
    public int day, currentTime;

    private Vector3 sunPos;
    private bool loaded;

    private string[] newDayNotifText = new string[] 
    {
        "A New Day! Full of Possibilites :?",
        "Midnight Comes and Goes, Dark as Night...",
        "Mark Another Tally... It's a New Day.",
    };
    private string[] nightNotifText = new string[]
    {
        "Darkness Awaits! Beware...",
        "Night Approaches... Watch Your Step!",
        "All That Slumbers Doesn't Do So At Night..."
    };
    private string[] morningNotifText = new string[]
    {
        "Dew Collects as the Morning Fog rolls in...",
        " *yawn* Why Did We Normalize Early Mornings ;/",
        "The Sun Rises Over the Horizon..."
    };

    #region Accessors

    public int MorningTime
    {
        get { return dayMorning; } 
    }

    #endregion

    void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("MULTIPLE DayNightCycles IN SCENE. Destroying " + this.name);
            Destroy(this);
        }
        else
        {
            instance = this;
            GameReferences.dayNightCycle = instance;
        }
    }

    void Start()
    {
        sun = GameObject.Find("GlobalLight").GetComponent<Light2D>();
        GameReferences.sunLight = sun;

        // Set Sun Position based on World Size (Middle x, Top of World) and Scale
        Level level = GameReferences.levelGenerator.GetLevelInstance();
        sun.transform.position = new Vector3((level.Width / 2) * level.Scale, (level.Height * level.Scale));
        sun.transform.localScale = new Vector3(level.Scale, level.Scale, 1);

        isRunning = true;
        StartCoroutine(TimeOfDayClock());

        if (!loaded)
        {
            currentTime = 200;
            day = 0;
        }

        if (isDay) { sun.intensity = dayBrightness; sun.color = dayColor; }
        else { sun.intensity = nightBrightness; sun.color = nightColor; }
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
                    StopCoroutine(SmoothLightingToDay());
                    StopCoroutine(SmoothLightingToNight());

                    StartCoroutine(SmoothLightingToDay());
                    isDay = true;

                    string dayNotif = morningNotifText[UnityEngine.Random.Range(0, morningNotifText.Length)];

                    GameReferences.uIHandler.SendNotif(dayNotif, 20f, Color.black);
                }
            }
            else if (currentTime >= dayNight && currentTime < dayLength) // If It's Night (After dayNight)
            {
                if (isDay)
                {
                    StopCoroutine(SmoothLightingToDay());
                    StopCoroutine(SmoothLightingToNight());

                    StartCoroutine(SmoothLightingToNight());
                    isDay = false;

                    string nightNotif = nightNotifText[UnityEngine.Random.Range(0, nightNotifText.Length)];

                    GameReferences.uIHandler.SendNotif(nightNotif, 20f, Color.black);
                }
            }
            else if (currentTime >= dayLength) // It's A Fresh New Day
            {
                currentTime = 0;
                day++;

                string newDayNotif = newDayNotifText[UnityEngine.Random.Range(0, newDayNotifText.Length)];

                GameReferences.uIHandler.SendNotif(newDayNotif, 20f, Color.black);
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
        SaveManager saveManager = GlobalReferences.saveManager;
        day = saveManager.loadedData.levelsSaved[saveManager.loadedData.playerData.levelIndex].day;
        currentTime = saveManager.loadedData.levelsSaved[saveManager.loadedData.playerData.levelIndex].time;
        loaded = true;
    }
}
