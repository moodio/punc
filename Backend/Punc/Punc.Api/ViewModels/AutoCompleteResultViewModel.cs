using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Punc.Api.ViewModels
{
    public class AutoCompleteResultViewModel
    {
        public int CallNumber { get; set; }

        public List<string> Predictions { get; set; }

    }
}
