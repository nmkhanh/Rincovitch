//using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using Autodesk.Windows;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;
//using TaskDialog = Autodesk.Revit.UI.TaskDialog;
//using TaskDialogCommonButtons = Autodesk.Revit.UI.TaskDialogCommonButtons;
//namespace RincovitchTools
//{
//  using Revit.Async;
//  using System;
//  using System.Collections.Generic;
//  using System.IO;
//  using System.Threading;
//  using System.Threading.Tasks;
//  using System.Windows;
//  using System.Windows.Media;
//  using System.Windows.Media.Imaging;
//  using System.Windows.Threading;

//  public static class PushButtonGifDecoder
//  {
//    // Trả về list các frame đã composited hoàn chỉnh (đã Freeze) kèm delay (ms),
//    // và đã scale về targetSize x targetSize (square).
//    // Decode và compositing thực hiện trên một thread STA riêng.
//    public static Task<List<(BitmapSource Frame, int DelayMs)>> DecodeGifFramesScaledAsync(string gifPath, int targetSize, CancellationToken token)
//    {
//      var tcs = new TaskCompletionSource<List<(BitmapSource, int)>>();

//      Thread sta = new Thread(() =>
//      {
//        try
//        {
//          if (token.IsCancellationRequested)
//          {
//            tcs.SetCanceled();
//            return;
//          }

//          List<(BitmapSource, int)> result = DecodeAndScale(gifPath, targetSize);
//          tcs.SetResult(result);
//        }
//        catch (OperationCanceledException)
//        {
//          tcs.SetCanceled();
//        }
//        catch (Exception ex)
//        {
//          tcs.SetException(ex);
//        }
//        finally
//        {
//          // Stop the dispatcher for this STA thread if running
//          try
//          {
//            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
//          }
//          catch { }
//        }
//      });

//      sta.SetApartmentState(ApartmentState.STA);
//      sta.IsBackground = true;
//      sta.Start();

//      return tcs.Task;
//    }

//    // Nội bộ: chạy trên STA thread
//    private static List<(BitmapSource, int)> DecodeAndScale(string gifPath, int targetSize)
//    {
//      var list = new List<(BitmapSource, int)>();

//      using (var fs = File.OpenRead(gifPath))
//      {
//        var decoder = new GifBitmapDecoder(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
//        if (decoder.Frames.Count == 0)
//          return list;

//        int canvasW = decoder.Frames[0].PixelWidth;
//        int canvasH = decoder.Frames[0].PixelHeight;

//        // init current composite as transparent
//        RenderTargetBitmap currentComposite = new RenderTargetBitmap(canvasW, canvasH, 96, 96, PixelFormats.Pbgra32);
//        {
//          var dvInit = new DrawingVisual();
//          using (var dc = dvInit.RenderOpen())
//          {
//            dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, canvasW, canvasH));
//          }
//          currentComposite.Render(dvInit);
//        }

//        BitmapSource backupComposite = null;

//        for (int i = 0; i < decoder.Frames.Count; ++i)
//        {
//          var frame = decoder.Frames[i];
//          var meta = frame.Metadata as BitmapMetadata;

//          // delay (1/100 sec)
//          int delayMs = 100;
//          try
//          {
//            object q = meta?.GetQuery("/grctlext/Delay");
//            if (q != null)
//            {
//              ushort d = (ushort)q;
//              delayMs = Math.Max(10, d * 10);
//            }
//          }
//          catch { }

//          // disposal
//          byte disposal = 0;
//          try
//          {
//            object q = meta?.GetQuery("/grctlext/Disposal");
//            if (q != null)
//              disposal = (byte)q;
//          }
//          catch { }

//          // frame position and size
//          int left = 0, top = 0, fw = frame.PixelWidth, fh = frame.PixelHeight;
//          try
//          {
//            object qL = meta?.GetQuery("/imgdesc/Left");
//            object qT = meta?.GetQuery("/imgdesc/Top");
//            object qW = meta?.GetQuery("/imgdesc/Width");
//            object qH = meta?.GetQuery("/imgdesc/Height");
//            if (qL != null)
//              left = (int)(ushort)qL;
//            if (qT != null)
//              top = (int)(ushort)qT;
//            if (qW != null)
//              fw = (int)(ushort)qW;
//            if (qH != null)
//              fh = (int)(ushort)qH;
//          }
//          catch { /* ignore */ }

