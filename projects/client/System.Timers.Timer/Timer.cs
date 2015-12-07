using System.Reflection;
using System.Threading;

namespace System.Timers
{
    /// <summary>Generates recurring events in an application.</summary>
    public class Timer : IDisposable
    {
        private double interval;

        private bool enabled;

        private bool initializing;

        private bool delayedEnable;

        private ElapsedEventHandler onIntervalElapsed;

        private bool autoReset;

        private object synchronizingObject;

        private bool disposed;

        private System.Threading.Timer timer;

        private TimerCallback callback;

        private object cookie;

        /// <summary>Occurs when the interval elapses.</summary>
        public event ElapsedEventHandler Elapsed
        {
            add
            {
                this.onIntervalElapsed = (ElapsedEventHandler)Delegate.Combine(this.onIntervalElapsed, value);
            }
            remove
            {
                this.onIntervalElapsed = (ElapsedEventHandler)Delegate.Remove(this.onIntervalElapsed, value);
            }
        }

        /// <summary>Gets or sets a value indicating whether the <see cref="T:System.Timers.Timer" /> should raise the <see cref="E:System.Timers.Timer.Elapsed" /> event each time the specified interval elapses or only after the first time it elapses.</summary>
        /// <returns>true if the <see cref="T:System.Timers.Timer" /> should raise the <see cref="E:System.Timers.Timer.Elapsed" /> event each time the interval elapses; false if it should raise the <see cref="E:System.Timers.Timer.Elapsed" /> event only once, after the first time the interval elapses. The default is true.</returns>
        public bool AutoReset
        {
            get
            {
                return this.autoReset;
            }
            set
            {
                if (this.autoReset != value)
                {
                    this.autoReset = value;
                    if (this.timer != null)
                    {
                        this.UpdateTimer();
                    }
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether the <see cref="T:System.Timers.Timer" /> should raise the <see cref="E:System.Timers.Timer.Elapsed" /> event.</summary>
        /// <returns>true if the <see cref="T:System.Timers.Timer" /> should raise the <see cref="E:System.Timers.Timer.Elapsed" /> event; otherwise, false. The default is false.</returns>
        /// <exception cref="T:System.ObjectDisposedException">This property cannot be set because the timer has been disposed.</exception>
        /// <exception cref="T:System.ArgumentException">The <see cref="P:System.Timers.Timer.Interval" /> property was set to a value greater than <see cref="F:System.Int32.MaxValue" /> before the timer was enabled. </exception>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.initializing)
                {
                    this.delayedEnable = value;
                    return;
                }
                if (this.enabled != value)
                {
                    if (!value)
                    {
                        if (this.timer != null)
                        {
                            this.cookie = null;
                            this.timer.Dispose();
                            this.timer = null;
                        }
                        this.enabled = value;
                        return;
                    }
                    this.enabled = value;
                    if (this.timer == null)
                    {
                        if (this.disposed)
                        {
                            throw new ObjectDisposedException(base.GetType().Name);
                        }
                        int num = (int)Math.Ceiling(this.interval);
                        this.cookie = new object();
                        this.timer = new System.Threading.Timer(this.callback, this.cookie, num, this.autoReset ? num : -1);
                        return;
                    }
                    else
                    {
                        this.UpdateTimer();
                    }
                }
            }
        }

        /// <summary>Gets or sets the interval at which to raise the <see cref="E:System.Timers.Timer.Elapsed" /> event.</summary>
        /// <returns>The time, in milliseconds, between <see cref="E:System.Timers.Timer.Elapsed" /> events. The value must be greater than zero and less than or equal to <see cref="F:System.Int32.MaxValue" />. The default is 100 milliseconds.</returns>
        /// <exception cref="T:System.ArgumentException">The interval is less than or equal to zero.-or-The interval is greater than <see cref="F:System.Int32.MaxValue" />, and the timer is currently enabled. (If the timer is not currently enabled, no exception is thrown until it is enabled.) </exception>
        public double Interval
        {
            get
            {
                return this.interval;
            }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentException("Interval cannot be less than or equal to 0", "value");
                }
                this.interval = value;
                if (this.timer != null)
                {
                    this.UpdateTimer();
                }
            }
        }

