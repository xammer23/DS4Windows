﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DS4Library;
namespace DS4Control
{
    public class DS4LightBar
    {
        private readonly static byte[/* Light On duration */, /* Light Off duration */] BatteryIndicatorDurations =
        {
            { 0, 0 }, // 0 is for "charging" OR anything sufficiently-"charged"
            { 28, 252 },
            { 56, 224 },
            { 84, 196 },
            { 112, 168 },
            { 140, 140 },
            { 168, 112 },
            { 196, 84 },
            { 224, 56}, // on 80% of the time at 80, etc.
            { 252, 28 } // on 90% of the time at 90
        };
        static double[] counters = new double[4] { 0, 0, 0, 0 };
        public static double[] fadetimer = new double[4] { 0, 0, 0, 0 };
        static bool[] fadedirection = new bool[4] { false, false, false, false };
        static DateTime oldnow = DateTime.UtcNow;
        public static void updateLightBar(DS4Device device, int deviceNum, DS4State cState, DS4StateExposed eState)
        {
            DS4Color color;
            if (!defualtLight)
            {
                if (Global.getShiftColorOn(deviceNum) && Global.getShiftModifier(deviceNum) > 0 && shiftMod(device, deviceNum, cState, eState))
                {
                    color = Global.loadShiftColor(deviceNum);
                }
                else
                {
                    if (Global.getRainbow(deviceNum) > 0)
                    {// Display rainbow
                        DateTime now = DateTime.UtcNow;
                        if (now >= oldnow + TimeSpan.FromMilliseconds(10)) //update by the millisecond that way it's a smooth transtion
                        {
                            oldnow = now;
                            if (device.Charging)
                                counters[deviceNum] -= 1.5 * 3 / Global.getRainbow(deviceNum);
                            else
                                counters[deviceNum] += 1.5 * 3 / Global.getRainbow(deviceNum);
                        }
                        if (counters[deviceNum] < 0)
                            counters[deviceNum] = 180000;
                        if (Global.getLedAsBatteryIndicator(deviceNum))
                            color = HuetoRGB((float)counters[deviceNum] % 360, (byte)(2.55 * device.Battery));
                        else
                            color = HuetoRGB((float)counters[deviceNum] % 360, 255);

                    }
                    else if (Global.getLedAsBatteryIndicator(deviceNum))
                    {
                        //if (device.Charging == false || device.Battery >= 100) // when charged, don't show the charging animation
                        {
                            DS4Color fullColor = new DS4Color
                            {
                                red = Global.loadColor(deviceNum).red,
                                green = Global.loadColor(deviceNum).green,
                                blue = Global.loadColor(deviceNum).blue
                            };

                            color = Global.loadLowColor(deviceNum);
                            DS4Color lowColor = new DS4Color
                            {
                                red = color.red,
                                green = color.green,
                                blue = color.blue
                            };

                            color = Global.getTransitionedColor(lowColor, fullColor, (uint)device.Battery);
                        }
                    }
                    else
                    {
                        color = Global.loadColor(deviceNum);
                    }


                    if (device.Battery <= Global.getFlashAt(deviceNum) && !defualtLight && !device.Charging)
                    {
                        if (!(Global.loadFlashColor(deviceNum).red == 0 &&
                            Global.loadFlashColor(deviceNum).green == 0 &&
                            Global.loadFlashColor(deviceNum).blue == 0))
                            color = Global.loadFlashColor(deviceNum);
                    }

                    if (Global.getIdleDisconnectTimeout(deviceNum) > 0 && Global.getLedAsBatteryIndicator(deviceNum) && (!device.Charging || device.Battery >= 100))
                    {//Fade lightbar by idle time
                        TimeSpan timeratio = new TimeSpan(DateTime.UtcNow.Ticks - device.lastActive.Ticks);
                        double botratio = timeratio.TotalMilliseconds;
                        double topratio = TimeSpan.FromSeconds(Global.getIdleDisconnectTimeout(deviceNum)).TotalMilliseconds;
                        double ratio = ((botratio / topratio) * 100);
                        if (ratio >= 50 && ratio <= 100)
                            color = Global.getTransitionedColor(color, new DS4Color { red = 0, green = 0, blue = 0 }, (uint)((ratio - 50) * 2));
                        else if (ratio >= 100)
                            color = Global.getTransitionedColor(color, new DS4Color { red = 0, green = 0, blue = 0 }, 100);
                    }
                    if (device.Charging && device.Battery < 100)
                        switch (Global.getChargingType(deviceNum))
                        {
                            case 1:
                                if (fadetimer[deviceNum] <= 0)
                                    fadedirection[deviceNum] = true;
                                else if (fadetimer[deviceNum] >= 105)
                                    fadedirection[deviceNum] = false;
                                if (fadedirection[deviceNum])
                                    fadetimer[deviceNum] += .1;
                                else
                                    fadetimer[deviceNum] -= .1;
                                color = Global.getTransitionedColor(color, new DS4Color { red = 0, green = 0, blue = 0 }, fadetimer[deviceNum]);
                                break;
                            case 2:
                                counters[deviceNum] += .167;
                                color = HuetoRGB((float)counters[deviceNum] % 360, 255);
                                break;
                            case 3:
                                color = Global.loadChargingColor(deviceNum);
                                break;
                            default:
                                break;
                        }
                }
            }
            else if (shuttingdown)
                color = new DS4Color { red = 0, green = 0, blue = 0 };
            else
            {
                if (device.ConnectionType == ConnectionType.BT)
                    color = new DS4Color { red = 32, green = 64, blue = 64 };
                else
                    color = new DS4Color { red = 0, green = 0, blue = 0 };
            }
            bool distanceprofile = (Global.getAProfile(deviceNum).ToLower().Contains("distance") || Global.tempprofilename[deviceNum].ToLower().Contains("distance"));
            if (distanceprofile && !defualtLight)
            { //Thing I did for Distance
                float rumble = device.LeftHeavySlowRumble / 2.55f;
                byte max= Math.Max(color.red, Math.Max(color.green, color.blue));
                if (device.LeftHeavySlowRumble > 100)
                    color = getTransitionedColor(new DS4Color { green = max, red = max }, rumble, new DS4Color { red = 255 });
                else
                    color = getTransitionedColor(color, device.LeftHeavySlowRumble, getTransitionedColor(new DS4Color { green = max, red = max }, 39.6078f, new DS4Color { red = 255 }));
            }
            DS4HapticState haptics = new DS4HapticState
            {
                LightBarColor = color
            };
            if (haptics.IsLightBarSet())
            {
                if (device.Battery <= Global.getFlashAt(deviceNum) && !defualtLight && !device.Charging)
                {
                    int level = device.Battery / 10;
                    //if (level >= 10)
                    //level = 0; // all values of ~0% or >~100% are rendered the same
                    haptics.LightBarFlashDurationOn = BatteryIndicatorDurations[level, 0];
                    haptics.LightBarFlashDurationOff = BatteryIndicatorDurations[level, 1];
                }
                else if (distanceprofile && device.LeftHeavySlowRumble > 155) //also part of Distance
                {
                    haptics.LightBarFlashDurationOff = haptics.LightBarFlashDurationOn = (byte)((-device.LeftHeavySlowRumble + 265));
                    haptics.LightBarExplicitlyOff = true;
                }
                else
                {
                    //haptics.LightBarFlashDurationOff = haptics.LightBarFlashDurationOn = 1;
                    haptics.LightBarFlashDurationOff = haptics.LightBarFlashDurationOn = 0;
                    haptics.LightBarExplicitlyOff = true;
                }
            }
            else
            {
                haptics.LightBarExplicitlyOff = true;
            }
            if (device.LightBarOnDuration != haptics.LightBarFlashDurationOn && device.LightBarOnDuration != 1 && haptics.LightBarFlashDurationOn == 0)
                haptics.LightBarFlashDurationOff = haptics.LightBarFlashDurationOn = 1;
            if (device.LightBarOnDuration == 1) //helps better reset the color
                System.Threading.Thread.Sleep(5);
            device.pushHapticState(haptics);
        }
        public static bool defualtLight = false, shuttingdown = false;

