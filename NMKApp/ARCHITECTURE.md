# NMKApp — Architecture Mind Map

> Sơ đồ tư duy toàn bộ logic ứng dụng. Mỗi nút ghi kèm tên file để dễ tìm kiếm.

---

## 1. Toàn cảnh (Overview Mindmap)

```mermaid
mindmap
  root((NMKApp))
    App.xaml / App.xaml.cs
      DI ConfigureServices — đăng ký tất cả services
      ToastNotificationManagerCompat.OnActivated
      ThemeManager.Apply LightTheme hoặc DarkTheme
    MainWindow.xaml / MainWindow.xaml.cs
      DataContext = MainWindowViewModel
      Tray icon — ẩn/hiện app
      Sidebar RadioButton navigation
      10 Pages hiển thị theo IsChecked của RadioButton
    AppConstants.cs — Core/AppConstants.cs
      SupabaseUrl + SupabaseKey
      MsalClientId + MsalTenantId
      MsalScopes User.Read Mail.Send
      WorkSchedule 8h30-12h30 và 13h30-17h30
      OneDrivePath
```

---

## 2. Auth Flow — Services/AuthService.cs

```mermaid
mindmap
  root((AuthService.cs))
    AuthenticateAsync — trả về email + isLoggedIn
      Bước 1 Registry
        HKCU\\SOFTWARE\\Microsoft\\Office\\16.0\\Outlook\\Profiles
        Đọc email không cần Outlook chạy
      Bước 2 COM Outlook
        Marshal.GetActiveObject — chỉ khi Outlook đang mở
        SmtpAddress từ DefaultAccount
        Không bao giờ mở Outlook mới
      Bước 3 MSAL Silent
        Token cache file: %LocalAppData%\\NMKApp\\MsalCache\\nmkapp_msal.json
        AcquireTokenSilent — không hiện UI
        Lấy email từ Graph /me
      Bước 4 MSAL Interactive
        AcquireTokenInteractive — mở Windows picker
        WithParentActivityOrWindow từ MainWindow handle
      Bước 5 Fallback
        Views/Dialogs/LoginEmailDialog.xaml
        User nhập email thủ công
        Không shutdown app
    GetGraphTokenAsync — static
      Dùng bởi MailService để lấy Bearer token
      Thử MSAL Silent → Interactive
```

---

## 3. Mail Flow — Services/MailService.cs

```mermaid
mindmap
  root((MailService.cs))
    SendEmailAsync — entry point
      Kiểm tra IsComOutlookAvailable
        Marshal.GetActiveObject Outlook.Application
        Nếu true → TrySendViaCOM
        Nếu false → SendViaGraphAsync
    TrySendViaCOM
      GetActiveObject Outlook.Application
      CreateItem + Recipients.Add + Send
      Không launch Outlook mới
      Bắt exception nếu COM lỗi → fallback Graph
    SendViaGraphAsync
      Lấy token qua AuthService.GetGraphTokenAsync
      POST https://graph.microsoft.com/v1.0/me/sendMail
      Body JSON contentType=HTML
    Typed helpers — dựa trên domain params
      SendTaskMailTypedAsync
        Gửi cho assignee khi task mới
        BuildTaskHtml — HTML header xanh
      SendTaskCompleteMailTypedAsync
        Gửi cho leader khi task hoàn thành
        BuildTaskHtml — HTML header xanh
      SendLeaveMailTypedAsync
        Gửi đơn nghỉ phép
        BuildLeaveHtml — HTML header cam
      ApprovalLeaveMailTypedAsync
        Gửi kết quả duyệt nghỉ phép
        Approved=xanh Rejected=đỏ
    IMailService.cs — interface định nghĩa 8 methods
```

---

## 4. Data Layer — Services/SupabaseService.cs + Data/Entities.cs

