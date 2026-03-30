using System.Collections.Generic;
using System.Windows;
using System;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using System.Windows.Media;
using Microsoft.Playwright;
using System.IO;

namespace RincovitchApp.API
{
  public class F_TotalSynergy
  {
    public static async Task Main()
    {
      using var playwright = await Playwright.CreateAsync();

      // 👉 DÙNG PROFILE EDGE THẬT CỦA BẠN
      var context = await playwright.Chromium.LaunchPersistentContextAsync(
          @"C:\Users\Khanh.nguyen\AppData\Local\Microsoft\Edge\User Data",
          new BrowserTypeLaunchPersistentContextOptions
          {
            Channel = "msedge",
            Headless = false,
          });

      var page = context.Pages.Count > 0
          ? context.Pages[0]
          : await context.NewPageAsync();
      await page.SetViewportSizeAsync(1920, 1080);
      // Mở URL
      await page.GotoAsync("https://app.totalsynergy.com/Timesheets/rincovitch");

      // =============================
      // 🔥 CHECK LOGIN
      // =============================
      await EnsureLoggedIn(page);

      Console.WriteLine("✅ Đã login, tiếp tục...");


      // Đảm bảo focus vào page
      await page.BringToFrontAsync();
      await page.ClickAsync("body");

      // =====================
      // DEBUG: Export HTML
      // =====================
      Console.WriteLine("\n🔍 Exporting HTML for debugging...");
      await ExportHTML(page);

      // =====================
      // INPUT
      // =====================
      Console.WriteLine("\n📋 Starting timesheet filling...\n");

      // Test data: 2 projects
      var testData = new[]
      {
      new { Project = "mel01", Stage = "Tensioning", Task = "Drafting", Days = new[] { ("Mon", "8"), ("Tue", "0"), ("Wed", "8"), ("Thu", "0"), ("Fri", "0"), ("Sat", "0"), ("Sun", "0") } },
      new { Project = "mel02", Stage = "Tensioning", Task = "Drafting", Days = new[] { ("Mon", "0"), ("Tue", "8"), ("Wed", "0"), ("Thu", "8"), ("Fri", "0"), ("Sat", "0"), ("Sun", "0") } }
    };

      int projectCount = 0;
      foreach (var row in testData)
      {
        projectCount++;
        try
        {
          Console.WriteLine($"\n⏳ [{projectCount}/{testData.Length}] Filling: {row.Project}");
          await FillTimesheet(page, row.Project, row.Stage, row.Task, row.Days);
          Console.WriteLine("🎯 SUCCESS");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"❌ ERROR: {ex.Message}");
        }

        if (projectCount < testData.Length)
        {
          await Task.Delay(1500); // Delay between rows
        }
      }

      Console.WriteLine("\n✅ All projects completed!");
      Console.WriteLine("\nPress Enter to exit...");
      Console.ReadLine();
    }

    // =============================

    private static async Task EnsureLoggedIn(IPage page)
    {
      Console.WriteLine("🔍 Kiểm tra login...");

      // Cách 1: check URL có login không
      if (page.Url.Contains("login", StringComparison.OrdinalIgnoreCase))
      {
        Console.WriteLine("🔐 Chưa login → chờ bạn login...");

        // Chờ redirect về trang chính
        await page.WaitForURLAsync(url => !url.Contains("login"), new()
        {
          Timeout = 300000 // 5 phút
        });

        Console.WriteLine("✅ Login xong (detect qua URL)");
        return;
      }

      // Cách 2: check element đặc trưng (timesheet grid)
      try
      {
        await page.WaitForSelectorAsync("text=Project", new()
        {
          Timeout = 5000
        });

        Console.WriteLine("✅ Đã login (detect qua UI)");
      }
      catch
      {
        Console.WriteLine("🔐 Có thể chưa login → chờ bạn thao tác...");

        // fallback: chờ user nhấn Enter
        Console.WriteLine("👉 Sau khi login xong, bấm Enter...");
        Console.ReadLine();
      }
    }

