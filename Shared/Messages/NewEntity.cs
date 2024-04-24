using Microsoft.Xna.Framework;
using Shared.Components;
using Shared.Entities;
using System.Text;

namespace Shared.Messages
{
    public class NewEntity : Message
    {
        public NewEntity(Entity entity) : base(Type.NewEntity)
        {
            this.id = entity.id;

            if (entity.contains<Components.Segment>())
            {
                this.hasSegment = true;
            }

            if (entity.contains<SnakeId>())
            {
                this.hasSnakeId = true;
                this.snakeId = entity.get<SnakeId>().id;
            }

            if (entity.contains<Score>())
            {
                this.hasScore = true;
                this.score = entity.get<Score>().score;
            }

            if (entity.contains<KillCount>())
            {
                this.hasKillCount = true;
                this.killCount = entity.get<KillCount>().killCount;
            }

            if (entity.contains<Shared.Components.TurnPoint>())
            {
                this.hasTurnPoint = true;
            }

            if (entity.contains<Shared.Components.Food>())
            {
                this.hasFood = true;
            }

            if (entity.contains<Name>())
            {
                this.hasName = true;
                this.name = entity.get<Name>().name;
            }

            if (entity.contains<PlayerType>())
            {
                this.hasPlayerType = true;
                this.playerType = entity.get<PlayerType>().playerType;
            }

            if (entity.contains<Appearance>())
            {
                this.hasAppearance = true;
                this.texture = entity.get<Appearance>().texture;
            }
            else
            {
                this.texture = "";
            }

            if (entity.contains<Position>())
            {
                this.hasPosition = true;
                this.position = entity.get<Position>().position;
                this.orientation = entity.get<Position>().orientation;
            }

            if (entity.contains<Size>())
            {
                this.hasSize = true;
                this.size = entity.get<Size>().size;
            }

            if (entity.contains<Movement>())
            {
                this.hasMovement = true;
                this.moveRate = entity.get<Movement>().moveRate;
            }

            if (entity.contains<Components.Input>())
            {
                this.hasInput = true;
                this.inputs = entity.get<Components.Input>().inputs;
            }
            else
            {
                this.inputs = new List<Components.Input.Type>();
            }
        }
        public NewEntity() : base(Type.NewEntity)
        {
            this.texture = "";
            this.inputs = new List<Components.Input.Type>();
        }

        public uint id { get; private set; }

        // Food Component
        public bool hasFood { get; private set; } = false;

        public bool hasSegment {get; private set; } = false;

        // SnakeId
        public bool hasSnakeId { get; private set; } = false;
        public int snakeId {  get; private set; }


        // Score
        public bool hasScore { get; private set; } = false;
        public int score {  get; private set; }

        // Kill Count
        public bool hasKillCount { get; private set; } = false;
        public int killCount {  get; private set; }

        // Turn Point
        public bool hasTurnPoint { get; private set; } = false;

        // Name
        public bool hasName { get; private set; } = false;
        public string name { get; private set; }

        // PlayerType
        public bool hasPlayerType { get; private set; } = false;
        public string playerType { get; private set; }

        // Appearance
        public bool hasAppearance { get; private set; } = false;
        public string texture { get; private set; }

        // Position
        public bool hasPosition { get; private set; } = false;
        public Vector2 position { get; private set; }
        public float orientation { get; private set; }
        
        // size
        public bool hasSize { get; private set; } = false;
        public Vector2 size { get; private set; }

        // Movement
        public bool hasMovement { get; private set; } = false;
        public float moveRate { get; private set; }

        // Input
        public bool hasInput { get; private set; } = false;
        public List<Components.Input.Type> inputs { get; private set; }

        public override byte[] serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.serialize());
            data.AddRange(BitConverter.GetBytes(id));

            data.AddRange(BitConverter.GetBytes(hasSegment));

            data.AddRange(BitConverter.GetBytes(hasSnakeId));
            if (hasSnakeId)
            {
                data.AddRange(BitConverter.GetBytes(snakeId));
            }

            data.AddRange(BitConverter.GetBytes(hasScore));
            if (hasScore)
            {
                data.AddRange(BitConverter.GetBytes(score));
            }

