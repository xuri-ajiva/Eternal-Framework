using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eternal.MathE {
    /// <summary>
    /// MicroStopwatch class
    /// </summary>
    public class MicroStopwatch : System.Diagnostics.Stopwatch {
        public const double µs = 1000000D;

        readonly double _microSecPerTick = µs / Frequency;

        public MicroStopwatch() {
            if ( !IsHighResolution ) {
                throw new Exception( "On this system the high-resolution " + "performance counter is not available" );
            }
        }

        public long ElapsedMicroseconds { get { return (long) ( ElapsedTicks * this._microSecPerTick ); } }
    }

    /// <summary>
    /// MicroTimer class
    /// </summary>
    public class MicroTimer {
        public delegate void MicroTimerElapsedEventHandler(object sender, MicroTimerEventArgs timerEventArgs);

        public event MicroTimerElapsedEventHandler MicroTimerElapsed;

        System.Threading.Thread _threadTimer             = null;
        long                    _ignoreEventIfLateBy     = long.MaxValue;
        long                    _timerIntervalInMicroSec = 0;
        bool                    _stopTimer               = true;

        public MicroTimer() { }

        public MicroTimer(long timerIntervalInMicroseconds) { Interval = timerIntervalInMicroseconds; }

        public long Interval {
            get { return System.Threading.Interlocked.Read( ref this._timerIntervalInMicroSec ); }
            set { System.Threading.Interlocked.Exchange( ref this._timerIntervalInMicroSec, value ); }
        }

        public long IgnoreEventIfLateBy {
            get { return System.Threading.Interlocked.Read( ref this._ignoreEventIfLateBy ); }
            set { System.Threading.Interlocked.Exchange( ref this._ignoreEventIfLateBy, value <= 0 ? long.MaxValue : value ); }
        }

        public bool Enabled {
            set {
                if ( value ) {
                    Start();
                }
                else {
                    Stop();
                }
            }
            get { return ( this._threadTimer != null && this._threadTimer.IsAlive ); }
        }

        public void Start() {
            if ( Enabled || Interval <= 0 ) {
                return;
            }

            this._stopTimer = false;

            System.Threading.ThreadStart threadStart = delegate() {
                NotificationTimer( ref this._timerIntervalInMicroSec, ref this._ignoreEventIfLateBy, ref this._stopTimer );
            };

            this._threadTimer          = new System.Threading.Thread( threadStart );
            this._threadTimer.Priority = System.Threading.ThreadPriority.Highest;
            this._threadTimer.Start();
        }

        public void Stop() { this._stopTimer = true; }

        public void StopAndWait() { StopAndWait( System.Threading.Timeout.Infinite ); }

        public bool StopAndWait(int timeoutInMilliSec) {
            this._stopTimer = true;

            if ( !Enabled || this._threadTimer.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId ) {
                return true;
            }

            return this._threadTimer.Join( timeoutInMilliSec );
        }

        public void Abort() {
            this._stopTimer = true;

            if ( Enabled ) {
                this._threadTimer.Abort();
            }
        }

        void NotificationTimer(ref long timerIntervalInMicroSec, ref long ignoreEventIfLateBy, ref bool stopTimer) {
            int  timerCount       = 0;
            long nextNotification = 0;

            MicroStopwatch microStopwatch = new MicroStopwatch();
            microStopwatch.Start();

            while ( !stopTimer ) {
                long callbackFunctionExecutionTime = microStopwatch.ElapsedMicroseconds - nextNotification;

                long timerIntervalInMicroSecCurrent = System.Threading.Interlocked.Read( ref timerIntervalInMicroSec );
                long ignoreEventIfLateByCurrent     = System.Threading.Interlocked.Read( ref ignoreEventIfLateBy );

                nextNotification += timerIntervalInMicroSecCurrent;
                timerCount++;
                long elapsedMicroseconds = 0;

                while ( ( elapsedMicroseconds = microStopwatch.ElapsedMicroseconds ) < nextNotification ) {
                    System.Threading.Thread.SpinWait( 10 );
                }

                long timerLateBy = elapsedMicroseconds - nextNotification;

                if ( timerLateBy >= ignoreEventIfLateByCurrent ) {
                    continue;
                }

                MicroTimerEventArgs microTimerEventArgs =
                    new MicroTimerEventArgs( timerCount, elapsedMicroseconds, timerLateBy, callbackFunctionExecutionTime );
                this.MicroTimerElapsed( this, microTimerEventArgs );
            }

            microStopwatch.Stop();
        }
    }

    /// <summary>
    /// MicroTimer Event Argument class
    /// </summary>
    public class MicroTimerEventArgs : EventArgs {
        // Simple counter, number times timed event (callback function) executed
        public int TimerCount { get; private set; }

        // Time when timed event was called since timer started
        public long ElapsedMicroseconds { get; private set; }

        // How late the timer was compared to when it should have been called
        public long TimerLateBy { get; private set; }

        // Time it took to execute previous call to callback function (OnTimedEvent)
        public long CallbackFunctionExecutionTime { get; private set; }

        public MicroTimerEventArgs(int timerCount, long elapsedMicroseconds, long timerLateBy, long callbackFunctionExecutionTime) {
            TimerCount                    = timerCount;
            ElapsedMicroseconds           = elapsedMicroseconds;
            TimerLateBy                   = timerLateBy;
            CallbackFunctionExecutionTime = callbackFunctionExecutionTime;
        }
    }
}