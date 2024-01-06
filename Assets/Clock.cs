using UnityEngine;
using UnityEngine.InputSystem;

namespace SuperSpeed {

    [AddComponentMenu("SuperSpeed/Clock")]
    public class Clock : MonoBehaviour {

        private float scale = 1.0f;
        private float time_scale;
        private float fixed_delta_time;
        private float adjusted_time_scale;
        private float adjusted_fixed_delta_time;

        internal static Clock instance;

        public float Scale { get => scale; set {} }

        public float Adjusted_time_scale { get => adjusted_time_scale; set {} }
        public float Adjusted_fixed_delta_time { get => adjusted_fixed_delta_time; set {} }

        // https://gist.github.com/rzubek/e1b73e2262f56e04f9f979a8203bf0c7
        // runtime scripting

        void Start() {
            instance = this;
            time_scale = Time.timeScale;
            fixed_delta_time = Time.fixedDeltaTime;
            changeScale(1.0f);
        }

        private bool low_fps = false;

        public void changeScale(float newScale) {
            scale = newScale;

            adjusted_time_scale = time_scale * scale;
            adjusted_fixed_delta_time = fixed_delta_time * scale;

            Time.timeScale = adjusted_time_scale;
            Time.fixedDeltaTime = adjusted_fixed_delta_time;

            // Unity enforces that maximumDeltaTime is always at least as large as Time.fixedDeltaTime.
            Time.maximumDeltaTime = fixed_delta_time;

            if (Keyboard.current.fKey.isPressed) {
                low_fps = !low_fps;
            }

            if (low_fps) {
                GameSettings.render_fps = 10;
            } else {
                GameSettings.render_fps = 60;
            }
            Application.targetFrameRate = GameSettings.render_fps;
        }
    }
}