// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (C) 2007-2015 Pivotal Software, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License.
//
//  The Original Code is RabbitMQ.
//
//  The Initial Developer of the Original Code is Pivotal Software, Inc.
//  Copyright (c) 2007-2015 Pivotal Software, Inc.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.Net;

namespace RabbitMQ.Client.Impl
{
    /// <summary> Instances of subclasses of subclasses
    /// HardProtocolException and SoftProtocolException are thrown in
    /// situations when we detect a problem with the connection-,
    /// channel- or wire-level parts of the AMQP protocol. </summary>
    public abstract class ProtocolException : ProtocolViolationException
    {
        protected ProtocolException(string message) : base(message)
        {
        }

        ///<summary>Retrieve the reply code to use in a
        ///connection/channel close method.</summary>
        public abstract ushort ReplyCode { get; }

        ///<summary>Retrieve the shutdown details to use in a
        ///connection/channel close method. Defaults to using
        ///ShutdownInitiator.Library, and this.ReplyCode and
        ///this.Message as the reply code and text,
        ///respectively.</summary>
        public virtual ShutdownEventArgs ShutdownReason
        {
            get { return new ShutdownEventArgs(ShutdownInitiator.Library, ReplyCode, Message, this); }
        }
    }
}
