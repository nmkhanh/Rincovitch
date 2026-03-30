using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace RincovitchTools.UITheme
{
  public class AttachedBehavior
  {
    #region DropBehavior
    public static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached("DropCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null, OnDropCommandChanged));

    public static void SetDropCommand(UIElement element, ICommand value) => element.SetValue(DropCommandProperty, value);
    public static ICommand GetDropCommand(UIElement element) => (ICommand)element.GetValue(DropCommandProperty);

    private static void OnDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is UIElement el)
      {
        el.AllowDrop = true;
        el.Drop -= OnDrop;
        el.DragOver -= OnDragOver;

        if (e.NewValue is ICommand)
        {
          el.Drop += OnDrop;
          el.DragOver += OnDragOver;
        }
      }
    }

    private static void OnDragOver(object sender, DragEventArgs e)
    {
      try
      {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
      }
      catch (Exception ex) { Trace.WriteLine($"Behavior DragOver: {ex}"); }
    }

    private static void OnDrop(object sender, DragEventArgs e)
    {
      try
      {
        if (sender is UIElement el)
        {
          var cmd = GetDropCommand(el);
          if (cmd != null && cmd.CanExecute(e))
          {
            cmd.Execute(e.Data.GetData(DataFormats.FileDrop));
          }
        }
        e.Handled = true;
      }
      catch (Exception ex) { Trace.WriteLine($"Behavior Drop: {ex}"); }
    }
    #endregion

    public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.RegisterAttached("ClickCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetClickCommand(DependencyObject element, ICommand value) => element.SetValue(ClickCommandProperty, value);
    public static ICommand GetClickCommand(DependencyObject element) => (ICommand)element.GetValue(ClickCommandProperty);

    public static readonly DependencyProperty ExpandedCommandProperty = DependencyProperty.RegisterAttached("ExpandedCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetExpandedCommand(DependencyObject element, ICommand value) => element.SetValue(ExpandedCommandProperty, value);
    public static ICommand GetExpandedCommand(DependencyObject element) => (ICommand)element.GetValue(ExpandedCommandProperty);

    #region ContextMenuCommands
    // Provide attached ICommand properties for context menu actions so view/XAML can bind commands easily.
    public static readonly DependencyProperty NewFolderCommandProperty = DependencyProperty.RegisterAttached("NewFolderCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetNewFolderCommand(DependencyObject element, ICommand value) => element.SetValue(NewFolderCommandProperty, value);
    public static ICommand GetNewFolderCommand(DependencyObject element) => (ICommand)element.GetValue(NewFolderCommandProperty);

    public static readonly DependencyProperty DownloadCommandProperty = DependencyProperty.RegisterAttached("DownloadCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetDownloadCommand(DependencyObject element, ICommand value) => element.SetValue(DownloadCommandProperty, value);
    public static ICommand GetDownloadCommand(DependencyObject element) => (ICommand)element.GetValue(DownloadCommandProperty);

    public static readonly DependencyProperty MoveCommandProperty = DependencyProperty.RegisterAttached("MoveCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetMoveCommand(DependencyObject element, ICommand value) => element.SetValue(MoveCommandProperty, value);
    public static ICommand GetMoveCommand(DependencyObject element) => (ICommand)element.GetValue(MoveCommandProperty);

    public static readonly DependencyProperty ShareCommandProperty = DependencyProperty.RegisterAttached("ShareCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetShareCommand(DependencyObject element, ICommand value) => element.SetValue(ShareCommandProperty, value);
    public static ICommand GetShareCommand(DependencyObject element) => (ICommand)element.GetValue(ShareCommandProperty);

    public static readonly DependencyProperty RenameCommandProperty = DependencyProperty.RegisterAttached("RenameCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetRenameCommand(DependencyObject element, ICommand value) => element.SetValue(RenameCommandProperty, value);
    public static ICommand GetRenameCommand(DependencyObject element) => (ICommand)element.GetValue(RenameCommandProperty);

    public static readonly DependencyProperty NewVersionCommandProperty = DependencyProperty.RegisterAttached("NewVersionCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetNewVersionCommand(DependencyObject element, ICommand value) => element.SetValue(NewVersionCommandProperty, value);
    public static ICommand GetNewVersionCommand(DependencyObject element) => (ICommand)element.GetValue(NewVersionCommandProperty);

    public static readonly DependencyProperty FavoriteCommandProperty = DependencyProperty.RegisterAttached("FavoriteCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetFavoriteCommand(DependencyObject element, ICommand value) => element.SetValue(FavoriteCommandProperty, value);
    public static ICommand GetFavoriteCommand(DependencyObject element) => (ICommand)element.GetValue(FavoriteCommandProperty);

    public static readonly DependencyProperty LoadToProjectCommandProperty = DependencyProperty.RegisterAttached("LoadToProjectCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetLoadToProjectCommand(DependencyObject element, ICommand value) => element.SetValue(LoadToProjectCommandProperty, value);
    public static ICommand GetLoadToProjectCommand(DependencyObject element) => (ICommand)element.GetValue(LoadToProjectCommandProperty);

    public static readonly DependencyProperty EditFamilyCommandProperty = DependencyProperty.RegisterAttached("EditFamilyCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetEditFamilyCommand(DependencyObject element, ICommand value) => element.SetValue(EditFamilyCommandProperty, value);
    public static ICommand GetEditFamilyCommand(DependencyObject element) => (ICommand)element.GetValue(EditFamilyCommandProperty);

    public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.RegisterAttached("DeleteCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetDeleteCommand(DependencyObject element, ICommand value) => element.SetValue(DeleteCommandProperty, value);
    public static ICommand GetDeleteCommand(DependencyObject element) => (ICommand)element.GetValue(DeleteCommandProperty);

    #endregion

    #region MenuCommands
    // Commands for menu items shown in the image: Help, Accounts, Theme (Dark/Light), Log out
    public static readonly DependencyProperty HelpCommandProperty = DependencyProperty.RegisterAttached("HelpCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetHelpCommand(DependencyObject element, ICommand value) => element.SetValue(HelpCommandProperty, value);
    public static ICommand GetHelpCommand(DependencyObject element) => (ICommand)element.GetValue(HelpCommandProperty);

    public static readonly DependencyProperty AccountsCommandProperty = DependencyProperty.RegisterAttached("AccountsCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetAccountsCommand(DependencyObject element, ICommand value) => element.SetValue(AccountsCommandProperty, value);
    public static ICommand GetAccountsCommand(DependencyObject element) => (ICommand)element.GetValue(AccountsCommandProperty);

    // Theme commands - you can bind either a single ThemeChangedCommand and pass parameter or use specific commands for Dark/Light.
    public static readonly DependencyProperty DarkThemeCommandProperty = DependencyProperty.RegisterAttached("DarkThemeCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetDarkThemeCommand(DependencyObject element, ICommand value) => element.SetValue(DarkThemeCommandProperty, value);
    public static ICommand GetDarkThemeCommand(DependencyObject element) => (ICommand)element.GetValue(DarkThemeCommandProperty);

    public static readonly DependencyProperty LightThemeCommandProperty = DependencyProperty.RegisterAttached("LightThemeCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetLightThemeCommand(DependencyObject element, ICommand value) => element.SetValue(LightThemeCommandProperty, value);
    public static ICommand GetLightThemeCommand(DependencyObject element) => (ICommand)element.GetValue(LightThemeCommandProperty);

    public static readonly DependencyProperty LogoutCommandProperty = DependencyProperty.RegisterAttached("LogoutCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetLogoutCommand(DependencyObject element, ICommand value) => element.SetValue(LogoutCommandProperty, value);
    public static ICommand GetLogoutCommand(DependencyObject element) => (ICommand)element.GetValue(LogoutCommandProperty);
    #endregion

    #region UploadMenuCommands
    // Additional commands shown in second image: Upload from project, Upload settings, Families, Folder
    public static readonly DependencyProperty UploadFromProjectCommandProperty = DependencyProperty.RegisterAttached("UploadFromProjectCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetUploadFromProjectCommand(DependencyObject element, ICommand value) => element.SetValue(UploadFromProjectCommandProperty, value);
    public static ICommand GetUploadFromProjectCommand(DependencyObject element) => (ICommand)element.GetValue(UploadFromProjectCommandProperty);

    public static readonly DependencyProperty UploadSettingsCommandProperty = DependencyProperty.RegisterAttached("UploadSettingsCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetUploadSettingsCommand(DependencyObject element, ICommand value) => element.SetValue(UploadSettingsCommandProperty, value);
    public static ICommand GetUploadSettingsCommand(DependencyObject element) => (ICommand)element.GetValue(UploadSettingsCommandProperty);

    public static readonly DependencyProperty UploadFamiliesCommandProperty = DependencyProperty.RegisterAttached("UploadFamiliesCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetUploadFamiliesCommand(DependencyObject element, ICommand value) => element.SetValue(UploadFamiliesCommandProperty, value);
    public static ICommand GetUploadFamiliesCommand(DependencyObject element) => (ICommand)element.GetValue(UploadFamiliesCommandProperty);

    public static readonly DependencyProperty UploadFolderCommandProperty = DependencyProperty.RegisterAttached("UploadFolderCommand", typeof(ICommand), typeof(AttachedBehavior), new PropertyMetadata(null));
    public static void SetUploadFolderCommand(DependencyObject element, ICommand value) => element.SetValue(UploadFolderCommandProperty, value);
    public static ICommand GetUploadFolderCommand(DependencyObject element) => (ICommand)element.GetValue(UploadFolderCommandProperty);
    #endregion
  }
}
