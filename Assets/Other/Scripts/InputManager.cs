using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

//Custom input management with customizable inputs
public static class InputManager
{
    private static Dictionary<string, KeyCode> keybinds;
    public class InputCustomKeybinds
    {
        //Default keybinds
        public string Forward = "W";
        public string Backward = "S";
        public string Left = "A";
        public string Right = "D";
        public string Jump = "Space";
        public string Sprint = "LeftShift";
        public string ToggleInventory = "E";
        public string PickupItem = "F";
        public string DropItem = "Q";
        public string ActivateEquipedItem = "Mouse0";
        public string PauseMenu = "O";
    }
    //Turn a string to a key code
    private static KeyCode String2Keycode(string s) { return (KeyCode)System.Enum.Parse(typeof(KeyCode), s); }
    //Load the keybinds then apply them
    public static void SetupKeybinds()
    {
        InputCustomKeybinds customKeybinds = (InputCustomKeybinds)SaverLoader.Load("controls.json", new InputCustomKeybinds(), typeof(InputCustomKeybinds));
        keybinds = new Dictionary<string, KeyCode>();
        FieldInfo[] properties = typeof(InputCustomKeybinds).GetFields();
        for (int i = 0; i < properties.Length; i++)
        {
            keybinds.Add(properties[i].Name, String2Keycode((string)properties[i].GetValue(customKeybinds)));
        }
    }
    //Get if the keyTag is pressed
    public static bool GetKey(string keyTag)
    {
        return Input.GetKey(keybinds[keyTag]);
    }
    //Returns true at the exact moment the keyTag got pressed
    public static bool GetKeyPress(string keyTag)
    {
        return Input.GetKeyDown(keybinds[keyTag]);
    }
    //Returns true at the exact moment the keyTag got released
    public static bool GetKeyRelease(string keyTag)
    {
        return Input.GetKeyUp(keybinds[keyTag]);
    }
}