using System;
using System.Collections.Generic;
using XLua;
using Yoozoo.Managers.NetworkV2.Core;

public static class EditorNetUtility
{


    [CSharpCallLua]
    public static List<Type> s
    {
        get
        {
            List<Type> list = new List<Type>();
            list.AddRange(new Type[] {
                typeof(NetDelegates.ChannelDelegates.NetworkChannelConnectedHandler ),
                typeof(NetDelegates.ChannelDelegates.NetworkChannelReconnectedHandler ),
                typeof(NetDelegates.ChannelDelegates.NetworkChannelReceivePacketHandler ),
                typeof(NetDelegates.ChannelDelegates.NetworkChannelStartUpHandler ),
                typeof(NetDelegates.ChannelDelegates.NetworkChannelClosedHandler ),
                typeof(NetDelegates.ChannelDelegates.NetworkChannelErrorHandler )
            });
            return list;
        }
    }



}
