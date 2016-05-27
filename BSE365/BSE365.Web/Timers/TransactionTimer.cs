using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BSE365.Timers
{
    public class TransactionTimer
    {
        private System.Timers.Timer _timer;
        private double timerTick = /*60*60*1000;*/ 10*60*1000; //for test

        public TransactionTimer()
        {
            this._timer = new System.Timers.Timer(timerTick); // 30000 milliseconds = 30 seconds
            this._timer.AutoReset = true;
            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            this._timer.Enabled = true;
        }


        public void Start()
        {
            timer_Elapsed(null, null);
            this._timer.Start();
        }


        public void Stop()
        {
            this._timer.Stop();
            this._timer = null;
        }


        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Trace.WriteLine("Running");
                //SyncHelper.Sync();
                Trace.WriteLine("---");
            }
            catch (Exception exception)
            {
                Trace.WriteLine("Exception : {0}", exception.Message);
            }
        }
    }
}