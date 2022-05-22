using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WcfService.ServerClasses;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> onlineUsers = new List<ServerUser>(); // список сервиса о юзерах онлайн

        public bool Connect(long userId)
        {
            ServerUser onlineUser = new ServerUser()
            {
                Id = userId,
                OperationContext = OperationContext.Current,
            };

            onlineUsers.Add(onlineUser);
            return true; // юзера успешно подлючили => возвращаем true
        }

        public bool Disconnect(long userId)
        {
            // находим юзера, который хочет отключиться от рассылки сообщений, и убираем его из списка сервера,
            // о тех юзерах, которым слать сообщения нужно
            var disconnectingUser = onlineUsers.FirstOrDefault(x => x.Id == userId);

            if (disconnectingUser != null)
            {
                onlineUsers.Remove(disconnectingUser);
            }

            return true; // // юзера успешно отключили от сети => возвращаем true
        }

        public long[] GetOnlineUsersId()
        {
            long[] onlineUsersId = new long[onlineUsers.Count()];

            for(int i = 0; i < onlineUsersId.Length; i++)
            {
                onlineUsersId[i] = onlineUsers[i].Id;
            }

            return onlineUsersId;
        }

        public void SendMsg(string msg, long userId)
        {
            foreach (ServerUser user in onlineUsers)
            {
                user.OperationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(msg, userId);
            }
        }
    }
}
