using CS5410;
using CS5410.Controls;
using CS5410.HighScores;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Client
{
    public class GameModel
    {
        private ContentManager m_contentManager;
        public Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();
        private Systems.Network m_systemNetwork = new Systems.Network();
        private Systems.KeyboardInput m_systemKeyboardInput;
        private Systems.Interpolation m_systemInterpolation = new Systems.Interpolation();
        private Systems.Renderer m_systemRenderer = new Systems.Renderer();
        private Shared.Systems.Movement m_systemMovement = new Shared.Systems.Movement();
        private SoundEffect m_crunch1;
        private SoundEffect m_crunch2;
        private SoundEffect m_crunch3;
        private SoundEffect m_crunch4;
        private SoundEffect m_crunch5;
        private SoundEffect m_crunch6;
        private SoundEffect m_crunch7;

        private SoundEffect m_grunt1;
        private SoundEffect m_grunt2;
        private SoundEffect m_grunt3;
        private SoundEffect m_grunt4;
        private SoundEffect m_grunt5;
        private SoundEffect m_grunt6;

        private Random random = new Random();

        private bool isFirstEntityReceived = false;

        public int WorldWidth {get; private set; }
        public int WorldHeight {get; private set;}

        /// <summary>
        /// This is where everything performs its update.
        /// </summary>
        public void update(TimeSpan elapsedTime)
        {
            m_systemNetwork.update(elapsedTime, MessageQueueClient.instance.getMessages());
            m_systemKeyboardInput.update(elapsedTime);
            m_systemInterpolation.update(elapsedTime);
            m_systemMovement.update(elapsedTime);
        }

        public void render(TimeSpan elapsedTime, SpriteBatch spriteBatch)
        {
            m_systemRenderer.update(elapsedTime, spriteBatch);
        }

        /// <summary>
        /// This is where all game model initialization occurs.  In the case
        /// of this "game', start by initializing the systems and then
        /// loading the art assets.
        /// </summary>
        public bool initialize(ContentManager contentManager, int screenWidth, int screenHeight)
        {
            m_contentManager = contentManager;
            
            m_crunch1 = contentManager.Load<SoundEffect>("Sounds/Food/crunch.1");
            m_crunch2 = contentManager.Load<SoundEffect>("Sounds/Food/crunch.2");
            m_crunch3 = contentManager.Load<SoundEffect>("Sounds/Food/crunch.3");
            m_crunch4 = contentManager.Load<SoundEffect>("Sounds/Food/crunch.4");
            m_crunch5 = contentManager.Load<SoundEffect>("Sounds/Food/crunch.5");
            m_crunch6 = contentManager.Load<SoundEffect>("Sounds/Food/crunch.6");
            m_crunch7 = contentManager.Load<SoundEffect>("Sounds/Food/crunch.7");

            m_grunt1 = contentManager.Load<SoundEffect>("Sounds/Death/3grunt1");
            m_grunt2 = contentManager.Load<SoundEffect>("Sounds/Death/3grunt2");
            m_grunt3 = contentManager.Load<SoundEffect>("Sounds/Death/3grunt3");
            m_grunt4 = contentManager.Load<SoundEffect>("Sounds/Death/3grunt4");
            m_grunt5 = contentManager.Load<SoundEffect>("Sounds/Death/3grunt5");
            m_grunt6 = contentManager.Load<SoundEffect>("Sounds/Death/3grunt6");

            m_systemRenderer.ContentManager = m_contentManager;
            
            m_systemNetwork.registerNewEntityHandler(handleNewEntity);
            m_systemNetwork.registerRemoveEntityHandler(handleRemoveEntity);

            WorldWidth = 750 * 3;
            WorldHeight = 750 * 3;

            m_systemKeyboardInput = new Systems.KeyboardInput(new List<Tuple<Shared.Components.Input.Type, Keys>>
            {
                Tuple.Create(Shared.Components.Input.Type.TurnUp, ControlsManager.Controls["TurnUp"]),
                Tuple.Create(Shared.Components.Input.Type.TurnDown, ControlsManager.Controls["TurnDown"]),
                Tuple.Create(Shared.Components.Input.Type.TurnLeft, ControlsManager.Controls["TurnLeft"]),
                Tuple.Create(Shared.Components.Input.Type.TurnRight, ControlsManager.Controls["TurnRight"])
            });

            ControlsManager.ControlsUpdated += UpdateSystemKeyboardInput;

            return true;
        }

        public void join(string name)
        {
            m_systemNetwork.join(name);
        }

        public void UpdateSystemKeyboardInput()
        {
            var newMappings = new List<Tuple<Shared.Components.Input.Type, Keys>>
            {
                Tuple.Create(Shared.Components.Input.Type.TurnUp, ControlsManager.Controls["TurnUp"]),
                Tuple.Create(Shared.Components.Input.Type.TurnDown, ControlsManager.Controls["TurnDown"]),
                Tuple.Create(Shared.Components.Input.Type.TurnLeft, ControlsManager.Controls["TurnLeft"]),
                Tuple.Create(Shared.Components.Input.Type.TurnRight, ControlsManager.Controls["TurnRight"])
            };

            m_systemKeyboardInput.UpdateControlMappings(newMappings);
        }

        public void shutdown()
        {

        }

        public void signalKeyPressed(Keys key)
        {
            m_systemKeyboardInput.keyPressed(key);
        }

        public void signalKeyReleased(Keys key)
        {
            m_systemKeyboardInput.keyReleased(key);
        }

        /// <summary>
        /// Based upon an Entity received from the server, create the
        /// entity at the client.
        /// </summary>
        private Entity createEntity(Shared.Messages.NewEntity message)
        {
            Entity entity = new Entity(message.id);

            if (message.hasSegment)
            {
                entity.add(new Shared.Components.Segment());
            }

            if (message.hasSnakeId)
            {
                entity.add(new Shared.Components.SnakeId(message.snakeId));
            }

            if (message.hasScore)
            {
                entity.add(new Shared.Components.Score(message.score));
            }

            if (message.hasKillCount)
            {
                entity.add(new Shared.Components.KillCount(message.killCount));
            }

            if (message.hasFood)
            {
                entity.add(new Shared.Components.Food());
            }

            if (message.hasTurnPoint)
            {
                entity.add(new Shared.Components.TurnPoint());
            }

            if (message.hasName)
            {
                entity.add(new Shared.Components.Name(message.name));
            }

            if (message.hasAppearance)
            {
                Texture2D texture = m_contentManager.Load<Texture2D>(message.texture);
                entity.add(new Components.Sprite(texture));
            }

            if (message.hasPosition)
            {
                entity.add(new Shared.Components.Position(message.position, message.orientation));
            }

            if (message.hasSize)
            {
                entity.add(new Shared.Components.Size(message.size));
            }

            if (message.hasMovement)
            {
                entity.add(new Shared.Components.Movement(message.moveRate));
            }

            if (message.hasInput)
            {
                entity.add(new Shared.Components.Input(message.inputs));
            }

            return entity;
        }
        
        private void RecordHighScore(Entity entity)
        {
            if (entity.contains<Shared.Components.Score>() && entity.contains<Shared.Components.Name>())
            {
                // int score = entity.get<Shared.Components.Score>().score;
                var score = m_systemNetwork.GetScore(entity.id);
                string name = entity.get<Shared.Components.Name>().name;
                HighScore newHighScore = new HighScore()
                {
                    Score = (uint)score,
                    PlayerName = name,
                    TimeStamp = DateTime.Now
                };
                HighScoreManager.AddHighScore(newHighScore);
            }
        }

        /// <summary>
        /// As entities are added to the game model, they are run by the systems
        /// to see if they are interested in knowing about them during their
        /// updates.
        /// </summary>
        private void addEntity(Entity entity)
        {
            if (entity == null)
            {
                return;
            }

            m_entities[entity.id] = entity;
            // if (!isFirstEntityReceived)
            // {
            //     isFirstEntityReceived = true;
            //     m_systemRenderer.m_playerId = entity.id;
            // }

            m_systemKeyboardInput.add(entity);
            m_systemRenderer.add(entity);
            m_systemNetwork.add(entity);
            m_systemInterpolation.add(entity);
            m_systemMovement.add(entity);
        }

        /// <summary>
        /// All entity lists for the systems must be given a chance to remove
        /// the entity.
        /// </summary>
        private void removeEntity(uint id)
        {

            if (m_entities.ContainsKey(id))
            {
                if (m_entities[id].contains<Shared.Components.Food>())
                {
                    int num = random.Next(1,8);
                    switch (num)
                    {
                        case 1: m_crunch1.Play();
                        break;
                        case 2: m_crunch2.Play();
                        break;
                        case 3: m_crunch3.Play();
                        break;
                        case 4: m_crunch4.Play();
                        break;
                        case 5: m_crunch5.Play();
                        break;
                        case 6: m_crunch6.Play();
                        break;
                        case 7: m_crunch7.Play();
                        break;
                    }
                }

                if (m_entities[id].contains<Shared.Components.SnakeId>())
                {
                    int num = random.Next(1,7);
                    switch (num)
                    {
                        case 1: m_grunt1.Play();
                        break;
                        case 2: m_grunt2.Play();
                        break;
                        case 3: m_grunt3.Play();
                        break;
                        case 4: m_grunt4.Play();
                        break;
                        case 5: m_grunt5.Play();
                        break;
                        case 6: m_grunt6.Play();
                        break;
                    }
                    RecordHighScore(m_entities[id]);
                }
                m_entities.Remove(id);

                m_systemKeyboardInput.remove(id);
                m_systemNetwork.remove(id);
                m_systemRenderer.remove(id);
                m_systemInterpolation.remove(id);
                m_systemMovement.remove(id);
            }
        }

        private void handleNewEntity(Shared.Messages.NewEntity message)
        {
            Entity entity = createEntity(message);
            addEntity(entity);
            if (!isFirstEntityReceived && message.hasSnakeId)
            {
                isFirstEntityReceived = true;
                m_systemRenderer.m_playerId = entity.id;
            }
        }


        /// <summary>
        /// Handler for the RemoveEntity message.  It removes the entity from
        /// the client game model (that's us!).
        /// </summary>
        private void handleRemoveEntity(Shared.Messages.RemoveEntity message)
        {
            removeEntity(message.id);
        }

    }
}
