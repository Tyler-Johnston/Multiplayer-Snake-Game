namespace Shared.Systems
{
    public class Movement : System
    {
        public Movement() 
            : base(
                //   typeof(Shared.Components.Movement),
                  typeof(Shared.Components.Position))
        { 
        }

        public override void update(TimeSpan elapsedTime)
        {
            foreach (var entity in m_entities.Values)
            {
                if (entity.contains<Components.Segment>())
                {
                    var snakeid = entity.get<Components.SnakeId>().id;
                    Console.WriteLine(snakeid);

                    foreach (var e in m_entities.Values)
                    {
                        if (e.contains<Components.TurnPoint>())
                        {
                            var tpsnake = e.get<Components.SnakeId>().id;
                            Console.WriteLine(tpsnake);
                            if (tpsnake == snakeid)
                            {
                                Console.WriteLine("We be snakin");
                            }
                        }
                    }
                }
                if (entity.contains<Components.Movement>())
                {
                    Shared.Entities.Utility.thrust(entity, elapsedTime);
                }
            }
        }
    }
}
