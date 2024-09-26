using UnityEngine;
using System.Collections.Generic;
using System;

public class TimerManager 
{
    /// <summary>
    /// Clock callback
    /// </summary>
    public delegate void TimerCallBack(int n);

    class TimerEntity
    {
        private static int TimeId = 1;

        public int id = 0;
        public float delay = 0;
        public float oldTime = 0;
        public int maxCount = 0;
        public TimerCallBack callBack;
        public System.Action<int> finish;
        public bool isRemove = false;

        public TimerEntity(float delay, TimerCallBack callBack, int maxCount)
        {
            this.delay = delay;
            this.callBack = callBack;
            this.maxCount = maxCount;
            this.oldTime = Time.realtimeSinceStartup;
            this.id = TimerEntity.TimeId++;
        }

        public TimerEntity(float delay, System.Action<int> finish, int maxCount)
        {
            this.delay = delay;
            this.finish = finish;
            this.maxCount = maxCount;
            this.oldTime = Time.realtimeSinceStartup;
            this.id = TimerEntity.TimeId++;
        }
    }


    class TimerInfo
    {
        private static int TimeId = 1;
        public int id;
        public float endTime;
        public System.Action<int> finish;
        public bool isRemove = false;

        public TimerInfo(float endTime, System.Action<int> finish)
        {
            this.endTime = endTime + Time.time;
            this.finish = finish;
            this.id = TimerInfo.TimeId++;
        }
    }

    public class TimerStringKey
    {
        public string Id;
        public float duration;
        public System.Action onFinish;
        public System.Action<int> onTick;
        public float ticker;
        
        public float mUpdateInterval = 1;
        public float mUpdateTimer = 0;
        public int mUpdateCounter = 0;
        /// <summary>
        /// force stop
        /// </summary>
        public bool forceTerminate;
        public TimerStringKey(string id, float duration, System.Action finish, System.Action<int> onTick)
        {
            this.Id = id;
            this.duration = duration;
            this.onFinish = finish;
            this.onTick = onTick;
            mUpdateCounter = 0;
        }

        public void ProcessTick()
        {
            mUpdateTimer += Time.deltaTime;
            if (mUpdateTimer>=mUpdateInterval)
            {
                mUpdateCounter++;
                onTick?.Invoke(mUpdateCounter);
                mUpdateTimer = 0;
            }
        }
    }

    public static readonly TimerManager Instance = new TimerManager();
    private Dictionary<int, TimerInfo> timerInfoDic;
    private List<TimerInfo> timerInfoList;

    private List<TimerStringKey> timerStringKeyList;
    private List<TimerStringKey> timerStringKeyListTimeOut;
    private Dictionary<int, TimerEntity> timerDic;
    private List<TimerEntity> timerList;
    private int count = 0;
    private int count2 = 0;

    private TimerStringKey currentTimer;

    public TimerManager()
    {
        timerDic = new Dictionary<int, TimerEntity>();
        timerList = new List<TimerEntity>();
        timerInfoDic = new Dictionary<int, TimerInfo>();
        timerInfoList = new List<TimerInfo>();
        timerStringKeyList = new List<TimerStringKey>();
        timerStringKeyListTimeOut = new List<TimerStringKey>();
    }

    /// <summary>
    /// Add timer
    /// </summary>
    public int AddTimer(float endTime, System.Action<int> finish)
    {
        TimerInfo timer = new TimerInfo(endTime, finish);

        if (!timerInfoDic.ContainsKey(timer.id))
        {
            timerInfoDic[timer.id] = timer;
            timerInfoList.Add(timer);
            count2++;
        }
        return timer.id;
    }

    private void RemoveTimer(int id, int index)
    {
        if (timerInfoDic.ContainsKey(id))
        {
            count2--;
            count2 = count2 <= 0 ? 0 : count2;
            timerInfoDic.Remove(id);
            timerInfoList.RemoveAt(index);
        }
    }

    /// <summary>
    /// Transsky timer
    /// startTime Online current timestamp
    /// </summary>
    public void AddTimer(string id, float duration, System.Action finish, System.Action<int> onTick)
    {
        TimerStringKey timer = new TimerStringKey(id,duration,finish:finish,onTick);
        timerStringKeyList.Add(timer);   
        onTick?.Invoke(0);
    }

    public void RemoveTimer(string id)
    {
        var timer = GetTimer(id);
        if(timer!= null)
        timer.forceTerminate = true;
    }

