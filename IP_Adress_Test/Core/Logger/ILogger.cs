namespace IP_Adress_Test.Core.Logger
{
    internal interface ILogger
    {
        void Info<T>(T message);
        void Warn<T>(T message);
        void Error<T>(T message);
    }
}