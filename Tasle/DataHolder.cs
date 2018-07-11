using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasle
{
    public class DataHolder
    {
        public byte[] mRecvDataCache;
        public byte[] mRecvData;

        private int mTail = -1;
        private int mPackLen;
        private const int mPackBytes = 1;

        public int Count
        {
            get
            {
                return mTail + 1;
            }
        }

        public int Capacity
        {
            get
            {
                return mRecvDataCache != null ? mRecvDataCache.Length : 0;
            }
        }

        public void Push(byte[] data, int lenght)
        {
            if (mRecvDataCache == null)
                mRecvDataCache = new byte[lenght];

            if (Count + lenght > Capacity)
            {
                byte[] newArr = new byte[this.Count + lenght];
                mRecvDataCache.CopyTo(newArr, 0);
                mRecvDataCache = newArr;
            }
            Array.Copy(data, 0, mRecvDataCache, mTail + 1, lenght);
            mTail += lenght;
        }

        public bool IsFinished()
        {
            if (Count == 0)
                return false;

            if (Count > 0)
            {
                DataStream reader = new DataStream(mRecvDataCache, true);

                mPackLen = (int)reader.ReadByte();
                //more than mPackBytes lenght and process
                if (mPackLen > mPackBytes)
                {
                    if (Count - mPackBytes >= mPackLen)
                    {
                        mRecvData = new byte[mPackLen];
                        Array.Copy(mRecvDataCache,1,mRecvData,0,mPackLen);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
