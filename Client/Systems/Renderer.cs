
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Entities;
using System;
using Microsoft.Xna.Framework.Content;
using Shared.Components;
using System.Linq;


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
        private uint? playerScore = null;
        private uint? killCount = null;
        private uint? highestPosition = null;
        // private ParticleSystem m_particleSystemFood;
        // private ParticleSystemRenderer m_renderCatch;

        public ContentManager ContentManager
        {
            set 
            { 
                m_contentManager = value;
                InitializeContent();
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

        private void InitializeContent()
        {
            if (m_contentManager != null)
            {
                m_background = m_contentManager.Load<Texture2D>("background");
                m_font = m_contentManager.Load<SpriteFont>("Fonts/menu");
                
                // m_particleSystemFood = new ParticleSystem(
                //     new Vector2(300, 300), // Assuming starting position
                //     10, // Max particles
                //     5, // Initial particles
                //     0.1f, // Birth rate
                //     0.05f, // Death rate
                //     500, // Max life of a particle
                //     100); // Min life of a particle

                // m_renderCatch = new ParticleSystemRenderer("Textures/particle");
                // m_renderCatch.LoadContent(contentManager);
            }
        }

        public override void update(TimeSpan elapsedTime) { }

        public void PrintEntityInfo(Entity entity)
{
    if (entity == null)
    {
        Console.WriteLine("Entity is null.");
        return;
    }

    Console.WriteLine("Entity Information:");
    Console.WriteLine($"ID: {entity.id}");

    if (entity.contains<Shared.Components.Name>())
    {
        var name = entity.get<Shared.Components.Name>().name;
        Console.WriteLine($"Name: {name}");
    }

    // Print Position Information
    if (entity.contains<Shared.Components.Position>())
    {
        var position = entity.get<Shared.Components.Position>();
        Console.WriteLine($"Position: X={position.position.X}, Y={position.position.Y}");
    }

    // Print Score Information
    if (entity.contains<Shared.Components.Score>())
    {
        var score = entity.get<Shared.Components.Score>();
        Console.WriteLine($"Score: {score.score}");
    }

    // Print PlayerType
    if (entity.contains<Shared.Components.PlayerType>())
    {
        var pt = entity.get<Shared.Components.PlayerType>();
        Console.WriteLine($"Player Type: {pt.playerType}");
    }

    // Print Sprite Information
    if (entity.contains<Client.Components.Sprite>())
    {
        var sprite = entity.get<Client.Components.Sprite>();
        Console.WriteLine($"Sprite Texture: {sprite.texture}"); // Adjust depending on how texture is stored
    }

    // Additional components can be added here in a similar fashion
}


        public void update(TimeSpan elapsedTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            updateViewport();

            spriteBatch.Draw(m_background, new Rectangle(-m_viewportOffsetX, -m_viewportOffsetY, WorldWidth, WorldHeight), Color.White);
            foreach (Entity entity in m_entities.Values)
            {
                RenderEntity(entity, spriteBatch);
            }

            if (m_playerId.HasValue && m_entities.ContainsKey(m_playerId.Value))
            {
                Entity player = m_entities[m_playerId.Value];
                PrintEntityInfo(player);
                if (player.contains<Score>() && player.contains<KillCount>())
                {
                    playerScore = (uint)player.get<Score>().score;
                    killCount = (uint)player.get<KillCount>().killCount;
                    UpdateHighestPosition(player);
                    Vector2 scorePosition = new Vector2(50, 50);
                    spriteBatch.DrawString(m_font, $"Score: {playerScore}", scorePosition, Color.White);
                }
            }
            else if (playerScore != null && killCount != null)
            {
                spriteBatch.DrawString(m_font, $"Final Score: {playerScore.Value}", new Vector2(550,250), Color.White);
                spriteBatch.DrawString(m_font, $"Kill Count: {killCount.Value}", new Vector2(550,315), Color.White);
                spriteBatch.DrawString(m_font, $"Highest Position: {highestPosition.Value}", new Vector2(550,380), Color.White);
            }

            RenderScoreBoard(spriteBatch);
            // m_particleSystemCatch.update(gameTime);
            spriteBatch.End();
        }

        private void UpdateHighestPosition(Entity player)
        {
            var currentRank = m_entities.Values
                .Where(e => e.contains<Score>())
                .OrderByDescending(e => e.get<Score>().score)
                .ToList().FindIndex(e => e == player) + 1;

            if (!highestPosition.HasValue || currentRank < highestPosition.Value)
            {
                highestPosition = (uint?)currentRank;
            }
        }

        private void RenderScoreBoard(SpriteBatch spriteBatch)
        {
            var topEntities = m_entities.Values
                .Where(e => e.contains<Score>())
                .OrderByDescending(e => e.get<Score>().score)
                .Take(5);

            int rank = 1;
            Vector2 scorePosition = new Vector2(1025, 50);
            foreach (var entity in topEntities)
            {
                string scoreText = $"{rank++}) {entity.get<Name>()?.name ?? "Unknown"}: {entity.get<Score>().score}";
                spriteBatch.DrawString(m_font, scoreText, scorePosition, Color.White);
                scorePosition.Y += 60;
            }
        }

        public void updateViewport()
        {
            if (m_playerId != null && m_entities.ContainsKey(m_playerId.Value))
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

            // m_renderCatch.draw(m_spriteBatch, m_particleSystemCatch);
        }
    }
}
