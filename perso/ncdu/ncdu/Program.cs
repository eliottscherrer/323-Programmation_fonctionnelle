using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace ncdu
{
    public class Program
    {
        const int MAX_HASHTAGS = 10;

        static void Main()
        {
            Console.CursorVisible = false;

            string rootPath = @"C:\Users\";
            DirectoryInfo root = new DirectoryInfo(rootPath);

            var entriesTask = Task.Run(() => GetEntries(root));

            ShowLoadingWhile(entriesTask);

            List<Entry> entries = entriesTask.Result;

            List<Entry> sortedEntries = entries.OrderByDescending(e => e.Size).ToList();

            long totalSize = sortedEntries.Sum(entry => entry.Size);

            Console.WriteLine($"--- {FormatSize(totalSize)} --- {root.FullName} ------------------------------");
            Console.WriteLine();

            sortedEntries.ForEach(entry => {
                double percentage = (double)entry.Size / totalSize;
                int barLength = (int)(percentage * MAX_HASHTAGS);
                string bar = new string('#', barLength);
                Console.WriteLine($"\t{FormatSize(entry.Size), 10} [{bar, -10}] {entry.Name}");
            });

            Console.ReadLine();
        }

        private static void ShowLoadingWhile(Task task)
        {
            try
            {
                int boxWidth = Math.Max(30, Math.Min(60, Console.WindowWidth - 4));
                int centerX = Math.Max(0, (Console.WindowWidth - boxWidth) / 2);
                int centerY = Math.Max(0, Console.WindowHeight / 2);

                string[] spinner = new[] { "-", "\\", "|", "/" };
                char blockChar = '█';
                int spin = 0;
                int tick = 0;

                int origLeft = Console.CursorLeft;
                int origTop = Console.CursorTop;

                while (!task.IsCompleted)
                {
                    Console.SetCursorPosition(centerX, Math.Max(0, centerY - 1));
                    string msg = $"Calcul des tailles... {spinner[spin % spinner.Length]}";
                    Console.Write(msg.PadRight(boxWidth));

                    Console.SetCursorPosition(centerX, centerY);
                    Console.Write("[");
                    int innerWidth = boxWidth - 2;
                    Console.Write(new string(' ', innerWidth));
                    Console.Write("]");

                    int pos = tick % innerWidth;
                    Console.SetCursorPosition(centerX + 1 + pos, centerY);
                    Console.Write(blockChar);

                    spin++;
                    tick = (tick + 1) % Math.Max(1, innerWidth);

                    Thread.Sleep(80);
                }

                Console.SetCursorPosition(centerX, Math.Max(0, centerY - 1));
                Console.Write(new string(' ', boxWidth));
                Console.SetCursorPosition(centerX, centerY);
                Console.Write(new string(' ', boxWidth));

                try { Console.SetCursorPosition(origLeft, origTop); } catch { }
            }
            catch
            {
                task.Wait();
            }
        }

        public static List<Entry> GetEntries(DirectoryInfo dir)
        {
            FileInfo[] files = new FileInfo[] { };
            try { files = dir.GetFiles(); } catch { }

            DirectoryInfo[] subDirs = new DirectoryInfo[] { };
            try { subDirs = dir.GetDirectories(); } catch { }

            IEnumerable<Entry> fileEntries = files.Select(fi => new Entry
            {
                Name = fi.Name,
                Size = fi.Length
            });

            IEnumerable<Entry> dirEntries = subDirs.Select(di => {
                long size = 0;
                try { size = GetDirectorySize(di); } catch { }
                return new Entry
                {
                    Name = Path.DirectorySeparatorChar + di.Name,
                    Size = size
                };
            });

            return fileEntries.Concat(dirEntries).ToList();
        }

        public static long GetDirectorySize(DirectoryInfo dir)
        {
            long size = 0;
            FileInfo[] files = new FileInfo[] { };
            try
            {
                files = dir.GetFiles();
            } catch { }

            foreach (FileInfo fi in files)
                size += fi.Length;

            DirectoryInfo[] subDirs = new DirectoryInfo[] { };
            try
            {
                subDirs = dir.GetDirectories();
            }
            catch { }

            foreach (DirectoryInfo di in subDirs)
                size += GetDirectorySize(di);

            return size;
        }

        public static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KiB", "MiB", "GiB", "TiB" };
            double len = bytes;
            int order = 0;

            // 1024 because "ibi" sizes
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
