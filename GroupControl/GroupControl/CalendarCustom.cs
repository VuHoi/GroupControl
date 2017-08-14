
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GroupControl.View;

namespace GroupControl
{
    
  

        public class CalendarCustom : Control, ICommandSource
        {
            static DateTime _currentMonth;
            static int _Month, _Year;
            static Grid grd;
            static TextBlock txtMonth;
            static Button btnPre, btnNext;

            static CalendarCustom()
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarCustom), new FrameworkPropertyMetadata(typeof(CalendarCustom)));
            }
            public static readonly DependencyProperty DayProperty = DependencyProperty.Register("getDayTime", typeof(DateTime), typeof(CalendarCustom), new PropertyMetadata(new DateTime(2000, 1, 1)));
            public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(CalendarCustom), new PropertyMetadata(null));
            public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CalendarCustom), new PropertyMetadata((ICommand)null, new PropertyChangedCallback(OnCommandChanged)));
            public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(CalendarCustom), new PropertyMetadata(null));
            public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CalendarCustom));



            public DateTime getDayTime
            {
                get { return (DateTime)GetValue(DayProperty); }
                set { SetValue(DayProperty, value); }
            }




            #region add command
            private EventHandler canExecuteChangedHandler;



            [TypeConverter(typeof(CommandConverter))]
            public ICommand Command
            {
                get { return (ICommand)GetValue(CommandProperty); }
                set { SetValue(CommandProperty, value); }
            }

            private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                CalendarCustom control = d as CalendarCustom;
                if (control != null)
                    control.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
            }
            protected virtual void OnCommandChanged(ICommand oldValue, ICommand newValue)
            {
                if (oldValue != null)
                    UnhookCommand(oldValue, newValue);

                HookupCommand(oldValue, newValue);

                CanExecuteChanged(null, null);
            }

            private void UnhookCommand(ICommand oldCommand, ICommand newCommand)
            {
                EventHandler handler = CanExecuteChanged;
                oldCommand.CanExecuteChanged -= CanExecuteChanged;
            }

            private void HookupCommand(ICommand oldCommand, ICommand newCommand)
            {
                EventHandler handler = new EventHandler(CanExecuteChanged);
                canExecuteChangedHandler = handler;
                if (newCommand != null)
                    newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }

            private void CanExecuteChanged(object sender, EventArgs e)
            {
                if (Command != null)
                {
                    RoutedCommand rc = Command as RoutedCommand;

                    if (rc != null)
                        IsEnabled = rc.CanExecute(CommandParameter, CommandTarget) ? true : false;
                    else
                        IsEnabled = Command.CanExecute(CommandParameter) ? true : false;
                }
            }


            public object CommandParameter
            {
                get { return GetValue(CommandParameterProperty); }
                set { SetValue(CommandParameterProperty, value); }
            }


            public IInputElement CommandTarget
            {
                get { return (IInputElement)GetValue(CommandTargetProperty); }
                set { SetValue(CommandTargetProperty, value); }
            }
            #endregion
            public event RoutedEventHandler ClickDay
            {
                add { AddHandler(ClickEvent, value); }
                remove { RemoveHandler(ClickEvent, value); }
            }

            void inputCalendar(DateTime currentMonth)
            {
                string DayOfWeek = currentMonth.DayOfWeek.ToString();
                switch (DayOfWeek)
                {
                    case "Monday":
                        currentMonth = currentMonth.AddDays(-1);
                        break;
                    case "Tuesday":
                        currentMonth = currentMonth.AddDays(-2);
                        break;
                    case "Wednesday":
                        currentMonth = currentMonth.AddDays(-3);
                        break;
                    case "Thursday":
                        currentMonth = currentMonth.AddDays(-4);
                        break;
                    case "Friday":
                        currentMonth = currentMonth.AddDays(-5);
                        break;
                    case "Saturday":
                        currentMonth = currentMonth.AddDays(-6);
                        break;
                    case "Sunday":
                        break;
                }
                for (int Week = 1; Week <= 6; Week++)
                {
                    for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
                    {
                        TextBlock day;
                        if (String.Format("{0:d/M/yyyy}", currentMonth) == String.Format("{0:d/M/yyyy}", DateTime.Now))
                            day = new TextBlock() { Background = new SolidColorBrush(Colors.Blue), Text = currentMonth.Day.ToString(), Foreground = new SolidColorBrush(Colors.Chocolate), HorizontalAlignment = HorizontalAlignment.Stretch, TextAlignment = TextAlignment.Center, FontSize = 20, FontWeight = FontWeights.Bold };
                        else if (currentMonth.Month != _Month)
                            day = new TextBlock() { Background = new SolidColorBrush(Colors.WhiteSmoke), Text = currentMonth.Day.ToString(), Foreground = new SolidColorBrush(Colors.Bisque), HorizontalAlignment = HorizontalAlignment.Stretch, TextAlignment = TextAlignment.Center, FontSize = 20, FontWeight = FontWeights.Bold };
                        else if (String.Format("{0:d/M/yyyy}", currentMonth) == String.Format("{0:d/M/yyyy}", getDayTime))
                            day = new TextBlock() { Background = new SolidColorBrush(Colors.Red), Text = currentMonth.Day.ToString(), Foreground = new SolidColorBrush(Colors.Chocolate), HorizontalAlignment = HorizontalAlignment.Stretch, TextAlignment = TextAlignment.Center, FontSize = 20, FontWeight = FontWeights.Bold };
                        else
                            day = new TextBlock() { Background = new SolidColorBrush(Colors.WhiteSmoke), Text = currentMonth.Day.ToString(), Foreground = new SolidColorBrush(Colors.Chocolate), HorizontalAlignment = HorizontalAlignment.Stretch, TextAlignment = TextAlignment.Center, FontSize = 20, FontWeight = FontWeights.Bold };
                        day.RenderTransform = new ScaleTransform();
                        day.RenderTransformOrigin = new Point(0.5, 0.5);
                        day.SetResourceReference(Control.StyleProperty, "StyleDay");
                        Grid.SetRow(day, Week);
                        Grid.SetColumn(day, dayOfWeek);

                        day.MouseEnter += Day_MouseEnter;
                        day.MouseLeave += Day_MouseLeave;
                        day.MouseLeftButtonDown += Day_MouseLeftButtonDown;
                        (grd as Grid).Children.Add(day);
                        currentMonth = currentMonth.AddDays(1);

                    }
                }
            }

            private void Day_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
          
            try
                {
                    if (String.Format("{0:d/M/yyyy}", new DateTime(_Year, _Month, Int32.Parse(((TextBlock)sender).Text))) != String.Format("{0:d/M/yyyy}", DateTime.Now)) ((TextBlock)sender).Background = new SolidColorBrush(Colors.Red);
                }
                catch { ((TextBlock)sender).Background = new SolidColorBrush(Colors.Red); }
                foreach (UIElement i in grd.Children)
                {
                    TextBlock a = i as TextBlock;
                    if (a != null && getDayTime != null)
                        try
                        {
                            if (getDayTime.Day == Int32.Parse((a).Text) && String.Format("{0:d/M/yyyy}", new DateTime(_Year, _Month, getDayTime.Day)) != String.Format("{0:d/M/yyyy}", DateTime.Now))
                            {
                                (i as TextBlock).Background = new SolidColorBrush(Colors.WhiteSmoke);

                            }
                        }
                        catch { (i as TextBlock).Background = new SolidColorBrush(Colors.WhiteSmoke); }
                }
                try
                {
                    if (getDayTime != new DateTime(_Year, _Month, Int32.Parse(((TextBlock)sender).Text)))
                        getDayTime = new DateTime(_Year, _Month, Int32.Parse(((TextBlock)sender).Text));
                    else getDayTime = DateTime.Now;
                }
                catch
                {
                    if (getDayTime != new DateTime(_Year, _Month - 1, Int32.Parse(((TextBlock)sender).Text)))
                        getDayTime = new DateTime(_Year, _Month - 1, Int32.Parse(((TextBlock)sender).Text));
                    else getDayTime = DateTime.Now;
                }

            if (((TextBlock)sender).Foreground.ToString() == (new SolidColorBrush(Colors.Bisque)).ToString())
            {
                if (Int32.Parse(((TextBlock)sender).Text) < 10)
                {
                    if (_Month < 12)
                    {
                        getDayTime = new DateTime(_Year, _Month + 1, Int32.Parse(((TextBlock)sender).Text));

                        BtnNext_Click(null, null);

                    }
                }
                else
                {
                    if (_Month > 1)
                    {
                        getDayTime = new DateTime(_Year, _Month - 1, Int32.Parse(((TextBlock)sender).Text));

                        BtnPre_Click(null, null);

                    }
                }
            }
                Point relativePoint = ((TextBlock)sender).TransformToAncestor(this).Transform(new Point(0, 0));

                EventDayVIew ab = new EventDayVIew(); if (relativePoint.X > 750) ab.Left = relativePoint.X - 410;
                else ab.Left = relativePoint.X + 205;
                ab.Top = relativePoint.Y;
                ab.Height = 400;
                ab.Width = 490;

                ab.ShowDialog();
            
                TextBlock Event = new TextBlock() { Background = new SolidColorBrush(Colors.WhiteSmoke) };
                Event.Text = "vu khac hoi";
            
                Event.Height = 30;
                Event.RenderTransform = new ScaleTransform();
                Event.RenderTransformOrigin = new Point(0.5, 0.5);
                Event.SetResourceReference(Control.StyleProperty, "triggerTextbox");
                Event.Margin = new Thickness(0, 10, 0, 0);
                Panel.SetZIndex(Event, Panel.GetZIndex((TextBlock)sender) - 1);
                Grid.SetRow(Event, Grid.GetRow((TextBlock)sender));
                Grid.SetColumn(Event, Grid.GetColumn((TextBlock)sender));
            MessageBox.Show(Grid.GetRow(Event).ToString());



            int isIndex = 0; int k = 0;
                for (isIndex = 10; isIndex > 0; isIndex--)
                {
                    try
                    {
                        if (Grid.GetColumn(grd.Children[grd.Children.IndexOf((TextBlock)sender) + isIndex]) == Grid.GetColumn((TextBlock)sender) && Grid.GetRow(grd.Children[grd.Children.IndexOf((TextBlock)sender) + isIndex]) == Grid.GetRow((TextBlock)sender))
                        {
                            k = 1;
                            break;
                        }
                    }
                    catch { }
                }

                if (k != 0)
                {
                    Event.Margin = new Thickness(0, (grd.Children[grd.Children.IndexOf((TextBlock)sender) + isIndex] as TextBlock).Margin.Top + 50, 0, 0);
                    grd.Children.Insert(grd.Children.IndexOf((TextBlock)sender) + isIndex + 1, Event); return;
                }
                grd.Children.Insert(grd.Children.IndexOf((TextBlock)sender) + 1, Event);

            


                // add even ClickDay
                RoutedEventArgs args = new RoutedEventArgs(CalendarCustom.ClickEvent);
                RaiseEvent(args);
                //add command
                if (Command != null)
                {
                    RoutedCommand rc = Command as RoutedCommand;
                    if (rc != null)
                        rc.Execute(CommandParameter, CommandTarget);
                    else
                        Command.Execute(CommandParameter);
                }
            }

            private void Day_MouseLeave(object sender, MouseEventArgs e)
            {
                Panel.SetZIndex((sender as TextBlock), 500);
                if (((TextBlock)sender).Text == DateTime.Now.Day.ToString() && _Month == DateTime.Now.Month && _Year == DateTime.Now.Year)
                {
                    if ((Int32.Parse(((TextBlock)sender).Text) <= 7) && (Grid.GetRow((TextBlock)sender) == 1) || (Int32.Parse(((TextBlock)sender).Text) <= 31) && (Grid.GetRow((TextBlock)sender) == 6)) ((TextBlock)sender).Background = new SolidColorBrush(Colors.WhiteSmoke);
                    return;
                }
                if (((TextBlock)sender).Text == getDayTime.Day.ToString() && _Month == getDayTime.Month && _Year == getDayTime.Year) return;

                ((TextBlock)sender).Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
            private void Day_MouseEnter(object sender, MouseEventArgs e)
            {
                Panel.SetZIndex((sender as TextBlock), 2000);
                SolidColorBrush slcbBackground = new SolidColorBrush(Colors.Blue);
                if (((TextBlock)sender).Text == getDayTime.Day.ToString() && _Month == getDayTime.Month && _Year == getDayTime.Year)
                {
                    slcbBackground = new SolidColorBrush(Colors.Red);
                    return;
                }

                ((TextBlock)sender).Background = slcbBackground;


            }
            public override void OnApplyTemplate()
            {
                base.OnApplyTemplate();
                grd = GetTemplateChild("grd") as Grid;
                txtMonth = GetTemplateChild("txtMonth") as TextBlock;
                btnNext = GetTemplateChild("btnNext") as Button;
                btnPre = GetTemplateChild("btnPre") as Button;
                btnPre.Click += BtnPre_Click;
                btnNext.Click += BtnNext_Click;
                _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _Month = _currentMonth.Month;
                _Year = _currentMonth.Year;
                displayMonth(_currentMonth.Month);
                inputCalendar(_currentMonth);

                grd.MouseWheel += Grd_MouseWheel;



            }
            private void Grd_MouseWheel(object sender, MouseWheelEventArgs e)
            {
                if (e.Delta > 0) BtnNext_Click(null, null);
                else BtnPre_Click(null, null);

            }
            private void BtnNext_Click(object sender, RoutedEventArgs e)
            {
                int i;
                for (i = 0; i < grd.Children.Count; i++)
                {

                    if (grd.Children[i].GetType() == typeof(TextBlock))
                    {
                        try
                        {
                            grd.Children.RemoveAt(i);
                            i--;
                        }
                        catch { };
                    }
                }

                _Month++;

                if (_Month == 13) { _Month = 1; _Year++; }
                displayMonth(_Month);
                _currentMonth = _currentMonth.AddMonths(1);

                inputCalendar(_currentMonth);

            }
            private void BtnPre_Click(object sender, RoutedEventArgs e)
            {

                int i;
                for (i = 0; i < grd.Children.Count; i++)
                {

                    if (grd.Children[i].GetType() == typeof(TextBlock))
                    {
                        try
                        {
                            grd.Children.RemoveAt(i);
                            i--;
                        }
                        catch { };
                    }
                }
                _Month--;
                if (_Month == 0) { _Month = 12; _Year--; }
                displayMonth(_Month);
                _currentMonth = _currentMonth.AddMonths(-1);
                inputCalendar(_currentMonth);
            }
            static void displayMonth(int month)
            {
                switch (month)
                {
                    case 1:
                        txtMonth.Text = "January ";
                        break;
                    case 2:
                        txtMonth.Text = "February ";
                        break;
                    case 3:
                        txtMonth.Text = "March ";
                        break;
                    case 4:
                        txtMonth.Text = "April ";
                        break;
                    case 5:
                        txtMonth.Text = "May ";
                        break;
                    case 6:
                        txtMonth.Text = "June ";
                        break;
                    case 7:
                        txtMonth.Text = "July ";
                        break;
                    case 8:
                        txtMonth.Text = "August ";
                        break;
                    case 9:
                        txtMonth.Text = "September ";
                        break;
                    case 10:
                        txtMonth.Text = "October ";
                        break;
                    case 11:
                        txtMonth.Text = "November ";
                        break;
                    case 12:
                        txtMonth.Text = "December ";
                        break;
                }
                txtMonth.Text = txtMonth.Text + _currentMonth.Year.ToString();
            }



        }
    }

