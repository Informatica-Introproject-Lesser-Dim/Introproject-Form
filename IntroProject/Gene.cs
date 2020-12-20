using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroProject
{
    public class Gene : ICloneable, IEquatable<Gene>
    {
        public string @class;

        // All between -1 and 1, will be scaled in their respective dependents

        public Dictionary<string, float> primStat = new Dictionary<string, float>()
        { {"velocity", 0.0f}
        , {"intelligence", 1.0f}
        , {"courage", 1.0f}
        , {"jumpEffectiveness", 1.0f}
        , {"relSize", 1.0f}
        , {"awarenessAuditory", 1.0f}
        , {"awarenessOdor", 1.0f}
        , {"awarenessVisual", 1.0f}
        };
        bool isMale = true;

        protected Func<bool> willMutate = () => true;

        public Gene() { }

        public Gene(Dictionary<string, float> primStat, bool isMale)
        {
            this.primStat = primStat;
            this.isMale = isMale;
        }

        public static Gene operator *(Gene a, Gene b) =>
            a.CloneTypedMutated() + b.CloneTypedMutated();

        public Gene CloneTypedMutated()
        {
            Gene clone = CloneTyped();

            if (clone.willMutate())
                return ApplyMutation(clone);
            return clone;
        }

        public Gene CloneTyped() => (Gene)this.MemberwiseClone();
        public object Clone() => this.MemberwiseClone();

        public static Gene operator +(Gene a, Gene b)
        {
          var combinedPrimStat = new Dictionary<string, float>();
          var combinedGenesAsPrimStatPairs =
                a.primStat.Keys.Zip(
                  a.primStat.Values.Zip(
                    b.primStat.Values,
                    (float a, float b) => (a + b) / 2),
                  (string key, float value) => new Tuple<string, float>(key, value));

          foreach (var pair in combinedGenesAsPrimStatPairs)
            combinedPrimStat.Add(pair.Item1, pair.Item2);

          return new Gene(combinedPrimStat, isMale: false);
        }

        public static Gene ApplyMutation(Gene toBeMutated)
        {
            toBeMutated.primStat["velocity"] -= 0.1f;
            return toBeMutated;
        }

        public bool Equals(Gene other) =>
          this.primStat.Values
            .Zip(other.primStat.Values, (float a, float b) => a == b)
            .All((bool allTrue) => allTrue);
    }
}
