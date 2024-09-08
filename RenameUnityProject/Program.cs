namespace RenameUnityProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int selectedIndex = 0;
            string? selectedOption = "";
            string[] unityProjects = Directory.GetDirectories(Directory.GetCurrentDirectory());
            do
            {
                Console.WriteLine("Select project to rename");
                for (int i = 0; i < unityProjects.Length; i++)
                {
                    string[] splitted = unityProjects[i].Split("\\");
                    Console.WriteLine(i + 1 + ". " + splitted[splitted.Length - 1]);
                }
                selectedOption = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Incorrect index");
            }
            while (!int.TryParse(selectedOption, out selectedIndex) || selectedIndex < 0 || selectedIndex > unityProjects.Length);
            Console.Clear();

            selectedIndex--;

            string? name = "";
            string[] dirs = unityProjects[selectedIndex].Split("\\");
            string oldName = dirs[dirs.Length - 1];
            do
            {
                Console.WriteLine("Write new name for "+ oldName+" unity project");
                name = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Incorrect name try something else");
            } while (!IsNameCorrect(name));
            Console.Clear();

            string newDirectoryName = "";
            for (int i = 0; i < dirs.Length - 1; i++)
                newDirectoryName += dirs[i] + "\\";

            newDirectoryName += name;

            Directory.Move(Directory.GetCurrentDirectory() + "\\" + oldName, newDirectoryName);
            RenameAllReferencesToOldName(newDirectoryName, oldName, name!);

            Console.WriteLine("Renamed " + oldName + " unity project to: " + name + " succesfuly\npress any key to return");
            Console.Read();
        }

        static void RenameAllReferencesToOldName(string baseDir, string oldName,string newName)
        {
            foreach(var file in Directory.GetFiles(baseDir))
            {
                //project will lock if we try to change something here
                if (file.Contains("Library\\SourceAssetDB"))
                    continue;
                if (file.Contains("Library\\ArtifactDB"))
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
                        File.Move(file, Path.Combine(baseDir, newName + Path.GetExtension(file)));
                        Console.WriteLine("renamed " + file + " to " + Path.Combine(baseDir, newName + Path.GetExtension(file)));
                    }
                }
                catch (Exception e)
                {
                    Console.Write("exception for " + file + "\n" + e.ToString());
                }

            }
            foreach (var dir in Directory.GetDirectories(baseDir))
                RenameAllReferencesToOldName(dir,oldName, newName);
        }

        static bool IsNameCorrect(string? name)
        {
            if (name == null)
                return false;

            if (name == "")
                return false;

            if (name[name.Length - 1] == ' ' || name[name.Length - 1] == '.')
                return false;

            char[] incorrectChars = { '/', '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
            for (int i = 0; i < name.Length; i++)
                for (int j = 0; j < incorrectChars.Length; j++)
                    if (name[i] == incorrectChars[j])
                        return false;

            string[] incorrectNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            for (int i=0;i<incorrectNames.Length;i++)
                if(name == incorrectNames[i])
                    return false;

            return true;
        }
    }
}
