using System.Text.Encodings.Web;
using System.Text.Json;

namespace Snowplow.Analytics.V4;

public static class EventTransformer4
{
    private static readonly X[] Fields =
    {
        new ("app_id", FieldTypes.StringField),
        new ("platform", FieldTypes.StringField),
        new ("etl_tstamp", FieldTypes.TstampField),
        new ("collector_tstamp", FieldTypes.TstampField),
        new ("dvce_created_tstamp", FieldTypes.TstampField),
        new ("event", FieldTypes.StringField),
        new ("event_id", FieldTypes.StringField),
        new ("txn_id", FieldTypes.IntField),
        new ("name_tracker", FieldTypes.StringField),
        new ("v_tracker", FieldTypes.StringField),
        new ("v_collector", FieldTypes.StringField),
        new ("v_etl", FieldTypes.StringField),
        new ("user_id", FieldTypes.StringField),
        new ("user_ipaddress", FieldTypes.StringField),
        new ("user_fingerprint", FieldTypes.StringField),
        new ("domain_userid", FieldTypes.StringField),
        new ("domain_sessionidx", FieldTypes.IntField),
        new ("network_userid", FieldTypes.StringField),
        new ("geo_country", FieldTypes.StringField),
        new ("geo_region", FieldTypes.StringField),
        new ("geo_city", FieldTypes.StringField),
        new ("geo_zipcode", FieldTypes.StringField),
        new ("geo_latitude", FieldTypes.DoubleField),
        new ("geo_longitude", FieldTypes.DoubleField),
        new ("geo_region_name", FieldTypes.StringField),
        new ("ip_isp", FieldTypes.StringField),
        new ("ip_organization", FieldTypes.StringField),
        new ("ip_domain", FieldTypes.StringField),
        new ("ip_netspeed", FieldTypes.StringField),
        new ("page_url", FieldTypes.StringField),
        new ("page_title", FieldTypes.StringField),
        new ("page_referrer", FieldTypes.StringField),
        new ("page_urlscheme", FieldTypes.StringField),
        new ("page_urlhost", FieldTypes.StringField),
        new ("page_urlport", FieldTypes.IntField),
        new ("page_urlpath", FieldTypes.StringField),
        new ("page_urlquery", FieldTypes.StringField),
        new ("page_urlfragment", FieldTypes.StringField),
        new ("refr_urlscheme", FieldTypes.StringField),
        new ("refr_urlhost", FieldTypes.StringField),
        new ("refr_urlport", FieldTypes.IntField),
        new ("refr_urlpath", FieldTypes.StringField),
        new ("refr_urlquery", FieldTypes.StringField),
        new ("refr_urlfragment", FieldTypes.StringField),
        new ("refr_medium", FieldTypes.StringField),
        new ("refr_source", FieldTypes.StringField),
        new ("refr_term", FieldTypes.StringField),
        new ("mkt_medium", FieldTypes.StringField),
        new ("mkt_source", FieldTypes.StringField),
        new ("mkt_term", FieldTypes.StringField),
        new ("mkt_content", FieldTypes.StringField),
        new ("mkt_campaign", FieldTypes.StringField),
        new ("contexts", FieldTypes.CustomContextsField),
        new ("se_category", FieldTypes.StringField),
        new ("se_action", FieldTypes.StringField),
        new ("se_label", FieldTypes.StringField),
        new ("se_property", FieldTypes.StringField),
        new ("se_value", FieldTypes.StringField),
        new ("unstruct_event", FieldTypes.UnstructField),
        new ("tr_orderid", FieldTypes.StringField),
        new ("tr_affiliation", FieldTypes.StringField),
        new ("tr_total", FieldTypes.DoubleField),
        new ("tr_tax", FieldTypes.DoubleField),
        new ("tr_shipping", FieldTypes.DoubleField),
        new ("tr_city", FieldTypes.StringField),
        new ("tr_state", FieldTypes.StringField),
        new ("tr_country", FieldTypes.StringField),
        new ("ti_orderid", FieldTypes.StringField),
        new ("ti_sku", FieldTypes.StringField),
        new ("ti_name", FieldTypes.StringField),
        new ("ti_category", FieldTypes.StringField),
        new ("ti_price", FieldTypes.DoubleField),
        new ("ti_quantity", FieldTypes.IntField),
        new ("pp_xoffset_min", FieldTypes.IntField),
        new ("pp_xoffset_max", FieldTypes.IntField),
        new ("pp_yoffset_min", FieldTypes.IntField),
        new ("pp_yoffset_max", FieldTypes.IntField),
        new ("useragent", FieldTypes.StringField),
        new ("br_name", FieldTypes.StringField),
        new ("br_family", FieldTypes.StringField),
        new ("br_version", FieldTypes.StringField),
        new ("br_type", FieldTypes.StringField),
        new ("br_renderengine", FieldTypes.StringField),
        new ("br_lang", FieldTypes.StringField),
        new ("br_features_pdf", FieldTypes.BoolField),
        new ("br_features_flash", FieldTypes.BoolField),
        new ("br_features_java", FieldTypes.BoolField),
        new ("br_features_director", FieldTypes.BoolField),
        new ("br_features_quicktime", FieldTypes.BoolField),
        new ("br_features_realplayer", FieldTypes.BoolField),
        new ("br_features_windowsmedia", FieldTypes.BoolField),
        new ("br_features_gears", FieldTypes.BoolField),
        new ("br_features_silverlight", FieldTypes.BoolField),
        new ("br_cookies", FieldTypes.BoolField),
        new ("br_colordepth", FieldTypes.StringField),
        new ("br_viewwidth", FieldTypes.IntField),
        new ("br_viewheight", FieldTypes.IntField),
        new ("os_name", FieldTypes.StringField),
        new ("os_family", FieldTypes.StringField),
        new ("os_manufacturer", FieldTypes.StringField),
        new ("os_timezone", FieldTypes.StringField),
        new ("dvce_type", FieldTypes.StringField),
        new ("dvce_ismobile", FieldTypes.BoolField),
        new ("dvce_screenwidth", FieldTypes.IntField),
        new ("dvce_screenheight", FieldTypes.IntField),
        new ("doc_charset", FieldTypes.StringField),
        new ("doc_width", FieldTypes.IntField),
        new ("doc_height", FieldTypes.IntField),
        new ("tr_currency", FieldTypes.StringField),
        new ("tr_total_base", FieldTypes.DoubleField),
        new ("tr_tax_base", FieldTypes.DoubleField),
        new ("tr_shipping_base", FieldTypes.DoubleField),
        new ("ti_currency", FieldTypes.StringField),
        new ("ti_price_base", FieldTypes.DoubleField),
        new ("base_currency", FieldTypes.StringField),
        new ("geo_timezone", FieldTypes.StringField),
        new ("mkt_clickid", FieldTypes.StringField),
        new ("mkt_network", FieldTypes.StringField),
        new ("etl_tags", FieldTypes.StringField),
        new ("dvce_sent_tstamp", FieldTypes.TstampField),
        new ("refr_domain_userid", FieldTypes.StringField),
        new ("refr_device_tstamp", FieldTypes.TstampField),
        new ("derived_contexts", FieldTypes.DerivedContextsField),
        new ("domain_sessionid", FieldTypes.StringField),
        new ("derived_tstamp", FieldTypes.TstampField),
        new ("event_vendor", FieldTypes.StringField),
        new ("event_name", FieldTypes.StringField),
        new ("event_format", FieldTypes.StringField),
        new ("event_version", FieldTypes.StringField),
        new ("event_fingerprint", FieldTypes.StringField),
        new ("true_tstamp", FieldTypes.TstampField)
    };

