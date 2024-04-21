namespace Shared.Components
{
    public class FoodId : Component
    {
        public FoodId(int id)
        {
            this.id = id;
        }

        public int id {  get; private set; }
    }
}
