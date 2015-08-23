namespace Listener.Device.Devices
{
    //Listener.
    using Driver;

    //Listener.Device.
    using Interface;

    class Lamp : ISimpleDevice
    {
        private static Pwm pwm;

        public Lamp()
        {
            pwm = new Pwm();
        }

        public void Switch(Status status)
        {
            switch (status)
            {
                case Status.On:
                    pwm.Set(Pwm.HeaderP1.Pin7, 1);
                    break;

                case Status.Off:
                    pwm.Set(Pwm.HeaderP1.Pin7, 0);
                    break;
            }
        }
    }
}
