using Newtonsoft.Json;
using System;
using System.Management;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RincovitchTools._00_General
{
  public static class HardwareIdHelper
  {
    public static string GetHWID()
    {
      string cpu = GetCPUId();
      string bios = GetBiosSerial();
      string board = GetMainboardSerial();

      string raw = $"{cpu}|{bios}|{board}";
      return ToSHA256(raw);
    }

    // =========================
    // CPU ID
    // =========================
    public static string GetCPUId()
    {
      try
      {
        using var searcher = new ManagementObjectSearcher(
            "SELECT ProcessorId FROM Win32_Processor");

        foreach (ManagementObject obj in searcher.Get())
          return obj["ProcessorId"]?.ToString()?.Trim() ?? "UNKNOWN_CPU";
      }
      catch { }

      return "UNKNOWN_CPU";
    }

    // =========================
    // BIOS Serial
    // =========================
    public static string GetBiosSerial()
    {
      try
      {
        using var searcher = new ManagementObjectSearcher(
            "SELECT SerialNumber FROM Win32_BIOS");

        foreach (ManagementObject obj in searcher.Get())
          return obj["SerialNumber"]?.ToString()?.Trim() ?? "UNKNOWN_BIOS";
      }
      catch { }

      return "UNKNOWN_BIOS";
    }

    // =========================
    // Mainboard Serial
    // =========================
    public static string GetMainboardSerial()
    {
      try
      {
        using var searcher = new ManagementObjectSearcher(
            "SELECT SerialNumber FROM Win32_BaseBoard");

        foreach (ManagementObject obj in searcher.Get())
          return obj["SerialNumber"]?.ToString()?.Trim() ?? "UNKNOWN_BOARD";
      }
      catch { }

      return "UNKNOWN_BOARD";
    }

    // =========================
    // SHA256 Hash
    // =========================
    private static string ToSHA256(string input)
    {
      using var sha = SHA256.Create();
      byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

      var sb = new StringBuilder();
      foreach (byte b in bytes)
        sb.Append(b.ToString("x2"));

      return sb.ToString();
    }
  }

  #region Models
  public class User : BaseViewModel
  {
    public string id { get; set; }
    public DateTime created_at { get; set; }
    public string user_name { get; set; }
  }
  #endregion

  public static class F_CheckTime
  {
    static readonly string SupabaseUrl = "https://ffbszfswcppvdhnmsqkd.supabase.co";
    static readonly string ApiKey = "sb_publishable_D2YDfYEZCww-pQFavtV1Xg_TG29l-l3";
    static HttpClient CreateClient()
    {
      var client = new HttpClient();
      client.DefaultRequestHeaders.Add("apikey", ApiKey);
      client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
      return client;
    }

    public static async Task<List<User>> GetUserByIdAsync()
    {
      try
      {
        using (var client = CreateClient())
        {
          var url = $"{SupabaseUrl}/rest/v1/users?id=eq.{HardwareIdHelper.GetHWID()}&select=*";

          var response = await client.GetAsync(url);
          if (response.IsSuccessStatusCode)
          {
            string result = await response.Content.ReadAsStringAsync();
            var result_obj = JsonConvert.DeserializeObject<List<User>>(result);

            return result_obj;
          }
          else
          {
            return null;
          }
        }
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    public static async Task InsertUserAsync()
    {
      try
      {
        using (var client = CreateClient())
        {
          var url = $"{SupabaseUrl}/rest/v1/users";

          var json = $@"
        {{
            ""id"": ""{HardwareIdHelper.GetHWID()}"",
            ""user_name"": ""{Environment.UserName}"",
            ""created_at"": ""{DateTime.UtcNow.ToString("o")}""
        }}";

          var content = new StringContent(json, Encoding.UTF8, "application/json");

          var response = await client.PostAsync(url, content);
          response.EnsureSuccessStatusCode();
        }
      }
      catch (Exception ex)
      {

      }
    }
  }
}
