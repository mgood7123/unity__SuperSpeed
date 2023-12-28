using UnityEngine;
using UnityEngine.InputSystem;

//public class world_time : MonoBehaviour
//{
//    public static float time_scale = 1.0f;
//    void Start()
//    {

//    }

//    void Update()
//    {
//        Time.timeScale = time_scale;
//    }
//}

namespace SuperSpeed {

    [AddComponentMenu("SuperSpeed/Clock")]
    public class Clock : MonoBehaviour {

        public float scale = 1.0f;

        public float time_scale = 1.0f;
        public float adjusted_time_scale;

        public float fixed_delta_time = 1.0f;
        public float adjusted_fixed_delta_time;

        public float delta_time;
        public float adjusted_movement_delta_time;
        public float adjusted_camera_delta_time;

        public float player_accel;
        public float adjusted_player_accel;

        public float animation_speed;
        public float adjusted_animation_speed;

        Quaternion quaternion = new Quaternion();
        Vector3 vector3 = new Vector3();
        internal static Clock instance;

        // https://gist.github.com/rzubek/e1b73e2262f56e04f9f979a8203bf0c7
        // runtime scripting

        void Start() {
            instance = this;
            time_scale = Time.timeScale;
            fixed_delta_time = Time.fixedDeltaTime;
            delta_time = Time.deltaTime;
        }

        void Update() {
            if ((Gamepad.current != null && Gamepad.current.leftShoulder.isPressed) || (Keyboard.current != null && Keyboard.current.enterKey.isPressed)) {
                scale = 0.05f;
            } else {
                scale = 1.0f;
            }

            adjusted_time_scale = time_scale * scale;
            adjusted_fixed_delta_time = fixed_delta_time * scale;
            delta_time = Time.deltaTime;

            Time.timeScale = adjusted_time_scale;
            Time.fixedDeltaTime = adjusted_fixed_delta_time;

            //Rigidbody[] rigidbodies = gameObject.GetComponents<Rigidbody>();

            //foreach (Rigidbody rb in rigidbodies) {
            //    quaternion.x = rb.rotation.x * time_scale;
            //    quaternion.y = rb.rotation.y * time_scale;
            //    quaternion.z = rb.rotation.z * time_scale;
            //    quaternion.w = rb.rotation.w * time_scale;
            //    rb.rotation = quaternion;
            //    vector3.x = rb.velocity.x * time_scale;
            //    vector3.y = rb.velocity.y * time_scale;
            //    vector3.z = rb.velocity.z * time_scale;
            //    rb.velocity = vector3;
            //}
        }
    }
}