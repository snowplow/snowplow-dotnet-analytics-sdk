/*
 * JsonShredderTest.cs
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

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snowplow.Analytics.Exceptions;
using Snowplow.Analytics.Json;
using Snowplow.Analytics.V4;
using Xunit;

namespace Snowplow.Analytics.Tests.V4
{
    public class JsonShredder4Test
    {
        [Theory]
        [InlineData("contexts", "iglu:com.acme/duplicated/jsonschema/20-0-5", "contexts_com_acme_duplicated_20")]
        [InlineData("contexts", "iglu:com.pascal/VoidCase_dummy/jsonschema/80-0-5", "contexts_com_pascal_void_case_dummy_80")]
        [InlineData("unstruct_event", "iglu:com.dev/VoidCase32/jsonschema/9-0-5", "unstruct_event_com_dev_void_case32_9")]
        [InlineData("unstruct_event", "iglu:com.tesla/schema80TestDb_system/jsonschema/7-0-5", "unstruct_event_com_tesla_schema80_test_db_system_7")]
        public void TestFixSchema(string prefix, string igluUri, string expected)
        {
            var fixedSchema = JsonShredder4.FixSchema(prefix, igluUri);
            Assert.Equal(expected, fixedSchema);
        }

        [Fact]
        public void TestParseContexts()
        {
            var actual = "{\n  'schema': 'any',\n  'data': [\n    {\n      'schema': 'iglu:com.acme/duplicated/jsonschema/20-0-5',\n      'data': {\n        'value': 1\n      }\n    },\n    {\n      'schema': 'iglu:com.acme/duplicated/jsonschema/20-0-5',\n      'data': {\n        'value': 2\n      }\n    },\n    {\n      'schema': 'iglu:com.acme/unduplicated/jsonschema/1-0-0',\n      'data': {\n        'type': 'test'\n      }\n    }\n  ]\n}";

            var expected = new Dictionary<string, List<JToken>>()
            {
                {
                    "contexts_com_acme_duplicated_20", new List<JToken>()
                    {
                        JToken.Parse("{'value': 1}"),
                        JToken.Parse("{'value': 2}")
                    }
                },
                {
                    "contexts_com_acme_unduplicated_1", new List<JToken>()
                    {
                        JToken.Parse("{'type': 'test'}")
                    }
                }

            };

            var actualDict = JsonShredder.ParseContexts(actual);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actualDict));
        }

        [Fact]
        public void TestContextWithMalformedField()
        {
            SnowplowEventTransformationException exception = null;
            var malformedContext = "{\n  'schema': 'any',\n  'data': [\n    {\n      'schema': 'failing'\n    },\n    {\n      'schema': 'iglu:com.acme/unduplicated/jsonschema/1-0-0',\n      'data': {\n        'value': 2\n      }\n    }\n  ]\n}";

            try
            {
                JsonShredder.ParseContexts(malformedContext);
            }
            catch (SnowplowEventTransformationException sete)
            {
                exception = sete;
            }
            Assert.True(exception.Message.Equals("Could not extract inner data field from custom context."));
        }

        [Fact]
        public void TestContextWithMultipleMalformedFields()
        {
            SnowplowEventTransformationException exception = null;
            var malformedContext = "{\n  'schema': 'any',\n  'data': [\n    {\n      'schema': 'failing',\n      'data': {\n        'value': 1\n      }\n    },\n    {\n      'data': {\n        'value': 2\n      }\n    },\n    {\n      'schema': 'iglu:com.acme/unduplicated/jsonschema/1-0-0'\n    }\n  ]\n}";

            try
            {
                JsonShredder.ParseContexts(malformedContext);
            }
            catch (SnowplowEventTransformationException sete)
            {
                exception = sete;
            }

            var expectedExceptions = new List<string>() {
                "Schema failing does not conform to schema pattern.",
                "Context JSON did not contain a stringly typed schema field.",
                "Could not extract inner data field from custom context." };

            var exceptionList = expectedExceptions.Except(exception.ErrorMessages);
            Assert.Equal(0, exceptionList.Count());

        }

        [Fact]
        public void TestUnstruct()
        {
            var actual = "{\n  'schema': 'any',\n  'data': {\n    'schema': 'iglu:com.snowplowanalytics.snowplow/social_interaction/jsonschema/1-0-0',\n    'data': {\n      'action': 'like',\n      'network': 'fb'\n    }\n  }\n}";

            var expected = JObject.Parse(@"{
                                        'unstruct_event_com_snowplowanalytics_snowplow_social_interaction_1': 
                                            {
											    'action':'like',
											    'network':'fb'
									        }
										}");

            var actualDict = JsonShredder.ParseUnstruct(actual);

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actualDict));

        }


    }
}
