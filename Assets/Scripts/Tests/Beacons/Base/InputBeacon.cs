using System;
using System.Collections;
using E7.Minefield;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
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

        public void PrepareVirtualDevices()
        {
            var preExistingDevices = InputSystem.devices.ToArray();
            
            mouse = Mouse;
            keyboard = Keyboard;
            
            Mouse.MakeCurrent();
            Keyboard.MakeCurrent();
            
            foreach (var inputDevice in preExistingDevices)
            {
                InputSystem.RemoveDevice(inputDevice);
            }
        }

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
            ClickButton(label, MouseButton.Left, ignoreError);
        
        public IEnumerator ClickRight<T>(T label, bool ignoreError = false) where T : Enum =>
            ClickButton(label, MouseButton.Right, ignoreError);
        
        private IEnumerator ClickButton<T>(T label, MouseButton mouseButton, bool ignoreError = false) where T : Enum
        {
            if (Beacon.FindActive(label, out ILabelBeacon foundBeacon) && foundBeacon is ScreenBeacon<T> screenBeacon)
            {
                var pressState = new MouseState
                {
                    position = screenBeacon.ScreenClickPosition
                };
                pressState.WithButton(mouseButton, true);

                var releaseState = new MouseState
                {
                    position = screenBeacon.ScreenClickPosition
                };
                releaseState.WithButton(mouseButton, false);

                InputSystem.QueueStateEvent(Mouse, pressState);
                // Make sure to wait one frame so that Monobehaviour.Update() can listen to the change
                yield return null;
                InputSystem.QueueStateEvent(Mouse, releaseState);
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
            yield return ClickButtonWhen(label, bc, MouseButton.Left, ignoreError);
        }
        
        public IEnumerator ClickRightWhen<T>(T label, BeaconConstraint bc, 
                                              bool ignoreError = false) where T : Enum
        {
            yield return ClickButtonWhen(label, bc, MouseButton.Right, ignoreError);
        }
        
        private IEnumerator ClickButtonWhen<T>(T label, BeaconConstraint bc, MouseButton buttonControl, 
                                               bool ignoreError = false) where T : Enum
        {
            yield return Beacon.WaitUntil(label, bc);
            yield return ClickButton(label, buttonControl, ignoreError);
        }
    }
}
