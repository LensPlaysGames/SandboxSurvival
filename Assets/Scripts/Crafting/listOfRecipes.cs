using System.Collections.Generic;
using UnityEngine;

namespace U_Grow
{
    public class listOfRecipes : MonoBehaviour
    {
        public static listOfRecipes instance;

        void Awake()
        {
            if (instance != null) { UnityEngine.Debug.LogError("Multiple Recipe Lists, WTF DID YOU DO"); Destroy(this); }
            else
            {
                instance = this;
                GameReferences.listOfRecipes = instance;
            }
        }

        public List<Recipe> recipes;
    }

}