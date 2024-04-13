using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Client;
using Microsoft.Xna.Framework.Media;
using System;

namespace CS5410
{
    public class GamePlayView : GameStateView
    {
        private bool m_loaded = false;
        public string m_name = "None";
        private uint m_playerId = 9;
        private Texture2D m_background;
        private bool initialSetupCompleted = false;
        private SpriteFont m_font;
        private GameModel m_gameModel = new GameModel();
        private Song m_music;
        private bool m_joined = false;

        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int m_viewportOffsetX = 0;
        public int m_viewportOffsetY = 0;

        public bool InitialSetUpCompleted
        {
            get { return initialSetupCompleted; }
            set { initialSetupCompleted = value; }
        }

        public override void loadContent(ContentManager contentManager)
        {
            if (!m_loaded)
            {
                MessageQueueClient.instance.initialize("localhost", 3000);
                m_gameModel.initialize(contentManager, ScreenWidth, ScreenHeight);
                m_font = contentManager.Load<SpriteFont>("Fonts/menu");
                m_background = contentManager.Load<Texture2D>("background");
                m_music = contentManager.Load<Song>("Sounds/Riverside Ride Long Loop");
                MediaPlayer.Play(m_music);
                MediaPlayer.IsRepeating = true;
                m_loaded = true;
            }
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            if (!initialSetupCompleted)
            {
                return GameStateEnum.InitialSetup;
            }

            foreach (var key in m_previouslyDown)
            {
                if (Keyboard.GetState().IsKeyUp(key))
                {
                    m_gameModel.signalKeyReleased(key);
                    m_previouslyDown.Remove(key);
                }
            }

            foreach (var key in Keyboard.GetState().GetPressedKeys())
            {
                if (!m_previouslyDown.Contains(key))
                {
                    m_gameModel.signalKeyPressed(key);
                    m_previouslyDown.Add(key);
                }
            }
            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            if (initialSetupCompleted)
            {
                m_spriteBatch.Begin();
<<<<<<< HEAD
                m_spriteBatch.Draw(m_background, new Rectangle(0, 0, m_gameModel.WorldWidth, m_gameModel.WorldHeight), Color.White);
=======

>>>>>>> liam.viewport
                m_spriteBatch.End();
                m_gameModel.render(gameTime.ElapsedGameTime, m_spriteBatch);
                
            }
        }
        private HashSet<Keys> m_previouslyDown = new HashSet<Keys>();
        public override void update(GameTime gameTime)
        {
            m_gameModel.update(gameTime.ElapsedGameTime);
            if (!m_joined && initialSetupCompleted)
            {
                m_gameModel.join(m_name);
                m_joined = true;
            }
        }
    }
}