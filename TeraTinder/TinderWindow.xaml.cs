using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC;
using TCC.Parsing;
using TCC.Utilities;
using TeraDataLite;
using TeraPacketParser.Messages;
using ModifierKeys = TCC.Data.ModifierKeys;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace TeraTinder
{
    [TccModule]
    public class TeraTinderModule
    {
        private static TinderWindow _win;
        public static void Init()
        {
            _win = new TinderWindow();
            KeyboardHook.Instance.RegisterCallback(new HotKey(Keys.T, ModifierKeys.Control | ModifierKeys.Alt), OnTinderHotkeyPressed);
            PacketAnalyzer.NewProcessor.Hook<S_SPAWN_USER>(OnSpawnUser);
            PacketAnalyzer.NewProcessor.Hook<S_LOAD_TOPO>(p => Clear());
            PacketAnalyzer.NewProcessor.Hook<S_RETURN_TO_LOBBY>(p => Clear());
        }

        private static void Clear()
        {
            _win.Dispatcher.BeginInvoke(new Action(() => { _win.VM.ClearMatches(); }));
        }

        private static void OnSpawnUser(S_SPAWN_USER p)
        {
            _win.Dispatcher.BeginInvoke(new Action(() =>
            {
                _win.VM.AddMatch(new CardVM(p.Name, $"{p.GuildRank} of {p.GuildName}", p.Level,
                    App.Random.Next(10, 1000), TccUtils.RaceFromTemplateId(p.TemplateId), p.PlayerId));
            }));
        }

        private static void OnTinderHotkeyPressed()
        {
            _win.Show();
        }
    }
    public partial class TinderWindow 
    {
        public MainVM VM { get; }
        public enum Status
        {
            Undefined,
            Passed,
            Liked
        }

        private bool _dragging;
        private Point _dragStart;
        private Point _prevPos;
        private double _vertOffset;
        private Status _status = Status.Undefined;

        public TinderWindow()
        {
            DataContext = new MainVM();
            VM = DataContext as MainVM;
            InitializeComponent();
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStart = e.MouseDevice.GetPosition(this);
            _dragging = true;
            this.CaptureMouse();
            var tx = TranslatePoint(new Point(0, 0), CurrentCard);
            _vertOffset = (_dragStart.Y + tx.Y) / CurrentCard.ActualHeight;
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            var pos = e.MouseDevice.GetPosition(this);
            if (_prevPos == pos) return;
            var trans = ((TransformGroup)CurrentCard.RenderTransform).Children[1] as TranslateTransform;
            trans.X = (pos - _dragStart).X;
            trans.Y = (pos - _dragStart).Y;
            _prevPos = pos;

            var rot = ((TransformGroup)CurrentCard.RenderTransform).Children[0] as RotateTransform;

            rot.Angle = trans.X * (_vertOffset >= .5 ? -1 : 1) * 0.05;
            CurrentCard.RateLevel = (float)trans.X * 0.005f;
            if (trans.X > CurrentCard.ActualWidth / 2)
            {
                _status = Status.Liked;
                CurrentCard.Rate = _status;
            }
            else if (trans.X < -CurrentCard.ActualWidth / 2)
            {
                _status = Status.Passed;
                CurrentCard.Rate = _status;
            }
            else
            {
                _status = Status.Undefined;
                CurrentCard.Rate = _status;
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_dragging) return;
            StopDragging();
        }

        private void StopDragging()
        {
            _dragging = false;
            this.ReleaseMouseCapture();
            var trans = ((TransformGroup)CurrentCard.RenderTransform).Children[1] as TranslateTransform;
            if (trans.X == 0 && trans.Y == 0) return;
            var rot = ((TransformGroup)CurrentCard.RenderTransform).Children[0] as RotateTransform;
            var anX = new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };
            var anY = new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };
            var anR = new DoubleAnimation(0, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };

            switch (_status)
            {
                case Status.Passed:
                    anX.To = -CurrentCard.ActualWidth;
                    break;
                case Status.Liked:
                    anX.To = CurrentCard.ActualWidth;
                    break;
                default:
                    anX.To = 0;
                    break;
            }
            anX.Completed += (_, __) =>
            {
                ((TransformGroup)CurrentCard.RenderTransform).Children[1] = new TranslateTransform(0, 0);
                ((TransformGroup)CurrentCard.RenderTransform).Children[0] = new RotateTransform(0);
                if (_status != Status.Undefined) VM.Current.InvokeRated();
                _status = Status.Undefined;
                CurrentCard.Rate = _status; // todo: move these to VM
                CurrentCard.RateLevel = 0;  // todo: move these to VM
            };
            trans.BeginAnimation(TranslateTransform.XProperty, anX);
            trans.BeginAnimation(TranslateTransform.YProperty, anY);
            rot.BeginAnimation(RotateTransform.AngleProperty, anR);

        }

        private void MainWindow_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released) return;
            StopDragging();
        }

        private List<CardVM> matches = new List<CardVM>
        {
            new CardVM("Foglio", "Usagi no Mura", 70,56, Race.Elin, 0),
            new CardVM("Chobee", "Endgame", 68,3, Race.Elin, 1),
            new CardVM("Foglia", "", 65,168, Race.HumanFemale, 2),
        };

        private int i = 0;
        private void AddTestMatch(object sender, MouseButtonEventArgs e)
        {
            if (i > 2) i = 0;
            VM.AddMatch(matches[i++]);
        }

        private void TinderWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
