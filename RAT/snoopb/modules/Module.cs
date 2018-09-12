using System;

namespace SnoopB.Modules
{
    public abstract class Module
    {
        protected Module(int minutesInterval)
        {
            MinutesInterval = minutesInterval;
        }

        public void Execute()
        {
            try
            {
                if (IsEnabled)
                    ExecuteInternal();
            }
            catch (Exception)
            {

            }
        }

        public void Execute(params object[] parameters)
        {
            try
            {
                if (IsEnabled)
                {
                    Parameters = parameters;
                    ExecuteInternal();
                }
            }
            catch (Exception)
            {

            }
        }

        protected abstract void ExecuteInternal();

        public static string ModuleId { get;}

        public int MinutesInterval { get; protected set; }

        public virtual bool IsEnabled
        {
            get { return MinutesInterval > 0; }
        }

        protected object[] Parameters { get; set; }
    }
}