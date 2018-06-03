using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneClient.Helpers
{
    public static class ByteExtentions
    {
        public static Boolean[] BytesIntoBooleanArray(this byte[] bytes)
        {
            Boolean[] resultArr = new Boolean[bytes.Length * 8];
            int byteIndex = -1;

            for (int i = 0; i < resultArr.Length; ++i)
            {
                if (i % 8 == 0)
                    ++byteIndex;
                resultArr[i] = (bytes[byteIndex] & (1 << i % 8)) == 1 << i % 8;
            }
        
            return resultArr;
        }
    }
}
