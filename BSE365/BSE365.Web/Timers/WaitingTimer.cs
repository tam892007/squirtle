using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using BSE365.Common.Constants;
using BSE365.Helper;

namespace BSE365.Timers
{
    public class WaitingTimer
    {
        private System.Timers.Timer _timer;
        public bool IsRunning { get; private set; }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                timer_Elapsed(null, null);
                this._timer = new System.Timers.Timer(TransactionConfig.WaitingHelperTimeout);
                this._timer.AutoReset = true;
                this._timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
                this._timer.Enabled = true;
                this._timer.Start();
            }
        }


        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                this._timer.Stop();
                this._timer = null;
            }
        }


        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Trace.WriteLine("Running");
                Execute();
                Trace.WriteLine("---");
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Exception : {0}", exception.Message);
            }
        }

        public void Execute()
        {
            Thread thread = new Thread(StoreHelper.AutoQueueReceive);
            thread.Start();
        }
    }
}