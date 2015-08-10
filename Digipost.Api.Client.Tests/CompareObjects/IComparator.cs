﻿using System.Collections.Generic;

namespace Digipost.Api.Client.Tests.CompareObjects
{
    interface IComparator
    {
        bool AreEqual(object expected, object actual);

        bool AreEqual(object expected, object actual, out IEnumerable<IDifference> differences);
    }
}
