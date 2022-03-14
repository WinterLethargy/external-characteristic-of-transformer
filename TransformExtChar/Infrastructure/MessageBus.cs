using System;
using System.Collections.Generic;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public static class MessageBus
    {
        public static event Action<object, object> Bus;

        public static void Send(object message, object param = null)
            => Bus?.Invoke(message, param);
    }
}
