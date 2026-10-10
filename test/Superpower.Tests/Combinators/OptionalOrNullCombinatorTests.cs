using System;
using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class OptionalOrNullCombinatorTests
    {
        [Fact]
        public void TextParserReturnsMatchOrNull()
        {
            TextParser<string?> parser = Character.EqualTo('a').Value("a").OptionalOrNull();
            AssertParser.SucceedsWith(parser, "a", "a");
            AssertParser.SucceedsWith(parser, "", null);
            TextParser<string?> optionalName = Identifier.CStyle.Text().OptionalOrNull();
            AssertParser.SucceedsWith(optionalName, "name", "name");
            AssertParser.SucceedsWith(optionalName, "", null);
        }

        [Fact]
        public void TokenParserReturnsMatchOrNull()
        {
            TokenListParser<char, string?> parser = Token.EqualTo('a').Value("a").OptionalOrNull();
            AssertParser.SucceedsWith(parser, "a", "a");
            AssertParser.SucceedsWith(parser, "", null);
        }

        [Fact]
        public void TextParserDoesNotConsumeAnUnmatchedInput()
        {
            var parser = Character.EqualTo('b').Value("b").OptionalOrNull()
                .Then(value => Character.EqualTo('a').Value(value));
            AssertParser.SucceedsWith(parser, "a", null);
        }

        [Fact]
        public void TokenParserDoesNotConsumeAnUnmatchedInput()
        {
            var parser = Token.EqualTo('b').Value("b").OptionalOrNull()
                .Then(value => Token.EqualTo('a').Value(value));
            AssertParser.SucceedsWith(parser, "a", null);
        }

        [Fact]
        public void TextParserPreservesPartialFailures()
        {
            var parser = Character.EqualTo('a').IgnoreThen(Character.EqualTo('b')).Value("ab");
            AssertParser.Fails(parser.OptionalOrNull(), "ac");
            AssertParser.SucceedsWith(parser.Try().OptionalOrNull().IgnoreThen(Span.EqualTo("ac").Value("ac")), "ac", "ac");
        }

        [Fact]
        public void TokenParserPreservesPartialFailures()
        {
            var parser = Token.EqualTo('a').IgnoreThen(Token.EqualTo('b')).Value("ab");
            AssertParser.Fails(parser.OptionalOrNull(), "ac");
            AssertParser.SucceedsWith(parser.Try().OptionalOrNull().IgnoreThen(Token.EqualTo('a').IgnoreThen(Token.EqualTo('c')).Value("ac")), "ac", "ac");
        }

        [Fact]
        public void TextParserRejectsNullArguments()
        {
            TextParser<string> parser = null!;
            Assert.Throws<ArgumentNullException>(() => parser.OptionalOrNull());
        }

        [Fact]
        public void TokenParserRejectsNullArguments()
        {
            TokenListParser<char, string> parser = null!;
            Assert.Throws<ArgumentNullException>(() => parser.OptionalOrNull());
        }

        [Fact]
        public void ExistingValueTypeOptionalsStillReturnNullableValues()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').Optional(), "a", (char?)'a');
            AssertParser.SucceedsWith(Character.EqualTo('a').Optional(), "", (char?)null);
            AssertParser.SucceedsWith(Token.EqualTo('a').Value('a').Optional(), "a", (char?)'a');
            AssertParser.SucceedsWith(Token.EqualTo('a').Value('a').Optional(), "", (char?)null);
        }
    }
}
