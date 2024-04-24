namespace Shared.Components
{
    public class FoodSpriteType : Component
    {
        public FoodSpriteType(string FoodSpriteType)
        {
            this.foodSpriteType = FoodSpriteType;
        }

        public string foodSpriteType {  get; set; }
    }
}
