
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
        private Texture2D m_background;
        private ContentManager m_contentManager;
        private int m_viewportOffsetX = 0;
        private int m_viewportOffsetY = 0;
        int VPW = 1300;
        int VPH = 750;

        private int WorldHeight = 750 * 3;
        private int WorldWidth = 750 * 3;     
        public uint? m_playerId = null;

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
                typeof(Shared.Components.Size)
                )
        {
        }

        private void InitializeFont()
        {
            if (m_contentManager != null)
            {
                m_background = m_contentManager.Load<Texture2D>("background");
                m_font = m_contentManager.Load<SpriteFont>("Fonts/menu");
            }
        }

        public override void update(TimeSpan elapsedTime) { }

        public void update(TimeSpan elapsedTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            updateViewport();

            spriteBatch.Draw(m_background, new Rectangle(-m_viewportOffsetX, -m_viewportOffsetY, WorldWidth, WorldHeight), Color.White);
            foreach (Entity entity in m_entities.Values)
            {
                RenderEntity(entity, spriteBatch);
            }

            spriteBatch.End();
        }

        public void updateViewport()
        {
            if (m_playerId != null)
            {
                Entity entity = m_entities[m_playerId.Value];
                var position = entity.get<Shared.Components.Position>().position;
                var orientation = entity.get<Shared.Components.Position>().orientation;
                var size = entity.get<Shared.Components.Size>().size;
                var texture = entity.get<Components.Sprite>().texture;
                var texCenter = entity.get<Components.Sprite>().center;

                m_viewportOffsetX = (int)Math.Min(Math.Max(position.X - VPW / 2, 0), WorldWidth - VPW);
                m_viewportOffsetY = (int)Math.Min(Math.Max(position.Y - VPH / 2, 0), WorldWidth - VPH);

                // Build a rectangle centered at position, with width/height of size
                int playerX = (m_viewportOffsetX == 0 || m_viewportOffsetX == WorldWidth - VPW) ? (int)position.X : (int)(VPW / 2);
                int playerY = (m_viewportOffsetY == 0 || m_viewportOffsetY == WorldWidth - VPH) ? (int)position.Y : (int)(VPH / 2);
                int viewportMaxXThreshold = WorldWidth - VPW / 2;
                int playerXOffset = WorldWidth - VPW;
                if (playerX > viewportMaxXThreshold)
                {
                    playerX = playerX - playerXOffset;
                }
                int viewportMaxYThreshold = WorldWidth - VPH / 2;
                int playerYOffset = WorldWidth - VPH;
                if (playerY > viewportMaxYThreshold)
                {
                    playerY = playerY - playerYOffset;
                }
            }
        }

        public void RenderEntity(Entity entity, SpriteBatch spriteBatch)
        {
            var pos = entity.get<Shared.Components.Position>().position;
            var orientation = entity.get<Shared.Components.Position>().orientation;
            int entityX = (int)(pos.X - m_viewportOffsetX);
            int entityY = (int)(pos.Y - m_viewportOffsetY);
            var size = entity.get<Shared.Components.Size>().size;
            var texture = entity.get<Components.Sprite>().texture;
            var texCenter = entity.get<Components.Sprite>().center;

            Rectangle rectangle = new Rectangle(
                entityX,
                entityY,
                (int)size.X,
                (int)size.Y
            );

            spriteBatch.Draw(
                texture, 
                rectangle, 
                null,
                Color.White,
                orientation,
                texCenter,
                SpriteEffects.None,
                0
            );

            if (entity.contains<Shared.Components.Name>())
            {
                var name = entity.get<Shared.Components.Name>().name;
                // Calculate the position for the name text to appear above the entity
                Vector2 textPosition = new Vector2(
                    entityX, 
                    entityY - 70);

                // Draw the name text above the entity
                spriteBatch.DrawString(m_font, name, textPosition, Color.White);
            }
            
        }
    }
}
