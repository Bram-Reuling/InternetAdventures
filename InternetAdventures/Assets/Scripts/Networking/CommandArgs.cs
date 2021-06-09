using System;
using UnityEngine;

namespace Networking
{
    public class CommandArgs : MonoBehaviour
    {
        private static string cmdInfo = "";

        private void Start()
        {
            string[] arguments = Environment.GetCommandLineArgs();

            foreach (string argument in arguments)
            {
                cmdInfo += argument.ToString() + "\n";
            }
        }

        private void OnGUI()
        {
            Rect r = new Rect(5, 5, 800, 500);
            GUI.Label(r,cmdInfo);
        }
    }
}