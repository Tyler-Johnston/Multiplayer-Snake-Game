using Microsoft.Xna.Framework;
using Shared.Components;

namespace Shared.Entities
{
    public class Player
    {
        public static Entity create(string texture, Vector2 position, float size, float moveRate, float rotateRate)
        {
            Entity entity = new Entity();

            entity.add(new Appearance(texture));

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate, rotateRate));

            List<Input.Type> inputs = new List<Input.Type>();
            // inputs.Add(Input.Type.Thrust);
            inputs.Add(Input.Type.TurnLeft);
            inputs.Add(Input.Type.TurnRight);
            inputs.Add(Input.Type.TurnUp);
            inputs.Add(Input.Type.TurnDown);
            entity.add(new Input(inputs));

            return entity;
        }
    }

    public class Utility
    {
        public static void turnLeft(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != 0.0f)
            {
                position.orientation = (float)(Math.PI);
            }
        }
        public static void turnRight(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI))
            {
                position.orientation = 0.0f;
            }
        }
        public static void turnUp(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI / 2))
            {
                position.orientation = (float)(Math.PI * 3 / 2);
            }
        }

        public static void turnDown(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI * 3 / 2))
            {
                position.orientation = (float)(Math.PI / 2);
            }
        }
        public static void turnDownLeft(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI / 4))
            {
                position.orientation = (float)(Math.PI * 5 / 4);
            }
        }
        public static void turnDownRight(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI * 3 / 4))
            {
                position.orientation = (float)(Math.PI * 7 / 4);
            }
        }
        public static void turnUpRight(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI * 5 / 4))
            {
                position.orientation = (float)(Math.PI / 4);
            }
        }
        public static void turnUpLeft(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI * 7 / 4))
            {
                position.orientation = (float)(Math.PI * 3 / 4);
            }
        }
    }
}
