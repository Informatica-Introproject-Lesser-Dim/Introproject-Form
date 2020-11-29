﻿namespace IntroProject
{
    public class Carnivoor : Wezen
    {
        public Carnivoor() : base() { }

        public Carnivoor(Wezen ouder, Wezen ouder1) : base(ouder, ouder1) { }

        public override Wezen MateWith(Wezen wezen)
        {
            base.MateWith(wezen);
            return new Carnivoor(this, wezen);
        }
    }
}
