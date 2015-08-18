//
// Copyright (c) Artur Durasiewicz. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System;

namespace PetaPoco.Repository
{
    public class FilterBuilderException : Exception
    {
        public FilterBuilderException()
        { }

        public FilterBuilderException(string message) : base(message)
        { }
    }
}
