﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ProcessMonitorUI.Helper;
using ProcessMonitorUI.Models;

using PMonitor.Core;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;

namespace ProcessMonitorUI.ViewModels
{
    public class MainWindowViewModel:INotifyPropertyChanged
    {
        #region Properties
        public AuthorizationData AuthorizationData { get; set; }
        private bool _isDemo = true;
        public bool IsDemo
        {
            get => _isDemo;
            set
            {
                if (_isDemo !=value)
                {
                    _isDemo = value;
                    OnPropertyChanged(nameof(IsDemo));
                }
            }
        }
        private string _startText;
        public string StartText
        {
            get => _startText;
            set
            {
                _startText = value;
                OnPropertyChanged("StartText");
            }
        }
        public ObservableCollection<string> LogTexts { get; set; }
        public RelayCommand SetLocationCommand { get; set; }
        public RelayCommand AuthInfoSaveCommand { get; set; }
        public RelayCommand StartMonitoringCommand { get; set; }
        public RelayCommand SetProcessLocationCommand { get; set; }
        #endregion

        #region Private fields
        private string _processLocation;
        private string _infoFilesLocation;
        private string _authInfoFilename;
        private string _dbFilename;

        private bool _monitoringStarted = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer _monitoringTimer;
        private IProcessMonitor pm = ProcessMonitorFactory.BuildDefaultOSProcessMonitor();
        #endregion

        public MainWindowViewModel()
        {
            RetreiveSettings();

            LogTexts = new ObservableCollection<string>();
            string authFilename = Path.Combine(_infoFilesLocation, _authInfoFilename);
            if (!UpdateAuthorizationData())
                AuthorizationData = new AuthorizationData();

            StartText = "Start";           

            SetLocationCommand = new RelayCommand(OnSetAuthInfoLocation);
            AuthInfoSaveCommand = new RelayCommand(OnAuthInfoSave);
            StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
            SetProcessLocationCommand = new RelayCommand(OnSetProcessLocation);

            _monitoringTimer = new DispatcherTimer();
            _monitoringTimer.Interval = new TimeSpan(0, 0, 30);
            _monitoringTimer.Tick += _monitoringTimer_Tick;

            AddLog("Monitor Program started");
        }
        
        #region Private Methods
        private void OnSetAuthInfoLocation()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _infoFilesLocation = dialog.SelectedPath;
                    AddLog("Info file Location changed as " + _infoFilesLocation);
                    UpdateAuthorizationData();
                }
            }

            CheckIfIsDeom();

            SaveSettings();
        }

        private void CheckIfIsDeom()
        {
            if (_infoFilesLocation.ToLower().Contains("demo"))
                IsDemo = true;
            else
                IsDemo = false;
        }

        private bool UpdateAuthorizationData()
        {
            try
            {
                string authFilename = Path.Combine(_infoFilesLocation, _authInfoFilename);
                AuthorizationData = Utility.XMLDeserialze(authFilename, typeof(AuthorizationData)) as AuthorizationData;
                AddLog("Authorization data read");
                return true;
            }
            catch (Exception)
            {
                AddLog("Authorization data can't be read");
                return false;
            }
            
        }

        private void OnAuthInfoSave()
        {
            try
            {
                string authFilename = Path.Combine(_infoFilesLocation, _authInfoFilename);
                Utility.XMLSerialize(authFilename, AuthorizationData);
                AddLog("Authorization info saved");
            }
            catch (Exception)
            {
                AddLog("Authorization info can't be saved");
            }
            
        }

        private void OnStartMonitoring()
        {
            if (_monitoringStarted)
                StopMonitoring();
            else
                StartMonitoring();
        }

        private void StopMonitoring()
        {
            StartText = "Start";
            AddLog("Monitoring stopped");
            _monitoringTimer.Stop();
            _monitoringStarted = false;

        }
        private void StartMonitoring()
        {
            StartText = "Stop";
            AddLog("Monitoring started");
            _monitoringTimer.Start();
            _monitoringStarted = true;
        }
        private void AddLog(string log)
        {
            string logText = DateTime.Now.ToString() + " " + log;
            LogTexts.Add(logText);
        }        

        private void OnSetProcessLocation()
        {
            using (var dialog = new OpenFileDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _processLocation = dialog.FileName;
                    AddLog("Process Location changed as " + _processLocation);                    
                }
            }
            SaveSettings();
        }
        private void _monitoringTimer_Tick(object sender, EventArgs e)
        {
            MonitorProcess();
        }

        private void MonitorProcess()
        {
            pm.RefreshInformation();
            BasicProcessInformation bpi = pm.GetProcessInformation().Single();            

            if (bpi.State == ProcessState.NotRunning)
                StartProcess();            
        }
        private void StartProcess()
        {
            AddLog("BoehmTrader will restart--");
            Process.Start(_processLocation);
        }

        private void RetreiveSettings()
        {
            _processLocation = Properties.Settings.Default.ProcessLocation;
            _infoFilesLocation = Properties.Settings.Default.InfoFilesLocation;
            _authInfoFilename = Properties.Settings.Default.AuthInfoFilename;
            _dbFilename = Properties.Settings.Default.DbFilename;

            CheckIfIsDeom();
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.ProcessLocation = _processLocation;
            Properties.Settings.Default.InfoFilesLocation = _infoFilesLocation;
            Properties.Settings.Default.AuthInfoFilename = _authInfoFilename;
            Properties.Settings.Default.DbFilename = _dbFilename;
            Properties.Settings.Default.Save();
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