    private enum FieldTypes : ushort
    {
        BoolField,
        StringField,
        TstampField,
        DoubleField,
        IntField,
        DerivedContextsField,
        CustomContextsField,
        UnstructField
    }

    public static string Transform(ReadOnlySpan<char> line)
    {
        Stream stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            SkipValidation = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        try
        {
            writer.WriteStartObject();

            int cursor = -1;
            int prevCursor = -1;
            int fieldIndex = 0;
            int totalFields = Fields.Length;

#if DEBUG
            if (line[^1] != '\t')
            {
                ////throw new Exception();
            }

            int tabCount = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t')
                {
                    tabCount++;
                }
            }

            if (tabCount != Fields.Length - 1)
            {
                throw new SnowplowEventTransformationException4($"Expected {Fields.Length} fields, received {tabCount + 1} fields.");
            }
#endif

            List<string> parseErrors = new List<string>();
            string? lattitude = default;
            string? longitude = default;

            do
            {
                var field = Fields[fieldIndex];
                while (++cursor < line.Length)
                {
                    if (line[cursor] == '\t')
                    {
                        break;
                    }
                }

                if (cursor == prevCursor + 1)
                {
                    // double tab
                    if (field.Field != FieldTypes.CustomContextsField &&
                        field.Field != FieldTypes.DerivedContextsField &&
                        field.Field != FieldTypes.UnstructField)
                    {
                        writer.WriteNull(field.Id);
                    }
                }
                else
                {
                    var token = line.Slice(prevCursor + 1, cursor - prevCursor - 1);

                    switch (field.Field)
                    {
                        case FieldTypes.StringField:
                            writer.WriteString(field.Id, token);
                            break;
                        case FieldTypes.IntField:
                            if (!int.TryParse(token, out int i))
                            {
                                parseErrors.Add($"Unexpected exception parsing field with key {field.Id} and value {token}");
                            }
                            writer.WriteNumber(field.Id, i);
                            break;
                        case FieldTypes.DoubleField:

                            if (!double.TryParse(token, out double d))
                            {
                                parseErrors.Add($"Unexpected exception parsing field with key {field.Id} and value {token}: Input string was not in a correct format.");
                            }

                            if (fieldIndex == 22)
                            {
                                lattitude = token.ToString();
                            }
                            else if (fieldIndex == 23)
                            {
                                longitude = token.ToString();
                            }

                            writer.WriteNumber(field.Id, d);
                            break;
                        case FieldTypes.TstampField:
                            writer.WriteString(field.Id, new string(token)
                                .TrimEnd('0') // Only to make it output-compatible with V1
                                .Replace(' ', 'T') + 'Z');
                            break;
                        case FieldTypes.BoolField:
                            if (token.Length == 1)
                            {
                                if (token[0] == '0')
                                {
                                    writer.WriteBoolean(field.Id, false);
                                }
                                else if (token[0] == '1')
                                {
                                    writer.WriteBoolean(field.Id, true);
                                }
                                else
                                {
                                    parseErrors.Add($"Invalid value {token} for field {field.Id}");
                                    writer.WriteBoolean(field.Id, false);
                                }
                            }
                            else if (!bool.TryParse(token, out bool b))
                            {
                                parseErrors.Add($"Invalid value {token} for field {field.Id}");
                                writer.WriteBoolean(field.Id, false);
                            }
                            else
                            {
                                writer.WriteBoolean(field.Id, b);
                            }

                            break;
                        case FieldTypes.CustomContextsField:
                            {
                                if (token.Length > 0)
                                {
                                    var json = JsonShredder4.ParseContexts(token);
                                    writer.WriteRawValue(json, true);
                                }

                                break;
                            }

                        case FieldTypes.UnstructField:
                            {
                                var json = JsonShredder4.ParseUnstruct(token);
                                writer.WriteRawValue(json, true);
                                break;
                            }

                        case FieldTypes.DerivedContextsField:
                            {
                                if (token.Length > 0)
                                {
                                    var json = JsonShredder4.ParseContexts(token);
                                    writer.WriteRawValue(json, true);
                                }

                                break;
                            }

                        default:
                            // Should logically never be hit without code changes.
                            throw new InvalidOperationException();
                    }
                }

                prevCursor = cursor;
                fieldIndex++;
            }
            while (fieldIndex < totalFields);

#if DEBUG
            if (fieldIndex != totalFields)
            {
                throw new Exception();
            }
#endif
            if (lattitude != null && longitude != null)
            {
                writer.WriteString("geo_location", string.Join(',', lattitude, longitude));
            }

            writer.WriteEndObject();

            if (parseErrors.Count > 0)
            {
                throw new SnowplowEventTransformationException4(parseErrors);
            }
        }
        finally
        {
            writer.Flush();
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
        }

        StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private struct X
    {
        public X(string id, FieldTypes field)
        {
            Id = id;
            Field = field;
        }

        public string Id { get; }
        public FieldTypes Field { get; }
    }
}
