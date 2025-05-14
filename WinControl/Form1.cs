using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using System.Drawing.Imaging;
using Telegram.Bot.Types.InputFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;

namespace WinControl
{
    public partial class Form1 : Form
    {
        TelegramBotClient botClient;
        public string botToken;
        public string password;
        public string sharedPath1;

        private PerformanceCounter cpuCounter;
        private PerformanceCounter memCounter;
        private float cpuUsage;
        private float availableMemory;
        string mode;
        bool close = false;

        private Form2 _form2;
        

        public Form1(string botToken, string password, Form2 form2, string sharedPath)
        {
            InitializeComponent();
            this.tray.MouseDoubleClick += new MouseEventHandler(tray_MouseDoubleClick);
            label1.Text = "Hosting on: " + botToken;
            System.IO.File.WriteAllText("token.txt", botToken);
            System.IO.File.WriteAllText("password.txt", password);
            System.IO.File.WriteAllText("sharedPath.txt", sharedPath);
            mode = "def";
            tray.Visible = true;
            _form2 = form2;
            sharedPath1 = sharedPath;
        }

     

        private void UpdateLabel(string text)
        {
            if (label1.InvokeRequired)
            {
                label1.Invoke(new Action(() => label1.Text = text));
            }
            else
            {
                label1.Text = text;
            }
        }

        private void tray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (close == false)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                tray.BalloonTipText = "Program is hidden in the tray";
                tray.ShowBalloonTip(5000);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _form2.Show();
            _form2.Opacity = 100;
            _form2.WindowState = FormWindowState.Normal;
            close = true;
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            botClient = new TelegramBotClient(System.IO.File.ReadAllText("token.txt"));
            StartReceiver();
        }

        public async Task StartReceiver()
        {
            var token = new CancellationTokenSource();
            var canceltoken = token.Token;
            var ReOpt = new ReceiverOptions { AllowedUpdates = { } };
            await botClient.ReceiveAsync(OnMessage, ErrorMessage, ReOpt, canceltoken);
        }

