using DollyProtocol;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartDolly
{

    public class SetPropertyEventArgs : EventArgs
    {
        public char TargetObject { get; set; }
        public char Property { get; set; }
        public char Command { get; set; }
        public string Value { get; set; }
    }

    public class GetPropertyEventArgs : EventArgs
    {
        public char TargetObject { get; set; }
        public char Property { get; set; }
        public char Command { get; set; }
    }

    public abstract class ViewModelBase : BindableBase
    {
        public event EventHandler<SetPropertyEventArgs> SetPropertyEventHandler;

        public virtual char objectId() { return '.'; }

        public ViewModelBase() { newValueFromDevice = false; }

        protected bool newValueFromDevice;

        protected virtual void SetPropertyRemote(char property, Int32 val)
        {
            if (newValueFromDevice) // Property value was received from the device (either as a GET response or SET), don't send it back!
            {
                return; 
            }
            else // Property value was set on the phone UI, send it to device!
            {
                SetPropertyEventArgs response = new SetPropertyEventArgs();
                response.TargetObject = objectId();
                response.Property = property;
                response.Command = Command.Set;
                response.Value = val.ToString();
                Debug.WriteLine("Set remote property: " + response.Property + " to " + response.Value);
                if (SetPropertyEventHandler != null)
                {
                    SetPropertyEventHandler(this, response);
                }
            }
        }

        public event EventHandler<GetPropertyEventArgs> GetPropertyEventHandler;

        protected virtual void GetPropertyRemote(char property)
        {
            GetPropertyEventArgs propertyToInit = new GetPropertyEventArgs();
            propertyToInit.TargetObject = objectId();
            propertyToInit.Property = property;
            propertyToInit.Command = Command.Get;
            if (GetPropertyEventHandler != null)
            {
                GetPropertyEventHandler(this, propertyToInit);
            }
        }
    }
}