//          // if disposal == 3 (restore to previous), backup current composite
//          if (disposal == 3)
//          {
//            var copy = new RenderTargetBitmap(canvasW, canvasH, 96, 96, PixelFormats.Pbgra32);
//            var dvCopy = new DrawingVisual();
//            using (var dc = dvCopy.RenderOpen())
//            {
//              dc.DrawImage(currentComposite, new Rect(0, 0, canvasW, canvasH));
//            }
//            copy.Render(dvCopy);
//            backupComposite = copy.Clone();
//            if (backupComposite.CanFreeze)
//              backupComposite.Freeze();
//          }

//          // compose currentComposite + frame at (left, top)
//          var dv = new DrawingVisual();
//          using (var dc = dv.RenderOpen())
//          {
//            dc.DrawImage(currentComposite, new Rect(0, 0, canvasW, canvasH));
//            dc.DrawImage(frame, new Rect(left, top, fw, fh));
//          }
//          var composed = new RenderTargetBitmap(canvasW, canvasH, 96, 96, PixelFormats.Pbgra32);
//          composed.Render(dv);
//          if (composed.CanFreeze)
//            composed.Freeze();

//          // scale to targetSize x targetSize preserving aspect by letterbox (fit inside)
//          var scaled = ScaleBitmapSourceToSquare(composed, targetSize);
//          if (scaled.CanFreeze)
//            scaled.Freeze();

//          list.Add((scaled, delayMs));

//          // apply disposal rules to update currentComposite for next frame
//          if (disposal == 2)
//          {
//            // restore to background (clear rect)
//            var dv2 = new DrawingVisual();
//            using (var dc = dv2.RenderOpen())
//            {
//              dc.DrawImage(currentComposite, new Rect(0, 0, canvasW, canvasH));
//              dc.DrawRectangle(Brushes.Transparent, null, new Rect(left, top, fw, fh));
//            }
//            var next = new RenderTargetBitmap(canvasW, canvasH, 96, 96, PixelFormats.Pbgra32);
//            next.Render(dv2);
//            currentComposite = next;
//          }
//          else if (disposal == 3)
//          {
//            if (backupComposite != null)
//            {
//              var dv2 = new DrawingVisual();
//              using (var dc = dv2.RenderOpen())
//              {
//                dc.DrawImage(backupComposite, new Rect(0, 0, canvasW, canvasH));
//              }
//              var next = new RenderTargetBitmap(canvasW, canvasH, 96, 96, PixelFormats.Pbgra32);
//              next.Render(dv2);
//              currentComposite = next;
//            }
//            // else leave currentComposite as is
//          }
//          else
//          {
//            // disposal 0 or 1: keep composed as currentComposite
//            currentComposite = composed;
//          }
//        } // for frames
//      } // using fs

//      return list;
//    }

//    // Scale a BitmapSource into a square targetSize x targetSize, center content, preserve aspect
//    private static BitmapSource ScaleBitmapSourceToSquare(BitmapSource src, int targetSize)
//    {
//      double scale = Math.Min((double)targetSize / src.PixelWidth, (double)targetSize / src.PixelHeight);
//      double w = src.PixelWidth * scale;
//      double h = src.PixelHeight * scale;
//      double offsetX = (targetSize - w) / 2.0;
//      double offsetY = (targetSize - h) / 2.0;

//      var dv = new DrawingVisual();
//      using (var dc = dv.RenderOpen())
//      {
//        // Đảm bảo chất lượng ảnh cao nhất khi scale
//        RenderOptions.SetBitmapScalingMode(dv, BitmapScalingMode.HighQuality);
//        dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, targetSize, targetSize));
//        dc.DrawImage(src, new Rect(offsetX, offsetY, w, h));
//      }

//      var target = new RenderTargetBitmap(targetSize, targetSize, 96, 96, PixelFormats.Pbgra32);
//      target.Render(dv);
//      return target;
//    }
//  }
//  public class PushButtonGifAnimatorOptimized : IDisposable
//  {
//    private readonly PushButton _pushButton;
//    private readonly Dispatcher _uiDispatcher;
//    private System.Threading.Timer _timer;
//    private List<(System.Windows.Media.Imaging.BitmapSource Frame, int DelayMs)> _frames;
//    private int _index = 0;
//    private int _isUpdating = 0;
//    private CancellationTokenSource _cts;
//    private volatile bool _loaded = false;
//    private readonly int _minDelayMs = 1; // enforce minimum delay to avoid too fast scheduling

