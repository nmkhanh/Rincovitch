using System.Collections.Generic;
using System.Windows;
using System;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using System.Windows.Media;

namespace RincovitchApp.API
{
  public class F_APIGeneral_Color
  {
    public static List<Brush> Generate(bool IsLight, int count = 300)
    {
      var list = new List<Brush>();
      var rand = new Random(12345);

      for (int i = 0; i < count; i++)
      {
        Brush bg;

        if (IsLight)
        {
          // nền tối cho chữ trắng
          bg = RandomDarkColor(rand);
        }
        else
        {
          // nền sáng cho chữ đen
          bg = RandomLightColor(rand);
        }

        list.Add(bg);
      }

      return list;
    }

    // ================= helpers =================

    private static Brush RandomDarkColor(Random rand)
    {
      return new SolidColorBrush(Color.FromRgb(
          (byte)rand.Next(0, 140),
          (byte)rand.Next(0, 140),
          (byte)rand.Next(0, 140)));
    }

    private static Brush RandomLightColor(Random rand)
    {
      return new SolidColorBrush(Color.FromRgb(
          (byte)rand.Next(150, 256),
          (byte)rand.Next(150, 256),
          (byte)rand.Next(150, 256)));
    }
  }
}

