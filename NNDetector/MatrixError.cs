using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixLibrary.Exception
{
    /// <summary>
    /// Thrown when a matrix error occured.
    /// </summary>
    public class MatrixError : System.Exception
    {
        /// <summary>
        /// Constructor for a simple message exception
        /// </summary>
        /// <param name="message">The message for the exception</param>
        public MatrixError(String message)
            : base(message)
        {
        }
    }
}
