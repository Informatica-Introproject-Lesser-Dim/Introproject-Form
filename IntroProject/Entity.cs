namespace IntroProject
{
    public abstract class Entity
    {
        protected int id; //Vincent: Hoe kan je er voor zorgen dat de id niet overlapt met id's van andere entities?
        public int x,y;
        public Hexagon chunk;

        public void Sterven()
        {
            //Haal entity weg uit de chunk entitie lijst, door entity met hetzelfde id als deze uit de lijst te halen.
        }
    }
}
