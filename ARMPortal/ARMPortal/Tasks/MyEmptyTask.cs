#region Using Statements

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace ARMPortal.Tasks
{
    // Static can be removed
    public static class MyEmptyTask
    {
        // static can be removed
        public static async Task StartCalculation(int timeDelay, CancellationToken token, IProgress<int> progress)
        {
            for (var i = 0; i <= 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    if (progress != null)
                        progress.Report(100);
                    token.ThrowIfCancellationRequested();
                }
                if (progress != null)
                    progress.Report(i);

                await Task.Delay(timeDelay/100);
            }

            if (progress != null)
                progress.Report(100);
        }
    }
}