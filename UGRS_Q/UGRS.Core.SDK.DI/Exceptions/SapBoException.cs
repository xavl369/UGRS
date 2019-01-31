﻿// file:	Exceptions\SapBoException.cs
// summary:	Implements the SapBo exception class

using System;
using System.Runtime.Serialization;

namespace UGRS.Core.SDK.DI.Exceptions
{
    /// <summary> (Serializable) exception for signalling SapBo errors. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    [Serializable]
    public class SapBoException : Exception
    {
        /// <summary> Default constructor. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>

        public SapBoException()
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <returns> A Tuple. </returns>

        public SapBoException(string pStrMessage)
            : base(pStrMessage)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrMessage"> The exception message. </param>
        /// <param name="pObjException"> The exception object. </param>
        /// <returns> A Tuple. </returns>

        public SapBoException(string pStrMessage, Exception pObjException)
            : base(pStrMessage, pObjException)
        {
        }

        /// <summary> Specialised constructor for use only by derived class. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pObjSerializationInfo"> Information describing the object serialization. </param>
        /// <param name="pObjContext"> Context for the object. </param>

        protected SapBoException(SerializationInfo pObjSerializationInfo, StreamingContext pObjContext)
        {
        }
    }
}
