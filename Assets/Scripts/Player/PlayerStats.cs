using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Stats")]
        public float health;
        public float stamina;
        [Space]
        public float tileDestroyTimeMultiplier;
        [Header("Movement Properties")]
        public float speedMultipler;
        public float jumpForceMultiplier;
        public float dashMultiplier;
    }
}