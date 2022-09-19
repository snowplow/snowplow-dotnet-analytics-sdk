/*
 * Constants.cs
 * 
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

namespace Snowplow.Analytics
{
    public static class Constants
    {
        //Indexes to GeoPoint data
        public static class GeoPointIndexes
        {
            public static readonly int LATITUDE_INDEX = 22;
            public static readonly int LONGITUDE_INDEX = 23;
        }

        public static readonly string UNSTRUCT_EVENT_KEY = "unstruct_event";
        public static readonly string CONTEXT_KEY = "contexts";

    }
}
