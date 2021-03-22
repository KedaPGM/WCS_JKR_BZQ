﻿using enums;
using module.device;
using module.goods;
using resource;
using System;
using System.Collections.Generic;
using System.Threading;
using task.task;
using tool.mlog;
using tool.timer;

namespace task.device
{
    public class TrafficControlMaster
    {
        #region[字段]
        private readonly object _in, _out;
        private Thread _mRefresh;
        private bool Refreshing = true;
        private MTimer mTimer;
        private List<TrafficControl> TrafficCtlList { set; get; }
        private Log mLog;
        #endregion

        #region[构造/初始化]

        public TrafficControlMaster()
        {
            mLog = (Log)new LogFactory().GetLog("交通管制", false);
            _in = new object();
            _out = new object();
            mTimer = new MTimer();
            TrafficCtlList = new List<TrafficControl>();
            //Init(); // 不管旧的
        }

        private void Init()
        {
            TrafficCtlList.Clear();
            TrafficCtlList.AddRange(PubMaster.Mod.TrafficCtlSql.QueryTrafficCtlList());
        }

        public void Start()
        {
            if (_mRefresh == null || !_mRefresh.IsAlive || _mRefresh.ThreadState == ThreadState.Aborted)
            {
                _mRefresh = new Thread(Handle)
                {
                    IsBackground = true
                };
            }

            _mRefresh.Start();
        }