            data.AddRange(BitConverter.GetBytes(hasKillCount));
            if (hasKillCount)
            {
                data.AddRange(BitConverter.GetBytes(killCount));
            }

            data.AddRange(BitConverter.GetBytes(hasTurnPoint));

            data.AddRange(BitConverter.GetBytes(hasFood));

            data.AddRange(BitConverter.GetBytes(hasName));
            if (hasName)
            {
                data.AddRange(BitConverter.GetBytes(name.Length));
                data.AddRange(Encoding.UTF8.GetBytes(name));
            }

            data.AddRange(BitConverter.GetBytes(hasPlayerType));
            if (hasPlayerType)
            {
                data.AddRange(BitConverter.GetBytes(playerType.Length));
                data.AddRange(Encoding.UTF8.GetBytes(playerType));
            }


            data.AddRange(BitConverter.GetBytes(hasAppearance));
            if (hasAppearance)
            {
                data.AddRange(BitConverter.GetBytes(texture.Length));
                data.AddRange(Encoding.UTF8.GetBytes(texture));
            }

            data.AddRange(BitConverter.GetBytes(hasPosition));
            if (hasPosition)
            {
                data.AddRange(BitConverter.GetBytes(position.X));
                data.AddRange(BitConverter.GetBytes(position.Y));
                data.AddRange(BitConverter.GetBytes(orientation));
            }

            data.AddRange(BitConverter.GetBytes(hasSize));
            if (hasSize)
            {
                data.AddRange(BitConverter.GetBytes(size.X));
                data.AddRange(BitConverter.GetBytes(size.Y));
            }

            data.AddRange(BitConverter.GetBytes(hasMovement));
            if (hasMovement)
            {
                data.AddRange(BitConverter.GetBytes(moveRate));
            }

            data.AddRange(BitConverter.GetBytes(hasInput));
            if (hasInput)
            {
                data.AddRange(BitConverter.GetBytes(inputs.Count));
                foreach (var input in inputs)
                {
                    data.AddRange(BitConverter.GetBytes((UInt16)input));
                }
            }

            return data.ToArray();
        }

        public override int parse(byte[] data)
        {
            int offset = base.parse(data);

            this.id = BitConverter.ToUInt32(data, offset);
            offset += sizeof(uint);

            this.hasSegment = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);

            this.hasSnakeId = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasSnakeId)
            {
                this.snakeId = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
            }

            this.hasScore = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasScore)
            {
                this.score = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
            }

            this.hasKillCount = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasKillCount)
            {
                this.killCount = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
            }

            this.hasTurnPoint = BitConverter.ToBoolean(data, offset);
            offset+= sizeof(bool);

            this.hasFood = BitConverter.ToBoolean(data, offset);
            offset+= sizeof(bool);

            this.hasName = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasName)
            {
                int nameSize = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
                this.name = Encoding.UTF8.GetString(data, offset, nameSize);
                offset += nameSize;
            }

            this.hasPlayerType = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasPlayerType)
            {
                int playerTypeSize = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
                this.playerType = Encoding.UTF8.GetString(data, offset, playerTypeSize);
                offset += playerTypeSize;
            }

            this.hasAppearance = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasAppearance)
            {
                int textureSize = BitConverter.ToInt32(data, offset);
                offset += sizeof(Int32);
                this.texture = Encoding.UTF8.GetString(data, offset, textureSize);
                offset += textureSize;
            }

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


            this.hasSize = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasSize)
            {
                float sizeX = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                float sizeY = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
                this.size = new Vector2(sizeX, sizeY);
            }

            this.hasMovement = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasMovement)
            {
                this.moveRate = BitConverter.ToSingle(data, offset);
                offset += sizeof(Single);
            }

            this.hasInput = BitConverter.ToBoolean(data, offset);
            offset += sizeof(bool);
            if (hasInput)
            {
                int howMany = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);
                for (int i = 0; i < howMany; i++)
                {
                    inputs.Add((Components.Input.Type)BitConverter.ToUInt16(data, offset));
                    offset += sizeof(UInt16);
                }
            }

            return offset;
        }
    }
}
