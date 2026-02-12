using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;

Thread newThread = new Thread(() =>
{
    string exePath = Environment.ProcessPath ?? AppContext.BaseDirectory;
    string exeDir = Path.GetDirectoryName(exePath) ?? AppDomain.CurrentDomain.BaseDirectory;
    string logPath = Path.Combine(exeDir, "log.txt");

    void Log(string message)
    {
        try { File.AppendAllLines(logPath, new[] { $"[{DateTime.Now:HH:mm:ss}] {message}" }); } catch { }
        Console.WriteLine(message);
    }

    try
    {
        Log("--- Start ---");
        
        if (Environment.GetCommandLineArgs().Length > 1)
        {
            string inputPath = Environment.GetCommandLineArgs()[1];
            if (File.Exists(inputPath) || Directory.Exists(inputPath))
            {
                Log($"Compression: {Path.GetFileName(inputPath)}");
                using var ms = new MemoryStream();
                
                using (var writer = WriterFactory.Open(ms, ArchiveType.Zip, new WriterOptions(CompressionType.LZMA)))
                {
                    if (Directory.Exists(inputPath))
                        writer.WriteAll(inputPath, "*", SearchOption.AllDirectories);
                    else
                        writer.Write(Path.GetFileName(inputPath), inputPath);
                }

                string b64 = Convert.ToBase64String(ms.ToArray());
                Clipboard.SetText(b64);
                Log("Success: Copied to clipboard.");
            }
        }
        else
        {
            string b64Data = Clipboard.GetText()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(b64Data)) {
                Log("Clipboard vuota.");
            }
            else {
                Log("Ripristino in corso...");
                byte[] bytes = Convert.FromBase64String(b64Data);
                using var ms = new MemoryStream(bytes);
                using (var archive = ZipArchive.Open(ms))
                {
                    foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
                    {
                        entry.WriteToDirectory(exeDir, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                        Log($"Extracted: {entry.Key}");
                    }
                }
                Log("Completed with success.");
            }
        }
    }
    catch (Exception ex)
    {
        Log($"ERROR: {ex.Message}");
    }

    Log("--- End ---\n");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
});

newThread.SetApartmentState(ApartmentState.STA);
newThread.Start();
newThread.Join();
