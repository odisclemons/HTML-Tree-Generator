using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    static class gl
    {
        // global strings
        public static string pathName;
        public static string docName;
        public static string webRoot;
        public static string fileTypes;
        public static int folderIndent;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // @"C:\Users\brick\Desktop" test path.  crashes when accessing folders that need permission
            gl.pathName = "";
            gl.folderIndent = 0;
            gl.docName = "";
            Console.WriteLine("What is the full path to the HTML file to output? (If blank then i'll put it on your desktop.");
            //gl.docName = Console.ReadLine();
            if (gl.docName == ""){ gl.docName = Environment.GetEnvironmentVariable("userprofile") + @"\Desktop\output.html"; }
            print("Output set to " + gl.docName);
            gl.webRoot = "";

            //lets add a slash to the end of the webroot if they didnt put one for consistency
            if (gl.webRoot.EndsWith("\\") == false){ gl.webRoot = gl.webRoot + "\\"; };

            //Check to see if user used command line arguments.  If not, prompt them for a path name
            if (args.Length == 0)
            {
                Console.WriteLine("Please type the full path of the folder and press ENTER (example: C:\\wwwroot\\Filesandstuff)");
                //gl.pathName = Console.ReadLine();
                Console.WriteLine("Please type the root folder of the web server and press ENTER (example: C:\\wwwroot\\) This IS case sensitive.");
                //gl.webRoot = Console.ReadLine();
                Console.WriteLine("Please type the allowed file types seperated by commas.  example: 'pdf, doc, txt' (leave blank to accept all files)");
                //gl.fileTypes = Console.ReadLine();
                Console.Clear();
                if (gl.pathName == "")
                {
                    //gl.pathName = @"X:\Portals\0\BoardItems\";
                    gl.pathName = @"X:\Portals\0\pdf\LEP Vital Documents and Forms";
                    gl.webRoot = @"X:\";
                    gl.fileTypes = "pdf txt doc";
                    print("No input, so selecting default test folder.");
                    print("Default path is: " + gl.pathName);
                    print("Root of web server is: " + gl.webRoot);
                };
            }
            else { print("User selected folder " + gl.pathName); };
            if (gl.webRoot == "") { gl.webRoot = gl.pathName; };


            prepDoc();
            processDirectories(gl.pathName);

            updateFile("</ul>");
            updateFile("</div>");
            updateFile(@"</body>");
            updateFile("</html>");
            pause();
        }
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        static void processDirectories(string directory)
        {
            //store all folders and subfolders as an array called 'directories'
            //get the length of the entire list first so i can define it in the array
            int number = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly).Length;
            string[] directories = new string[number];
            string temp = "";

            //     create opening ul tag for folder.
            Console.WriteLine("Working on " + directory + " right now.");
            //if this is the root folder, dont add collapse css tag, otherwise collapse it so the user has to click on it
            if (directory == gl.pathName) {
                print("Folder is " + directory + " so it is at the top of the tree.");
                temp = "<li><a href=\"#" + "\">" + justGetName(directory) + "</a><ul>";
            } else {
                temp = "<li><a href=\"#" + "\">" + justGetName(directory) + "</a><ul class=\"collapse\">";
            };
            updateFile(temp);

            print("Opening tag for folder: " + directory);
            if (number > 0)
            {
                for (int i = 0; i < number; i++)
                {
                    directories[i] = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly)[i];
                    print("Process recursive directory " + directories[i]);
                    processDirectories(directories[i]);
                };
                number--;
            } else {
                print("No more folders inside. Listing files. Closing folder " + directory);
                print("Listing files inside " + directory);
                listFiles(directory, gl.docName, gl.webRoot);
                updateFile("</ul></li>");
            };

            if (isRoot(directory))
            {
                print("Closing ROOT Folder " + directory);
                listFiles(directory, gl.docName, gl.webRoot);
                updateFile("</ul></li>");
            };
        }
        

        static bool isRoot(string directory)
        {
            int number = Directory.GetDirectories(gl.pathName, "*", SearchOption.TopDirectoryOnly).Length;
            string[] directories = new string[number];
            for (int i = 0; i < number; i++)
            {
                directories[i] = Directory.GetDirectories(gl.pathName, "*", SearchOption.TopDirectoryOnly)[i];
                if (directories[i] == directory) { return true; };
            }
            return false;
        }

        static string cleanFolderPath(string path, string root)
        {
            path = path.Replace(root, "/");
            return path;
        }

        static string justGetName(string folderName)
        {
            int length = folderName.Length;
            string temp = "";
            int i = length;
            do
            {
                temp = folderName.Substring(i, length - i);
                // start counting from the last character to the first.  
                //if it has a slash, everything from the next char to the end is the name
                if (temp.StartsWith(@"\"))
                {
                    //if the first character is a 0, then it is a number appended to make sorting happen.  hide that from the user.  otherwise return the original name
                    //return folderName.Substring(i + 1, length - (i + 1));
                    if (folderName.Substring(i + 1, length - (i + 1)).StartsWith("0")) {
                        return folderName.Substring(i + 5, (length - 4) - (i + 1));
                    } else {
                        return folderName.Substring(i + 1, length - (i + 1));
                    };
                }
                i--;
            } while (i > 0);

            return folderName;
        }

        static void updateFile(string updateString)
        {
            using (StreamWriter sw = File.AppendText(gl.docName))
            {
                sw.WriteLine(updateString);
            }

        }

        static void pause()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void listFiles(string pathName, string docNameLocal, string root)
        {
            string temp = "";
            string ext = "";
            print("Listing Files in " + pathName);
            print(Directory.GetFiles(pathName).Length + " files inside " + pathName);
            for (int i = 0; i < Directory.GetFiles(pathName).Length; i++)
            {
                //get file and path name for the next file
                ext = Directory.GetFiles(pathName)[i].ToLower().ToString();
                //now remove everything except the extension at the end
                ext = ext.Substring(ext.Length - 3, 3);
                print(gl.fileTypes);
                Console.WriteLine(gl.fileTypes.Split(ext));
                if (gl.fileTypes.Split(ext).Length - 1 > 0)
                {
                    temp = "<li><a href=\"" + cleanFolderPath(Directory.GetFiles(pathName)[i], root) + "\">" + justGetName(Directory.GetFiles(pathName)[i]) + "</a></li>";
                    temp = temp.Replace("\\", "/");
                    print("adding " + temp);
                    updateFile(temp);
                }
                
            }
        }

        static void print(string words)
        {
            Console.WriteLine(words);
        }

        static void prepDoc()
        {
            using (StreamWriter sw = File.CreateText(gl.docName))
            {
                sw.WriteLine(@"<html>");
                sw.WriteLine(@"<head>");
                sw.WriteLine("<link href=\"http://netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css\" rel=\"stylesheet\" />");
                sw.WriteLine("<link href=\"http://maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css\" rel=\"stylesheet\" />");
                //sw.WriteLine("<link href=\"/Portals/0/multinestedlists/style.css\" rel=\"stylesheet\" />");
                sw.WriteLine("<script src = \"http://code.jquery.com/jquery-1.11.0.min.js\"></script>");
                //sw.WriteLine("<script src=\"/Portals/0/multinestedlists/MultiNestedList.js\"></script>");
                sw.WriteLine(@"</head>");
                sw.WriteLine(@"<body>");
                sw.WriteLine("<div class=\"container\">");
                sw.WriteLine(@"<h4>These are public documents for transparency. Press the &#39;-&#39; symbol to expand a folder.</h4>");
                sw.WriteLine("<ul class=\"multi-nested-list\">");
            };
        }
    }
}
