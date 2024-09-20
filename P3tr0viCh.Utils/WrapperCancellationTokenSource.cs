using System.Threading;

namespace P3tr0viCh.Utils
{
    public class WrapperCancellationTokenSource
    {
        private CancellationTokenSource cancellationTokenSource;

        public void Finally()
        {
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        public void Cancel()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        public void Start()
        {
            Cancel();

            cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken Token => cancellationTokenSource.Token;

        public bool IsCancellationRequested => cancellationTokenSource.IsCancellationRequested;
    }
}
