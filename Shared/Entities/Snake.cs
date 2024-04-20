﻿using Microsoft.Xna.Framework;
using Shared.Components;

namespace Shared.Entities
{
    public class Snake
    {
        public static Entity createHead(int snakeId, string texture, string name, Vector2 position, float size, float moveRate, float rotateRate)
        {
            Entity entity = new Entity();

            entity.add(new Name(name));
            entity.add(new SnakeId(snakeId));
            entity.add(new Appearance(texture));

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate, rotateRate));

            List<Input.Type> inputs = new List<Input.Type>();
            inputs.Add(Input.Type.TurnUp);
            inputs.Add(Input.Type.TurnDown);
            inputs.Add(Input.Type.TurnLeft);
            inputs.Add(Input.Type.TurnRight);
            entity.add(new Input(inputs));

            return entity;
        }
    }

    public class Utility
    {
        private const float DIR_UP = (float)(2 * Math.PI * 0.75);
        private const float DIR_DOWN = (float)(2 * Math.PI * 0.25);
        private const float DIR_LEFT = 0;
        private const float DIR_RIGHT = (float)(2 * Math.PI * 0.50);
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
        
        public static Entity? turnUp(Entity entity)
        {
            var position = entity.get<Position>();

            // Can't turn if already moving up or moving down, because can't move backward over itself
            if (position.orientation != DIR_UP && position.orientation != DIR_DOWN)
            {
                position.orientation = DIR_UP;
                // Because we accepted a turn, this is a new turn point right here and in this direction
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }

            return null;
        }

        public static Entity? turnDown(Entity entity)
        {
            var position = entity.get<Position>();

            // Can't turn if already moving down or moving up, because can't move backward over itself
            if (position.orientation != DIR_DOWN && position.orientation != DIR_UP)
            {
                position.orientation = DIR_DOWN;
                // Because we accepted a turn, this is a new turn point right here and in this direction
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }

            return null;
        }

        public static Entity? turnRight(Entity entity)
        {
            var position = entity.get<Position>();

            // Can't turn if moving already moving left or right, because can't move backward over itself
            if (position.orientation != DIR_LEFT && position.orientation != DIR_RIGHT)
            {
                position.orientation = DIR_LEFT;
                // Because we accepted a turn, this is a new turn point right here and in this direction
                int snakeId = entity.get<SnakeId>().id;
                return Shared.Entities.TurnPoint.create(snakeId, position.position, position.orientation);
            }

            return null;
        }

        public static Entity? turnLeft(Entity entity)
        {
            var position = entity.get<Position>();

            // Can't turn if moving already moving right or left, because can't move backward over itself
            if (position.orientation != DIR_RIGHT && position.orientation != DIR_LEFT)
            {
                position.orientation = DIR_RIGHT;
                // Because we accepted a turn, this is a new turn point right here and in this direction
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