        public static bool shiftMod(DS4Device device, int deviceNum, DS4State cState, DS4StateExposed eState)
        {
            bool shift;
            switch (Global.getShiftModifier(deviceNum))
            {
                case 1: shift = Mapping.getBoolMapping(DS4Controls.Cross, cState, eState); break;
                case 2: shift = Mapping.getBoolMapping(DS4Controls.Circle, cState, eState); break;
                case 3: shift = Mapping.getBoolMapping(DS4Controls.Square, cState, eState); break;
                case 4: shift = Mapping.getBoolMapping(DS4Controls.Triangle, cState, eState); break;
                case 5: shift = Mapping.getBoolMapping(DS4Controls.Options, cState, eState); break;
                case 6: shift = Mapping.getBoolMapping(DS4Controls.Share, cState, eState); break;
                case 7: shift = Mapping.getBoolMapping(DS4Controls.DpadUp, cState, eState); break;
                case 8: shift = Mapping.getBoolMapping(DS4Controls.DpadDown, cState, eState); break;
                case 9: shift = Mapping.getBoolMapping(DS4Controls.DpadLeft, cState, eState); break;
                case 10: shift = Mapping.getBoolMapping(DS4Controls.DpadRight, cState, eState); break;
                case 11: shift = Mapping.getBoolMapping(DS4Controls.PS, cState, eState); break;
                case 12: shift = Mapping.getBoolMapping(DS4Controls.L1, cState, eState); break;
                case 13: shift = Mapping.getBoolMapping(DS4Controls.R1, cState, eState); break;
                case 14: shift = Mapping.getBoolMapping(DS4Controls.L2, cState, eState); break;
                case 15: shift = Mapping.getBoolMapping(DS4Controls.R2, cState, eState); break;
                case 16: shift = Mapping.getBoolMapping(DS4Controls.L3, cState, eState); break;
                case 17: shift = Mapping.getBoolMapping(DS4Controls.R3, cState, eState); break;
                case 18: shift = Mapping.getBoolMapping(DS4Controls.TouchLeft, cState, eState); break;
                case 19: shift = Mapping.getBoolMapping(DS4Controls.TouchUpper, cState, eState); break;
                case 20: shift = Mapping.getBoolMapping(DS4Controls.TouchMulti, cState, eState); break;
                case 21: shift = Mapping.getBoolMapping(DS4Controls.TouchRight, cState, eState); break;
                case 22: shift = Mapping.getBoolMapping(DS4Controls.GyroZNeg, cState, eState); break;
                case 23: shift = Mapping.getBoolMapping(DS4Controls.GyroZPos, cState, eState); break;
                case 24: shift = Mapping.getBoolMapping(DS4Controls.GyroXPos, cState, eState); break;
                case 25: shift = Mapping.getBoolMapping(DS4Controls.GyroXNeg, cState, eState); break;
                case 26: shift = device.getCurrentState().Touch1; break;
                default: shift = false; break;
            }
            return shift;
        }
        public static DS4Color HuetoRGB(float hue, byte sat)
        {
            byte C = sat;
            int X = (int)((C * (float)(1 - Math.Abs((hue / 60) % 2 - 1))));
            if (0 <= hue && hue < 60)
                return new DS4Color { red = C, green = (byte)X, blue = 0 };
            else if (60 <= hue && hue < 120)
                return new DS4Color { red = (byte)X, green = C, blue = 0 };
            else if (120 <= hue && hue < 180)
                return new DS4Color { red = 0, green = C, blue = (byte)X };
            else if (180 <= hue && hue < 240)
                return new DS4Color { red = 0, green = (byte)X, blue = C };
            else if (240 <= hue && hue < 300)
                return new DS4Color { red = (byte)X, green = 0, blue = C };
            else if (300 <= hue && hue < 360)
                return new DS4Color { red = C, green = 0, blue = (byte)X };
            else
                return new DS4Color { red = 255, green = 0, blue = 0 };
        }

        public static DS4Color getTransitionedColor(DS4Color c1, double ratio, DS4Color c2)
        {
            c1.red = applyRatio(c1.red, c2.red, ratio);
            c1.green = applyRatio(c1.green, c2.green, ratio);
            c1.blue = applyRatio(c1.blue, c2.blue, ratio);
            return c1;
        }

        private static byte applyRatio(byte b1, byte b2, double r)
        {
            if (r > 100)
                r = 100;
            else if (r < 0)
                r = 0;
            uint ratio = (uint)r;
            if (b1 > b2)// b2 == 255)
            {
                ratio = 100 - (uint)r;
            }
            byte bmax = Math.Max(b1, b2);
            byte bmin = Math.Min(b1, b2);
            byte bdif = (byte)(bmax - bmin);
            return (byte)(bmin + (bdif * ratio / 100));
        }
    }
}
