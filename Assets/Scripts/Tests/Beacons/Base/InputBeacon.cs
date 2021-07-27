using System;
using System.Collections;
using E7.Minefield;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Utilities;

namespace Tests.Beacons.Base
{
    public class InputBeacon
    {
        private readonly InputTestFixture inputTestFixture = new InputTestFixture();
        private Mouse mouse;
        private Keyboard keyboard;

        private Mouse Mouse => mouse ??= InputSystem.AddDevice<Mouse>();
        private Keyboard Keyboard => keyboard ??= InputSystem.AddDevice<Keyboard>();

        public IEnumerator PressKey(Func<Keyboard, KeyControl> func)
        {
            inputTestFixture.PressAndRelease(func(Keyboard));
            yield return null;
        }
        
        public IEnumerator PressKey(KeyControl keyControl)
        {
            inputTestFixture.PressAndRelease(keyControl);
            yield return null;
        }

        public IEnumerator ClickLeft<T>(T label, bool ignoreError = false) where T : Enum =>
            ClickButton(label, Mouse.leftButton, ignoreError);
        
        public IEnumerator ClickRight<T>(T label, bool ignoreError = false) where T : Enum =>
            ClickButton(label, Mouse.rightButton, ignoreError);
        
        private IEnumerator ClickButton<T>(T label, ButtonControl buttonControl, bool ignoreError = false) where T : Enum
        {
            if (Beacon.FindActive(label, out ILabelBeacon foundBeacon) && foundBeacon is ScreenBeacon<T> screenBeacon)
            {
                inputTestFixture.Move(Mouse.position, screenBeacon.ScreenClickPosition);
                TestHack.mouseMove = true;
                yield return null;
                TestHack.mouseMove = false;
                
                if (buttonControl == Mouse.rightButton)
                    TestHack.rightClick = true;
                if (buttonControl == Mouse.leftButton)
                    TestHack.leftClick = true;
                
                inputTestFixture.PressAndRelease(buttonControl);
                yield return null;
                
                if (buttonControl == Mouse.rightButton)
                    TestHack.rightClick = false;
                if (buttonControl == Mouse.leftButton)
                    TestHack.leftClick = false;
                // We need this to wait a frame, so we don't do stuff before it has time to process click
                yield return null;
                
            }
            else
            {
                if (!ignoreError)
                {
                    throw new Exception($"Label {label} not found on any screen beacons in the scene");
                }
            }
        }
        
        public IEnumerator ClickLeftWhen<T>(T label, BeaconConstraint bc, 
                                             bool ignoreError = false) where T : Enum
        {
            yield return ClickButtonWhen(label, bc, Mouse.leftButton, ignoreError);
        }
        
        public IEnumerator ClickRightWhen<T>(T label, BeaconConstraint bc, 
                                              bool ignoreError = false) where T : Enum
        {
            yield return ClickButtonWhen(label, bc, Mouse.rightButton, ignoreError);
        }
        
        private IEnumerator ClickButtonWhen<T>(T label, BeaconConstraint bc, ButtonControl buttonControl, 
                                               bool ignoreError = false) where T : Enum
        {
            yield return Beacon.WaitUntil(label, bc);
            yield return ClickButton(label, buttonControl, ignoreError);
        }
    }
}
