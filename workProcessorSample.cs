using System;
using UnityEngine;
using WorkProcessorSpace;


//need add to empty object in Hieraechy by "Add Component"
public class workProcessorSample : MonoBehaviour
{
    private enum testEvent
    {
        testNoData,
        testCallback,
        testData,
        testNeedDataCallback,
        testDataAndCallback,
        testDataAndNeedDataCallback,
        testQueueNoData,
        testQueueCallback,
        testQueueData,
        testQueueNeedDataCallback,
        testQueueDataAndCallback,
        testQueueDataAndNeedDataCallback,
        testQueueFinish,
        testErrorDoneID,
       　ddd,
    }

    private WorkProcessor workProcessor = null;

    private bool isWork = true;

    private void Start()
    {
        if (!isWork)
            return;

        workProcessor = WorkProcessor.GetInstance();
        testFunction();
　　}

   #region TEST WOERKPROCESSOR CODE

    delegate void Test();
    delegate void Test<T>(T inData);

    private void testNoData()
    {
        if (!isWork)
            return;

        Debug.Log("testNoData IS OK");
    }

    private void testErrorData()
    {
        if (!isWork)
            return;

        Debug.Log("testErrorData IS OK");
    }

    private void testCallback(Delegate inFunc)
    {
        if (!isWork)
            return;

        Debug.Log("testCallback IS OK");

        inFunc.DynamicInvoke();
    }

    private void testNeedDataCallback(Delegate inFunc)
    {
        if (!isWork)
            return;

        string ok = "testNeedDataCallback is OK";
        inFunc.DynamicInvoke(ok);
    }

    private void testData(string inString)
    {
        if (!isWork)
            return;

        Debug.Log(inString);
    }

    private void testDataAndCallback(string inText, Delegate inFunc)
    {
        if (!isWork)
            return;

        Debug.Log(inText);
        inFunc.DynamicInvoke();
    }

    private void testDataAndNeedDataCallback(string inText, Delegate inFunc)
    {
        if (!isWork)
            return;

        Debug.Log(inText);
        string ok = "testDataAndNeedDataCallback Callback IS OK";
        inFunc.DynamicInvoke(ok);
    }

    private void testQueueErrorDoneID(DoneIDData inData)
    {
        if (!isWork)
            return;

        Debug.Log("testQueueNoData IS OK");
        workProcessor.GoQueue(testEvent.testQueueFinish);
        DoneIDData errorDoneIDData = new DoneIDData();
        errorDoneIDData.DoneID = 1;
        workProcessor.ReportFinish(errorDoneIDData);
    }

    private void testQueueNoData(DoneIDData inData)
    {
        if (!isWork)
            return;

        Debug.Log("testQueueNoData IS OK");
        workProcessor.GoQueue(testEvent.testQueueFinish);
        workProcessor.ReportFinish(inData);

    }

    private void testQueueErrorNoData(DoneIDData inData)
    {
        if (!isWork)
            return;

        Debug.Log("testQueueErrorNoData IS OK");
        workProcessor.GoQueue(testEvent.testQueueFinish);
        workProcessor.ReportFinish(inData);

    }

    private void testQueueCallback(DoneIDData inData, Delegate inFunc)
    {
        if (!isWork)
            return;

        inFunc.DynamicInvoke();
        workProcessor.GoQueue(testEvent.testQueueFinish);
        workProcessor.ReportFinish(inData);
    }

    private void testQueueNeedDataCallback(DoneIDData inData, Delegate inFunc)
    {
        if (!isWork)
            return;

        string ok = "testQueueNeedDataCallback Callback IS OK";
        inFunc.DynamicInvoke(ok);
        workProcessor.GoQueue(testEvent.testQueueFinish);
        workProcessor.ReportFinish(inData);
    }

    private void testQueueData(DoneIDData inData, string inText)
    {
        if (!isWork)
            return;

        Debug.Log(inText);
        workProcessor.GoQueue(testEvent.testQueueFinish);
        workProcessor.ReportFinish(inData);
    }

    private void testQueueDataAndCallback(DoneIDData inData, string inText, Delegate inFunc)
    {
        if (!isWork)
            return;

        Debug.Log(inText);
        inFunc.DynamicInvoke();

        workProcessor.GoQueue(testEvent.testQueueFinish);
        workProcessor.ReportFinish(inData);
    }

    private void testQueueDataAndNeedDataCallback(DoneIDData inData, string inText, Delegate inFunc)
    {
        if (!isWork)
            return;

        Debug.Log(inText);
        string ok = "testQueueDataAndNeedDataCallback Callback IS OK";
        inFunc.DynamicInvoke(ok);

        workProcessor.GoQueue(testEvent.testQueueFinish);
        workProcessor.ReportFinish(inData);
    }

