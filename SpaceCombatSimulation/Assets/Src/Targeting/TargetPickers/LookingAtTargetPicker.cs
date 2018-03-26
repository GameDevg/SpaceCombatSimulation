﻿using Assets.Src.Evolution;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.Targeting.TargetPickers
{
    class LookingAtTargetPicker : GeneticallyConfigurableTargetPicker
    {
        private Rigidbody _aimingObject;

        /// <summary>
        /// kull targets more than 90 degrees awy from looked direction
        /// </summary>
        public bool KullInvalidTargets = false;

        /// <summary>
        /// used for velocity correction.
        /// Set to null to not correct for velocity (default)
        /// </summary>
        public float? ProjectileSpeed;

        public LookingAtTargetPicker(Rigidbody aimingObject)
        {
            _aimingObject = aimingObject;
        }

        public override IEnumerable<PotentialTarget> FilterTargets(IEnumerable<PotentialTarget> potentialTargets)
        {
            potentialTargets = potentialTargets.Select(t => AddScoreForAngle(t));

            if (KullInvalidTargets && potentialTargets.Any(t => t.IsValidForCurrentPicker))
            {
                return potentialTargets.Where(t => t.IsValidForCurrentPicker);
            }
            return potentialTargets;
        }

        private PotentialTarget AddScoreForAngle(PotentialTarget target)
        {
            var reletiveLocation = target.LocationInAimedSpace(_aimingObject, ProjectileSpeed);

            var angle = Vector3.Angle(reletiveLocation, Vector3.forward);
            
            var newScore = Multiplier * (1 - (angle/ 180));
            newScore += angle < Threshold ? FlatBoost : 0;
            target.Score = target.Score + newScore;
            target.IsValidForCurrentPicker = angle < 90;
            return target;
        }
    }
}
