using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FDUnziper
{
    public class FDHelper
    {
        private static FDHelper _instance = new FDHelper();
        private FDHelper() { }
        private string _rarExePath, _targetPath;

        public static FDHelper Instance
        {
            get { return _instance; }
        }

        public void Initialize(string rarExePath, string targetPath)
        {
            _rarExePath = rarExePath;
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            _targetPath = targetPath;
        }

        public void Unzip(string path)
        {
            var files = GetFiles(path);
            //Func<string,string> cmd = fileName=>
            //{
            //    string res = $"\"{_rarExePath}\" \"{fileName}\"";
            if (files.Count > 0)
            {
                foreach (var file in files)
                {
                    File.SetAttributes(file.FileName, FileAttributes.Normal);
                    var target = Path.Combine(_targetPath, file.DirShorName);
                    if (!Directory.Exists(target))
                    {
                        Directory.CreateDirectory(target);
                    }
                    target = $"\"{target}\"";
                    //ExecuteProcess($"\"{_rarExePath}\"", $"x \"{file.FileName}\" {target}", target);
                    Process.Start($"\"{_rarExePath}\"", $"x \"{file.FileName}\" {target}");
                }
            }
        }

        public void ExecuteProcess(string exe, string commandInfo, string workingDir = "", ProcessWindowStyle processWindowStyle = ProcessWindowStyle.Hidden, bool isUseShellExe = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exe;
            startInfo.Arguments = commandInfo;
            startInfo.WindowStyle = processWindowStyle;
            startInfo.UseShellExecute = isUseShellExe;
            if (!string.IsNullOrWhiteSpace(workingDir))
            {
                startInfo.WorkingDirectory = workingDir;
            }

            ExecuteProcess(startInfo);
        }

        /// <summary>
        /// 直接另启动一个进程
        /// </summary>
        /// <param name="startInfo">启动进程时使用的一组值</param>
        public void ExecuteProcess(ProcessStartInfo startInfo)
        {
            try
            {
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                process.Close();
                process.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("进程执行失败：\n\r" + startInfo.FileName + "\n\r" + ex.Message);
            }
        }

        private List<FileInfo> GetFiles(string path)
        {
            if (!Directory.Exists(path))
                return new List<FileInfo>();
            var files = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
            List<FileInfo> list = new List<FileInfo>();
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    if (IsSpecifiedType(file, out FileType type))
                    {
                        list.Add(new FileInfo(file, type));
                    }
                }
                else if (Directory.Exists(file))
                {
                    var res = GetFiles(file);
                    if (res.Count > 0)
                    {
                        list.AddRange(res);
                    }
                }
            }
            return list;
        }

        private bool IsSpecifiedType(string fileName, out FileType type)
        {
            var ext = Path.GetExtension(fileName);
            type = FileType.None;
            if (ext.Equals(".rar", StringComparison.OrdinalIgnoreCase))
            {
                type = FileType.Rar;
            }
            else if (ext.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                type = FileType.Zip;
            }
            return type != FileType.None;
        }
    }

    internal class FileInfo
    {
        public string Name { get; set; }

        public string DirShorName { get; set; }

        public string FileName { get; set; }

        public FileType Type { get; set; }

        public FileInfo(string fileName, FileType fileType)
        {
            FileName = fileName;
            Type = fileType;
            Name = Path.GetFileNameWithoutExtension(fileName);
            DirShorName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(fileName));
        }
    }

    internal enum FileType
    {
        None,
        Rar,
        Zip
    }
}