```mermaid
mindmap
  root((Data Layer))
    NMKApp.Supabase
      URL ondwkhoelyfpzugwyqnd.supabase.co
      Anon key sb_publishable_lkCPpf...
    Services/SupabaseService.cs
      InitializeAsync — khởi tạo client
      GetUsersAsync → List UserModel
      GetProjectsAsync → List ProjectModel
      GetTasksAsync → List TaskModel
      GetNotifysAsync email → List NotifyModel
      GetVersionsAsync → List VersionModel
      GetLeavesAsync → List LeaveModel
      InsertTaskAsync → ServiceResult TaskModel
      UpdateTaskAsync → ServiceResult TaskModel
      DeleteTaskAsync id
      InsertUserAsync → ServiceResult UserModel
      UpdateUserAsync → ServiceResult UserModel
      DeleteUserAsync id
      InsertLeaveAsync → ServiceResult LeaveModel
      UpdateLeaveAsync
      UpdateNotifyAsync
    Data/Entities.cs
      TaskEntity → TaskModel ánh xạ NMK_Task
      UserEntity → UserModel ánh xạ NMK_User
      LeaveEntity → LeaveModel ánh xạ NMK_Leave
      NotifyEntity → NotifyModel ánh xạ NMK_Notify
      ProjectEntity → ProjectModel ánh xạ NMK_Project
      VersionEntity → VersionModel ánh xạ NMK_Version
    Services/RealtimeService.cs
      SubscribeAsync
        onTaskUpdated callback
        onNotifyInserted callback
        onNotifyUpdated callback
        onLeaveUpdated callback
        onVersionInserted callback
      Postgrest Realtime websocket
```

---

## 5. Models — Models/*.cs

```mermaid
mindmap
  root((Models))
    TaskModel.cs
      Id Name OnlyName Folder
      Status 0=Done 1=InProgress 2=Checked 3=Assigned
      DateStart DateEnd CreateAt
      IsAssignedTo IsChecked IsProgress
      Project ProjectModel
      User UserModel
      FileAttachs ObservableCollection FileAttachModel
      TaskChild ObservableCollection TaskModel
      State computed ✓ ⏳ 📋 🔔
      Width Day Index — dùng trong Timeline Gantt
    UserModel.cs
      Id Name Team Email Role RoleEnum
      Color Brush ColorStatus Brush
      ImageString base64
      ProjectIds ObservableCollection string
    LeaveModel.cs
      Id CreateBy SendTo CC Type Reason
      Approval 0=Rejected 1=Approved 2=Pending
      Background SolidColorBrush — màu theo trạng thái
      User UserModel
      LeaveList ObservableCollection DayModel
    NotifyModel.cs
      Id TaskId Title SendTo CreateBy
      Type 0=Task 1=Done 2=Leave 3=Info
      IsRead
      FontWeight Bold nếu chưa đọc
    ProjectModel.cs
      Id Name Key Description RevitVersion
      Color Brush ColorString SolidColorPaint
      Image ImageSource
      Tasks ObservableCollection TaskModel
    VersionModel.cs
      Id Version Data CreateAt
    DayModel.cs
      Name DateTime Holiday Week Month Year
      IsEnabled IsChecked IsVisible
      Leave LeaveModel — ngày nghỉ gắn vào ngày
    ScheduleModel.cs
      Users TasksPerUser cho schedule grid
    RoleVisibleModel.cs
      VisibleUser VisibleAdmin VisibleLeader
      VisibleAdminApp VisibleMiddle VisibleOnlyAdminApp
      Tất cả Visibility — dùng cho sidebar + UI ẩn/hiện
```

---

## 6. ViewModels — ViewModels/*.cs

