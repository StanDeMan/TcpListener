namespace Listener.Device.Devices
{
    //Listener.
    using Driver;
    //Listener.Device.
    using Interface;

    class LedStrip : IDimmableDevice
    {
        private static double value;
        private static Pwm pwm;

        public LedStrip()
        {
            //TODO: later we store this value and read the stored value for preset
            value = 0.8;
            pwm = new Pwm();
        }

        public void Switch(Status status)
        {
            switch (status)
            {
                case Status.On:
                    pwm.Set(Pwm.HeaderP1.Pin7, value < 1 ? value : 1);
                    break;

                case Status.Off:
                    pwm.Set(Pwm.HeaderP1.Pin7, 0);
                    break;
            }
        }

        public void Dim(float newValue)
        {
            value = newValue;
            pwm.Set(Pwm.HeaderP1.Pin7, value); 
        }
    }
}
