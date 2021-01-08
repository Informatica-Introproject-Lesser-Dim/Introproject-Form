using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroProject
{
    public abstract class Gene : ICloneable, IEquatable<Gene>
    {
        public string @class;

        private List<float[]>[] Genotype; //we beginnen met velocity en jumpheight en courage maar dat verranderd later
        protected List<float> Fenotype;
        private Random random;
        // All between -1 and 1, will be scaled in their respective dependents

        //if mutateScale is at 1 then if your random device gives you a 1 you go all the way to the extreme value (wich happens to be 1) 
        //if it's 0.5 you end up in the middle of your current pos and the max value
        private float mutateScale = 0.2f;
        private float extraSensitivity = 0.1f;
        //this is  basicly the 3 dimensional Genotype into a 1 dimensional fenotype
        //because of this you need to specify wich place has the correct gene
        //this is just an easy fix but it doesnt matter that much since 
        //you only need to add one line, one value and one mode in the lookup table for every gene you add
        public int Gender { get { return (int)Fenotype[0]; } }// either 0 or 1

        public float Size { get { return (Fenotype[1]/2 + 0.5f) * 9700 + 300; } } //within certain max and min values (right now between 100 and 1000)

        public float HungerBias { get { return ToInfinity(Fenotype[2]/2 + 0.5f); } }//positive and to infinity

        public float sexualPreference { get { return ToInfinity(Fenotype[3] / 2 + 0.5f) + 50; } } //to infinity but still needs to have at least 50
        public float energyDistribution { get { return Fenotype[4] / 3 + 0.33f; /* Settings.MatingCost */ } } //just a percentage between 0 and 1
        //Second Allel
        public float PassivePreference { get { return ToInfinity(Fenotype[5] / 2 + 0.5f)/5; } }
        public float PassiveBias { get { return Fenotype[6]/3 + 2.0f/3; } }// positive and to infinity
        public float ActiveBias { get { return (Fenotype[7]/2 + 0.5f) * (1 - PassiveBias) + this.PassiveBias; } } //active must be higher than passive
        public float ActivePreference { get { return ToInfinity(Fenotype[8] / 2 + 0.5f) / 15 + this.PassiveBias; } }
        //Third Allel
        public virtual float Velocity { get; } //within certain max and min values

        public virtual float SprintSpeed { get; }

        public virtual float SprintDuration { get; }
        public virtual float perception { get { return Fenotype[12] / 2 + 0.5f; } }
        public float Courage { get { return Fenotype[13]; } }//not implemented anywhere yet

        //Fourth Allel

        public float DistanceBias { get { return ToInfinity(Fenotype[14] / 2 + 0.5f); } } //it's in the name dummy

        public float JumpHeight { get { return Fenotype[15] / 4 + 0.25f; } } //only positive values

        

        public float EatSpeed { get { return Fenotype[17] / 2 + 0.5f; } }
        //-1 for "gene isnt used"
        //0 for the average
        //1 for the biggest
        //2 for the biggest but also unable to mutate (only used for the male gene) 
        //3 extra sensitivity with mutation
        //4 sensitive + biggest
        private int[,] lookupTable = new int[4,5] { { 2, 3, 3, 3, 0}, { 3, 3, 3,3 , -1}, {0, 0, 0, 3, 3 }, {3,3,3,3,-1 } }; //it's not a bad thing if this is bigger than the actual lists being used

        protected Func<bool> willMutate = () => true;

        public Gene() {
            random = new Random();
            //you have two parts of the genotype, these two parts do exactly the same and code for the same genes
            //within that you have the different allels each allel contains a few values and each value decides an attribute
            //the allels can differ in size, all the genes in one allel are given to the next generation as one
            Genotype = new List<float[]>[2] {GenotypeRandom(), GenotypeRandom()};
            //just add random genes here, dont forget to edit the lookup table and create a poperty accordingly

            
            int isMale = random.Next(0, 2);
            if (isMale == 1)
            {
                Genotype[0][0][0] = 1;
                Genotype[1][0][0] = 0;
            }
            else {
                Genotype[0][0][0] = 0;
                Genotype[1][0][0] = 0;
            }


            calcFenotype();
        }

        private float NormDist(float range) {
            return NormDist()*range;
        }

        //get a random value between -1 and 1 (will go according to the normal distribution later on)
        private float NormDist() {
            random = new Random();
            return (float)((random.NextDouble() - 0.5) * 2);
        }

        public float ToInfinity(float x) {
            if (x == 1 || x == -1)
                return 0;
            return x / (1 - Math.Abs(x));
        }

        private List<float[]> GenotypeRandom() {
            List<float[]> result = new List<float[]>();
            for (int i = 0; i < lookupTable.GetLength(0); i++) {
                List<float> temp = new List<float>();
                for (int j = 0; j < lookupTable.GetLength(1); j++)
                    if (lookupTable[i, j] != -1) {
                        if (lookupTable[i, j] == 2)
                            temp.Add(random.Next(0, 3)%2);
                        else
                            temp.Add(NormDist(0.75f));
                    }
                if (temp.Count == 0)
                    continue;
                float[] temp2 = new float[temp.Count]; //List to array conversion
                for (int j = 0; j < temp.Count; j++)
                    temp2[j] = temp[j];
                result.Add(temp2);
            }   
            return result;
        }

        public Gene(List<float[]> a, List<float[]> b) {
            Genotype = new List<float[]>[2] { a, b };
            calcFenotype();
        }

        public Gene(List<float[]>[] genoType) {
            Genotype = new List<float[]>[2];
            for (int i = 0; i < 2; i++) {
                List<float[]> chromosome = new List<float[]>();
                for (int j = 0; j < genoType[i].Count; j++) {
                    chromosome.Add((float[]) genoType[i][j].Clone());
                }
                Genotype[i] = chromosome;
            }
        }

        private void calcFenotype() {
            //this is basicly turning a 3 dimensional array into a one dimensional array
            
            Fenotype = new List<float>();
            int start = 0;

            float[] dominant = null; //the male gene is the "dominant" gene
            if (Genotype[0][1][1] == 1)
                dominant = Genotype[0][1];
            else if (Genotype[1][1][1] == 1)
                dominant = Genotype[1][1];

            if (dominant != null) {
                start = 1; //if this gene is already added it needs to be skipped in the main for loop
                for (int j = 0; j < dominant.Length; j++)
                    Fenotype.Add(dominant[j]);
            }

            for (int i = start; i < Genotype[0].Count; i++) {
                for (int j = 0; j < Genotype[0][i].Length; j++) {
                    Fenotype.Add(geneCalc(Genotype[0][i][j], Genotype[1][i][j],lookupTable[i,j]));
                }
            }
        }

        public List<float[]> getAllel() {
            List<float[]> result = new List<float[]>();
            for (int i = 0; i < Genotype[0].Count; i++)
            {
                result.Add((float[]) Genotype[random.Next(0, 2)][i].Clone()); //here could be a cloning by reference problem but it's probably fine
            }
            return result;
        } 

        private float geneCalc(float a, float b, int mode) {
            //mode 0 is the average, the others are just the highest gene overpowers the other for now
            if (mode == 0 || mode == 3)
                return (a + b) / 2; 
            if (a > b)
                return a;
            return b;
        }

        public static Gene operator *(Gene a, Gene b) =>
            (a + b).Mutate();

        public Gene Mutate() {
            Random random = new Random();
            //choosing a random gene wich is to be mutated
            int i;
            int j;
            int k;
            do
            {
                i = random.Next(0, 2);
                j = random.Next(0, Genotype[i].Count);
                k = random.Next(0, Genotype[i][j].Length);
            } while (lookupTable[j,k] != 2); //if it's 2 then you're on the male gene wich cannot be mutated
            Genotype[i][j][k] = Mutate(Genotype[i][j][k], lookupTable[j,k]);
            this.calcFenotype();
            return this;
        }

        private float Mutate(float x, int n) {
            //range is the amount of you can go in a certain direction
            //mutatescale is how much of the range you're allowed to use
            //val is the actual random number used
            float val = NormDist(1); //value between -1 and 1
            float range = x + 1; //amount of space left of the x
            if (val > 0) 
                range = 2 - range; //amount of space to the right
            if (n >= 3)
                val *= extraSensitivity; //value is decreased depending on the extra sensitivity if necessary
            x += val * range * mutateScale; //mutatescale is basicly what part of the entire range you move if you get a one
            
            return x;
        }

        public Gene CloneTyped() => (Gene)this.Clone();

        public Object Clone() {
            return new HerbivoreGene(this.Genotype);
        }

        public static Gene operator +(Gene a, Gene b)
        {
            return new HerbivoreGene(a.getAllel(), b.getAllel());
        }

        public bool Equals(Gene other) {
            //just basic checking every gene
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < Genotype[0].Count; j++)
                    for (int k = 0; k < Genotype[0][j].Length; k++)
                        if (Genotype[i][j][k] != other.Genotype[i][j][k])
                            return false;
            return true;
        }

        public bool EqualFenoType(Gene other) {
            return other.EqualFenoType(this.Fenotype);
        }

        public bool EqualFenoType(List<float> other) {
            if (other.Count != Fenotype.Count)
                return false;
            for (int i = 0; i < other.Count; i++)
                if (Fenotype[i] != other[i])
                    return false;
            return true;
        }
    }

    public class HerbivoreGene : Gene 
    {
        public HerbivoreGene() : base() { }
        public HerbivoreGene(List<float[]>[] genoType) : base(genoType) { }
        public HerbivoreGene(List<float[]> a, List<float[]> b) : base(a, b) { }

        public static HerbivoreGene operator +(HerbivoreGene a, HerbivoreGene b)
        {
            return new HerbivoreGene(a.getAllel(), b.getAllel());
        }

        public override float Velocity { get { return (Fenotype[9] / 2 + 0.5f) * 4.5f + 0.5f; } } //within certain max and min values

        public override float SprintSpeed { get { return (Fenotype[10]/2 + 0.5f)*2 + 1 + Velocity; } }

        public override float SprintDuration { get { return (Fenotype[11] / 2 + 0.5f) * 990 + 10; } }
    }

    public class CarnivoreGene : Gene
    {
        public CarnivoreGene() : base() { }
        public CarnivoreGene(List<float[]>[] genoType) : base(genoType) { }
        public CarnivoreGene(List<float[]> a, List<float[]> b) : base(a, b) { }

        public override float Velocity { get { return (Fenotype[9] / 2 + 0.5f) * 4.5f + 0.5f; } } //within certain max and min values

        public override float SprintSpeed { get { return (Fenotype[10] / 2 + 0.5f) * 3 + 2 + Velocity; } }

        public override float SprintDuration { get { return (Fenotype[11] / 2 + 0.5f)* 990 + 10; } }

        public float LivingTargetBias { get { return ToInfinity(Fenotype[16] / 2 + 0.5f) / 5; } }

        public static CarnivoreGene operator +(CarnivoreGene a, CarnivoreGene b)
        {
            return new CarnivoreGene(a.getAllel(), b.getAllel());
        }
    }
}
