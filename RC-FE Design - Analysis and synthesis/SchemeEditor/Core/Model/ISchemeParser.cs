using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace RC_FE_Design___Analysis_and_synthesis.SchemeEditor.Core.Model
{
    public interface ISchemeParser
    {
        TreeSolution Parse(string model, ISchemeCreator creator, ParseOptions options);
    } 

}
