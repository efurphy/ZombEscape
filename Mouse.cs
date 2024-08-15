using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombEscape
{
    internal static class Mouse
    {
        public static bool LeftButtonDown { get; private set; } // is the left mouse button held down?
        private static bool LeftButtonDownPrev { get; set; } // leftButtonDown, but from the previous Update call.
        public static bool LeftButtonClicked { get; private set; } // was the left mouse button clicked on this frame?
        public static bool LeftButtonReleased { get; private set; } // was the left mouse button released on this frame?

        // should be called every frame.
        public static void Update()
        {
            LeftButtonDownPrev = LeftButtonDown;
            LeftButtonDown = (Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == ButtonState.Pressed);
            LeftButtonClicked = (LeftButtonDown && !LeftButtonDownPrev);
            LeftButtonReleased = (!LeftButtonDown && LeftButtonDownPrev);
        }
    }
}
