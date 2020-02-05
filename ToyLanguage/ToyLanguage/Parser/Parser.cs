using System.Collections.Generic;

using ToyLanguage.Interfaces;

namespace ToyLanguage.Parser
{
    internal struct ParserResult
    {
        private readonly bool _ok;
        private readonly List<IStatement> _statements;
    }

    internal class Parser
    {
    }
}