    public TimerStringKey GetTimer(string id)
    {
        return timerStringKeyList.Find(t => t.Id == id);
    }

    public bool Exist(string id)
    {
        return timerStringKeyList.Exists(t => t.Id == id);
    }

    public void Update()
    {
        if (timerInfoList != null && timerInfoList.Count > 0)
        {
            TimerInfo timer;
            for (int i = 0; i < timerInfoList.Count; i++)
            {
                timer = timerInfoList[i];
                if (timer.isRemove)
                {
                    RemoveTimer(timer.id, i);
                    continue;
                }
                float sysTime = Time.time;
                if (timer.endTime >= sysTime)
                {
                    if (timer.finish != null)
                    {
                        timer.finish(Mathf.FloorToInt((timer.endTime - sysTime)));
                    }
                }
                else
                    RemoveTimer(timer.id, i);
            }
        }



        if (timerList != null && timerList.Count > 0)
        {
            TimerEntity timer;
            float nowTime = Time.realtimeSinceStartup;
            int num = 0;
            for (int i = 0; i < timerList.Count;)
            {
                timer = timerList[i];
                if (timer.isRemove)
                {
                    RemoveTimer(timer, i);
                    continue;
                }
                num = (int)((nowTime - timer.oldTime) / timer.delay);
                if (num > 0)
                {
                    timer.maxCount -= num;
                    timer.oldTime += num * timer.delay;
                    if (timer.callBack != null)
                        timer.callBack(num);
                    if (timer.finish != null)
                        timer.finish(num);
                    if (timer.maxCount <= 0)
                    {
                        RemoveTimer(timer, i);
                        continue;
                    }
                }
                i++;
            }
        }

        if (timerStringKeyList.Count>0)
        {
            for (int i = 0; i < timerStringKeyList.Count; i++)
            {
                currentTimer = timerStringKeyList[i];
                currentTimer.ticker += Time.deltaTime;
                
                currentTimer.ProcessTick();
                
                if (currentTimer.ticker>=currentTimer.duration 
                    || currentTimer.forceTerminate)
                {
                    currentTimer.onFinish?.Invoke();
                    timerStringKeyListTimeOut.Add(currentTimer);
                }
            }
            
            if (timerStringKeyListTimeOut.Count>0)
            {
                for (int i = 0; i < timerStringKeyListTimeOut.Count; i++)
                {
                    timerStringKeyList.Remove(timerStringKeyListTimeOut[i]);
                }
                timerStringKeyListTimeOut.Clear();
            }
        }
    }


    /// <summary>
    /// Add clock
    /// </summary>
    public int Add(TimerCallBack callBack, float delay = 1f, int maxCount = int.MaxValue, bool immediate = false)
    {
        TimerEntity timer = new TimerEntity(delay, callBack, maxCount);
        if (immediate)
        {
            callBack(1);
        }
        timerDic.Add(timer.id, timer);
        timerList.Add(timer);
        count++;
        return timer.id;
    }

    /// <summary>
    /// Add clock
    /// </summary>
    public int Add2(System.Action<int> finish, float delay = 1f, int maxCount = int.MaxValue, bool immediate = false)
    {
        TimerEntity timer = new TimerEntity(delay, finish, maxCount);
        if (immediate)
        {
            finish(1);
        }
        timerDic.Add(timer.id, timer);
        timerList.Add(timer);
        count++;
        return timer.id;
    }

    /// <summary>
    /// 移除时钟
    /// </summary>
    public void Remove(int id)
    {
        TimerEntity timer;
        if (timerDic.TryGetValue(id, out timer))
        {
            timer.isRemove = true;
        }
        TimerInfo timer2;
        if (timerInfoDic.TryGetValue(id, out timer2))
        {
            timer2.isRemove = true;
            count2--;
            count2 = count2 <= 0 ? 0 : count2;
            foreach (var item in timerInfoList)
            {
                if (item.id == id)
                {
                    timerInfoList.Remove(item);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Remove the clock and clean up
    /// </summary>
    public void Remove(ref int id)
    {
        Remove(id);
        id = -1;
    }

    /// <summary>
    /// Remove clock
    /// </summary>
    private void RemoveTimer(TimerEntity timer, int index)
    {
        timer.callBack = null;
        timerDic.Remove(timer.id);
        timerList.RemoveAt(index);
        count--;
    }

    internal void AddTimer(float v, object callback)
    {
        throw new NotImplementedException();
    }
}