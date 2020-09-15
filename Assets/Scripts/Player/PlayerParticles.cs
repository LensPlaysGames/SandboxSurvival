using UnityEngine;

namespace U_Grow
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerParticles : MonoBehaviour
    {
        private void Awake()
        {
            // Subscribe to Event from PlayerMovement
            GetComponent<PlayerMovement>().OnSprint += SprintParticles;
            GetComponent<PlayerMovement>().OnDash += DashParticles;
        }

        [SerializeField]
        ParticleSystem dashParticles;
        [SerializeField]
        ParticleSystem sprintParticles;

        private void SprintParticles() { Instantiate(sprintParticles, GameReferences.player.transform); }
        private void DashParticles() { Instantiate(dashParticles, GameReferences.player.transform); }
    }
}