```mermaid
mindmap
  root((ViewModels))
    MainWindowViewModel.cs — orchestrator
      Collections chia sẻ xuống child VM
        Users UserCollection
        Projects ProjectCollection
        Tasks TaskCollection
        TasksTemporary TaskCollection
        Notifys NotifyCollection
        Leaves LeaveCollection LeaveAssignTo LeaveCollection
      ListCollectionViews
        UsersCollection UsersCollectionRole
        ProjectsCollection
        NotifysCollection NotifysCollectionCount
        LeavesCollection LeavesAssignToCollection LeavesAssignToCollectionCount
      Filters toàn cục
        FilterMonth FilterYear FilterToday
        IsAssignedTo SelectedProject
        FiltersStatus ObservableCollection StatusModel
      LoadCommand — khởi tạo toàn bộ app
        1 AuthenticateAsync
        2 InitializeAsync Supabase
        3 LoadDataAsync
        4 InitializeCollectionViews
        5 SubscribeRealtimeAsync
      RefreshCommand — reload tất cả data
      BackupDataAsync — ghi JSON lên OneDrive
      Realtime handlers
        OnTaskUpdated OnNotifyInserted
        OnNotifyUpdated OnLeaveUpdated
        OnVersionInserted
    DashboardViewModel.cs — Views/DashboardPage.xaml
      TasksProjectCollection grouped by Project
      TaskNewCommand → TaskEditDialog + InsertTaskAsync + SendTaskMailTypedAsync
      TaskEditCommand → TaskEditDialog + UpdateTaskAsync
      TaskDeleteCommand → DeleteTaskAsync
      TaskCompleteCommand → Status=0 + SendTaskCompleteMailTypedAsync + Toast
      TaskStartCommand → Status=1
      TaskCheckedCommand → Status=2
      TaskReCheckedCommand → Status=1 revert
      TaskAcceptCommand → Status=3
      FilesDroppedCommand → xử lý drag-drop files
    TimelineViewModel.cs — Views/TimelinePage.xaml
      TasksUserCollection grouped by User
      DaysWeekCollection
      Days ObservableCollection DayModel
      MinDay MaxDay DatePicker
      FilterDayCommand — tái tạo Days và Width cho task bars
    EmailViewModel.cs — Views/EmailPage.xaml
      TasksEmailCollection Filter Status==3 and IsAssignedTo==false
      TasksEmailCollectionCount — chưa StateAccepted
      SelectAllCommand true/false — toggle IsChecked
      SendCommand — gửi mail cho tất cả task IsChecked
    NotifyViewModel.cs — Views/NotifyPage.xaml
      UnreadCount computed
      ReadCommand notify → UpdateNotifyAsync + IsRead=true
      ReadAllCommand → batch UpdateNotifyAsync tất cả unread
    UserViewModel.cs — Views/UserPage.xaml
      SelectedUser
      AddCommand → UserEditDialog + InsertUserAsync
      EditCommand → UserEditDialog + UpdateUserAsync
      DeleteCommand → MessageBox confirm + DeleteUserAsync
      ImageCommand → OpenFileDialog + base64 + UpdateUserAsync
    LeaveViewModel.cs — Views/LeavePage.xaml
      SelectedLeave IsLoading
      UsersCollectionLeave
      ApplyCommand → LeaveApplyDialog + InsertLeaveAsync + SendLeaveMailTypedAsync
      ApproveCommand → SetApprovalAsync approved=true
      RejectCommand → SetApprovalAsync approved=false
      SetApprovalAsync → UpdateLeaveAsync + ApprovalLeaveMailTypedAsync
    ProjectViewModel.cs — Views/ProjectPage.xaml
      AddCommand → TODO InsertProjectAsync
      EditCommand → TODO UpdateProjectAsync
      DeleteCommand → TODO DeleteProjectAsync
      ColorCommand → TODO chọn màu project
      ImageCommand → TODO chọn ảnh project
    ScheduleViewModel.cs — Views/SchedulePage.xaml
      DaysSchedules ObservableCollection DayModel
      DaysWeekCollectionSchedules
      TasksUserCollectionAdminSchedule
      MinDaySchedules MaxDaySchedules
      ProjectSchedulesAssignTo ScheduleModel
      UsersSchedulesAdmin ScheduleModel
      FilterDayCommand — tạo lại DaysSchedules
      StatusChangeCommand — cập nhật trạng thái ngày
    SettingsViewModel.cs — Views/SettingsPage.xaml
      FileVersionSelectCommand → OpenFileDialog chọn file exe
      FileVersionUploadCommand → upload lên Supabase Storage
      FileVersionUpdateCommand → tải và cài đặt bản mới
    TemporaryViewModel.cs — Views/TemporaryPage.xaml
      TasksTemporaryCollection
      SelectAllCommand toggle IsChecked
      AddTaskCommand → thêm item tạm
      DeleteCommand → xóa item tạm
      SaveCommand → chuyển temporary → tasks thật
```

