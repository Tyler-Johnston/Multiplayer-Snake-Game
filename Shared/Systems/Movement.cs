namespace Shared.Systems
{
    public class Movement : System
    {
        public Movement() 
            : base(
                  typeof(Shared.Components.Movement),
                  typeof(Shared.Components.Position))
        { 
        }

        public override void update(TimeSpan elapsedTime)
        {
            foreach (var entity in m_entities.Values)
            {
                // Segments and tails are the only things with TPQs
                if (entity.contains<Shared.Components.TurnPointQueue>())
                {
                    // Ensure Queue isn't empty
                    if (entity.get<Shared.Components.TurnPointQueue>().queue.Count > 0)
                    {
                        // TurnPoint at the front of the segments queue
                        Shared.Entities.Entity tp = entity.get<Shared.Components.TurnPointQueue>().queue.Peek();
                        var tpOrientation = tp.get<Shared.Components.Position>().orientation;
                        var segOrientation = entity.get<Shared.Components.Position>().orientation;

                        
                        // if (tp.get<Shared.Components.Position>().position - entity.get<Shared.Components.Position>().position)
                        // {
                            entity.get<Shared.Components.Position>().orientation = tpOrientation;
                            entity.get<Shared.Components.TurnPointQueue>().queue.Dequeue();
                        // }
                    }
                }
                Shared.Entities.Utility.thrust(entity, elapsedTime);
            }
        }

    }
}
