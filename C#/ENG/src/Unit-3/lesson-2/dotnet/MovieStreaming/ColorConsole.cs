using System;
using System.Collections.Generic;
using System.Text;

namespace MovieStreaming;

public static class ColorConsole
{
    public static void WriteLineGreen(string message) => WriteLineColor(message, ConsoleColor.Green);
    public static void WriteLineYellow(string message) => WriteLineColor(message, ConsoleColor.Yellow);

    static void WriteLineColor(string message, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = color;
        Console.WriteLine(message);

        Console.ForegroundColor = originalColor;
    }
}
