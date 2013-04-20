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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JeromeTerry.PokerHandHistory.Data.SQLite
{
    public sealed class DbLock : IDisposable
    {
        DataStore _store;

        public DbLock(DataStore store)
        {
            this._store = store;

            this.ObtainExclusiveAccess();
        }

        internal bool ObtainExclusiveAccess()
        {
            System.Threading.Monitor.Enter(_store._lockObject);
            _store._lockCount++;
            return true;
        }

        internal bool ReleaseExclusiveAccess()
        {
            if (_store._lockCount > 0)
            {
                System.Threading.Monitor.Exit(_store._lockObject);
                _store._lockCount--;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            this.ReleaseExclusiveAccess();
        }
    }
}
