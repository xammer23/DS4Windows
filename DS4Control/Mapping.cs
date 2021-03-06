﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS4Library;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DS4Control
{
    public class Mapping
    {
        /*
         * Represent the synthetic keyboard and mouse events.  Maintain counts for each so we don't duplicate events.
         */
        public class SyntheticState
        {
            public struct MouseClick
            {
                public int leftCount, middleCount, rightCount, fourthCount, fifthCount, wUpCount, wDownCount, toggleCount;
                public bool toggle;
            }
            public MouseClick previousClicks, currentClicks;
            public struct KeyPress
            {
                public int vkCount, scanCodeCount, repeatCount, toggleCount; // repeat takes priority over non-, and scancode takes priority over non-
                public bool toggle;
            }
            public class KeyPresses
            {
                public KeyPress previous, current;
            }
            public Dictionary<UInt16, KeyPresses> keyPresses = new Dictionary<UInt16, KeyPresses>();

            public void SavePrevious(bool performClear)
            {
                previousClicks = currentClicks;
                if (performClear)
                    currentClicks.leftCount = currentClicks.middleCount = currentClicks.rightCount = currentClicks.fourthCount = currentClicks.fifthCount = currentClicks.wUpCount = currentClicks.wDownCount = currentClicks.toggleCount = 0;
                foreach (KeyPresses kp in keyPresses.Values)
                {
                    kp.previous = kp.current;
                    if (performClear)
                    {
                        kp.current.repeatCount = kp.current.scanCodeCount = kp.current.vkCount = kp.current.toggleCount = 0;
                        //kp.current.toggle = false;
                    }
                }
            }
        }
        public static SyntheticState globalState = new SyntheticState();
        public static SyntheticState[] deviceState = { new SyntheticState(), new SyntheticState(), new SyntheticState(), new SyntheticState() };

        // TODO When we disconnect, process a null/dead state to release any keys or buttons.
        public static DateTime oldnow = DateTime.UtcNow;
        private static bool pressagain = false;
        private static int wheel = 0, keyshelddown = 0;
        public static void Commit(int device)
        {
            SyntheticState state = deviceState[device];
            lock (globalState)
            {
                globalState.currentClicks.leftCount += state.currentClicks.leftCount - state.previousClicks.leftCount;
                globalState.currentClicks.middleCount += state.currentClicks.middleCount - state.previousClicks.middleCount;
                globalState.currentClicks.rightCount += state.currentClicks.rightCount - state.previousClicks.rightCount;
                globalState.currentClicks.fourthCount += state.currentClicks.fourthCount - state.previousClicks.fourthCount;
                globalState.currentClicks.fifthCount += state.currentClicks.fifthCount - state.previousClicks.fifthCount;
                globalState.currentClicks.wUpCount += state.currentClicks.wUpCount - state.previousClicks.wUpCount;
                globalState.currentClicks.wDownCount += state.currentClicks.wDownCount - state.previousClicks.wDownCount;
                globalState.currentClicks.toggleCount += state.currentClicks.toggleCount - state.previousClicks.toggleCount;
                globalState.currentClicks.toggle = state.currentClicks.toggle;

                if (globalState.currentClicks.toggleCount != 0 && globalState.previousClicks.toggleCount == 0 && globalState.currentClicks.toggle)
                {
                    if (globalState.currentClicks.leftCount != 0 && globalState.previousClicks.leftCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTDOWN);
                    if (globalState.currentClicks.rightCount != 0 && globalState.previousClicks.rightCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTDOWN);
                    if (globalState.currentClicks.middleCount != 0 && globalState.previousClicks.middleCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEDOWN);
                    if (globalState.currentClicks.fourthCount != 0 && globalState.previousClicks.fourthCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 1);
                    if (globalState.currentClicks.fifthCount != 0 && globalState.previousClicks.fifthCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 2);
                }
                else if (globalState.currentClicks.toggleCount != 0 && globalState.previousClicks.toggleCount == 0 && !globalState.currentClicks.toggle)
                {
                    if (globalState.currentClicks.leftCount != 0 && globalState.previousClicks.leftCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTUP);
                    if (globalState.currentClicks.rightCount != 0 && globalState.previousClicks.rightCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTUP);
                    if (globalState.currentClicks.middleCount != 0 && globalState.previousClicks.middleCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEUP);
                    if (globalState.currentClicks.fourthCount != 0 && globalState.previousClicks.fourthCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 1);
                    if (globalState.currentClicks.fifthCount != 0 && globalState.previousClicks.fifthCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 2);
                }

                if (globalState.currentClicks.toggleCount == 0 && globalState.previousClicks.toggleCount == 0)
                {
                    if (globalState.currentClicks.leftCount != 0 && globalState.previousClicks.leftCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTDOWN);
                    else if (globalState.currentClicks.leftCount == 0 && globalState.previousClicks.leftCount != 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTUP);

                    if (globalState.currentClicks.middleCount != 0 && globalState.previousClicks.middleCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEDOWN);
                    else if (globalState.currentClicks.middleCount == 0 && globalState.previousClicks.middleCount != 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEUP);

                    if (globalState.currentClicks.rightCount != 0 && globalState.previousClicks.rightCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTDOWN);
                    else if (globalState.currentClicks.rightCount == 0 && globalState.previousClicks.rightCount != 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTUP);

                    if (globalState.currentClicks.fourthCount != 0 && globalState.previousClicks.fourthCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 1);
                    else if (globalState.currentClicks.fourthCount == 0 && globalState.previousClicks.fourthCount != 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 1);

                    if (globalState.currentClicks.fifthCount != 0 && globalState.previousClicks.fifthCount == 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 2);
                    else if (globalState.currentClicks.fifthCount == 0 && globalState.previousClicks.fifthCount != 0)
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 2);

                    if (globalState.currentClicks.wUpCount != 0 && globalState.previousClicks.wUpCount == 0)
                    {
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_WHEEL, 100);
                        oldnow = DateTime.UtcNow;
                        wheel = 100;
                    }
                    else if (globalState.currentClicks.wUpCount == 0 && globalState.previousClicks.wUpCount != 0)
                        wheel = 0;

                    if (globalState.currentClicks.wDownCount != 0 && globalState.previousClicks.wDownCount == 0)
                    {
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_WHEEL, -100);
                        oldnow = DateTime.UtcNow;
                        wheel = -100;
                    }
                    if (globalState.currentClicks.wDownCount == 0 && globalState.previousClicks.wDownCount != 0)
                        wheel = 0;
                }
            

                if (wheel != 0) //Continue mouse wheel movement
                {
                    DateTime now = DateTime.UtcNow;
                    if (now >= oldnow + TimeSpan.FromMilliseconds(100) && !pressagain)
                    {
                        oldnow = now;
                        InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_WHEEL, wheel);
                    }
                }

                // Merge and synthesize all key presses/releases that are present in this device's mapping.
                // TODO what about the rest?  e.g. repeat keys really ought to be on some set schedule
                foreach (KeyValuePair<UInt16, SyntheticState.KeyPresses> kvp in state.keyPresses)
                {
                    SyntheticState.KeyPresses gkp;
                    if (globalState.keyPresses.TryGetValue(kvp.Key, out gkp))
                    {
                        gkp.current.vkCount += kvp.Value.current.vkCount - kvp.Value.previous.vkCount;
                        gkp.current.scanCodeCount += kvp.Value.current.scanCodeCount - kvp.Value.previous.scanCodeCount;
                        gkp.current.repeatCount += kvp.Value.current.repeatCount - kvp.Value.previous.repeatCount;
                        gkp.current.toggle = kvp.Value.current.toggle;
                        gkp.current.toggleCount += kvp.Value.current.toggleCount - kvp.Value.previous.toggleCount;
                    }
                    else
                    {
                        gkp = new SyntheticState.KeyPresses();
                        gkp.current = kvp.Value.current;
                        globalState.keyPresses[kvp.Key] = gkp;
                    }

                    if (gkp.current.toggleCount != 0 && gkp.previous.toggleCount == 0 && gkp.current.toggle)
                    {
                        if (gkp.current.scanCodeCount != 0)
                            InputMethods.performSCKeyPress(kvp.Key);
                        else
                            InputMethods.performKeyPress(kvp.Key);
                    }
                    else if (gkp.current.toggleCount != 0 && gkp.previous.toggleCount == 0 && !gkp.current.toggle)
                    {
                        if (gkp.previous.scanCodeCount != 0) // use the last type of VK/SC
                            InputMethods.performSCKeyRelease(kvp.Key);
                        else
                            InputMethods.performKeyRelease(kvp.Key);
                    }
                    else if (gkp.current.vkCount + gkp.current.scanCodeCount != 0 && gkp.previous.vkCount + gkp.previous.scanCodeCount == 0)
                    {
                        if (gkp.current.scanCodeCount != 0)
                        {
                            oldnow = DateTime.UtcNow;
                            InputMethods.performSCKeyPress(kvp.Key);
                            pressagain = false;
                            keyshelddown = kvp.Key;
                        }
                        else
                        {
                            oldnow = DateTime.UtcNow;
                            InputMethods.performKeyPress(kvp.Key);
                            pressagain = false;
                            keyshelddown = kvp.Key;
                        }
                    }
                    else if (gkp.current.toggleCount != 0 || gkp.previous.toggleCount != 0 || gkp.current.repeatCount != 0 || // repeat or SC/VK transition
                        ((gkp.previous.scanCodeCount == 0) != (gkp.current.scanCodeCount == 0))) //repeat keystroke after 500ms
                    {
                        if (keyshelddown == kvp.Key)
                        {
                            DateTime now = DateTime.UtcNow;
                            if (now >= oldnow + TimeSpan.FromMilliseconds(500) && !pressagain)
                            {
                                oldnow = now;
                                pressagain = true;
                            }
                            if (pressagain && gkp.current.scanCodeCount != 0)
                            {
                                now = DateTime.UtcNow;
                                if (now >= oldnow + TimeSpan.FromMilliseconds(25) && pressagain)
                                {
                                    oldnow = now;
                                    InputMethods.performSCKeyPress(kvp.Key);
                                }
                            }
                            else if (pressagain)
                            {
                                now = DateTime.UtcNow;
                                if (now >= oldnow + TimeSpan.FromMilliseconds(25) && pressagain)
                                {
                                    oldnow = now;
                                    InputMethods.performKeyPress(kvp.Key);
                                }
                            }
                        }
                    }
                    else if ((gkp.current.toggleCount == 0 && gkp.previous.toggleCount == 0) && gkp.current.vkCount + gkp.current.scanCodeCount == 0 && gkp.previous.vkCount + gkp.previous.scanCodeCount != 0)
                    {
                        if (gkp.previous.scanCodeCount != 0) // use the last type of VK/SC
                        {
                            InputMethods.performSCKeyRelease(kvp.Key);
                            //InputMethods.performKeyRelease(kvp.Key);
                            pressagain = false;
                        }
                        else
                        {
                            InputMethods.performKeyRelease(kvp.Key);
                            pressagain = false;
                        }
                    }
                }
                globalState.SavePrevious(false);
            }
            state.SavePrevious(true);
        }
        public enum Click { None, Left, Middle, Right, Fourth, Fifth, WUP, WDOWN };
        public static void MapClick(int device, Click mouseClick)
        {
            switch (mouseClick)
            {
                case Click.Left:
                    deviceState[device].currentClicks.leftCount++;
                    break;
                case Click.Middle:
                    deviceState[device].currentClicks.middleCount++;
                    break;
                case Click.Right:
                    deviceState[device].currentClicks.rightCount++;
                    break;
                case Click.Fourth:
                    deviceState[device].currentClicks.fourthCount++;
                    break;
                case Click.Fifth:
                    deviceState[device].currentClicks.fifthCount++;
                    break;
                case Click.WUP:
                    deviceState[device].currentClicks.wUpCount++;
                    break;
                case Click.WDOWN:
                    deviceState[device].currentClicks.wDownCount++;
                    break;
            }
        }

        /** Map the touchpad button state to mouse or keyboard events. */
        public static void MapTouchpadButton(int device, DS4Controls what, Click mouseEventFallback, DS4State MappedState = null)
        {
            SyntheticState deviceState = Mapping.deviceState[device];
            string macro = Global.getCustomMacro(device, what);
            if (macro != "0")
            { 
            }                  
            else if (Global.getCustomKey(device, what) != 0)
            {
                ushort key = Global.getCustomKey(device, what);  
                DS4KeyType keyType = Global.getCustomKeyType(device, what);
                SyntheticState.KeyPresses kp;
                if (!deviceState.keyPresses.TryGetValue(key, out kp))
                    deviceState.keyPresses[key] = kp = new SyntheticState.KeyPresses();
                if (keyType.HasFlag(DS4KeyType.ScanCode))
                    kp.current.scanCodeCount++;
                else
                    kp.current.vkCount++;
                if (keyType.HasFlag(DS4KeyType.Toggle)) 
                    kp.current.toggleCount++;
            }
            else
            {
                X360Controls button = Global.getCustomButton(device, what);
                switch (button)
                {
                    case X360Controls.None:
                        switch (mouseEventFallback)
                        {
                            case Click.Left:
                                deviceState.currentClicks.leftCount++;
                                return;
                            case Click.Middle:
                                deviceState.currentClicks.middleCount++;
                                return;
                            case Click.Right:
                                deviceState.currentClicks.rightCount++;
                                return;
                            case Click.Fourth:
                                deviceState.currentClicks.fourthCount++;
                                return;
                            case Click.Fifth:
                                deviceState.currentClicks.fifthCount++;
                                return;
                            case Click.WUP:
                                deviceState.currentClicks.wUpCount++;
                                return;
                            case Click.WDOWN:
                                deviceState.currentClicks.wDownCount++;
                                return;
                        }
                        return;
                    case X360Controls.LeftMouse:
                        deviceState.currentClicks.leftCount++;
                        return;
                    case X360Controls.MiddleMouse:
                        deviceState.currentClicks.middleCount++;
                        return;
                    case X360Controls.RightMouse:
                        deviceState.currentClicks.rightCount++;
                        return;
                    case X360Controls.FourthMouse:
                        deviceState.currentClicks.fourthCount++;
                        return;
                    case X360Controls.FifthMouse:
                        deviceState.currentClicks.fifthCount++;
                        return;
                    case X360Controls.WUP:
                        deviceState.currentClicks.wUpCount++;
                        return;
                    case X360Controls.WDOWN:
                        deviceState.currentClicks.wDownCount++;
                        return;

                    case X360Controls.A:
                        MappedState.Cross = true;
                        return;
                    case X360Controls.B:
                        MappedState.Circle = true;
                        return;
                    case X360Controls.X:
                        MappedState.Square = true;
                        return;
                    case X360Controls.Y:
                        MappedState.Triangle = true;
                        return;
                    case X360Controls.LB:
                        MappedState.L1 = true;
                        return;
                    case X360Controls.LS:
                        MappedState.L3 = true;
                        return;
                    case X360Controls.RB:
                        MappedState.R1 = true;
                        return;
                    case X360Controls.RS:
                        MappedState.R3 = true;
                        return;
                    case X360Controls.DpadUp:
                        MappedState.DpadUp = true;
                        return;
                    case X360Controls.DpadDown:
                        MappedState.DpadDown = true;
                        return;
                    case X360Controls.DpadLeft:
                        MappedState.DpadLeft = true;
                        return;
                    case X360Controls.DpadRight:
                        MappedState.DpadRight = true;
                        return;
                    case X360Controls.Guide:
                        MappedState.PS = true;
                        return;
                    case X360Controls.Back:
                        MappedState.Share = true;
                        return;
                    case X360Controls.Start:
                        MappedState.Options = true;
                        return;
                    case X360Controls.LT:
                        if (MappedState.L2 == 0)
                            MappedState.L2 = 255;
                        return;
                    case X360Controls.RT:
                        if (MappedState.R2 == 0)
                            MappedState.R2 = 255;
                        return;

                    case X360Controls.Unbound:
                        return;

                    default:
                        if (MappedState == null)
                            return;
                        break;
                }
            }
        }

        public static int DS4ControltoInt(DS4Controls ctrl)
        {
            switch (ctrl)
            {
                case DS4Controls.Share: return 1;
                case DS4Controls.Options: return 2;
                case DS4Controls.L1: return 3;
                case DS4Controls.R1: return 4;
                case DS4Controls.L3: return 5;
                case DS4Controls.R3: return 6;
                case DS4Controls.DpadUp: return 7;
                case DS4Controls.DpadDown: return 8;
                case DS4Controls.DpadLeft: return 9;
                case DS4Controls.DpadRight: return 10;
                case DS4Controls.PS: return 11;
                case DS4Controls.Cross: return 12;
                case DS4Controls.Square: return 13;
                case DS4Controls.Triangle: return 14;
                case DS4Controls.Circle: return 15;
                case DS4Controls.LXNeg: return 16;
                case DS4Controls.LYNeg: return 17;
                case DS4Controls.RXNeg: return 18;
                case DS4Controls.RYNeg: return 19;
                case DS4Controls.LXPos: return 20;
                case DS4Controls.LYPos: return 21;
                case DS4Controls.RXPos: return 22;
                case DS4Controls.RYPos: return 23;
                case DS4Controls.L2: return 24;
                case DS4Controls.R2: return 25;
                case DS4Controls.TouchMulti: return 26;
                case DS4Controls.TouchLeft: return 27;
                case DS4Controls.TouchRight: return 28;
                case DS4Controls.TouchUpper: return 29;
                case DS4Controls.GyroXNeg: return 30;
                case DS4Controls.GyroXPos: return 31;
                case DS4Controls.GyroZNeg: return 32;
                case DS4Controls.GyroZPos: return 33;
            }
            return 0; 
        }
        public static bool[] pressedonce = new bool[261], macrodone = new bool[34];
        public static int test = 0;
        /** Map DS4 Buttons/Axes to other DS4 Buttons/Axes (largely the same as Xinput ones) and to keyboard and mouse buttons. */
        public static async void MapCustom(int device, DS4State cState, DS4State MappedState, DS4StateExposed eState)
        {
            bool shift;
            cState.CopyTo(MappedState);
            SyntheticState deviceState = Mapping.deviceState[device];
            switch (Global.getShiftModifier(device))
            {
                case 1: shift = getBoolMapping(DS4Controls.Cross, cState, eState); break;
                case 2: shift = getBoolMapping(DS4Controls.Circle, cState, eState); break;
                case 3: shift = getBoolMapping(DS4Controls.Square, cState, eState); break;
                case 4: shift = getBoolMapping(DS4Controls.Triangle, cState, eState); break;
                case 5: shift = getBoolMapping(DS4Controls.Options, cState, eState); break;
                case 6: shift = getBoolMapping(DS4Controls.Share, cState, eState); break;
                case 7: shift = getBoolMapping(DS4Controls.DpadUp, cState, eState); break;
                case 8: shift = getBoolMapping(DS4Controls.DpadDown, cState, eState); break;
                case 9: shift = getBoolMapping(DS4Controls.DpadLeft, cState, eState); break;
                case 10: shift = getBoolMapping(DS4Controls.DpadRight, cState, eState); break;
                case 11: shift = getBoolMapping(DS4Controls.PS, cState, eState); break;
                case 12: shift = getBoolMapping(DS4Controls.L1, cState, eState); break;
                case 13: shift = getBoolMapping(DS4Controls.R1, cState, eState); break;
                case 14: shift = getBoolMapping(DS4Controls.L2, cState, eState); break;
                case 15: shift = getBoolMapping(DS4Controls.R2, cState, eState); break;
                case 16: shift = getBoolMapping(DS4Controls.L3, cState, eState); break;
                case 17: shift = getBoolMapping(DS4Controls.R3, cState, eState); break;
                case 18: shift = getBoolMapping(DS4Controls.TouchLeft, cState, eState); break;
                case 19: shift = getBoolMapping(DS4Controls.TouchUpper, cState, eState); break;
                case 20: shift = getBoolMapping(DS4Controls.TouchMulti, cState, eState); break;
                case 21: shift = getBoolMapping(DS4Controls.TouchRight, cState, eState); break;
                case 22: shift = getBoolMapping(DS4Controls.GyroZNeg, cState, eState); break;
                case 23: shift = getBoolMapping(DS4Controls.GyroZPos, cState, eState); break;
                case 24: shift = getBoolMapping(DS4Controls.GyroXPos, cState, eState); break;
                case 25: shift = getBoolMapping(DS4Controls.GyroXNeg, cState, eState); break;
                case 26: shift = cState.Touch1; break;
                default: shift = false; break;
            }
            if (shift)
                MapShiftCustom(device, cState, MappedState, eState);
            foreach (KeyValuePair<DS4Controls, string> customKey in Global.getCustomMacros(device))
            {
                if (shift == false ||
                    (Global.getShiftCustomMacro(device, customKey.Key) == "0" &&
                    Global.getShiftCustomKey(device, customKey.Key) == 0 &&
                    Global.getShiftCustomButton(device, customKey.Key) == X360Controls.None))
                {
                    DS4KeyType keyType = Global.getCustomKeyType(device, customKey.Key);
                    if (getBoolMapping(customKey.Key, cState, eState))
                    {
                        resetToDefaultValue(customKey.Key, MappedState);
                        bool LXChanged = (Math.Abs(127 - MappedState.LX) < 5);
                        bool LYChanged = (Math.Abs(127 - MappedState.LY) < 5);
                        bool RXChanged = (Math.Abs(127 - MappedState.RX) < 5);
                        bool RYChanged = (Math.Abs(127 - MappedState.RY) < 5);
                        string[] skeys;
                        int[] keys;
                        if (!string.IsNullOrEmpty(customKey.Value))
                        {
                            skeys = customKey.Value.Split('/');
                            keys = new int[skeys.Length];
                        }
                        else
                        {
                            skeys = new string[0];
                            keys = new int[0];
                        }
                        for (int i = 0; i < keys.Length; i++)
                            keys[i] = ushort.Parse(skeys[i]);
                        bool[] keydown = new bool[261];
                        if (keys.Length > 0 && keys[0] > 260 && keys[0] < 300)
                        {
                            if (keys[0] == 261 && !MappedState.Cross) MappedState.Cross = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 262 && !MappedState.Circle) MappedState.Circle = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 263 && !MappedState.Square) MappedState.Square = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 264 && !MappedState.Triangle) MappedState.Triangle = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 265 && !MappedState.DpadUp) MappedState.DpadUp = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 266 && !MappedState.DpadDown) MappedState.DpadDown = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 267 && !MappedState.DpadLeft) MappedState.DpadLeft = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 268 && !MappedState.DpadRight) MappedState.DpadRight = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 269 && !MappedState.Options) MappedState.Options = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 270 && !MappedState.Share) MappedState.Share = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 271 && !MappedState.PS) MappedState.PS = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 272 && !MappedState.L1) MappedState.L1 = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 273 && !MappedState.R1) MappedState.R1 = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 274 && MappedState.L2 == 0) MappedState.L2 = getByteMapping(device, customKey.Key, cState, eState);
                            if (keys[0] == 275 && MappedState.R2 == 0) MappedState.R2 = getByteMapping(device, customKey.Key, cState, eState);
                            if (keys[0] == 276 && !MappedState.L3) MappedState.L3 = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 277 && !MappedState.R3) MappedState.R3 = getBoolMapping(customKey.Key, cState, eState);
                            if (keys[0] == 278 && LYChanged) MappedState.LY = getXYAxisMapping(device, customKey.Key, cState, eState);
                            if (keys[0] == 279 && LYChanged) MappedState.LY = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                            if (keys[0] == 280 && LXChanged) MappedState.LX = getXYAxisMapping(device, customKey.Key, cState, eState);
                            if (keys[0] == 281 && LXChanged) MappedState.LX = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                            if (keys[0] == 282 && RYChanged) MappedState.RY = getXYAxisMapping(device, customKey.Key, cState, eState);
                            if (keys[0] == 283 && RYChanged) MappedState.RY = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                            if (keys[0] == 284 && RXChanged) MappedState.RX = getXYAxisMapping(device, customKey.Key, cState, eState);
                            if (keys[0] == 285 && RXChanged) MappedState.RX = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                        }
                        if (!macrodone[DS4ControltoInt(customKey.Key)])
                        {
                            macrodone[DS4ControltoInt(customKey.Key)] = true;
                            for (int i = 0; i < keys.Length; i++)
                            {
                                if (keys[i] >= 300) //ints over 300 used to delay
                                    await Task.Delay(keys[i] - 300);
                                else if (keys[i] < 261 && !keydown[keys[i]])
                                {
                                    if (keys[i] == 256) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTDOWN); //anything above 255 is not a keyvalue
                                    else if (keys[i] == 257) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTDOWN);
                                    else if (keys[i] == 258) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEDOWN);
                                    else if (keys[i] == 259) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 1);
                                    else if (keys[i] == 260) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 2);
                                    else if (keyType.HasFlag(DS4KeyType.ScanCode))
                                        InputMethods.performSCKeyPress((ushort)keys[i]);
                                    else
                                        InputMethods.performKeyPress((ushort)keys[i]);
                                    keydown[keys[i]] = true;
                                }
                                else if (keys[i] < 261)
                                {
                                    if (keys[i] == 256) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTUP); //anything above 255 is not a keyvalue
                                    else if (keys[i] == 257) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTUP);
                                    else if (keys[i] == 258) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEUP);
                                    else if (keys[i] == 259) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 1);
                                    else if (keys[i] == 260) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 2);
                                    else if (keyType.HasFlag(DS4KeyType.ScanCode))
                                        InputMethods.performSCKeyRelease((ushort)keys[i]);
                                    else
                                        InputMethods.performKeyRelease((ushort)keys[i]);
                                    keydown[keys[i]] = false;
                                }
                            }
                            for (ushort i = 0; i < keydown.Length; i++)
                            {
                                if (keydown[i])
                                    if (keyType.HasFlag(DS4KeyType.ScanCode))
                                        InputMethods.performSCKeyRelease(i);
                                    else
                                        InputMethods.performKeyRelease(i);
                            }
                            if (keyType.HasFlag(DS4KeyType.HoldMacro))
                            {
                                await Task.Delay(50);
                                macrodone[DS4ControltoInt(customKey.Key)] = false;
                            }
                        }
                    }
                    else if (!getBoolMapping(customKey.Key, cState, eState))
                        macrodone[DS4ControltoInt(customKey.Key)] = false;
                }
            }
            foreach (KeyValuePair<DS4Controls, ushort> customKey in Global.getCustomKeys(device))
            {
                if (shift == false ||
                    (Global.getShiftCustomMacro(device, customKey.Key) == "0" &&
                    Global.getShiftCustomKey(device, customKey.Key) == 0 &&
                    Global.getShiftCustomButton(device, customKey.Key) == X360Controls.None))
                {
                    DS4KeyType keyType = Global.getCustomKeyType(device, customKey.Key);
                    if (getBoolMapping(customKey.Key, cState, eState))
                    {
                        resetToDefaultValue(customKey.Key, MappedState);
                        SyntheticState.KeyPresses kp;
                        if (!deviceState.keyPresses.TryGetValue(customKey.Value, out kp))
                            deviceState.keyPresses[customKey.Value] = kp = new SyntheticState.KeyPresses();
                        if (keyType.HasFlag(DS4KeyType.ScanCode))
                            kp.current.scanCodeCount++;
                        else
                            kp.current.vkCount++;
                        if (keyType.HasFlag(DS4KeyType.Toggle))
                        {
                            if (!pressedonce[customKey.Value])
                            {
                                kp.current.toggle = !kp.current.toggle;
                                pressedonce[customKey.Value] = true;
                            }
                            kp.current.toggleCount++;
                        }
                        kp.current.repeatCount++;
                    }
                    else
                        pressedonce[customKey.Value] = false;
                }
            }

            MappedState.LX = 127;
            MappedState.LY = 127;
            MappedState.RX = 127;
            MappedState.RY = 127;
            int MouseDeltaX = 0;
            int MouseDeltaY = 0;

            Dictionary<DS4Controls, X360Controls> customButtons = Global.getCustomButtons(device);
            //foreach (KeyValuePair<DS4Controls, X360Controls> customButton in customButtons)
            // resetToDefaultValue(customButton.Key, MappedState); // erase default mappings for things that are remapped
            foreach (KeyValuePair<DS4Controls, X360Controls> customButton in customButtons)
            {
                if (shift == false ||
                    (Global.getShiftCustomMacro(device, customButton.Key) == "0" &&
                    Global.getShiftCustomKey(device, customButton.Key) == 0 &&
                    Global.getShiftCustomButton(device, customButton.Key) == X360Controls.None))
                {
                    if (customButton.Key == DS4Controls.Square)
                    Console.WriteLine("hello");
                    DS4KeyType keyType = Global.getCustomKeyType(device, customButton.Key);
                    int keyvalue = 0;
                    switch (customButton.Value)
                    {
                        case X360Controls.LeftMouse: keyvalue = 256; break;
                        case X360Controls.RightMouse: keyvalue = 257; break;
                        case X360Controls.MiddleMouse: keyvalue = 258; break;
                        case X360Controls.FourthMouse: keyvalue = 259; break;
                        case X360Controls.FifthMouse: keyvalue = 260; break;
                    }
                    if (keyType.HasFlag(DS4KeyType.Toggle))
                    {
                        if (getBoolMapping(customButton.Key, cState, eState))
                        {
                            resetToDefaultValue(customButton.Key, MappedState);
                            if (!pressedonce[keyvalue])
                            {
                                deviceState.currentClicks.toggle = !deviceState.currentClicks.toggle;
                                test++;
                                pressedonce[keyvalue] = true;
                            }
                            deviceState.currentClicks.toggleCount++;
                        }
                        else// if (test = 1)// && pressedonce[keyvalue])
                        {
                            pressedonce[keyvalue] = false;
                        }
                    }
                    bool LXChanged = Math.Abs(127 - MappedState.LX) <= 5;
                    bool LYChanged = Math.Abs(127 - MappedState.LY) <= 5;
                    bool RXChanged = Math.Abs(127 - MappedState.RX) <= 5;
                    bool RYChanged = Math.Abs(127 - MappedState.RY) <= 5;

                    //once++;

                    resetToDefaultValue(customButton.Key, MappedState); // erase default mappings for things that are remapped
                    switch (customButton.Value)
                    {
                        case X360Controls.A:
                            if (!MappedState.Cross)
                                MappedState.Cross = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.B:
                            if (!MappedState.Circle)
                                MappedState.Circle = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.X:
                            if (!MappedState.Square)
                                MappedState.Square = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.Y:
                            if (!MappedState.Triangle)
                                MappedState.Triangle = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.LB:
                            if (!MappedState.L1)
                                MappedState.L1 = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.LS:
                            if (!MappedState.L3)
                                MappedState.L3 = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.RB:
                            if (!MappedState.R1)
                                MappedState.R1 = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.RS:
                            if (!MappedState.R3)
                                MappedState.R3 = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.DpadUp:
                            if (!MappedState.DpadUp)
                                MappedState.DpadUp = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.DpadDown:
                            if (!MappedState.DpadDown)
                                MappedState.DpadDown = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.DpadLeft:
                            if (!MappedState.DpadLeft)
                                MappedState.DpadLeft = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.DpadRight:
                            if (!MappedState.DpadRight)
                                MappedState.DpadRight = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.Guide:
                            if (!MappedState.PS)
                                MappedState.PS = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.Back:
                            if (!MappedState.Share)
                                MappedState.Share = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.Start:
                            if (!MappedState.Options)
                                MappedState.Options = getBoolMapping(customButton.Key, cState, eState);
                            break;
                        case X360Controls.LXNeg:
                            if (LXChanged)
                                MappedState.LX = getXYAxisMapping(device, customButton.Key, cState, eState);
                            break;
                        case X360Controls.LYNeg:
                            if (LYChanged)
                                MappedState.LY = getXYAxisMapping(device, customButton.Key, cState, eState);
                            break;
                        case X360Controls.RXNeg:
                            if (RXChanged)
                                MappedState.RX = getXYAxisMapping(device, customButton.Key, cState, eState);
                            break;
                        case X360Controls.RYNeg:
                            if (RYChanged)
                                MappedState.RY = getXYAxisMapping(device, customButton.Key, cState, eState);
                            break;
                        case X360Controls.LXPos:
                            if (LXChanged)
                                MappedState.LX = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                            break;
                        case X360Controls.LYPos:
                            if (LYChanged)
                                MappedState.LY = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                            break;
                        case X360Controls.RXPos:
                            if (RXChanged)
                                MappedState.RX = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                            break;
                        case X360Controls.RYPos:
                            if (RYChanged)
                                MappedState.RY = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                            break;
                        case X360Controls.LT:
                            if (MappedState.L2 == 0)
                                MappedState.L2 = getByteMapping(device, customButton.Key, cState, eState);
                            break;
                        case X360Controls.RT:
                            if (MappedState.R2 == 0)
                                MappedState.R2 = getByteMapping(device, customButton.Key, cState, eState);
                            break;
                        case X360Controls.LeftMouse:
                            if (getBoolMapping(customButton.Key, cState, eState))
                                deviceState.currentClicks.leftCount++;
                            break;
                        case X360Controls.RightMouse:
                            if (getBoolMapping(customButton.Key, cState, eState))
                                deviceState.currentClicks.rightCount++;
                            break;
                        case X360Controls.MiddleMouse:
                            if (getBoolMapping(customButton.Key, cState, eState))
                                deviceState.currentClicks.middleCount++;
                            break;
                        case X360Controls.FourthMouse:
                            if (getBoolMapping(customButton.Key, cState, eState))
                                deviceState.currentClicks.fourthCount++;
                            break;
                        case X360Controls.FifthMouse:
                            if (getBoolMapping(customButton.Key, cState, eState))
                                deviceState.currentClicks.fifthCount++;
                            break;
                        case X360Controls.WUP:
                            if (getBoolMapping(customButton.Key, cState, eState))
                                deviceState.currentClicks.wUpCount++;
                            break;
                        case X360Controls.WDOWN:
                            if (getBoolMapping(customButton.Key, cState, eState))
                                deviceState.currentClicks.wDownCount++;
                            break;
                        case X360Controls.MouseUp:
                            if (MouseDeltaY == 0)
                            {
                                MouseDeltaY = getMouseMapping(device, customButton.Key, cState, eState, 0);
                                MouseDeltaY = -Math.Abs((MouseDeltaY == -2147483648 ? 0 : MouseDeltaY));
                            }
                            break;
                        case X360Controls.MouseDown:
                            if (MouseDeltaY == 0)
                            {
                                MouseDeltaY = getMouseMapping(device, customButton.Key, cState, eState, 1);
                                MouseDeltaY = Math.Abs((MouseDeltaY == -2147483648 ? 0 : MouseDeltaY));
                            }
                            break;
                        case X360Controls.MouseLeft:
                            if (MouseDeltaX == 0)
                            {
                                MouseDeltaX = getMouseMapping(device, customButton.Key, cState, eState, 2);
                                MouseDeltaX = -Math.Abs((MouseDeltaX == -2147483648 ? 0 : MouseDeltaX));
                            }
                            break;
                        case X360Controls.MouseRight:
                            if (MouseDeltaX == 0)
                            {
                                MouseDeltaX = getMouseMapping(device, customButton.Key, cState, eState, 3);
                                MouseDeltaX = Math.Abs((MouseDeltaX == -2147483648 ? 0 : MouseDeltaX));
                            }
                            break;
                    }
                }
            }
            //if (!LX)
            if (Math.Abs(127 - MappedState.LX) <= 5)// || (Math.Abs(127 - cState.LX) > 5))
                MappedState.LX = cState.LX;
            if (Math.Abs(127 - MappedState.LY) <= 5)
                MappedState.LY = cState.LY;
            if (Math.Abs(127 - MappedState.RX) <= 5)
                MappedState.RX = cState.RX;
            if (Math.Abs(127 - MappedState.RY) <= 5)
                MappedState.RY = cState.RY;
            InputMethods.MoveCursorBy(MouseDeltaX, MouseDeltaY);
        }

        public static async void MapShiftCustom(int device, DS4State cState, DS4State MappedState, DS4StateExposed eState)
        {
            cState.CopyTo(MappedState);
            SyntheticState deviceState = Mapping.deviceState[device];
            foreach (KeyValuePair<DS4Controls, string> customKey in Global.getShiftCustomMacros(device)) //with delays
            {
                DS4KeyType keyType = Global.getShiftCustomKeyType(device, customKey.Key);
                if (getBoolMapping(customKey.Key, cState, eState))
                {
                    resetToDefaultValue(customKey.Key, MappedState);                    
                    bool LXChanged = (Math.Abs(127 - MappedState.LX) < 5);
                    bool LYChanged = (Math.Abs(127 - MappedState.LY) < 5);
                    bool RXChanged = (Math.Abs(127 - MappedState.RX) < 5);
                    bool RYChanged = (Math.Abs(127 - MappedState.RY) < 5);
                    string[] skeys;
                    int[] keys;
                    if (!string.IsNullOrEmpty(customKey.Value))
                    {
                        skeys = customKey.Value.Split('/');
                        keys = new int[skeys.Length];
                    }
                    else
                    {
                        skeys = new string[0];
                        keys = new int[0];
                    }
                    for (int i = 0; i < keys.Length; i++)
                        keys[i] = ushort.Parse(skeys[i]);
                    bool[] keydown = new bool[261];
                    if (keys.Length > 0 && keys[0] > 260 && keys[0] < 300)
                    {
                        if (keys[0] == 261 && !MappedState.Cross) MappedState.Cross = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 262 && !MappedState.Circle) MappedState.Circle = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 263 && !MappedState.Square) MappedState.Square = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 264 && !MappedState.Triangle) MappedState.Triangle = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 265 && !MappedState.DpadUp) MappedState.DpadUp = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 266 && !MappedState.DpadDown) MappedState.DpadDown = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 267 && !MappedState.DpadLeft) MappedState.DpadLeft = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 268 && !MappedState.DpadRight) MappedState.DpadRight = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 269 && !MappedState.Options) MappedState.Options = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 270 && !MappedState.Share) MappedState.Share = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 271 && !MappedState.PS) MappedState.PS = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 272 && !MappedState.L1) MappedState.L1 = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 273 && !MappedState.R1) MappedState.R1 = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 274 && MappedState.L2 == 0) MappedState.L2 = getByteMapping(device, customKey.Key, cState, eState);
                        if (keys[0] == 275 && MappedState.R2 == 0) MappedState.R2 = getByteMapping(device, customKey.Key, cState, eState);
                        if (keys[0] == 276 && !MappedState.L3) MappedState.L3 = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 277 && !MappedState.R3) MappedState.R3 = getBoolMapping(customKey.Key, cState, eState);
                        if (keys[0] == 278 && LYChanged) MappedState.LY = getXYAxisMapping(device, customKey.Key, cState, eState);
                        if (keys[0] == 279 && LYChanged) MappedState.LY = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                        if (keys[0] == 280 && LXChanged) MappedState.LX = getXYAxisMapping(device, customKey.Key, cState, eState);
                        if (keys[0] == 281 && LXChanged) MappedState.LX = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                        if (keys[0] == 282 && RYChanged) MappedState.RY = getXYAxisMapping(device, customKey.Key, cState, eState);
                        if (keys[0] == 283 && RYChanged) MappedState.RY = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                        if (keys[0] == 284 && RXChanged) MappedState.RX = getXYAxisMapping(device, customKey.Key, cState, eState);
                        if (keys[0] == 285 && RXChanged) MappedState.RX = getXYAxisMapping(device, customKey.Key, cState, eState, true);
                    }
                    if (!macrodone[DS4ControltoInt(customKey.Key)])
                    {
                        macrodone[DS4ControltoInt(customKey.Key)] = true;
                        for (int i = 0; i < keys.Length; i++)
                        {
                            if (keys[i] >= 300) //ints over 300 used to delay
                                await Task.Delay(keys[i] - 300);
                            else if (keys[i] < 261 && !keydown[keys[i]])
                            {
                                if (keys[i] == 256) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTDOWN); //anything above 255 is not a keyvalue
                                else if (keys[i] == 257) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTDOWN);
                                else if (keys[i] == 258) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEDOWN);
                                else if (keys[i] == 259) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 1);
                                else if (keys[i] == 260) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONDOWN, 2);
                                else if (keyType.HasFlag(DS4KeyType.ScanCode))
                                    InputMethods.performSCKeyPress((ushort)keys[i]);
                                else
                                    InputMethods.performKeyPress((ushort)keys[i]);
                                keydown[keys[i]] = true;
                            }
                            else if (keys[i] < 261)
                            {
                                if (keys[i] == 256) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_LEFTUP); //anything above 255 is not a keyvalue
                                else if (keys[i] == 257) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_RIGHTUP);
                                else if (keys[i] == 258) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_MIDDLEUP);
                                else if (keys[i] == 259) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 1);
                                else if (keys[i] == 260) InputMethods.MouseEvent(InputMethods.MOUSEEVENTF_XBUTTONUP, 2);
                                else if (keyType.HasFlag(DS4KeyType.ScanCode))
                                    InputMethods.performSCKeyRelease((ushort)keys[i]);
                                else
                                    InputMethods.performKeyRelease((ushort)keys[i]);
                                keydown[keys[i]] = false;
                            }
                        }
                        for (ushort i = 0; i < keydown.Length; i++)
                        {
                            if (keydown[i])
                                if (keyType.HasFlag(DS4KeyType.ScanCode))
                                    InputMethods.performSCKeyRelease(i);
                                else
                                    InputMethods.performKeyRelease(i);
                        }
                        if (keyType.HasFlag(DS4KeyType.HoldMacro))
                        {
                            await Task.Delay(50);
                            macrodone[DS4ControltoInt(customKey.Key)] = false;
                        }
                    }
                }
                else if (!getBoolMapping(customKey.Key, cState, eState))
                    macrodone[DS4ControltoInt(customKey.Key)] = false;
            }
            foreach (KeyValuePair<DS4Controls, ushort> customKey in Global.getShiftCustomKeys(device))
            {
                DS4KeyType keyType = Global.getShiftCustomKeyType(device, customKey.Key);
                if (getBoolMapping(customKey.Key, cState, eState))
                {
                    resetToDefaultValue(customKey.Key, MappedState);
                    SyntheticState.KeyPresses kp;
                    if (!deviceState.keyPresses.TryGetValue(customKey.Value, out kp))
                        deviceState.keyPresses[customKey.Value] = kp = new SyntheticState.KeyPresses();
                    if (keyType.HasFlag(DS4KeyType.ScanCode))
                        kp.current.scanCodeCount++;
                    else
                        kp.current.vkCount++;
                    if (keyType.HasFlag(DS4KeyType.Toggle))
                    {
                        if (!pressedonce[customKey.Value])
                        {
                            kp.current.toggle = !kp.current.toggle;
                            pressedonce[customKey.Value] = true;
                        }
                        kp.current.toggleCount++;
                    }
                    kp.current.repeatCount++;
                }
                else
                    pressedonce[customKey.Value] = false;
            }

            MappedState.LX = 127;
            MappedState.LY = 127;
            MappedState.RX = 127;
            MappedState.RY = 127;
            int MouseDeltaX = 0;
            int MouseDeltaY = 0;

            Dictionary<DS4Controls, X360Controls> customButtons = Global.getShiftCustomButtons(device);
            //foreach (KeyValuePair<DS4Controls, X360Controls> customButton in customButtons)
            // resetToDefaultValue(customButton.Key, MappedState); // erase default mappings for things that are remapped
            foreach (KeyValuePair<DS4Controls, X360Controls> customButton in customButtons)
            {
                if (customButton.Key == DS4Controls.Square)
                    Console.WriteLine("Hi");
                DS4KeyType keyType = Global.getShiftCustomKeyType(device, customButton.Key);
                int keyvalue = 0;
                switch (customButton.Value)
                {
                    case X360Controls.LeftMouse: keyvalue = 256; break;
                    case X360Controls.RightMouse: keyvalue = 257; break;
                    case X360Controls.MiddleMouse: keyvalue = 258; break;
                    case X360Controls.FourthMouse: keyvalue = 259; break;
                    case X360Controls.FifthMouse: keyvalue = 260; break;
                }
                if (keyType.HasFlag(DS4KeyType.Toggle))
                {
                    if (getBoolMapping(customButton.Key, cState, eState))
                    {
                        resetToDefaultValue(customButton.Key, MappedState);
                        if (!pressedonce[keyvalue])
                        {
                            deviceState.currentClicks.toggle = !deviceState.currentClicks.toggle;
                            test++;
                            pressedonce[keyvalue] = true;
                        }
                        deviceState.currentClicks.toggleCount++;
                    }
                    else// if (test = 1)// && pressedonce[keyvalue])
                    {
                        pressedonce[keyvalue] = false;
                    }
                }
                bool LXChanged = Math.Abs(127 - MappedState.LX) <= 5;
                bool LYChanged = Math.Abs(127 - MappedState.LY) <= 5;
                bool RXChanged = Math.Abs(127 - MappedState.RX) <= 5;
                bool RYChanged = Math.Abs(127 - MappedState.RY) <= 5;

                //once++;

                resetToDefaultValue(customButton.Key, MappedState); // erase default mappings for things that are remapped
                switch (customButton.Value)
                {
                    case X360Controls.A:
                        if (!MappedState.Cross)
                            MappedState.Cross = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.B:
                        if (!MappedState.Circle)
                            MappedState.Circle = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.X:
                        if (!MappedState.Square)
                            MappedState.Square = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.Y:
                        if (!MappedState.Triangle)
                            MappedState.Triangle = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.LB:
                        if (!MappedState.L1)
                            MappedState.L1 = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.LS:
                        if (!MappedState.L3)
                            MappedState.L3 = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.RB:
                        if (!MappedState.R1)
                            MappedState.R1 = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.RS:
                        if (!MappedState.R3)
                            MappedState.R3 = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.DpadUp:
                        if (!MappedState.DpadUp)
                            MappedState.DpadUp = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.DpadDown:
                        if (!MappedState.DpadDown)
                            MappedState.DpadDown = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.DpadLeft:
                        if (!MappedState.DpadLeft)
                            MappedState.DpadLeft = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.DpadRight:
                        if (!MappedState.DpadRight)
                            MappedState.DpadRight = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.Guide:
                        if (!MappedState.PS)
                            MappedState.PS = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.Back:
                        if (!MappedState.Share)
                            MappedState.Share = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.Start:
                        if (!MappedState.Options)
                            MappedState.Options = getBoolMapping(customButton.Key, cState, eState);
                        break;
                    case X360Controls.LXNeg:
                        if (LXChanged)
                            MappedState.LX = getXYAxisMapping(device, customButton.Key, cState, eState);
                        break;
                    case X360Controls.LYNeg:
                        if (LYChanged)
                            MappedState.LY = getXYAxisMapping(device, customButton.Key, cState, eState);
                        break;
                    case X360Controls.RXNeg:
                        if (RXChanged)
                            MappedState.RX = getXYAxisMapping(device, customButton.Key, cState, eState);
                        break;
                    case X360Controls.RYNeg:
                        if (RYChanged)
                            MappedState.RY = getXYAxisMapping(device, customButton.Key, cState, eState);
                        break;
                    case X360Controls.LXPos:
                        if (LXChanged)
                            MappedState.LX = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                        break;
                    case X360Controls.LYPos:
                        if (LYChanged)
                            MappedState.LY = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                        break;
                    case X360Controls.RXPos:
                        if (RXChanged)
                            MappedState.RX = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                        break;
                    case X360Controls.RYPos:
                        if (RYChanged)
                            MappedState.RY = getXYAxisMapping(device, customButton.Key, cState, eState, true);
                        break;
                    case X360Controls.LT:
                        if (MappedState.L2 == 0)
                            MappedState.L2 = getByteMapping(device, customButton.Key, cState, eState);
                        break;
                    case X360Controls.RT:
                        if (MappedState.R2 == 0)
                            MappedState.R2 = getByteMapping(device, customButton.Key, cState, eState);
                        break;
                    case X360Controls.LeftMouse:
                        if (getBoolMapping(customButton.Key, cState, eState))
                            deviceState.currentClicks.leftCount++;
                        break;
                    case X360Controls.RightMouse:
                        if (getBoolMapping(customButton.Key, cState, eState))
                            deviceState.currentClicks.rightCount++;
                        break;
                    case X360Controls.MiddleMouse:
                        if (getBoolMapping(customButton.Key, cState, eState))
                            deviceState.currentClicks.middleCount++;
                        break;
                    case X360Controls.FourthMouse:
                        if (getBoolMapping(customButton.Key, cState, eState))
                            deviceState.currentClicks.fourthCount++;
                        break;
                    case X360Controls.FifthMouse:
                        if (getBoolMapping(customButton.Key, cState, eState))
                            deviceState.currentClicks.fifthCount++;
                        break;
                    case X360Controls.WUP:
                        if (getBoolMapping(customButton.Key, cState, eState))
                            deviceState.currentClicks.wUpCount++;
                        break;
                    case X360Controls.WDOWN:
                        if (getBoolMapping(customButton.Key, cState, eState))
                            deviceState.currentClicks.wDownCount++;
                        break;
                    case X360Controls.MouseUp:
                        if (MouseDeltaY == 0)
                        {
                            MouseDeltaY = getMouseMapping(device, customButton.Key, cState, eState, 0);
                            MouseDeltaY = -Math.Abs((MouseDeltaY == -2147483648 ? 0 : MouseDeltaY));
                        }
                        break;
                    case X360Controls.MouseDown:
                        if (MouseDeltaY == 0)
                        {
                            MouseDeltaY = getMouseMapping(device, customButton.Key, cState, eState, 1);
                            MouseDeltaY = Math.Abs((MouseDeltaY == -2147483648 ? 0 : MouseDeltaY));
                        }
                        break;
                    case X360Controls.MouseLeft:
                        if (MouseDeltaX == 0)
                        {
                            MouseDeltaX = getMouseMapping(device, customButton.Key, cState, eState, 2);
                            MouseDeltaX = -Math.Abs((MouseDeltaX == -2147483648 ? 0 : MouseDeltaX));
                        }
                        break;
                    case X360Controls.MouseRight:
                        if (MouseDeltaX == 0)
                        {
                            MouseDeltaX = getMouseMapping(device, customButton.Key, cState, eState, 3);
                            MouseDeltaX = Math.Abs((MouseDeltaX == -2147483648 ? 0 : MouseDeltaX));
                        }
                        break;
                }
            }
            //if (!LX)
            if (Math.Abs(127 - MappedState.LX) <= 5)// || (Math.Abs(127 - cState.LX) > 5))
                MappedState.LX = cState.LX;
            if (Math.Abs(127 - MappedState.LY) <= 5)
                MappedState.LY = cState.LY;
            if (Math.Abs(127 - MappedState.RX) <= 5)
                MappedState.RX = cState.RX;
            if (Math.Abs(127 - MappedState.RY) <= 5)
                MappedState.RY = cState.RY;
            InputMethods.MoveCursorBy(MouseDeltaX, MouseDeltaY);
        }

        public static DateTime[] mousenow = { DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow };
        public static double mvalue = 0;
        public static int[] mouseaccel = new int[34];
        public static bool[] mousedoublecheck = new bool[34];
        private static int getMouseMapping(int device, DS4Controls control, DS4State cState, DS4StateExposed eState, int mnum)
        {
            int controlnum = DS4ControltoInt(control);
            double SXD = Global.getSXDeadzone(device);
            double SZD = Global.getSZDeadzone(device);
            int deadzone = 10;
            double value = 0;
            int speed = Global.getButtonMouseSensitivity(device)+15;
            double root = 1.002;
            double divide = 10000d;
            DateTime now = mousenow[mnum];
            bool leftsitcklive = ((cState.LX < 127 - deadzone || 127 + deadzone < cState.LX) || (cState.LY < 127 - deadzone || 127 + deadzone < cState.LY));
            bool rightsitcklive =  ((cState.RX < 127 - deadzone || 127 + deadzone < cState.RX) || (cState.RY < 127 - deadzone || 127 + deadzone < cState.RY));
            switch (control)
            {
                case DS4Controls.LXNeg:
                    if (leftsitcklive)
                        value = -(cState.LX - 127) / 2550d * speed;
                    break;
                case DS4Controls.LXPos:
                    if (leftsitcklive)
                        value = (cState.LX - 127) / 2550d * speed;
                    break;
                case DS4Controls.RXNeg:
                    if (rightsitcklive)
                        value = -(cState.RX - 127) / 2550d * speed;
                    break;
                case DS4Controls.RXPos:
                    if (rightsitcklive)
                        value = (cState.RX - 127) / 2550d * speed;
                    break;
                case DS4Controls.LYNeg:
                    if (leftsitcklive)
                        value = -(cState.LY - 127) / 2550d * speed;
                    break;
                case DS4Controls.LYPos:
                    if (leftsitcklive)
                        value = (cState.LY - 127) / 2550d * speed;
                    break;
                case DS4Controls.RYNeg:
                    if (rightsitcklive)
                        value = -(cState.RY - 127) / 2550d * speed;
                    break;
                case DS4Controls.RYPos:
                    if (rightsitcklive)
                        value = (cState.RY - 127) / 2550d * speed;
                    break;
                /*case DS4Controls.LXNeg:
                    if (cState.LX < 127 - deadzone)
                        value = Math.Pow(root + speed / divide, -(cState.LX - 127)) - 1;
                    break;
                case DS4Controls.LXPos:
                    if (cState.LX > 127 + deadzone)
                        value = Math.Pow(root + speed / divide, (cState.LX - 127)) -1;
                    break;
                case DS4Controls.RXNeg:
                    if (cState.RX < 127 - deadzone)
                        value = Math.Pow(root + speed / divide, -(cState.RX - 127)) - 1;                        
                    break;
                case DS4Controls.RXPos:
                    if (cState.RX > 127 + deadzone)
                        value = Math.Pow(root + speed / divide, (cState.RX - 127)) - 1;
                    break;
                case DS4Controls.LYNeg:
                    if (cState.LY < 127 - deadzone)
                        value = Math.Pow(root + speed / divide, -(cState.LY - 127)) - 1;
                    break;
                case DS4Controls.LYPos:
                    if (cState.LY > 127 + deadzone)
                        value = Math.Pow(root + speed / divide,(cState.LY - 127))  - 1;
                    break;
                case DS4Controls.RYNeg:
                    if (cState.RY < 127 - deadzone)
                        value = Math.Pow(root + speed / divide,-(cState.RY - 127)) - 1;
                    break;
                case DS4Controls.RYPos:
                    if (cState.RY > 127 + deadzone)
                        value = Math.Pow(root + speed / divide, (cState.RY - 127))  - 1;
                    break;*/
                case DS4Controls.Share: value = (cState.Share ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.Options: value = (cState.Options ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.L1: value = (cState.L1 ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.R1: value = (cState.R1 ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.L3: value = (cState.L3 ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.R3: value = (cState.R3 ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.DpadUp: value = (cState.DpadUp ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.DpadDown: value = (cState.DpadDown ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.DpadLeft: value = (cState.DpadLeft ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.DpadRight: value = (cState.DpadRight ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.PS: value = (cState.PS ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.Cross: value = (cState.Cross ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.Square: value = (cState.Square ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.Triangle: value = (cState.Triangle ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.Circle: value = (cState.Circle ? Math.Pow(root + speed / divide, 100) - 1 : 0); break;
                case DS4Controls.L2: value = Math.Pow(root + speed / divide, cState.L2 / 2d) - 1; break;
                case DS4Controls.R2: value = Math.Pow(root + speed / divide, cState.R2 / 2d) - 1; break;
            }
            if (eState != null)
                switch (control)
                {
                    case DS4Controls.GyroXPos: return (byte)(eState.GyroX > SXD * 7500 ? 
                        Math.Pow(root + speed / divide, eState.GyroX / 62) : 0);
                    case DS4Controls.GyroXNeg: return (byte)(eState.GyroX < -SXD * 7500 ? 
                        Math.Pow(root + speed / divide, -eState.GyroX / 48) : 0);
                    case DS4Controls.GyroZPos: return (byte)(eState.GyroZ > SZD * 7500 ? 
                        Math.Pow(root + speed / divide, eState.GyroZ / 62) : 0);
                    case DS4Controls.GyroZNeg: return (byte)(eState.GyroZ < -SZD * 7500 ?
                        Math.Pow(root + speed / divide, -eState.GyroZ / 62) : 0);
                }
            bool LXChanged = (Math.Abs(127 - cState.LX) < deadzone);
            bool LYChanged = (Math.Abs(127 - cState.LY) < deadzone);
            bool RXChanged = (Math.Abs(127 - cState.RX) < deadzone);
            bool RYChanged = (Math.Abs(127 - cState.RY) < deadzone);
            if (LXChanged || LYChanged || RXChanged || RYChanged)
                now = DateTime.UtcNow;
            if (Global.getMouseAccel(device))
            {
                if (value > 0)
                    mouseaccel[controlnum]++;
                else if (!mousedoublecheck[controlnum])
                    mouseaccel[controlnum] = 0;
                mousedoublecheck[controlnum] = value != 0;
                if (mouseaccel[controlnum] > 1000)
                    value *= (double)Math.Min(2000, (mouseaccel[controlnum])) / 1000d;
            }
            if (value <= 1)
            {
                if (now >= mousenow[mnum] + TimeSpan.FromMilliseconds((1 - value) * 500))
                {
                    mousenow[mnum] = now;
                    return 1;
                }
                else
                    return 0;
            }
            else
                return (int)value;
        }

        public static bool compare(byte b1, byte b2)
        {
            if (Math.Abs(b1 - b2) > 10)
            {
                return false;
            }
            return true;
        }

        static bool[] touchArea = { true, true, true, true };

        public static byte getByteMapping(int device, DS4Controls control, DS4State cState, DS4StateExposed eState)
        {
            double SXD = Global.getSXDeadzone(device);
            double SZD = Global.getSZDeadzone(device);
            if (!cState.TouchButton)
                for (int i = 0; i < 4; i++)
                    touchArea[i] = false;
            if (!(touchArea[0] || touchArea[1] || touchArea[2] || touchArea[3]))
            {
                if (cState.Touch2)
                    touchArea[0] = true;
                if (cState.TouchLeft && !cState.Touch2 && cState.Touch1)
                    touchArea[1] = true;
                if (cState.TouchRight && !cState.Touch2 && cState.Touch1)
                    touchArea[2] = true;
                if (!cState.Touch1)
                    touchArea[3] = true;
            }
            switch (control)
            {
                case DS4Controls.Share: return (byte)(cState.Share ? 255 : 0);
                case DS4Controls.Options: return (byte)(cState.Options ? 255 : 0);
                case DS4Controls.L1: return (byte)(cState.L1 ? 255 : 0);
                case DS4Controls.R1: return (byte)(cState.R1 ? 255 : 0);
                case DS4Controls.L3: return (byte)(cState.L3 ? 255 : 0);
                case DS4Controls.R3: return (byte)(cState.R3 ? 255 : 0);
                case DS4Controls.DpadUp: return (byte)(cState.DpadUp ? 255 : 0);
                case DS4Controls.DpadDown: return (byte)(cState.DpadDown ? 255 : 0);
                case DS4Controls.DpadLeft: return (byte)(cState.DpadLeft ? 255 : 0);
                case DS4Controls.DpadRight: return (byte)(cState.DpadRight ? 255 : 0);
                case DS4Controls.PS: return (byte)(cState.PS ? 255 : 0);
                case DS4Controls.Cross: return (byte)(cState.Cross ? 255 : 0);
                case DS4Controls.Square: return (byte)(cState.Square ? 255 : 0);
                case DS4Controls.Triangle: return (byte)(cState.Triangle ? 255 : 0);
                case DS4Controls.Circle: return (byte)(cState.Circle ? 255 : 0);
                case DS4Controls.LXNeg: return (byte)cState.LX;
                case DS4Controls.LYNeg: return (byte)cState.LY;
                case DS4Controls.RXNeg: return (byte)cState.RX;
                case DS4Controls.RYNeg: return (byte)cState.RY;
                case DS4Controls.LXPos: return (byte)(cState.LX - 127 < 0 ? 0 : (cState.LX - 127));
                case DS4Controls.LYPos: return (byte)(cState.LY - 123 < 0 ? 0 : (cState.LY - 123));
                case DS4Controls.RXPos: return (byte)(cState.RX - 129 < 0 ? 0 : (cState.RX - 125));
                case DS4Controls.RYPos: return (byte)(cState.RY - 125 < 0 ? 0 : (cState.RY - 127));
                case DS4Controls.L2: return (byte)cState.L2;
                case DS4Controls.R2: return (byte)cState.R2;
            }
            if (eState != null)
                switch (control)
                {
                    case DS4Controls.GyroXPos: return (byte)(eState.GyroX > SXD * 7500 ? Math.Min(255, eState.GyroX / 31) : 0);
                    case DS4Controls.GyroXNeg: return (byte)(eState.GyroX < -SXD * 7500 ? Math.Min(255, -eState.GyroX / 31) : 0);
                    case DS4Controls.GyroZPos: return (byte)(eState.GyroZ > SZD * 7500 ? Math.Min(255, eState.GyroZ / 31) : 0);
                    case DS4Controls.GyroZNeg: return (byte)(eState.GyroZ < -SZD * 7500 ? Math.Min(255, -eState.GyroZ / 31) : 0);
                }
            if (cState.TouchButton)
            {
                if (control == DS4Controls.TouchMulti)
                    if (!(touchArea[1] || touchArea[2] || touchArea[3]))
                        return (byte)(touchArea[0] ? 255 : 0);
                if (control == DS4Controls.TouchLeft)
                    if (!(touchArea[0] || touchArea[2] || touchArea[3]))
                        return (byte)(touchArea[1] ? 255 : 0);
                if (control == DS4Controls.TouchRight)
                    if (!(touchArea[0] || touchArea[1] || touchArea[3]))
                        return (byte)(touchArea[2] ? 255 : 0);
                if (control == DS4Controls.TouchUpper)
                    if (!(touchArea[0] || touchArea[1] || touchArea[2]))
                        return (byte)(touchArea[3] ? 255 : 0);
            }
            return 0;
        }
        public static bool getBoolMapping(DS4Controls control, DS4State cState, DS4StateExposed eState)
        {
            if (!cState.TouchButton)
                for (int i = 0; i < 4; i++)
                    touchArea[i] = false;
            if (!(touchArea[0] || touchArea[1] || touchArea[2] || touchArea[3]))
            {
                if (cState.Touch2)
                    touchArea[0] = true;
                if (cState.TouchLeft && !cState.Touch2 && cState.Touch1)
                    touchArea[1] = true;
                if (cState.TouchRight && !cState.Touch2 && cState.Touch1)
                    touchArea[2] = true;
                if (!cState.Touch1)
                    touchArea[3] = true;
            }
            switch (control)
            {
                case DS4Controls.Share: return cState.Share;
                case DS4Controls.Options: return cState.Options;
                case DS4Controls.L1: return cState.L1;
                case DS4Controls.R1: return cState.R1;
                case DS4Controls.L3: return cState.L3;
                case DS4Controls.R3: return cState.R3;
                case DS4Controls.DpadUp: return cState.DpadUp;
                case DS4Controls.DpadDown: return cState.DpadDown;
                case DS4Controls.DpadLeft: return cState.DpadLeft;
                case DS4Controls.DpadRight: return cState.DpadRight;
                case DS4Controls.PS: return cState.PS;
                case DS4Controls.Cross: return cState.Cross;
                case DS4Controls.Square: return cState.Square;
                case DS4Controls.Triangle: return cState.Triangle;
                case DS4Controls.Circle: return cState.Circle;
                case DS4Controls.LXNeg: return cState.LX < 127 - 55;
                case DS4Controls.LYNeg: return cState.LY < 127 - 55;
                case DS4Controls.RXNeg: return cState.RX < 127 - 55;
                case DS4Controls.RYNeg: return cState.RY < 127 - 55;
                case DS4Controls.LXPos: return cState.LX > 127 + 55;
                case DS4Controls.LYPos: return cState.LY > 127 + 55;
                case DS4Controls.RXPos: return cState.RX > 127 + 55;
                case DS4Controls.RYPos: return cState.RY > 127 + 55;
                case DS4Controls.L2: return cState.L2 > 100;
                case DS4Controls.R2: return cState.R2 > 100;
            }
            if (eState != null)
                switch (control)
                {
                    case DS4Controls.GyroXPos: return eState.GyroX > 5000;
                    case DS4Controls.GyroXNeg: return eState.GyroX < -5000;
                    case DS4Controls.GyroZPos: return eState.GyroZ > 5000;
                    case DS4Controls.GyroZNeg: return eState.GyroZ < -5000;
                }
            if (cState.TouchButton)
            {
                if (control == DS4Controls.TouchMulti)
                    if (!(touchArea[1] || touchArea[2] || touchArea[3]))
                        return touchArea[0];
                if (control == DS4Controls.TouchLeft)
                    if (!(touchArea[0] || touchArea[2] || touchArea[3]))
                        return touchArea[1];
                if (control == DS4Controls.TouchRight)
                    if (!(touchArea[0] || touchArea[1] || touchArea[3]))
                        return touchArea[2];
                if (control == DS4Controls.TouchUpper)
                    if (!(touchArea[0] || touchArea[1] || touchArea[2]))
                        return touchArea[3];
            }
            return false;
        }

        public static byte getXYAxisMapping(int device, DS4Controls control, DS4State cState, DS4StateExposed eState, bool alt = false)
        {
            byte trueVal = 0;
            byte falseVal = 127;
            double SXD = Global.getSXDeadzone(device);
            double SZD = Global.getSZDeadzone(device);
            if (alt)
                trueVal = 255;
            if (!cState.TouchButton)
                for (int i = 0; i < 4; i++)
                    touchArea[i] = false;
            if (!(touchArea[0] || touchArea[1] || touchArea[2] || touchArea[3]))
            {
                if (cState.Touch2)
                    touchArea[0] = true;
                if (cState.TouchLeft && !cState.Touch2 && cState.Touch1)
                    touchArea[1] = true;
                if (cState.TouchRight && !cState.Touch2 && cState.Touch1)
                    touchArea[2] = true;
                if (!cState.Touch1)
                    touchArea[3] = true;
            }
            switch (control)
            {
                case DS4Controls.Share: return (byte)(cState.Share ? trueVal : falseVal);
                case DS4Controls.Options: return (byte)(cState.Options ? trueVal : falseVal);
                case DS4Controls.L1: return (byte)(cState.L1 ? trueVal : falseVal);
                case DS4Controls.R1: return (byte)(cState.R1 ? trueVal : falseVal);
                case DS4Controls.L3: return (byte)(cState.L3 ? trueVal : falseVal);
                case DS4Controls.R3: return (byte)(cState.R3 ? trueVal : falseVal);
                case DS4Controls.DpadUp: return (byte)(cState.DpadUp ? trueVal : falseVal);
                case DS4Controls.DpadDown: return (byte)(cState.DpadDown ? trueVal : falseVal);
                case DS4Controls.DpadLeft: return (byte)(cState.DpadLeft ? trueVal : falseVal);
                case DS4Controls.DpadRight: return (byte)(cState.DpadRight ? trueVal : falseVal);
                case DS4Controls.PS: return (byte)(cState.PS ? trueVal : falseVal);
                case DS4Controls.Cross: return (byte)(cState.Cross ? trueVal : falseVal);
                case DS4Controls.Square: return (byte)(cState.Square ? trueVal : falseVal);
                case DS4Controls.Triangle: return (byte)(cState.Triangle ? trueVal : falseVal);
                case DS4Controls.Circle: return (byte)(cState.Circle ? trueVal : falseVal);
                case DS4Controls.L2: if (alt) return (byte)(127 + cState.L2 / 2); else return (byte)(127 - cState.L2 / 2);
                case DS4Controls.R2: if (alt) return (byte)(127 + cState.R2 / 2); else return (byte)(127 - cState.R2 / 2);
            }
            if (eState != null)
            {
                switch (control)
                {
                    case DS4Controls.GyroXPos: if (eState.GyroX > SXD * 7500)
                            if (alt) return (byte)Math.Min(255, 127 + eState.GyroX / 62); else return (byte)Math.Max(0, 127 - eState.GyroX / 62);
                        else return falseVal;
                    case DS4Controls.GyroXNeg: if (eState.GyroX < -SXD * 7500)
                            if (alt) return (byte)Math.Min(255, 127 + -eState.GyroX / 62); else return (byte)Math.Max(0, 127 - -eState.GyroX / 62);
                        else return falseVal;
                    case DS4Controls.GyroZPos: if (eState.GyroZ > SZD * 7500)
                            if (alt) return (byte)Math.Min(255, 127 + eState.GyroZ / 62); else return (byte)Math.Max(0, 127 - eState.GyroZ / 62);
                        else return falseVal;
                    case DS4Controls.GyroZNeg: if (eState.GyroZ < -SZD * 7500)
                            if (alt) return (byte)Math.Min(255, 127 + -eState.GyroZ / 62); else return (byte)Math.Max(0, 127 - -eState.GyroZ / 62);
                        else return falseVal;
                }
            }
            if (!alt)
            {
                switch (control)
                {
                    case DS4Controls.LXNeg: return cState.LX;
                    case DS4Controls.LYNeg: return cState.LY;
                    case DS4Controls.RXNeg: return cState.RX;
                    case DS4Controls.RYNeg: return cState.RY;
                    case DS4Controls.LXPos: return (byte)(255 - cState.LX);
                    case DS4Controls.LYPos: return (byte)(255 - cState.LY);
                    case DS4Controls.RXPos: return (byte)(255 - cState.RX);
                    case DS4Controls.RYPos: return (byte)(255 - cState.RY);
                }
            }
            else
            {
                switch (control)
                {
                    case DS4Controls.LXNeg: return (byte)(255 - cState.LX);
                    case DS4Controls.LYNeg: return (byte)(255 - cState.LY);
                    case DS4Controls.RXNeg: return (byte)(255 - cState.RX);
                    case DS4Controls.RYNeg: return (byte)(255 - cState.RY);
                    case DS4Controls.LXPos: return cState.LX;
                    case DS4Controls.LYPos: return cState.LY;
                    case DS4Controls.RXPos: return cState.RX;
                    case DS4Controls.RYPos: return cState.RY;
                }
            }
            if (cState.TouchButton)
            {
                if (control == DS4Controls.TouchMulti)
                    if (!(touchArea[1] || touchArea[2] || touchArea[3]))
                        return (byte)(touchArea[0] ? trueVal : falseVal);
                if (control == DS4Controls.TouchLeft)
                    if (!(touchArea[0] || touchArea[2] || touchArea[3]))
                        return (byte)(touchArea[1] ? trueVal : falseVal);
                if (control == DS4Controls.TouchRight)
                    if (!(touchArea[0] || touchArea[1] || touchArea[3]))
                        return (byte)(touchArea[2] ? trueVal : falseVal);
                if (control == DS4Controls.TouchUpper)
                    if (!(touchArea[0] || touchArea[1] || touchArea[2]))
                        return (byte)(touchArea[3] ? trueVal : falseVal);
            }
            return 0;
        }

        //Returns false for any bool, 
        //if control is one of the xy axis returns 127
        //if its a trigger returns 0
        public static void resetToDefaultValue(DS4Controls control, DS4State cState)
        {
            switch (control)
            {
                case DS4Controls.Share: cState.Share = false; break;
                case DS4Controls.Options: cState.Options = false; break;
                case DS4Controls.L1: cState.L1 = false; break;
                case DS4Controls.R1: cState.R1 = false; break;
                case DS4Controls.L3: cState.L3 = false; break;
                case DS4Controls.R3: cState.R3 = false; break;
                case DS4Controls.DpadUp: cState.DpadUp = false; break;
                case DS4Controls.DpadDown: cState.DpadDown = false; break;
                case DS4Controls.DpadLeft: cState.DpadLeft = false; break;
                case DS4Controls.DpadRight: cState.DpadRight = false; break;
                case DS4Controls.PS: cState.PS = false; break;
                case DS4Controls.Cross: cState.Cross = false; break;
                case DS4Controls.Square: cState.Square = false; break;
                case DS4Controls.Triangle: cState.Triangle = false; break;
                case DS4Controls.Circle: cState.Circle = false; break;
                case DS4Controls.LXNeg: cState.LX = 127; break;
                case DS4Controls.LYNeg: cState.LY = 127; break;
                case DS4Controls.RXNeg: cState.RX = 127; break;
                case DS4Controls.RYNeg: cState.RY = 127; break;
                case DS4Controls.LXPos: cState.LX = 127; break;
                case DS4Controls.LYPos: cState.LY = 127; break;
                case DS4Controls.RXPos: cState.RX = 127; break;
                case DS4Controls.RYPos: cState.RY = 127; break;
                case DS4Controls.L2: cState.L2 = 0; break;
                case DS4Controls.R2: cState.R2 = 0; break;
                //case DS4Controls.TouchButton: cState.TouchLeft = false; break;
                //case DS4Controls.TouchMulti: cState.Touch2 = false; break;
                //case DS4Controls.TouchRight: cState.TouchRight = false; break;
                //case DS4Controls.TouchUpper: cState.TouchButton = false; break;
            }
        }


        // Arthritis mode, compensate for cumulative pressure F=kx on the triggers by allowing the user to remap the entire trigger to just the initial portion.
        private static byte[,]  leftTriggerMap = new byte[4,256], rightTriggerMap = new byte[4,256];
        private static double[] leftTriggerMiddle = new double[4], // linear trigger remapping, 0.5 is in the middle of 0 and 255 from the native controller.
            oldLeftTriggerMiddle = new double[4],
            rightTriggerMiddle = new double[4], oldRightTriggerMiddle = new double[4];
        private static void initializeTriggerMap(byte[,] map, double triggerMiddle, int deviceNum)
        {
            double midpoint = 256.0 * triggerMiddle;
            for (uint x = 0; x <= 255; x++)
            {
                double mapped;
                if (x < midpoint) // i.e. with standard 0.5, 0..127->0..127, etc.; with 0.25, 0..63->0..127 and 64..255->128..255\
                    mapped = (x * 0.5 / triggerMiddle);
                else
                    mapped = 128.0 + 128.0 * (x - midpoint) / (256.0 - midpoint);
                map[deviceNum, x] = (byte)mapped;
            }

        }
        public static byte mapLeftTrigger(byte orig, int deviceNum)
        {
            leftTriggerMiddle[deviceNum] = Global.getLeftTriggerMiddle(deviceNum);
            if (leftTriggerMiddle[deviceNum] != oldLeftTriggerMiddle[deviceNum])
            {
                oldLeftTriggerMiddle[deviceNum] = leftTriggerMiddle[deviceNum];
                initializeTriggerMap(leftTriggerMap, leftTriggerMiddle[deviceNum],deviceNum);
            }
            return leftTriggerMap[deviceNum, orig];
        }
        public static byte mapRightTrigger(byte orig, int deviceNum)
        {
            rightTriggerMiddle[deviceNum] = Global.getRightTriggerMiddle(deviceNum);
            if (rightTriggerMiddle[deviceNum] != oldRightTriggerMiddle[deviceNum])
            {
                oldRightTriggerMiddle[deviceNum] = rightTriggerMiddle[deviceNum];
                initializeTriggerMap(rightTriggerMap, rightTriggerMiddle[deviceNum], deviceNum);
            }
            return rightTriggerMap[deviceNum,orig];
        }
    }
}
