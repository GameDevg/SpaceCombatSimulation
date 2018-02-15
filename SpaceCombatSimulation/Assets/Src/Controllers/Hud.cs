﻿using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Src.Interfaces;
using Assets.Src.Targeting;

namespace Assets.Src.Controllers
{
    public class Hud : MonoBehaviour
    {
        /// <summary>
        /// tag of a child object of a fhing to watch or follow.
        /// </summary>
        public List<string> MainTags = new List<string> { "SpaceShip" };
        public List<string> SecondaryTags = new List<string> { "Projectile" };
        private List<string> _allTags = new List<string> { "SpaceShip", "Projectile" };
        
        public Camera Camera;
        
        public Texture ReticleTexture;
        public Texture HealthFGTexture;
        public Texture HealthBGTexture;

        public ReticleState ShowReticles = ReticleState.ALL;

        public float MinShowDistanceDistance = 20;

        private ITargetDetector _detector;

        void Start()
        {
            _detector = new ChildTagTargetDetector
            {
                Tags = _allTags
            };
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                CycleReticleState();
            }
        }
            public void OnGUI()
        {
            DrawHealthBars();
        }

        private void DrawHealthBars()
        {
            if (ShowReticles != ReticleState.NONE)
            {
                var targets = _detector.DetectTargets();

                foreach (var target in targets)
                {
                    DrawSingleLable(target);
                }
            }
        }

        private void DrawSingleLable(PotentialTarget target)
        {
            // Find the 2D position of the object using the main camera
            Vector3 boxPosition = Camera.main.WorldToScreenPoint(target.Transform.position);
            if (boxPosition.z > 0)
            {
                var distance = Vector3.Distance(Camera.transform.position, target.Transform.position);

                // "Flip" it into screen coordinates
                boxPosition.y = Screen.height - boxPosition.y;

                //Draw the distance from the followed object to this object - only if it's suitably distant, and has no parent.
                if (distance > MinShowDistanceDistance && target.Transform.parent == null)
                {
                    GUI.Box(new Rect(boxPosition.x - 20, boxPosition.y + 25, 40, 40), Math.Round(distance).ToString());
                }

                var rect = new Rect(boxPosition.x - 50, boxPosition.y - 50, 100, 100);
                if (ReticleTexture != null)
                    GUI.DrawTexture(rect, ReticleTexture);

                var healthControler = target.Transform.GetComponent<HealthControler>();
                if (healthControler != null && healthControler.IsDamaged)
                {
                    if (HealthBGTexture != null)
                        GUI.DrawTexture(rect, HealthBGTexture);
                    if (HealthFGTexture != null)
                    {
                        rect.width *= healthControler.HealthProportion;
                        GUI.DrawTexture(rect, HealthFGTexture);
                    }
                    //Debug.Log(boxPosition.z + "--x--" + boxPosition.x + "----y--" + boxPosition.y);
                }
            }
        }
    
        private void CycleReticleState()
        {
            switch (ShowReticles)
            {
                case ReticleState.NONE:
                    ShowReticles = ReticleState.ALL;
                    _allTags = new List<string>();
                    _allTags.AddRange(MainTags);
                    _allTags.AddRange(SecondaryTags);
                    _detector = new ChildTagTargetDetector
                    {
                        Tags = _allTags
                    };
                    break;
                case ReticleState.ALL:
                    ShowReticles = ReticleState.MAIN;
                    _allTags = MainTags;
                    _detector = new ChildTagTargetDetector
                    {
                        Tags = _allTags
                    };
                    break;
                case ReticleState.MAIN:
                    ShowReticles = ReticleState.NONE;
                    break;
            }
            Debug.Log(ShowReticles);
        }

        public enum ReticleState
        {
            NONE, MAIN, ALL
        }
    }
}