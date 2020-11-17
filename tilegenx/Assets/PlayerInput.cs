/**
  * @file PlayerInput.cs
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
    public class PlayerInput : MonoBehaviour
    {
        /// <summary>
        /// This is the key used to move positively along the X axys.
        /// </summary>
        public KeyCode xPositiveInput;
        /// <summary>
        /// This is the key used to move negatively along the X axys.
        /// </summary>
        public KeyCode xNegativeInput;

        /// <summary>
        /// This is the key used to move positively along the Y axys.
        /// </summary>
        public KeyCode yPositiveInput;
        /// <summary>
        /// This is the key used to move negatively along the Y axys.
        /// </summary>
        public KeyCode yNegativeInput;

        public static sbyte xInput { get; private set; }
        public static sbyte yInput { get; private set; }

        public static float inputTime { get; private set; }

        public static Vector2 mouseInput { get; private set; }

        public static Camera cam { get; private set; }

        private void Start()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        void Update()
        {
            XInput(xPositiveInput, xNegativeInput);
            YInput(yPositiveInput, yNegativeInput);

            //Debug.Log("X: " + xInput + ", Y:" + yInput);
            InputTime(xPositiveInput, xNegativeInput, yPositiveInput, yNegativeInput);

            InputMouse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xInputPositive"></param>
        /// <param name="xInputNegative"></param>
        private static void XInput(KeyCode xInputPositive, KeyCode xInputNegative)
        {
            xInput = xInput != 0 ? (sbyte)0 : xInput;
            if (Input.GetKey(xInputPositive) || Input.GetKeyDown(xInputPositive))
            {
                xInput = xInput != 1 ? (sbyte)1 : xInput;
                return;
            }
            if (Input.GetKey(xInputNegative) || Input.GetKeyDown(xInputNegative))
            {
                xInput = xInput != -1 ? (sbyte)-1 : xInput;
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="yInputPositive"></param>
        /// <param name="yInputNegative"></param>
        private static void YInput(KeyCode yInputPositive, KeyCode yInputNegative)
        {
            yInput = yInput != 0 ? (sbyte)0 : yInput;
            if (Input.GetKey(yInputPositive) || Input.GetKeyDown(yInputPositive))
            {
                yInput = yInput != 1 ? (sbyte)1 : yInput;
                return;
            }
            if (Input.GetKey(yInputNegative) || Input.GetKeyDown(yInputNegative))
            {
                yInput = yInput != -1 ? (sbyte)-1 : yInput;
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPositiveInput"></param>
        /// <param name="xNegativeInput"></param>
        /// <param name="yPositiveInput"></param>
        /// <param name="yNegativeInput"></param>
        private static void InputTime(KeyCode xPositiveInput, KeyCode xNegativeInput, KeyCode yPositiveInput, KeyCode yNegativeInput)
        {
            if (Input.GetKey(xPositiveInput) || Input.GetKey(xNegativeInput) || Input.GetKey(yPositiveInput) || Input.GetKey(yNegativeInput))
            {
                inputTime += Time.deltaTime;
            }
            else
            {
                inputTime = inputTime != 0f ? 0f : inputTime;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private static void InputMouse()
        {
            mouseInput = cam.ScreenToWorldPoint(Input.mousePosition);
        }


        /// <summary>
        /// Change current animator animation to newState. 
        /// Return true if newState is equal to the current state of the animator.
        /// </summary>
        /// <param name="newState">The new animation state the animator will play.</param>
        /// <returns></returns>
        public static byte GetInput(KeyCode newInput)
        {
            byte result;

            result = Input.GetKey(newInput) ? (byte)1 : (byte)0;

            return result;
        }
    }
}


