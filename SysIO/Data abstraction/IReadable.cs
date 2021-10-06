using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.SysIO
{
    public interface IReadable
    {
        /// <summary>
        /// Creates an instance from <paramref name="content"/>.
        /// </summary>
        /// <param name="content">This object representaion in <see cref="string"/>, if <paramref name="content"/> is more than 1 line it might produce unexpected behavior.</param>
        /// <returns></returns>
        public IStoreable FromStringFile(SingleLineString content);
    }
}
