﻿using Assets.Src.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.ShipCamera
{
    public class WeightedCameraOrientator : ICameraOrientator
    {
        private List<BaseCameraOrientator> _orientators;
        private IEnumerable<BaseCameraOrientator> _active = new List<BaseCameraOrientator>();

        public WeightedCameraOrientator(List<BaseCameraOrientator> orientators)
        {
            _orientators = orientators;
        }

        public Vector3 ReferenceVelocity {
            get {
                var weightedVectors = _active.Select(o => o.ReferenceVelocity * o.Priority);
                var totalVector = Vector3.zero;
                foreach(var v in weightedVectors)
                {
                    totalVector += v;
                }
                return totalVector/_totalWeight;
            }
        }

        public Vector3 ParentLocationTarget {
            get
            {
                var weightedVectors = _active.Select(o => o.ParentLocationTarget * o.Priority).ToList();
                var totalVector = Vector3.zero;
                foreach (var v in weightedVectors)
                {
                    totalVector += v;
                }
                return totalVector / _totalWeight;
            }
        }

        public Vector3 CameraLocationTarget {
            get
            {
                var weightedVectors = _active.Select(o => o.CameraLocationTarget * o.Priority );
                var totalVector = Vector3.zero;
                foreach (var v in weightedVectors)
                {
                    totalVector += v;
                }
                return totalVector / _totalWeight;
            }
        }

        public Quaternion ParentOrientationTarget { get { return ParentPollTarget.magnitude > 0 ? Quaternion.LookRotation(ParentPollTarget) : Quaternion.identity; } }

        public Quaternion CameraOrientationTarget { get { return CameraPollTarget.magnitude > 0 ? Quaternion.LookRotation(CameraPollTarget) : Quaternion.identity; } }

        public Vector3 ParentPollTarget {
            get
            {
                var weightedVectors = _active.Select(o => o.ParentPollTarget * o.Priority);
                var totalVector = Vector3.zero;
                foreach (var v in weightedVectors)
                {
                    totalVector += v;
                }
                return totalVector / _totalWeight;
            }
        }

        public Vector3 CameraPollTarget {
            get
            {
                var weightedVectors = _active.Select(o => o.CameraPollTarget * o.Priority);
                var totalVector = Vector3.zero;
                foreach (var v in weightedVectors)
                {
                    totalVector += v;
                }
                return totalVector / _totalWeight;
            }
        }

        public float CameraFieldOfView {
            get
            {
                var weightedVectors = _active.Select(o => o.CameraFieldOfView * o.Priority);
                float total = 0;
                foreach (var v in weightedVectors)
                {
                    total += v;
                }
                return total / _totalWeight;
            }
        }

        public bool HasTargets { get { return _orientators.Any(o => o.HasTargets && o.Priority > 0); } }

        private float _totalWeight {
            get
            {
                return _active.Sum(o => o.Priority);
            }
        }

        public ShipCamTargetValues CalculateTargets()
        {
            _active = _orientators.Where(o => o.HasTargets && o.Priority > 0);

            var targetValues = _active.ToDictionary(o => o.Priority, o => o.CalculateTargets());

            //Debug.Log(string.Join(", ", _active.OrderByDescending(o => o.Priority).Select(o => o.Description + o.Priority).ToArray()));
            return new ShipCamTargetValues(ParentLocationTarget, ParentOrientationTarget, CameraLocationTarget, CameraOrientationTarget, CameraFieldOfView, ReferenceVelocity);
        }
    }
}
