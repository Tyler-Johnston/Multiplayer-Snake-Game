
using Microsoft.Xna.Framework;
using Shared.Components;
using Shared.Entities;
using System.Text;

namespace Shared.Messages
{
    public class UpdateEntity : Message
    {
        public UpdateEntity(Entity entity, TimeSpan updateWindow) : base(Type.UpdateEntity)
        {
            this.id = entity.id;

            if (entity.contains<Position>())
            {
                this.hasPosition = true;
                this.position = entity.get<Position>().position;
                this.orientation = entity.get<Position>().orientation;
            }

            if (entity.contains<Score>())
            {
                this.hasScore = true;
                this.score = entity.get<Score>().score;
            }

            this.updateWindow = updateWindow;
        }

        public UpdateEntity(): base(Type.UpdateEntity)
        {
        }

        public uint id { get; private set; }

        // Position
        public bool hasPosition { get; private set; } = false;
        public Vector2 position { get; private set; }
        public float orientation { get; private set; }

        // Score
        public bool hasScore { get; private set; } = false;
        public int score { get; private set; }

        // Kill Count
        public bool hasKillCount { get; private set; } = false;
        public int killCount { get; private set; }

        // Only the milliseconds are used/serialized
        public TimeSpan updateWindow { get; private set; } = TimeSpan.Zero;

        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());
            data.AddRange(BitConverter.GetBytes(id));

            data.AddRange(BitConverter.GetBytes(hasPosition));
            if (hasPosition)
            {
                data.AddRange(BitConverter.GetBytes(position.X));
                data.AddRange(BitConverter.GetBytes(position.Y));
                data.AddRange(BitConverter.GetBytes(orientation));
            }

            data.AddRange(BitConverter.GetBytes(hasScore));
            if (hasScore)
            {
                data.AddRange(BitConverter.GetBytes(score));
            }

            data.AddRange(BitConverter.GetBytes(hasKillCount));
            if (hasKillCount)
            {
                data.AddRange(BitConverter.GetBytes(hasKillCount));
            }

            data.AddRange(BitConverter.GetBytes(updateWindow.Milliseconds));

            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            this.id = BitConverter.ToUInt32(data, offset);
            offset += sizeof(uint);

            this.hasPosition = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasPosition)
            {
                float positionX = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                float positionY = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                this.position = new Vector2(positionX, positionY);
                this.orientation = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
            }

            this.hasScore = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasScore)
            {
                this.score = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);
            }

            this.hasKillCount = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasKillCount)
            {
                this.killCount = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);
            }

            this.updateWindow = new TimeSpan(0, 0, 0, 0, BitConverter.ToInt32(data, offset));
            offset += sizeof(Int32);

            return offset;
        }
    }
}
