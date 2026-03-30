using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;
using static System.Net.WebRequestMethods;
using File = WixSharp.File;

namespace MyUniversalInstaller
{
  class Program
  {
    static void Main()
    {
      // ==========================================================
      // 1. CẤU HÌNH THÔNG SỐ
      // ==========================================================
      string projectName = "RincovitchApp";
      string companyName = "Rincovitch";
      string version = "1.0.3";
      string outputName = "RincovitchAppSetup";
      string sourceDir = @"..\..\..\bundle";
      //string sourceDir = @"bundle";
      string mainExeName = "RincovitchApp.exe";

      // ==========================================================
      // 2. CHUẨN BỊ DANH SÁCH THỰC THỂ (ENTITIES)
      // ==========================================================

      // Danh sách chứa tất cả file và folder con
      var entityList = new List<WixEntity>();

      // Thêm file thực thi chính và Shortcut
      var mainFile = new File(new Id("RincovitchAppId"), $@"{sourceDir}\{mainExeName}",
                          new FileShortcut(projectName, @"%ProgramMenu%"),
                          new FileShortcut(projectName, @"%Desktop%"),
                          new FileShortcut(projectName, @"%Startup%")
                          {
                            Arguments = "--background",
                            
                          });
      entityList.Add(mainFile);

      // Gọi hàm quét toàn bộ các file/folder còn lại và thêm vào danh sách
      entityList.AddRange(GenerateWixEntities(sourceDir, mainExeName));

      // ==========================================================
      // 3. KHỞI TẠO PROJECT
      // ==========================================================
      var project = new Project
      {
        Name = projectName,
        GUID = new Guid("D02D554A-4AAC-42C4-99A6-24F46DE69A6B"),
        OutDir = "Releases",
        Platform = Platform.x64,
        Description = "Project Support Developer Work With Task",
        Version = new Version(version),
        OutFileName = $"{outputName}-v{version}",
        InstallScope = InstallScope.perUser,
        UI = WUI.WixUI_ProgressOnly,

        MajorUpgrade = new MajorUpgrade
        {
          AllowSameVersionUpgrades = true,
          Schedule = UpgradeSchedule.afterInstallInitialize,
          DowngradeErrorMessage = "A newer version has already been installed."
        },

        // Giao diện hình ảnh (Phải đảm bảo file tồn tại)
        BackgroundImage = @"..\..\..\Resources\RincovitchBG.png",
        BannerImage = @"..\..\..\Resources\RincovitchBN.png",

        Media = new List<Media> { new Media { EmbedCab = true } },

        ControlPanelInfo =
                {
                    Manufacturer = companyName,
                    Comments = "Project Support Developer With Task",
                    ProductIcon = @"..\..\..\Resources\RincovitchIcon.ico"
                },

        // Định nghĩa cấu trúc thư mục
        Dirs = new[]
          {
                    new Dir($@"%LocalAppData%\{companyName}\{projectName}",
                        entityList.ToArray() // Chuyển List thành Mảng ở đây là an toàn nhất
                    )
                }
      };

      //project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
      // 2. TỰ ĐỘNG CHẠY APP: Sau khi cài xong thì kích hoạt App luôn
      // SỬA: Thay đổi WixEntity[] thành Action[]
        project.Actions = new WixSharp.Action[] 
        {
            new InstalledFileAction(
                "RincovitchAppId", 
                "--background", 
                Return.asyncNoWait, 
                When.After, 
                Step.InstallFinalize, 
                Condition.NOT_Installed)
        };
      // 4. BUILD
      project.BuildMsi();
      Console.WriteLine("Build completed!");
    }

    /// <summary>
    /// Hàm quét đệ quy toàn bộ thư mục để lấy mọi package và sub-folders
    /// </summary>
    private static WixEntity[] GenerateWixEntities(string sourceDir, string skipFile)
    {
      var entities = new List<WixEntity>();

      // Lấy các file trong thư mục hiện tại
      var files = Directory.GetFiles(sourceDir);
      foreach (var file in files)
      {
        if (Path.GetFileName(file).Equals(skipFile, StringComparison.OrdinalIgnoreCase))
          continue;

        entities.Add(new File(file));
      }

      // Lấy các thư mục con và quét đệ quy
      var dirs = Directory.GetDirectories(sourceDir);
      foreach (var dir in dirs)
      {
        var dirName = Path.GetFileName(dir);
        // Tạo một Dir entity mới và nạp các entity con của nó vào
        var subDir = new Dir(dirName, GenerateWixEntities(dir, ""));
        entities.Add(subDir);
      }

      return entities.ToArray();
    }
  }
}