    private static async Task ExportHTML(IPage page)
    {
      try
      {
        string html = await page.ContentAsync();
        string outputPath = "timesheet_debug.html";
        File.WriteAllText(outputPath, html);
        Console.WriteLine($"✓ HTML exported to: {Path.GetFullPath(outputPath)}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"❌ Failed to export HTML: {ex.Message}");
      }
    }

    private static async Task FillTimesheet(IPage page, string projectName, string stageName, string taskName, (string Day, string Hours)[] dayHours)
    {
      try
      {
        // 1. ADD ROW
        Console.WriteLine("  ➕ Add row...");
        try
        {
          var addRowBtn = page.Locator("button[aria-label*='Add' i], button[title*='Add' i], button:has(i.fa-plus)");
          int addCount = await addRowBtn.CountAsync();

          if (addCount > 0)
          {
            await addRowBtn.First.ClickAsync();
            Console.WriteLine($"    ✓ Added");
          }
        }
        catch { }

        await page.WaitForTimeoutAsync(1000);

        // 2. PROJECT
        Console.WriteLine($"  📁 Project: {projectName}...");
        var projectSelect = page.Locator("timesheet-project-select").Last;
        var projectInput = projectSelect.Locator("input").Last;

        await projectInput.ClickAsync();
        await page.WaitForTimeoutAsync(300);
        await projectInput.TypeAsync(projectName, new() { Delay = 40 });
        await page.WaitForTimeoutAsync(1200);

        await page.Keyboard.PressAsync("ArrowDown");
        await page.WaitForTimeoutAsync(300);
        await page.Keyboard.PressAsync("Enter");
        Console.WriteLine($"    ✓ Selected");

        await page.WaitForTimeoutAsync(1800);

        // 3. STAGE
        Console.WriteLine($"  📌 Stage: {stageName}...");

        var stageSelect = page.Locator("timesheet-stage-select").Last;
        var stageInput = stageSelect.Locator("input").Last;

        // Tab to next day (or next row)
        await page.Keyboard.PressAsync("Tab");
        await page.WaitForTimeoutAsync(300);
        await stageInput.TypeAsync(stageName, new() { Delay = 40 });
        await page.WaitForTimeoutAsync(1200);

        await page.Keyboard.PressAsync("ArrowDown");
        await page.WaitForTimeoutAsync(150);
        await page.Keyboard.PressAsync("Enter");
        Console.WriteLine($"    ✓ Selected");

        await page.WaitForTimeoutAsync(1800);

        // 4. TASK
        Console.WriteLine($"  📋 Task: {taskName}...");
        var taskSelect = page.Locator("timesheet-task-select").Last;
        var taskInput = taskSelect.Locator("input").Last;

        // Tab to next day (or next row)
        await page.Keyboard.PressAsync("Tab");
        await page.WaitForTimeoutAsync(300);
        await taskInput.TypeAsync(taskName, new() { Delay = 40 });
        await page.WaitForTimeoutAsync(1200);

        await page.Keyboard.PressAsync("ArrowDown");
        await page.WaitForTimeoutAsync(150);
        await page.Keyboard.PressAsync("Enter");
        Console.WriteLine($"    ✓ Selected");

        await page.WaitForTimeoutAsync(1800);

        // 5. HOURS - Navigate to hours fields
        // Column order: Project => Stage => Hours remaining (SKIP) => Task => Rate (SKIP) => Mon => Tue => Wed => Thu => Fri => Sat => Sun
        // After Task, we need to Tab twice to skip: 1) Hours remaining, 2) Rate
        Console.WriteLine($"  ⏱ Hours: ", "");

        // Tab to Hours remaining (skip it)
        await page.Keyboard.PressAsync("Tab");
        await page.WaitForTimeoutAsync(200);


        foreach (var (day, hours) in dayHours)
        {
          // Type hours for this day
          await page.Keyboard.TypeAsync(hours, new() { Delay = 30 });
          Console.Write($"{day}:{hours} ");

          // Tab to next day (or next row)
          await page.Keyboard.PressAsync("Tab");
          await page.WaitForTimeoutAsync(150);
          // Tab to next day (or next row)
          if (hours != "0")
          {
            await page.Keyboard.PressAsync("Tab");
            await page.WaitForTimeoutAsync(150);
          }

        }

        Console.WriteLine("\n    ✅ Done");
      }
      catch (Exception ex)
      {
        throw new Exception($"Error: {ex.Message}", ex);
      }
    }
  }
}

