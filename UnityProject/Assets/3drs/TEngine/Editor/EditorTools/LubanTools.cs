using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;


namespace TEngine.Editor
{
    
    public static class LubanTools
    {
        [MenuItem("TEngine/Tools/Luban 转表")]
        public static void ExportConfig()
         {
             if (Application.platform == RuntimePlatform.WindowsEditor)
             {
                 Application.OpenURL(System.IO.Path.Combine(Application.dataPath, $"/../../Configs/GameConfig/gen_code_bin_to_project_lazyload.bat"));
             }
             else if (Application.platform == RuntimePlatform.OSXEditor)
             {
                 string shell = Application.dataPath + $"/../../Configs/GameConfig/gen_code_bin_to_project_lazyload.sh";

                 string workingDirectory = Application.dataPath + "/../";
                 ExecuteExternalCmd("sh", shell, workingDirectory);
             }

             AssetDatabase.Refresh();
             Log.Info($"Config Output Succeed!");
         }
        
        
        /// <summary>
        /// 执行一段shell命令
        /// </summary>
        /// <param name="fileName">要启动的应用程序</param>
        /// <param name="arguments">参数</param>
        /// <param name="workingDirectory">工作目录，就是要提前cd的目录</param>
        /// <returns></returns>
        /// <exception cref="SystemException"></exception>
        public static string ExecuteExternalCmd(string fileName, string arguments, string workingDirectory = "")
        {
            string data = null;
            try
            {
                //Debug.Log($"ExecuteExternalCmd: {fileName} {arguments} in directory {workingDirectory}");

                ProcessStartInfo startInfo = new ProcessStartInfo(fileName, arguments)
                {
                    CreateNoWindow = true, //不显示程序窗口
                    UseShellExecute = false, //是否使用操作系统shell启动
                    RedirectStandardOutput = true, //输出信息
                    RedirectStandardError = true, //输出错误
                    WorkingDirectory = workingDirectory,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                };


                using (Process process = Process.Start(startInfo))
                {
                    process.OutputDataReceived += (send, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            string text = e.Data.Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                data += text + "\n";
                            }
                        }
                    };

                    process.ErrorDataReceived += (send, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            string text = e.Data.Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                UnityEngine.Debug.LogError(text);
                            }
                        }
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    //等待程序执行完退出进程
                    process.WaitForExit();
                    process.Close();
                }

            }
            catch (Exception exception)
            {
                throw new SystemException(
                    $"ExecuteExternalCmd({fileName} {arguments} in {workingDirectory}\n{exception.Message}) Exception: {exception.Message}");
            }

            return data;
        }
        
        [MenuItem("TEngine/Tools/Luban Edt 转表")]
        public static void BuildLubanExcel()
        {
            string projectRoot = GetProjectRootPath();
            string lubanDllPath = NormalizePath(Path.Combine(projectRoot, "../Tools/Luban/Luban.dll"));
            string configPath = NormalizePath(Path.Combine(projectRoot, $"../Configs/GameConfig/luban.conf"));
            string workingDirectory = NormalizePath(projectRoot);
            
            if (!File.Exists(lubanDllPath))
            {
                Log.Error($"Luban DLL 未找到: {lubanDllPath}");
                return;
            }

            if (!File.Exists(configPath))
            {
                Log.Error($"配置文件未找到: {configPath}");
                return;
            }

            string arguments = BuildCommandArguments(lubanDllPath, configPath);
            string executable = GetExecutable();
            bool success = RunExternalScript(executable, arguments, workingDirectory, out var output, out var error);
            
            if (success)
            {
                AssetDatabase.Refresh();
                Log.Info($"<color=white>Config 导表成功！</color>");
            }
            else
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Log.Error($"<color=red>[导表失败]:</color>\n{error}");
                }
                else if (!string.IsNullOrEmpty(output))
                {
                    Log.Error($"<color=red>[导表失败]:</color>\n{output}");
                }
                else
                {
                    Log.Error($"<color=red>[导表失败]: 未知错误</color>");
                }
            }
        }
        
        [MenuItem("TEngine/Tools/打开表格目录")]
        public static void OpenConfigFolder()
        {
            CopyFile();
            OpenFolderHelper.Execute(Application.dataPath + @"/../../Configs/GameConfig");
        }
        
        private static readonly string[] potentialPaths = 
        {
            "/usr/local/share/dotnet/dotnet",    // 官方安装的默认路径
            "/opt/homebrew/bin/dotnet"          // Homebrew 安装的路径
        };
        private static bool RunExternalScript(string executable, string arguments, string workingDirectory, out string output, out string error)
        {
            output = string.Empty;
            error = string.Empty;

            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                using (Process process = Process.Start(processInfo))
                {
                    output = process.StandardOutput.ReadToEnd().Trim();
                    error = process.StandardError.ReadToEnd().Trim();
                    process.WaitForExit();

                    return process.ExitCode == 0;
                }
            }
            catch (System.Exception ex)
            {
                error = $"运行外部脚本时出错：{ex.Message}";
                Log.Error($"[运行外部脚本异常]: {error}");
                return false;
            }
        }

        private static string GetExecutable()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Path.Combine(GetProjectRootPath(), "dotnet-sdk-7.0.100-win-x64/dotnet");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                foreach (var path in potentialPaths)
                {
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
                throw new System.PlatformNotSupportedException("未找到 dotnet 执行文件！");
            }
            else
            {
                throw new System.PlatformNotSupportedException("当前平台不支持导表操作！");
            }
        }
        
        private static string BuildCommandArguments( string dllPath, string configPath)
        {
            string outputCodeDir, binOutputDir, customTemplateDir;
             binOutputDir = Path.Combine(GetProjectRootPath(),"Assets/AssetRaw/Configs/bytes");
             outputCodeDir = Path.Combine(GetProjectRootPath(),"Assets/GameScripts/HotFix/GameProto/GameConfig");
             customTemplateDir = Path.Combine(GetProjectRootPath(),
                 $"../Configs/GameConfig/CustomTemplate/CustomTemplate_Client_LazyLoad");
            // 构建命令参数
            return $"\"{dllPath}\" " +
                   $"-t client " +
                   $"-c cs-bin " +
                   $"-d bin " +
                   $"--conf \"{configPath}\" " +
                   $"--customTemplateDir=\"{customTemplateDir}\" " +
                   $"-x outputCodeDir=\"{outputCodeDir}\" " +
                   $"-x bin.outputDataDir=\"{binOutputDir}\" ";
        }
        public static void CopyFile()
        {
            string sourcePath = Path.Combine(GetProjectRootPath(), $"../Configs/GameConfig/CustomTemplate/ConfigSystem.cs");
            string destinationPath = Path.Combine(GetProjectRootPath(),"Assets/GameScripts/HotFix/GameProto/ConfigSystem.cs");

            // 确保源文件存在
            if (File.Exists(sourcePath))
            {
                // 复制文件并覆盖已存在的文件
                try
                {
                    File.Copy(sourcePath, destinationPath, true);
                    Log.Info("File copied successfully.");
                }
                catch (IOException e)
                {
                    Log.Error($"Error copying file: {e.Message}");
                }
            }
            else
            {
                Log.Error($"Source file does not exist at: {sourcePath}");
            }
        }
        private static string GetProjectRootPath()
        {
            string dataPath = Application.dataPath;
            return Path.GetFullPath(Path.Combine(dataPath, "../"));
        }

        private static string NormalizePath(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}