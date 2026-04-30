using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorMVVM.Models
{
    public interface ICalculator
    {
        double Result { get; }
        void Evaluate(string expression);
    }
}
