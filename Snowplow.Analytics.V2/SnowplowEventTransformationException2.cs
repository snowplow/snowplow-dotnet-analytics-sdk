/*
 * SnowplowEventTransformationException.cs
 * Copyright (c) 2017 Snowplow Analytics Ltd. All rights reserved.
 * This program is licensed to you under the Apache License Version 2.0,
 * and you may not use this file except in compliance with the Apache License
 * Version 2.0. You may obtain a copy of the Apache License Version 2.0 at
 * http://www.apache.org/licenses/LICENSE-2.0.
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the Apache License Version 2.0 is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the Apache License Version 2.0 for the specific
 * language governing permissions and limitations there under.
 * Authors: Devesh Shetty
 * Copyright: Copyright (c) 2017 Snowplow Analytics Ltd
 * License: Apache License Version 2.0
 */

namespace Snowplow.Analytics.V2
{
    public class SnowplowEventTransformationException2 : Exception
    {
        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>The error messages.</value>
        public List<string> ErrorMessages { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The message.</value>
        public override string Message
        {
            get
            {
                return ConvertListToStringSequence(ErrorMessages);
            }
        }

        public SnowplowEventTransformationException2(List<string> messages)
            : base(ConvertListToStringSequence(messages))
        {
            ErrorMessages = messages;
        }

        public SnowplowEventTransformationException2(string message)
            : base(message)
        {
            ErrorMessages = new List<string>() { message };
        }

        private static string ConvertListToStringSequence(List<string> messages) => string.Join("\n", messages);

    }
}
