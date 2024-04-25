
namespace Shared.Components
{
    public class Input : Component
    {
        public enum Type : UInt16
        {
            TurnLeft,
            TurnRight,
            TurnUp,
            TurnDown,
            TurnUpRight,
            TurnUpLeft,
            TurnDownLeft,
            TurnDownRight
        }

        public Input(List<Type> inputs)
        {
            this.inputs = inputs;
        }

        public List<Type> inputs { get; private set; }
    }
}
