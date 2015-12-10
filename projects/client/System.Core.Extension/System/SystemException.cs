// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
namespace System
{

    using System;
    using System.Runtime.Serialization;
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class SystemException : Exception
    {
        public SystemException()
            : base("Arg_SystemException")
        {
            base.HResult = __HResults.COR_E_SYSTEM;
        }

        public SystemException(String message)
            : base(message)
        {
            base.HResult = __HResults.COR_E_SYSTEM;
        }

        public SystemException(String message, Exception innerException)
            : base(message, innerException)
        {
            base.HResult = __HResults.COR_E_SYSTEM;
        }

        protected SystemException(SerializationInfo info, StreamingContext context) //: base(info, context)
        {
        }
    }
}

