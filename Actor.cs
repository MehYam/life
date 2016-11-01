using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using lifeEngine.behavior;

namespace lifeEngine
{
    public class Actor : ITickHandler
    {
        // HACKS
        class Attributes
        {
            class Health
            {
                public float head;
                public float neck;
                public float leftArm;
                public float rightArm;
                public float leftHand;
                public float rightHand;
                public float leftLeg;
                public float rightLeg;
                public float leftFoot;
                public float rightFoot;
            }
            class Traits
            {
                float mentalStamina;
                float mentalDexterity;
                float mentalStrength;

                float physicalStamina;
                float physicalDexterity;
                float physicalStrength;

                float coordination;
                float happiness;
                float empathy;
                float wit;

                float looks;
                float stimulation;

                float genderIdentification;
                float genderAttraction;
                float sexDrive;

                bool rightHanded;
            }
            float height;
            float weight;

            float MovementSpeed
            {
                // factors:  p stam, p dex, p str, weight, leg and feet health
                get { return 0; }
            }
            float PunchSpeed // time between punches
            {
                // factors: p stam, p dex, arm health
                get { return 0; }
            }
            float PunchPower
            {
                // factors: p str
                get { return 0; }
            }
            float KickSpeed
            {
                // factors: p stam, p dex, leg health
                get { return 0; }
            }
            float KickPower
            {
                // factors: p str
                get { return 0; }
            }
        }
        
        // END HACKS

        public readonly char type;
        public readonly float speedTPS = 3;
        public Point<float> pos = new Point<float>(0, 0);  // Actors get sub-tile precision
        public Actor(char type = 'A')
        {
            this.type = type;
        }
        public List<IBehavior> _priorities = new List<IBehavior>();
        public void FixedUpdate(float time, float deltaTime)
        {
            if (_priorities.Count > 0)
            {
                _priorities[0].FixedUpdate(time, deltaTime);
                if (_priorities[0].IsComplete)
                {
                    _priorities.RemoveAt(0);
                }
            }
        }
        public void AddPriority(IBehavior behavior)
        {
            _priorities.Add(behavior);
        }
        public override string ToString()
        {
            return string.Format("{0}, ({1})", type, pos);
        }
    }
}
