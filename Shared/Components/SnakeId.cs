namespace Shared.Components
{
    public class SnakeId : Component
    {
        public SnakeId(int id)
        {
            this.id = id;
        }

        public int id { get; private set; }
    }
}
