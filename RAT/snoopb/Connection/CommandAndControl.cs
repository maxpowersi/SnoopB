using System.Collections.Generic;
using System.IO;

namespace SnoopB.Connection
{
    public class CommandAndControl
    {
        //Get actions from the C&C
        public List<Action> GetActions(string uniqueId)
        {
            var actions = new List<Action>();
            return actions;
        }

        //Checks if the C&C is OK
        public bool Ping(string uniqueId)
        {
            return true;
        }

        //Get the module interval configuration from theC&C
        public List<ModuleConfiguration> GetModuleConfigurations(string uniqueId)
        {
            return new List<ModuleConfiguration>();
        }

        //Register the current machine like alive, and listening
        public void RegisterMachineAlive(string uniqueId)
        {
            
        }

        //Send 
        public void SendActionResponse(string uniqueId, Action action, FileStream file)
        {
            
        }
        public void SendActionResponse(string uniqueId, Action action, string text)
        {

        }

        public void SendModuleIntervalResponse(string uniqueId, string ModuleId, List<FileStream> files)
        {
            
        }
        public void SendModuleIntervalResponse(string uniqueId, string ModuleId, List<string> texts)
        {

        }
    }
}