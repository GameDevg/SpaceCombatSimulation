﻿using Assets.Src.ObjectManagement;
using Assets.Src.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Src.Targeting
{
    public static class TargetingExtensions
    {
        #region TargetVersions
        public static Vector3 LocationInOthersSpace(this Target target, Rigidbody turnTable, float? projectileSpeed)
        {
            return target.Rigidbody != null ?
                target.Rigidbody.LocationInOthersSpace(turnTable, projectileSpeed) :
                target.Transform.LocationInOthersSpace(turnTable);
        }

        public static Vector3 LocationInElevationHubSpaceAfterTurnTableTurn(this Target target, Rigidbody thisTurret, Transform turnTable, Rigidbody elevationHub, float? projectileSpeed)
        {
            return target.Rigidbody != null ?
                target.Rigidbody.LocationInElevationHubSpaceAfterTurnTableTurn(thisTurret, turnTable, elevationHub, projectileSpeed):
                target.Transform.LocationInElevationHubSpaceAfterTurnTableTurn(thisTurret, turnTable, elevationHub);
        }
        
        public static Vector3 LocationInAimedSpace(this Target target, Rigidbody aimingObject, float? projectileSpeed)
        {
            return target.Rigidbody != null ?
                target.Rigidbody.LocationInOthersSpace(aimingObject, projectileSpeed):
                target.Transform.LocationInOthersSpace(aimingObject);
        }
        
        public static Vector3 CorrectForVelocity(this Target target, Rigidbody baseObject, float? projectileSpeed)
        {
            return target.Rigidbody != null ?
                target.Rigidbody.CorrectForVelocity(baseObject, projectileSpeed):
                target.Transform.position;
        }

        /// <summary>
        /// Returns the distance to the target from the given Rigidbody.
        /// Returns float.MaxValue if the target or turret Rigidbody is null.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="thisTurret"></param>
        /// <param name="projectileSpeed"></param>
        /// <returns></returns>
        public static float DistanceToTurret(this Target target, Rigidbody thisTurret, float? projectileSpeed)
        {
            return target.Rigidbody != null ?
                 target.Rigidbody.DistanceToTurret(thisTurret, projectileSpeed):
                 target.Transform.DistanceToTurret(thisTurret.transform);
        }

        /// <summary>
        /// Returns the distance to the target from the given Rigidbody.
        /// Returns float.MaxValue if the target or turret Rigidbody is null.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="thisTurret"></param>
        /// <param name="projectileSpeed"></param>
        /// <returns></returns>
        public static float DistanceToTurret(this Target target, Transform thisTurret)
        {
            return target.Transform.DistanceToTurret(thisTurret);
        }
        #endregion

        #region RigidbodyVersions
        public static Vector3 LocationInOthersSpace(this Rigidbody target, Rigidbody origin, float? projectileSpeed)
        {
            if (origin == null)
            {
                return Vector3.zero;
            }

            var location = target.CorrectForVelocity(origin, projectileSpeed);

            return origin.transform.InverseTransformPoint(location);
        }

        public static Vector3 LocationInElevationHubSpaceAfterTurnTableTurn(this Rigidbody target, Rigidbody thisTurret, Transform turnTable, Rigidbody elevationHub, float? projectileSpeed)
        {
            if (turnTable == null || elevationHub == null)
            {
                return Vector3.zero;
            }

            var location = target.CorrectForVelocity(thisTurret, projectileSpeed);

            //Debug.Log("WorldLocation Now: " + location);
            location = turnTable.InverseTransformPoint(location);
            //Debug.Log("LocationInTurnTableSpace Now: " + location);

            var elevation = location.y;

            location.y = 0;

            var distance = location.magnitude;

            var effectiveLocation = new Vector3(0, elevation, distance);
            //Debug.Log("effectiveLocation: " + effectiveLocation);

            var locationInWorldSpace = turnTable.TransformPoint(effectiveLocation);
            //Debug.Log("locationInWorldSpace: " + locationInWorldSpace);

            //Debug.Log("EffectiveLocation in hub space: " + ElevationHub.transform.InverseTransformPoint(locationInWorldSpace));

            return elevationHub.transform.InverseTransformPoint(locationInWorldSpace);
        }

        public static Vector3 CorrectForVelocity(this Rigidbody potentialTarget, Rigidbody baseObject, float? projectileSpeed)
        {
            var location = potentialTarget.position;

            if (!projectileSpeed.HasValue || projectileSpeed.Value == 0)
            {
                return location;
            }

            var projectileSpeedValue = projectileSpeed.Value;

            var velocity = potentialTarget.velocity;

            var distance = potentialTarget.DistanceToTurret(baseObject, null);

            velocity = baseObject == null ? velocity : velocity - baseObject.velocity;

            var offsetdistance = distance * velocity / projectileSpeedValue;

            //Debug.Log("s=" + distance + "v=" + velocity + ", vp=" + _projectileSpeed + ", aiming " + offsetdistance + " ahead.");

            return location + offsetdistance;
        }

        /// <summary>
        /// Returns the distance to the target from the given Rigidbody.
        /// Returns float.MaxValue if the target or turret Rigidbody is null.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="thisTurret"></param>
        /// <param name="projectileSpeed"></param>
        /// <returns></returns>
        public static float DistanceToTurret(this Rigidbody target, Rigidbody thisTurret, float? projectileSpeed)
        {
            if(target == null || thisTurret == null)
            {
                return float.MaxValue;
            }
            var location = target.CorrectForVelocity(thisTurret, projectileSpeed);
            var dist = Vector3.Distance(location, thisTurret.position);
            return dist;
        }
        #endregion

        #region TransformVersions
        public static Vector3 LocationInOthersSpace(this Transform target, Rigidbody origin)
        {
            if (origin == null)
            {
                return Vector3.zero;
            }
            var location = target.position;

            return origin.transform.InverseTransformPoint(location);
        }

        public static Vector3 LocationInElevationHubSpaceAfterTurnTableTurn(this Transform target, Rigidbody thisTurret, Transform turnTable, Rigidbody elevationHub)
        {
            if (turnTable == null || elevationHub == null)
            {
                return Vector3.zero;
            }

            var location = target.position;

            //Debug.Log("WorldLocation Now: " + location);
            location = turnTable.InverseTransformPoint(location);
            //Debug.Log("LocationInTurnTableSpace Now: " + location);

            var elevation = location.y;

            location.y = 0;

            var distance = location.magnitude;

            var effectiveLocation = new Vector3(0, elevation, distance);
            //Debug.Log("effectiveLocation: " + effectiveLocation);

            var locationInWorldSpace = turnTable.TransformPoint(effectiveLocation);
            //Debug.Log("locationInWorldSpace: " + locationInWorldSpace);

            //Debug.Log("EffectiveLocation in hub space: " + ElevationHub.transform.InverseTransformPoint(locationInWorldSpace));

            return elevationHub.transform.InverseTransformPoint(locationInWorldSpace);
        }

        /// <summary>
        /// Returns the distance to the target from the given Rigidbody.
        /// Returns float.MaxValue if the target or turret Rigidbody is null.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="thisTurret"></param>
        /// <param name="projectileSpeed"></param>
        /// <returns></returns>
        public static float DistanceToTurret(this Transform target, Transform thisTurret)
        {
            if (target == null || thisTurret.IsInvalid())
            {
                return float.MaxValue;
            }
            var location = target.position;
            var dist = Vector3.Distance(location, thisTurret.position);
            return dist;
        }
        #endregion

        public static Vector3 ComponentPerpendicularTo(this Vector3 u, Vector3 v)
        {
            //https://math.stackexchange.com/questions/1455740/resolve-u-into-components-that-are-parallel-and-perpendicular-to-any-other-nonze
            var perpendicularComponent = u - u.ComponentParalellTo(v);
            return perpendicularComponent;
        }

        public static Vector3 ComponentParalellTo(this Vector3 u, Vector3 v)
        {
            //https://math.stackexchange.com/questions/1455740/resolve-u-into-components-that-are-parallel-and-perpendicular-to-any-other-nonze
            var numerator = Vector3.Dot(u, v);
            var denominator = Vector3.Dot(v, v);
            var division = numerator / denominator;

            var paralell = (division * v);
            return paralell;
        }
    }
}
