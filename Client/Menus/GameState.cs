using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Client;
using CS5410.Controls;
using CS5410.HighScores;

namespace CS5410
{
    public class GameState : Game
    {
        private GraphicsDeviceManager m_graphics;
        private IGameState m_currentState;
        private Dictionary<GameStateEnum, IGameState> m_states;

        public GameState()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            m_graphics.PreferredBackBufferWidth = 1300;
            m_graphics.PreferredBackBufferHeight = 750;

            m_graphics.ApplyChanges();

            // Create all the game states here
            m_states = new Dictionary<GameStateEnum, IGameState>
            {
                { GameStateEnum.MainMenu, new MainMenuView() },
                { GameStateEnum.GamePlay, new GamePlayView() },
                { GameStateEnum.HighScores, new HighScoresView() },
                { GameStateEnum.CustomizeControls, new CustomizeControlsView() },
                { GameStateEnum.InitialSetup, new InitialSetupView() },
                { GameStateEnum.Credits, new CreditsView() }
            };

            // Give all game states a chance to initialize, other than constructor
            foreach (var item in m_states)
            {
                item.Value.initialize(this.GraphicsDevice, m_graphics);
            }

            // pass in the screen height and width to gameplay view so it will be used for the world size in game model
            ((GamePlayView)m_states[GameStateEnum.GamePlay]).ScreenHeight = m_graphics.PreferredBackBufferHeight;
            ((GamePlayView)m_states[GameStateEnum.GamePlay]).ScreenWidth = m_graphics.PreferredBackBufferWidth;

            // We are starting with the main menu
            m_currentState = m_states[GameStateEnum.MainMenu];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Give all game states a chance to load their content
            foreach (var item in m_states)
            {
                item.Value.loadContent(this.Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            GameStateEnum nextStateEnum = m_currentState.processInput(gameTime);

            // Special case; after initial set up view is done, let gameplayview know about it so it doesnt render that anymore
            if (nextStateEnum == GameStateEnum.GamePlay && m_currentState is InitialSetupView setupView && setupView.IsSetupFinished)
            {
                GamePlayView play = (GamePlayView)m_states[GameStateEnum.GamePlay];
                InitialSetupView view = (InitialSetupView)m_states[GameStateEnum.InitialSetup];

                play.m_name = view.playerName;
                
                ((GamePlayView)m_states[GameStateEnum.GamePlay]).InitialSetUpCompleted = true;
            }

            // Special case for exiting the game
            if (nextStateEnum == GameStateEnum.Exit)
            {
                MessageQueueClient.instance.sendMessage(new Shared.Messages.Disconnect());
                MessageQueueClient.instance.shutdown();
                Exit();
            } 
            else
            {
                m_currentState.update(gameTime);
                // reload content?
                if (m_currentState != m_states[nextStateEnum] && nextStateEnum == GameStateEnum.GamePlay)
                {
                    m_states[nextStateEnum].loadContent(Content);
                }
                m_currentState = m_states[nextStateEnum];
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            m_currentState.render(gameTime);

            base.Draw(gameTime);
        }
    }
}
