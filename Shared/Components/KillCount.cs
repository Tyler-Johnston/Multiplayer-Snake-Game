namespace Shared.Components
{
    public class KillCount : Component
    {
        public KillCount(int count)
        {
            this.killCount = count;
        }

        public int killCount {  get; set; }
    }
}
