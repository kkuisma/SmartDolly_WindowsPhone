using BluetoothConnectionManager;
using DollyProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDolly
{
    public class Camera : ViewModelBase
    {
        public static class Property
        {
            public const char ExposureTime      = 'A';
            public const char PostExposureTime  = 'B';
            public const char FocusTime         = 'C';
            public const char ExposureMode      = 'D';
            public const char FocalLength       = 'E';
        }

        private int _exposureTime;      // milliseconds
        private int _postExposureTime;  // milliseconds
        private int _focusTime;         // milliseconds
        private bool _exposureMode;     // BULB=false or CAMERA=true
        private int _focalLength;       // mm
         
        public override char objectId() { return TargetObject.Camera; }

        public Int32 ExposureTime
        {
            get { return _exposureTime; }
            set { SetProperty(ref _exposureTime, value); SetPropertyRemote(Property.ExposureTime, _exposureTime); }
        }

        public Int32 PostExposureTime
        {
            get { return _postExposureTime; }
            set { SetProperty(ref _postExposureTime, value); SetPropertyRemote(Property.PostExposureTime, _postExposureTime); }
        }

        public Int32 FocusTime
        {
            get { return _focusTime; }
            set { SetProperty(ref _focusTime, value); SetPropertyRemote(Property.FocusTime, _focusTime); }
        }

        public bool ExposureMode
        {
            get { return _exposureMode; }
            set { SetProperty(ref _exposureMode, value); SetPropertyRemote(Property.ExposureMode, Convert.ToInt32(_exposureMode)); }
        }

        public Int32 FocalLength
        {
            get { return _focalLength; }
            set { SetProperty(ref _focalLength, value); SetPropertyRemote(Property.FocalLength, _focalLength); }
        }

        public void getInitValues()
        {
            GetPropertyRemote(Property.ExposureTime);
            GetPropertyRemote(Property.PostExposureTime);
            GetPropertyRemote(Property.FocusTime);
            GetPropertyRemote(Property.ExposureMode);
            GetPropertyRemote(Property.FocalLength);
        }

        public void handleMessage(string msg)
        {
            string[] messageArray = msg.Split(Protocol.ValueSeparator);

            char property = msg[0];
            char command = msg[1];

            if (command == Command.GetResponse)
            {
                newValueFromDevice = true;

                Int32 newValue = Convert.ToInt32(messageArray[1]);
                switch (property)
                {
                    case Property.ExposureTime:
                        ExposureTime = newValue;
                        break;
                    case Property.PostExposureTime:
                        PostExposureTime = newValue;
                        break;
                    case Property.FocusTime:
                        FocusTime = newValue;
                        break;
                    case Property.ExposureMode:
                        ExposureMode = (newValue != 0) ? true : false;
                        break;
                    case Property.FocalLength:
                        FocalLength = newValue;
                        break;
                }
                newValueFromDevice = false;
            }
        }
    }
}
