using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalculatorMVVM.Models
{
    public class Calculator : ICalculator
    {
        public double Result { get; private set; }

        public void Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("Выражение не может быть пустым");
            }

            if (!Regex.IsMatch(expression, @"^[0-9+\-*/()\s.,]+$"))
            {
                throw new ArgumentException("Выражение содержит недопустимые символы");
            }

            try
            {
                var dataTable = new DataTable();
                var result = dataTable.Compute(expression, "");
                Result = Convert.ToDouble(result);
            }
            catch (EvaluateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Ошибка при вычислении выражения: {ex.Message}");
            }
        }
    }
}
