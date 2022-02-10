using System;
using System.Collections.Generic;
using System.Text;

namespace TransformExtChar.Infrastructure
{
    public static class MessageBus
    {
        public static event Action<object> Bus;

        public static void Send(object data)
            => Bus?.Invoke(data);
    }
}
