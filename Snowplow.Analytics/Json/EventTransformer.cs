/*
 * EventTransformer.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snowplow.Analytics.Exceptions;

namespace Snowplow.Analytics.Json
{
    public static class EventTransformer
    {
        private static JObject StringField(string key, string val) => JObject.Parse("{ \"" + key + "\": \"" + ReformatString(val) + "\"}");
        private static JObject IntField(string key, string val) => JObject.Parse("{\"" + key + "\": " + int.Parse(val) + "}");
        private static JObject DoubleField(string key, string val) => JObject.Parse("{\"" + key + "\": " + double.Parse(val) + "}");
        private static JObject BoolField(string key, string val) => HandleBooleanField(key, val);
        private static JObject TstampField(string key, string val) => JObject.Parse("{\"" + key + "\": \"" + ReformatTstamp(val) + "\"}");
        private static JObject CustomContextsField(string key, string val) => JsonShredder.ParseContexts(val);
        private static JObject DerivedContextsField(string key, string val) => JsonShredder.ParseContexts(val);
        private static JObject UnstructField(string key, string val) => JsonShredder.ParseUnstruct(val);

        private static readonly Dictionary<string, Func<string, string, JObject>>
            ENRICHED_EVENT_FIELD_TYPES = new Dictionary<string, Func<string, string, JObject>>()
            {
                {"app_id", StringField},
                {"platform", StringField},
                {"etl_tstamp", TstampField},
                {"collector_tstamp", TstampField},
                {"dvce_created_tstamp", TstampField},
                {"event", StringField},
                {"event_id", StringField},
                {"txn_id", IntField},
                {"name_tracker", StringField},
                {"v_tracker", StringField},
                {"v_collector", StringField},
                {"v_etl", StringField},
                {"user_id", StringField},
                {"user_ipaddress", StringField},
                {"user_fingerprint", StringField},
                {"domain_userid", StringField},
                {"domain_sessionidx", IntField},
                {"network_userid", StringField},
                {"geo_country", StringField},
                {"geo_region", StringField},
                {"geo_city", StringField},
                {"geo_zipcode", StringField},
                {"geo_latitude", DoubleField},
                {"geo_longitude", DoubleField},
                {"geo_region_name", StringField},
                {"ip_isp", StringField},
                {"ip_organization", StringField},
                {"ip_domain", StringField},
                {"ip_netspeed", StringField},
                {"page_url", StringField},
                {"page_title", StringField},
                {"page_referrer", StringField},
                {"page_urlscheme", StringField},
                {"page_urlhost", StringField},
                {"page_urlport", IntField},
                {"page_urlpath", StringField},
                {"page_urlquery", StringField},
                {"page_urlfragment", StringField},
                {"refr_urlscheme", StringField},
                {"refr_urlhost", StringField},
                {"refr_urlport", IntField},
                {"refr_urlpath", StringField},
                {"refr_urlquery", StringField},
                {"refr_urlfragment", StringField},
                {"refr_medium", StringField},
                {"refr_source", StringField},
                {"refr_term", StringField},
                {"mkt_medium", StringField},
                {"mkt_source", StringField},
                {"mkt_term", StringField},
                {"mkt_content", StringField},
                {"mkt_campaign", StringField},
                {"contexts", CustomContextsField},
                {"se_category", StringField},
                {"se_action", StringField},
                {"se_label", StringField},
                {"se_property", StringField},
                {"se_value", StringField},
                {"unstruct_event", UnstructField},
                {"tr_orderid", StringField},
                {"tr_affiliation", StringField},
                {"tr_total", DoubleField},
                {"tr_tax", DoubleField},
                {"tr_shipping", DoubleField},
                {"tr_city", StringField},
                {"tr_state", StringField},
                {"tr_country", StringField},
                {"ti_orderid", StringField},
                {"ti_sku", StringField},
                {"ti_name", StringField},
                {"ti_category", StringField},
                {"ti_price", DoubleField},
                {"ti_quantity", IntField},
                {"pp_xoffset_min", IntField},
                {"pp_xoffset_max", IntField},
                {"pp_yoffset_min", IntField},
                {"pp_yoffset_max", IntField},
                {"useragent", StringField},
                {"br_name", StringField},
                {"br_family", StringField},
                {"br_version", StringField},
                {"br_type", StringField},
                {"br_renderengine", StringField},
                {"br_lang", StringField},
                {"br_features_pdf", BoolField},
                {"br_features_flash", BoolField},
                {"br_features_java", BoolField},
                {"br_features_director", BoolField},
                {"br_features_quicktime", BoolField},
                {"br_features_realplayer", BoolField},
                {"br_features_windowsmedia", BoolField},
                {"br_features_gears", BoolField},
                {"br_features_silverlight", BoolField},
                {"br_cookies", BoolField},
                {"br_colordepth", StringField},
                {"br_viewwidth", IntField},
                {"br_viewheight", IntField},
                {"os_name", StringField},
                {"os_family", StringField},
                {"os_manufacturer", StringField},
                {"os_timezone", StringField},
                {"dvce_type", StringField},
                {"dvce_ismobile", BoolField},
                {"dvce_screenwidth", IntField},
                {"dvce_screenheight", IntField},
                {"doc_charset", StringField},
                {"doc_width", IntField},
                {"doc_height", IntField},
                {"tr_currency", StringField},
                {"tr_total_base", DoubleField},
                {"tr_tax_base", DoubleField},
                {"tr_shipping_base", DoubleField},
                {"ti_currency", StringField},
                {"ti_price_base", DoubleField},
                {"base_currency", StringField},
                {"geo_timezone", StringField},
                {"mkt_clickid", StringField},
                {"mkt_network", StringField},
                {"etl_tags", StringField},
                {"dvce_sent_tstamp", TstampField},
                {"refr_domain_userid", StringField},
                {"refr_device_tstamp", TstampField},
                {"derived_contexts", DerivedContextsField},
                {"domain_sessionid", StringField},
                {"derived_tstamp", TstampField},
                {"event_vendor", StringField},
                {"event_name", StringField},
                {"event_format", StringField},
                {"event_version", StringField},
                {"event_fingerprint", StringField},
                {"true_tstamp", TstampField}
            };


        /// <summary>
        /// Convert a string with Enriched event TSV to a JSON string
        /// </summary>
        /// <param name="line">enriched event TSV line.</param>
        /// <returns>ValidatedRecord for the event</returns>
        public static string Transform(String line) => JsonifyGoodEvent(line.Split('\t'));

        /// <summary>
        /// Converts an array of field values to a JSON whose keys are the field names
        /// </summary>
        /// <param name="eventArray">Array of values for the event</param>
        /// <exception cref="SnowplowEventTransformationException"> </exception>
        /// <returns>ValidatedRecord containing JSON for the event and the event_id (if it exists)</returns>
        private static Func<string[], string> JsonifyGoodEvent = (eventArray) =>
        {
            if (eventArray.Length != ENRICHED_EVENT_FIELD_TYPES.Count)
            {
                throw new SnowplowEventTransformationException($"Expected {ENRICHED_EVENT_FIELD_TYPES.Count} fields, " +
                                                               $"received {eventArray.Length} fields.");
            }
            else
            {
                //contains keys of entities to be ignored in the output if they are null
                var ignoreKeyList = new List<string>() { "contexts", "derived_contexts", "unstruct_event" };

                JObject output = new JObject();
                var latitude = eventArray[Constants.GeoPointIndexes.LATITUDE_INDEX];
                var longitude = eventArray[Constants.GeoPointIndexes.LONGITUDE_INDEX];

                if (!string.IsNullOrEmpty(latitude)
                    && !string.IsNullOrEmpty(longitude))
                {
                    output.Add("geo_location", $"{latitude},{longitude}");
                }

                List<string> errors = new List<string>();
                var keys = new List<string>(ENRICHED_EVENT_FIELD_TYPES.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    if (string.IsNullOrEmpty(eventArray[i]))
                    {
                        if (ignoreKeyList.Any(item => (string.Compare(item, key, StringComparison.CurrentCulture) == 0)))
                        {
                            //don't include keys of the above list if they have null values
                            continue;
                        }

                        output[key] = null;
                    }
                    else
                    {
                        try
                        {
                            JObject obj = ENRICHED_EVENT_FIELD_TYPES[key](key, eventArray[i]);

                            foreach (var prop in obj)
                            {
                                output[prop.Key] = prop.Value;
                            }
                        }
                        catch (SnowplowEventTransformationException sete)
                        {
                            errors.Add(sete.Message);
                        }
                        catch (Exception e)
                        {
                            errors.Add($"Unexpected exception parsing field with key {key} and value {eventArray[i]}: {e.Message}");
                        }
                    }

                }

                if (errors.Count == 0)
                {
                    return JsonConvert.SerializeObject(output);
                }
                else
                {
                    throw new SnowplowEventTransformationException(errors);
                }

            }
        };


        /// <summary>
        /// Converts "0" to false and "1" to true
        /// </summary>
        /// <param name="key">The field name</param>
        /// <param name="val">The field value - should be "0" or "1"</param>
        /// <exception cref="SnowplowEventTransformationException"> Thrown when field value is not "0" or "1"</exception>
        /// <returns>Validated JObject</returns>
        private static Func<string, string, JObject> HandleBooleanField = (key, val) =>
        {
            if (val.Equals("1"))
            {
                return JObject.Parse("{\"" + key + "\": true }");
            }
            else if (val.Equals("0"))
            {
                return JObject.Parse("{\"" + key + "\": false }");
            }

            throw new SnowplowEventTransformationException($"Invalid value {val} for field {key}");
        };

        /// <summary>
        /// Converts a timestamp to ISO 8601 format
        /// </summary>
        /// <param name="tstamp">Timestamp of the form YYYY-MM-DD hh:mm:ss</param>
        /// <returns>ISO 8601 timestamp</returns>
        private static string ReformatTstamp(string tstamp) => tstamp.Replace(" ", "T") + "Z";

        private static string ReformatString(string s) => s.Replace("\"", "\\\"");
    }
}
