using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroupControl.ViewModel
{
    class DayHandel : ViewModelBase
    {
       


       
            public DayHandel()
            {
                Hidden = false;
            }

            bool? _Hidden = null;
            private ICommand _CmbSelectionChangeCommand;
            private ICommand _CmbSentEmailSelectionChangeCommand;
            public bool? Hidden
            {
                get
                {
                    return _Hidden;
                }

                set
                {
                    _Hidden = value; NotifyPropertyChanged("Hidden");
                }
            }
            private String _name;
            public String Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
            private ICommand _TestCommand;
            public ICommand TestCommand
            {
                get
                {
                    if (_TestCommand == null)
                    {
                        _TestCommand = new RelayCommand<CalendarCustom>((p) => true, OnTestCommand);
                    }
                    return _TestCommand;
                }

                set
                {
                    _TestCommand = value;
                }
            }

            private ICommand _HiddenCommand;
            public ICommand HiddenCommand
            {
                get
                {
                    if (_HiddenCommand == null)
                    {
                        _HiddenCommand = new RelayCommand<CalendarCustom>((p) => true, OnHiddenCommand);
                    }
                    return _HiddenCommand;
                }

                set
                {
                    _HiddenCommand = value;
                }
            }
            //Cmb change image
            public ICommand CmbSelectionChangeCommand
            {
                get
                {
                    _CmbSelectionChangeCommand = new RelayCommand<ComboBox>((p) => true, OnImageCmbSelectionChangeCommand);
                    return _CmbSelectionChangeCommand;
                }

                set
                {
                    _CmbSelectionChangeCommand = value;
                }
            }
            //cmb Sent Email
            public ICommand CmbSentEmailSelectionChangeCommand
            {
                get
                {
                    _CmbSentEmailSelectionChangeCommand = new RelayCommand<ComboBox>((R) => true, OnCmbSentEmailSelectionChangeCommand);
                    return _CmbSentEmailSelectionChangeCommand;
                }

                set
                {
                    _CmbSentEmailSelectionChangeCommand = value;
                }
            }



            private void OnCmbSentEmailSelectionChangeCommand(ComboBox cmb)
            {
                MessageBox.Show(cmb.SelectedIndex.ToString());
            }

            private void OnImageCmbSelectionChangeCommand(ComboBox cmb)
            {
                MessageBox.Show(cmb.SelectedIndex.ToString());
            }

            int a = 0;
            private void OnHiddenCommand(CalendarCustom cld)
            {
                if (a == 0)
                {
                    Hidden = true; a = 1;
                }
                else { Hidden = false; a = 0; }
            }

            private void OnTestCommand(CalendarCustom cld)
            {
                ChooseDay = cld.getDayTime.ToString();

            }
            private string chooseDay;
            public string ChooseDay
            {
                get
                {
                    return chooseDay;
                }

                set
                {
                    chooseDay = value; NotifyPropertyChanged("ChooseDay");
                }
            }

        }
    }



