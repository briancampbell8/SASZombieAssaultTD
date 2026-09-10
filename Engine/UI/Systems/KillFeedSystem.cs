// ====================================================================================================
//  FILE: KillFeedSystem.cs
//  PATH: ./Engine/UI/Systems/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide Update() behavior for the UI subsystem.
//      - Provide Render() behavior for the UI subsystem.
//      - Provide GetStatistics() behavior for the UI subsystem.
//      - Provide ClearFeed() behavior for the UI subsystem.
//      - Provide Shutdown() behavior for the UI subsystem.
//      - Provide ToString() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Systems;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using DeathType = SASZombieAssaultTD.Engine.GameRoot.GamePlay.DeathType;

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
        public System.Numerics.Vector3 Position { get; set; }

        public KillAttributedEvent(
            string killerName,
            string victimName,
            int scoreAwarded,
            DeathType deathType,
            System.Numerics.Vector3 position)
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
        private readonly ECSRuntimeEvents _eventBus;
        private readonly Queue<KillFeedEntry> _killEntries = new();
        private readonly bool _debugOutput = true;

        public System.Numerics.Vector3 Position { get; set; } = new(10, 100, 0);
        public int MaxEntries { get; set; } = 5;
        public float DisplayDuration { get; set; } = 5.0f;
        public string FontName { get; set; } = "Arial";
        public int FontSize { get; set; } = 16;
        public Color KillerColor { get; set; } = Color.LightGreen;
        public Color VictimColor { get; set; } = Color.LightCoral;
        public Color SeparatorColor { get; set; } = Color.White;
        public float EntrySpacing { get; set; } = 20.0f;
        public string SeparatorText { get; set; } = "killed";

        public KillFeedSystem(ECSRuntimeEvents eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            SubscribeToEvents();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "KillFeedSystem initialized.");
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<KillAttributedEvent>(OnKillAttributed);
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Subscribed to KillAttributedEvent.");
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

            DLogger.Log($"{killEvent.KillerName} killed {killEvent.VictimName} ({killEvent.DeathType}).");
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

        public void Render(D3D11Adapter_Core context)
        {
            if (context == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "Render failed: Null render context.");
                return;
            }

            float yPosition = Position.Y;
            int index = 0;

            foreach (var entry in _killEntries)
            {
                RenderKillEntry(entry, yPosition + (index++ * EntrySpacing), context);
            }
        }

        private void RenderKillEntry(KillFeedEntry entry, float yPosition, D3D11Adapter_Core context)
        {
            float alpha = System.Math.Clamp(entry.TimeRemaining / DisplayDuration, 0f, 1f);

            float currentX = Position.X;
            currentX = RenderText(context, entry.KillerName, currentX, yPosition, GetColorWithAlpha(KillerColor, alpha));
            currentX = RenderText(context, $" {SeparatorText} ", currentX, yPosition, GetColorWithAlpha(SeparatorColor, alpha));
            RenderText(context, entry.VictimName, currentX, yPosition, GetColorWithAlpha(VictimColor, alpha));

            RenderDeathTypeIcon(entry.DeathType, currentX, yPosition, alpha, context);
        }

        private float RenderText(D3D11Adapter_Core context, string text, float x, float y, Color color)
        {
            if (string.IsNullOrEmpty(text)) return x;

            context.DrawText(text, (int)x, y, FontSize, color);
            return x + context.MeasureText(text, FontSize).X + 5f;
        }

        private void RenderDeathTypeIcon(DeathType deathType, float x, float y, float alpha, D3D11Adapter_Core context)
        {
            string iconText = GetDeathTypeIcon(deathType);
            var iconColor = GetColorWithAlpha(GetDeathTypeColor(deathType), alpha);

            context.DrawText(iconText, x, y, FontSize, iconColor);
        }

        private static Color GetColorWithAlpha(Color color, float alpha) =>
            new Color(
                color.R,
                color.G,
                color.B,
                (byte)(System.Math.Clamp(alpha, 0f, 1f) * color.A));

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
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Feed cleared.");
        }

        public void Shutdown()
        {
            _eventBus.Unsubscribe<KillAttributedEvent>(OnKillAttributed);
            _killEntries.Clear();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "Shutdown complete.");
        }

        private void Log(string message)
        {
            if (_debugOutput)
            {
                DLogger.Log($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
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
        public System.Numerics.Vector3 Position { get; set; }

        public override string ToString() =>
            $"Kill Feed Statistics - Entries: {CurrentEntries}/{MaxEntries}, " +
            $"Duration: {DisplayDuration}s, Position: ({Position.X}, {Position.Y})";
    }
}
