using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Gameplay;
using DeathType = SASZombieAssaultTD.Engine.Gameplay.DeathType;
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Kill attributed event for tracking kills.
    /// </summary>
    public class KillAttributedEvent
    {
        public string KillerName { get; set; }
        public string VictimName { get; set; }
        public int ScoreAwarded { get; set; }
        public DeathType DeathType { get; set; }
        public Vector3 Position { get; set; }

        public KillAttributedEvent(string killerName, string victimName, int scoreAwarded, DeathType deathType, Vector3 position)
        {
            KillerName = killerName;
            VictimName = victimName;
            ScoreAwarded = scoreAwarded;
            DeathType = deathType;
            Position = position;
        }
    }

    /// <summary>
    /// Displays recent kills in a scrolling/fading list with death type information.
    /// </summary>
    public class KillFeedSystem
    {
        private readonly EventRouter _eventBus;
        private readonly Queue<KillFeedEntry> _killEntries = new();
        private readonly bool _debugOutput = true;

        public Vector3 Position { get; set; } = new(10, 100, 0);
        public int MaxEntries { get; set; } = 5;
        public float DisplayDuration { get; set; } = 5.0f;
        public string FontName { get; set; } = "Arial";
        public int FontSize { get; set; } = 16;
        public Color KillerColor { get; set; } = Color.LightGreen;
        public Color VictimColor { get; set; } = Color.LightCoral;
        public Color SeparatorColor { get; set; } = Color.White;
        public float EntrySpacing { get; set; } = 20.0f;
        public string SeparatorText { get; set; } = "killed";

        public KillFeedSystem(EventRouter eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            SubscribeToEvents();
            DebugLog("KillFeedSystem initialized.");
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<KillAttributedEvent>(OnKillAttributed);
            DebugLog("Subscribed to KillAttributedEvent.");
        }

        private void OnKillAttributed(KillAttributedEvent killEvent)
        {
            AddKillEntry(new KillFeedEntry
            {
                KillerName = killEvent.KillerName,
                VictimName = killEvent.VictimName,
                DeathType = killEvent.DeathType,
                TimeRemaining = DisplayDuration
            });

            DebugLog($"Added kill entry: {killEvent.KillerName} killed {killEvent.VictimName} ({killEvent.DeathType}).");
        }

        private void AddKillEntry(KillFeedEntry entry)
        {
            _killEntries.Enqueue(entry);

            if (_killEntries.Count > MaxEntries)
            {
                _killEntries.Dequeue();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var entry in _killEntries)
            {
                entry.TimeRemaining -= deltaTime;
            }

            while (_killEntries.Count > 0 && _killEntries.Peek().TimeRemaining <= 0f)
            {
                _killEntries.Dequeue();
            }
        }

        public void Render(IRenderContext context)
        {
            if (context == null)
            {
                DebugLog("Render failed: Null render context.");
                return;
            }

            float yPosition = Position.Y;
            int index = 0;

            foreach (var entry in _killEntries)
            {
                RenderKillEntry(entry, yPosition + (index++ * EntrySpacing), context);
            }
        }

        private void RenderKillEntry(KillFeedEntry entry, float yPosition, IRenderContext context)
        {
            float alpha = System.Math.Clamp(entry.TimeRemaining / DisplayDuration, 0f, 1f);

            float currentX = Position.X;
            currentX = RenderText(context, entry.KillerName, currentX, yPosition, GetColorWithAlpha(KillerColor, alpha));
            currentX = RenderText(context, $" {SeparatorText} ", currentX, yPosition, GetColorWithAlpha(SeparatorColor, alpha));
            RenderText(context, entry.VictimName, currentX, yPosition, GetColorWithAlpha(VictimColor, alpha));

            RenderDeathTypeIcon(entry.DeathType, currentX, yPosition, alpha, context);
        }

        private float RenderText(IRenderContext context, string text, float x, float y, Color color)
        {
            if (string.IsNullOrEmpty(text)) return x;

            context.DrawText(text, x, y, FontSize, color);
            return x + context.MeasureText(text, FontSize).X + 5f;
        }

        private void RenderDeathTypeIcon(DeathType deathType, float x, float y, float alpha, IRenderContext context)
        {
            string iconText = GetDeathTypeIcon(deathType);
            var iconColor = GetColorWithAlpha(GetDeathTypeColor(deathType), alpha);

            context.DrawText(iconText, x, y, FontSize, iconColor);
        }

        private static Color GetColorWithAlpha(Color color, float alpha) =>
            new Color(color.R, color.G, color.B, System.Math.Clamp(alpha, 0f, 1f) * color.A);

        private static string GetDeathTypeIcon(DeathType deathType) => deathType switch
        {
            DeathType.Bullet => "🔫",
            DeathType.Explosion => "💥",
            DeathType.Fire => "🔥",
            DeathType.Melee => "⚔️",
            DeathType.Poison => "☠️",
            DeathType.Electric => "⚡",
            DeathType.Fall => "📍",
            DeathType.Drowning => "💧",
            DeathType.Freeze => "❄️",
            DeathType.Other => "💀",
            _ => "?"
        };

        private static Color GetDeathTypeColor(DeathType deathType) => deathType switch
        {
            DeathType.Bullet => Color.Yellow,
            DeathType.Explosion => Color.Orange,
            DeathType.Fire => Color.Red,
            DeathType.Melee => Color.Gray,
            DeathType.Poison => Color.Purple,
            DeathType.Electric => Color.Cyan,
            DeathType.Fall => Color.Brown,
            DeathType.Drowning => Color.Blue,
            DeathType.Freeze => Color.LightBlue,
            DeathType.Other => Color.DarkGray,
            _ => Color.White
        };

        public KillFeedStatistics GetStatistics() => new()
        {
            CurrentEntries = _killEntries.Count,
            MaxEntries = MaxEntries,
            DisplayDuration = DisplayDuration,
            Position = Position
        };

        public void ClearFeed()
        {
            _killEntries.Clear();
            DebugLog("Feed cleared.");
        }

        public void Shutdown()
        {
            _eventBus.Unsubscribe<KillAttributedEvent>(OnKillAttributed);
            _killEntries.Clear();
            DebugLog("Shutdown complete.");
        }

        private void DebugLog(string message)
        {
            if (_debugOutput)
            {
                System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            }
        }
    }

    internal class KillFeedEntry
    {
        public string KillerName { get; set; } = string.Empty;
        public string VictimName { get; set; } = string.Empty;
        public DeathType DeathType { get; set; }
        public float TimeRemaining { get; set; }
    }

    public class KillFeedStatistics
    {
        public int CurrentEntries { get; set; }
        public int MaxEntries { get; set; }
        public float DisplayDuration { get; set; }
        public Vector3 Position { get; set; }

        public override string ToString() =>
            $"Kill Feed Statistics - Entries: {CurrentEntries}/{MaxEntries}, Duration: {DisplayDuration}s, Position: ({Position.X}, {Position.Y})";
    }
}
