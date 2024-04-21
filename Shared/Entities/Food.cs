using Microsoft.Xna.Framework;
using Shared.Components;

namespace Shared.Entities
{
    public class Food
    {
        public static Entity create(int foodId, string texture, Vector2 position, float size)
        {
            Entity entity = new Entity();

            entity.add(new FoodId(foodId));
            entity.add(new Shared.Components.Food());
            entity.add(new Appearance(texture));
            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));

            return entity;
        }
    }

}
