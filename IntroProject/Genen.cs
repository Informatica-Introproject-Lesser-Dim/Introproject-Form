using System;
using System.Drawing;

namespace IntroProject
{
    public class Genen : ICloneable, IEquatable<Genen>
    {
        public string @class;
        int snelheidx;
        int snelheidy;
        int spronghoogte;
        int moed;

        protected Func<bool> willMutate = () => true;

        public Genen() {}

        public Genen(int snelheidx, int snelheidy, int sprongHoogte, int moed)
            : this(new Point(snelheidx, snelheidy), sprongHoogte, moed) { }
        public Genen(Point snelheid, int sprongHoogte, int moed)
        {
            this.snelheidx = snelheid.X;
            this.snelheidy = snelheid.Y;
            this.spronghoogte = sprongHoogte;
            this.moed = moed;
        }

        public static Genen operator *(Genen a, Genen b) =>
            a.CloneTypedMutated() + b.CloneTypedMutated();

        public Genen CloneTypedMutated()
        {
            Genen clone = CloneTyped();

            if (this.willMutate())
                return ApplyMutation(clone);
            return clone;
        }

        public Genen CloneTyped() => (Genen)this.MemberwiseClone();
        public object Clone() => this.MemberwiseClone();

        public static Genen operator +(Genen a, Genen b) =>
          new Genen( (a.snelheidx + b.snelheidx) / 2
                   , (a.snelheidy + b.snelheidy) / 2
                   , (a.spronghoogte + b.spronghoogte) / 2
                   , (a.moed + b.moed) / 2
                   );

        public static Genen ApplyMutation(Genen toBeMutated)
        {
            toBeMutated.snelheidx += 2;
            return toBeMutated;
        }

        public bool Equals(Genen other)
          => this.snelheidx == other.snelheidx
          && this.snelheidy == other.snelheidy
          && this.spronghoogte == other.spronghoogte
          && this.moed == other.moed;
    }
}
