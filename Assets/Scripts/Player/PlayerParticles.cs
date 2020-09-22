using UnityEngine;

namespace LensorRadii.U_Grow
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerParticles : MonoBehaviour
    {
        private void Awake()
        {
            // Subscribe to Event from PlayerMovement
            GetComponent<PlayerMovement>().OnSprint += SprintParticles;
            GetComponent<PlayerMovement>().OnDash += DashParticles;
            GetComponent<PlayerMovement>().OnJump += JumpParticles;
        }

        [SerializeField]
        ParticleSystem dashParticles;
        [SerializeField]
        ParticleSystem sprintParticles;
        [SerializeField]
        ParticleSystem jumpParticles;

        private void SprintParticles() { Instantiate(sprintParticles, GameReferences.player.transform); }
        private void DashParticles() { Instantiate(dashParticles, GameReferences.player.transform); }
        private void JumpParticles() { Instantiate(jumpParticles, GameReferences.player.transform); }
    }
}