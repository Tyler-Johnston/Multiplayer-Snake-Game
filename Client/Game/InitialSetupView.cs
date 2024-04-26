using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using CS5410.Controls;

namespace CS5410
{
    public class InitialSetupView : GameStateView
    {
        private SpriteFont font;
        private int currentStep = 0;
        private bool m_finished = false;
        private bool m_waitForKeyRelease = false;
        public string playerName = "";
        public string controlScheme = "";
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
                if (currentStep == 0)
                {
                    foreach (var key in pressedKeys)
                    {
                        if (key == Keys.Back && playerName.Length > 0)
                        {
                            playerName = playerName[..^1];
                            m_waitForKeyRelease = true;
                        }
                        else if (key >= Keys.A && key <= Keys.Z && playerName.Length < 6)
                        {
                            char keyChar = (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)) ? key.ToString()[0] : char.ToLower(key.ToString()[0]);
                            playerName += keyChar;
                            m_waitForKeyRelease = true;
                        }
                    }
                }
                else if (currentStep == 1)
                {
                    if (keyboardState.IsKeyDown(Keys.M))
                    {
                        controlScheme = "Mouse";
                        currentStep++;
                        m_waitForKeyRelease = true;
                    }
                    else if (keyboardState.IsKeyDown(Keys.K))
                    {
                        controlScheme = "Keyboard";
                        currentStep++;
                        m_waitForKeyRelease = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && (currentStep > 1 || playerName.Length > 0))
                {
                    currentStep++;
                    if (currentStep > 2)
                    {
                        m_finished = true;
                        return GameStateEnum.GamePlay;
                    }
                    m_waitForKeyRelease = true;
                }
                else if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    return GameStateEnum.MainMenu;
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
            Vector2 stringSize;

            switch (currentStep)
            {
                case 0:
                    message = "Enter Name:";
                    break;
                case 1:
                    message = "Press M for mouse controls. Press K for keyboard controls";
                    break;
                case 2:
                    message = $"Snake changes direction with {controlScheme} controls.\nArrow keys are used for this by default.";
                    break;
            }
            stringSize = font.MeasureString(message);
            m_spriteBatch.DrawString(font, message,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2,
                            m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y * 2),
                Color.White);

            if (currentStep == 0)
            {
                string nameMessage = $"{playerName}";
                Vector2 nameSize = font.MeasureString(nameMessage);
                m_spriteBatch.DrawString(font, nameMessage,
                    new Vector2(m_graphics.PreferredBackBufferWidth / 2 - nameSize.X / 2,
                                m_graphics.PreferredBackBufferHeight / 2 - nameSize.Y / 2),
                    Color.LightGreen);
            }
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
