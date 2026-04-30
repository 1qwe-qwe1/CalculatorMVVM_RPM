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

            if (!Regex.IsMatch(expression, @"^[0-9+\-*/^()\s.,a-zA-Z]+$"))
            {
                throw new ArgumentException("Выражение содержит недопустимые символы");
            }

            expression = expression.Replace(" ", "");

            try
            {
                int pos = 0;
                double result = Parse(expression, ref pos);

                if (pos < expression.Length)
                {
                    throw new ArgumentException($"Неожиданный символ: {expression[pos]}");
                }

                Result = result;
            }
            catch (DivideByZeroException)
            {
                throw new DivideByZeroException();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Ошибка: {ex.Message}");
            }
        }

        private double Parse(string expr, ref int pos)
        {
            double result = ParseTerm(expr, ref pos);

            while (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
            {
                char op = expr[pos++];
                double term = ParseTerm(expr, ref pos);
                result = op == '+' ? result + term : result - term;
            }
            return result;
        }

        private double ParseTerm(string expr, ref int pos)
        {
            double result = ParseFactor(expr, ref pos);

            while (pos < expr.Length && (expr[pos] == '*' || expr[pos] == '/'))
            {
                char op = expr[pos++];
                double factor = ParseFactor(expr, ref pos);
                if (op == '/')
                {
                    if (factor == 0) throw new DivideByZeroException();
                    result /= factor;
                }
                else result *= factor;
            }
            return result;
        }

        private double ParseFactor(string expr, ref int pos)
        {
            if (pos < expr.Length && (expr[pos] == '+' || expr[pos] == '-'))
            {
                char op = expr[pos++];
                double value = ParseFactor(expr, ref pos);
                return op == '-' ? -value : value;
            }

            double result = ParsePower(expr, ref pos);

            // Возведение в степень
            if (pos < expr.Length && expr[pos] == '^')
            {
                pos++;
                result = Math.Pow(result, ParseFactor(expr, ref pos));
            }
            return result;
        }

        private double ParsePower(string expr, ref int pos)
        {
            if (pos >= expr.Length)
                throw new ArgumentException("Неожиданный конец выражения");

            char current = expr[pos];

            if (char.IsDigit(current) || current == '.')
            {
                int start = pos;
                while (pos < expr.Length && (char.IsDigit(expr[pos]) || expr[pos] == '.' || expr[pos] == ','))
                    pos++;

                string num = expr.Substring(start, pos - start).Replace(',', '.');
                return double.Parse(num, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (current == '(')
            {
                pos++;
                double result = Parse(expr, ref pos);
                if (pos >= expr.Length || expr[pos] != ')')
                    throw new ArgumentException("Отсутствует ')'");
                pos++;
                return result;
            }

            // Функции
            if (char.IsLetter(current))
            {
                return ParseFunction(expr, ref pos);
            }

            throw new ArgumentException($"Неожиданный символ: {current}");
        }

        private double ParseFunction(string expr, ref int pos)
        {
            int start = pos;
            while (pos < expr.Length && char.IsLetter(expr[pos]))
                pos++;

            string func = expr.Substring(start, pos - start).ToLower();

            if (pos >= expr.Length || expr[pos] != '(')
                throw new ArgumentException($"После {func} ожидается '('");

            pos++;
            double arg = Parse(expr, ref pos);

            if (pos >= expr.Length || expr[pos] != ')')
                throw new ArgumentException($"Отсутствует ')' для {func}");
            pos++;

            switch (func)
            {
                case "sin":
                    return Math.Sin(arg);
                case "cos":
                    return Math.Cos(arg);
                case "tan":
                    return Math.Tan(arg);
                case "ln":
                    if (arg <= 0)
                        throw new ArgumentException("ln от неположительного числа");
                    return Math.Log(arg);
                case "log":
                    if (arg <= 0)
                        throw new ArgumentException("log от неположительного числа");
                    return Math.Log10(arg);
                case "sqrt":
                    if (arg < 0)
                        throw new ArgumentException("sqrt от отрицательного числа");
                    return Math.Sqrt(arg);
                case "abs":
                    return Math.Abs(arg);
                default:
                    throw new ArgumentException($"Неизвестная функция: {func}");
            }
        }
    }
}
