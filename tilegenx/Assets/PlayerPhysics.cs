/**
  * @file PlayerPhysics.cs
  * @author Daniel Molinero
  * @version 0.1.0
  * @section Copyright © <2020+> <Daniel Molinero>
  * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
  * to deal in the Software without restriction, including without limitation the rights to use, copy,
  * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
  * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
  *
  * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
  *
  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  **/

using UnityEngine;

namespace Player
{
    public class PlayerPhysics : MonoBehaviour
    {
        /// <summary>
        /// This is the <see cref="Rigidbody2D"/> attached to the player.
        /// </summary>
        public static Rigidbody2D rb = null;
        /// <summary>
        /// This is the <see cref="CapsuleCollider2D"/> attached to the player.
        /// </summary>
        public static new CapsuleCollider2D collider { get; private set; }
        /// <summary>
        /// This is the base speed applied to the movement in units per second.
        /// </summary>
        public float speed;
        /// <summary>
        /// This is the curve applied to <see cref="speed"/>. 
        /// </summary>
        public AnimationCurve curve;

        public static bool stopMovement;
        public static Vector2 playerPosition { get; private set; }

        private void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();

            collider = gameObject.GetComponent<CapsuleCollider2D>();
        }
        private void Update()
        {
            playerPosition = transform.position;
        }
        private void FixedUpdate()
        {
            if (stopMovement != true)
            {
                rb.velocity = new Vector2(PlayerInput.xInput * speed * curve.Evaluate(PlayerInput.inputTime),
                PlayerInput.yInput * speed * curve.Evaluate(PlayerInput.inputTime));
            }

        }
    }
}

