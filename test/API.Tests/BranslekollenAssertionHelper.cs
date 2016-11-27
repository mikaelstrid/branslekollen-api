using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace API.Tests
{
    public class BranslekollenAssertionHelper : AssertionHelper
    {
        public void Expect(ObjectResult result, IResolveConstraint expr)
        {
            Assert.That(result.Value, expr.Resolve());
        }
    }
}