namespace IntroProject
{
    public abstract class Wezen : Entity
    {
        public Genen Genen { get; protected set; }
        public int Leven;
        //Evolutieschaal Vincent: wat was daar ookalweer de bedoeling van?
        public Wezen()
        {
            Genen = new Genen();
            Genen.@class = this.GetType().Name;
            //mogelijk met een rng variable geven, of marges die in de contrustor worden gebracht.
        }

        public Wezen(Wezen ouder1, Wezen ouder2) : this(ouder1.Genen, ouder2.Genen) { }

        public Wezen(Genen ouder1, Genen ouder2)
        {
            //twee wezens maken een nieuw wezen.
        }

        public void eet(Entity entity)
        {
            //algemene eet methode. Is een concept moet nog beter worden uitgewerkt.
        }

        // Assuming this is the same type as wezen: we don't want Herbivores mating Carnivores
        public abstract Wezen? MateWith(Wezen wezen);
    }

    public sealed class WezenTestable : Wezen
    {
        private bool matingWillWork;
        public WezenTestable() : base() { }

        public WezenTestable(bool matingWillWork) : base()
        {
            this.matingWillWork = matingWillWork;
        }

        public WezenTestable(Wezen ouder1, Wezen ouder2) : base(ouder1, ouder2) { }

        public override Wezen MateWith(Wezen wezen) =>
            new WezenTestable(this, wezen);
    }
}
