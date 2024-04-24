namespace Shared.Components
{
    public class PlayerType : Component
    {
        public PlayerType(string playerType)
        {
            this.playerType = playerType;
        }

        public string playerType {  get; set; }
    }
}