        public void Handle()
        {
            while (Refreshing)
            {
                Thread.Sleep(1000);
                if (Monitor.TryEnter(_in, TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        TrafficCtlList.RemoveAll(c => c.TrafficControlStatus == TrafficControlStatusE.已完成);
                        if (TrafficCtlList != null || TrafficCtlList.Count != 0)
                        {
                            foreach (TrafficControl ctl in TrafficCtlList)
                            {
                                try
                                {
                                    switch (ctl.TrafficControlType)
                                    {
                                        case TrafficControlTypeE.运输车交管运输车:
                                            ControlCarrierByCarrier(ctl);
                                            break;
                                        case TrafficControlTypeE.摆渡车交管摆渡车:
                                            ControlFerryByFerry(ctl);
                                            break;
                                        case TrafficControlTypeE.运输车交管摆渡车:
                                            ControlFerryByCarrier(ctl);
                                            break;
                                        case TrafficControlTypeE.摆渡车交管运输车:
                                            ControlCarrierByFerry(ctl);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    mLog.Error(true, "[ID:" + ctl?.id + "]", e);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        mLog.Error(true, e.Message, e);
                    }
                    finally
                    {
                        Monitor.Exit(_in);
                    }
                }
            }
        }

        public void Stop()
        {
            Refreshing = false;
            _mRefresh?.Abort();
        }
        #endregion

        #region[获取对象]

        public List<TrafficControl> GetTrafficCtlList()
        {
            return TrafficCtlList;
        }

        public List<TrafficControl> GetTrafficCtlList(TrafficControlTypeE type)
        {
            return TrafficCtlList.FindAll(c => c.TrafficControlType == type);
        }

        public List<TrafficControl> GetTrafficCtlList(List<TrafficControlTypeE> types)
        {
            return TrafficCtlList.FindAll(c => types.Contains(c.TrafficControlType));
        }

        public List<TrafficControl> GetTrafficCtlList(List<TrafficControlTypeE> types, uint areaid)
        {
            return TrafficCtlList.FindAll(c => c.area == areaid && types.Contains(c.TrafficControlType));
        }

        public List<TrafficControl> GetTrafficCtlList(List<TrafficControlTypeE> types, List<uint> areaids)
        {
            return TrafficCtlList.FindAll(c => types.Contains(c.TrafficControlType) && areaids.Contains(c.area));
        }

        #endregion

        #region[方法/判断]

        /// <summary>
        /// 新增交管
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool AddTrafficControl(TrafficControl tc, out string result)
        {
            if (Monitor.TryEnter(_out, TimeSpan.FromSeconds(2)))
            {
                try
                {
                    if (TrafficCtlList.Exists(c => c.TrafficControlType == tc.TrafficControlType &&
                                                                 c.restricted_id == tc.restricted_id &&
                                                                 c.control_id == tc.control_id &&
                                                                 c.TrafficControlStatus != TrafficControlStatusE.已完成))
                    {
                        result = "已经存在相同交管类型的对应设备！";
                        return false;
                    }

                    tc.id = PubMaster.Dic.GenerateID(DicTag.NewTranId);
                    tc.TrafficControlStatus = TrafficControlStatusE.交管中;
                    tc.create_time = DateTime.Now;
                    PubMaster.Mod.TrafficCtlSql.AddTrafficCtl(tc);
                    TrafficCtlList.Add(tc);
                    result = "生成交管.";
                    return true;
                }
                finally
                {
                    Monitor.Exit(_out);
                }
            }

            result = "稍后再试！";
            return false;
        }


        /// <summary>
        /// 是否存在设备已被交管
        /// </summary>
        /// <param name="restricted_id"></param>
        /// <returns></returns>
        public bool ExistsRestricted(uint restricted_id)
        {
            return TrafficCtlList.Exists(c => c.TrafficControlStatus == TrafficControlStatusE.交管中 && c.restricted_id == restricted_id);
        }

        /// <summary>
        /// 是否存在设备已被同类型交管
        /// </summary>
        /// <param name="tct"></param>
        /// <param name="devid"></param>
        /// <returns></returns>
        public bool ExistsTrafficControl(TrafficControlTypeE tct, uint devid)
        {
            return TrafficCtlList.Exists(c => c.TrafficControlStatus == TrafficControlStatusE.交管中 &&
                c.TrafficControlType == tct && (c.restricted_id == devid || c.control_id == devid));
        }


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="ctl"></param>
        /// <param name="status"></param>
        /// <param name="memo"></param>
        internal void SetStatus(TrafficControl ctl, TrafficControlStatusE status, string memo = "")
        {
            if (ctl.TrafficControlStatus != status)
            {
                ctl.TrafficControlStatus = status;
                ctl.update_time = System.DateTime.Now;
                PubMaster.Mod.TrafficCtlSql.EditTrafficCtl(ctl, TrafficControlUpdateE.Status);
            }
            mLog.Status(true, string.Format("交管：{0}，状态【{1} -> {2}】, 备注：{3}", ctl.id, ctl.TrafficControlStatus, status, memo));
        }

        #endregion

        #region [交管逻辑]

        /// <summary>
        /// 运输车交管运输车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlCarrierByCarrier(TrafficControl ctl)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 摆渡车交管摆渡车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlFerryByFerry(TrafficControl ctl)
        {
            try
            {
                // 是否存在被运输车交管
                if (ExistsTrafficControl(TrafficControlTypeE.运输车交管摆渡车, ctl.control_id))
                {
                    return;
                }

                // 交管车当前位置是否满足结束交管条件
                if (IsMeetLocationForFerry(ctl.control_id, ctl.from_track_id, ctl.to_track_id, out string result))
                {
                    SetStatus(ctl, TrafficControlStatusE.已完成, result);
                    return;
                }

                // 是否允许交管摆渡车移动
                if (IsAllowToMoveForFerry(ctl.control_id, out result))
                {
                    // 让交管车定位到结束点
                    if (PubTask.Ferry.DoLocateFerry(ctl.control_id, ctl.to_track_id, out result))
                    {
                        SetStatus(ctl, TrafficControlStatusE.已完成, result);
                        return;
                    }
                }
                SetStatus(ctl, TrafficControlStatusE.交管中, result);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 运输车交管摆渡车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlFerryByCarrier(TrafficControl ctl)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 摆渡车交管运输车
        /// </summary>
        /// <param name="ctl"></param>
        private void ControlCarrierByFerry(TrafficControl ctl)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region [ 交管摆渡车判断 ]

        /// <summary>
        /// 是否满足交管摆渡车位置条件
        /// </summary>
        /// <returns></returns>
        private bool IsMeetLocationForFerry(uint ferryid, uint fromTraid, uint toTraid, out string result)
        {
            result = "";
            // 当前轨道ID
            uint nowTraid = PubTask.Ferry.GetFerryCurrentTrackId(ferryid);
            if (nowTraid == 0)
            {
                result = "没有交管车当前轨道数据【跳过】";
                return false;
            }

            // 当前轨道顺序
            short Norder = PubMaster.Track.GetTrack(nowTraid)?.order ?? 0;
            // 起始轨道顺序
            short Forder = PubMaster.Track.GetTrack(fromTraid)?.order ?? 0;
            // 结束轨道顺序
            short Torder = PubMaster.Track.GetTrack(toTraid)?.order ?? 0;

            if (Norder == 0 || Forder == 0 || Torder == 0)
            {
                // 没配置？不管了 直接完成
                result = "没配置轨道序号【满足条件】";
                return true;
            }

            // 当前位置 需要在移动方向之外
            if ((Torder > Forder && Norder >= Torder) ||
                (Torder < Forder && Norder <= Torder))
            {
                result = "在范围移动方向之外【满足条件】";
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否允许交管摆渡车移动
        /// </summary>
        /// <returns></returns>
        private bool IsAllowToMoveForFerry(uint ferryid, out string result)
        {
            FerryTask ferry = PubTask.Ferry.GetFerry(ferryid);
            if (!PubTask.Ferry.IsAllowToMove(ferry, out result))
            {
                return false;
            }

            // 是否锁定任务 判断任务节点是否允许移动
            if (ferry.IsLock && ferry.TransId != 0)
            {
                StockTrans trans = PubTask.Trans.GetTrans(ferry.TransId);
                if (trans != null && !trans.finish)
                {
                    List<uint> Ftraids = ferry.GetFerryCurrentTrackIds();
                    if (Ftraids == null || Ftraids.Count == 0)
                    {
                        result = "无摆渡车对位数据！";
                        return false;
                    }

                    // 空车 - 在运输车对应位置 则不能移动
                    uint Ctraid = PubTask.Carrier.GetCarrierTrackID(trans.carrier_id);
                    if (Ftraids.Contains(Ctraid))
                    {
                        result = "对应运输车任务待定！";
                        return false;
                    }

                    // 载车 - 在任务的对应位置 则不能移动
                    if (ferry.Load == DevFerryLoadE.载车)
                    {
                        switch (trans.TransType)
                        {
                            case TransTypeE.入库:
                            case TransTypeE.手动入库:
                                if (trans.TransStaus == TransStatusE.取砖流程 && Ftraids.Contains(trans.take_track_id))
                                {
                                    result = "准备取货！";
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.放砖流程 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = "准备卸货！";
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.取消 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = "取消中！";
                                    return false;
                                }
                                break;
                            case TransTypeE.出库:
                            case TransTypeE.手动出库:
                                if (trans.TransStaus == TransStatusE.取砖流程)
                                {
                                    // 运输车无货 需要取砖
                                    if (PubTask.Carrier.IsNotLoad(trans.carrier_id) && Ftraids.Contains(trans.take_track_id))
                                    {
                                        result = "准备取货！";
                                        return false;
                                    }
                                    // 运输车载货 需要放砖
                                    if (PubTask.Carrier.IsLoad(trans.carrier_id) && Ftraids.Contains(trans.give_track_id))
                                    {
                                        result = "准备卸货！";
                                        return false;
                                    }
                                }
                                if (trans.TransStaus == TransStatusE.还车回轨 && Ftraids.Contains(trans.finish_track_id))
                                {
                                    result = "准备还车回轨！";
                                    return false;
                                }
                                if (trans.TransStaus == TransStatusE.取消 && Ftraids.Contains(trans.take_track_id))
                                {
                                    result = "取消中！";
                                    return false;
                                }
                                break;
                            case TransTypeE.倒库:
                            case TransTypeE.移车:
                                if (trans.TransStaus == TransStatusE.移车中 && Ftraids.Contains(trans.give_track_id))
                                {
                                    result = "准备移车！";
                                    return false;
                                }
                                break;
                        }
                    }

                }
            }

            return true;
        }

        #endregion

    }
}
