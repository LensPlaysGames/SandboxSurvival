using System;

namespace LensorRadii.U_Grow
{
    [Serializable]
    public class Recipe
    {
        public string name;

        public Slot[] ingredients;
        public Slot output;
    }
}