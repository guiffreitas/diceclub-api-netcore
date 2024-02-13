using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.Models
{
    public class ResultModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Result { get; set; } = default;
    }
}
