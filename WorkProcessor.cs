using System;
using System.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace WorkProcessorSpace
{
    public class DoneIDData
    {
        public DoneIDData()
        {}

        private int doneID = -1;
        private bool isSetID = false;

        public bool IsSetID
        {
            get
            {
                return isSetID;
            }
        }
        public int DoneID
        {
            get
            {
                return doneID;
            }
            set
            {
                if (!isSetID)
                {
                    isSetID = true;
                    doneID = value;
                }
            }
        }
    }

    public delegate void FunctionType();
    public delegate void FunctionType<E>(E DoneIDData);
    public delegate void FunctionType<E,T>(E DoneIDData, T rceiveDataOrCallBackFunc);
    public delegate void FunctionType<E, T,U>(E DoneIDData, T receiveData,U callbackFunc);

    public class WorkProcessor
    {
        private WorkProcessor()
        {
            mainWorkList = waitWorkList0;
            subWorkList = waitWorkList1;
            checkDeadTimer.Elapsed += checkDeadWaitData;
            checkDeadTimer.Enabled = true;
            checkDeadTimer.Stop();
        }

        private enum dataType
        {
            noData,
            haveData,
            noDataQueue,
            haveDataQueue
        }

        private enum workCheckSituation
        {
            normal,
            noCommand,
            noFunction,
            notSameFunction
        }

        private bool isShowLog = true;
        private bool isShowCaution = false;
        private bool workAddRemoving = false;
        private static WorkProcessor instance = null;
        private List<Delegate> waitWorkList0 = new List<Delegate>();
        private List<Delegate> waitWorkList1 = new List<Delegate>();
        private List<Delegate> mainWorkList = null;
        private List<Delegate> subWorkList = null;
             
        private Dictionary<object, Delegate> noDataCommandDictionary = new Dictionary<object, Delegate>();
        private Dictionary<object, Delegate> commandDictionary = new Dictionary<object, Delegate>();
        private Dictionary<object, Delegate> noDataQueueCommandDictionary = new Dictionary<object, Delegate>();
        private Dictionary<object, Delegate> queueCommandDictionary = new Dictionary<object, Delegate>();

        private int doneID = 0;
        private List<int> standbyDoneIDList = new List<int>();
        private System.Random random = new System.Random();
        private bool workDoneWaiting = false;
        private bool checkDeadStart = false;
        private System.Timers.Timer checkDeadTimer=new System.Timers.Timer(5000);

        public static WorkProcessor GetInstance()
        {
            if (instance == null)
                instance = new WorkProcessor();

            return instance;
        }

       
        #region No Data Function

        //normal function
        private void addNoDataFunc(object inCommandType, FunctionType func)
        {
            if (workAddCheck(inCommandType, func, dataType.noData))
                noDataCommandDictionary[inCommandType] = (FunctionType)noDataCommandDictionary[inCommandType] + func;
            else
                showLog("ERROR AddWork "+ inCommandType + " command is exist ");
        }
        public void AddWork<E>(E commandType, FunctionType func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddWork "+ commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addNoDataFunc(commandType, func);
            workAddRemoving = false;
        }
               
        //have call back function
        private void addNoDataFunc(object inCommandType, FunctionType<Delegate> func)
        {
            if (workAddCheck(inCommandType, func, dataType.noData))
                noDataCommandDictionary[inCommandType] = (FunctionType<Delegate>)noDataCommandDictionary[inCommandType] + func;
            else
                showLog("ERROR AddWork " + inCommandType + " command is exist ");
        }
        public void AddWork<E>(E commandType, FunctionType<Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addNoDataFunc(commandType, func);
            workAddRemoving = false;
        }

        //have DoneID Data
        private void addNoDataQueueFunc(object inCommandType, FunctionType<DoneIDData> func)
        {
            if (workAddCheck(inCommandType, func, dataType.noDataQueue))
                noDataQueueCommandDictionary[inCommandType] = (FunctionType<DoneIDData>)noDataQueueCommandDictionary[inCommandType] + func;
            else
                showLog("ERROR AddQueueWork " + inCommandType + " command is exist ");
        }
        public void AddQueueWork<E>(E commandType, FunctionType<DoneIDData> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddQueueWork command must be enum"); 
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addNoDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }

        //have DoneIDData and callback function
        private void addNoDataQueueFunc(object inCommandType, FunctionType<DoneIDData, Delegate> func)
        {
            if (workAddCheck(inCommandType, func, dataType.noDataQueue))
                noDataQueueCommandDictionary[inCommandType] = (FunctionType<DoneIDData, Delegate>)noDataQueueCommandDictionary[inCommandType] + func;
            else
                showLog("ERROR AddQueueWork " + inCommandType + " command is exist ");
        }
        public void AddQueueWork<E>(E commandType, FunctionType<DoneIDData, Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddQueueWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addNoDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }
        #endregion


        #region Have Receive Data Function

        //Only have receive data
        private void addHaveDataFunc<T>(object inCommand, FunctionType<T> func)
        {
            if (workAddCheck(inCommand, func, dataType.haveData))
                commandDictionary[inCommand] = (FunctionType<T>)commandDictionary[inCommand] + func;
            else
                showLog("ERROR AddWork " + inCommand + " command is exist ");
        }
        public void AddWork<E,T>(E commandType, FunctionType<T> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addHaveDataFunc(commandType, func);
            workAddRemoving = false;
        }

        //have receive data and callback function
        private void addHaveDataFunc<T>(object inCommand, FunctionType<T, Delegate> func)
        {
            if (workAddCheck(inCommand, func, dataType.haveData))
                commandDictionary[inCommand] = (FunctionType<T, Delegate>)commandDictionary[inCommand] + func;
            else
                showLog("ERROR AddWork " + inCommand + " command is exist ");
        }
        public void AddWork<E, T>(E commandType, FunctionType<T, Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addHaveDataFunc(commandType, func);
            workAddRemoving = false;
        }

        //have doneIDData and receive data
        private void addHaveDataQueueFunc<T>(object inCommand, FunctionType<DoneIDData, T> func)
        {
            if (workAddCheck(inCommand, func, dataType.haveDataQueue))
                queueCommandDictionary[inCommand] = (FunctionType<DoneIDData, T>)queueCommandDictionary[inCommand] + func;
            else
                showLog("ERROR AddQueueWork " + inCommand + " command is exist "); 
        }
        public void AddQueueWork<E, T>(E commandType, FunctionType<DoneIDData, T> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddQueueWork command must be enum"); ;
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addHaveDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }

        //have DoneIDData receive data and callback function
        private void addHaveDataQueueFunc<T>(object inCommand, FunctionType<DoneIDData, T, Delegate> func)
        {
            if (workAddCheck(inCommand, func, dataType.haveDataQueue))
                queueCommandDictionary[inCommand] = (FunctionType<DoneIDData, T, Delegate>)queueCommandDictionary[inCommand] + func;
            else
                showLog("ERROR AddQueueWork " + inCommand + " command is exist ");
        }
        public void AddQueueWork<E, T>(E commandType, FunctionType<DoneIDData, T, Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR AddQueueWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR AddQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION AddQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            addHaveDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }
        #endregion


        #region Remove no data function

        //remove normal function
        private void removeNoDataFunc(object inCommand, FunctionType func)
        {
            switch (checkWorkExist(inCommand, func, dataType.noData))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveWork " +inCommand+" command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        noDataCommandDictionary[inCommand] = (FunctionType)noDataCommandDictionary[inCommand] - func;
                        workRemove(inCommand, dataType.noData);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveWork "+ inCommand+" command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.noData);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }
        }
        public void RemoveWork<E>(E commandType, FunctionType func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveWork " + commandType + " function is null ");
                return;
            }


            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeNoDataFunc(commandType, func);
            workAddRemoving = false;
        }

        //remove have callback function
        private void removeNoDataFunc(object inCommand, FunctionType<Delegate> func)
        {
            switch (checkWorkExist(inCommand, func, dataType.noData))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveWork " + inCommand + " command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        noDataCommandDictionary[inCommand] = (FunctionType<Delegate>)noDataCommandDictionary[inCommand] - func;
                        workRemove(inCommand, dataType.noData);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveWork " + inCommand + " command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.noData);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }
        }
        public void RemoveWork<E>(E commandType, FunctionType<Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveWork " + commandType + " function is null ");
                return;
            }


            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeNoDataFunc(commandType, func);
            workAddRemoving = false;
        }

        ///remove queue function
        private void removeNoDataQueueFunc(object inCommand, FunctionType<DoneIDData> func)
        {
            switch (checkWorkExist(inCommand, func, dataType.noDataQueue))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        noDataQueueCommandDictionary[inCommand] = (FunctionType<DoneIDData>)noDataQueueCommandDictionary[inCommand] - func;
                        workRemove(inCommand, dataType.noDataQueue);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.noDataQueue);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }
        }
        public void RemoveQueueWork<E>(E commandType, FunctionType<DoneIDData> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveQueueWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeNoDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }

        //remove queue have callback function
        private void removeNoDataQueueFunc(object inCommand, FunctionType<DoneIDData,Delegate> func)
        {
            switch (checkWorkExist(inCommand, func, dataType.noDataQueue))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        noDataQueueCommandDictionary[inCommand] = (FunctionType<DoneIDData,Delegate>)noDataQueueCommandDictionary[inCommand] - func;
                        workRemove(inCommand, dataType.noDataQueue);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.noDataQueue);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }
        }
        public void RemoveQueueWork<E>(E commandType, FunctionType<DoneIDData,Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveQueueWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeNoDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }
        #endregion


        #region Remove have data function

        //remove have receive data functioon 
        private void removeHaveDataFunc<T>(object inCommand, FunctionType<T> inFunc)
        {
            switch (checkWorkExist(inCommand, inFunc, dataType.haveData))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveWork " + inCommand + " command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        commandDictionary[inCommand] = (FunctionType<T>)commandDictionary[inCommand] - inFunc;
                        workRemove(inCommand, dataType.haveData);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveWork " + inCommand + " command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.haveData);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }

        }
        public void RemoveWork<E,T>(E commandType, FunctionType<T> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeHaveDataFunc(commandType, func);
            workAddRemoving = false;
        }

        //remove have receive data and callback function
        private void removeHaveDataFunc<T>(object inCommand, FunctionType<T,Delegate> inFunc)
        {
            switch (checkWorkExist(inCommand, inFunc, dataType.haveData))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveWork " + inCommand + " command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        commandDictionary[inCommand] = (FunctionType<T,Delegate>)commandDictionary[inCommand] - inFunc;
                        workRemove(inCommand, dataType.haveData);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveWork " + inCommand + " command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.haveData);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }

        }
        public void RemoveWork<E, T>(E commandType, FunctionType<T,Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeHaveDataFunc(commandType, func);
            workAddRemoving = false;
        }

        //remove have receive data queue function
        private void removeHaveDataQueueFunc<T>(object inCommand, FunctionType<DoneIDData, T> inFunc)
        {
            switch (checkWorkExist(inCommand, inFunc, dataType.haveDataQueue))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        queueCommandDictionary[inCommand] = (FunctionType<DoneIDData, T>)queueCommandDictionary[inCommand] - inFunc;
                        workRemove(inCommand, dataType.haveDataQueue);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.haveDataQueue);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }

        }
        public void RemoveQueueWork<E, T>(E commandType, FunctionType<DoneIDData, T> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveQueueWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeHaveDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }

        //remove have receive data and callback function
        private void removeHaveDataQueueFunc<T>(object inCommand, FunctionType<DoneIDData,T,Delegate> inFunc)
        {
            switch (checkWorkExist(inCommand, inFunc, dataType.haveDataQueue))
            {
                case workCheckSituation.noCommand:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command is not exist");
                    break;
                case workCheckSituation.normal:
                    {
                        queueCommandDictionary[inCommand] = (FunctionType<DoneIDData,T,Delegate>)queueCommandDictionary[inCommand] - inFunc;
                        workRemove(inCommand, dataType.haveDataQueue);
                    }
                    break;
                case workCheckSituation.noFunction:
                    {
                        showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function not exist,still remove");
                        workRemove(inCommand, dataType.haveDataQueue);
                    }
                    break;
                case workCheckSituation.notSameFunction:
                    showLog("ERROR RemoveQueueWork " + inCommand + " command exist but function differentt,not remove");
                    break;
                default:
                    break;
            }

        }
        public void RemoveQueueWork<E, T>(E commandType, FunctionType<DoneIDData,T,Delegate> func) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR RemoveQueueWork command must be enum");
                return;
            }

            if (func == null)
            {
                showLog("ERROR RemoveQueueWork " + commandType + " function is null ");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveQueueWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeHaveDataQueueFunc(commandType, func);
            workAddRemoving = false;
        }
        #endregion

        //remove all function
        private void removeAllWorks()
        {
            commandDictionary.Clear();
            noDataCommandDictionary.Clear();
            queueCommandDictionary.Clear();
            noDataQueueCommandDictionary.Clear();
        }
        public void RemoveAllWorks()
        {
            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION RemoveAllWork doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            removeAllWorks();
            workAddRemoving = false;
        }

        #region Gowork No Data

        //dispatch normal function
        private void goNoDataFunc(object inCommand)
        {
            if(!noDataCommandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoWork "+ inCommand+" command not exist");
                return;
            }

            Delegate d;
            noDataCommandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoWork " + inCommand + " command function not exist");
                return;
            }

            FunctionType func = d as FunctionType;

            if (func != null)
                func();
            else
                showLog("ERROR GoWork " + inCommand + " command function type different");

        }
        public void GoWork<E>(E commandType) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoWork command must be enum");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoWork " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goNoDataFunc(commandType);
            workAddRemoving = false;
        }

        //dispatch have callback function
        private void goNoDataFunc(object inCommand,Delegate inFunc)
        {
            if (!noDataCommandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoWorkWithCallback " + inCommand + " command not exist");
                return;
            }

            Delegate d;
            noDataCommandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoWorkWithCallback " + inCommand + " command function not exist");
                return;
            }

            FunctionType<Delegate> func = d as FunctionType<Delegate>;

            if (func != null)
                func(inFunc);
            else
                showLog("ERROR GoWorkWithCallback " + inCommand + " command function type different");

        }
        public void GoWorkWithCallback<E>(E commandType,Delegate inFunc) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoWorkWithCallback command must be enum");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoWorkWithCallback " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goNoDataFunc(commandType,inFunc);
            workAddRemoving = false;
        }
        #endregion


        #region Gowork have data function

        //dispatch have receive data function
        private void goHaveDataFunc<T>(object inCommand,T inData)
        {
            if (!commandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoWorkWithData " + inCommand + " command not exist");
                return;
            }

            Delegate d;
            commandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoWorkWithData " + inCommand + " command function not exist");
                return;
            }

            FunctionType<T> func = d as FunctionType<T>;

            if (func != null)
                func(inData);
            else
                showLog("ERROR GoWorkWithData " + inCommand + " command function type different");
        }
        public void GoWorkWithData<E,T>(E commandType, T inData) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoWorkWithData command must be enum");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoWorkWithData " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goHaveDataFunc(commandType, inData);
            workAddRemoving = false;
        }

        //dispatch have receive data and callback function
        private void goHaveDataFunc<T>(object inCommand, T inData,Delegate inFunc)
        {
            if (!commandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoWorkWithDataCallback " + inCommand + " command not exist");
                return;
            }

            Delegate d;
            commandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoWorkWithDataCallback " + inCommand + " command function not exist");
                return;
            }

            FunctionType<T,Delegate> func = d as FunctionType<T,Delegate>;

            if (func != null)
                func(inData,inFunc);
            else
                showLog("ERROR GoWorkWithDataCallback " + inCommand + " command function type different");
        }
        public void GoWorkWithDataCallback<E, T>(E commandType, T inData,Delegate inFunc) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoWorkWithDataCallback command must be enum");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoWorkWithDataCallback " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goHaveDataFunc(commandType, inData,inFunc);
            workAddRemoving = false;
        }
        #endregion


        #region GoWorkQueue no data 

        //dispatch normal function in queue
        private void goQueueNoDataFunc(object inCommand)
        {
            if (!noDataQueueCommandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoQueue " + inCommand + " command not exist");
                return;
            }

            Delegate d;
            noDataQueueCommandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoQueue " + inCommand + " command function not exist");
                return;
            }
                       
            FunctionType<DoneIDData> func = d as FunctionType<DoneIDData>;
            if(func==null)
            {
                showLog("ERROR GoQueue " + inCommand + " command function type different");
                return;
            }

            int id = getDoneID();
            standbyDoneIDList.Add(id);
            DoneIDData doneIDData = makeDoneIDData(id);

            if (!workDoneWaiting)
            {
                workDoneWaiting = true;
                func(doneIDData);
            }
            else
            {
                FunctionType doFunc = delegate ()
                {
                    func(doneIDData);
                };

                subWorkList.Add(doFunc);
            }
        }
        public void GoQueue<E>(E commandType) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoQueue command must be enum");
                return;
            }

           if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoQueue " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goQueueNoDataFunc(commandType);
            workAddRemoving = false;
        }

        //dispatch have callback function in queue
        private void goQueueNoDataFunc(object inCommand,Delegate inFunc)
        {
            if (!noDataQueueCommandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoQueueWithCallback " + inCommand + " command not exist");
                return;
            }

            Delegate d;
            noDataQueueCommandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoQueueWithCallback " + inCommand + " command function not exist");
                return;
            }

            FunctionType<DoneIDData, Delegate> func = d as FunctionType<DoneIDData, Delegate>;
            if (func == null)
            {
                showLog("ERROR GoQueueWithCallback " + inCommand + " command function type different");
                return;
            }

            int id = getDoneID();
            standbyDoneIDList.Add(id);
            DoneIDData doneIDData = makeDoneIDData(id);

            if (!workDoneWaiting)
            {
                workDoneWaiting = true;
                func(doneIDData, inFunc);
            }
            else
            {
                FunctionType doFunc = delegate ()
                {
                    func(doneIDData, inFunc);
                };
                subWorkList.Add(doFunc);
            }
        }
        public void GoQueueWithCallback<E>(E commandType, Delegate inFunc) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoQueueWithCallback command must be enum");
                return;
            }

          if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoQueueWithCallback " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goQueueNoDataFunc(commandType, inFunc);
            workAddRemoving = false;
        }
        #endregion


        #region GoworkQueue have data

        //dispatch have data function in queue
        private void goQueueHaveDataFunc<T>(object inCommand,T inReceiveData)
        {
            if (!queueCommandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoQueueWithData " + inCommand + " command not exist");
                return;
            }

            Delegate d;
            queueCommandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoQueueWithData " + inCommand + " command function not exist");
                return;
            }

            FunctionType<DoneIDData, T> func = d as FunctionType<DoneIDData, T>;
            if (func == null)
            {
                showLog("ERROR GoQueueWithData " + inCommand + " command function type different");
                return;
            }

            int id = getDoneID();
            standbyDoneIDList.Add(id);
            DoneIDData doneIDData = makeDoneIDData(id);

            if (!workDoneWaiting)
            {
                workDoneWaiting = true;
                func(doneIDData, inReceiveData);
            }
            else
            {
                FunctionType doFunc = delegate ()
                {
                    func(doneIDData, inReceiveData);
                };
                subWorkList.Add(doFunc);
            }
        }
        public void GoQueueWithData<E,T>(E commandType,T inReceiveData) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoQueueWithData command must be enum");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoQueueWithData " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goQueueHaveDataFunc(commandType,inReceiveData);
            workAddRemoving = false;
        }

        //dispatch have data and callback function in queue
        private void goQueueHaveDataFunc<T>(object inCommand, T inReceiveData, Delegate inFunc)
        {
            if (!queueCommandDictionary.ContainsKey(inCommand))
            {
                showLog("ERROR GoQueueWithDataCallback " + inCommand + " command not exist");
                return;
            }

            Delegate d;
            queueCommandDictionary.TryGetValue(inCommand, out d);
            if (d == null)
            {
                showLog("ERROR GoQueueWithDataCallback " + inCommand + " command function not exist");
                return;
            }

            FunctionType<DoneIDData, T, Delegate> func = d as FunctionType<DoneIDData, T, Delegate>;
            if (func == null)
            {
                showLog("ERROR GoQueueWithDataCallback " + inCommand + " command function type different");
                return;
            }

            int id = getDoneID();
            standbyDoneIDList.Add(id);
            DoneIDData doneIDData = makeDoneIDData(id);

            if (!workDoneWaiting)
            {
                workDoneWaiting = true;
                func(doneIDData, inReceiveData, inFunc);
            }
            else
            {
                FunctionType doFunc = delegate ()
                {
                    func(doneIDData, inReceiveData, inFunc);
                };
                subWorkList.Add(doFunc);
            }
        }
        public void GoQueueWithDataCallback<E,T>(E commandType,T inReceiveData, Delegate inFunc) where E : struct, IConvertible
        {
            if (!typeof(E).IsEnum)
            {
                showLog("ERROR GoQueueWithDataCallback command must be enum");
                return;
            }

            if (workAddRemoving)
            {
                if (isShowCaution)
                    showLog("CAUTION GoQueueWithDataCallback " + commandType + " doing when other work not finish");

                //maybe do something

            }

            workAddRemoving = true;
            goQueueHaveDataFunc(commandType,inReceiveData ,inFunc);
            workAddRemoving = false;
        }
        #endregion


        #region Report Finish, every work's DoneID is not -1,is need to call ReportFinish and send back the DoneID

        private void checkNextWork()
        {
            if (mainWorkList.Count == 0)
            {
                if (subWorkList.Count != 0)
                    mainSubChange();
                else
                    workDoneWaiting = false;
            }

            if (mainWorkList.Count != 0)
            {
                Delegate d = mainWorkList[0];
                mainWorkList.RemoveAt(0);
                d.DynamicInvoke();
            }
        }
        private void checkWaitWorkAndDoNextWork(DoneIDData inData)
        {

            if (inData.DoneID == -1)
            {
                showLog("ERROR checkWaitWorkAndDoNextWork DONEID is -1");
                return;
            }

            if (inData.DoneID != standbyDoneIDList[0])//report finish error
            {
                showLog("ERROR report finish doneID different");

                if (!checkDeadStart)
                {
                    checkDeadStart = true;
                    showLog("!!!!!WORKPROCESSOR TIMER START !!!!");
                    checkDeadTimer.Start();
                }

                return;
            }

            //report doneID is correct
            standbyDoneIDList.RemoveAt(0);

            if (checkDeadStart)//report error early but this time is correct
            {
                checkDeadStart = false;
                showLog("!!!!!WORKPROCESSOR TIMER STOP !!!!");
                checkDeadTimer.Stop();
            }

            checkNextWork();
        }
        public void ReportFinish(DoneIDData inData)
        {
            if (workDoneWaiting && standbyDoneIDList.Count != 0)
                checkWaitWorkAndDoNextWork(inData);
            else
                showLog("ERROR report finish without wait doneID");
        }
        #endregion


        private DoneIDData makeDoneIDData(int inID)
        {
            DoneIDData data = new DoneIDData();
            data.DoneID = inID;

            return data;
        }

        private int getDoneID()
        {
            doneID += random.Next(1, 100);
            if (doneID > 50000)
                doneID = 0;

            return doneID;
        }

        private void mainSubChange()
        {
            List<Delegate> list = mainWorkList;
            mainWorkList = subWorkList;
            subWorkList = list;
        }

        private void checkDeadWaitData(System.Object source, ElapsedEventArgs e)
        {
            showLog("!!!!!WORKPROCESSOR TIMER STOP IN checkDeadWaitData !!!!");
            checkDeadTimer.Stop();

            if (checkDeadStart)
            {
                checkDeadStart = false;

                if(standbyDoneIDList.Count!=0)
                    standbyDoneIDList.RemoveAt(0);
               
                checkNextWork();
            }
        }

        private bool workAddCheck(object commandType, Delegate listenerBeingAdded, dataType inType) 
        {
            Dictionary<object, Delegate> dic = null;
            if (inType == dataType.haveData)
                dic = commandDictionary;
            else if (inType == dataType.noData)
                dic = noDataCommandDictionary;
            else if (inType == dataType.noDataQueue)
                dic = noDataQueueCommandDictionary;
            else if (inType == dataType.haveDataQueue)
                dic = queueCommandDictionary;

            if (!dic.ContainsKey(commandType))
            {
                dic.Add(commandType, null);
                return true;
            }

            return false;
        }

        private workCheckSituation checkWorkExist(object commandType, Delegate listenerBeingRemoved, dataType inType)
        {
            Dictionary<object, Delegate> dic = null;
            if (inType == dataType.haveData)
                dic = commandDictionary;
            else if (inType == dataType.noData)
                dic = noDataCommandDictionary;
            else if (inType == dataType.noDataQueue)
                dic = noDataQueueCommandDictionary;
            else if (inType == dataType.haveDataQueue)
                dic = queueCommandDictionary;

            if (dic.ContainsKey(commandType))
            {
                Delegate d = dic[commandType];

                if (d == null)
                    return workCheckSituation.noFunction;

                if (d != listenerBeingRemoved)
                    return workCheckSituation.notSameFunction;

                return workCheckSituation.normal;
            }

            return workCheckSituation.noCommand;
        }

        private void workRemove(object commandType, dataType inType) 
        {
            Dictionary<object, Delegate> dic = null;
            if (inType == dataType.haveData)
                dic = commandDictionary;
            else if(inType== dataType.noData)
                dic = noDataCommandDictionary;
            else if (inType == dataType.noDataQueue)
                dic = noDataQueueCommandDictionary;
            else if (inType == dataType.haveDataQueue)
                dic = queueCommandDictionary;

            if (dic[commandType] == null)
                dic.Remove(commandType);
        }

        private void showLog(string inText)
        {
            if (!isShowLog)
                return;

            Debug.Log(inText);
        }
    }
}
