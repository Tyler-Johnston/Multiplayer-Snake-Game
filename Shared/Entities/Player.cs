﻿using Microsoft.Xna.Framework;
using Shared.Components;

namespace Shared.Entities
{
    public class Player
    {
        public static Entity create(int snakeId, string texture, string name, Vector2 position, float size, float moveRate)
        {
            Entity entity = new Entity();

            entity.add(new SnakeId(snakeId));
            entity.add(new Name(name));
            entity.add(new Appearance(texture));

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate));

            List<Input.Type> inputs = new List<Input.Type>();
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
        public static void thrust(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            var vectorX = Math.Cos(position.orientation);
            var vectorY = Math.Sin(position.orientation);

            position.position = new Vector2(
                (float)(position.position.X + vectorX * movement.moveRate * elapsedTime.Milliseconds),
                (float)(position.position.Y + vectorY * movement.moveRate * elapsedTime.Milliseconds));
        }

        public static Entity? turnLeft(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != 0.0f)
            {
                position.orientation = (float)(Math.PI);
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
        public static Entity? turnRight(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI))
            {
                position.orientation = 0.0f;
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
        public static Entity? turnUp(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI / 2))
            {
                position.orientation = (float)(Math.PI * 3 / 2);
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
        public static Entity? turnDown(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI * 3 / 2))
            {
                position.orientation = (float)(Math.PI / 2);
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
        public static Entity? turnDownLeft(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();


            if (position.orientation != (float)(Math.PI * 7 / 4))
            {
                position.orientation = (float)(Math.PI * 3 / 4);
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
        public static Entity? turnDownRight(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI * 5 / 4))
            {
                position.orientation = (float)(Math.PI / 4);
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
        public static Entity? turnUpRight(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();


            if (position.orientation != (float)(Math.PI * 3 / 4))
            {
                position.orientation = (float)(Math.PI * 7 / 4);
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
        public static Entity? turnUpLeft(Entity entity)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            if (position.orientation != (float)(Math.PI / 4))
            {
                position.orientation = (float)(Math.PI * 5 / 4);
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }
            return null;
        }
    }
}