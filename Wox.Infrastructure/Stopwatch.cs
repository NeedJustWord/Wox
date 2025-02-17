﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Wox.Infrastructure.Logger;

namespace Wox.Infrastructure
{
    public static class Stopwatch
    {
        private static readonly Dictionary<string, long> Count = new Dictionary<string, long>();
        private static readonly object Locker = new object();

        /// <summary>
        /// This stopwatch will appear only in Debug mode
        /// </summary>
        public static long StopWatchDebug(this NLog.Logger logger, string message, Action action, [CallerMemberName] string methodName = "")
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();
            var milliseconds = stopWatch.ElapsedMilliseconds;
            string info = $"{message} <{milliseconds}ms>";
            logger.WoxDebug(info, methodName);
            return milliseconds;
        }

        public static long StopWatchNormal(this NLog.Logger logger, string message, Action action, [CallerMemberName] string methodName = "")
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();
            var milliseconds = stopWatch.ElapsedMilliseconds;
            string info = $"{message} <{milliseconds}ms>";
            logger.WoxInfo(info, methodName);
            return milliseconds;
        }

        public static void StartCount(string name, Action action)
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();
            var milliseconds = stopWatch.ElapsedMilliseconds;
            lock (Locker)
            {
                if (Count.TryGetValue(name, out var value))
                {
                    Count[name] = value + milliseconds;
                }
                else
                {
                    Count[name] = 0;
                }
            }
        }

        public static void EndCount()
        {
            foreach (var item in Count)
            {
                var info = $"{item.Key} already cost {item.Value}ms";
                //Logger.WoxDebug(info);
            }
        }
    }
}
