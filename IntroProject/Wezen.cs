namespace IntroProject
{
    public abstract class Wezen : Entity
    {
        protected int snelheidx;
        protected int snelheidy;
        protected int spronghoogte;
        protected int moed;
        public int Leven;
        //Evolutieschaal Vincent: wat was daar ookalweer de bedoeling van?
        public Wezen()
        {
            //mogelijk met een rng variable geven, of marges die in de contrustor worden gebracht.
        }
        public Wezen(Wezen ouder, Wezen ouder1)
        {
            //twee wezens maken een nieuw wezen.

        }
        public void eet(Entity entity)
        {
            //algemene eet methode. Is een concept moet nog beter worden uitgewerkt.
        }
    }
}
