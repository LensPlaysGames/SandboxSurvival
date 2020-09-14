using UnityEngine;
using UnityEngine.UI;

namespace U_Grow
{
    public class StaminaBar : MonoBehaviour
    {
        public Slider staminaSlider;
        public Player player;
        public PlayerMovement playerMoveScript;

        void Start()
        {
            staminaSlider = GetComponent<Slider>();
            playerMoveScript = GameReferences.player.GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (staminaSlider.value != playerMoveScript.GetPlayerStamina()) { staminaSlider.value = playerMoveScript.GetPlayerStamina(); }
        }
    }
}