---

## 7. Views — Views/*.xaml

```mermaid
mindmap
  root((Views))
    MainWindow.xaml
      Sidebar 60px fixed
        RadioButton navigation GroupName=Navigation
        Role-based Visibility từ RoleVisible
        Refresh ToggleButton
      Content Grid Column=1
        10 UserControl pages
        Visibility BoolConverter từ RadioButton.IsChecked
    DashboardPage.xaml — DataContext DashboardViewModel
      Toolbar
        New Task Button → TaskNewCommand
        FiltersStatus CheckBox list
        IsAssignedTo ToggleButton
        FilterMonth ComboBox
        FilterToday CheckBox
      ListView grouped by Project
        GroupStyle header màu Secondary
        Status dot Ellipse màu theo Status 0-3
        Task name + User.Name
        DateStart DateEnd columns
        Project.Key column
        Buttons Start/Done/Check/Edit/✕
        Visibility DataTrigger theo Status và IsAssignedTo
    TimelinePage.xaml — DataContext TimelineViewModel
      Toolbar DatePicker MinDay MaxDay + Apply
      FiltersStatus + IsAssignedTo từ parent Window
      Left panel ListView TasksUserCollection
        Grouped by User
        Status dot + task name 32px height
      Right panel Gantt
        Day headers 50px/day
        Canvas bars Width=Day từ TaskModel
        Bar color theo Status
    EmailPage.xaml — DataContext EmailViewModel
      Toolbar Select All / Deselect All / Send
      ListView TasksEmailCollection
        Checkbox IsChecked binding
        Task name + Project.Name
        User.Name + User.Email
        DateStart DateEnd
      StatusBar đếm TasksEmailCollectionCount.Count
    NotifyPage.xaml — DataContext NotifyViewModel
      Toolbar Mark All Read + UnreadCount
      ListView DataContext.NotifysCollection từ Window
        Unread dot Ellipse ẩn khi IsRead
        Title FontWeight Bold/Normal
        Type badge màu 0-3
        Mark Read Button ẩn khi IsRead
    UserPage.xaml — DataContext UserViewModel
      Toolbar New User + SearchUser TextBox
      ListView DataContext.UsersCollection từ Window
        Avatar Border màu Color + chữ cái đầu
        Name + Email
        Team column
        Role badge màu theo role
        CreateAt
        Buttons 🖼 Edit ✕
    LeavePage.xaml — DataContext LeaveViewModel
      Toolbar Apply Leave
      GridSplitter chia 2 cột
      Left My Leave Requests
        DataContext.LeavesAssignToCollection từ Window
        Type Reason SendTo
        Approval badge Pending/Approved/Rejected
      Right Requests to Approve
        Visibility VisibleLeader
        DataContext.LeavesCollection từ Window
        ✓ Approve / ✕ Reject buttons khi Approval==2
    ProjectPage.xaml — DataContext ProjectViewModel
      Toolbar New Project + SearchProject TextBox
      ListView DataContext.ProjectsCollection từ Window
        Color border + Key text
        Name + Description
        RevitVersion Tasks.Count
        Buttons 🎨 🖼 Edit ✕
    SchedulePage.xaml — DataContext ScheduleViewModel
      Toolbar DatePicker MinDay MaxDay + Apply
      Day header row ItemsControl DaysSchedules
        50px/day ddd dd/MM
        Đỏ nếu IsEnabled=false
      Rows ItemsControl DataContext.UsersCollection từ Window
        Left 160px User label
        Right day cells 60×48px mỗi ô
        Xanh nếu IsChecked Đỏ nếu IsEnabled=false
    SettingsPage.xaml — DataContext SettingsViewModel
      Version Info card 2 cột CurrentVersion LatestVersion
      Update available banner ẩn khi IsVersionUpdate=false
      Buttons Select / Upload / Update Now
      Account card CurrentUser info
    TemporaryPage.xaml — DataContext TemporaryViewModel
      Toolbar New Item / Select All / Deselect All
      ListView TasksTemporaryCollection
        Checkbox IsChecked
        OnlyName + Name
        DateStart DateEnd User.Name
        Delete ✕ button
      Bottom bar Save to Tasks + count
    Dialogs/TaskEditDialog.xaml
      Title input
      Description TextBox multiline
      Project ComboBox
      User ComboBox
      DateStart DateEnd DatePicker
      OK / Cancel
    Dialogs/UserEditDialog.xaml
      Name Email inputs
      Role ComboBox user/leader/admin/adminapp
      Team input
    Dialogs/LeaveApplyDialog.xaml
      SendTo CC inputs
      LeaveType ComboBox
      Reason TextBox
      LeaveDays number input
    Dialogs/LoginEmailDialog.xaml
      Email input validation contains @
      OK / Cancel
```

