using UnityEngine;
using System;
using System.Collections;

public class NetUtil
{
    // 辅助函数
    // 写入类型，考虑大小端的转换
    public static void WriteLong(byte[] data, int offset, long value, bool isBigEndian = true)
    {
        data[offset + 0] = (byte)((value >> 56) & 0xff);
        data[offset + 1] = (byte)((value >> 48) & 0xff);
        data[offset + 2] = (byte)((value >> 40) & 0xff);
        data[offset + 3] = (byte)((value >> 32) & 0xff);
        data[offset + 4] = (byte)((value >> 24) & 0xff);
        data[offset + 5] = (byte)((value >> 16) & 0xff);
        data[offset + 6] = (byte)((value >> 8) & 0xff);
        data[offset + 7] = (byte)((value) & 0xff);
    }

    public static void WriteInt(byte[] data, int offset, int value, bool isBigEndian = true)
    {
        data[offset + 0] = (byte)((value >> 24) & 0xff);
        data[offset + 1] = (byte)((value >> 16) & 0xff);
        data[offset + 2] = (byte)((value >> 8) & 0xff);
        data[offset + 3] = (byte)((value) & 0xff);
    }

    public static void WriteByte(byte[] data, int offset, byte[] buf, int length)
    {
        Array.Copy(buf, 0, data, offset, length);
    }

    private static byte[] s_convertBuffer = new byte[8];
    public static long ReadLong(byte[] data, int offset, bool isBigEndian = true)
    {
        const int TYPE_SIZE = sizeof(long);
        for (int i = 0; i < TYPE_SIZE; ++i) {
            s_convertBuffer[i] = data[TYPE_SIZE - i - 1];
        }

        return BitConverter.ToInt64(s_convertBuffer, 0);
    }

    public static int ReadInt(byte[] data, int offset, bool isBigEndian = true)
    {
        const int TYPE_SIZE = sizeof(int);
        for (int i = 0; i < TYPE_SIZE; ++i) {
            s_convertBuffer[i] = data[TYPE_SIZE - i - 1];
        }

        return BitConverter.ToInt32(s_convertBuffer, 0);
    }

    public static void ReadByte(byte[] data, int offset, byte[] outputBuffer, int size)
    {
        Array.Copy(data, offset, outputBuffer, 0, size);
    }

    public static long ReadLongFromRing(byte[] ringBuffer, int startIndex, int ringBufferSize, bool iBigEndian = true)
    {
        const int TYPE_SIZE = sizeof(long);
        CopyFromRingBuffer(ringBuffer, startIndex, ringBufferSize, s_convertBuffer, TYPE_SIZE);
        for (int i = 0; i < TYPE_SIZE / 2; ++i) {
            byte a = s_convertBuffer[TYPE_SIZE - i - 1];
            s_convertBuffer[TYPE_SIZE - i - 1] = s_convertBuffer[i];
            s_convertBuffer[i] = a;
        }

        return BitConverter.ToInt64(s_convertBuffer, 0);
    }

    public static int ReadIntFromRing(byte[] ringBuffer, int startIndex, int ringBufferSize, bool iBigEndian = true)
    {
        const int TYPE_SIZE = sizeof(int);
        CopyFromRingBuffer(ringBuffer, startIndex, ringBufferSize, s_convertBuffer, TYPE_SIZE);
        for (int i = 0; i < TYPE_SIZE / 2; ++i) {
            byte a = s_convertBuffer[TYPE_SIZE - i - 1];
            s_convertBuffer[TYPE_SIZE - i - 1] = s_convertBuffer[i];
            s_convertBuffer[i] = a;
        }

        return BitConverter.ToInt32(s_convertBuffer, 0);
    }

    public static void ReadByteFromRing(byte[] ringBuffer, int startIndex, int ringBufferSize, byte[] dest, int size)
    {
        CopyFromRingBuffer(ringBuffer, startIndex, ringBufferSize, dest, size);
    }

    private static void CopyFromRingBuffer(byte[] ringBuffer, int startIndex, int ringBufferSize, byte[] dest, int size)
    {
        if (startIndex + size > ringBufferSize) {
            // 如果一个消息有回卷（被拆成两份在环形缓冲区的头尾）
            int copyLen = ringBufferSize - startIndex;
            // 先拷贝末尾的数据
            Array.Copy(ringBuffer, startIndex, dest, 0, copyLen);
            // 再拷贝头部的剩余部分
            Array.Copy(ringBuffer, 0, dest, copyLen, size - copyLen);
        } else {
            // 没有被回卷，可以直接拷贝
            Array.Copy(ringBuffer, startIndex, dest, 0, size);
        }
    }
}
