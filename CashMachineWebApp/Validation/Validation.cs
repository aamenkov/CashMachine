using System;
using System.Collections.Generic;
using System.Linq;
using CashMachineWebApp.Models;

namespace CashMachineWebApp.Validation
{
    public static class Validation
    {
        private static readonly int[] _expectedInput = {10, 50, 100, 200, 500, 1000, 2000, 5000}; 
        public static bool CheckCassette(Cassette cassette)
        {
            if ((cassette.Amount > 0) && (Array.IndexOf(_expectedInput, cassette.Value) >= 0 ))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckCassetteList(List<Cassette> list)
        {
            var check = true;
            foreach (var variable in list.Where(variable => !CheckCassette(variable)))
            {
                check = false;
            }

            if (list.Count > 8)
                check = false;

            return check;
        }
    }
}
