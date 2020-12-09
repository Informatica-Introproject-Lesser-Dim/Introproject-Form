using System;
using System.Drawing;

namespace IntroProject
{
    public class Gene : ICloneable, IEquatable<Gene>
    {
        public string @class;
        int courage;
        float jumpHeight;
        int speedX; //waarom is dit verdeelt in twee speeds?
        int speedY;
        public float speed;

        protected Func<bool> willMutate = () => true;

        public Gene() {
            speed = 1.0f;
            jumpHeight = 0.05f;
        }

        public Gene(int speedX, int speedY, float jumpHeight, int courage)
            : this(speedX, jumpHeight, courage) { }
        public Gene(float speed, float jumpHeight, int courage)
        {
            this.speed = speed;
            this.jumpHeight = jumpHeight;
            this.courage = courage;
        }

        public static Gene operator *(Gene a, Gene b) =>
            a.CloneTypedMutated() + b.CloneTypedMutated();

        public Gene CloneTypedMutated()
        {
            Gene clone = CloneTyped();

            if (this.willMutate())
                return ApplyMutation(clone);
            return clone;
        }

        public Gene CloneTyped() => (Gene)this.MemberwiseClone();
        public object Clone() => this.MemberwiseClone();

        public static Gene operator +(Gene a, Gene b) =>
          new Gene( (a.speedX + b.speedX) / 2
                  , (a.speedY + b.speedY) / 2
                  , (a.jumpHeight + b.jumpHeight) / 2
                  , (a.courage + b.courage) / 2
                  );

        public static Gene ApplyMutation(Gene toBeMutated)
        {
            toBeMutated.speedX += 2;
            return toBeMutated;
        }

        public bool Equals(Gene other)
          => this.speedX == other.speedX
          && this.speedY == other.speedY
          && this.jumpHeight == other.jumpHeight
          && this.courage == other.courage;
    }
}
