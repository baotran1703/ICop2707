using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace ICOP_3
{
    public static class DirectoryF
    {
        public static string MainFolderName;
        public static string Model;
        public static string Setting;
        public static string History;
        public static string MES;
        public static string Current;
        public static string ICOP_EXT = ".icop";
        public static string Photo;
        public static string ProjectPath;

        public static void DirectoryMain()
        {

            if (Directory.Exists(MainFolderName))
            {
                Directory.CreateDirectory(MainFolderName);
            }

            Model = MainFolderName + "\\Model";
            Setting = MainFolderName + "\\Setting";
            History = MainFolderName + "\\History";
            MES = MainFolderName + "\\MES";
            Current = MainFolderName + "\\Current";
            Photo = MainFolderName + "\\Photo";

            if (!Directory.Exists(Model))
            {
                Directory.CreateDirectory(Model);
            }

            if (!Directory.Exists(Setting))
            {
                Directory.CreateDirectory(Setting);
            }

            if (!Directory.Exists(History))
            {
                Directory.CreateDirectory(History);
            }

            if (!Directory.Exists(MES))
            {
                Directory.CreateDirectory(MES);
            }

            if (!Directory.Exists(Current))
            {
                Directory.CreateDirectory(Current);
            }

            if (!Directory.Exists(Photo))
            {
                Directory.CreateDirectory(Photo);
            }
        }

        public static void SAVE()
        {
            string jsonString = JsonSerializer.Serialize(MainFolderName);
            File.WriteAllText (AppDomain.CurrentDomain.BaseDirectory + "MainFolderName.txt", jsonString);
        }

        public static bool TryCreateFolders()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "MainFolderName.txt"))
            {
                return false;
            }
            else
            {
                MainFolderName = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "MainFolderName.txt");
                MainFolderName = MainFolderName.Replace("\"", "");
                DirectoryMain();
                return true;
            }
        }

        public static bool SaveProject(ModelLogic.IcopModel Project, string project)
        {
            if (project == null) return false;
            else
            {
                if (project != null)
                {
                    ProjectPath = MainFolderName + "\\Model";
                    string ProjectFolder = ProjectPath + "\\" + project;
                    if (!Directory.Exists(ProjectFolder))
                    {
                        Directory.CreateDirectory(ProjectFolder);
                    }
                    try
                    {
                        string jsonString = JsonSerializer.Serialize(Project);
                        File.WriteAllText(ProjectFolder + "\\" + Project.Name + ICOP_EXT, jsonString);
                        if (Project.MergeSource != null)
                        {
                            if (File.Exists(ProjectFolder + "\\" + Project.Name + ".png"))
                            {   
                                File.Delete(ProjectFolder + "\\" + Project.Name + ".png");
                                using (var fileStream = new FileStream(ProjectFolder + "\\" + Project.Name + ".png", FileMode.OpenOrCreate, FileAccess.Write))
                                { 
                                    BitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(Project.MergeSource));
                                    encoder.Save(fileStream);
                                }
                            }
                            else
                            {
                                using (var fileStream = new FileStream(ProjectFolder + "\\" + Project.Name + ".png", FileMode.OpenOrCreate, FileAccess.Write))
                                {   
                                    BitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(Project.MergeSource));
                                    encoder.Save(fileStream);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Project not have image.");
                        }
                        Console.WriteLine(jsonString);

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        Console.WriteLine(e.ToString());
                        return false;
                    }

                }

            }
            return true;
        }

        public static bool saveModel(ModelLogic.IcopModel Model, string project, string model)
        {
            if (project == null && model == null) return false;
            else
            {
                ProjectPath = MainFolderName + "\\Model";
                string ProjectFolder = ProjectPath + "\\" + project;
                string ModelFolder = ProjectFolder + "\\" + Model.Name;
                if (!Directory.Exists(ModelFolder))
                {
                    Directory.CreateDirectory(ModelFolder);
                }

                try
                {
                    if (model != null)
                    {
                        string jsonString = JsonSerializer.Serialize(Model);
                        File.WriteAllText(ModelFolder + "\\" + Model.Name + ICOP_EXT, jsonString);

                        if (Model.MergeSource != null)
                        {
                            if (File.Exists(ModelFolder + "\\" + Model.Name + ".png"))
                            {
                                File.Delete(ModelFolder + "\\" + Model.Name + ".png");

                                using (var fileStream = new FileStream(ModelFolder + "\\" + Model.Name + ".png", FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    BitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(Model.MergeSource));
                                    encoder.Save(fileStream);
                                }
                            }
                                                     
                            else
                            {
                                using (var fileStream = new FileStream(ModelFolder + "\\" + Model.Name + ".png", FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    BitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(Model.MergeSource));
                                    encoder.Save(fileStream);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(" Model not have image.");
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return false;
                }
            }
            return true;
        }

    }



}



