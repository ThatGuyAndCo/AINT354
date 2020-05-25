using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Jobs;
//using Unity.Collections;

public abstract class CustomEventJob// : IJob
{
    /*//Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.Byte> dataArrayByte;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.SByte> dataArraySByte;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.Int16> dataArrayInt16;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.UInt16> dataArrayUInt16;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.Int32> dataArrayInt32;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.UInt32> dataArrayUInt32;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.Int64> dataArrayInt64;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.UInt64> dataArrayUInt64;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.IntPtr> dataArrayIntPtr;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.Single> dataArraySingle;
    //Only access the data array of the type you are going to trigger the event with. This is the one that's data will be returned to the callback.
    public NativeArray<System.Double> dataArrayDouble;

    private string nativeArrayToUse;

    public void setDataArray(NativeArray<System.Byte> dataArray) { if (nativeArrayToUse == "") { this.dataArrayByte = dataArray; nativeArrayToUse = "byte"; } }
    public void setDataArray(NativeArray<System.SByte> dataArray) { if (nativeArrayToUse == "") { this.dataArraySByte = dataArray; nativeArrayToUse = "sbyte"; } }
    public void setDataArray(NativeArray<System.Int16> dataArray) { if (nativeArrayToUse == "") { this.dataArrayInt16 = dataArray; nativeArrayToUse = "int16"; } }
    public void setDataArray(NativeArray<System.UInt16> dataArray) { if (nativeArrayToUse == "") { this.dataArrayUInt16 = dataArray; nativeArrayToUse = "uint16"; } }
    public void setDataArray(NativeArray<System.Int32> dataArray) { if (nativeArrayToUse == "") { this.dataArrayInt32 = dataArray; nativeArrayToUse = "int32"; } }
    public void setDataArray(NativeArray<System.UInt32> dataArray) { if (nativeArrayToUse == "") { this.dataArrayUInt32 = dataArray; nativeArrayToUse = "uint32"; } }
    public void setDataArray(NativeArray<System.Int64> dataArray) { if (nativeArrayToUse == "") { this.dataArrayInt64 = dataArray; nativeArrayToUse = "int64"; } }
    public void setDataArray(NativeArray<System.UInt64> dataArray) { if (nativeArrayToUse == "") { this.dataArrayUInt64 = dataArray; nativeArrayToUse = "uint64"; } }
    public void setDataArray(NativeArray<System.IntPtr> dataArray) { if (nativeArrayToUse == "") { this.dataArrayIntPtr = dataArray; nativeArrayToUse = "intptr"; } }
    public void setDataArray(NativeArray<System.Single> dataArray) { if (nativeArrayToUse == "") { this.dataArraySingle = dataArray; nativeArrayToUse = "single"; } }
    public void setDataArray(NativeArray<System.Double> dataArray) { if (nativeArrayToUse == "") { this.dataArrayDouble = dataArray; nativeArrayToUse = "double"; } }

    //Overwrite this method
    public abstract void Execute();*/
}