using IntroProject.Core.Error;

namespace IntroProject
{
    public abstract class Creature : Entity
    {
        public Gene gene { get; protected set; }
        public int isAlive;
        public bool isReadyToMate = true;

        public Creature()
        {
            gene = new Gene();
            gene.@class = this.GetType().Name;
            // gene should be random at first
        }

        public void calcFoodDistance() {
            //checks his own hexagon for food

            //starts search lines of increasing length
        }

        public Creature(Creature parentA, Creature parentB) : this(parentA.gene, parentB.gene) { }

        public Creature(Gene parentA, Gene parentB) {}

        public void eat(Entity entity) { }

        // Assuming this is the same type as wezen: we don't want Herbivores mating Carnivores
        public virtual void MatingSuccess()
        {
            isReadyToMate = false;
        }

        public virtual Creature? MateWith(Creature other)
        {
            if (!this.isReadyToMate || !other.isReadyToMate)
                throw new UnreadyForMating();

            this.MatingSuccess();
            other.MatingSuccess();
            return this;
        }
    }
}
