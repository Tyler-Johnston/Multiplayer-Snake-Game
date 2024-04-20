using Shared.Components;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace Shared.Entities
{
    public class TurnPoint
    {
        public static Entity create(int snakeId, Vector2 position, float direction)
        {
            Entity entity = new Entity();

            entity.add(new Shared.Components.TurnPoint());
            entity.add(new SnakeId(snakeId));
            entity.add(new Position(position, direction));
            entity.add(new Shared.Components.Size(new Vector2(50, 50)));
            // entity.add(new Appearance("Textures/turn-point"));

            return entity;
        }
    }
}
