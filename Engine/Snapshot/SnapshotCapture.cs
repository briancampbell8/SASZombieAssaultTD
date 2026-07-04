//============================================================================
//File: SnapshotCapture.cs
//Program: SnapshotCapture
//Subsystem: Snapshot System / Daily Build Documentation
//
//Purpose:
//    Captures framebuffer screenshots, manages today.png, finalizes daily
//    snapshots with timestamps, and provides snapshot metadata.
//
//Diagnostics:
//    - All logging via DLogger.Log()
//    - Deterministic, grep‑friendly trace naming
//    - No System.Diagnostics
//============================================================================

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
//

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Snapshot
{
    public static class SnapshotCapture
    {
        private static readonly string SnapshotDirectory =
            Path.Combine(Environment.CurrentDirectory, "Snapshots");

        private static readonly string SnapshotFile = "today.png";
        private static readonly string SnapshotPath =
            Path.Combine(SnapshotDirectory, SnapshotFile);

        //=====================================================================
        //SCREENSHOT CAPTURE (WINDOW)
        //=====================================================================

        public static void CaptureScreenshot()
        {
            DLogger.Log("SnapshotCapture.CaptureScreenshot.Start", SnapshotPath);

            try
            {
                Directory.CreateDirectory(SnapshotDirectory);

                using var bitmap = CaptureWindow();
                if (bitmap == null)
                {
                    DLogger.Log("SnapshotCapture.CaptureScreenshot.NullBitmap",
                        "Real framebuffer capture not implemented");
                    return;
                }

                bitmap.Save(SnapshotPath, ImageFormat.Png);

                var info = new FileInfo(SnapshotPath);
                DLogger.Log("SnapshotCapture.CaptureScreenshot.Complete",
                    $"Size={info.Length}, Created={info.CreationTime:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                DLogger.Log("SnapshotCapture.CaptureScreenshot.Error", ex.Message);
            }
        }

        //=====================================================================
        //SCREENSHOT CAPTURE (FRAMEBUFFER)
        //=====================================================================

        public static void CaptureFromFramebuffer(byte[] pixels, int width, int height)
        {
            if (pixels == null || pixels.Length == 0)
            {
                DLogger.Log("SnapshotCapture.CaptureFromFramebuffer.Error",
                    "Pixel buffer is null or empty");
                return;
            }

            DLogger.Log("SnapshotCapture.CaptureFromFramebuffer.Start",
                $"{width}x{height}, Bytes={pixels.Length}");

            try
            {
                Directory.CreateDirectory(SnapshotDirectory);

                using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                var bmpData = bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
                bitmap.UnlockBits(bmpData);

                bitmap.Save(SnapshotPath, ImageFormat.Png);

                var info = new FileInfo(SnapshotPath);
                DLogger.Log("SnapshotCapture.CaptureFromFramebuffer.Complete",
                    $"Size={info.Length}, Created={info.CreationTime:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                DLogger.Log("SnapshotCapture.CaptureFromFramebuffer.Error", ex.Message);
            }
        }

        //=====================================================================
        //FINALIZE DAILY SNAPSHOT
        //=====================================================================

        public static void FinalizeDailySnapshot()
        {
            DLogger.Log("SnapshotCapture.FinalizeDailySnapshot.Start", SnapshotPath);

            if (!File.Exists(SnapshotPath))
            {
                DLogger.Log("SnapshotCapture.FinalizeDailySnapshot.Missing",
                    "today.png does not exist");
                return;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string finalName = $"today_{timestamp}.png";
                string finalPath = Path.Combine(SnapshotDirectory, finalName);

                File.Move(SnapshotPath, finalPath);

                var info = new FileInfo(finalPath);
                DLogger.Log("SnapshotCapture.FinalizeDailySnapshot.Complete",
                    $"Final={finalName}, Size={info.Length}");
            }
            catch (Exception ex)
            {
                DLogger.Log("SnapshotCapture.FinalizeDailySnapshot.Error", ex.Message);
            }
        }

        //=====================================================================
        //SNAPSHOT INFO
        //=====================================================================

        public static SnapshotFileInfo GetSnapshotInfo()
        {
            if (!File.Exists(SnapshotPath))
            {
                return new SnapshotFileInfo
                {
                    Exists = false,
                    Path = SnapshotPath
                };
            }

            var info = new FileInfo(SnapshotPath);

            return new SnapshotFileInfo
            {
                Exists = true,
                Path = SnapshotPath,
                SizeBytes = info.Length,
                CreatedTime = info.CreationTime,
                ModifiedTime = info.LastWriteTime
            };
        }

        //=====================================================================
        //FINALIZED SNAPSHOT LISTING
        //=====================================================================

        public static string[] ListFinalizedSnapshots()
        {
            if (!Directory.Exists(SnapshotDirectory))
                return Array.Empty<string>();

            var files = Directory.GetFiles(SnapshotDirectory, "today_*.png");
            Array.Sort(files, (a, b) => b.CompareTo(a)); //newest first
            return files;
        }

        //=====================================================================
        //CLEANUP
        //=====================================================================

        public static void CleanupOldSnapshots(int keepCount = 30)
        {
            var files = ListFinalizedSnapshots();
            if (files.Length <= keepCount)
                return;

            for (int i = keepCount; i < files.Length; i++)
            {
                File.Delete(files[i]);
                DLogger.Log("SnapshotCapture.CleanupOldSnapshots.Delete",
                    Path.GetFileName(files[i]));
            }
        }

        //=====================================================================
        //COMMANDS
        //=====================================================================

        public static void FinalizeCommand(string[] args)
        {
            DLogger.Log("SnapshotCapture.FinalizeCommand", "Manual trigger");
            FinalizeDailySnapshot();
        }

        public static void StatusCommand(string[] args)
        {
            var info = GetSnapshotInfo();

            if (info.Exists)
            {
                DLogger.Log("SnapshotCapture.Status.Exists",
                    $"Size={info.SizeBytes}, Created={info.CreatedTime}, Modified={info.ModifiedTime}");
            }
            else
            {
                DLogger.Log("SnapshotCapture.Status.None", "No current snapshot");
            }

            var finalized = ListFinalizedSnapshots();
            DLogger.Log("SnapshotCapture.Status.FinalizedCount", finalized.Length.ToString());
        }

        //=====================================================================
        //PLACEHOLDER WINDOW CAPTURE
        //=====================================================================

        private static Bitmap CaptureWindow()
        {
            DLogger.Log("SnapshotCapture.CaptureWindow.Warning",
                "Real framebuffer capture not implemented");
            return null;
        }
    }

    //========================================================================
    //SNAPSHOT FILE INFO STRUCT
    //========================================================================

    public sealed class SnapshotFileInfo
    {
        public bool Exists { get; set; }
        public string Path { get; set; }
        public long SizeBytes { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string Error { get; set; }
    }
}
