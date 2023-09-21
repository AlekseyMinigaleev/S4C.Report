using C4S.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace S4C.Tests.Helpers
{
    public class JTokenExtensionsTests
    {
        [Theory]
        [InlineData("{\"object\":{\"array\":[{\"arrayValue1\":\"123\"},{\"arrayValue2\":\"123\"}],}}")]
        [InlineData("{\"object\":{\"array\":[\"123\",\"123\"],}}")]
        public void ExtractArrayValues(string jsonString)
        {
            var jObject = JObject.Parse(jsonString);
            var @object = jObject.GetValue<JToken>("object");
            var jArray = @object.GetValue<JArray>("array");

            string value1;
            string value2;
            try
            {
                value1 = jArray[0].GetValue<string>("arrayValue1");
                value2 = jArray[1].GetValue<string>("arrayValue2");
            }
            catch (InvalidOperationException)
            {
                value1 = jArray[0].Value<string>();
                value2 = jArray[1].Value<string>();
            }

            Assert.Equal("123", value1);
            Assert.Equal("123", value2);
        }

        [Theory]
        [InlineData("{\"object\":{\"field\":123}}")]
        public void ExtractValues(string jsonString)
        {
            var jObject = JObject.Parse(jsonString);
            var value = jObject
                .GetValue<JToken>("object")
                .GetValue<int>("field");

            Assert.Equal(123, value);
        }
    }
}