---

## 8. Services & DI — App.xaml.cs

```mermaid
mindmap
  root((DI — App.xaml.cs ConfigureServices))
    Singleton
      IAuthService → AuthService.cs
      ISupabaseService → SupabaseService.cs
      IRealtimeService → RealtimeService.cs
      IBackupService → BackupService.cs
      INavigationService → NavigationService.cs
      IMailService → MailService.cs
      IToastService → ToastService.cs
    Transient
      MainWindowViewModel.cs
      DashboardViewModel.cs
      TimelineViewModel.cs
      EmailViewModel.cs
      UserViewModel.cs
      ProjectViewModel.cs
      LeaveViewModel.cs
      NotifyViewModel.cs
      ScheduleViewModel.cs
      SettingsViewModel.cs
      TemporaryViewModel.cs
    Toast init
      ToastService.Initialize
      Tạo shortcut Start Menu nếu chưa có
    MainWindow
      Resolve MainWindow từ DI container
      Set DataContext = MainWindowViewModel
```

---

## 9. UITheme — UITheme/*.xaml

```mermaid
mindmap
  root((UITheme))
    BaseTheme.xaml — merge tất cả resource dicts
    LightTheme.xaml
      PrimaryBackgroundBrush trắng
      SecondaryBackgroundBrush xám nhạt
      PrimaryForegroundBrush tối
      PrimaryBorderBrush xám
    DarkTheme.xaml
      PrimaryBackgroundBrush tối
      SecondaryBackgroundBrush tối hơn
    Controls/Button.xaml
      ButtonPrimaryStyle xanh #1565c0
      ButtonSecondaryStyle border theme
      ButtonSuccessStyle xanh lá #388E3C
      ButtonDangerStyle đỏ #C62828
      ButtonWarningStyle cam #E65100
      CornerRadius=4 hover/press opacity
    Controls/CheckBox.xaml
    Controls/ProgressBar.xaml
    Controls/TextBlock.xaml
    Controls/ToggleButton.xaml
      BaseToggleButton
      ToggleButtonVertical
    Controls/RadioButton.xaml
      RadioButtonIconContentVerticalStyle sidebar nav
    Controls/TextBox.xaml
    Controls/Dialog.xaml
    Controls/DataGrid.xaml DataGridItem.xaml
    Controls/ListBox.xaml ListBoxItem.xaml
    Controls/Combobox.xaml ComboboxItem.xaml
    Controls/ScrollViewer.xaml
    Controls/Tooltip.xaml
    Controls/TreeView.xaml TreeViewItem.xaml
    Converter.xaml
      BoolConverter bool→Visibility
      NullConverter null→Visibility
      CountToVisibility
      ObjectNullToVisibility
      MultiBoolToBool
      MultiBoolToDouble
      BoolToDouble
      GroupDescriptionIndexer
    AttachedBehaviors.cs
      UI.Icon attached property
      UI.IconFalse UI.IconTrue
```