        public async Task OnMessage(ITelegramBotClient botClient,Update update, CancellationToken cancellation)
        {
            if(update.Message is Telegram.Bot.Types.Message message)
            {
                SendMessage(message);
            }
            if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Document)
            {
                Document document = update.Message.Document;
                string fileId = document.FileId;
                Random random = new Random();
                int min = 100;
                int max = 99999;
                int randomNumber = random.Next(min, max);
                string fileName = randomNumber + document.FileName;
                long fileSize = (long)document.FileSize;  

                await DownloadFileAsync(botClient, fileId, fileName, cancellation, update);
            }
            if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Audio)
            {
                Audio document = update.Message.Audio;
                string fileId = document.FileId;
                Random random = new Random();
                int min = 100;
                int max = 99999;
                int randomNumber = random.Next(min, max);
                string fileName = randomNumber + document.FileName;
                long fileSize = (long)document.FileSize;  

                await DownloadFileAsync(botClient, fileId, fileName, cancellation, update);
            }
            if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Video)
            {
                Video document = update.Message.Video;
                string fileId = document.FileId;
                Random random = new Random();
                int min = 100;
                int max = 99999;
                int randomNumber = random.Next(min, max);
                string fileName = fileId + randomNumber + ".mp4"; 
                long fileSize = (long)document.FileSize;  

                await DownloadFileAsync(botClient, fileId, fileName, cancellation, update);
            }
            if (update.Type == UpdateType.Message && update.Message?.Type == MessageType.Photo)
            {
                PhotoSize[] document1 = update.Message.Photo;
                PhotoSize document = document1.Last();
                string fileId = document.FileId;
                Random random = new Random();
                int min = 100;
                int max = 99999;
                int randomNumber = random.Next(min, max);
                string fileName = fileId + randomNumber + ".png";
                long fileSize = (long)document.FileSize;

                await DownloadFileAsync(botClient, fileId, fileName, cancellation, update);
            }
        }

        public async Task ErrorMessage(ITelegramBotClient telegramBot, Exception e,CancellationToken cancellation)
        {
            if(e is ApiRequestException requestException)
            {
                await botClient.SendTextMessageAsync("", e.Message.ToString());
            }
        }

        public async Task SendMessage(Telegram.Bot.Types.Message message)
        {
            Process cmd1 = new Process();
            cmd1.StartInfo.FileName = "cmd.exe";
            cmd1.StartInfo.UseShellExecute = false;
            cmd1.StartInfo.RedirectStandardInput = true;
            cmd1.StartInfo.RedirectStandardOutput = true;
            cmd1.StartInfo.RedirectStandardError = true;
            cmd1.StartInfo.CreateNoWindow = false;
            cmd1.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            cmd1.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            if (System.IO.File.Exists(message.From.Id.ToString()+".txt"))
            {
                if (mode == "def")
                {
                    if (message.Text.ToLower() == "shutdown")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Shutdown...");
                        Process.Start("shutdown", "/s /t 0");
                    }
                    else if (message.Text.ToLower() == "reboot")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Reboot...");
                        Process.Start("shutdown", "/r /t 0");
                    }
                    else if (message.Text.ToLower() == "help")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "1. shutdown - Shutting down the computer\n 2. reboot - Restarting the computer\n  3. msgBox Your message here - Shows a message box on the screen\n   4. prtSc - Takes a screenshot of all the screens and sends\n    5. statistics - Shows information about RAM and processor\n     6. run Your command here - Executes the command you entered\n      7. close - Closes the program (May remain in the background)\n       8. kill - Closes the program completely without leaving it in the background\n        9. sendKeys You key or text here - Sends a text or keystroke (Read more here: https://learn.microsoft.co/office/vba/language/reference/user-interface-help/sendkeys-statement )\n         10. curPos Your value here (example: 400,200) - Sets the cursor position to the value you entered\n\n\nSend file, video, audio, photo and they will be saved automatically to the folder that you specified during setup");
                    }
                    else if (message.Text.ToLower() == "prtsc")
                    {
                        try
                        {
                            string filePath = "all_screens_screenshot.png";
                            CaptureAllScreens(filePath);
                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                var fileToSend = new InputOnlineFile(fileStream);
                                await botClient.SendPhotoAsync(
                                    chatId: message.Chat.Id,
                                    photo: fileToSend,
                                    caption: "",
                                    cancellationToken: CancellationToken.None
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            botClient.SendTextMessageAsync(message.Chat.Id, $"Error: {ex.Message}");
                        }
                    }
                    else if (message.Text.StartsWith("msgBox "))
                    {
                        string textToShow = message.Text.Substring("msgBox ".Length);
                        ShowMessageBox(textToShow, message.From.Id.ToString());
                        botClient.SendTextMessageAsync(message.Chat.Id, "Done");

                    }
                    else if (message.Text.ToLower() == "statistics")
                    {
                        memCounter = new PerformanceCounter("Memory", "Available MBytes");
                        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");  
                        cpuCounter.NextValue();
                        Thread.Sleep(1000);
                        availableMemory = memCounter.NextValue();
                        botClient.SendTextMessageAsync(message.Chat.Id, "CPU usage: " + cpuCounter.NextValue() + "% , Available RAM: " + availableMemory.ToString());
                    }
                    else if (message.Text.ToLower() == "processes")
                    {
                        Process[] processes = Process.GetProcesses();
                        foreach (Process process in processes)
                        {
                            try
                            {
                                botClient.SendTextMessageAsync(message.Chat.Id, $"\nID: {process.Id}, Name: {process.ProcessName}, Title: {process.MainWindowTitle}");
                            }
                            catch (Exception ex)
                            {
                                botClient.SendTextMessageAsync(message.Chat.Id, $"ID: {process.Id}, Name: {process.ProcessName}, Error when receiving the title: {ex.Message}");
                            }
                        }
                    }
                    else if (message.Text.StartsWith("run "))
                    {
                        string cmd = message.Text.Substring("run ".Length);
                        try
                        {
                            Process.Start(cmd);
                        }
                        catch (Exception ex)
                        {
                            botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
                        }
                    }
                    else if (message.Text.StartsWith("close "))
                    {
                        string process = message.Text.Substring("close ".Length);
                        CloseProcessWindow(process);
                        botClient.SendTextMessageAsync(message.Chat.Id, "Done");
                    }
                    else if (message.Text.StartsWith("kill "))
                    {
                        string process = message.Text.Substring("kill ".Length);
                        KillProcess(process);
                        botClient.SendTextMessageAsync(message.Chat.Id, "Done");
                    }
                    else if (message.Text.StartsWith("winget "))
                    {
                        string package = message.Text.Substring("winget ".Length);
                        ProcessStartInfo processInfo = new ProcessStartInfo("cmd", "/c winget " + package);
                        processInfo.Verb = "runas";
                        try
                        {
                            Process process = Process.Start(processInfo);
                        }
                        catch (Exception ex)
                        {
                            botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
                        }
                        botClient.SendTextMessageAsync(message.Chat.Id, "Done");
                    }
                    else if (message.Text.StartsWith("sendKeys "))
                    {
                        string text = message.Text.Substring("sendKeys ".Length);
                        SendKeys.SendWait(text);
                    }
                    else if (message.Text.StartsWith("curPos "))
                    {
                        string pos = message.Text.Substring("curPos ".Length);

                        string[] numbers = pos.Split(',');
                        if (int.TryParse(numbers[0], out int num1))
                        {

                        }
                        if (int.TryParse(numbers[1], out int num2))
                        {
                        }
                        Cursor.Position = new Point(num1, num2);
                    }
                    /*else if (message.Text.ToLower() == "cmd mode")
                    {
                        cmd1.Start();
                        
                        mode = "cmd";
                    }*/
                    /*else if (message.Text.ToLower() == "getDiskTree")
                    {
                        string filePath = "disk_tree.txt";

                        try
                        {
                            try
                            {
                                StringBuilder sb = new StringBuilder();
                                DriveInfo[] drives = DriveInfo.GetDrives();

                                Parallel.ForEach(drives, drive =>
                                {
                                    if (drive.IsReady)
                                    {
                                        AppendDriveInfo(drive, sb);
                                        AppendDirectory(drive.RootDirectory, sb, "  ");
                                    }
                                    else
                                    {
                                        lock (sb)
                                        {
                                            sb.AppendLine($"Диск {drive.Name} не готов.");
                                        }
                                    }
                                });

                                System.IO.File.WriteAllText(filePath, sb.ToString());
                                
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка: {ex.Message}");
                            }

                            botClient.SendDocumentAsync(message.Chat.Id, "disk_tree.txt");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при создании или записи файла: {ex.Message}");
                        }
                    }*/
                }
                else
                {
                    StreamWriter inputWriter = cmd1.StandardInput;
                    StreamReader outputReader = cmd1.StandardOutput;
                    StreamReader errorReader = cmd1.StandardError;
                    while (true)
                    {
                        Console.Write("cmd> ");
                        string command = message.Text;

                        if (string.IsNullOrEmpty(command))
                        {
                            continue;
                        }

                        if (command.ToLower() == "exit")
                        {
                            break;
                        }

                        inputWriter.WriteLine(command);
                        inputWriter.Flush();

                        StringBuilder output = new StringBuilder();
                        StringBuilder error = new StringBuilder();

                        while (!outputReader.EndOfStream || !errorReader.EndOfStream)
                        {
                            if (!outputReader.EndOfStream)
                            {
                                string line = outputReader.ReadLine();
                                Console.WriteLine(line);
                                output.AppendLine(line);
                            }

                            if (!errorReader.EndOfStream)
                            {
                                string line = errorReader.ReadLine();
                                Console.Error.WriteLine(line);
                                error.AppendLine(line);
                            }

                            
                            System.Threading.Thread.Sleep(10);
                        }

/*                        File.AppendAllText(outputFilePath, $"cmd> {command}\r\n", Encoding.UTF8);
                        File.AppendAllText(outputFilePath, output.ToString(), Encoding.UTF8);
                        if (error.Length > 0)
                        {
                            File.AppendAllText(outputFilePath, "Ошибки:\r\n", Encoding.UTF8);
                            File.AppendAllText(outputFilePath, error.ToString(), Encoding.UTF8);
                        }
                        File.AppendAllText(outputFilePath, "\r\n", Encoding.UTF8);*/


                    }
                }
            }
            else
            {
                if (message.Text.ToLower() == System.IO.File.ReadAllText("password.txt"))
                {
                    System.IO.File.WriteAllText(message.From.Id.ToString()+".txt", "true");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Access granted");
                } else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Please enter your password");
                }
            }
        }

        public class User32
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
            public const int WM_CLOSE = 0x0010;
        }

        public void CloseProcessWindow(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                try
                {
                    User32.PostMessage(process.MainWindowHandle, User32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    process.WaitForExit(5000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка закрытия окна: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка завершения процесса: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        static void ShowMessageBox(string text, string from)
        {
            Thread thread = new Thread(() =>
            {
                MessageBox.Show(text, "Message from: " + from, MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void CaptureAllScreens(string filePath)
        {
            try
            {
                int screenWidth = SystemInformation.VirtualScreen.Width;
                int screenHeight = SystemInformation.VirtualScreen.Height;
                int screenLeft = SystemInformation.VirtualScreen.Left;
                int screenTop = SystemInformation.VirtualScreen.Top;

                using (Bitmap screenshot = new Bitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
                {
                    using (Graphics gfx = Graphics.FromImage(screenshot))
                    {
                        gfx.CopyFromScreen(screenLeft, screenTop, 0, 0, new Size(screenWidth, screenHeight), CopyPixelOperation.SourceCopy);
                    }

                    screenshot.Save(filePath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании скриншота: {ex.Message}");
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void show_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }

        public static void DeleteUnwantedTextFiles(string directoryPath, string[] filesToKeep)
        {
            try
            {
                string[] allTextFiles = Directory.GetFiles(directoryPath, "*.txt");

                HashSet<string> filesToKeepSet = new HashSet<string>(filesToKeep);

                foreach (string filePath in allTextFiles)
                {
                    string fileName = Path.GetFileName(filePath);

                    if (!filesToKeepSet.Contains(fileName))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при удалении файла {filePath}: {ex.Message}");
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Директория не найдена: {directoryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string[] filesToKeep = { "auto.txt", "token.txt", "password.txt" };

            DeleteUnwantedTextFiles(directoryPath, filesToKeep);
            tray.BalloonTipText = "All users who had access were deleted successfully!";
            tray.ShowBalloonTip(5000);
        }

        private static void AppendDriveInfo(DriveInfo drive, StringBuilder sb)
        {
            lock (sb)
            {
                sb.AppendLine($"Диск: {drive.Name}");
                sb.AppendLine($"  Тип: {drive.DriveType}");
                sb.AppendLine($"  Формат: {drive.DriveFormat}");
                sb.AppendLine($"  Метка: {drive.VolumeLabel}");
                sb.AppendLine($"  Свободное место: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB");
                sb.AppendLine($"  Всего места: {drive.TotalSize / (1024 * 1024 * 1024)} GB");
            }
        }

        private static void AppendDirectory(DirectoryInfo directory, StringBuilder sb, string indent)
        {
            try
            {
                lock (sb)
                {
                    sb.AppendLine(indent + directory.Name);
                }

                List<FileSystemInfo> fileSystemInfos = new List<FileSystemInfo>();
                try
                {
                    fileSystemInfos.AddRange(directory.GetDirectories());
                    fileSystemInfos.AddRange(directory.GetFiles());
                }
                catch (UnauthorizedAccessException)
                {
                    lock (sb)
                    {
                        sb.AppendLine(indent + "  Доступ запрещен");
                    }
                    return;
                }
                catch (Exception ex)
                {
                    lock (sb)
                    {
                        sb.AppendLine(indent + $"  Ошибка при доступе: {ex.Message}");
                    }
                    return;
                }


                Parallel.ForEach(fileSystemInfos, fsi =>
                {

                    if (fsi is DirectoryInfo)
                    {
                        AppendDirectory((DirectoryInfo)fsi, sb, indent + "  ");
                    }
                    else if (fsi is FileInfo)
                    {
                        lock (sb)
                        {
                            sb.AppendLine(indent + "  " + fsi.Name);
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                lock (sb)
                {
                    sb.AppendLine(indent + $"  Ошибка: {ex.Message}");
                }
            }
        }

        async Task DownloadFileAsync(ITelegramBotClient botClient, string fileId, string fileName, CancellationToken cancellationToken, Update update)
        {
            try
            {
                Telegram.Bot.Types.File fileInfo = await botClient.GetFileAsync(fileId, cancellationToken: cancellationToken);
                string filePath = fileInfo.FilePath;

                string downloadPath = Path.Combine(sharedPath1, fileName);

                string directoryPath = Path.GetDirectoryName(downloadPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (FileStream fileStream = new FileStream(downloadPath, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(filePath, fileStream, cancellationToken: cancellationToken);
                }

                Console.WriteLine($"File {fileName} downloaded successfully to {downloadPath}");

                await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"File '{fileName}' saved successfully",
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");

                await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Произошла ошибка при сохранении файла: {ex.Message}",
                    cancellationToken: cancellationToken);
            }
        }

        


        /*public static void RunInteractiveCmd()
        {
           


            process.Start();

            StreamWriter inputWriter = process.StandardInput;
            StreamReader outputReader = process.StandardOutput;
            StreamReader errorReader = process.StandardError;

            // Бесконечный цикл для ввода команд
            while (true)
            {


                inputWriter.WriteLine(command);
                inputWriter.Flush(); // Отправляем команду в CMD

                // Читаем вывод
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                // Читаем вывод и ошибки, пока не будет прочитано все
                while (!outputReader.EndOfStream || !errorReader.EndOfStream)
                {
                    if (!outputReader.EndOfStream)
                    {
                        string line = outputReader.ReadLine();
                        Console.WriteLine(line); // Отображаем вывод на экране
                        output.AppendLine(line);
                    }

                    if (!errorReader.EndOfStream)
                    {
                        string line = errorReader.ReadLine();
                        Console.Error.WriteLine(line); // Отображаем ошибки на экране
                        error.AppendLine(line);
                    }

                    // Небольшая задержка, чтобы избежать блокировки
                    System.Threading.Thread.Sleep(10);
                }

                *//*// Добавляем вывод и ошибки в файл
                File.AppendAllText(outputFilePath, $"cmd> {command}\r\n", Encoding.UTF8);
                File.AppendAllText(outputFilePath, output.ToString(), Encoding.UTF8);
                if (error.Length > 0)
                {
                    File.AppendAllText(outputFilePath, "Ошибки:\r\n", Encoding.UTF8);
                    File.AppendAllText(outputFilePath, error.ToString(), Encoding.UTF8);
                }
                File.AppendAllText(outputFilePath, "\r\n", Encoding.UTF8);*//*


            }

            // Закрываем потоки и процесс
            inputWriter.Close();
            outputReader.Close();
            errorReader.Close();
            process.Close();

            Console.WriteLine("Выход из командной строки.");
        }*/
    }
}
