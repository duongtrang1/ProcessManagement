﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Ionic.Zip;
using ProcessManagement.Controllers;
using ProcessManagement.Models;
using System.Text;

namespace ProcessManagement.Services
{
    public class FileService
    {
        ///=============================================================================================
        public readonly PMSEntities db = new PMSEntities();
        CommonService commonService = new CommonService();
        ///=============================================================================================
        ///
        public FileManager saveFile(int idGroup, HttpPostedFileBase file, string savePath, Direction Direction)
        {
            FileManager f = new FileManager();
            if (file != null && file.ContentLength > 0)
            {
                string AppPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = AppPath + savePath;

                string fileName = Path.GetFileName(file.FileName);
                string path = Path.Combine(filePath, fileName);
                string extension = Path.GetExtension(path);
                if (!File.Exists(path))
                {
                    createDirectory(savePath);
                }

                string fileNameWithOutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                DirectoryInfo d = new DirectoryInfo(filePath);
                FileInfo[] files = d.GetFiles(string.Format("*{0}", extension));
                int count = 0;
                foreach (FileInfo fs in files)
                {
                            
                    if (fs.Name.Replace(extension,"").StartsWith(fileNameWithOutExtension))
                        count++;
                }
                if (count > 0)
                {
                    fileName = string.Format("{0} ({1}){2}", fileNameWithOutExtension, count, extension);
                    path = Path.Combine(filePath, fileName);
                }
                file.SaveAs(path);


                f.Id = commonService.getRandomString(50);
                f.IdGroup = idGroup;
                f.Name = fileName;
                f.Path = savePath;
                f.Type = extension;
                f.Direction = Direction.ToString();
                f.Create_At = DateTime.Now;
                f.Update_At = DateTime.Now;
                db.FileManagers.Add(f);
                db.SaveChanges();
            }
            return f;

        }
        //public List<string> getAllFileNameFromFolder(string path)
        //{
        //    List<string> f = new List<string>();
        //    DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
        //    if (d.Exists)
        //    {
        //        FileInfo[] Files = d.GetFiles(); //Getting Text files

        //        foreach (FileInfo file in Files)
        //        {
        //            f.Add(file.Name);
        //        }
        //    }
        //    return f;

