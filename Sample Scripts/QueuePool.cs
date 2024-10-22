using System;
using System.Collections.Generic;

namespace Medimind.Scripts
{
    public class QueueDate<T>
    {
        public DateTime time;
        public T data;

        public QueueDate(T data)
        {
            this.time = DateTime.Now;
            this.data = data;
        }
    }
    public class QueuePool
    {
        public class QueueDirector<T>
        {
            public int Count { get { return que.Count; } }

            public Queue<QueueDate<T>> que = new Queue<QueueDate<T>>();
            public Action<QueueDate<T>> action;

            public QueueDirector(Action<QueueDate<T>> action)
            {
                this.action = action;
            }

            public void Add(T data)
            {
                que.Enqueue(new QueueDate<T>(data));
            }
            public QueueDate<T> Get()
            {
                QueueDate<T> dequeue = que.Dequeue();
                if (action != null)
                    action.Invoke(dequeue);

                return dequeue;
            }
        }

        public static QueueDirector<string> log;
        public static QueueDirector<byte[]> date;

        public static void InitLog(Action<QueueDate<string>> feedback)
        {
            log = new QueueDirector<string>(feedback);
        }
        public static void InitData(Action<QueueDate<byte[]>> feedback)
        {
            date = new QueueDirector<byte[]>(feedback);
        }

        public static void Update(object sender, EventArgs e)
        {
            while (0 < date.Count)
            {
                date.Get();
            }
            while (0 < log.Count)
            {
                QueueDate<string> logData = log.Get();
                Console.WriteLine($"[{logData.time}]{logData.data}");
            }
        }
    }
}
