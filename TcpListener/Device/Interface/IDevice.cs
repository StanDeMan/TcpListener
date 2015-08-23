namespace Listener.Device.Interface
{
    public enum Status
    {
		Off = 0,
        On = 1
    }

    interface ISimpleDevice
    {
        void Switch(Status status);
    }

    interface IDimmableDevice : ISimpleDevice
    {
        void Dim(float value);
    }
}
