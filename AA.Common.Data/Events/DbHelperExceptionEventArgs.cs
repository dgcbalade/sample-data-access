using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AA.Common.Events
{
    public class DbHelperExceptionEventArgs : EventArgs
    {
        public Exception Error { get; private set; }
        public string Message { get; private set; }

        public DbHelperExceptionEventArgs(Exception error, string message = null)
        {
            this.Error = error;
            this.Message = message;
        }
    }
}