---

## 10. Auth Decision Tree (text diagram)

```
Khởi động app
     │
     ▼ AuthService.AuthenticateAsync()
┌─────────────────────────────┐
│  Bước 1: Windows Registry   │ ──▶ tìm Outlook Profiles email
│  HKCU\Office\16.0\Outlook   │
└──────────────┬──────────────┘
               │ Không tìm thấy?
               ▼
┌─────────────────────────────┐
│  Bước 2: COM Outlook        │ ──▶ GetActiveObject (chỉ nếu đang chạy)
│  Marshal.GetActiveObject    │
└──────────────┬──────────────┘
               │ Không chạy?
               ▼
┌─────────────────────────────┐
│  Bước 3: MSAL Silent        │ ──▶ Token từ cache file JSON
│  AcquireTokenSilent         │     → GET /me → lấy email
└──────────────┬──────────────┘
               │ Không có cache?
               ▼
┌─────────────────────────────┐
│  Bước 4: MSAL Interactive   │ ──▶ Windows account picker
│  AcquireTokenInteractive    │     User chọn account
└──────────────┬──────────────┘
               │ User bỏ qua / lỗi?
               ▼
┌─────────────────────────────┐
│  Bước 5: LoginEmailDialog   │ ──▶ User nhập email thủ công
│  Views/Dialogs/...xaml      │     Validate @
└──────────────┬──────────────┘
               │ User cancel?
               ▼
        Application.Shutdown()
```

---

## 11. Mail Routing (text diagram)

```
MailService.SendEmailAsync(to, subject, html)
     │
     ├─ IsComOutlookAvailable?
     │   Marshal.GetActiveObject("Outlook.Application")
     │   │
     │   ├─ YES ──▶ TrySendViaCOM()
     │   │           CreateItem(0) + Recipients.Add(to)
     │   │           + HTMLBody = html + Send()
     │   │           Không cần token, không mở Outlook mới
     │   │
     │   └─ NO ──▶ GetGraphTokenAsync()
     │               MSAL silent → AcquireTokenInteractive
     │               │
     │               ├─ OK ──▶ POST graph.microsoft.com/v1.0/me/sendMail
     │               │          Authorization: Bearer {token}
     │               │          Body: { message: { toRecipients, subject,
     │               │                   body: { contentType: "HTML", content } } }
     │               │
     │               └─ Fail ──▶ Debug.WriteLine (bỏ qua nhẹ nhàng)
```

---

## 12. Realtime Flow — Services/RealtimeService.cs