    private void testQueueFinish(DoneIDData inData)
    {
        if (!isWork)
            return;

        Debug.Log("TEST QUEUE FINSH");
        workProcessor.ReportFinish(inData);
    }

    private void testFunction()
    {
        //add and go na data function test    OK
        workProcessor.AddWork(testEvent.testNoData, testNoData);
        workProcessor.GoWork(testEvent.testNoData);

        //add and go callback function test   OK
        workProcessor.AddWork(testEvent.testCallback, testCallback);
        Test bb = delegate ()
        {
            Debug.Log("testCallback Delagate OK");
        };
        workProcessor.GoWorkWithCallback(testEvent.testCallback, bb);

        //add and go need data callback  OK
        workProcessor.AddWork(testEvent.testNeedDataCallback, testNeedDataCallback);
        Test<string> bb2 = delegate (string intext)
        {
            Debug.Log(intext);
        };
        workProcessor.GoWorkWithCallback(testEvent.testNeedDataCallback, bb2);

        //add and go need data   OK
        workProcessor.AddWork<testEvent, string>(testEvent.testData, testData);
        workProcessor.GoWorkWithData(testEvent.testData, "testData IS OK");

        //add and go data and callback  OK
        workProcessor.AddWork<testEvent, string>(testEvent.testDataAndCallback, testDataAndCallback);
        Test bb3 = delegate ()
        {
            Debug.Log("testDataAndCallback Callback IS OK");
        };
        workProcessor.GoWorkWithDataCallback(testEvent.testDataAndCallback, "testDataAndCallback IS OK", bb3);

        //add and go data and need data callback  OK
        workProcessor.AddWork<testEvent, string>(testEvent.testDataAndNeedDataCallback, testDataAndNeedDataCallback);
        Test<string> bb4 = delegate (string inText)
        {
            Debug.Log(inText);
        };
        workProcessor.GoWorkWithDataCallback(testEvent.testDataAndNeedDataCallback, "testDataAndNeedDataCallback IS OK", bb4);

        //add check finish work
        workProcessor.AddQueueWork(testEvent.testQueueFinish, testQueueFinish);

        //add and go queue no data  OK
        workProcessor.AddQueueWork(testEvent.testQueueNoData, testQueueNoData);
        workProcessor.GoQueue(testEvent.testQueueNoData);

        //add and go queue have callback  OK
        workProcessor.AddQueueWork(testEvent.testQueueCallback, testQueueCallback);
        Test bb45 = delegate ()
        {
            Debug.Log("testQueueCallback Callback IS OK");
        };
        workProcessor.GoQueueWithCallback(testEvent.testQueueCallback, bb45);

        //add and go queue have need data callback  OK
        workProcessor.AddQueueWork(testEvent.testQueueNeedDataCallback, testQueueNeedDataCallback);
        Test<string> bb5 = delegate (string inText)
        {
            Debug.Log(inText);
        };
        workProcessor.GoQueueWithCallback(testEvent.testQueueNeedDataCallback, bb5);


        //add and go queue have data  OK
        workProcessor.AddQueueWork<testEvent, string>(testEvent.testQueueData, testQueueData);
        workProcessor.GoQueueWithData(testEvent.testQueueData, "testQueueData IS OK");

        //add and go queue have data and callback  OK
        workProcessor.AddQueueWork<testEvent, string>(testEvent.testQueueDataAndCallback, testQueueDataAndCallback);
        Test bb6 = delegate ()
        {
            Debug.Log("testQueueDataAndCallback Callback IS OK");
        };
        workProcessor.GoQueueWithDataCallback(testEvent.testQueueDataAndCallback, "testQueueDataAndCallback IS OK", bb6);

        //add and go queue have data and need data callback  OK
        workProcessor.AddQueueWork<testEvent, string>(testEvent.testQueueDataAndNeedDataCallback, testQueueDataAndNeedDataCallback);
        Test<string> bb7 = delegate (string inText)
        {
            Debug.Log(inText);
        };
        workProcessor.GoQueueWithDataCallback(testEvent.testQueueDataAndNeedDataCallback, "testQueueDataAndNeedDataCallback IS OK", bb7);


        workProcessor.RemoveWork(testEvent.testNoData, testNoData);
        workProcessor.RemoveWork(testEvent.testCallback, testCallback);
        workProcessor.RemoveWork(testEvent.testNeedDataCallback, testNeedDataCallback);
        workProcessor.RemoveWork<testEvent, string>(testEvent.testData, testData);
        workProcessor.RemoveWork<testEvent, string>(testEvent.testDataAndCallback, testDataAndCallback);
        workProcessor.RemoveWork<testEvent, string>(testEvent.testDataAndNeedDataCallback, testDataAndNeedDataCallback);
        workProcessor.RemoveQueueWork(testEvent.testQueueNoData, testQueueNoData);
        workProcessor.RemoveQueueWork(testEvent.testQueueCallback, testQueueCallback);
        workProcessor.RemoveQueueWork(testEvent.testQueueNeedDataCallback, testQueueNeedDataCallback);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueData, testQueueData);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueDataAndCallback, testQueueDataAndCallback);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueDataAndNeedDataCallback, testQueueDataAndNeedDataCallback);



        Debug.Log("!!!!!!!!!!!!!!!ERROR TEST START");

        //RemoveWork test remove no command function OK
        workProcessor.RemoveWork(testEvent.testNoData, testNoData);
        workProcessor.RemoveWork(testEvent.testCallback, testCallback);
        workProcessor.RemoveWork(testEvent.testNeedDataCallback, testNeedDataCallback);
        workProcessor.RemoveWork<testEvent, string>(testEvent.testData, testData);
        workProcessor.RemoveWork<testEvent, string>(testEvent.testDataAndCallback, testDataAndCallback);
        workProcessor.RemoveWork<testEvent, string>(testEvent.testDataAndNeedDataCallback, testDataAndNeedDataCallback);
        workProcessor.RemoveQueueWork(testEvent.testQueueNoData, testQueueNoData);
        workProcessor.RemoveQueueWork(testEvent.testQueueCallback, testQueueCallback);
        workProcessor.RemoveQueueWork(testEvent.testQueueNeedDataCallback, testQueueNeedDataCallback);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueData, testQueueData);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueDataAndCallback, testQueueDataAndCallback);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueDataAndNeedDataCallback, testQueueDataAndNeedDataCallback);

        //RemoveWork different function  OK
        workProcessor.RemoveWork(testEvent.testNoData, testCallback);
        workProcessor.RemoveWork(testEvent.testCallback, testNoData);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueData, testQueueDataAndNeedDataCallback);
        workProcessor.RemoveQueueWork<testEvent, string>(testEvent.testQueueDataAndCallback, testQueueData);


        //addWork  OK
        workProcessor.AddWork(0, testNoData);
        workProcessor.AddWork(testEvent.testNoData, testNoData);
        FunctionType gf = null;
        workProcessor.AddWork(testEvent.testNoData, gf);

        //addQueueWork  OK
        workProcessor.AddQueueWork(0, testQueueNoData);
        workProcessor.AddQueueWork(testEvent.testQueueNoData, testQueueNoData);
        FunctionType gfff = null;
        workProcessor.AddWork(testEvent.testQueueNoData, gfff);

        //RemoveWork  OK
        workProcessor.RemoveWork(0, testNoData);
        FunctionType gff = null;
        workProcessor.RemoveWork(testEvent.testNoData, gff);
        workProcessor.RemoveWork(testEvent.testData, testNoData);
        workProcessor.RemoveWork(testEvent.testNoData, testErrorData);

        //RemoveQueueWork OK
        workProcessor.RemoveQueueWork(0, testQueueNoData);
        FunctionType<DoneIDData> gffff = null;
        workProcessor.RemoveQueueWork(testEvent.testQueueNoData, gffff);
        workProcessor.RemoveQueueWork(testEvent.testQueueData, testQueueNoData);
        workProcessor.RemoveQueueWork(testEvent.testQueueNoData, testQueueErrorNoData);

        //GoWork
        workProcessor.GoWork(testEvent.ddd);
        Test dfg = delegate ()
        {
            Debug.Log("ERROR");
        };
        workProcessor.GoWorkWithCallback(testEvent.ddd, dfg);
        DoneIDData doneIDData = new DoneIDData();
        workProcessor.GoWorkWithData(testEvent.testData, doneIDData);
        workProcessor.GoWorkWithDataCallback(testEvent.testData, doneIDData, dfg);

        //GoWorkByQueue
        workProcessor.GoQueue(testEvent.ddd);
        Test dfg2 = delegate ()
        {
            Debug.Log("ERROR");
        };
        workProcessor.GoWorkWithCallback(testEvent.ddd, dfg2);
        DoneIDData doneIDData1 = new DoneIDData();
        workProcessor.GoWorkWithData(testEvent.testData, doneIDData1);
        workProcessor.GoWorkWithDataCallback(testEvent.testData, doneIDData1, dfg2);

        //test report errorDoneID
        workProcessor.AddQueueWork(testEvent.testErrorDoneID, testQueueErrorDoneID);
        workProcessor.GoQueue(testEvent.testErrorDoneID);

        Debug.Log("!!!!!!!!!!!!!!!ERROR TEST END");

        workProcessor.RemoveAllWorks();

        //test RemoveAllWorks 
        workProcessor.GoWork(testEvent.testNoData);

    }
    #endregion
}
