namespace IntroProject
{
    public class Planten : Entity
    {
        private int voedingswaarde;

        public Planten((int, int) positie, int voedingswaarde) //De input waarde zijn voor referentie.
        { // dit kan anders/ MOET NOG BESPROKEN WORDEN
            this.positie = positie;
            this.voedingswaarde = voedingswaarde;
        }
        private Wezen voed(Wezen wezen) //concept hoe een plant een wezen kan voeden, mogelijk moet herbivoor de specificatie zijn hier en niet wezen. 
        {//MOET NOG BESPROKEN WORDEN
            wezen.Leven += voedingswaarde; 
            //death methode aanroepen
            return wezen;
        }
    }
}
