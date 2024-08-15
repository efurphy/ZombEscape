using Autofac;
using MonoGame.Extended.Content.Tiled;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace ZombEscape
{
    internal class Timer
    {
        static readonly List<Timer> timers = new();

        long time;
        long? startTime;

        Action OnComplete;
        Action OnUpdate;

        public Timer(float seconds, Action onComplete=null, Action onUpdate=null)
        {
            time = (long)(seconds * 10_000_000);
            startTime = null;
            OnComplete = onComplete;
            OnUpdate = onUpdate;
        }

        public void Start()
        {
            this.startTime = DateTime.Now.Ticks;
            timers.Add(this);
        }

        public static void Update()
        {
            foreach (Timer timer in timers)
            {
                if (DateTime.Now.Ticks - timer.startTime > timer.time)
                {
                    timer.OnComplete?.Invoke();
                }
                else if (timer.OnUpdate != null)
                {
                    timer.OnUpdate();
                }
            }
        }

        public static Timer StartTimer(float seconds, Action onComplete = null, Action onUpdate = null)
        {
            var timer = new Timer(seconds, onComplete, onUpdate);
            timer.Start();
            return timer;
        }
    }
}
