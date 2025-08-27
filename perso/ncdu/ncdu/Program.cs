using System;
using System.IO;
using System.Collections.Generic;

namespace ncdu
{
    public class Program
    {
        const int MAX_HASHTAGS = 10;

        static void Main()
        {
            string rootPath = @"C:\Users\pg13qnw\Documents\GitHub\323-Programmation_fonctionnelle";
            DirectoryInfo root = new DirectoryInfo(rootPath);

            List<Entry> entries = GetEntries(root);

            long totalSize = 0;
            foreach (Entry entry in entries)
            {
                totalSize += entry.Size;
            }

            Console.WriteLine($"--- {FormatSize(totalSize)} --- {root.FullName} ------------------------------");
            Console.WriteLine();

            entries.Sort((Entry a, Entry b) => b.Size.CompareTo(a.Size));

            foreach (Entry entry in entries)
            {
                double percentage = (double)entry.Size / totalSize;
                int barLength = (int)(percentage * MAX_HASHTAGS);
                string bar = new string('#', barLength);
                Console.WriteLine($"\t{FormatSize(entry.Size), 10} [{bar, -10}] {entry.Name}");
            }

            Console.ReadLine();
        }

        public static List<Entry> GetEntries(DirectoryInfo dir)
        {
            List<Entry> list = new List<Entry>();

            // Files in directory
            FileInfo[] files = new FileInfo[] { };
            try { files = dir.GetFiles(); } catch { }

            foreach (FileInfo fi in files)
            {
                list.Add(new Entry
                {
                    Name = fi.Name,
                    Size = fi.Length
                });
            }

            // Sub directories
            DirectoryInfo[] subDirs = new DirectoryInfo[] { };
            try { subDirs = dir.GetDirectories(); } catch { }

            foreach (DirectoryInfo di in subDirs)
            {
                long size = 0;
                try { size = GetDirectorySize(di); } catch { }
                list.Add(new Entry
                {
                    Name = di.Name + Path.DirectorySeparatorChar,
                    Size = size
                });
            }

            return list;
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
