using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Controls;

const string installationDir = @"C:\ProgramData\Autodesk\ApplicationPlugins\Rincovitch.bundle";
const string sourceDir = @"..\..\..\Rincovitch.bundle";
const string projectName = "RincovitchRevit";
const string outputName = "RincovitchRevit";
const string outputDir = "output";
const string version = "1.0.3";

var project = new Project
{
  GUID = new Guid("0347E6CE-EB02-4C4E-BE63-6EF422BE1AA0"),
  Name = projectName,
  OutDir = outputDir,
  Platform = Platform.x64,
  Description = "Project Support Developer Work With Revit API",
  UI = WUI.WixUI_InstallDir,
  Version = new Version(version),
  OutFileName = $"{outputName}-{version}",
  InstallScope = InstallScope.perUser,
  MajorUpgrade = MajorUpgrade.Default,
  BackgroundImage = @"..\..\..\Resources\RincovitchBG.png",
  BannerImage = @"..\..\..\Resources\RincovitchBN.png",
  Media = new System.Collections.Generic.List<Media> { new Media { EmbedCab = true } }, // ép thành 1 file MSI duy nhất
  ControlPanelInfo =
    {
        Manufacturer = "Autodesk",
        Comments = "Project Support Developer With Revit API",
        ProductIcon = @"..\..\..\Resources\Rincovitch.ico"
    },
  Dirs = new[]
    {
        new Dir(installationDir,
            GenerateWixEntities(sourceDir)
        )
    }
};

WixEntity[] GenerateWixEntities(string bundleFolder)
{
  var entities = new List<WixEntity>();

  foreach (var file in Directory.GetFiles(bundleFolder))
    entities.Add(new WixSharp.File(file));

  foreach (var dir in Directory.GetDirectories(bundleFolder))
    entities.Add(BuildDir(dir));

  return entities.ToArray();
}

Dir BuildDir(string folder)
{
  var dirEntities = new List<WixEntity>();

  foreach (var file in Directory.GetFiles(folder))
    dirEntities.Add(new WixSharp.File(file));

  foreach (var subDir in Directory.GetDirectories(folder))
    dirEntities.Add(BuildDir(subDir));

  return new Dir(Path.GetFileName(folder), dirEntities.ToArray());
}

MajorUpgrade.Default.AllowSameVersionUpgrades = true;
project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.InstallDirDlg);
project.BuildMsi();
