using System;
using System.Collections;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    public class RoadAssistMod : IUserMod
    {
        public string Name
        {
            get { return "RoadAssist"; }
        }

        public string Description
        {
            get { return "Allows right-angle construction."; }
        }
    }
}
