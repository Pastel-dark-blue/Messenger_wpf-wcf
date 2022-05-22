using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(IServerChatCallback))]
    public interface IServiceChat
    {
        [OperationContract]
        bool Connect(long userId);

        [OperationContract]
        bool Disconnect(long userId);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, long userId);

        [OperationContract]
        // массив id пользователей онлайн (т.е. тех, кто подключен к сервису)
        long[] GetOnlineUsersId();
    }

    public interface IServerChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg, long userId);
    }
}
