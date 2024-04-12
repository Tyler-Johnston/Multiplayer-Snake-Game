﻿using Microsoft.Xna.Framework;
using Shared.Components;

namespace Shared.Entities
{
    public class Player
    {
        public static Entity create(string texture, string name, Vector2 position, float size, float moveRate, float rotateRate)
        {
            Entity entity = new Entity();

            entity.add(new Name(name));
            entity.add(new Appearance(texture));

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate, rotateRate));

            List<Input.Type> inputs = new List<Input.Type>();
            // inputs.Add(Input.Type.Thrust);
            inputs.Add(Input.Type.RotateLeft);
            inputs.Add(Input.Type.RotateRight);
            entity.add(new Input(inputs));

            return entity;
        }
    }

    public class Utility
    {
        // public static void thrust(Entity entity, TimeSpan elapsedTime)
        // {
        //     var position = entity.get<Position>();
        //     var movement = entity.get<Movement>();

        //     var vectorX = Math.Cos(position.orientation);
        //     var vectorY = Math.Sin(position.orientation);

        //     position.position = new Vector2(
        //         (float)(position.position.X + vectorX * movement.moveRate * elapsedTime.Milliseconds),
        //         (float)(position.position.Y + vectorY * movement.moveRate * elapsedTime.Milliseconds));
        // }

        public static void rotateLeft(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            position.orientation = position.orientation - movement.rotateRate * elapsedTime.Milliseconds;
        }

        public static void rotateRight(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            position.orientation = position.orientation + movement.rotateRate * elapsedTime.Milliseconds;
        }
    }
}
