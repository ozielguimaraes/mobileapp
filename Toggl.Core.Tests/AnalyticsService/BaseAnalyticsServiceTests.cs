using System.Linq;
using System.Reflection;
using FluentAssertions;
using Toggl.Core.Analytics;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.AnalyticsService
{
    public sealed class BaseAnalyticsServiceTests
    {
        public sealed class TheAnalyticsEventAttribute
        {
            [Fact]
            public void HasTheSameNumberOfArgumentsAsPropertyGenericTypes()
            {
                var properties = typeof(BaseAnalyticsService)
                    .GetProperties()
                    .Where(property => property.HasCustomAttribute<AnalyticsEventAttribute>())
                    .Select(property => new
                    {
                        Name = property.Name,
                        AttributeArgumentCount = property.GetCustomAttribute<AnalyticsEventAttribute>().ParameterNames.Length,
                        GenericTypeCount = property.PropertyType.GenericTypeArguments.Length
                    })
                    .Where(property => property.AttributeArgumentCount != property.GenericTypeCount)
                    .Should()
                    .BeEmpty();
            }
        }
    }
}
