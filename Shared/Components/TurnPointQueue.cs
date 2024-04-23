using System.Collections.Generic;

namespace Shared.Components
{
    public class TurnPointQueue : Component
    {
        public Queue<Shared.Entities.Entity> queue = new Queue<Shared.Entities.Entity>();
        public TurnPointQueue() { }
    }
}