        /// <summary>Gets or sets the object used to marshal event-handler calls that are issued when an interval has elapsed.</summary>
        /// <returns>The <see cref="T:System.ComponentModel.ISynchronizeInvoke" /> representing the object used to marshal the event-handler calls that are issued when an interval has elapsed. The default is null.</returns>
        public object SynchronizingObject
        {
            get
            {
                return this.synchronizingObject;
            }
            set
            {
                this.synchronizingObject = value;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Timers.Timer" /> class, and sets all the properties to their initial values.</summary>
        public Timer()
        {
            this.interval = 100.0;
            this.enabled = false;
            this.autoReset = true;
            this.initializing = false;
            this.delayedEnable = false;
            this.callback = new TimerCallback(this.MyTimerCallback);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Timers.Timer" /> class, and sets the <see cref="P:System.Timers.Timer.Interval" /> property to the specified number of milliseconds.</summary>
        /// <param name="interval">The time, in milliseconds, between events. The value must be greater than zero and less than or equal to <see cref="F:System.Int32.MaxValue" />.</param>
        /// <exception cref="T:System.ArgumentException">The value of the <paramref name="interval" /> parameter is less than or equal to zero, or greater than <see cref="F:System.Int32.MaxValue" />. </exception>
        public Timer(double interval) : this()
        {
            if (interval <= 0.0)
            {
                throw new ArgumentException("Interval must be greater than 0", "interval");
            }
            double num = Math.Ceiling(interval);
            if (num > 2147483647.0 || num <= 0.0)
            {
                throw new ArgumentException("Interval too large", "interval");
            }
            this.interval = (double)((int)num);
        }

        private void UpdateTimer()
        {
            int num = (int)Math.Ceiling(this.interval);
            this.timer.Change(num, this.autoReset ? num : -1);
        }

        /// <summary>Begins the run-time initialization of a <see cref="T:System.Timers.Timer" /> that is used on a form or by another component.</summary>
        public void BeginInit()
        {
            this.Close();
            this.initializing = true;
        }

        /// <summary>Releases the resources used by the <see cref="T:System.Timers.Timer" />.</summary>
        public void Close()
        {
            this.initializing = false;
            this.delayedEnable = false;
            this.enabled = false;
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }

        /// <summary>Ends the run-time initialization of a <see cref="T:System.Timers.Timer" /> that is used on a form or by another component.</summary>
        public void EndInit()
        {
            this.initializing = false;
            this.Enabled = this.delayedEnable;
        }

        /// <summary>Starts raising the <see cref="E:System.Timers.Timer.Elapsed" /> event by setting <see cref="P:System.Timers.Timer.Enabled" /> to true.</summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="T:System.Timers.Timer" /> is created with an interval equal to or greater than <see cref="F:System.Int32.MaxValue" /> + 1, or set to an interval less than zero.</exception>
        public void Start()
        {
            this.Enabled = true;
        }

        /// <summary>Stops raising the <see cref="E:System.Timers.Timer.Elapsed" /> event by setting <see cref="P:System.Timers.Timer.Enabled" /> to false.</summary>
        public void Stop()
        {
            this.Enabled = false;
        }

        private void MyTimerCallback(object state)
        {
            if (state != this.cookie)
            {
                return;
            }
            if (!this.autoReset)
            {
                this.enabled = false;
            }
            ElapsedEventArgs elapsedEventArgs = new ElapsedEventArgs(DateTime.Now);
            try
            {
                ElapsedEventHandler elapsedEventHandler = this.onIntervalElapsed;
                if (elapsedEventHandler != null)
                {
                    MethodInfo beginInvokeMI = null;
                    bool invokeThroughSynchronizingObject = false;

                    if (SynchronizingObject != null)
                    {
                        PropertyInfo invokeRequiredPI = SynchronizingObject.GetType().GetTypeInfo().GetDeclaredProperty("InvokeRequired");
                        if (invokeRequiredPI != null)
                        {
                            object invokeRequiredValue = invokeRequiredPI.GetValue(SynchronizingObject);
                            if (invokeRequiredValue is bool && (bool)invokeRequiredValue)
                            {
                                beginInvokeMI = SynchronizingObject.GetType().GetTypeInfo().GetDeclaredMethod("BeginInvoke");
                                if (beginInvokeMI != null)
                                {
                                    ParameterInfo[] beginInvokeParameters = beginInvokeMI.GetParameters();
                                    if (beginInvokeParameters.Length == 2 && beginInvokeParameters[0].ParameterType == typeof(Delegate) && beginInvokeParameters[1].ParameterType == typeof(object[]))
                                    {
                                        invokeThroughSynchronizingObject = true;
                                    }
                                }
                            }
                        }
                    }

                    if (invokeThroughSynchronizingObject)
                        beginInvokeMI.Invoke(SynchronizingObject, new object[] { elapsedEventHandler, new object[] { this, elapsedEventArgs } });
                    else
                        elapsedEventHandler(this, elapsedEventArgs);
                }
            }
            catch
            {
            }
        }

        /// <summary>Releases all resources used by the current <see cref="T:System.Timers.Timer" />.</summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>Releases all resources used by the current <see cref="T:System.Timers.Timer" />.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        protected void Dispose(bool disposing)
        {
            this.Close();
            this.disposed = true;
        }
    }
}
