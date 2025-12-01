using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YungkuSystem.Controls;
using YungkuSystem.Globalization;

namespace Yungku.BNU01_V1.Handler.Logic.StationAction
{
    /// <summary>
    /// 测试位置类型枚举
    /// </summary>
    public enum TestPositionType
    {
        测试位置1,
        测试位置2,
        返回上料位
    }

    /// <summary>
    /// 测试位置移动控制类
    /// </summary>
    public class ActionTestPosition : ActionObject
    {
        public override string ObjectClass
        {
            get { return "测试位置移动"; }
        }

        public override bool IsContainner
        {
            get { return false; }
        }

        public override bool IsRunState
        {
            get { return true; }
        }

        /// <summary>
        /// 执行是否成功
        /// </summary>
        public bool IsSucceed { get; set; } = true;

        private bool xMoveOk = false;
        private bool yMoveOk = false;
        private bool zMoveOk = false;
        private bool rMoveOk = false;

        [MyDisplayName("测试位置类型"), MyCategory("参数")]
        public TestPositionType PositionType { get; set; } = TestPositionType.测试位置1;

        /// <summary>
        /// 复制对象成员
        /// </summary>
        protected override void CloneMembers(YungkuSystem.Script.Core.Base dest)
        {
            base.CloneMembers(dest);
            ActionTestPosition obj = dest as ActionTestPosition;
            obj.PositionType = this.PositionType;
        }

        protected override void Execute()
        {
            try
            {
                switch (StateIndex)
                {
                    case ACT_STATE_START:
                        #region    
                        ValidHardware();
                        Watcher.StopAllWatch();

                        if (MyApp.NeedReset || MyApp.ShareData.ishoming)
                        {
                            To(ACT_STATE_END);
                        }

                        if (!CurrentHeadObject.HasModudeState)
                        {
                            To(ACT_STATE_END);
                        }
                        else if (!HasPass && !MustExecute)
                        {
                            To(ACT_STATE_END);
                        }
                        else
                        {
                            switch (PositionType)
                            {
                                case TestPositionType.测试位置1:
                                    To("准备移动到测试位置1");
                                    break;
                                case TestPositionType.测试位置2:
                                    To("准备移动到测试位置2");
                                    break;
                                case TestPositionType.返回上料位:
                                    To("准备返回上料位置");
                                    break;
                            }
                        }
                        #endregion
                        break;

                    case ACT_STATE_END:
                        #region
                        string finishMsg = "";
                       
                        WriteInfo(finishMsg);
                        Finish();
                        #endregion
                        break;

                    default:
                        OnAlarm(G.Text("测试位置移动Action程序逻辑错误!"), true);
                        State = YungkuSystem.Script.Core.ActionState.Error;
                        To(ACT_STATE_END);
                        break;
                }
            }
            catch (Exception ex)
            {
                OnAlarm($"测试位置移动异常: {ex.Message}", true);
                To(ACT_STATE_END);
            }
        }
    }
}