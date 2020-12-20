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

        //if mutateScale is at 1 then if your random device gives you a 1 you go all the way to the extreme value (wich happens to be 1) 
        //if it's 0.5 you end up in the middle of your current pos and the max value
        private float mutateScale = 0.2f;

        //this is  basicly the 3 dimensional Genotype into a 1 dimensional fenotype
        //because of this you need to specify wich place has the correct gene
        //this is just an easy fix but it doesnt matter that much since 
        //you only need to add one line, one value and one mode in the lookup table for every gene you add
        public int Gender { get { return (int)Fenotype[0]; } }
        public float JumpHeight { get { return Fenotype[1]; } }
        public float Velocity { get { return Fenotype[2]; } }
        public float Courage { get { return Fenotype[3]; } }

        //0 for the average
        //1 for the biggest
        //2 for the biggest but also unable to mutate (only used for the male gene) 
        private int[,] lookupTable = new int[2,2] { { 2, 0 }, { 0, 0 } }; //it's not a bad thing if this is bigger than the actual lists being used

        protected Func<bool> willMutate = () => true;

        public Gene() {
            //you have two parts of the genotype, these two parts do exactly the same and code for the same genes
            //within that you have the different allels each allel contains a few values and each value decides an attribute
            //the allels can differ in size, all the genes in one allel are given to the next generation as one


            //this later on will have to be replaced with a random function
            Genotype = new List<float[]>[2] {new List<float[]>(), new List<float[]>() }; 
            Genotype[0].Add(new float[2] { 1.0f, 0.08f });//mannelijk en jumpHeight
            Genotype[0].Add(new float[2] { 0.5f, 0.3f }); //snelheid en courage
            Genotype[1].Add(new float[2] { 0f, 0.08f });//mannelijk en jumpHeight
            Genotype[1].Add(new float[2] { 0.5f, 0.3f }); //snelheid en courage
            //just add random genes here, dont forget to edit the lookup table and create a poperty accordingly

            calcFenotype();
        }

        public Gene(List<float[]> a, List<float[]> b) {
            Genotype = new List<float[]>[2] { a, b };
            calcFenotype();
        }

        private void calcFenotype() {
            //this is basicly turning a 3 dimensional array into a one dimensional array
            
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
            //mode 0 is the average, the others are just the highest gene overpowers the other for now
            if (mode == 0)
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
            Genotype[i][j][k] = Mutate(Genotype[i][j][k]);
            this.calcFenotype();
            return this;
        }

        private float Mutate(float x) {
            //range is the amount of you can go in a certain direction
            //mutatescale is how much of the range you're allowed to use
            //val is the actual random number used
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
    }
}
