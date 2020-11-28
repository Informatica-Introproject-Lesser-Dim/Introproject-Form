namespace IntroProject
{
    public class Herbivoor : Wezen
    {
        public Herbivoor(Wezen ouder, Wezen ouder1) : base(ouder, ouder1) { }

        public override Wezen MateWith(Wezen wezen) =>
            new Herbivoor(this, wezen);
    }
}
