using Microsoft.Xna.Framework;
using Shared.Components;

namespace Shared.Entities
{
    public class Tail
    {
        public static Entity createTail(int snakeId, string texture, Vector2 position, float size, float moveRate)
        {
            Entity entity = new Entity();
            entity.add(new Shared.Components.Tail());
            entity.add(new Shared.Components.TurnPointQueue());

            entity.add(new Appearance(texture));
            entity.add(new SnakeId(snakeId));

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate));

            return entity;
        }
    }
}