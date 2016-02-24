using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace JavaScriptViewEngine.Babel.Tests
{
    public class BabelEngineTests : IDisposable
    {
        IBabelEngine _babelEngine;

        public BabelEngineTests()
        {
            VroomJs.AssemblyLoader.EnsureLoaded();
            _babelEngine = new BabelEngine(new JsEngineBuilder());
        }

        [Fact]
        public void CanTransformES2015()
        {
            var result = _babelEngine.Transform(@"let q = 99;let myVariable = `${q} bottles of beer on the wall, ${q} bottles of beer.`;
            ", new BabelConfig());
            result.Should().Be(@"var q = 99;var myVariable = q + "" bottles of beer on the wall, "" + q + "" bottles of beer."";");
        }

        public void Dispose()
        {
            _babelEngine.Dispose();
        }
    }
}
