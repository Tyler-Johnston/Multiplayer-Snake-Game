
using Microsoft.Xna.Framework.Input;
using Shared.Entities;
using System;
using System.Collections.Generic;
using CS5410.Controls;
using System.Numerics;

namespace Client.Systems
{
    public class KeyboardInput : Shared.Systems.System
    {
        private class KeyToType
        {
            public Dictionary<Keys, Shared.Components.Input.Type> m_keyToType = new Dictionary<Keys, Shared.Components.Input.Type>();
        }

        private Dictionary<Shared.Components.Input.Type, Keys> m_typeToKey = new Dictionary<Shared.Components.Input.Type, Keys>();
        private Dictionary<uint, KeyToType> m_keyToFunction = new Dictionary<uint, KeyToType>();

        private HashSet<Keys> m_keysPressed = new HashSet<Keys>();
        private List<Shared.Components.Input.Type> m_inputEvents = new List<Shared.Components.Input.Type>();

        private MouseState mouseState;
        private Vector2 mousePos;
        public string controlScheme = "None";
        public bool mouseFlag = false;
        public uint? m_playerId = null;
        int VPW = 1300;
        int VPH = 750;
        private int WorldWidth = 750 * 3;

        public KeyboardInput(List<Tuple<Shared.Components.Input.Type, Keys>> mapping) : base(typeof(Shared.Components.Input))
        {
            foreach (var input in mapping)
            {
                m_typeToKey[input.Item1] = input.Item2;
            }
        }

