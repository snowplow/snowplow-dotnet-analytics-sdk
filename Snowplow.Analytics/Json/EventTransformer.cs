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
using Newtonsoft.Json.Linq;
using Snowplow.Analytics.Exceptions;

namespace Snowplow.Analytics.Json
{
    public class EventTransformer
    {
        private static Func<string, string, JObject> StringField = (key, val) => JObject.FromObject(new { key = val });
        private static Func<string, string, JObject> IntField = (key, val) => JObject.FromObject(new { key = int.Parse(val) });
        private static Func<string, string, JObject> DoubleField = (key, val) => JObject.FromObject(new { key = double.Parse(val) });
        private static Func<string, string, JObject> BoolField = (key, val) => HandleBooleanField(key, val);
        private static Func<string, string, JObject> TstampField = (key, val) => JObject.FromObject(new { key = ReformatTstamp(val) });

        private static Func<string, string, JObject> CustomContextsField = (key, val) => new JObject();
		private static Func<string, string, JObject> DerivedContextsField = (key, val) => new JObject();
		private static Func<string, string, JObject> UnstructField = (key, val) => new JObject();


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
		public static string Transform(string line)
        {
            return String.Empty;
        }

        /// <summary>
        /// Converts "0" to false and "1" to true
        /// </summary>
        /// <param name="key">The field name</param>
        /// <param name="val">The field value - should be "0" or "1"</param>
        /// <exception cref="SnowplowEventTransformationException"> Thrown when field value is not "0" or "1"</exception>
        /// <returns>Validated JObject</returns>
        private static Func<string, string, JObject> HandleBooleanField = (key, val) => {
            if (val.Equals("1"))
			{
				return JObject.FromObject(new { key = true });
			}
            else if (val.Equals("0"))
			{
				return JObject.FromObject(new { key = false });
			}

			throw new SnowplowEventTransformationException($"Invalid value {val} for field {key}");
        };

		/// <summary>
		/// Converts a timestamp to ISO 8601 format
		/// </summary>
		/// <param name="tstamp">Timestamp of the form YYYY-MM-DD hh:mm:ss</param>
		/// <returns>ISO 8601 timestamp</returns>
		private static Func<string, string> ReformatTstamp = (tstamp) => tstamp.Replace(" ", "T") + "Z";


	}
}
