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
                        // If tp isn't the same direction as current segment direction, change segment dir
                        if (tp.get<Shared.Components.Position>().orientation != entity.get<Shared.Components.Position>().orientation)
                        {
                            entity.get<Shared.Components.TurnPointQueue>().queue.Dequeue();
                            Console.WriteLine(entity.get<Shared.Components.Position>().orientation);

                            entity.get<Shared.Components.Position>().orientation 
                                = tp.get<Shared.Components.Position>().orientation;
                        }
                    }
                }
                Shared.Entities.Utility.thrust(entity, elapsedTime);
            }
        }

    }
}
