/* ====================================================================================================
 *  FILE: DiagnosticsMonitor.cs
 *  PATH: Engine/Diagnostics/DiagnosticsMonitor.cs
 *  SUBSYSTEM: Diagnostics
 *  ROLE: Centralized monitoring component for engine health state and log-derived metrics.
 *
 *  RESPONSIBILITIES:
 *      - Track log volume and compute logs-per-second metrics over a fixed interval.
 *      - Detect repeated identical error messages and count duplicate error sequences.
 *      - Maintain and expose a latched emergency-stop state derived from monitoring conditions.
 *      - Provide monitoring statistics for health checks and supervisory components.
 *      - Integrate with DebugLogger by consuming log events via a registration call.
 *
 *  NON-RESPONSIBILITIES:
 *      - Log formatting, routing, or sink management (handled by DebugLogger).
 *      - Markdown, trace, or external reporting output (handled by higher-level diagnostics).
 *      - Subsystem-specific recovery, restart, or shutdown policies.
 *      - Configuration management or runtime tuning of thresholds.
 *
 *  ARCHITECTURAL NOTES:
 *      - All monitoring state is stored internally and updated using atomic operations where required.
 *      - DebugLogger must call RegisterLogEvent(...) for every emitted log to keep metrics accurate.
 *      - Emergency-stop state is latched and remains active until ResetEmergencyStop() is called.
 *      - Thresholds are fixed constants and must not be modified at runtime to preserve determinism.
 *      - This component is read by health-check code but does not initiate shutdown on its own.
 * ==================================================================================================== */

using System;
using System.Diagnostics;
using System.Threading;

namespace SASZombieAssaultTD.Engine.Diagnostics   // ⭐ FIXED NAMESPACE
{
    public static class DiagnosticsMonitor
    {
        private static long _intervalLogCount = 0;
        private static long _logsPerSecond = 0;
        private static long _duplicateErrorCount = 0;
        private static string _lastErrorMessage = string.Empty;
        private static int _lastErrorRepeatCount = 0;
        private static volatile bool _emergencyStop = false;

        private static readonly Stopwatch _rateTimer = Stopwatch.StartNew();

        private const int LOG_RATE_THRESHOLD = 2000;
        private const int DUPLICATE_ERROR_THRESHOLD = 10;

        public static void RegisterLogEvent(DiagnosticCategory category, string message)
        {
            Interlocked.Increment(ref _intervalLogCount);

            if (_rateTimer.ElapsedMilliseconds >= 1000)
            {
                _logsPerSecond = Interlocked.Exchange(ref _intervalLogCount, 0);
                _rateTimer.Restart();

                if (_logsPerSecond > LOG_RATE_THRESHOLD)
                    TriggerEmergencyStop($"Log rate exceeded threshold: {_logsPerSecond}/sec");
            }

            if (category == DiagnosticCategory.Error || category == DiagnosticCategory.Exception)
            {
                if (message == _lastErrorMessage)
                {
                    _lastErrorRepeatCount++;

                    if (_lastErrorRepeatCount >= DUPLICATE_ERROR_THRESHOLD)
                    {
                        Interlocked.Increment(ref _duplicateErrorCount);
                        TriggerEmergencyStop($"Repeated identical error detected: {message}");
                    }
                }
                else
                {
                    _lastErrorMessage = message;
                    _lastErrorRepeatCount = 1;
                }
            }
        }

        public static (bool isEmergencyStop, long logsPerSecond, long duplicateErrors) GetMonitoringStats()
        {
            return (_emergencyStop, _logsPerSecond, _duplicateErrorCount);
        }

        public static void ResetEmergencyStop()
        {
            _emergencyStop = false;
            _duplicateErrorCount = 0;
            _lastErrorRepeatCount = 0;

            DebugLogger.Info("DiagnosticsMonitor", "Emergency stop reset");
        }

        public static bool IsHealthy() => !_emergencyStop;

        private static void TriggerEmergencyStop(string reason)
        {
            if (_emergencyStop)
                return;

            _emergencyStop = true;

            DebugLogger.Error("DiagnosticsMonitor", $"Emergency stop triggered: {reason}");
        }
    }
}
