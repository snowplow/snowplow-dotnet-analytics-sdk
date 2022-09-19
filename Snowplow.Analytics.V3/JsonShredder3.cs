/*
 * JsonShredder.cs
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

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Snowplow.Analytics.V3
{
    /// <summary>
    /// Converts unstructured events and custom contexts to a format which the Elasticsearch
    /// mapper can understand
    /// </summary>
    public static class JsonShredder3
    {
        //Canonical Iglu Schema URI regex
        private static readonly string SCHEMA_PATTERN = @"^iglu:" +                      // Protocol
                                                      @"([a-zA-Z0-9-_.]+)/" +            // Vendor
                                                      @"([a-zA-Z0-9-_]+)/" +             // Name
                                                      @"([a-zA-Z0-9-_]+)/" +             // Format
                                                      @"([1-9][0-9]*" +                  // MODEL (cannot start with 0)
                                                      @"(?:-(?:0|[1-9][0-9]*)){2})$";    // REVISION and ADDITION
                                                                                         // Extract whole SchemaVer within single group


        /// <summary>
        /// Extracts the vendor, name, format and version from Iglu uri.
        /// </summary>
        /// <param name="uri">Iglu URI.</param>
        /// <returns>The schema information.</returns>
        /// <exception cref="SnowplowEventTransformationException">Thrown when it is not a valid uri</exception>
        private static Dictionary<string, string> ExtractSchema(string uri)
        {
            Match match = Regex.Match(uri, SCHEMA_PATTERN);

            if (match.Success)
            {
                var groups = match.Groups;

                return new Dictionary<string, string>(){
                    {"vendor", groups[1].Value},
                    {"name", groups[2].Value},
                    {"format", groups[3].Value},
                    {"version", groups[4].Value}
                };
            }
            else
            {
                throw new SnowplowEventTransformationException3($"Schema {uri} does not conform to schema pattern.");
            }

        }

        /// <summary>
        /// Transform Iglu URI into elasticsearch-compatible column name
        /// </summary>
        /// <example>
        /// "iglu:com.acme/PascalCase/jsonschema/13-0-0" -> "context_com_acme_pascal_case_13"
        /// </example>
        /// <param name="prefix">"context" or "unstruct_event".</param>
        /// <param name="igluUri">Schema field from an incoming JSON, should be already validated</param>
        /// <returns>Elasticsearch field name</returns>
        public static string FixSchema(string prefix, string igluUri)
        {
            var schemaDict = ExtractSchema(igluUri);

            // Split the vendor's reversed domain name using underscores rather than dots
            var snakeCaseOrganization = Regex.Replace(schemaDict["vendor"], @"[\.\-]", @"_").ToLower();

            // Change the name from PascalCase or lisp-case to snake_case if necessary
            var snakeCaseName = Regex.Replace(schemaDict["name"], @"[\.\-]", @"_");
            snakeCaseName = Regex.Replace(snakeCaseName, @"([^A-Z_])([A-Z])",
                                              (match) => match.Groups[1].Value + "_" + match.Groups[2].Value).ToLower();

            // Extract the schemaver version's model
            var model = schemaDict["version"].Split('-')[0];

            return $"{prefix}_{snakeCaseOrganization}_{snakeCaseName}_{model}";
        }

        /// <summary>
        /// Convert a contexts JSON to an Elasticsearch-compatible JObject
        /// </summary>
        /// <param name="contextJsons">List of inner custom context JSONs</param>
        /// <returns>List of validated tuples containing a fixed schema string and the original data JObject</returns>
        /// <exception cref="SnowplowEventTransformationException">Thrown when data or schema field is missing</exception>
        public static Func<string, JObject> ParseContexts = (contextJsons) =>
        {
            var json = JObject.Parse(contextJsons);
            var data = json["data"];

            if (data == null)
            {
                throw new SnowplowEventTransformationException3("Could not extract data field from custom context.");
            }

            var distinctContexts = new Dictionary<string, List<JToken>>();

            //storing all the encountered errors.
            var errors = new HashSet<string>();
            foreach (var context in data)
            {
                //an error flag
                var errorEncountered = false;

                var innerData = context["data"];
                if (innerData == null)
                {
                    errors.Add("Could not extract inner data field from custom context.");
                    errorEncountered = true;
                }

                var contextSchema = context["schema"];
                if (contextSchema == null)
                {
                    errors.Add("Context JSON did not contain a stringly typed schema field.");
                    errorEncountered = true;
                }

                if (errorEncountered)
                {
                    //no point in processing further
                    continue;
                }

                try
                {
                    const string CONTEXT_KEY = "contexts";
                    var schema = FixSchema(CONTEXT_KEY, context["schema"] !.Value<string>()!);

                    if (distinctContexts.ContainsKey(schema))
                    {
                        distinctContexts[schema].Add(innerData!);
                    }
                    else
                    {
                        distinctContexts.Add(schema, new List<JToken>() { innerData! });
                    }
                }
                catch (SnowplowEventTransformationException3 e)
                {
                    errors.Add(e.Message);
                }

            }

            if (errors.Count == 0)
            {
                return JObject.FromObject(distinctContexts);
            }
            else
            {
                throw new SnowplowEventTransformationException3(errors.ToList());
            }

        };

        /// <summary>
        /// Convert an unstructured event JSON to an Elasticsearch-compatible JObject
        /// </summary>
        /// <param name="unstruct">Unstructured event JSON</param>
        /// <returns>Unstructured event JSON in an Elasticsearch-compatible format</returns>
        /// <exception cref="SnowplowEventTransformationException">Thrown when data or schema field is missing</exception>
        public static Func<string, JObject> ParseUnstruct = (unstruct) =>
        {
            //will have all the errors we encounter
            var errors = new HashSet<string>();

            var json = JObject.Parse(unstruct);
            var data = json["data"];

            if (data == null)
            {
                throw new SnowplowEventTransformationException3("Could not extract data field from unstructured event.");
            }

            var schema = data["schema"];
            if (schema == null)
            {
                errors.Add("Unstructured event JSON did not contain a stringly typed schema field.");
            }

            var innerData = data["data"];
            if (innerData == null)
            {
                errors.Add("Could not extract inner data field from unstructured event.");
            }

            if (errors.Count != 0)
            {
                throw new SnowplowEventTransformationException3(errors.ToList());
            }

            const string UNSTRUCT_EVENT_KEY = "unstruct_event";
            var fixedSchema = FixSchema(UNSTRUCT_EVENT_KEY, schema!.Value<string>()!);

            var dict = new Dictionary<string, JToken>() { { fixedSchema, innerData! } };

            return JObject.FromObject(dict);
        };

    }
}
