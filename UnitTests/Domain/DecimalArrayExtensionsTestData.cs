// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License").
//   You may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Provides test data for the DecimalArrayExtensions unit tests.
/// </summary>
/// <remarks>
/// This class generates two sets of test data:
/// - VarianceData: Provides decimal arrays along with their expected variance values for both population and sample calculation types.
/// - StandardDeviationData: Provides decimal arrays along with their expected standard deviation values for both population and sample calculation types.
/// </remarks>
public sealed class DecimalArrayExtensionsTestData
{
    /// <summary>
    /// Gets a collection of test cases containing varied decimal arrays and their corresponding variance values.
    /// </summary>
    public static IEnumerable<object[]> VarianceData =>
        [
        // CalculationType.Population
        [new[] { 1m, 2m, 3m, 4m, 5m }, CalculationType.Population, 2m],
            [new[] { 10m, 20m, 30m, 40m, 50m }, CalculationType.Population, 200m],
            [new[] { -5m, 0m, 5m, 10m, 15m }, CalculationType.Population, 50m],
            [new[] { 0.01m, 0.02m, 0.03m, 0.04m, 0.05m }, CalculationType.Population, 0.0002m],
            [new[] { -1000m, 0m, 1000m }, CalculationType.Population, 666666.6666666666666666667m],
            [new[] { 100m, 200m, 300m, 400m, 500m, 600m, 700m }, CalculationType.Population, 40000m],

            // CalculationType.Sample
            [new[] { 1m, 2m, 3m, 4m, 5m }, CalculationType.Sample, 2.5m],
            [new[] { 10m, 20m, 30m, 40m, 50m }, CalculationType.Sample, 250m],
            [new[] { -5m, 0m, 5m, 10m, 15m }, CalculationType.Sample, 62.5m],
            [new[] { 0.01m, 0.02m, 0.03m, 0.04m, 0.05m }, CalculationType.Sample, 0.00025m],
            [new[] { -1000m, 0m, 1000m }, CalculationType.Sample, 1000000m],
            [new[] { 100m, 200m, 300m, 400m, 500m, 600m, 700m }, CalculationType.Sample, 46666.666666666666666666666667m],
        ];

    /// <summary>
    /// Gets a collection of test cases containing varied decimal arrays and their corresponding standard deviation values.
    /// </summary>
    public static IEnumerable<object[]> StandardDeviationData =>
        [
        // CalculationType.Population
        [new[] { 1m, 2m, 3m, 4m, 5m }, CalculationType.Population, 1.414213562373095m],
            [new[] { 10m, 20m, 30m, 40m, 50m }, CalculationType.Population, 14.14213562373095m],
            [new[] { -5m, 0m, 5m, 10m, 15m }, CalculationType.Population, 7.0710678118654755m],
            [new[] { 0.01m, 0.02m, 0.03m, 0.04m, 0.05m }, CalculationType.Population, 0.0141421356237m],
            [new[] { -1000m, 0m, 1000m }, CalculationType.Population, 816.4965809277261m],
            [new[] { 100m, 200m, 300m, 400m, 500m, 600m, 700m }, CalculationType.Population, 200m],

            // CalculationType.Sample
            [new[] { 1m, 2m, 3m, 4m, 5m }, CalculationType.Sample, 1.5811388300841898m],
            [new[] { 10m, 20m, 30m, 40m, 50m }, CalculationType.Sample, 15.811388300841896m],
            [new[] { -5m, 0m, 5m, 10m, 15m }, CalculationType.Sample, 7.905694150420948m],
            [new[] { 0.01m, 0.02m, 0.03m, 0.04m, 0.05m }, CalculationType.Sample, 0.015811388300m],
            [new[] { -1000m, 0m, 1000m }, CalculationType.Sample, 1000m],
            [new[] { 100m, 200m, 300m, 400m, 500m, 600m, 700m }, CalculationType.Sample, 216.024689946929m],
        ];
}
