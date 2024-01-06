using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class BasicRigidBodyRagdoll : MonoBehaviour
{
	public LayerMask ragdollFromLayers;
	public bool canRagdoll;
	public bool forceRagdoll;

	internal bool in_ragdoll;

	internal int ragdoll_duration_ms = 1000;
	internal int ragdoll_ms_remaining;

	internal bool ragdoll_was_forced;

	private void OnTriggerEnter(Collider collider) {
		// object moves towards player
		foreach(Collider c in ragdoll_colliders) {
			if (c.gameObject == collider.gameObject) return;
		}
		if (!ragdoll_was_forced && canRagdoll) {
			Ragdoll(collider);
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit) {
		// player moves towards object
	}

	internal int old_fps = 0;
	internal float ms_per_frame__60;
	internal float ms_per_frame__world_scale;
	internal float ms_per_frame__player_scale;
	internal float ms_per_frame__world_scale_player_scale;

	void Update() {
		if (forceRagdoll) {
			ragdoll_was_forced = true;
			ragdoll_ms_remaining = ragdoll_duration_ms;
			turn_ragdoll_on();
		} else {
			if (ragdoll_was_forced) {
				turn_ragdoll_off();
				ragdoll_was_forced = false;
			}
		}

		ms_per_frame__60 = 1000.0f / GameSettings.timer_fps;
		ms_per_frame__world_scale = ms_per_frame__60 * cached_third_person_controller.world_current_scale;
		ms_per_frame__player_scale = ms_per_frame__60 * cached_third_person_controller.player_current_scale;
		ms_per_frame__world_scale_player_scale = ms_per_frame__world_scale * cached_third_person_controller.player_current_scale;

		if (in_ragdoll) {
			if (ragdoll_ms_remaining >= (int)ms_per_frame__world_scale_player_scale) {
				ragdoll_ms_remaining -= (int)ms_per_frame__world_scale_player_scale;
			} else {
				ragdoll_ms_remaining = 0;
			}

			if (ragdoll_ms_remaining == 0) {
				turn_ragdoll_off();
			}
		}
	}

	ThirdPersonController cached_third_person_controller;
	CharacterController cached_character_controller;
	Animator cached_animator;

	List<Collider> ragdoll_colliders = new List<Collider>();
	List<Rigidbody> ragdoll_bodies = new List<Rigidbody>();

	void Awake() {
		Collider[] colliders = GetComponentsInChildren<Collider>();
		
		foreach(Collider c in colliders) {
			if (c.gameObject != gameObject) {
				ragdoll_colliders.Add(c);
			}
		}

		Rigidbody[] rigid_bodies = GetComponentsInChildren<Rigidbody>();
		
		foreach(Rigidbody r in rigid_bodies) {
			if (r.gameObject != gameObject) {
				ragdoll_bodies.Add(r);
			}
		}

		foreach(Collider c in ragdoll_colliders) {
			c.isTrigger = true;
		}

		foreach(Rigidbody r in ragdoll_bodies) {
			r.isKinematic = true;
		}
	}

	void turn_ragdoll_on() {
		if (cached_animator != null) {
			cached_animator.enabled = false;
		}

		if (cached_character_controller != null) {
			cached_character_controller.enabled = false;
		}

		foreach(Collider c in ragdoll_colliders) {
			c.isTrigger = false;
		}

		foreach(Rigidbody r in ragdoll_bodies) {
			r.isKinematic = false;
		}

		in_ragdoll = true;
	}

	void turn_ragdoll_off() {
		foreach(Rigidbody r in ragdoll_bodies) {
			r.isKinematic = true;
		}

		foreach(Collider c in ragdoll_colliders) {
			c.isTrigger = true;
		}

		if (cached_character_controller != null) {
			cached_character_controller.enabled = true;
		}

		if (cached_animator != null) {
			cached_animator.enabled = true;
		}

		in_ragdoll = false;
	}

	void Start() {
		if (cached_animator == null) {
			cached_animator = GetComponent<Animator>();
		}

		if (cached_character_controller == null) {
			cached_character_controller = GetComponent<CharacterController>();
		}

		if (cached_third_person_controller == null) {
			cached_third_person_controller = GetComponent<ThirdPersonController>();
		}

		turn_ragdoll_off();

		canRagdoll = true;
		forceRagdoll = false;
	}

	private void Ragdoll(Collider collider)
	{
		Rigidbody body = collider.attachedRigidbody;
		if (body == null) return;

		var bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & ragdollFromLayers.value) == 0) return;

		ragdoll_ms_remaining = ragdoll_duration_ms;
		turn_ragdoll_on();
	}
}