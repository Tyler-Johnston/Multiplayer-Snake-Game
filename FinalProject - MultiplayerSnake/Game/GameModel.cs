﻿using CS5410;
using CS5410.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared.Entities;
using System;
using System.Collections.Generic;

namespace Client
{
    public class GameModel
    {
        private ContentManager m_contentManager;
        private Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();
        private Systems.Network m_systemNetwork = new Systems.Network();
        private Systems.KeyboardInput m_systemKeyboardInput;
        private Systems.Interpolation m_systemInterpolation = new Systems.Interpolation();
        private Systems.Renderer m_systemRenderer = new Systems.Renderer();

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


            m_systemRenderer.ContentManager = m_contentManager;
            
            m_systemNetwork.registerNewEntityHandler(handleNewEntity);
            m_systemNetwork.registerRemoveEntityHandler(handleRemoveEntity);

            WorldWidth = screenWidth * 3;
            WorldHeight = screenHeight * 3;

            m_systemKeyboardInput = new Systems.KeyboardInput(new List<Tuple<Shared.Components.Input.Type, Keys>>
            {
                Tuple.Create(Shared.Components.Input.Type.Thrust, ControlsManager.Controls["Thrust"]),
                Tuple.Create(Shared.Components.Input.Type.RotateLeft, ControlsManager.Controls["RotateLeft"]),
                Tuple.Create(Shared.Components.Input.Type.RotateRight, ControlsManager.Controls["RotateRight"])
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
                Tuple.Create(Shared.Components.Input.Type.Thrust, ControlsManager.Controls["Thrust"]),
                Tuple.Create(Shared.Components.Input.Type.RotateLeft, ControlsManager.Controls["RotateLeft"]),
                Tuple.Create(Shared.Components.Input.Type.RotateRight, ControlsManager.Controls["RotateRight"])
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
                entity.add(new Shared.Components.Movement(message.moveRate, message.rotateRate));
            }

            if (message.hasInput)
            {
                entity.add(new Shared.Components.Input(message.inputs));
            }

            return entity;
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
            m_systemKeyboardInput.add(entity);
            m_systemRenderer.add(entity);
            m_systemNetwork.add(entity);
            m_systemInterpolation.add(entity);
        }

        /// <summary>
        /// All entity lists for the systems must be given a chance to remove
        /// the entity.
        /// </summary>
        private void removeEntity(uint id)
        {
            m_entities.Remove(id);

            m_systemKeyboardInput.remove(id);
            m_systemNetwork.remove(id);
            m_systemRenderer.remove(id);
            m_systemInterpolation.remove(id);
        }

        private void handleNewEntity(Shared.Messages.NewEntity message)
        {
            Entity entity = createEntity(message);
            addEntity(entity);
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