```
Supabase Realtime WebSocket
     │
     ├─ Table: NMK_Task (UPDATE)
     │   → onTaskUpdated(TaskModel)
     │   → MainWindowViewModel.OnTaskUpdated
     │   → RefreshAllViews()
     │
     ├─ Table: NMK_Notify (INSERT)
     │   → onNotifyInserted(NotifyModel)
     │   → kiểm tra SendTo == CurrentUser.Email
     │   → Notifys.Items.Add + RefreshAllViews
     │   → ToastService.ShowNewNotification
     │
     ├─ Table: NMK_Notify (UPDATE)
     │   → onNotifyUpdated(NotifyModel)
     │   → cập nhật IsRead của existing notify
     │
     ├─ Table: NMK_Leave (UPDATE)
     │   → onLeaveUpdated(LeaveModel)
     │   → RefreshAllViews
     │
     └─ Table: NMK_Version (INSERT)
         → onVersionInserted(VersionModel)
         → DialogMessage hiển thị "New Version Available"
```


    Auth Flow
      AuthService.cs
        1 Registry
          HKCU Office 16 Outlook Profiles
        2 COM Outlook
          GetActiveObject
          Only if already running
        3 MSAL Silent
          Token cache disk
          AcquireTokenSilent
        4 MSAL Interactive
          WithParentActivityOrWindow
          Windows account picker
        5 Fallback dialog
          LoginEmailDialog.xaml

    Data Layer
      Supabase
        NMKApp.Supabase v1.1.1
        URL ondwkhoelyfpzugwyqnd.supabase.co
        Tables
          NMK_Version
          NMK_User
          NMK_Task
          NMK_Project
          NMK_Notify
          NMK_Leave
          NMK_Task_Backup
      SupabaseService.cs
        InitializeAsync
        CRUD per entity
        GetTasksByIdAsync
      RealtimeService.cs
        SubscribeAsync
        onTaskUpdated
        onNotifyInserted
        onLeaveUpdated
        onVersionInserted

    Mail
      MailService.cs
        IsOutlookAvailable COM
        Route COM first
        Fallback Graph API
        COM via GetActiveObject
        Graph via sendMail endpoint
        HTML builders
          BuildTaskHtml
          BuildLeaveHtml
      IMailService.cs
        SendTaskMailAsync raw
        SendTaskMailTypedAsync domain
        SendLeaveMailAsync raw
        SendLeaveMailTypedAsync domain
        ApprovalLeaveMailTypedAsync
      AuthService.GetGraphTokenAsync
        Provides Bearer token

    ViewModels
      MainWindowViewModel
        LoadCommand
          Authenticate
          InitializeAsync Supabase
          LoadDataAsync
          InitializeCollectionViews
          SubscribeRealtime
        BackupDataAsync
        RefreshCommand
        Child VMs
          DashboardVM
          TimelineVM
          EmailVM
          UserVM
          ProjectVM
          NotifyVM
          LeaveVM
          ScheduleVM
          SettingsVM
          TemporaryVM
        Collections
          Users UserModel
          Projects ProjectModel
          Tasks TaskModel
          Notifys NotifyModel
          Leaves LeaveModel
        Filters
          FilterMonth FilterYear
          FilterToday
          IsAssignedTo
          SelectedProject
          FiltersStatus

      DashboardViewModel
        TasksProjectCollection
          Grouped by Project
          Filtered by status month today
        TaskNewCommand
        TaskEditCommand
        TaskDeleteCommand
        TaskCompleteCommand status 0
        TaskStartCommand status 1
        TaskCheckedCommand status 2
        TaskReCheckedCommand revert to 1
        TaskAcceptCommand status 3
        FilesDroppedCommand

      LeaveViewModel
        ApplyCommand
          InsertLeaveAsync
          SendLeaveMailTypedAsync
        ApproveCommand
        RejectCommand
        ApprovalLeaveMailTypedAsync

      NotifyViewModel
        ReadCommand
        ReadAllCommand
        UpdateNotifyAsync mark read

      UserViewModel
        AddCommand
        EditCommand
        DeleteCommand
        ImageCommand base64

      EmailViewModel
        TasksEmailCollection
          Status 3 and not assignedTo
        SelectAllCommand
        SendCommand
          SendTaskMailTypedAsync

    Views
      MainWindow.xaml
        Sidebar RadioButton nav
        Role-based visibility
        Page Content Grid
        Taskbar overlay
      DashboardPage.xaml
        Toolbar filters
        ListView grouped by Project
        Per-task action buttons
          Start Done Check Edit Delete
        Status color indicator
      Dialogs
        TaskEditDialog new and edit
        UserEditDialog new and edit
        LeaveApplyDialog
        LoginEmailDialog fallback

    Models
      TaskModel
        Status 0 Complete 1 InProgress 2 Checked 3 Assigned
        IsAssignedTo
        FileAttachs
        TaskChild
      UserModel
        RoleEnum RoleType
        ImageString base64
      LeaveModel
        LeaveList ObservableCollection DayModel
        Approval 0 Rejected 1 Approved 2 Pending
      NotifyModel
        IsRead
        Type Status
      ProjectModel
        Key Color

    Services
      BackupService
        BackupAsync
        OneDrive path
        JSON files
      ToastService
        Initialize shortcut
        Show title message taskId
        ShowTaskComplete
        ShowNewNotification
      NavigationService
        Navigate CurrentPage
      ISupabaseService
        Full CRUD all entities

    UITheme
      BaseTheme.xaml merges Controls
      Controls
        Button.xaml Primary Secondary Success Danger Warning
        ToggleButton.xaml
        RadioButton.xaml nav sidebar
        Dialog.xaml templates
      LightTheme.xaml
      DarkTheme.xaml
      ThemeManager switch runtime
      Converters BoolConverter NullConverter etc

    Constants AppConstants.cs
      SupabaseUrl
      SupabaseKey
      MsalClientId 30a2d671
      MsalTenantId common
      MsalScopes User.Read Mail.Send
      Roles adminapp admin leader user
      WorkSchedule 8:30 12:30 13:30 17:30
      OneDrivePath
