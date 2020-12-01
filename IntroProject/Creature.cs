using IntroProject.Core.Error;

namespace IntroProject
{
    public abstract class Creature : Entity
    {
        public Genen Genen { get; protected set; }
        public int isAlive;
        public bool isReadyToMate = true;

        public Creature()
        {
            Genen = new Genen();
            Genen.@class = this.GetType().Name;
            // genes should be random at first
        }

        public Creature(Creature parentA, Creature parentB) : this(parentA.Genen, parentB.Genen) { }

        public Creature(Genen parentA, Genen parentB) {}

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
