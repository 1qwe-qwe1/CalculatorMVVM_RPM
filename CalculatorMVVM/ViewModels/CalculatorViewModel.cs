using CalculatorMVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalculatorMVVM.ViewModels
{
    public class CalculatorViewModel : ViewModelBase
    {
        private string _expression = string.Empty;
        private string _result = string.Empty;
        private string _errorMessage = string.Empty;
        private readonly ICalculator _calculator;

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                NotifyPropertyChanged(nameof(Expression));
                
                ErrorMessage = string.Empty;
            }
        }

        public string Result
        {
            get => _result;
            private set
            {
                _result = value;
                NotifyPropertyChanged(nameof(Result));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ObservableCollection<HistoryItem> History { get; }
            = new ObservableCollection<HistoryItem>();

        public ICommand CalculateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand ClearHistoryCommand { get; private set; }

        public CalculatorViewModel(ICalculator calculator)
        {
            _calculator = calculator;
            CalculateCommand = new ActionCommand(Calculate);
            ClearCommand = new ActionCommand(Clear);
            ClearHistoryCommand = new ActionCommand(ClearHistory);
        }

        private void Calculate()
        {
            if (string.IsNullOrWhiteSpace(Expression))
            {
                ErrorMessage = "Введите выражение для вычисления";
                Result = string.Empty;
                return;
            }

            try
            {
                _calculator.Evaluate(Expression);
                Result = _calculator.Result.ToString();
                ErrorMessage = string.Empty;

                History.Insert(0, new HistoryItem(Expression, Result));
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = ex.Message;
                Result = string.Empty;
            }
            catch (DivideByZeroException)
            {
                ErrorMessage = "Ошибка: деление на ноль";
                Result = string.Empty;
            }
            catch (Exception)
            {
                ErrorMessage = "Ошибка при вычислении выражения";
                Result = string.Empty;
            }
        }

        private void Clear()
        {
            Expression = string.Empty;
            Result = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void ClearHistory()
        {
            History.Clear();
        }
    }

    public class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
