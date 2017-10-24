using Hornet.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hornet.ViewModel.ViewModel
{
    public class ProgressViewModel : WindowCreatingViewModel
    {
        public ProgressViewModel()
        {
            StartMonitor();
        }

        private DateTime _startTime = DateTime.Now;

        private string _TimeElapsed;

        public string TimeElapsed
        {
            get { return _TimeElapsed; }
            set
            {
                _TimeElapsed = value;
                RaisePropertyChanged("TimeElapsed");
            }
        }

        private string _TotalProcessed;

        public string TotalProcessed
        {
            get { return _TotalProcessed; }
            set
            {
                _TotalProcessed = value;
                RaisePropertyChanged("TotalProcessed");
            }
        }

        private long _Skipped;

        public long Skipped
        {
            get { return _Skipped; }
            set
            {
                _Skipped = value;
                RaisePropertyChanged("Skipped");
            }
        }


        private long _HashMatches;

        public long HashMatches
        {
            get { return _HashMatches; }
            set
            {
                _HashMatches = value;
                RaisePropertyChanged("HashMatches");
            }
        }


        private long _RegexMatches;

        public long RegexMatches
        {
            get { return _RegexMatches; }
            set
            {
                _RegexMatches = value;
                RaisePropertyChanged("RegexMatches");
            }
        }

        private int _QueueSize;

        public int QueueSize
        {
            get { return _QueueSize; }
            set
            {
                _QueueSize = value;
                RaisePropertyChanged("QueueSize");
            }
        }

        private bool _IsProgressIndeterminate;

        public bool IsProgressIndeterminate
        {
            get { return _IsProgressIndeterminate; }
            set
            {
                _IsProgressIndeterminate = value;
                RaisePropertyChanged("IsProgressIndeterminate");
            }
        }

        private long _ProgressMax;

        public long ProgressMax
        {
            get { return _ProgressMax; }
            set
            {
                _ProgressMax = value;
                RaisePropertyChanged("ProgressMax");
            }
        }

        private long _ProgressValue;

        public long ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                _ProgressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }

        private double _Progress;

        public double Progress
        {
            get { return _Progress; }
            set
            {
                _Progress = value;
                RaisePropertyChanged("Progress");
            }
        }


        private void StartMonitor()
        {
            Thread thread = new Thread(() =>
            {
                while (HornetScanManager.Status.ScanRunning)
                {
                    UpdateBindings();
                    Thread.Sleep(500);                
                }
            });

            thread.Start();
        }

        private void UpdateBindings()
        {
            StringBuilder sb = new StringBuilder();

            TimeSpan elapsed = DateTime.Now - _startTime;

            if (elapsed.Days > 1) sb.Append($"{elapsed.Days} days ");
            if (elapsed.Days == 1) sb.Append($"{elapsed.Days} day ");

            if (elapsed.Hours > 1) sb.Append($"{elapsed.Hours} hours ");
            if (elapsed.Hours == 1) sb.Append($"{elapsed.Hours} hour ");

            if (elapsed.Minutes > 1) sb.Append($"{elapsed.Minutes} minutes ");
            if (elapsed.Minutes == 1) sb.Append($"{elapsed.Minutes} minute ");

            if (elapsed.Seconds > 1) sb.Append($"{elapsed.Seconds} seconds ");
            if (elapsed.Seconds == 1) sb.Append($"{elapsed.Seconds} second ");


            TimeElapsed = sb.ToString();
            sb.Clear();

            sb.Append(HornetScanManager.Status.TotalFilesSucceeded.ToString("#,##0"));
            sb.Append(" files processed (");
            sb.Append(GetFriendlySize(HornetScanManager.Status.TotalBytesProcessed));
            sb.Append(")");

            TotalProcessed = sb.ToString();
            sb.Clear();

            Skipped = HornetScanManager.Status.TotalFilesSkipped;
            HashMatches = HornetScanManager.Results.HashGroups.Sum(g => g.Matches.Count);
            RegexMatches = HornetScanManager.Results.RegexGroups.Sum(g => g.Matches.Count);
            QueueSize = HornetScanManager.CurrentQueueSize;

            if (HornetScanManager.Status.EnumerationFinished)
            {
                IsProgressIndeterminate = false;
                ProgressMax = HornetScanManager.Status.TotalFileCount;
                ProgressValue = HornetScanManager.Status.TotalFilesEncrypted +
                                HornetScanManager.Status.TotalFilesFailed +
                                HornetScanManager.Status.TotalFilesSkipped +
                                HornetScanManager.Status.TotalFilesSucceeded;
                Progress = ProgressValue / (double)ProgressMax;
            }
            else
            {
                IsProgressIndeterminate = true;
            }
        }

        private static double _TB = Math.Pow(1024, 4);
        private static double _GB = Math.Pow(1024, 3);
        private static double _MB = Math.Pow(1024, 2);
        private static double _KB = Math.Pow(1024, 1);

        private string GetFriendlySize(long bytes)
        {
            if (bytes > _TB) return (bytes / _TB).ToString("#,##0.00") + " TB";
            if (bytes > _GB) return (bytes / _GB).ToString("#,##0.00") + " GB";
            if (bytes > _MB) return (bytes / _MB).ToString("#,##0.00") + " MB";
            if (bytes > _KB) return (bytes / _KB).ToString("#,##0.00") + " KB";

            return bytes.ToString("#,##0") + " B";
        }
    }
}
