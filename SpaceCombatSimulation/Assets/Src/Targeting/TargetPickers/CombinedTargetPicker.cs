﻿using Assets.Src.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.Targeting
{
    public class CombinedTargetPicker : MonoBehaviour, ITargetPicker
    {
        private IOrderedEnumerable<ITargetPicker> _targeters;

        void Start()
        {
            var pickers = GetComponents<ITargetPicker>();
            _targeters = pickers.Where(p => p != this).OrderBy(p => p.TargetPickerPriority);
            if (!_targeters.Any()) Debug.LogWarning(name + " has no target pickers!");
        }

        public float TargetPickerPriority
        {
            get
            {
                return _targeters.First().TargetPickerPriority;
            }
        }

        public IEnumerable<PotentialTarget> FilterTargets(IEnumerable<PotentialTarget> potentialTargets)
        {
            foreach (var targeter in _targeters)
            {
                potentialTargets = targeter.FilterTargets(potentialTargets);
            }
            return potentialTargets;
        }
    }
}
