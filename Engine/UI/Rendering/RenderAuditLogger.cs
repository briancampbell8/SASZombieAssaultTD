/*
//File: RenderAuditLogger.cs
//Purpose: Deterministic audit logging for rendering operations.
//Records every rendering decision for debugging, replay, and verification.
//Supports frame capture and deterministic replay for audit-friendly behavior.

//Architecture:
//-Logs every draw call with full parameter state
//- Captures render state changes for deterministic replay
//- Supports frame capture to file for offline analysis
//- Provides real-time statistics for performance monitoring

//Usage:
//var audit = new RenderAuditLogger();
//audit.BeginFrame(123);
//audit.LogDrawCall("DrawTexture", texture, rect, color);
//audit.EndFrame();
//audit.SaveFrameCapture("frame_123.json");
//

*/

//using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Drawing; //For Color struct, replace with engine's color type if different

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    ///<summary>
    ///Provides deterministic audit logging for all rendering operations.
    ///Enables frame capture, replay debugging, and deterministic verification.
    ///</summary>
    public class RenderAuditLogger
    {
        private readonly object _lock = new();
        private readonly List<AuditEntry> _currentFrame = new();
        private readonly Queue<List<AuditEntry>> _frameHistory = new();

        ///<summary>
        ///Maximum number of frames to keep in history buffer.
        ///</summary>
        private const int MAX_FRAME_HISTORY = 60;

        ///<summary>
        ///Current frame number being recorded.
        ///</summary>
        private long _currentFrameNumber = 0;

        ///<summary>
        ///Flag indicating whether audit logging is enabled.
        ///</summary>
        private bool _isEnabled = true;

        ///<summary>
        ///Gets or sets whether audit logging is enabled.
        ///</summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        ///<summary>
        ///Begins recording a new frame.
        ///</summary>
        ///<param name="frameNumber">Frame number for identification.</param>
        public void BeginFrame(long frameNumber)
        {
            if (!_isEnabled) return;

            lock (_lock)
            {
                //Save previous frame to history
                if (_currentFrame.Count > 0)
                {
                    _frameHistory.Enqueue(new List<AuditEntry>(_currentFrame));
                    if (_frameHistory.Count > MAX_FRAME_HISTORY)
                        _frameHistory.Dequeue();
                }

                _currentFrameNumber = frameNumber;
                _currentFrame.Clear();

                LogEvent("FrameBegin", new Dictionary<string, object>
                {
                    { "FrameNumber", frameNumber },
                    { "Timestamp", DateTime.UtcNow.ToString("O") }
                });
            }
        }

        ///<summary>
        ///Ends recording the current frame.
        ///</summary>
        public void EndFrame()
        {
            if (!_isEnabled) return;

            lock (_lock)
            {
                LogEvent("FrameEnd", new Dictionary<string, object>
                {
                    { "FrameNumber", _currentFrameNumber },
                    { "EntryCount", _currentFrame.Count }
                });

                System.Diagnostics.Debug.WriteLine("Debug", $"RenderAuditLogger: Frame {_currentFrameNumber} recorded with {_currentFrame.Count} entries");
            }
        }

        ///<summary>
        ///Logs a draw texture operation.
        ///</summary>
        public void LogDrawTexture(ITexture2D texture, Rectangle destRect, Color color, string source = null)
        {
            if (!_isEnabled) return;

            LogEvent("DrawTexture", new Dictionary<string, object>
            {
                { "TexturePath", texture?.ToString() ?? "null" },
                { "DestRect", new { X = destRect.X, Y = destRect.Y, Width = destRect.Width, Height = destRect.Height } },
                { "Color", new { R = color.R, G = color.G, B = color.B, A = color.A } },
                { "Source", source ?? "unknown" }
            });
        }

        ///<summary>
        ///Logs a draw rectangle operation.
        ///</summary>
        public void LogDrawRect(Rectangle rect, Color color, string source = null)
        {
            if (!_isEnabled) return;

            LogEvent("DrawRect", new Dictionary<string, object>
            {
                { "Rect", new { X = rect.X, Y = rect.Y, Width = rect.Width, Height = rect.Height } },
                { "Color", new { R = color.R, G = color.G, B = color.B, A = color.A } },
                { "Source", source ?? "unknown" }
            });
        }

        ///<summary>
        ///Logs a draw text operation.
        ///</summary>
        public void LogDrawText(string text, Vector2 position, Color color, string source = null)
        {
            if (!_isEnabled) return;

            LogEvent("DrawText", new Dictionary<string, object>
            {
                { "Text", text },
                { "Position", new { X = position.X, Y = position.Y } },
                { "Color", new { R = color.R, G = color.G, B = color.B, A = color.A } },
                { "Source", source ?? "unknown" }
            });
        }

        ///<summary>
        ///Logs a state change operation.
        ///</summary>
        public void LogStateChange(string stateType, string fromValue, string toValue, string source = null)
        {
            if (!_isEnabled) return;

            LogEvent("StateChange", new Dictionary<string, object>
            {
                { "StateType", stateType },
                { "FromValue", fromValue },
                { "ToValue", toValue },
                { "Source", source ?? "unknown" }
            });
        }

        ///<summary>
        ///Logs a render command submission.
        ///</summary>
        public void LogRenderCommand(RenderCommand command, string source = null)
        {
            if (!_isEnabled) return;

            LogEvent("RenderCommand", new Dictionary<string, object>
            {
                { "CommandType", command.Type.ToString() },
                { "ElementId", command.Element?.ToString() ?? "null" },
                { "MaterialHash", command.Material?.GetHashCode() ?? 0 },
                { "SortKey", command.SortKey },
                { "Source", source ?? "unknown" }
            });
        }

        ///<summary>
        ///Saves the current frame capture to a JSON file.
        ///</summary>
        ///<param name="filePath">Path to save the capture file.</param>
        public void SaveFrameCapture(string filePath)
        {
            lock (_lock)
            {
                var capture = new FrameCapture
                {
                    FrameNumber = _currentFrameNumber,
                    Timestamp = DateTime.UtcNow,
                    Entries = new List<AuditEntry>(_currentFrame)
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(capture, options);
                File.WriteAllText(filePath, json);

                System.Diagnostics.Debug.WriteLine("Info", $"RenderAuditLogger: Frame capture saved to {filePath}");
            }
        }

        ///<summary>
        ///Gets the current frame entries for inspection.
        ///</summary>
        public IReadOnlyList<AuditEntry> GetCurrentFrame()
        {
            lock (_lock)
            {
                return new List<AuditEntry>(_currentFrame).AsReadOnly();
            }
        }

        ///<summary>
        ///Gets audit statistics.
        ///</summary>
        public (long CurrentFrame, int CurrentEntryCount, int FrameHistoryCount) GetStats()
        {
            lock (_lock)
            {
                return (_currentFrameNumber, _currentFrame.Count, _frameHistory.Count);
            }
        }

        ///<summary>
        ///Clears all recorded frames and resets state.
        ///</summary>
        public void Clear()
        {
            lock (_lock)
            {
                _currentFrame.Clear();
                _frameHistory.Clear();
                _currentFrameNumber = 0;
                System.Diagnostics.Debug.WriteLine("Info", "RenderAuditLogger: All records cleared");
            }
        }

        ///<summary>
        ///Internal method to log an audit event.
        ///</summary>
        private void LogEvent(string eventType, Dictionary<string, object> data)
        {
            var entry = new AuditEntry
            {
                Sequence = _currentFrame.Count,
                Timestamp = DateTime.UtcNow,
                EventType = eventType,
                Data = data
            };

            _currentFrame.Add(entry);
        }
    }

    ///<summary>
    ///Represents a single audit entry.
    ///</summary>
    public class AuditEntry
    {
        public int Sequence { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    ///<summary>
    ///Represents a complete frame capture for serialization.
    ///</summary>
    public class FrameCapture
    {
        public long FrameNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public List<AuditEntry> Entries { get; set; }
    }
}
