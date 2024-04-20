
using Microsoft.Xna.Framework;
using Shared.Components;
using Shared.Entities;
using Shared.Messages;
using System;

namespace Server
{
    public class GameModel
    {
        private int m_nextSnakeId = 0;
        private HashSet<int> m_clients = new HashSet<int>();
        private Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();
        private Dictionary<int, uint> m_clientToEntityId = new Dictionary<int, uint>();

        Systems.Network m_systemNetwork = new Server.Systems.Network();
        Shared.Systems.Movement m_systemMovement = new Shared.Systems.Movement();

        /// <summary>
        /// This is where the server-side simulation takes place.  Messages
        /// from the network are processed and then any necessary client
        /// updates are sent out.
        /// </summary>
        public void update(TimeSpan elapsedTime)
        {
            m_systemNetwork.update(elapsedTime, MessageQueueServer.instance.getMessages());
            m_systemMovement.update(elapsedTime);
        }

        /// <summary>
        /// Setup notifications for when new clients connect.
        /// </summary>
        public bool initialize()
        {
            m_systemNetwork.registerHandler(Shared.Messages.Type.Join, handleJoin);
            m_systemNetwork.registerDisconnectHandler(handleDisconnect);

            // Entity food = Shared.Entities.Food.create("Textures/egg", new Vector2(120, 100), 50);
            // var message = new Shared.Messages.NewEntity(food);
            // addEntity(food);
            // MessageQueueServer.instance.broadcastMessage(message);

            // Entity food2 = Shared.Entities.Food.create("Textures/egg", new Vector2(120, 120), 50);
            // var message2 = new Shared.Messages.NewEntity(food2);
            // MessageQueueServer.instance.broadcastMessage(message2);

            // Entity food = Shared.Entities.Food.create("Textures/egg", new Vector2(200, 200), 50);
            // the 'addEntity' crashes with that dictionary key error
            // the error is likely happening in System

            // Entity food = Shared.Entities.Food.create("Textures/egg", new Vector2(120, 100), 50);
            // var message = new Shared.Messages.NewEntity(food);
            // Entity food = Shared.Entities.Food.create("Textures/egg", new Vector2(120, 100), 50);
            // var message = new Shared.Messages.NewEntity(food);
            // addEntity(food);
            // MessageQueueServer.instance.broadcastMessage(message);

            // Entity food2 = Shared.Entities.Food.create("Textures/egg", new Vector2(120, 120), 50);
            // var message2 = new Shared.Messages.NewEntity(food2);
            // MessageQueueServer.instance.broadcastMessage(message2);

            // Entity food = Shared.Entities.Food.create("Textures/egg", new Vector2(200, 200), 50);
            // addEntity(food);
            // MessageQueueServer.instance.broadcastMessage(message);

            MessageQueueServer.instance.registerConnectHandler(handleConnect);

            return true;
        }

        /// <summary>
        /// Give everything a chance to gracefully shutdown.
        /// </summary>
        public void shutdown()
        {

        }

        /// <summary>
        /// Upon connection of a new client, create a player entity and
        /// send that info back to the client, along with adding it to
        /// the server simulation.
        /// </summary>
        private void handleConnect(int clientId)
        {
            m_clients.Add(clientId);

            MessageQueueServer.instance.sendMessage(clientId, new Shared.Messages.ConnectAck());
        }

        /// <summary>
        /// When a client disconnects, need to tell all the other clients
        /// of the disconnect.
        /// </summary>
        /// <param name="clientId"></param>
        private void handleDisconnect(int clientId)
        {
            m_clients.Remove(clientId);

            Message message = new Shared.Messages.RemoveEntity(m_clientToEntityId[clientId]);
            MessageQueueServer.instance.broadcastMessage(message);

            removeEntity(m_clientToEntityId[clientId]);

            m_clientToEntityId.Remove(clientId);
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
            m_systemNetwork.add(entity);
            m_systemMovement.add(entity);
        }

        /// <summary>
        /// All entity lists for the systems must be given a chance to remove
        /// the entity.
        /// </summary>
        private void removeEntity(uint id)
        {
            m_entities.Remove(id);
            m_systemNetwork.remove(id);
            m_systemMovement.remove(id);
        }

        /// <summary>
        /// For the indicated client, sends messages for all other entities
        /// currently in the game simulation.
        /// </summary>
        private void reportAllEntities(int clientId)
        {
            foreach (var item in m_entities)
            {
                MessageQueueServer.instance.sendMessage(clientId, new Shared.Messages.NewEntity(item.Value));
            }
        }

        /// <summary>
        /// Handler for the Join message.  It gets a player entity created,
        /// added to the server game model, and notifies the requesting client
        /// of the player.
        /// </summary>
         private void handleJoin(int clientId, TimeSpan elapsedTime, Shared.Messages.Message message)
        {
            Shared.Messages.Join messageJoin = (Shared.Messages.Join) message;
            // Step 1: Tell the newly connected player about all other entities
            reportAllEntities(clientId);

            // Step 2: Create an entity for the newly joined player and sent it
            //         to the newly joined client
            int minX = 100;
            int minY = 100;
            int maxX = 2100;
            int maxY = 2100;
            Random random = new Random();
            int x = random.Next(minX, maxX + 1);
            int y = random.Next(minY, maxY + 1);

            // Entity player = Shared.Entities.Player.create(m_nextSnakeId++, "Textures/head", messageJoin.name, new Vector2(100, 100), 50, 0.2f);
            Entity player = Shared.Entities.Snake.createHead(m_nextSnakeId++, "Textures/head", messageJoin.name, new Vector2(100, 100), 50, 0.2f, (float)Math.PI / 1000);
            addEntity(player);
            m_clientToEntityId[clientId] = player.id;

            // Step 3: Send the new player entity to the newly joined client
            MessageQueueServer.instance.sendMessage(clientId, new NewEntity(player));


            // Step 4: Let all other clients know about this new player entity

            // We change the appearance for a player ship entity for all other clients to a different texture
            player.remove<Appearance>();
            player.add(new Appearance("Textures/head_enemy"));

            // Remove components not needed for "other" players
            player.remove<Shared.Components.Input>();

            Message messageNewEntity = new NewEntity(player);
            foreach (int otherId in m_clients)
            {
                if (otherId != clientId)
                {
                    MessageQueueServer.instance.sendMessage(otherId, messageNewEntity);
                }
            }
        }
    }
}
