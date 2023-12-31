using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool super_perception;
		public bool super_speed;
		public bool paused;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
        private CursorLockMode oldLockState;
        private bool oldVisible;
        private float oldTimeScale;
        private float oldFixedDeltaTime;
        private GameObject pause_ui;


#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnSuperSpeed(InputValue value) {
			SuperSpeedInput(value.isPressed);
		}

		public void OnSuperSprint(InputValue value) {
			SuperSprintInput(value.isPressed);
		}

		public void OnPauseInput(InputValue value) {
			Debug.Log("pause button pressed (InputValue)");
			PauseInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void SuperSpeedInput(bool newSuperSpeedState)
		{
			super_perception = newSuperSpeedState;
		}

		public void SuperSprintInput(bool newSuperSprintState)
		{
			super_speed = newSuperSprintState;
		}

		public void Start() {
			pause_ui = GameObject.FindGameObjectWithTag("GAME_PAUSE");
			pause_ui.SetActive(false);
		}

		public void PauseInput(bool newPauseState) {
			Debug.Log("pause button pressed");
			paused = !paused;
			if (paused) {
				oldLockState = Cursor.lockState;
				oldVisible = Cursor.visible;
				oldTimeScale = Time.timeScale;
				oldFixedDeltaTime = Time.fixedDeltaTime;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				Time.timeScale = 0;
				Time.fixedDeltaTime = 0;
				pause_ui.SetActive(true);
			} else {
				pause_ui.SetActive(false);
				Cursor.lockState = oldLockState;
				Cursor.visible = oldVisible;
				Time.timeScale = oldTimeScale;
				Time.fixedDeltaTime = oldFixedDeltaTime;
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			cursorLocked = hasFocus;
		}

		void Update() {
			if (Gamepad.current == null) {
				cursorLocked = false;
			}

			Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
		}
	}
	
}