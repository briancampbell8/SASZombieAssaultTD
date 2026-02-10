/*
    File:    Program.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Application entry point that constructs and starts the engine root.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
using SASZombieAssaultTD.Engine.Core;
using System;

namespace SASZombieAssaultTD
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var root = new GameRoot();
            root.Initialize();
        }
    }
}