// ====================================================================================================
//  FILE: DiagnosticReportUI.cs
//  PATH: Engine/Diagnostics/UI/
//  MODULE: Diagnostics Pipeline (UI Rendering Stage)
//
//  ROLE:
//      Renders Markdown produced by Writer → MarkdownLogWriter.
//      Provides a deterministic, non-blocking diagnostics viewer.
//
//  CONTRACT:
//      - Must NEVER throw.
//      - Must NEVER block engine execution.
//      - Must NEVER reformat Markdown.
//      - Must ALWAYS display exactly what MarkdownLogWriter provides.
//      - Must NOT depend on DiagnosticEntry or JSON.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Windows.Forms;

namespace SASZombieAssaultTD.Engine.Diagnostics.UI
{
    public partial class DiagnosticReportUI : UserControl
    {
        public DiagnosticReportUI() =>
            //    InitializeComponent(); Already handled by SetupViewer() to avoid designer issues.
            SetupViewer();

        private void SetupViewer()
        {
            try
            {
                // A simple multiline textbox for Markdown display.
                // You can replace this with a Markdown renderer later.
                _viewer = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Dock = DockStyle.Fill,
                    BackColor = System.Drawing.Color.White,
                    ForeColor = System.Drawing.Color.Black,
                    Font = new System.Drawing.Font("Consolas", 10)
                };

                Controls.Add(_viewer);
            }
            catch
            {
                // Must never throw.
            }
        }

        private TextBox _viewer;

        /// <summary>
        /// Accepts Markdown from MarkdownLogWriter and displays it.
        /// </summary>
        public void DisplayMarkdown(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return;

            try
            {
                // Append exactly what Writer produced.
                _viewer.AppendText(markdown + Environment.NewLine + Environment.NewLine);
            }
            catch
            {
                // Must never throw.
            }
        }
    }
}
