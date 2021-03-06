﻿using AZMonitoring.Structures.Pages;
using FireSharp.EventStreaming;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static AZMonitoring.statics;

namespace AZMonitoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool fro = false,ch = false,chts = false,ocprovlist = false;
        internal static Views.ChatingPage chatingPage;
        Views.Streaming.StramingMainPage StramingMainPage = new Views.Streaming.StramingMainPage();
        private Views.All_Chats_Page achsp;
        private bool _ShowingDialog;
        private bool _AllowClose;
        Views.Prov_Page provpageobj = new Views.Prov_Page();
        public MainWindow()
        {
            InitializeComponent();
            DB = new DAL.DAL();
            DB.CreateConnection(this);
            statics.staticframe = MainFrameContainer;
            achsp = new Views.All_Chats_Page(this);
            statics.myDH = GODH;
            //DB.test_addChats();
            //DB.Test_addpersons();
            //DB.test_add_positions();
            //DB.Test_addProvinces();
            //DB.test_addGinstruct();
            //DB.test_addinstruct();
            //DB.test_addadministrations();
            //DB.test_addinstituation();
        }
        internal void Initialize_Data_Manage_Pages()
        {
            statics.Data_Mang_Pages = new List<StPages>();
            //int x = statics.LogedPersonPosition.Level;
            /*صلاحيات المدير للأشراف علي مستوي الجمهورية*/ statics.Data_Mang_Pages.Add(new StPages { Page = new Views.SysManage.Prov_manage_Page(), Header = "صفحة إدارة المحافظات" });
            //if (x < 2) { /* صلاحيات مدير الادارة المركزية*/ }
            //if (x < 3) { /* صلاحيات الوكيل الثقافي - الوكيل الشرعي - مدير رعاية الطلاب*/ }
            //if (x < 4) { /* صلاحيات الموجه العام - الموجه الاول*/ }
            //if (x < 5) { /* صلاحيات مسؤلين التوجيه العام*/ }
            //if (x < 6) { /* صلاحيات الموجهين التابعين للادارت التعليمية*/ }
            //if (x < 7) { /* صلاحيات شيخ المعهد*/ }
            //if (x < 8) { /* صلاحيات المعلم في المفهد*/ }

        }
        internal async void Initialize_Prov_Control_List()
        {
            if (statics.LogedPersonPosition != null)
            {
                await Dispatcher.InvokeAsync(async () => {
                    statics.Provinces = new List<Province>();
                    if (statics.LogedPersonPosition.Level == 0)
                    {
                        statics.Provinces.AddRange(await DB.GetAllProvinces());
                    }
                    else { statics.Provinces.Add(await DB.GetProvincebyName(statics.LogedPersonPosition.IDProvince)); }
                    MainListViewProv.ItemsSource = statics.Provinces;
                    MainListViewProv.Items.Refresh();
                });
            }
        }
        internal async void Initialize_Chat(string snap)
        {
            if (statics.LogedPerson != null && statics.DChats != null)
            {
                await Dispatcher.InvokeAsync(async () => {
                    statics.DChats.Add(await DChat.GetDChat(await DB.GetChat(snap)));
                    achsp.Refresh();
                });
            }
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void TXTOpenLastChatBTN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(statics.CurrentChat != null)
            {
                if (!ch)
                {
                    OpenChatFrame();
                }
                else
                {
                    ResetColors();
                    OCFrame(false);
                    fro = chts = ch = false;
                }
            }
        }
        public async void OpenChatFrame(DChat chat = null)
        {
            if(chat != null)
            {
                chatingPage.newChatWindow(chat);
            }
            if (fro)
            {
                OCFrame(false);
                await Task.Run(() => { Thread.Sleep(300); });
            }
            ChatingFrame.Content = chatingPage;
            ResetColors();
            TXTOpenLastChatBTN.Background = Brushes.Gainsboro;
            TXTOpenLastChatBTN.Foreground = Brushes.Teal;
            OCFrame(true);
            fro = ch = true;
            chts = false;
        }
        private void TXTAllChatsBTN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AllChatsBTNMethod();
        }
        async void AllChatsBTNMethod()
        {
            if (!chts)
            {
                if (fro)
                {
                    OCFrame(false);
                    await Task.Run(() => { Thread.Sleep(300); });
                }
                ChatingFrame.Content = achsp;
                ResetColors();
                TXTAllChatsBTN.Background = Brushes.Gainsboro;
                TXTAllChatsBTN.Foreground = Brushes.Teal;
                OCFrame(true);
                fro = chts = true;
                ch = false;
            }
            else
            {
                ResetColors();
                OCFrame(false);
                fro = chts = chts = false;
            }
        }
        public void ResetColors()
        {
            TXTOpenLastChatBTN.Background = Brushes.Teal;
            TXTAllChatsBTN.Background = Brushes.Teal;
            TXTOpenLastChatBTN.Foreground = Brushes.Gainsboro;
            TXTAllChatsBTN.Foreground = Brushes.Gainsboro;
        }
        private void TXTExitBTN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        private void TXTMaxMinBTN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }
        private void TXTMiniBTN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (MessageBox.Show("هل تريد الخروج من النظام الأن ؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RightAlign) != MessageBoxResult.Yes){ e.Cancel = true; }
            if (_AllowClose) return;

            //NB: Because we are making an async call we need to cancel the closing event
            e.Cancel = true;

            //we are already showing the dialog, ignore
            if (_ShowingDialog) return;
            if(await OpenChecking("هل تريد الخروج ؟"))
            {
                if (statics.LogedPerson != null) { await DB.SetOffline(statics.LogedPerson.ID); }
                _AllowClose = true;
                Close();
            }
            
        }
        internal async Task<bool> OpenChecking(string ss, UIElement obj = null)
        {
            TextBlock txt1 = new TextBlock();
            txt1.HorizontalAlignment = HorizontalAlignment.Center;
            txt1.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF53B3B"));
            txt1.Margin = new Thickness(4);
            txt1.TextWrapping = TextWrapping.WrapWithOverflow;
            txt1.FontSize = 18;
            txt1.Text = ss ;

            Button btn1 = new Button();
            Style style = Application.Current.FindResource("MaterialDesignFlatButton") as Style;
            btn1.Style = style;
            btn1.Width = 115;
            btn1.Height = 30;
            btn1.Margin = new Thickness(5);
            btn1.Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            btn1.CommandParameter = true;
            btn1.Content = "نعم";

            Button btn2 = new Button();
            Style style2 = Application.Current.FindResource("MaterialDesignFlatButton") as Style;
            btn2.Style = style2;
            btn2.Width = 115;
            btn2.Height = 30;
            btn2.Margin = new Thickness(5);
            btn2.Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
            btn2.CommandParameter = false;
            btn2.Content = "لا";


            DockPanel dck = new DockPanel();
            dck.Children.Add(btn1);
            dck.Children.Add(btn2);

            StackPanel stk = new StackPanel();
            stk.Width = 250;
            if(obj != null) { stk.Children.Add(obj); }
            stk.Children.Add(txt1);
            stk.Children.Add(dck);
            object result = await OpenDialogHost(stk);
            if (result is bool boolResult && boolResult)
            {
                return true;
            }
            return false;
        }
        private void BTNLoginBTN_Click(object sender, RoutedEventArgs e)
        {
            Logingin();
        }
        void InitiateChats()
        {
            statics.DChats = new List<DChat>();
            DB.SetChatsListener(statics.LogedPerson.ID,AddChat);
            //if(statics.LogedPerson.Chats != null)
            //{
            //    foreach (var item in await DB.GetChats(statics.LogedPerson.Chats))
            //    {
            //        statics.DChats.Add(item);
            //    }
            //    achsp.Refresh();
            //}
        }
        void AddChat(Chat chat)
        {
            Dispatcher.Invoke(async() => {
                statics.DChats.Add(await DChat.GetDChat(chat));
                achsp.Refresh();
            });
        }
        async void Logingin()
        {
            LoginBorder.IsEnabled = false;
            var p = await DB.GetLogedPerson(TXTLoginUsername.Text, TXTLoginPass.Password);
            if (Check.IsChecked.Value)
            {
                Properties.Settings.Default.SaveMe = true;
                Properties.Settings.Default.User = TXTLoginUsername.Text;
                Properties.Settings.Default.Pass = TXTLoginPass.Password;
                Properties.Settings.Default.Save();
            }
            if (p != null)
            {
                //initialize loged person
                statics.LogedPerson = DPerson.GetDPerson(p);
                DataContext = statics.LogedPerson;
                var pp = await DB.GetPositionByID(statics.LogedPerson.IDPosition);
                statics.LogedPersonPosition = pp;
                DB.SetStreamListener(statics.LogedPerson.ID, OpenComingVCSream);
                InitiateChats();
                DB.SetOnline(statics.LogedPerson.ID);
                //initialize components
                Initialize_Prov_Control_List();
                Initialize_Data_Manage_Pages();

                chatingPage = new Views.ChatingPage(this);

                MainLoginPanel.IsEnabled = false;
                MainLoginPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(300, 1, 0));
                await Task.Run(() => { Thread.Sleep(300); });
                MainLoginPanel.Visibility = Visibility.Hidden;
                MainDockPanel.Visibility = Visibility;
                MainDockPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(300, 0, 1));
                await Task.Run(() => { Thread.Sleep(300); });
                MainDockPanel.IsEnabled = true;
                TXTLoginUsername.Text = TXTLoginPass.Password = "";
                //MainFrameContainer.Content = new Views.Position_Person_Page(statics.LogedPerson);
                var x = new Views.instituation.InstitutionPage();
                x.ini(new Institution { ID = "id", ClassesCount = 33, Description = "Descrip", IDAdministration = "Admin", Name = "Name", SheikhID = "kLd1to8mHbddDFiFVisu8Q7279g1", Stage = Stages.PreparatoryStage, StudentsCount = 344, Type = Type.Private });
                MainFrameContainer.Content = x;

                GC.Collect();
            }
            else
            {
                MessageBox.Show($"فشل في تسجيل الدخول", "فشل", MessageBoxButton.OK, MessageBoxImage.Error);
                MainLoginPanel.IsEnabled = true;
                LoginBorder.IsEnabled = true;
            }
        }
        async Task MainDockVisibility(bool openorclose, int d = 400)
        {
            if (openorclose)
            {
                MainDockPanel.Visibility = Visibility.Visible;
                MainDockPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(d, 0, 1));
                await Task.Run(() => { Thread.Sleep(d); });
                MainDockPanel.IsEnabled = true;
            }
            else
            {
                MainDockPanel.IsEnabled = false;
                MainDockPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(d, 0, 1));
                await Task.Run(() => { Thread.Sleep(d); });
                MainDockPanel.Visibility = Visibility.Hidden;
            }
        }
        async Task MainLoginVisiblity(bool openorclose,int d = 400)
        {
            if (openorclose)
            {
                MainLoginPanel.Visibility = Visibility.Visible;
                MainLoginPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(d, 0, 1));
                await Task.Run(() => { Thread.Sleep(d); });
                MainLoginPanel.IsEnabled = true;
            }
            else
            {
                MainLoginPanel.IsEnabled = false;
                MainLoginPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(d, 1, 0));
                await Task.Run(() => { Thread.Sleep(d); });
                MainLoginPanel.Visibility = Visibility.Hidden;
            }
        }
        private void TXTLogingotoSignup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("برجاء الاتصال بالإدارة المختصة بالمشروع\n01000000000","لا تملك حساب؟",MessageBoxButton.OK);
        }
        public static Brush Getbfroms(string hex)
        {
            try { return (SolidColorBrush)(new BrushConverter()).ConvertFromString(hex); }
            catch { return Brushes.White; }
        }
        private void MainListViewProv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(MainListViewProv.SelectedItem != null)
            {
                try
                {
                    statics.currentprov= (Province)MainListViewProv.SelectedItem;
                    provpageobj.InitializeFields(statics.currentprov);
                    MainFrameContainer.Content = provpageobj;
                }
                catch { }

            }
        }
        void resetControlers()
        {
            if (ocprovlist) { CloseMainProvListView(); }
            MainListViewProv.SelectedIndex = -1;
            MainSettingsBTN.Background = MainDashboardBTN.Background = MainSysManageBTN.Background = MainAboutPBTN.Background = MainStreamPBTN.Background = Getbfroms("#0c000000");
        }
        private void MainProvBTN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ocprovlist) { CloseMainProvListView(); }
            else
            {
                resetControlers();
                MainProvBTN.BeginAnimation(Border.MaxHeightProperty, statics.GetCDAnim(300, 40, 300));
                ocprovlist = true;
                TXTProvOpenClose.Text = "";
                MainProvBTN.Background = Getbfroms("#33000000");
            }
        }
        private void MainSysManageBTN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrameContainer.Content = new Views.SysManage.SysManageMainPage();
            resetControlers();
            MainSysManageBTN.Background = Getbfroms("#33000000");
        }
        private void MainSettingsBTN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrameContainer.Content = new Views.Setting.SettingMainPage();
            resetControlers();
            MainSettingsBTN.Background = Getbfroms("#33000000");
        }
        private void MainAboutPBTN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrameContainer.Content = new Views.AboutPage();
            resetControlers();
            MainAboutPBTN.Background = Getbfroms("#33000000");
        }
        private void MainStreamPBTN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrameContainer.Content = StramingMainPage;
            resetControlers();
            MainStreamPBTN.Background = Getbfroms("#33000000");
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Views.Position_Person_Page profilePage = new Views.Position_Person_Page(statics.LogedPerson);
            MainFrameContainer.Content = profilePage;
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Logout();
        }
        private async void Logout()
        {
            if(MessageBox.Show("هل تريد تسجيل الخروج الان؟", "تسجيل الخروج", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.RtlReading) == MessageBoxResult.Yes)
            {
                DB.SetOffline(statics.LogedPerson.ID);
                statics.LogedPerson = null;
                statics.LogedPersonPosition = null;
                statics.Provinces = null;
                statics.Data_Mang_Pages = null;
                statics.DChats = null;
                MainDockPanel.IsEnabled = false;
                DB.DisposeStreamListener();
                MainDockPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(300, 1, 0));
                await Task.Run(() => { Thread.Sleep(300); });
                MainDockPanel.Visibility = Visibility.Hidden;

                MainLoginPanel.Visibility = Visibility.Visible;
                MainLoginPanel.BeginAnimation(OpacityProperty, statics.GetCDAnim(300, 0, 1));
                MainLoginPanel.IsEnabled = true;
                LoginBorder.IsEnabled = true;
                MainFrameContainer.Content = null;
                resetControlers();
                ResetColors();
                OCFrame(false);
                fro = ch = chts = ocprovlist = false;
                GC.Collect();
            }
        }
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            AllChatsBTNMethod();
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                bor.BorderThickness = new Thickness(8);
            }
            else { bor.BorderThickness = new Thickness(0); }
        }
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {

        }
        private void MainDashboardBTN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrameContainer.Content = new Views.Position_Person_Page(statics.LogedPerson);
            resetControlers();
            MainDashboardBTN.Background = Getbfroms("#33000000");
        }
        void CloseMainProvListView()
        {
            MainProvBTN.BeginAnimation(Border.MaxHeightProperty, statics.GetCDAnim(300, 300, 40));
            ocprovlist = false;
            TXTProvOpenClose.Text = "";
            MainProvBTN.Background = Getbfroms("#0c000000");
        }
        public void OCFrame(bool openclose, int time = 400)
        {
            if (openclose) { ChatingFrame.BeginAnimation(WidthProperty, statics.GetCDAnim(time, 0, 300)); }
            else { ChatingFrame.BeginAnimation(WidthProperty, statics.GetCDAnim(time, 300, 0)); }
        }
        internal async Task<object> GODH(Panel cont)
        {
            object v = null;
            await Dispatcher.Invoke(async () => { v = await OpenDialogHost(cont); });
            return v;
        }
        internal async Task<object> OpenDialogHost(Panel content)
        {
            if (_ShowingDialog) return null;
            _ShowingDialog = true;
            content.Margin = new Thickness(10);
            content.MaxHeight = ActualHeight - 150;
            content.MaxWidth = ActualWidth - 100;
            object result = await MaterialDesignThemes.Wpf.DialogHost.Show(content);
            _ShowingDialog = false;
            return result;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SaveMe)
            {
                Check.IsChecked = true;
                TXTLoginUsername.Text = Properties.Settings.Default.User;
                TXTLoginPass.Password = Properties.Settings.Default.Pass;
                Logingin();
            }
        }
        internal async void OpenVideoChat(string partner)
        {
            if (!await DB.CheckPersonStreamAvailable(partner))
            {
                var txt = new TextBlock();
                txt.Text = "هذا المستخدم في حالة اتصال الان!! الرجاء المحاولة لاحقاً";

                Button btn2 = new Button();
                Style style2 = Application.Current.FindResource("MaterialDesignFlatButton") as Style;
                btn2.Style = style2;
                btn2.Width = 115;
                btn2.Height = 30;
                btn2.Margin = new Thickness(5);
                btn2.Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                btn2.CommandParameter = false;
                btn2.Content = "إغلاق";


                StackPanel stk = new StackPanel();
                stk.Children.Add(txt);
                stk.Children.Add(btn2);
                object result = await OpenDialogHost(stk);
                return;
            }
            else if (!await DB.IsOnline(partner)) 
            {
                var txt = new TextBlock();
                txt.Text = "هذا المستخدم غير متصل بالانترنت حالياً!! الرجاء المحاولة لاحقاً";

                Button btn2 = new Button();
                Style style2 = Application.Current.FindResource("MaterialDesignFlatButton") as Style;
                btn2.Style = style2;
                btn2.Width = 115;
                btn2.Height = 30;
                btn2.Margin = new Thickness(5);
                btn2.Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                btn2.CommandParameter = false;
                btn2.Content = "إغلاق";


                StackPanel stk = new StackPanel();
                stk.Children.Add(txt);
                stk.Children.Add(btn2);
                object result = await OpenDialogHost(stk);
                return;
            }
            else
            {
                Frame frame = new Frame();
                frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                frame.Height = 420;
                frame.Width = 640;

                var vd = new Views.VideoPages.VideoChatPage();
                var ls = await vd.Createvideochat();
                if (ls == null) { return; }
                DB.DisposeStreamListener();
                DB.SetVideoChat(partner, new OpenTokConfig {SENDER_ID = statics.LogedPerson.ID, SESSION_ID = ls[0], TOKEN = ls[1] });
                frame.Content = vd;

                Button btn2 = new Button();
                Style style2 = Application.Current.FindResource("MaterialDesignFlatButton") as Style;
                btn2.Click += (s, ee) => { DB.CloseVideoChat(partner); vd.disconnect(); };
                btn2.Style = style2;
                btn2.Width = 115;
                btn2.Height = 30;
                btn2.Margin = new Thickness(5);
                btn2.Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                btn2.CommandParameter = false;
                btn2.Content = "إغلاق";


                StackPanel stk = new StackPanel();
                stk.Children.Add(frame);
                stk.Children.Add(btn2);
                object result = await OpenDialogHost(stk);
            }
        }
        internal void OpenComingVCSream(OpenTokConfig config)
        {
            Dispatcher.Invoke(async() => {
                try
                {

                    Frame frame = new Frame();
                    frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                    if (MessageBox.Show($"يريد {await DB.GetPersonName(config.SENDER_ID)} الاتصال بك مكالمة فيديو","مكالمة فيديو قادمة!!",MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        DB.CloseComingVideoChat(config.SENDER_ID);
                        return;
                    }
                    await Task.Run(()=>{Thread.Sleep(500); });
                    frame.Height = 420;
                    frame.Width = 640;

                    var vd = new Views.VideoPages.VideoChatPage();
                    vd.EnterChat(config.SESSION_ID, config.TOKEN);
                    DB.DisposeStreamListener();
                    frame.Content = vd;

                    Button btn2 = new Button();
                    Style style2 = Application.Current.FindResource("MaterialDesignFlatButton") as Style;
                    btn2.Click += (s, ee) => { vd.disconnect(); DB.CloseComingVideoChat(config.SENDER_ID); DB.SetStreamListener(statics.LogedPerson.ID, OpenComingVCSream); };
                    btn2.Style = style2;
                    btn2.Width = 115;
                    btn2.Height = 30;
                    btn2.Margin = new Thickness(5);
                    btn2.Command = MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand;
                    btn2.CommandParameter = false;
                    btn2.Content = "إغلاق";


                    StackPanel stk = new StackPanel();
                    stk.Children.Add(frame);
                    stk.Children.Add(btn2);
                    object result = await OpenDialogHost(stk);
                }
                catch (Exception ex) { }
            });
        }
        internal async void addedsnaphundler(ValueAddedEventArgs snap)
        {

        }
    }


}