```

## Layer Map

```
┌────────────────────────────────────────────────┐
│                    Views (WPF)                  │
│  MainWindow · DashboardPage · 9 pages · Dialogs│
└────────────────┬───────────────────────────────┘
                 │ DataContext binding
┌────────────────▼───────────────────────────────┐
│               ViewModels (MVVM)                 │
│  MainWindowVM · DashboardVM · Email/Notify/...  │
└────────────────┬───────────────────────────────┘
                 │ DI injected
┌────────────────▼───────────────────────────────┐
│                Services (DI)                    │
│  AuthService · MailService · SupabaseService    │
│  ToastService · BackupService · RealtimeService │
└────────────────┬───────────────────────────────┘
                 │
┌────────────────▼───────────────────────────────┐
│          Data / External                         │
│  Supabase (PostgreSQL) · Microsoft Graph API    │
│  Windows Registry · COM Outlook · Toast WNS     │
└────────────────────────────────────────────────┘
```

## Auth Decision Tree

```
Start
  │
  ▼
Registry (HKCU\Office\16.0\Outlook\Profiles)
  │ found?
  ├─ YES ──► use email ──► LoadData
  │
  ▼ NO
COM Outlook (GetActiveObject — never launches new process)
  │ running + logged in?
  ├─ YES ──► use SmtpAddress ──► LoadData
  │
  ▼ NO
MSAL cached token (disk cache)
  │ cached account?
  ├─ YES ──► AcquireTokenSilent ──► fetch email via Graph ──► LoadData
  │
  ▼ NO
MSAL Interactive (Windows account picker)
  │ user selects account?
  ├─ YES ──► AcquireTokenInteractive ──► LoadData
  │
  ▼ NO
LoginEmailDialog (manual input)
  │ user enters email?
  ├─ YES ──► LoadData (limited — no mail sending)
  │
  ▼ NO
  Application.Shutdown()
```

## Mail Routing

```
sendMail(to, subject, htmlBody)
  │
  ├─ IsComOutlookAvailable?
  │   YES ──► GetActiveObject + CreateItem + Send (no auth needed)
  │
  └─ NO ──► GetGraphTokenAsync (MSAL silent)
              │
              ├─ token? ──► POST /me/sendMail (Graph API)
              │
              └─ no token ──► Debug.WriteLine (skip silently)
```
