namespace Shared.Components
{
    public class Score : Component
    {
        public Score(int score)
        {
            this.score = score;
        }

        public int score {  get; set; }
    }
}
