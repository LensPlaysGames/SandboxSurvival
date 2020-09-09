using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider staminaSlider;
    public Player player;

    void Start()
    {
        staminaSlider = GetComponent<Slider>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (staminaSlider.value != player.GetPlayerStamina()) { staminaSlider.value = player.GetPlayerStamina(); }
    }
}
