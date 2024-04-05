using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CS5410
{
    public class InitialSetupView : GameStateView
    {
        private SpriteFont font;
        private int currentStep = 0;
        private bool m_finished = false;
        private bool m_waitForKeyRelease = false;

        
        public bool IsSetupFinished
        {
            get { return m_finished; }
        }
        
        public override void loadContent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/menu");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();

            if (!m_waitForKeyRelease)
            {
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    currentStep++;
                    if (currentStep > 3)
                    {
                        m_finished = true;
                        return GameStateEnum.GamePlay;
                    }
                    m_waitForKeyRelease = true;
                }
            }

            if (m_waitForKeyRelease && pressedKeys.Length == 0)
            {
                m_waitForKeyRelease = false;
            }

            return GameStateEnum.InitialSetup;
        }


        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            string message = "";
            switch (currentStep)
            {
                case 0:
                    message = "Enter Name: [Use Keyboard]";
                    break;
                case 1:
                    message = "Choose Input Method: Mouse/Keyboard";
                    break;
                case 2:
                    message = "Tutorial: Snake changes direction with keypress";
                    break;
                case 3:
                    message = "Press Space to Join Game!";
                    break;
            }
            Vector2 stringSize = font.MeasureString(message);
            m_spriteBatch.DrawString(font, message,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2,
                            m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y),
                Color.White);
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
