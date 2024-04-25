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
        private int m_nextFoodId = 0;
        private HashSet<int> m_clients = new HashSet<int>();
        private Dictionary<uint, Entity> m_entities = new Dictionary<uint, Entity>();
        private Dictionary<int, uint> m_clientToEntityId = new Dictionary<int, uint>();
        private List<Entity> m_foodList = new List<Entity>();
        private int minX = 0;
        private int minY = 0;
        private int maxX = 2200;
        private int maxY = 2200;
        private Random random = new Random();

        private int maxFoodThreshold = 100;
        private const float foodUpdateInterval = 5f;
        private float foodUpdateTime = 0f;
        private const float foodAnimateInterval = 1f;
        private float foodAnimateTime = 0f;
        private int minFoodSize = 20;
        private int maxFoodSize = 70;

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
            checkSnakeCollisionwithFood(elapsedTime);
            removeSnakesAtBorders(elapsedTime);

            foodUpdateTime += (float)elapsedTime.TotalSeconds;
            if (foodUpdateTime >= foodUpdateInterval)
            {
                updateFood();
                foodUpdateTime = 0f;
            }
        }

        private void removeSnakesAtBorders(TimeSpan elapsedTime)
        {
            List<uint> toRemove = new List<uint>();
            foreach (Entity entity in new List<Entity>(m_entities.Values))
            {
                if (entity.contains<Shared.Components.SnakeId>())
                {
                    Vector2 pos = entity.get<Shared.Components.Position>().position;
                    if (pos.X <= minX || pos.X >= maxX || pos.Y <= minY || pos.Y >= maxY)
                    {
                        toRemove.Add(entity.id);
                    }
                }
            }

            foreach (uint id in toRemove)
            {
                snakeDeath(id);
            }
        }

        private void snakeDeath(uint id)
        {
            if (m_entities[id].contains<Score>() && m_entities[id].contains<Position>())
            {
                // drop 1 food per 5 points the player had (can change this amount to whatever)
                var score = m_entities[id].get<Score>().score;
                var position = m_entities[id].get<Position>().position;
                int numFoodtoDrop = score / 5;
                for (int i = 0; i < numFoodtoDrop; i++)
                {
                    int x = random.Next((int)position.X-30, (int)position.X+30);
                    int y = random.Next((int)position.Y-30, (int)position.Y+30);
                    x = Math.Max(minX + 30, Math.Min(maxX - 30, x));
                    y = Math.Max(minY + 30, Math.Min(maxY - 30, y));
                    int size = random.Next(minFoodSize, maxFoodSize);
                    Entity food = Shared.Entities.Food.create(m_nextFoodId++, "Textures/egg", new Vector2(x, y), size);
                    m_foodList.Add(food);
                    addEntity(food);
                    MessageQueueServer.instance.broadcastMessage(new Shared.Messages.NewEntity(food));
                }

                removeEntity(id);
                Message removeMessage = new Shared.Messages.RemoveEntity(id);
                MessageQueueServer.instance.broadcastMessage(removeMessage);

                // to-do: remove any segments associated with that snake on death

            }
        }

        private void checkSnakeCollisionwithFood(TimeSpan elapsedTime)
        {
            foreach (Entity entity in m_entities.Values)
            {
                if (entity.contains<Shared.Components.SnakeId>())
                {
                    var snakePos = entity.get<Shared.Components.Position>().position;
                    float snakeRadius = entity.get<Shared.Components.Size>().size.X / 2;

                    List<Entity> eatenFoods = new List<Entity>();

                    foreach (var food in m_foodList)
                    {
                        var foodPos = food.get<Shared.Components.Position>().position;
                        float foodRadius = food.get<Shared.Components.Size>().size.X / 2;

                        float dx = snakePos.X - foodPos.X;
                        float dy = snakePos.Y - foodPos.Y;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                        if (distance <= snakeRadius + foodRadius)
                        {
                            eatenFoods.Add(food);
                        }
                    }
                    foreach (var food in eatenFoods)
                    {
                        // remove that food entity
                        m_foodList.Remove(food);
                        removeEntity(food.id);
                        Message message = new Shared.Messages.RemoveEntity(food.id);
                        MessageQueueServer.instance.broadcastMessage(message);

                        // increment snake's score
                        Entity snake = m_entities[entity.id];
                        if (snake.contains<Score>())
                        {
                            Score scoreComponent = snake.get<Score>();
                            scoreComponent.score += 1;
                            var myMessage = new Shared.Messages.UpdateEntity(snake, elapsedTime);
                            MessageQueueServer.instance.broadcastMessage(myMessage);
                        }
                    }
                }
            }
        }

        private void updateFood()
        {
            if (m_foodList.Count < maxFoodThreshold)
            {
                int foodNeeded = maxFoodThreshold - m_foodList.Count;
                for (int i = 0; i < foodNeeded; i++)
                {
                    int x = random.Next(minX + 100, maxX - 99);
                    int y = random.Next(minY + 100, maxY - 99);
                    int size = random.Next(minFoodSize, maxFoodSize);
                    Entity food = Shared.Entities.Food.create(m_nextFoodId++, "Textures/egg", new Vector2(x, y), size);
                    m_foodList.Add(food);
                    addEntity(food);
                    MessageQueueServer.instance.broadcastMessage(new Shared.Messages.NewEntity(food));
                }
            }
        }

        /// <summary>
        /// Setup notifications for when new clients connect.
        /// </summary>
        public bool initialize()
        {
            m_systemNetwork.registerHandler(Shared.Messages.Type.Join, handleJoin);
            m_systemNetwork.registerDisconnectHandler(handleDisconnect);

            for (int i = 0; i < maxFoodThreshold; i++)
            {
                int x = random.Next(minX + 100, maxX - 99);
                int y = random.Next(minY + 100, maxY - 99);
                int size = random.Next(minFoodSize, maxFoodSize);
                Entity food = Shared.Entities.Food.create(m_nextFoodId++, "Textures/egg", new Vector2(x, y), size);
                m_foodList.Add(food);
                addEntity(food);
            }

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
            if (m_entities[id].contains<Shared.Components.SnakeId>())
            {
                foreach (var entity in m_entities)
                {
                    if (entity.Key == id)
                    {
                        continue;
                    }
                    if (entity.Value.contains<Shared.Components.SnakeId>() && entity.Value.get<Shared.Components.SnakeId>().id == m_entities[id].get<Shared.Components.SnakeId>().id)
                    {
                        m_entities.Remove(entity.Key);

                        m_systemNetwork.remove(entity.Key);
                        m_systemMovement.remove(entity.Key);
                    }
                }
            }
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
        /// 
        private void handleJoin(int clientId, TimeSpan elapsedTime, Shared.Messages.Message message)
        {
            Shared.Messages.Join messageJoin = (Shared.Messages.Join) message;

            int x = random.Next(minX + 100, maxX - 99);
            int y = random.Next(minY + 100, maxY - 99);
            Entity player = Shared.Entities.Snake.createHead(++m_nextSnakeId, "Textures/head", messageJoin.name, new Vector2(x, y), 50, 0.2f, 0, 0);
            // player.add(new PlayerType("Player"));
            Entity tail = Shared.Entities.Segment.createSegment(m_nextSnakeId, "Textures/tail", new Vector2(x - 50, y), 50, 0.2f);

            // Send the initial entities to the joining client
            MessageQueueServer.instance.sendMessage(clientId, new NewEntity(player));
            MessageQueueServer.instance.sendMessage(clientId, new NewEntity(tail));

            // Change appearance for other clients
            player.remove<Appearance>();
            tail.remove<Appearance>();
            player.add(new Appearance("Textures/head_enemy"));
            player.remove<PlayerType>();
            player.add(new PlayerType("Enemy"));
            tail.add(new Appearance("Textures/tail_enemy"));
            player.remove<Shared.Components.Input>();

            // Inform all other clients about the new entities
            Message messageNewPlayer = new NewEntity(player);
            Message messageNewTail = new NewEntity(tail);
            foreach (int otherId in m_clients)
            {
                if (otherId != clientId)
                {
                    MessageQueueServer.instance.sendMessage(otherId, messageNewPlayer);
                    MessageQueueServer.instance.sendMessage(otherId, messageNewTail);
                }
            }

            // Add new entities to the server's game model
            addEntity(player);
            addEntity(tail);
            m_clientToEntityId[clientId] = player.id;

            // Send all other known entities to the newly joined client
            reportAllEntities(clientId);
        }
    }
}
