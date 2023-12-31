﻿using Tayx.Graphy.Utils.NumString;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets {
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        private float CameraAngleOverride = 0.0f;

        private bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse {
            get {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake() {
            // get a reference to our main camera
            if (_mainCamera == null) {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start() {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            ragdoll = GetComponent<BasicRigidBodyRagdoll>();

            AssignAnimationIDs();


            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private float player_speed;
        private float animation_speed;
        private float camera_rotation_speed;

        private BasicRigidBodyRagdoll ragdoll;

        private void Update() {
            UpdateText();
            ManageSpeed();
            if (!ragdoll.in_ragdoll) {
                JumpAndGravity();
                GroundedCheck();
                Move();
            }
        }

        private void LateUpdate() {
            CameraRotation();
        }

        private void AssignAnimationIDs() {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        [Header("Player Time Scale")]
        public float min_scale = 0.0005f;
        public float max_scale = 1.0f;
        public float world_current_scale = 1.0f;
        public float player_current_scale = 1.0f;
        bool was_super_speed = false;
        bool was_fast_perception = false;

        private void ManageSpeed() {

            // we interpolate the scale to prevent teleporting during activation/deactivation

            if (_input.super_speed) {
                // player must counteract world speed
                // safe speed-up matters
                was_super_speed = true;
                was_fast_perception = false;
                float delta = Time.deltaTime * (30.0f / SuperSpeed.Clock.instance.Scale);
                world_current_scale = Mathf.SmoothStep(world_current_scale, min_scale, delta);
                float delta2 = Time.deltaTime * (20.0f / SuperSpeed.Clock.instance.Scale);
                player_current_scale = Mathf.SmoothStep(player_current_scale, max_scale / world_current_scale, delta2);
                SuperSpeed.Clock.instance.changeScale(world_current_scale);
                player_speed = player_current_scale;
            } else if (_input.super_perception) {
                // doesnt matter too much here, player must move slow
                was_super_speed = false;
                was_fast_perception = true;
                float delta = Time.deltaTime * (30.0f / SuperSpeed.Clock.instance.Scale);
                world_current_scale = Mathf.SmoothStep(world_current_scale, min_scale, delta);
                float delta2 = Time.deltaTime * (10.0f / SuperSpeed.Clock.instance.Scale);
                player_current_scale = Mathf.SmoothStep(player_current_scale, max_scale, delta2);
                SuperSpeed.Clock.instance.changeScale(world_current_scale);
                player_speed = player_current_scale;
            } else {
                if (was_super_speed) {
                    // player must counteract world speed
                    // safe slow-up matters
                    float delta = Time.deltaTime * (5.0f / SuperSpeed.Clock.instance.Scale);
                    world_current_scale = Mathf.SmoothStep(world_current_scale, max_scale, delta);
                    if (player_current_scale > 10.0f) {
                        player_current_scale = 10.0f;
                    }
                    float delta2 = Time.deltaTime * (10.0f / SuperSpeed.Clock.instance.Scale);
                    player_current_scale = Mathf.SmoothStep(player_current_scale, max_scale, delta2);
                    SuperSpeed.Clock.instance.changeScale(world_current_scale);
                    player_speed = player_current_scale;
                } else {
                    // doesnt matter too much here, player already moving slow
                    float delta = Time.deltaTime * (5.0f / SuperSpeed.Clock.instance.Scale);
                    world_current_scale = Mathf.SmoothStep(world_current_scale, max_scale, delta);
                    player_current_scale = 1.0f;
                    SuperSpeed.Clock.instance.changeScale(world_current_scale);
                    player_speed = player_current_scale;
                }
            }

            animation_speed = player_speed;

            camera_rotation_speed = 1.0f / SuperSpeed.Clock.instance.Scale;
        }

        private void GroundedCheck() {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void CameraRotation() {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition) {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime * camera_rotation_speed;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        void UpdateText() {
            float mag = _controller.velocity.magnitude;
            float mph = mag * 2.237f;
            float kmh = mag * 3.6f;

            // TODO: compute percieved time (in relation to real time)
            // TODO: world scale: 0.5
            // TODO: real time: 1 sec
            // TODO: perc time: 500 ms
            long ns_in_second = 1000 * 1000 * 1000;

            player_kmh_view.instance.text.text =
            ""
            + kmh.ToInt() + " KMH\n"
            + mph.ToInt() + " MPH\n"
            + "velocity: " + _controller.velocity + "\n"
            + "velocity.magnitude: " + mag + "\n"
            + "player speed: " + player_current_scale + "\n"
            + "world speed:  " + world_current_scale + "\n"
            + "\n"
            // + "1 second in game @ 1.0 world speed: " + ((1 / (1.0f / GameSettings.timer_fps)) / GameSettings.timer_fps) + "\n"
            // + "1 second in game @ " + world_current_scale + " world speed: " + ((1 / (world_current_scale / GameSettings.timer_fps)) / GameSettings.timer_fps) + "\n"
            + "1 Second\n"
            + "-------------"
            + "1 Second\n"
            + "\n"
            + "[ Percieved Time Until Next Real Second ]\n"
            + "Hours : Minutes : Seconds : Milliseconds : Microseconds : Nanoseconds\n"
            + "00       : 00         : 00           : 00                 : 00                   : 00\n"
            ;
        }

        private void Move() {

            // https://discussions.unity.com/t/how-can-i-make-an-on-screen-speedometer/2975/3

            float anim_speed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) anim_speed = 0.0f;
            float targetSpeed = anim_speed * player_speed;

            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            _speed = targetSpeed * inputMagnitude;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed / player_speed, Time.deltaTime * (SpeedChangeRate * player_speed));
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero) {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;

                transform.rotation = Quaternion.Euler(0.0f, Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime / player_speed), 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime * player_speed);

            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            _animator.speed = 1.0f * animation_speed;
        }

        private void JumpAndGravity() {

            if (Grounded) {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f) {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f) {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    _animator.SetBool(_animIDJump, true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f) {
                    _jumpTimeoutDelta -= Time.deltaTime * player_speed;
                }
            } else {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f) {
                    _fallTimeoutDelta -= Time.deltaTime * player_speed;
                } else {
                    // update animator if using character
                    _animator.SetBool(_animIDFreeFall, true);
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity) {
                _verticalVelocity += Gravity * Time.deltaTime * player_speed;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected() {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent) {
            if (animationEvent.animatorClipInfo.weight > 0.5f) {
                if (FootstepAudioClips.Length > 0) {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent) {
            if (animationEvent.animatorClipInfo.weight > 0.5f) {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}