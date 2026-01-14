using System;
using System.Collections.Generic;

namespace P3tr0viCh.Utils
{
    public class ProgramStatus<T> where T : Enum
    {
        public class Status
        {
            private readonly T status;

            public Status(T status)
            {
                this.status = status;
            }

            public T Value => status;
        }

        private readonly List<Status> statuses = new List<Status>();

        public ProgramStatus()
        {
        }

        public delegate void StatusChangedEventHandler(object sender, T status);
        public event StatusChangedEventHandler StatusChanged;

        public Status Start(T status)
        {
            var statusStart = new Status(status);

            statuses.Add(statusStart);

            StatusChanged?.Invoke(this, status);

            return statusStart;
        }

        public void Stop(Status status)
        {
            statuses.Remove(status);

            StatusChanged?.Invoke(this, Current);
        }

        public bool IsIdle => statuses.Count == 0;

        public T Current => IsIdle ? default : statuses[statuses.Count - 1].Value;

        public int Count(T status)
        {
            var count = 0;

            foreach (var stat in statuses)
            {
                if (stat.Value.Equals(status)) count++;
            }

            return count;
        }

        public bool Contains(T status)
        {
            foreach (var stat in statuses)
            {
                if (stat.Value.Equals(status)) return true;
            }

            return false;
        }
    }
}