using System.Collections.Generic;

namespace P3tr0viCh.Utils
{
    public class ProgramStatus<T> where T : System.Enum
    {
        public class Status
        {
            private readonly T status;

            public Status(T status)
            {
                this.status = status;
            }

            public T Value { get { return status; } }
        }

        private readonly List<Status> statuses = new List<Status>();

        private readonly T idle;

        public ProgramStatus(T idle)
        {
            this.idle = idle;
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

        public bool IsIdle()
        {
            return statuses.Count == 0;
        }

        public T Current
        {
            get
            {
                if (IsIdle())
                {
                    return idle;
                }
                else
                {
                    return statuses[statuses.Count - 1].Value;
                }
            }
        }
    }
}