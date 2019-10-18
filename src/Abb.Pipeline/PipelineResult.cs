using System;
using System.Collections.Generic;
using System.Text;

namespace Abb.Pipeline
{
    public class PipelineResult
    {
        public bool Succeeded { get; }

        public object Result { get; }
    }
}
