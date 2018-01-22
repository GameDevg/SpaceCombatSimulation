﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Src.Evolution
{
    public class BaseEvolutionController : MonoBehaviour
    {
        public int DatabaseId;

        public EvolutionShipConfig ShipConfig;
        protected EvolutionMutationWrapper _mutationControl = new EvolutionMutationWrapper();
        protected EvolutionMatchController _matchControl;

        protected IEnumerable<Transform> ListShips()
        {
            return GameObject.FindGameObjectsWithTag(ShipConfig.SpaceShipTag)
                    .Where(s =>
                        s.transform.parent != null &&
                        s.transform.parent.GetComponent("Rigidbody") != null
                    ).Select(s => s.transform.parent);
        }
    }
}