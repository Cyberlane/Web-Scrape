using System.Linq;
using Xunit;

namespace Web_Scrape.Test
{
    public class ElementParserTest
    {
        private readonly ElementParser _parser;

        public ElementParserTest()
        {
            _parser = new ElementParser();
        }

        [Fact]
        public void WhenPassingSimpleHtmlWithAnAnchorTagReturnSingleAnchorTag()
        {
            var html = "<a href='#'>Link</a>";
            var result = _parser.FindElements<AnchorTag>("a", html);
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void WhenPassingSimpleHtmlWithAnAnchorTagEnsureTagHasAttributesSet()
        {
            var html = "<a href='#' title='moose'>Link</a>";
            var result = _parser.FindElements<AnchorTag>("a", html);
            var first = result.First();
            Assert.Equal("Link", first.Text);
            Assert.Equal("#", first.href);
            Assert.Equal("moose", first.title);
        }
    }

    public class AnchorTag : IElement
    {
        public string href { get; set; }
        public string title { get; set; }
        public string Text { get; set; }
    }
}
