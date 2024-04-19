﻿
using Microsoft.Xna.Framework.Input;
using Shared.Entities;
using System;
using System.Collections.Generic;
using CS5410.Controls;

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

        public KeyboardInput(List<Tuple<Shared.Components.Input.Type, Keys>> mapping) : base(typeof(Shared.Components.Input))
        {
            foreach (var input in mapping)
            {
                m_typeToKey[input.Item1] = input.Item2;
            }
        }

public override void update(TimeSpan elapsedTime)
{
    foreach (var item in m_entities)
    {
        List<Shared.Components.Input.Type> inputs = new List<Shared.Components.Input.Type>();
        var keyMap = m_keyToFunction[item.Key].m_keyToType;

        if (m_keysPressed.Contains(ControlsManager.Controls["TurnUp"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnRight"]))
        {
            inputs.Add(Shared.Components.Input.Type.TurnUpRight);
            Shared.Entities.Utility.turnUpRight(item.Value, elapsedTime);
        } 
        else if (m_keysPressed.Contains(ControlsManager.Controls["TurnDown"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnRight"]))
        {
            inputs.Add(Shared.Components.Input.Type.TurnDownRight);
            Shared.Entities.Utility.turnDownRight(item.Value, elapsedTime);
        }
        else if (m_keysPressed.Contains(ControlsManager.Controls["TurnUp"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnLeft"]))
        {
            inputs.Add(Shared.Components.Input.Type.TurnUpLeft);
            Shared.Entities.Utility.turnUpLeft(item.Value, elapsedTime);
        }
        else if (m_keysPressed.Contains(ControlsManager.Controls["TurnDown"]) && m_keysPressed.Contains(ControlsManager.Controls["TurnLeft"]))
        {
            inputs.Add(Shared.Components.Input.Type.TurnDownLeft);
            Shared.Entities.Utility.turnDownLeft(item.Value, elapsedTime);
        }
        else
        {
            // Continue with individual key checks
            foreach (var key in m_keysPressed)
            {
                if (keyMap.ContainsKey(key))
                {
                    var type = keyMap[key];
                    inputs.Add(type);

                    // Client-side prediction of the input
                    switch (type)
                    {
                        case Shared.Components.Input.Type.TurnLeft:
                            Shared.Entities.Utility.turnLeft(item.Value, elapsedTime);
                            break;
                        case Shared.Components.Input.Type.TurnRight:
                            Shared.Entities.Utility.turnRight(item.Value, elapsedTime);
                            break;
                        case Shared.Components.Input.Type.TurnUp:
                            Shared.Entities.Utility.turnUp(item.Value, elapsedTime);
                            break;
                        case Shared.Components.Input.Type.TurnDown:
                            Shared.Entities.Utility.turnDown(item.Value, elapsedTime);
                            break;
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
