namespace IntroProject
{
    static class Settings
    {
        //geef nog alles de correcte initiele waarden, en de sliders moeten nog goed worden gescaled.
        public static float StepSize = 1; //YET TO COME, but where and how?
        public static int StartCarnivore = 3; //Verstand? en anders in Map
        public static int StartHerbivore = 20; //MenuScreen updates on new creation
        public static float MiddleHeight = 0f; //Hexagon.cs update on new creation
        public static float MatingCost = 0.75f; //Creature
        public static int GrassGrowth = 200; //Vegetation.cs
        public static int GrassMaxFeed = 1500; //Vegetation.cs
        public static float WalkEnergy = 0.1f; //Calculator.cs
        public static float JumpEnergy = 0.001f; //Calculator.cs
        public static float PassiveEnergy = 0.00001f; //Calculator.cs
        public static bool AddHeatMap = false; //Hexagon.cs
        public static int LanguageIndex = 1;
    }
}
