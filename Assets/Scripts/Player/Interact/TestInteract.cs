using UnityEngine;

namespace U_Grow
{
    public class TestInteract : MonoBehaviour, IInteracteable
    {
        public void Use()
        {
            Debug.Log("Whoopie, you did it, fat ass!");
        }
    }
}