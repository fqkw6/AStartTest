﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Lockstep
{
	public class LSAnimator : LSAnimatorBase
	{
		[SerializeField]
		private string idling = "idling";

		[SerializeField]
		private string moving = "moving";

		[SerializeField]
		private string engaging = "engaging";

		[SerializeField]
		private string dying = "dying";

		[Space(10f), SerializeField]
		private string fire = "fire";

		private AnimationClip idlingClip;
		private  AnimationClip movingClip;
		private  AnimationClip engagingClip;
		private  AnimationClip dyingClip;
		private  AnimationClip fireClip;

		private Animation animator;

		public override void Setup()
		{
			base.Setup();

		}

		public override void Initialize()
		{
			base.Initialize();
            animator = GetComponent<Animation>();
            if (animator == null)
                animator = this.GetComponentInChildren<Animation>();
            if (CanAnimate = (animator != null))
            {
                //States
                idlingClip = animator.GetClip(idling);
                movingClip = animator.GetClip(moving);
                engagingClip = animator.GetClip(engaging);
                dyingClip = animator.GetClip(dying);
                //Impulses
                fireClip = animator.GetClip(fire);
            }
			Play(AnimState.Idling);
		}

		public override void Play(AnimState state)
		{
			base.Play(state);

			if (CanAnimate)
			{
				AnimationClip clip = GetStateClip(state);
				if (clip.IsNotNull())
				{
					animator.CrossFade(clip.name);
				}
			}
		}

		public override void Play(AnimImpulse impulse, int rate = 0)
		{
			base.Play(impulse, rate);

			if (CanAnimate)
			{ 
				AnimationClip clip = GetImpulseClip(impulse);
				if (clip.IsNotNull())
				{
					animator.Blend(clip.name);
				}
			}
		}

		private AnimationClip GetStateClip(AnimState state)
		{
			switch (state)
			{
				case AnimState.Moving:
					return movingClip;
				case AnimState.Idling:
					return idlingClip;
				case AnimState.Engaging:
					return engagingClip;
				case AnimState.Dying:
					return dyingClip;
			}
			return idlingClip;
		}

		public string GetStateName(AnimState state)
		{
			switch (state)
			{
				case AnimState.Moving:
					return moving;
				case AnimState.Idling:
					return idling;
				case AnimState.Engaging:
					return engaging;
				case AnimState.Dying:
					return dying;
			}
			return idling;
		}

		public string GetImpulseName(AnimImpulse impulse)
		{
			switch (impulse)
			{
				case AnimImpulse.Fire:
					return fire;

			}
			return idling;
		}

		private AnimationClip GetImpulseClip(AnimImpulse impulse)
		{
			switch (impulse)
			{
				case AnimImpulse.Fire:
					return fireClip;

			}
			return idlingClip;
		}
	}
}