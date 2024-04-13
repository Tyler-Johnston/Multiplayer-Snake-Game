
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Entities;
using System;
using Microsoft.Xna.Framework.Content;

namespace Client.Systems
{
    public class Renderer : Shared.Systems.System
    {

        private SpriteFont m_font;
        private ContentManager m_contentManager;

        public ContentManager ContentManager
        {
            set 
            { 
                m_contentManager = value;
                InitializeFont();
            }
        }


        public Renderer() :
            base(
                typeof(Client.Components.Sprite),
                typeof(Shared.Components.Position),
                typeof(Shared.Components.Size),
                typeof(Shared.Components.Name)
                )
        {
        }

        private void InitializeFont()
        {
            if (m_contentManager != null)
            {
                m_font = m_contentManager.Load<SpriteFont>("Fonts/menu");
            }
        }

        public override void update(TimeSpan elapsedTime) { }

        public void update(TimeSpan elapsedTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();

            foreach (Entity entity in m_entities.Values)
            {
                var position = entity.get<Shared.Components.Position>().position;
                var orientation = entity.get<Shared.Components.Position>().orientation;
                var name = entity.get<Shared.Components.Name>().name;
                var size = entity.get<Shared.Components.Size>().size;
                var texture = entity.get<Components.Sprite>().texture;
                var texCenter = entity.get<Components.Sprite>().center;

                // Build a rectangle centered at position, with width/height of size
                Rectangle rectangle = new Rectangle(
                    (int)(position.X - size.X / 2),
                    (int)(position.Y - size.Y / 2),
                    (int)size.X,
                    (int)size.Y);

                spriteBatch.Draw(
                    texture, 
                    rectangle, 
                    null,
                    Color.White,
                    orientation,
                    texCenter,
                    SpriteEffects.None,
                    0);

                // Calculate the position for the name text to appear above the entity
                Vector2 textPosition = new Vector2(position.X - m_font.MeasureString(name).X / 2, position.Y - size.Y / 2 - 70);

                // Draw the name text above the entity
                spriteBatch.DrawString(m_font, name, textPosition, Color.White);
            }

            spriteBatch.End();
        }
    }
}
