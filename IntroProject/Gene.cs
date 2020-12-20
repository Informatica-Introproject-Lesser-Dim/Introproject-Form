using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroProject
{
    public class Gene : ICloneable, IEquatable<Gene>
    {
        public string @class;

        private List<float[]>[] Genotype; //we beginnen met velocity en jumpheight en courage maar dat verranderd later
        private List<float> Fenotype;
        // All between -1 and 1, will be scaled in their respective dependents

        //public Dictionary<string, float> primStat = new Dictionary<string, float>()
        //{ {"velocity", 0.5f}
        //, {"intelligence", 1.0f}
        //, {"courage", 1.0f}
        //, {"jumpEffectiveness", 0.08f}
        //, {"relSize", 1.0f}
        //, {"awarenessAuditory", 1.0f}
        //, {"awarenessOdor", 1.0f}
        //, {"awarenessVisual", 1.0f}
        //};

        private float mutateScale = 0.2f;
        public int Gender { get { return (int)Fenotype[0]; } }
        public float JumpHeight { get { return Fenotype[1]; } }
        public float Velocity { get { return Fenotype[2]; } }
        public float Courage { get { return Fenotype[3]; } }


        private int[,] lookupTable = new int[2,2] { { 2, 0 }, { 0, 0 } }; //it's not a bad thing if this is bigger than the actual lists being used

        protected Func<bool> willMutate = () => true;

        public Gene() {
            Genotype = new List<float[]>[2] {new List<float[]>(), new List<float[]>() };
            Genotype[0].Add(new float[2] { 1.0f, 0.08f });//mannelijk en jumpHeight
            Genotype[0].Add(new float[2] { 0.5f, 0.3f }); //snelheid en courage
            Genotype[1].Add(new float[2] { 0f, 0.08f });//mannelijk en jumpHeight
            Genotype[1].Add(new float[2] { 0.5f, 0.3f }); //snelheid en courage
            //bedenk hier hoe de genen precies in elkaar gaan zitten

            calcFenotype();
        }

        public Gene(List<float[]> a, List<float[]> b) {
            Genotype = new List<float[]>[2] { a, b };
            calcFenotype();
        }

        private void calcFenotype() {
            Fenotype = new List<float>();
            for (int i = 0; i < Genotype[0].Count; i++) {
                for (int j = 0; j < Genotype[0][i].Length; j++) {
                    Fenotype.Add(geneCalc(Genotype[0][i][j], Genotype[1][i][j],lookupTable[i,j]));
                }
            }
        }

        public List<float[]> getAllel() {
            List<float[]> result = new List<float[]>();
            Random random = new Random();
            for (int i = 0; i < Genotype[0].Count; i++)
            {
                result.Add(Genotype[random.Next(0, 2)][i]); //here could be a cloning by reference problem but it's probably fi
            }
            return result;
        } 

        private float geneCalc(float a, float b, int mode) {
            if (mode == 0)
                return (a + b) / 2;
            if (a > b)
                return a;
            return b;
        }

        //public Gene(Dictionary<string, float> primStat, bool isMale)
        //{
        //    this.primStat = primStat;
        //    this.isMale = isMale;
        //}

        public static Gene operator *(Gene a, Gene b) =>
            (a + b).Mutate();

        //public Gene CloneTypedMutated()
        //{
        //    Gene clone = CloneTyped();

        //    if (clone.willMutate())
        //        return ApplyMutation(clone);
        //    return clone;
        //}

        public Gene Mutate() {
            Random random = new Random();
            int i;
            int j;
            int k;
            do
            {
                i = random.Next(0, 2);
                j = random.Next(0, Genotype[i].Count);
                k = random.Next(0, Genotype[i][j].Length);
            } while (lookupTable[j,k] != 2); //if it's 2 then you're on the male gene wich cannot be mutated
            Genotype[i][j][k] = Mutate(Genotype[i][j][k]);
            this.calcFenotype();
            return this;
        }

        private float Mutate(float x) {
            Random random = new Random();
            float val = (float)random.NextDouble() * 2 - 1; //value between -1 and 1
            float range = x + 1; //amount of space left of the x
            if (val > 0) 
                range = 2 - range; //amount of space to the right
            x += val * range * mutateScale; //mutatescale is basicly what part of the entire range you move if you get a one
            return x;
        }

        public Gene CloneTyped() => (Gene)this.MemberwiseClone();
        public object Clone() => this.MemberwiseClone();

        public static Gene operator +(Gene a, Gene b)
        {
            return new Gene(a.getAllel(), b.getAllel());
          //var combinedPrimStat = new Dictionary<string, float>();
          //var combinedGenesAsPrimStatPairs =
          //      a.primStat.Keys.Zip(
          //        a.primStat.Values.Zip(
          //          b.primStat.Values,
          //          (float a, float b) => (a + b) / 2),
          //        (string key, float value) => new Tuple<string, float>(key, value));

          //foreach (var pair in combinedGenesAsPrimStatPairs)
          //  combinedPrimStat.Add(pair.Item1, pair.Item2);

          //return new Gene(combinedPrimStat, isMale: false);
        }

        //public static Gene ApplyMutation(Gene toBeMutated)
        //{
        //    toBeMutated.primStat["velocity"] -= 0.1f;
        //    return toBeMutated;
        //}

        public bool Equals(Gene other) {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < Genotype[0].Count; j++)
                    for (int k = 0; k < Genotype[0][j].Length; k++)
                        if (Genotype[i][j][k] != other.Genotype[i][j][k])
                            return false;
            return true;
        }
    }
}
