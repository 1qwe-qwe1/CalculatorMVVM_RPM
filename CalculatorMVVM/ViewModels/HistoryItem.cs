using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorMVVM.ViewModels
{
    public class HistoryItem : ViewModelBase
    {
        private string _expression = string.Empty;
        private string _result = string.Empty;
        private string _timestamp = string.Empty;

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                NotifyPropertyChanged(nameof(Expression));
            }
        }

        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                NotifyPropertyChanged(nameof(Result));
            }
        }

        public string Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                NotifyPropertyChanged(nameof(Timestamp));
            }
        }

        public HistoryItem(string expression, string result)
        {
            Expression = expression;
            Result = result;
            Timestamp = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
