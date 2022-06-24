using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RlktLuaEncodingConversion
{
    internal class Program
    {
        static void InstallContextMenu()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            //Right click on folder
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\Background\\shell\\lua_encoding", "", "Encode .LUAs", RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\Background\\shell\\lua_encoding\\command", "", string.Format("\"{0}\" \"%v\"", path), RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\Background\\shell\\lua_encoding", "Icon", path, RegistryValueKind.String);

            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\shell\\lua_encoding", "", "Encode .LUAs", RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\shell\\lua_encoding\\command", "", string.Format("\"{0}\" \"%v\"", path), RegistryValueKind.String);
            Registry.SetValue("HKEY_CURRENT_USER\\SOFTWARE\\Classes\\Directory\\shell\\lua_encoding", "Icon", path, RegistryValueKind.String);
        }

        public static bool ConvertToUTF8(string filename)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filename);

                //EF BB BF
                if (fileData[0] == 0xEF && fileData[1] == 0xBB && fileData[2] == 0xBF)
                {
                    return false;
                }

                byte[] newFileData = new byte[fileData.Length + 3];
                newFileData[0] = 0xEF;
                newFileData[1] = 0xBB;
                newFileData[2] = 0xBF;

                Array.Copy(fileData, 0, newFileData, 3, fileData.Length);

                File.WriteAllBytes(filename, newFileData);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{filename}]Error: {ex.Message}");
            }

            return false;
        }

        static void Main(string[] args)
        {
            InstallContextMenu();

            if (args.Length != 1)
            {
                Console.WriteLine($"usage: program.exe <folder path>");
                return;
            }

            string[] files = Directory.GetFiles(args[0]);
            foreach (var file in files)
            {
                if (file.Contains(".lua") || file.Contains(".LUA"))
                {
                    Program.ConvertToUTF8(file);
                }
            }
        }
    }
}
