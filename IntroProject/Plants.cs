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

    public class Grass : Plants
    {

        public Grass(int x, int y, int nutritionValue) : base(x, y, nutritionValue)
        {
            this.x = x;
            this.y = y;
            this.nutritionValue = nutritionValue;
            color = Color.Green;
        }
        public void expand()
        {
            int expensionAge = 200;
            if (age > expensionAge)
            {
                //spawn one or more grasses in edjacent hexagons of current pos
            }
        }
    }
}
