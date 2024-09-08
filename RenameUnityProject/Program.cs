using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace RenameUnityProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] unityProjects = Directory.GetDirectories(Directory.GetCurrentDirectory());
            Console.WriteLine("select project to rename");
            for(int i = 0; i<unityProjects.Length; i++)
            {
                string[] splitted = unityProjects[i].Split("\\");
                Console.WriteLine(i + 1 + ". " + splitted[splitted.Length - 1]);
            }

            int usedIndex = -1;
            try
            {
                usedIndex += int.Parse(Console.ReadLine());
            }catch(Exception e)
            {
                Console.WriteLine("wrong index");
            }

            Console.WriteLine("Please write name for your unity project");
            string? name = Console.ReadLine();
            while (IsNameIncorrect(name))
            {
                Console.WriteLine("incorrect name please use something else");
                name = Console.ReadLine();
            }

            string[] dirs = unityProjects[usedIndex].Split("\\");
            string oldName = dirs[dirs.Length - 1];

            string newDirectoryName = "";
            for (int i = 0; i < dirs.Length - 1; i++)
            {
                newDirectoryName += dirs[i] + "\\";
            }
            newDirectoryName += name;

            Directory.Move(Directory.GetCurrentDirectory() + "\\" + oldName, newDirectoryName);

            RenameAllReferencesToOldName(newDirectoryName, oldName, name);
            Console.WriteLine("renamed project to: " + name + " succesfuly\n press any key to return");
            Console.Read();
        }

        static void RenameAllReferencesToOldName(string dir, string oldName,string? newName)
        {
            if (newName == null)
                return;

            foreach(var file in Directory.GetFiles(dir))
            {
                if (file == Environment.GetCommandLineArgs()[0])
                    continue;
                if (file.Contains("Library\\SourceAssetDB"))
                    continue;
                try
                {
                    string text = File.ReadAllText(file);
                    if (text.Contains(oldName))
                    {
                        text = text.Replace(oldName, newName);
                        File.WriteAllText(file, text);
                        Console.WriteLine("renamed references in " + file);
                    }

                    if (Path.GetFileName(file).Contains(oldName))
                    {
                        File.Move(file, Path.Combine(dir, newName + Path.GetExtension(file)));
                        Console.WriteLine("renamed " + file + " to " + Path.Combine(dir, newName + Path.GetExtension(file)));
                    }
;

                }
                catch (Exception e)
                {
                    Console.Write("exception for " + file + "\n" + e.ToString());
                }

            }
            foreach (var d in Directory.GetDirectories(dir))
                RenameAllReferencesToOldName(d,oldName, newName);
        }

        static bool IsNameIncorrect(string? name)
        {
            if (name == null)
                return true;

            if (name[name.Length - 1] == ' ' || name[name.Length - 1] == '.') 
                return true;
            //td add incorrect windows names 
            //CON, PRN, AUX, NUL
            //COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9
            //LPT1, LPT2, LPT3, LPT4, LPT5, LPT6, LPT7, LPT8, LPT9

            char[] incorrectChars = { '/', '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
            for (int i = 0; i < name.Length; i++)
                for (int j = 0; j < incorrectChars.Length; j++)
                    if (name[i] == incorrectChars[j])
                        return true;
            return false;
        }
    }
}
