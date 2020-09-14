using UnityEngine;

namespace U_Grow
{
    public class PlayerInteract : MonoBehaviour
    {
        public GameObject player;

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (Input.GetButtonDown("Interact"))
            {
                // Find Object To Interact With
                IInteracteable interacteable = collider.GetComponent<IInteracteable>();
                if (interacteable != null)
                {
                    // USE THAT SON OF A BITCH YOU DIRTY DOG YOU
                    interacteable.Use();
                }
            }
        }
    }
}