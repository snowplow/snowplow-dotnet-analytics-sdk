/*
 * EventTransformerTest.cs
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
using Snowplow.Analytics.Json;
using Xunit;

namespace Snowplow.Analytics.Tests.Json
{
	public class EventTransformerTest
	{
		private static readonly string _unstructJson = "{\n    'schema': 'iglu:com.snowplowanalytics.snowplow/contexts/jsonschema/1-0-0',\n    'data': {\n      'schema': 'iglu:com.snowplowanalytics.snowplow/link_click/jsonschema/1-0-1',\n      'data': {\n        'targetUrl': 'http://www.example.com',\n        'elementClasses': ['foreground'],\n        'elementId': 'exampleLink'\n      }\n    }\n  }";

		private static readonly string _contextsJson = "{\n    'schema': 'iglu:com.snowplowanalytics.snowplow/contexts/jsonschema/1-0-0',\n    'data': [\n      {\n        'schema': 'iglu:org.schema/WebPage/jsonschema/1-0-0',\n        'data': {\n          'genre': 'blog',\n          'inLanguage': 'en-US',\n          'datePublished': '2014-11-06T00:00:00Z',\n          'author': 'Devesh Shetty',\n          'breadcrumb': [\n            'blog',\n            'releases'\n          ],\n          'keywords': [\n            'snowplow',\n            'javascript',\n            'tracker',\n            'event'\n          ]\n        }\n      },\n      {\n        'schema': 'iglu:org.w3/PerformanceTiming/jsonschema/1-0-0',\n        'data': {\n          'navigationStart': 1415358089861,\n          'unloadEventStart': 1415358090270,\n          'unloadEventEnd': 1415358090287,\n          'redirectStart': 0,\n          'redirectEnd': 0,\n          'fetchStart': 1415358089870,\n          'domainLookupStart': 1415358090102,\n          'domainLookupEnd': 1415358090102,\n          'connectStart': 1415358090103,\n          'connectEnd': 1415358090183,\n          'requestStart': 1415358090183,\n          'responseStart': 1415358090265,\n          'responseEnd': 1415358090265,\n          'domLoading': 1415358090270,\n          'domInteractive': 1415358090886,\n          'domContentLoadedEventStart': 1415358090968,\n          'domContentLoadedEventEnd': 1415358091309,\n          'domComplete': 0,\n          'loadEventStart': 0,\n          'loadEventEnd': 0\n        }\n      }\n    ]\n  }";

		private static readonly string _derivedContextsJson = "{\n    'schema': 'iglu:com.snowplowanalytics.snowplow\\/contexts\\/jsonschema\\/1-0-1',\n    'data': [\n      {\n        'schema': 'iglu:com.snowplowanalytics.snowplow\\/ua_parser_context\\/jsonschema\\/1-0-0',\n        'data': {\n          'useragentFamily': 'IE',\n          'useragentMajor': '7',\n          'useragentMinor': '0',\n          'useragentPatch': null,\n          'useragentVersion': 'IE 7.0',\n          'osFamily': 'Windows XP',\n          'osMajor': null,\n          'osMinor': null,\n          'osPatch': null,\n          'osPatchMinor': null,\n          'osVersion': 'Windows XP',\n          'deviceFamily': 'Other'\n        }\n      }\n    ]\n  }";

		private Dictionary<string, string> GetInputWithContextAndUnstructEvent()
		{
			return new Dictionary<string, string>(){
				{ "app_id", "angry-birds"},
				{ "platform", "web"},
				{ "etl_tstamp", "2017-01-26 00:01:25.292"},
				{ "collector_tstamp", "2013-11-26 00:02:05"},
				{ "dvce_created_tstamp", "2013-11-26 00:03:57.885"},
				{ "event", "page_view"},
				{ "event_id", "c6ef3124-b53a-4b13-a233-0088f79dcbcb"},
				{ "txn_id", "41828"},
				{ "name_tracker", "cloudfront-1"},
				{ "v_tracker", "js-2.1.0"},
				{ "v_collector", "clj-tomcat-0.1.0"},
				{ "v_etl", "serde-0.5.2"},
				{ "user_id", "jon.doe@email.com"},
				{ "user_ipaddress", "92.231.54.234"},
				{ "user_fingerprint", "2161814971"},
				{ "domain_userid", "bc2e92ec6c204a14"},
				{ "domain_sessionidx", "3"},
				{ "network_userid", "ecdff4d0-9175-40ac-a8bb-325c49733607"},
				{ "geo_country", "US"},
				{ "geo_region", "TX"},
				{ "geo_city", "New York"},
				{ "geo_zipcode", "94109"},
				{ "geo_latitude", "37.443604"},
				{ "geo_longitude", "-122.4124"},
				{ "geo_region_name", "Florida"},
				{ "ip_isp", "FDN Communications"},
				{ "ip_organization", "Bouygues Telecom"},
				{ "ip_domain", "nuvox.net"},
				{ "ip_netspeed", "Cable/DSL"},
				{ "page_url", "http://www.snowplowanalytics.com"},
				{ "page_title", "On Analytics"},
				{ "page_referrer", ""},
				{ "page_urlscheme", "http"},
				{ "page_urlhost", "www.snowplowanalytics.com"},
				{ "page_urlport", "80"},
				{ "page_urlpath", "/product/index.html"},
				{ "page_urlquery", "id=GTM-DLRG"},
				{ "page_urlfragment", "4-conclusion"},
				{ "refr_urlscheme", ""},
				{ "refr_urlhost", ""},
				{ "refr_urlport", ""},
				{ "refr_urlpath", ""},
				{ "refr_urlquery", ""},
				{ "refr_urlfragment", ""},
				{ "refr_medium", ""},
				{ "refr_source", ""},
				{ "refr_term", ""},
				{ "mkt_medium", ""},
				{ "mkt_source", ""},
				{ "mkt_term", ""},
				{ "mkt_content", ""},
				{ "mkt_campaign", ""},
				{ "contexts", _contextsJson},
				{ "se_category", ""},
				{ "se_action", ""},
				{ "se_label", ""},
				{ "se_property", ""},
				{ "se_value", ""},
				{ "unstruct_event", _unstructJson},
				{ "tr_orderid", ""},
				{ "tr_affiliation", ""},
				{ "tr_total", ""},
				{ "tr_tax", ""},
				{ "tr_shipping", ""},
				{ "tr_city", ""},
				{ "tr_state", ""},
				{ "tr_country", ""},
				{ "ti_orderid", ""},
				{ "ti_sku", ""},
				{ "ti_name", ""},
				{ "ti_category", ""},
				{ "ti_price", ""},
				{ "ti_quantity", ""},
				{ "pp_xoffset_min", ""},
				{ "pp_xoffset_max", ""},
				{ "pp_yoffset_min", ""},
				{ "pp_yoffset_max", ""},
				{ "useragent", ""},
				{ "br_name", ""},
				{ "br_family", ""},
				{ "br_version", ""},
				{ "br_type", ""},
				{ "br_renderengine", ""},
				{ "br_lang", ""},
				{ "br_features_pdf", "1"},
				{ "br_features_flash", "0"},
				{ "br_features_java", ""},
				{ "br_features_director", ""},
				{ "br_features_quicktime", ""},
				{ "br_features_realplayer", ""},
				{ "br_features_windowsmedia", ""},
				{ "br_features_gears", ""},
				{ "br_features_silverlight", ""},
				{ "br_cookies", ""},
				{ "br_colordepth", ""},
				{ "br_viewwidth", ""},
				{ "br_viewheight", ""},
				{ "os_name", ""},
				{ "os_family", ""},
				{ "os_manufacturer", ""},
				{ "os_timezone", ""},
				{ "dvce_type", ""},
				{ "dvce_ismobile", ""},
				{ "dvce_screenwidth", ""},
				{ "dvce_screenheight", ""},
				{ "doc_charset", ""},
				{ "doc_width", ""},
				{ "doc_height", ""},
				{ "tr_currency", ""},
				{ "tr_total_base", ""},
				{ "tr_tax_base", ""},
				{ "tr_shipping_base", ""},
				{ "ti_currency", ""},
				{ "ti_price_base", ""},
				{ "base_currency", ""},
				{ "geo_timezone", ""},
				{ "mkt_clickid", ""},
				{ "mkt_network", ""},
				{ "etl_tags", ""},
				{ "dvce_sent_tstamp", ""},
				{ "refr_domain_userid", ""},
				{ "refr_device_tstamp", ""},
				{ "derived_contexts", _derivedContextsJson},
				{ "domain_sessionid", "2b15e5c8-d3b1-11e4-b9d6-1681e6b88ec1"},
				{ "derived_tstamp", "2013-11-26 00:03:57.886"},
				{ "event_vendor", "com.snowplowanalytics.snowplow"},
				{ "event_name", "link_click"},
				{ "event_format", "jsonschema"},
				{ "event_version", "1-0-0"},
				{ "event_fingerprint", "e3dbfa9cca0412c3d4052863cefb547f"},
				{ "true_tstamp", "2013-11-26 00:03:57.886"}
			};
		}

		private Dictionary<string, string> GetInputWithoutContextAndUnstructEvent()
		{
			return new Dictionary<string, string>(){
				{ "app_id", "angry-birds"},
				{ "platform", "web"},
				{ "etl_tstamp", "2017-01-26 00:01:25.292"},
				{ "collector_tstamp", "2013-11-26 00:02:05"},
				{ "dvce_created_tstamp", "2013-11-26 00:03:57.885"},
				{ "event", "page_view"},
				{ "event_id", "c6ef3124-b53a-4b13-a233-0088f79dcbcb"},
				{ "txn_id", "41828"},
				{ "name_tracker", "cloudfront-1"},
				{ "v_tracker", "js-2.1.0"},
				{ "v_collector", "clj-tomcat-0.1.0"},
				{ "v_etl", "serde-0.5.2"},
				{ "user_id", "jon.doe@email.com"},
				{ "user_ipaddress", "92.231.54.234"},
				{ "user_fingerprint", "2161814971"},
				{ "domain_userid", "bc2e92ec6c204a14"},
				{ "domain_sessionidx", "3"},
				{ "network_userid", "ecdff4d0-9175-40ac-a8bb-325c49733607"},
				{ "geo_country", "US"},
				{ "geo_region", "TX"},
				{ "geo_city", "New York"},
				{ "geo_zipcode", "94109"},
				{ "geo_latitude", "37.443604"},
				{ "geo_longitude", "-122.4124"},
				{ "geo_region_name", "Florida"},
				{ "ip_isp", "FDN Communications"},
				{ "ip_organization", "Bouygues Telecom"},
				{ "ip_domain", "nuvox.net"},
				{ "ip_netspeed", "Cable/DSL"},
				{ "page_url", "http://www.snowplowanalytics.com"},
				{ "page_title", "On Analytics"},
				{ "page_referrer", ""},
				{ "page_urlscheme", "http"},
				{ "page_urlhost", "www.snowplowanalytics.com"},
				{ "page_urlport", "80"},
				{ "page_urlpath", "/product/index.html"},
				{ "page_urlquery", "id=GTM-DLRG"},
				{ "page_urlfragment", "4-conclusion"},
				{ "refr_urlscheme", ""},
				{ "refr_urlhost", ""},
				{ "refr_urlport", ""},
				{ "refr_urlpath", ""},
				{ "refr_urlquery", ""},
				{ "refr_urlfragment", ""},
				{ "refr_medium", ""},
				{ "refr_source", ""},
				{ "refr_term", ""},
				{ "mkt_medium", ""},
				{ "mkt_source", ""},
				{ "mkt_term", ""},
				{ "mkt_content", ""},
				{ "mkt_campaign", ""},
				{ "contexts", ""},
				{ "se_category", ""},
				{ "se_action", ""},
				{ "se_label", ""},
				{ "se_property", ""},
				{ "se_value", ""},
				{ "unstruct_event", ""},
				{ "tr_orderid", ""},
				{ "tr_affiliation", ""},
				{ "tr_total", ""},
				{ "tr_tax", ""},
				{ "tr_shipping", ""},
				{ "tr_city", ""},
				{ "tr_state", ""},
				{ "tr_country", ""},
				{ "ti_orderid", ""},
				{ "ti_sku", ""},
				{ "ti_name", ""},
				{ "ti_category", ""},
				{ "ti_price", ""},
				{ "ti_quantity", ""},
				{ "pp_xoffset_min", ""},
				{ "pp_xoffset_max", ""},
				{ "pp_yoffset_min", ""},
				{ "pp_yoffset_max", ""},
				{ "useragent", ""},
				{ "br_name", ""},
				{ "br_family", ""},
				{ "br_version", ""},
				{ "br_type", ""},
				{ "br_renderengine", ""},
				{ "br_lang", ""},
				{ "br_features_pdf", "1"},
				{ "br_features_flash", "0"},
				{ "br_features_java", ""},
				{ "br_features_director", ""},
				{ "br_features_quicktime", ""},
				{ "br_features_realplayer", ""},
				{ "br_features_windowsmedia", ""},
				{ "br_features_gears", ""},
				{ "br_features_silverlight", ""},
				{ "br_cookies", ""},
				{ "br_colordepth", ""},
				{ "br_viewwidth", ""},
				{ "br_viewheight", ""},
				{ "os_name", ""},
				{ "os_family", ""},
				{ "os_manufacturer", ""},
				{ "os_timezone", ""},
				{ "dvce_type", ""},
				{ "dvce_ismobile", ""},
				{ "dvce_screenwidth", ""},
				{ "dvce_screenheight", ""},
				{ "doc_charset", ""},
				{ "doc_width", ""},
				{ "doc_height", ""},
				{ "tr_currency", ""},
				{ "tr_total_base", ""},
				{ "tr_tax_base", ""},
				{ "tr_shipping_base", ""},
				{ "ti_currency", ""},
				{ "ti_price_base", ""},
				{ "base_currency", ""},
				{ "geo_timezone", ""},
				{ "mkt_clickid", ""},
				{ "mkt_network", ""},
				{ "etl_tags", ""},
				{ "dvce_sent_tstamp", ""},
				{ "refr_domain_userid", ""},
				{ "refr_device_tstamp", ""},
				{ "derived_contexts", ""},
				{ "domain_sessionid", "2b15e5c8-d3b1-11e4-b9d6-1681e6b88ec1"},
				{ "derived_tstamp", "2013-11-26 00:03:57.886"},
				{ "event_vendor", "com.snowplowanalytics.snowplow"},
				{ "event_name", "link_click"},
				{ "event_format", "jsonschema"},
				{ "event_version", "1-0-0"},
				{ "event_fingerprint", "e3dbfa9cca0412c3d4052863cefb547f"},
				{ "true_tstamp", "2013-11-26 00:03:57.886"}
			};
		}

		private string GetSerializedExpectedOutputForInputWithContextAndUnstructEvent()
		{
			var expected = JObject.Parse(@"{
                'geo_location' : '37.443604,-122.4124',
                'app_id' : 'angry-birds',
                'platform' : 'web',
                'etl_tstamp' : '2017-01-26T00:01:25.292Z',
                'collector_tstamp' : '2013-11-26T00:02:05Z',
                'dvce_created_tstamp' : '2013-11-26T00:03:57.885Z',
                'event' : 'page_view',
                'event_id' : 'c6ef3124-b53a-4b13-a233-0088f79dcbcb',
                'txn_id' : 41828,
                'name_tracker' : 'cloudfront-1',
                'v_tracker' : 'js-2.1.0',
                'v_collector' : 'clj-tomcat-0.1.0',
                'v_etl' : 'serde-0.5.2',
                'user_id' : 'jon.doe@email.com',
                'user_ipaddress' : '92.231.54.234',
                'user_fingerprint' : '2161814971',
                'domain_userid' : 'bc2e92ec6c204a14',
                'domain_sessionidx' : 3,
                'network_userid' : 'ecdff4d0-9175-40ac-a8bb-325c49733607',
                'geo_country' : 'US',
                'geo_region' : 'TX',
                'geo_city' : 'New York',
                'geo_zipcode' : '94109',
                'geo_latitude' : 37.443604,
                'geo_longitude' : -122.4124,
                'geo_region_name' : 'Florida',
                'ip_isp' : 'FDN Communications',
                'ip_organization' : 'Bouygues Telecom',
                'ip_domain' : 'nuvox.net',
                'ip_netspeed' : 'Cable/DSL',
                'page_url' : 'http://www.snowplowanalytics.com',
                'page_title' : 'On Analytics',
                'page_referrer' : null,
                'page_urlscheme' : 'http',
                'page_urlhost' : 'www.snowplowanalytics.com',
                'page_urlport' : 80,
                'page_urlpath' : '/product/index.html',
                'page_urlquery' : 'id=GTM-DLRG',
                'page_urlfragment' : '4-conclusion',
                'refr_urlscheme' : null,
                'refr_urlhost' : null,
                'refr_urlport' : null,
                'refr_urlpath' : null,
                'refr_urlquery' : null,
                'refr_urlfragment' : null,
                'refr_medium' : null,
                'refr_source' : null,
                'refr_term' : null,
                'mkt_medium' : null,
                'mkt_source' : null,
                'mkt_term' : null,
                'mkt_content' : null,
                'mkt_campaign' : null,
                'contexts_org_schema_web_page_1' : [ {
                  'genre' : 'blog',
                  'inLanguage' : 'en-US',
                  'datePublished' : '2014-11-06T00:00:00Z',
                  'author' : 'Devesh Shetty',
                  'breadcrumb' : ['blog', 'releases'],
                      'keywords' : [ 'snowplow', 'javascript', 'tracker', 'event' ]
                } ],
                'contexts_org_w3_performance_timing_1' : [ {
                  'navigationStart' : 1415358089861,
                  'unloadEventStart' : 1415358090270,
                  'unloadEventEnd' : 1415358090287,
                  'redirectStart' : 0,
                  'redirectEnd' : 0,
                  'fetchStart' : 1415358089870,
                  'domainLookupStart' : 1415358090102,
                  'domainLookupEnd' : 1415358090102,
                  'connectStart' : 1415358090103,
                  'connectEnd' : 1415358090183,
                  'requestStart' : 1415358090183,
                  'responseStart' : 1415358090265,
                  'responseEnd' : 1415358090265,
                  'domLoading' : 1415358090270,
                  'domInteractive' : 1415358090886,
                  'domContentLoadedEventStart' : 1415358090968,
                  'domContentLoadedEventEnd' : 1415358091309,
                  'domComplete' : 0,
                  'loadEventStart' : 0,
                  'loadEventEnd' : 0
                } ],
                'se_category' : null,
                'se_action' : null,
                'se_label' : null,
                'se_property' : null,
                'se_value' : null,
                'unstruct_event_com_snowplowanalytics_snowplow_link_click_1' : {
                  'targetUrl' : 'http://www.example.com',
                  'elementClasses' : [ 'foreground' ],
                  'elementId' : 'exampleLink'
                },
                'tr_orderid' : null,
                'tr_affiliation' : null,
                'tr_total' : null,
                'tr_tax' : null,
                'tr_shipping' : null,
                'tr_city' : null,
                'tr_state' : null,
                'tr_country' : null,
                'ti_orderid' : null,
                'ti_sku' : null,
                'ti_name' : null,
                'ti_category' : null,
                'ti_price' : null,
                'ti_quantity' : null,
                'pp_xoffset_min' : null,
                'pp_xoffset_max' : null,
                'pp_yoffset_min' : null,
                'pp_yoffset_max' : null,
                'useragent' : null,
                'br_name' : null,
                'br_family' : null,
                'br_version' : null,
                'br_type' : null,
                'br_renderengine' : null,
                'br_lang' : null,
                'br_features_pdf' : true,
                'br_features_flash' : false,
                'br_features_java' : null,
                'br_features_director' : null,
                'br_features_quicktime' : null,
                'br_features_realplayer' : null,
                'br_features_windowsmedia' : null,
                'br_features_gears' : null,
                'br_features_silverlight' : null,
                'br_cookies' : null,
                'br_colordepth' : null,
                'br_viewwidth' : null,
                'br_viewheight' : null,
                'os_name' : null,
                'os_family' : null,
                'os_manufacturer' : null,
                'os_timezone' : null,
                'dvce_type' : null,
                'dvce_ismobile' : null,
                'dvce_screenwidth' : null,
                'dvce_screenheight' : null,
                'doc_charset' : null,
                'doc_width' : null,
                'doc_height' : null,
                'tr_currency' : null,
                'tr_total_base' : null,
                'tr_tax_base' : null,
                'tr_shipping_base' : null,
                'ti_currency' : null,
                'ti_price_base' : null,
                'base_currency' : null,
                'geo_timezone' : null,
                'mkt_clickid' : null,
                'mkt_network' : null,
                'etl_tags' : null,
                'dvce_sent_tstamp' : null,
                'refr_domain_userid' : null,
                'refr_device_tstamp' : null,
                'contexts_com_snowplowanalytics_snowplow_ua_parser_context_1': [{
                  'useragentFamily': 'IE',
                  'useragentMajor': '7',
                  'useragentMinor': '0',
                  'useragentPatch': null,
                  'useragentVersion': 'IE 7.0',
                  'osFamily': 'Windows XP',
                  'osMajor': null,
                  'osMinor': null,
                  'osPatch': null,
                  'osPatchMinor': null,
                  'osVersion': 'Windows XP',
                  'deviceFamily': 'Other'
                }],
                'domain_sessionid': '2b15e5c8-d3b1-11e4-b9d6-1681e6b88ec1',
                'derived_tstamp': '2013-11-26T00:03:57.886Z',
                'event_vendor': 'com.snowplowanalytics.snowplow',
                'event_name': 'link_click',
                'event_format': 'jsonschema',
                'event_version': '1-0-0',
                'event_fingerprint': 'e3dbfa9cca0412c3d4052863cefb547f',
                'true_tstamp': '2013-11-26T00:03:57.886Z'
                }");

			return JsonConvert.SerializeObject(expected);
		}

		private string GetSerializedExpectedOutputForInputWithoutContextAndUnstructEvent()
		{
			var expected = JObject.Parse(@"{
                'geo_location' : '37.443604,-122.4124',
                'app_id' : 'angry-birds',
                'platform' : 'web',
                'etl_tstamp' : '2017-01-26T00:01:25.292Z',
                'collector_tstamp' : '2013-11-26T00:02:05Z',
                'dvce_created_tstamp' : '2013-11-26T00:03:57.885Z',
                'event' : 'page_view',
                'event_id' : 'c6ef3124-b53a-4b13-a233-0088f79dcbcb',
                'txn_id' : 41828,
                'name_tracker' : 'cloudfront-1',
                'v_tracker' : 'js-2.1.0',
                'v_collector' : 'clj-tomcat-0.1.0',
                'v_etl' : 'serde-0.5.2',
                'user_id' : 'jon.doe@email.com',
                'user_ipaddress' : '92.231.54.234',
                'user_fingerprint' : '2161814971',
                'domain_userid' : 'bc2e92ec6c204a14',
                'domain_sessionidx' : 3,
                'network_userid' : 'ecdff4d0-9175-40ac-a8bb-325c49733607',
                'geo_country' : 'US',
                'geo_region' : 'TX',
                'geo_city' : 'New York',
                'geo_zipcode' : '94109',
                'geo_latitude' : 37.443604,
                'geo_longitude' : -122.4124,
                'geo_region_name' : 'Florida',
                'ip_isp' : 'FDN Communications',
                'ip_organization' : 'Bouygues Telecom',
                'ip_domain' : 'nuvox.net',
                'ip_netspeed' : 'Cable/DSL',
                'page_url' : 'http://www.snowplowanalytics.com',
                'page_title' : 'On Analytics',
                'page_referrer' : null,
                'page_urlscheme' : 'http',
                'page_urlhost' : 'www.snowplowanalytics.com',
                'page_urlport' : 80,
                'page_urlpath' : '/product/index.html',
                'page_urlquery' : 'id=GTM-DLRG',
                'page_urlfragment' : '4-conclusion',
                'refr_urlscheme' : null,
                'refr_urlhost' : null,
                'refr_urlport' : null,
                'refr_urlpath' : null,
                'refr_urlquery' : null,
                'refr_urlfragment' : null,
                'refr_medium' : null,
                'refr_source' : null,
                'refr_term' : null,
                'mkt_medium' : null,
                'mkt_source' : null,
                'mkt_term' : null,
                'mkt_content' : null,
                'mkt_campaign' : null,
                'se_category' : null,
                'se_action' : null,
                'se_label' : null,
                'se_property' : null,
                'se_value' : null,
                'tr_orderid' : null,
                'tr_affiliation' : null,
                'tr_total' : null,
                'tr_tax' : null,
                'tr_shipping' : null,
                'tr_city' : null,
                'tr_state' : null,
                'tr_country' : null,
                'ti_orderid' : null,
                'ti_sku' : null,
                'ti_name' : null,
                'ti_category' : null,
                'ti_price' : null,
                'ti_quantity' : null,
                'pp_xoffset_min' : null,
                'pp_xoffset_max' : null,
                'pp_yoffset_min' : null,
                'pp_yoffset_max' : null,
                'useragent' : null,
                'br_name' : null,
                'br_family' : null,
                'br_version' : null,
                'br_type' : null,
                'br_renderengine' : null,
                'br_lang' : null,
                'br_features_pdf' : true,
                'br_features_flash' : false,
                'br_features_java' : null,
                'br_features_director' : null,
                'br_features_quicktime' : null,
                'br_features_realplayer' : null,
                'br_features_windowsmedia' : null,
                'br_features_gears' : null,
                'br_features_silverlight' : null,
                'br_cookies' : null,
                'br_colordepth' : null,
                'br_viewwidth' : null,
                'br_viewheight' : null,
                'os_name' : null,
                'os_family' : null,
                'os_manufacturer' : null,
                'os_timezone' : null,
                'dvce_type' : null,
                'dvce_ismobile' : null,
                'dvce_screenwidth' : null,
                'dvce_screenheight' : null,
                'doc_charset' : null,
                'doc_width' : null,
                'doc_height' : null,
                'tr_currency' : null,
                'tr_total_base' : null,
                'tr_tax_base' : null,
                'tr_shipping_base' : null,
                'ti_currency' : null,
                'ti_price_base' : null,
                'base_currency' : null,
                'geo_timezone' : null,
                'mkt_clickid' : null,
                'mkt_network' : null,
                'etl_tags' : null,
                'dvce_sent_tstamp' : null,
                'refr_domain_userid' : null,
                'refr_device_tstamp' : null,
                'domain_sessionid': '2b15e5c8-d3b1-11e4-b9d6-1681e6b88ec1',
                'derived_tstamp': '2013-11-26T00:03:57.886Z',
                'event_vendor': 'com.snowplowanalytics.snowplow',
                'event_name': 'link_click',
                'event_format': 'jsonschema',
                'event_version': '1-0-0',
                'event_fingerprint': 'e3dbfa9cca0412c3d4052863cefb547f',
                'true_tstamp': '2013-11-26T00:03:57.886Z'
                }");

			return JsonConvert.SerializeObject(expected);
		}

		/// <summary>
		/// Converts the dictionary to tsv.
		/// </summary>
		/// <returns>TSV</returns>
		/// <param name="dict">Dict.</param>
		public string ConvertDictToTsv(Dictionary<string, string> dict) => string.Join("\t", dict.Select(data => data.Value.Replace("\n", string.Empty)));

		[Fact]
		public void TestTransformForInputWithContextAndUnstructEvent()
		{
			var input = GetInputWithContextAndUnstructEvent();
			var expected = GetSerializedExpectedOutputForInputWithContextAndUnstructEvent();

			var tsv = ConvertDictToTsv(input);
			var transformedTsv = EventTransformer.Transform(tsv);

			Assert.Equal(expected, transformedTsv);

		}

		[Fact]
		public void TestTransformForInputWithoutContextAndUnstructEvent()
		{
			var input = GetInputWithoutContextAndUnstructEvent();
			var expected = GetSerializedExpectedOutputForInputWithoutContextAndUnstructEvent();

			//convert data into TSV
			var tsv = ConvertDictToTsv(input);
			var transformedTsv = EventTransformer.Transform(tsv);

			Assert.Equal(expected, transformedTsv);

		}

		[Fact]
		public void TestWrongTsvLength()
		{
			Exception exception = null;
			var tsv = "two\tfields";

			try
			{
				EventTransformer.Transform(tsv);
			}
			catch (SnowplowEventTransformationException sete)
			{
				exception = sete;
			}
			Assert.Equal("Expected 131 fields, received 2 fields.", exception.Message);
		}

		[Fact]
		public void TestMalformedField()
		{
			Exception exception = null;

			var malformedFieldsTsv = new string('\t', 110) + "bad_tax_base" + new string('\t', 20);

			try
			{
				EventTransformer.Transform(malformedFieldsTsv);
			}
			catch (SnowplowEventTransformationException sete)
			{
				exception = sete;
			}

			Assert.True(exception.Message.StartsWith("Unexpected exception parsing field with key tr_tax_base and value bad_tax_base",
													 StringComparison.CurrentCulture));

		}

		[Fact]
		protected void TestMultipleMalformedField()
		{
			SnowplowEventTransformationException exception = null;

			var malformedFieldsTsv = new string('\t', 102) + "bad_dvce_ismobile" + new string('\t', 8) + "bad_tax_base" + new string('\t', 20);

			try
			{
				EventTransformer.Transform(malformedFieldsTsv);
			}
			catch (SnowplowEventTransformationException sete)
			{
				exception = sete;
			}

			var expectedExceptions = new List<string>() {
				"Invalid value bad_dvce_ismobile for field dvce_ismobile",
				"Unexpected exception parsing field with key tr_tax_base and value bad_tax_base: Input string was not in a correct format."
				};

			var exceptionList = expectedExceptions.Except(exception.ErrorMessages);
			Assert.Equal(0, exceptionList.Count());

		}

	}
}