//    // Constructor starts decoding on STA thread asynchronously.
//    // gifPath: path to GIF file
//    // initialIntervalMs: fallback interval until first frame is ready (not critical)
//    // targetSize: pixel size for icon (e.g., 32)
//    public PushButtonGifAnimatorOptimized(PushButton pushButton, string gifPath, int initialIntervalMs = 30, int targetSize = 32)
//    {
//      _pushButton = pushButton ?? throw new ArgumentNullException(nameof(pushButton));
//      _uiDispatcher = Dispatcher.CurrentDispatcher;
//      _cts = new CancellationTokenSource();

//      // start decode on STA thread; when done, start timer scheduling by per-frame delays
//      Task.Run(async () =>
//      {
//        try
//        {
//          var frames = await PushButtonGifDecoder.DecodeGifFramesScaledAsync(gifPath, targetSize, _cts.Token).ConfigureAwait(false);
//          if (frames == null || frames.Count == 0)
//            return;

//          // copy into local list
//          _frames = frames;

//          // mark loaded & set initial image on UI thread
//          await _uiDispatcher.BeginInvoke(new Action(() =>
//          {
//            try
//            {
//              _pushButton.LargeImage = _frames[0].Frame;
//              // Thủ thuật ép Ribbon vẽ lại ngay lập tức
//              if (Autodesk.Windows.ComponentManager.Ribbon != null)
//              {
//                Autodesk.Windows.ComponentManager.Ribbon.UpdateLayout();
//              }
//              Interlocked.Exchange(ref _isUpdating, 0);
//            }
//            catch { }
//          }), DispatcherPriority.Render);

//          _loaded = true;

//          // start background timer scheduling the first transition after frames[0].DelayMs
//          int firstDelay = Math.Max(_minDelayMs, _frames[0].DelayMs);
//          _timer = new System.Threading.Timer(TimerCallback, null, firstDelay, Timeout.Infinite);
//        }
//        catch (OperationCanceledException) { /* cancelled */ }
//        catch (Exception ex)
//        {
//          System.Diagnostics.Debug.WriteLine("Animator decode/start error: " + ex);
//        }
//      });
//    }

//    // Timer callback runs on ThreadPool thread
//    private void TimerCallback(object state)
//    {
//      if (!_loaded || _frames == null || _frames.Count == 0)
//        return;

//      // guard reentrancy: if already invoking UI update, skip this tick
//      if (Interlocked.Exchange(ref _isUpdating, 1) == 1)
//        return;

//      try
//      {
//        // advance index
//        _index = (_index + 1) % _frames.Count;
//        var frame = _frames[_index];

//        // marshal UI update; when UI update completes, schedule next timer with next frame delay
//        _uiDispatcher.BeginInvoke(new Action(() =>
//        {
//          try
//          {
//            _pushButton.LargeImage = frame.Frame;
//          }
//          catch { /* ignore */ }
//          finally
//          {
//            Interlocked.Exchange(ref _isUpdating, 0);

//            // schedule next tick from threadpool (timer.Change)
//            int nextDelay = Math.Max(_minDelayMs, _frames[_index].DelayMs);
//            try
//            {
//              _timer?.Change(nextDelay, Timeout.Infinite);
//            }
//            catch { /* timer disposed possibly */ }
//          }
//        }), DispatcherPriority.Render);
//      }
//      catch
//      {
//        Interlocked.Exchange(ref _isUpdating, 0);
//      }
//    }

//    public void Pause()
//    {
//      try
//      {
//        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
//      }
//      catch { }
//    }

//    public void Resume()
//    {
//      if (!_loaded || _frames == null || _frames.Count == 0)
//        return;
//      int nextDelay = Math.Max(_minDelayMs, _frames[_index].DelayMs);
//      try
//      {
//        _timer?.Change(nextDelay, Timeout.Infinite);
//      }
//      catch { }
//    }

//    public void Dispose()
//    {
//      try
//      {
//        _cts?.Cancel();
//      }
//      catch { }
//      _cts?.Dispose();
//      _timer?.Dispose();
//      _timer = null;
//      _frames?.Clear();
//      _frames = null;
//    }
//  }
//}
 