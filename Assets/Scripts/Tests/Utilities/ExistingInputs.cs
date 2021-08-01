using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests.Utilities
{
    [Serializable]
    public static class ExistingInputs
    {
         public static InputDevice[] preExistingDevices;

        public static InputDevice[] GetExistingDevices()
        {
            preExistingDevices ??= InputSystem.devices.ToArray();
            return preExistingDevices;
        }
    }
}
