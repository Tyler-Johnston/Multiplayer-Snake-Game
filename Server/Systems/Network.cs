using Microsoft.Xna.Framework;
using Shared.Entities;
using Shared.Messages;

namespace Server.Systems
{
    public class Network : Shared.Systems.System
    {
        public delegate void Handler(int clientId, TimeSpan elapsedTime, Shared.Messages.Message message);
        public delegate void DisconnectHandler(int clientId);
        public delegate void InputHandler(Entity entity, Shared.Components.Input.Type type, TimeSpan elapsedTime);

        private Dictionary<Shared.Messages.Type, Handler> m_commandMap = new Dictionary<Shared.Messages.Type, Handler>();
        private DisconnectHandler m_disconnectHandler;

        private HashSet<uint> m_reportThese = new HashSet<uint>();

        /// <summary>
        /// Primary activity in the constructor is to setup the command map
        /// that maps from message types to their handlers.
        /// </summary>
        public Network() :
            base(
                typeof(Shared.Components.Movement),
                typeof(Shared.Components.Position)
            )
        {

            // Register our own disconnect handler
            registerHandler(Shared.Messages.Type.Disconnect, (int clientId, TimeSpan elapsedTime, Shared.Messages.Message message) =>
            {
                if (m_disconnectHandler != null)
                {
                    m_disconnectHandler(clientId);
                }
            });

            // Register our own input handler
            registerHandler(Shared.Messages.Type.Input, (int clientId, TimeSpan elapsedTime, Shared.Messages.Message message) =>
            {
                handleInput((Shared.Messages.Input)message);
            });
        }

        // Have to implement this because it is abstract in the base class
        public override void update(TimeSpan elapsedTime) { }

        /// <summary>
        /// Have our own version of update, because we need a list of messages to work with, and
        /// messages aren't entities.
        /// </summary>
        public void update(TimeSpan elapsedTime, Queue<Tuple<int, Message>> messages)
        {
            if (messages != null)
            {
                while (messages.Count > 0)
                {
                    var message = messages.Dequeue();
                    if (m_commandMap.ContainsKey(message.Item2.type))
                    {
                        m_commandMap[message.Item2.type](message.Item1, elapsedTime, message.Item2);
                    }
                }
            }

            // Send updated game state updates back out to connected clients
            updateClients(elapsedTime);
        }

        public void registerDisconnectHandler(DisconnectHandler handler)
        {
            m_disconnectHandler = handler;
        }

        public void registerHandler(Shared.Messages.Type type, Handler handler)
        {
            m_commandMap[type] = handler;
        }

        /// <summary>
        /// Handler for the Input message.  This simply passes the responsibility
        /// to the registered input handler.
        /// </summary>
        /// <param name="message"></param>
        private void handleInput(Shared.Messages.Input message)
        {
            var entity = m_entities[message.entityId];
            foreach (var input in message.inputs)
            {
                switch (input)
                {
                    case Shared.Components.Input.Type.TurnLeft:
                        Shared.Entities.Utility.turnLeft(entity, message.elapsedTime);
                        m_reportThese.Add(message.entityId);
                        break;
                    case Shared.Components.Input.Type.TurnRight:
                        Shared.Entities.Utility.turnRight(entity, message.elapsedTime);
                        m_reportThese.Add(message.entityId);
                        break;
                    case Shared.Components.Input.Type.TurnUp:
                        Shared.Entities.Utility.turnUp(entity, message.elapsedTime);
                        m_reportThese.Add(message.entityId);
                        break;
                    case Shared.Components.Input.Type.TurnDown:
                        Shared.Entities.Utility.turnDown(entity, message.elapsedTime);
                        m_reportThese.Add(message.entityId);
                        break;
                    case Shared.Components.Input.Type.TurnUpRight:
                        Shared.Entities.Utility.turnUpRight(entity, message.elapsedTime);
                        break;
                    case Shared.Components.Input.Type.TurnDownRight:
                        Shared.Entities.Utility.turnDownRight(entity, message.elapsedTime);
                        break;
                    case Shared.Components.Input.Type.TurnUpLeft:
                        Shared.Entities.Utility.turnUpLeft(entity, message.elapsedTime);
                        break;
                    case Shared.Components.Input.Type.TurnDownLeft:
                        Shared.Entities.Utility.turnDownLeft(entity, message.elapsedTime);
                        break;
                }
            }
        }

        /// <summary>
        /// For the entities that have updates, send those updates to all
        /// connected clients.
        /// </summary>
        private void updateClients(TimeSpan elapsedTime)
        {
            foreach (var entityId in m_reportThese)
            {
                var entity = m_entities[entityId];
                var message = new Shared.Messages.UpdateEntity(entity, elapsedTime);
                MessageQueueServer.instance.broadcastMessageWithLastId(message);
            }

            m_reportThese.Clear();
        }
    }
}
