#region License
// Copyright (c) 2012 Jerome Terry
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System.Text.RegularExpressions;

namespace JeromeTerry.PokerHandHistory.PartyPoker
{
    public class LineHandler
    {
        ProcessLineHandler _handler;

        public ProcessLineHandler Handler
        {
            get { return _handler; }
            set { _handler = value; }
        }

        Regex _regex;

        public Regex Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }

        public LineHandler(Regex regex, ProcessLineHandler handler)
        {
            _regex = regex;
            _handler = handler;
        }

        public bool Handle(object sender, ProcessLineEventArgs e)
        {
            Match match = this.Regex.Match(e.Line);
            e.Match = match;

            if (match.Success)
            {
                _handler(this, e);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
