using BluetoothConnectionManager;
using DollyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDolly
{
    public class Motor : ViewModelBase
    {
        public static class Property
        {
            public const char StepsPerUnit  = 'A';
            public const char Speed         = 'B';
            public const char Ramp          = 'C';
            public const char PostTime      = 'D';
            public const char Direction     = 'E';
        }

        private int _stepsPerUnit;  // steps
        private int _speed;         // mm/sec
        private int _ramp;          // sec
        private int _postTime;      // sec
        private bool  _direction;     // TODO: Chech the type! LEFT/RIGHT or FORWARD/REVERSED??

        public override char objectId() { return TargetObject.Motor; }

        public int StepsPerUnit
        {
            get { return _stepsPerUnit; }
            set { SetProperty(ref _stepsPerUnit, value); SetPropertyRemote(Property.StepsPerUnit, _stepsPerUnit); }
        }

        public int Speed
        {
            get { return _speed; }
            set { SetProperty(ref _speed, value); SetPropertyRemote(Property.Speed, _speed); }
        }

        public int Ramp
        {
            get { return _ramp; }
            set { SetProperty(ref _ramp, value); SetPropertyRemote(Property.Ramp, _ramp); }
        }

        public int PostTime
        {
            get { return _postTime; }
            set { SetProperty(ref _postTime, value); SetPropertyRemote(Property.PostTime, _postTime); }
        }

        public bool Direction
        {
            get { return _direction; }
            set { SetProperty(ref _direction, value); SetPropertyRemote(Property.Direction, Convert.ToInt32(_direction)); }
        }

        public void getInitValues()
        {
            GetPropertyRemote(Property.StepsPerUnit);
            GetPropertyRemote(Property.Speed);
            GetPropertyRemote(Property.Ramp);
            GetPropertyRemote(Property.PostTime);
            GetPropertyRemote(Property.Direction);
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
                    case Property.StepsPerUnit:
                        StepsPerUnit = newValue;
                        break;
                    case Property.Speed:
                        Speed = newValue;
                        break;
                    case Property.Ramp:
                        Ramp = newValue;
                        break;
                    case Property.PostTime:
                        PostTime = newValue;
                        break;
                    case Property.Direction:
                        Direction = (newValue != 0) ? true: false;
                        break;
                }
                newValueFromDevice = false;
            }
        }
    }
}