        public override void update(TimeSpan elapsedTime)
        {
            if (controlScheme == "Mouse")
            {
                mouseFlag = true;
            }
            mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            // only update mousePos if mouse is moved
            if (mouseState.X != mousePos.X || mouseState.Y != mousePos.Y) {
                // only update mousePos if mouse is within window
                if (mouseState.X >= 0 && mouseState.Y >= 0 && mouseState.X <= 1300 && mouseState.Y <= 750) {
                    mousePos = new Vector2(mouseState.X, mouseState.Y);
                }
            }
            
            foreach (var item in m_entities)
            {
                List<Shared.Components.Input.Type> inputs = new List<Shared.Components.Input.Type>();
                var keyMap = m_keyToFunction[item.Key].m_keyToType;

                Shared.Entities.Entity? turnPoint = null;

                if (!item.Value.contains<Shared.Components.Input>())
                {
                    continue;
                }

                // MOUSE MOVEMENT
                if (mouseFlag) {
                    // Get viewport offsets
                    Entity entity = m_entities[m_playerId.Value];
                    var position = entity.get<Shared.Components.Position>().position;
                    int m_viewportOffsetX = (int)Math.Min(Math.Max(position.X - VPW / 2, 0), WorldWidth - VPW);
                    int m_viewportOffsetY = (int)Math.Min(Math.Max(position.Y - VPH / 2, 0), WorldWidth - VPH);
                    int playerX = (m_viewportOffsetX == 0 || m_viewportOffsetX == WorldWidth - VPW) ? (int)position.X : (int)(VPW / 2);
                    int playerY = (m_viewportOffsetY == 0 || m_viewportOffsetY == WorldWidth - VPH) ? (int)position.Y : (int)(VPH / 2);
                    int viewportMaxXThreshold = WorldWidth - VPW / 2;
                    int playerXOffset = WorldWidth - VPW;
                    if (playerX > viewportMaxXThreshold)
                    {
                        playerX = playerX - playerXOffset;
                    }
                    int viewportMaxYThreshold = WorldWidth - VPH / 2;
                    int playerYOffset = WorldWidth - VPH;
                    if (playerY > viewportMaxYThreshold)
                    {
                        playerY = playerY - playerYOffset;
                    }

                    // DEADZONED @ X: 600 to 700 / Y: 325 to 425
                    // UP
                    // if (mousePos.X >= 600 && mousePos.X <= 700 && mousePos.Y >= 0 && mousePos.Y <= 325) {
                    //     inputs.Add(Shared.Components.Input.Type.TurnUp);
                    //     turnPoint = Shared.Entities.Utility.turnUp(item.Value);
                    // }
                    // // DOWN
                    // if (mousePos.X >= 600 && mousePos.X <= 700 && mousePos.Y >= 425 && mousePos.Y <= 750) {
                    //     inputs.Add(Shared.Components.Input.Type.TurnDown);
                    //     turnPoint = Shared.Entities.Utility.turnDown(item.Value);
                    // }
                    // // LEFT
                    // if (mousePos.X >= 0 && mousePos.X <= 600 && mousePos.Y >= 325 && mousePos.Y <= 425) {
                    //     inputs.Add(Shared.Components.Input.Type.TurnLeft);
                    //     turnPoint = Shared.Entities.Utility.turnLeft(item.Value);
                    // }
                    // // RIGHT
                    // if (mousePos.X >= 700 && mousePos.X <= 1300 && mousePos.Y >= 325 && mousePos.Y <= 425) {
                    //     inputs.Add(Shared.Components.Input.Type.TurnRight);
                    //     turnPoint = Shared.Entities.Utility.turnRight(item.Value);
                    // }


                    if (mousePos.X >= playerX && mousePos.Y <= playerY) {
                        if (Math.Abs(mousePos.X - playerX) < 100 || Math.Abs(mousePos.Y - playerY) < 100) {
                            // RIGHT
                            if (Math.Abs(mousePos.X - playerX) > Math.Abs(mousePos.Y - playerY) || playerX > 1290) {
                                inputs.Add(Shared.Components.Input.Type.TurnRight);
                                turnPoint = Shared.Entities.Utility.turnRight(item.Value);
                            }
                            // UP
                            else {
                                inputs.Add(Shared.Components.Input.Type.TurnUp);
                                turnPoint = Shared.Entities.Utility.turnUp(item.Value);
                            }

                        }
                        // UP-RIGHT
                        else {
                            inputs.Add(Shared.Components.Input.Type.TurnUpRight);
                            turnPoint = Shared.Entities.Utility.turnUpRight(item.Value);
                        }
                    }

                    if (mousePos.X >= playerX && mousePos.Y >= playerY) {
                        if (Math.Abs(mousePos.X - playerX) < 100 || Math.Abs(mousePos.Y - playerY) < 100) {
                            // RIGHT
                            if (Math.Abs(mousePos.X - playerX) > Math.Abs(mousePos.Y - playerY) || playerX > 1290) {
                                inputs.Add(Shared.Components.Input.Type.TurnRight);
                                turnPoint = Shared.Entities.Utility.turnRight(item.Value);
                            }
                            // DOWN
                            else {
                                inputs.Add(Shared.Components.Input.Type.TurnDown);
                                turnPoint = Shared.Entities.Utility.turnDown(item.Value);
                            }
                        }
                        // DOWN-RIGHT
                        else {
                        inputs.Add(Shared.Components.Input.Type.TurnDownRight);
                        turnPoint = Shared.Entities.Utility.turnDownRight(item.Value);
                        }
                    }

                    if (mousePos.X <= playerX && mousePos.Y <= playerY) {
                        if (Math.Abs(mousePos.X - playerX) < 100 || Math.Abs(mousePos.Y - playerY) < 100) {
                            // LEFT
                            if (Math.Abs(mousePos.X - playerX) > Math.Abs(mousePos.Y - playerY) || playerX < 10) {
                                inputs.Add(Shared.Components.Input.Type.TurnLeft);
                                turnPoint = Shared.Entities.Utility.turnLeft(item.Value);
                            }
                            // UP
                            else {
                                inputs.Add(Shared.Components.Input.Type.TurnUp);
                                turnPoint = Shared.Entities.Utility.turnUp(item.Value);
                            }
                        // UP-LEFT
                        }
                        else {
                            inputs.Add(Shared.Components.Input.Type.TurnUpLeft);
                            turnPoint = Shared.Entities.Utility.turnUpLeft(item.Value);
                        }
                    }
                   
                    if (mousePos.X <= playerX && mousePos.Y >= playerY) {
                        if (Math.Abs(mousePos.X - playerX) < 100 || Math.Abs(mousePos.Y - playerY) < 100) {
                            // LEFT
                            if (Math.Abs(mousePos.X - playerX) > Math.Abs(mousePos.Y - playerY) || playerX < 10) {
                                inputs.Add(Shared.Components.Input.Type.TurnLeft);
                                turnPoint = Shared.Entities.Utility.turnLeft(item.Value);
                            }
                            // DOWN
                            else {
                                inputs.Add(Shared.Components.Input.Type.TurnDown);
                                turnPoint = Shared.Entities.Utility.turnDown(item.Value);
                            }

                        }
                        // DOWN-LEFT
                        else {
                            inputs.Add(Shared.Components.Input.Type.TurnDownLeft);
                            turnPoint = Shared.Entities.Utility.turnDownLeft(item.Value);
                        }
                    }

                }
                // KEYBOARD MOVEMENT
                else {
                    if (m_keysPressed.Contains(ControlsManager.Controls["TurnUp"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnRight"]))
                    {
                        inputs.Add(Shared.Components.Input.Type.TurnUpRight);
                        turnPoint = Shared.Entities.Utility.turnUpRight(item.Value);
                    } 
                    else if (m_keysPressed.Contains(ControlsManager.Controls["TurnDown"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnRight"]))
                    {
                        inputs.Add(Shared.Components.Input.Type.TurnDownRight);
                        turnPoint = Shared.Entities.Utility.turnDownRight(item.Value);
                    }
                    else if (m_keysPressed.Contains(ControlsManager.Controls["TurnUp"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnLeft"]))
                    {
                        inputs.Add(Shared.Components.Input.Type.TurnUpLeft);
                        turnPoint = Shared.Entities.Utility.turnUpLeft(item.Value);
                    }
                    else if (m_keysPressed.Contains(ControlsManager.Controls["TurnDown"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnLeft"]))
                    {
                        inputs.Add(Shared.Components.Input.Type.TurnDownLeft);
                        turnPoint = Shared.Entities.Utility.turnDownLeft(item.Value);
                    }
                    else
                    {
                        foreach (var key in m_keysPressed)
                        {
                            if (m_keyToFunction[item.Key].m_keyToType.ContainsKey(key))
                            {
                                // var type = keyMap[key];
                                var type = m_keyToFunction[item.Key].m_keyToType[key];
                                inputs.Add(type);

                                // Client-side prediction of the input
                                switch (type)
                                {
                                    case Shared.Components.Input.Type.TurnUp:
                                        turnPoint = Shared.Entities.Utility.turnUp(item.Value);
                                        break;
                                    case Shared.Components.Input.Type.TurnDown:
                                        turnPoint = Shared.Entities.Utility.turnDown(item.Value);
                                        break;
                                    case Shared.Components.Input.Type.TurnLeft:
                                        turnPoint = Shared.Entities.Utility.turnLeft(item.Value);
                                        break;
                                    case Shared.Components.Input.Type.TurnRight:
                                        turnPoint = Shared.Entities.Utility.turnRight(item.Value);
                                        break;
                                }
                            }
                        }
                    }
                }

                if (turnPoint != null)
                {
                    // TODO: Update this for the server side as well
                    var id = turnPoint.get<Shared.Components.SnakeId>().id;
                    foreach (Entity entity in m_entities.Values)
                    {
                        if (entity.contains<Shared.Components.SnakeId>() && entity.get<Shared.Components.SnakeId>().id == id)
                        {
                            if (entity.contains<Shared.Components.TurnPointQueue>())
                            {
                                entity.get<Shared.Components.TurnPointQueue>().queue.Enqueue(turnPoint);
                            }
                        }
                    }
                }
                

                if (inputs.Count > 0)
                {
                    MessageQueueClient.instance.sendMessageWithId(new Shared.Messages.Input(item.Key, inputs, elapsedTime));
                }
            }
        }


        public void UpdateControlMappings(List<Tuple<Shared.Components.Input.Type, Keys>> newMappings)
        {
            // Update m_typeToKey with new mappings
            m_typeToKey.Clear();
            foreach (var mapping in newMappings)
            {
                m_typeToKey[mapping.Item1] = mapping.Item2;
            }

            // Update each entity's m_keyToFunction to reflect the new mappings
            foreach (var entityKey in m_keyToFunction.Keys)
            {
                var entityMappings = m_keyToFunction[entityKey];
                entityMappings.m_keyToType.Clear();
                var entityInputs = m_entities[entityKey].get<Shared.Components.Input>().inputs;

                foreach (var inputType in entityInputs)
                {
                    if (m_typeToKey.TryGetValue(inputType, out var key))
                    {
                        entityMappings.m_keyToType[key] = inputType;
                    }
                }
            }
        }

        public override bool add(Entity entity)
        {
            if (!base.add(entity))
            {
                return false;
            }

            KeyToType map = new KeyToType();
            foreach (var input in entity.get<Shared.Components.Input>().inputs)
            {
                map.m_keyToType[m_typeToKey[input]] = input;
            }
            
            m_keyToFunction[entity.id] = map;

            return true;
        }

        public override void remove(uint id)
        {
            base.remove(id);

            m_keyToFunction.Remove(id);
        }

        public void keyPressed(Keys key)
        {
            m_keysPressed.Add(key);
        }

        public void keyReleased(Keys key)
        {
            m_keysPressed.Remove(key);
        }
    }
}