        //}
        public FileManager findFile(string id)
        {
            FileManager file = db.FileManagers.Find(id);
            return file;
        }
        public FileManager findFile(int idGroup, string path)
        {
            FileManager file = db.FileManagers.FirstOrDefault(x => x.IdGroup == idGroup &&  x.Path == path);
            return file;
        }
        public List<FileManager> findFiles(int idGroup, string path)
        {
            List<FileManager> file = db.FileManagers.Where(x => x.IdGroup == idGroup && x.Path == path).ToList();
            return file;

        }
        public void removeFile(FileManager file)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = AppPath + string.Format("{0}/{1}", file.Path, file.Name);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            db.FileManagers.Remove(file);
            db.SaveChanges();

        }
        public void removeFile(string path)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = AppPath + path;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        public FileManager changeFileName(FileManager file,string filename)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = AppPath + string.Format("{0}/{1}", file.Path, file.Name);
            string targetPath = AppPath + string.Format("{0}/{1}", file.Path, filename);
            if (File.Exists(filePath))
            {
                File.Copy(filePath, targetPath, true);
                File.Delete(filePath);
            }
            file.Name = filename;
            //TODO: Update Type cho file
            file.Update_At = DateTime.Now;
            db.SaveChanges();
            return file;
        }
        public bool checkFileExist(int idGroup,string name, Direction direction, string path)
        {
            FileManager file = db.FileManagers.FirstOrDefault(x => x.IdGroup == idGroup && x.Direction == direction.ToString() && x.Path == path && x.Name == name);
            return file != null ? true : false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Group Id</param>
        //public void CreateDirectory(int id)
        //{
        //    string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        //    string filePath = AppPath + "App_Data\\Files\\group";
        //    string folderName = string.Format("{0}",id);
        //    filePath += String.Format("\\{0}", folderName);
        //    string stepPath = filePath + String.Format("\\process");
        //    string introPath = filePath + String.Format("\\intro");
        //    DirectoryInfo stepDirectory = Directory.CreateDirectory(stepPath);
        //    DirectoryInfo introDirectory = Directory.CreateDirectory(introPath);


        //}
        public void createDirectory(string path)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = AppPath + path;
            if (!Directory.Exists(filePath))
            {
                DirectoryInfo introDirectory = Directory.CreateDirectory(filePath);
            }
        }
        public void removeDirectory(string path)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = AppPath + path;
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath,true);
                IQueryable<FileManager> filemanager = db.FileManagers.Where(x => x.Path == path);
                db.FileManagers.RemoveRange(filemanager);
                db.SaveChanges();
            }

        }
        public void emptyDirectory(string path)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = AppPath + path;
            DirectoryInfo fileDirectory = new DirectoryInfo(filePath);
            if (fileDirectory.Exists)
            {
                FileInfo[] files = fileDirectory.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
                FileManager filemanager = db.FileManagers.FirstOrDefault(x => x.Path == path);
                if (filemanager != null)
                {
                    db.FileManagers.Remove(filemanager);
                    db.SaveChanges();
                }
            }
        }
        public void copyDirectory(string path, string destination,bool copyOnly = false,bool copySubDirs = false)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string currentPath = AppPath + path;
            string destinationPath = AppPath + destination;
            if (!Directory.Exists(destinationPath))
            {
                createDirectory(destination);
            }
            DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
            DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);
            if (currentDirectory.Exists)
            {
                FileInfo[] files = currentDirectory.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.CopyTo(Path.Combine(destinationDirectory.FullName, file.Name), overwrite:true);
                    if (!copyOnly)
                    {
                        FileManager f = db.FileManagers.FirstOrDefault(x => x.Path == path);
                        FileManager fn = new FileManager();
                        fn.Id = commonService.getRandomString(50);
                        fn.IdGroup = f.IdGroup;
                        fn.Name = f.Name;
                        fn.Type = f.Type;
                        fn.Path = destination;
                        fn.Direction = f.Direction;
                        fn.Create_At = DateTime.Now;
                        fn.Update_At = DateTime.Now;
                        db.FileManagers.Add(fn);
                        db.SaveChanges();
                    }
                }
                if (copySubDirs)
                {
                    DirectoryInfo[] dirs = currentDirectory.GetDirectories();
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string subPath = Path.Combine(path, subdir.Name);
                        string subDestination = Path.Combine(destination, subdir.Name);
                        copyDirectory(path: subPath, destination: subDestination, copyOnly, copySubDirs);
                    }
                }
            }
        }
        public bool checkFileOverSize(HttpPostedFileBase file)
        {
            ConfigRule fileSizeRule = db.ConfigRules.Find("filesize");
            if (fileSizeRule != null && file != null)
            {
                //get file size
                int fileSize = file.ContentLength;

                //convert to byte
                int value = fileSizeRule.Value;
                string unit = fileSizeRule.Unit.ToLower();
                int pow;
                switch (unit)
                {
                    case "kb":
                        pow = 1;
                        break;
                    case "mb":
                        pow = 2;
                        break;
                    case "gb":
                        pow = 3;
                        break;
                    default:
                        pow = 0;
                        break;
                }
                double toByte = value * Math.Pow(1024, pow);
                return fileSize > toByte ? true : false; 
            }
            return false;
        }
        public void createJsonFile(string path,object content)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string copyPath = AppPath + path + "/data.json";

            string json = JsonConvert.SerializeObject(content);
            File.WriteAllText(copyPath, json);
        }
        public FileManager zipFile(int groupid, string fileName, string path)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(AppPath, path, fileName);
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncoding = Encoding.UTF8;
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.Comment = "This zip was created for copy process,do not change any file\r\nFile này tạo ra dùng để sao chép quy trình, đừng chỉnh sửa gì\r\nDevTeam: ClockWorks";
                zip.AddDirectory(Path.Combine(AppPath, path, "Upload"));
                zip.Password = "clockworks-pms";
                zip.AddFile(Path.Combine(AppPath, path, "data.json"), "");
                zip.Save(filePath);
            }

            //create filemangement
            FileManager file = new FileManager();
            file.Id = commonService.getRandomString(50);
            file.IdGroup = groupid;
            file.Name = fileName;
            file.Type = ".pms";
            file.Path = path;
            file.Direction = Direction.Export.ToString();
            file.Create_At = DateTime.Now;
            file.Update_At = DateTime.Now;
            db.FileManagers.Add(file);
            db.SaveChanges();
            return file;
        }
    }
}