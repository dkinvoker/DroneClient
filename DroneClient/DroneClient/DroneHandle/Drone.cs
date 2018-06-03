using AR.Drone.WinApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneClient.DroneHandle
{
    public static class Drone
    {
        public static string CommandsText(Boolean[] commands)
        {
            string result = string.Empty;

            if (commands[CommandBooleanPossition.EmergencyIndex])
                result += "Emergency; ";
            if (commands[CommandBooleanPossition.HoverIndex])
                result += "Hover; ";
            if (commands[CommandBooleanPossition.LandIndex])
                result += "Land; ";
            if (commands[CommandBooleanPossition.ResetEmergencyIndex])
                result += "ResetEmergency; ";
            if (commands[CommandBooleanPossition.StartIndex])
                result += "Start; ";
            if (commands[CommandBooleanPossition.StopIndex])
                result += "Stop; ";
            if (commands[CommandBooleanPossition.TakeoffIndex])
                result += "Takeoff; ";

            return result;
        }
    }
}
