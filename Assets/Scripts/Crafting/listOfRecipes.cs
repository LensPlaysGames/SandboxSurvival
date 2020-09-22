using System.Collections.Generic;
using UnityEngine;

namespace LensorRadii.U_Grow
{
    public class ListOfRecipes : MonoBehaviour
    {
        public static ListOfRecipes instance;

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