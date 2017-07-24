# Snowplow .NET Analytics SDK

[![Build Status][appveyor-image]][appveyor]
[![Release][release-image]][releases]
[![License][license-image]][license]

## Overview

The **[Snowplow][snowplow]** Analytics SDK for .NET lets you work with **[Snowplow enriched events][enriched-events]** in your .NET event processing and data modeling jobs.

Use this SDK with **[Azure Data Lake Analytics][adla]**, **[Azure Functions][af]**, **[AWS Lambda][lambda]**, **[Microsoft Orleans][orleans]** and other .NET-compatible data processing frameworks.

## Find out more

| Technical Docs                  | Setup Guide               | Roadmap                 | Contributing                      |
|---------------------------------|---------------------------|-------------------------|-----------------------------------|
| ![i1][techdocs-image]           | ![i2][setup-image]       | ![i3][roadmap-image]   | ![i4][contributing-image]        |
| **[Technical Docs][techdocs]** | **[Setup Guide][setup]** | **[Roadmap][roadmap]** | **[Contributing][contributing]** |## Copyright and license

## Copyright and license

The Snowplow .NET Analytics SDK is copyright 2017 Snowplow Analytics Ltd.

Licensed under the **[Apache License, Version 2.0][license]** (the "License");
you may not use this software except in compliance with the License.

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

[appveyor-image]: https://ci.appveyor.com/api/projects/status/47dvoyc94i2i9gkq/branch/master?svg=true
[appveyor]: https://ci.appveyor.com/project/snowplow/snowplow-dotnet-analytics-sdk

[license-image]: http://img.shields.io/badge/license-Apache--2-blue.svg?style=flat
[license]: http://www.apache.org/licenses/LICENSE-2.0

[release-image]: https://img.shields.io/badge/release-0.2.0-blue.svg?style=flat
[releases]: https://github.com/snowplow/snowplow-dotnet-analytics-sdk/releases

[techdocs-image]: https://d3i6fms1cm1j0i.cloudfront.net/github/images/techdocs.png
[setup-image]: https://d3i6fms1cm1j0i.cloudfront.net/github/images/setup.png
[roadmap-image]: https://d3i6fms1cm1j0i.cloudfront.net/github/images/roadmap.png
[contributing-image]: https://d3i6fms1cm1j0i.cloudfront.net/github/images/contributing.png

[setup]: https://github.com/snowplow/snowplow/wiki/.NET-Analytics-SDK-setup
[techdocs]: https://github.com/snowplow/snowplow/wiki/.NET-Analytics-SDK
[roadmap]: https://github.com/snowplow/snowplow/wiki/.NET-Tracker-Roadmap
[contributing]: https://github.com/snowplow/snowplow/wiki/.NET-Tracker-Contributing

[snowplow]: http://snowplowanalytics.com
[enriched-event-source]: https://github.com/snowplow/snowplow/blob/master/3-enrich/scala-common-enrich/src/main/scala/com.snowplowanalytics.snowplow.enrich/common/outputs/EnrichedEvent.scala
[enriched-event-example-scala]: https://github.com/snowplow/snowplow-scala-analytics-sdk/blob/master/src/test/scala/com.snowplowanalytics.snowplow.analytics.scalasdk.json/EventTransformerSpec.scala#L121
[enriched-event-example-scala-2]: https://github.com/snowplow/snowplow/blob/master/3-enrich/scala-hadoop-shred/src/test/scala/com.snowplowanalytics.snowplow.enrich.hadoop/jobs/good/CrossBatchDeduplicationSpec.scala#L57-L67
[enriched-events]: https://github.com/snowplow/snowplow/wiki/canonical-event-model
[event-data-modeling]: http://snowplowanalytics.com/blog/2016/03/16/introduction-to-event-data-modeling/

[adla]: https://azure.microsoft.com/en-gb/services/data-lake-analytics/
[af]: https://azure.microsoft.com/en-gb/services/functions/
[lambda]: https://aws.amazon.com/lambda/
[orleans]: https://dotnet.github.io/orleans/
