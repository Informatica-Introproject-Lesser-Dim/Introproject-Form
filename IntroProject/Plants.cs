using System.Drawing;

namespace IntroProject
{
    public class Plants : Entity
    {
        protected int age;
        protected int nutritionValue;

        public Plants(int x, int y, int nutritionValue) //De input waarde zijn voor referentie.
        { // dit kan anders/ MOET NOG BESPROKEN WORDEN
            this.x = x;
            this.y = y;
            this.nutritionValue = nutritionValue;
            color = Color.Green;
        }
        private Creature feed(Creature wezen) //concept hoe een plant een wezen kan voeden, mogelijk moet herbivoor de specificatie zijn hier en niet wezen.
        {//MOET NOG BESPROKEN WORDEN
            wezen.isAlive += nutritionValue;
            //death methode aanroepen
            return wezen;
        }

       
    }
}
