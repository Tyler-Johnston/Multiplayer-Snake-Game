﻿
using Microsoft.Xna.Framework;
using Shared.Components;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Client.Systems
{
    public class Network : Shared.Systems.System
    {
        public delegate void Handler(TimeSpan elapsedTime, Shared.Messages.Message message);
        public delegate void RemoveEntityHandler(RemoveEntity message);
        public delegate void NewEntityHandler(NewEntity message);

        private Dictionary<Shared.Messages.Type, Handler> m_commandMap = new Dictionary<Shared.Messages.Type, Handler>();
        private RemoveEntityHandler m_removeEntityHandler;
        private NewEntityHandler m_newEntityHandler;
        private uint m_lastMessageId = 0;
        private HashSet<uint> m_updatedEntities = new HashSet<uint>();

        /// <summary>
        /// Primary activity in the constructor is to setup the command map
        // that maps from message types to their handlers.
        /// </summary>
        public Network() :
            base(typeof(Shared.Components.Position))
        {

            registerHandler(Shared.Messages.Type.ConnectAck, (TimeSpan elapsedTime, Message message) =>
            {
                handleConnectAck(elapsedTime, (ConnectAck)message);
            });

            registerHandler(Shared.Messages.Type.NewEntity, (TimeSpan elapsedTime, Message message) =>
            {
                m_newEntityHandler((NewEntity)message);
            });

            registerHandler(Shared.Messages.Type.UpdateEntity, (TimeSpan elapsedTime, Message message) =>
            {
                handleUpdateEntity(elapsedTime, (UpdateEntity)message);
            });

            registerHandler(Shared.Messages.Type.RemoveEntity, (TimeSpan elapsedTime, Message message) =>
            {
                m_removeEntityHandler((RemoveEntity)message);
            });
        }

        // Have to implement this because it is abstract in the base class
        public override void update(TimeSpan elapsedTime) { }

        /// <summary>
        /// Have our own version of update, because we need a list of messages to work with, and
        /// messages aren't entities.
        /// </summary>
        public void update(TimeSpan elapsedTime, Queue<Message> messages)
        {
            m_updatedEntities.Clear();

            if (messages != null)
            {
                while (messages.Count > 0)
                {
                    var message = messages.Dequeue();
                    if (m_commandMap.ContainsKey(message.type))
                    {
                        m_commandMap[message.type](elapsedTime, message);
                    }

                    if (message.messageId.HasValue)
                    {
                        m_lastMessageId = message.messageId.Value;
                    }
                }
            }

            // After processing all the messages, perform server reconciliation by
            // resimulating the inputs from any sent messages not yet acknowledged by the server.
            var sent = MessageQueueClient.instance.getSendMessageHistory(m_lastMessageId);
            while (sent.Count > 0)
            {
                var message = (Shared.Messages.Input)sent.Dequeue();
                if (message.type == Shared.Messages.Type.Input)
                {
                    var entity = m_entities[message.entityId];
                    if (m_updatedEntities.Contains(entity.id))
                    {
                        foreach (var input in message.inputs)
                        {
                            switch (input)
                            {
                                // case Shared.Components.Input.Type.Thrust:
                                //     Shared.Entities.Utility.thrust(entity, message.elapsedTime);
                                //     break;
                                case Shared.Components.Input.Type.TurnLeft:
                                    Shared.Entities.Utility.turnLeft(entity);
                                    break;
                                case Shared.Components.Input.Type.TurnRight:
                                    Shared.Entities.Utility.turnRight(entity);
                                    break;
                                case Shared.Components.Input.Type.TurnUp:
                                    Shared.Entities.Utility.turnUp(entity);
                                    break;
                                case Shared.Components.Input.Type.TurnDown:
                                    Shared.Entities.Utility.turnDown(entity);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void registerHandler(Shared.Messages.Type type, Handler handler)
        {
            m_commandMap[type] = handler;
        }

        public void registerNewEntityHandler(NewEntityHandler handler)
        {
            m_newEntityHandler = handler;
        }

        public void registerRemoveEntityHandler(RemoveEntityHandler handler)
        {
            m_removeEntityHandler = handler;
        }

        /// <summary>
        /// Handler for the ConnectAck message.  This records the clientId
        /// assigned to it by the server, it also sends a request to the server
        /// to join the game.
        /// </summary>
        private void handleConnectAck(TimeSpan elapsedTime, ConnectAck message) 
        {
        }

        public void join(string name)
        {
            MessageQueueClient.instance.sendMessage(new Join(name));
        }

        /// <summary>
        /// Handler for the UpdateEntity message.  It checks to see if the client
        /// actually has the entity, and if it does, updates the components
        /// that are in common between the message and the entity.
        /// </summary>
        private void handleUpdateEntity(TimeSpan elapsedTime, UpdateEntity message) 
        { 
            if (m_entities.ContainsKey(message.id))
            {
                var entity = m_entities[message.id];
                if (entity.contains<Components.Goal>() && message.hasPosition)
                {
                    var position = entity.get<Position>();
                    var goal = entity.get<Components.Goal>();

                    goal.updateWindow = message.updateWindow;
                    goal.updatedTime = TimeSpan.Zero;
                    goal.goalPosition = new Vector2(message.position.X, message.position.Y);
                    goal.goalOrientation = message.orientation;

                    goal.startPosition = position.position;
                    goal.startOrientation = position.orientation;
                }
                else if (entity.contains<Position>() && message.hasPosition)
                {
                    entity.get<Position>().position = message.position;
                    entity.get<Position>().orientation = message.orientation;

                    m_updatedEntities.Add(entity.id);
                }
            }
        }
    }
}
