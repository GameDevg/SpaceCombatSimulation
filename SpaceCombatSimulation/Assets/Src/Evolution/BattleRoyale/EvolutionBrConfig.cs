﻿using System;

namespace Assets.Src.Evolution
{
    public class EvolutionBrConfig : BaseEvolutionConfig
    {
        public const int MAX_COMBATANTS = 6;
        public const int MIN_COMBATANTS = 1;

        private int _numberOfCombatants = 2;
        public int NumberOfCombatants {
            get {
                return Math.Max(
                    MIN_COMBATANTS,
                    Math.Min(
                        MAX_COMBATANTS, _numberOfCombatants
                    )
                );
            }
            set
            {
                _numberOfCombatants = value;
            }
        }

        public float OnSphereRandomisationRadius = 50;
        public float InSphereRandomisationRadius = 0;
    }
}
