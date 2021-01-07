using System.Drawing;

namespace IntroProject
{
    public class DeathPile : Entity
    {
        protected int age;
        protected int nutritionValue;

        public DeathPile(int x, int y, int nutritionValue) //De input waarde zijn voor referentie.
        { // dit kan anders/ MOET NOG BESPROKEN WORDEN
            this.x = x;
            this.y = y;
            this.nutritionValue = nutritionValue;
            color = Color.SaddleBrown;
        }
        private Creature feed(Creature wezen) //concept hoe een plant een wezen kan voeden, mogelijk moet herbivoor de specificatie zijn hier en niet wezen.
        {//MOET NOG BESPROKEN WORDEN
            wezen.Alive += nutritionValue;
            //death methode aanroepen
            return wezen;
        }

       
    }
}
