﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Core
{
    public class DbException : Exception
    {
        public DbException(string errorMsg)
            : base(errorMsg)
        {
        }
    }
}