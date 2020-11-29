namespace IntroProject
{
    public class Herbivoor : Wezen
    {
        public Herbivoor() : base() {}
        public Herbivoor(Wezen ouder, Wezen ouder1) : base(ouder, ouder1) { }

        public override Wezen MateWith(Wezen wezen)
        {
            base.MateWith(wezen);
            return new Herbivoor(this, wezen);
        }
    }
}
