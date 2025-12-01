using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yungku.BNU01_V1.Handler.Config.TestConfig;
using YungkuSystem;
using YungkuSystem.Motion.Manage;
using YungkuSystem.Structs;
using YungkuSystem.Tester;
using YungkuSystem.TestFlow;

namespace Yungku.BNU01_V1.Handler.Logic.Objects
{
    public  class ProductObject : MachineBase
    {

        /// <summary>
        /// 下相机产品拍照数据
        /// </summary>
        //public Vector2D InspectedDownVector = new Vector2D();
        /// <summary>
        /// 是否有模组
        /// </summary>
        public  bool HasModule = false;

        /// <summary>
        /// 子治具放料次数
        /// </summary>
        public int PlaceNumber = 0;
        /// <summary>
        /// 指示产品是否放料OK
        /// </summary>
        public bool PlaceProduceOk = false;

        private string codeString = string.Empty;
        /// <summary>
        /// 产品二维码
        /// </summary>
        public string CodeString
        {
            get { return codeString; }
            set { codeString = value; }
        }
        /// <summary>
        /// 配置文件
        /// </summary>
        public ProductConfig Config
        {
            get
            {
                if (this.BaseConfig == null)
                    return new ProductConfig();
                return this.BaseConfig as ProductConfig;
            }
        }

        /// <summary>
        /// 真空控制控制
        /// </summary>
        //public GPIOMap VacuumOutPut = null;
        //public GPIOMap VacuumInPut = null;

        private ProductObject(int index)
        {
            this.Index = index;
            this.Name = "Product" + index.ToString();
            if (MyApp.Config.TestSetting.ProductConfigs.Count <= Index)
            {
                MyApp.Config.TestSetting.ProductConfigs.Add(new ProductConfig());
            }
            this.BaseConfig = MyApp.Config.TestSetting.ProductConfigs[Index];
            //switch (index)
            //{
            //    case 0:
            //        //VacuumOutPut = MyApp.HW.AJigVacuum_Out;
            //        //VacuumInPut = MyApp.HW.AJigVacuumSensor_in;
            //        break;
            //    case 1:
            //        //VacuumOutPut = MyApp.HW.BJigVacuum_Out;
            //        //VacuumInPut = MyApp.HW.BJigVacuumSensor_in;
            //        break;
            //    case 2:
            //        //VacuumOutPut = MyApp.HW.CJigVacuum_Out;
            //        //VacuumInPut = MyApp.HW.CJigVacuumSensor_in;
            //        break;
            //    case 3:
            //        //VacuumOutPut = MyApp.HW.DJigVacuum_Out;
            //        //VacuumInPut = MyApp.HW.DJigVacuumSensor_in;
            //        break;
            //        //    case 4:
            //        //        VacuumOutPut = MyApp.HW.IO_Out_Jig_C1_VacuumOpen;
            //        //        VacuumInPut = MyApp.HW.IO_In_Jig_C1_Vacuum_Sensor;
            //        //        break;
            //        //    case 5:
            //        //        VacuumOutPut = MyApp.HW.IO_Out_Jig_C2_VacuumOpen;
            //        //        VacuumInPut = MyApp.HW.IO_In_Jig_C2_Vacuum_Sensor;
            //        //        break;
            //        //    case 6:
            //        //        VacuumOutPut = MyApp.HW.IO_Out_Jig_D1_VacuumOpen;
            //        //        VacuumInPut = MyApp.HW.IO_In_Jig_D1_Vacuum_Sensor;
            //        //        break;
            //        //    case 7:
            //        //        VacuumOutPut = MyApp.HW.IO_Out_Jig_D2_VacuumOpen;
            //        //        VacuumInPut = MyApp.HW.IO_In_Jig_D2_Vacuum_Sensor;
            //        //        break;
            //    }
            }
        /// <summary>
        /// 检查治具中是否有产品，并更新HasModule属性值
        /// </summary>
        /// <returns>指示治具中是否有产品</returns>
        //public bool CheckHasModule()
        //{

        //    bool preOut = GetVacuumOut();
        //    bool result = false;
        //    SetVacuum(true);//开启真空
        //    int startTick = System.Environment.TickCount;
        //    do
        //    {
        //        result = GetVacuumValue();
        //        Thread.Sleep(10);
        //        Application.DoEvents();
        //    } while (!result && (Math.Abs(System.Environment.TickCount - startTick) < 800));
        //    result = GetVacuumValue();
        //    SetVacuum(preOut);
        //    HasModule = result;
        //    return result;
        //}
        /// <summary>
        /// 设置真空状态
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        //public void SetVacuum(bool val)
        //{
        //    VacuumOutPut.Set(val);
        //}
        /// <summary>
        /// 获取一个值，该值只是真空值是否达标
        /// </summary>
        /// <returns></returns>
        //public bool GetVacuumValue()
        //{
        //    return VacuumInPut.GetValue();
        //}
        //public bool GetVacuumOut()
        //{
        //    return VacuumOutPut.GetValue();
        //}
        public IModuleGroup TestClient
        {
            get
            {
                IModuleGroup client =
                    MyApp.GetInstance().TestSystem.ModuleGroups.FindModuleGroup(this.Name);
                return client;
            }
        }



        /// <summary>
        /// 绑定自定义治具数据到治具对象中
        /// </summary>
        /// <param name="machine"></param>
        public static void BindToMachine(Machine machine)
        {
            int index = 0;
            foreach (Turntable tt in machine.TestItems)
            {
                foreach (Head head in tt.TestItems)
                {
                    foreach (Jig jig in head.TestItems)
                    {
                        foreach (Product p in jig.TestItems)
                        {
                            p.BindingObject = new ProductObject(index++);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有治具的名称
        /// </summary>
        /// <param name="machine"></param>
        public static List<string> GetNames(Machine machine)
        {
            List<string> names = new List<string>();
            foreach (Turntable tt in machine.TestItems)
            {
                foreach (Head head in tt.TestItems)
                {
                    foreach (YungkuSystem.TestFlow.Jig jig in head.TestItems)
                    {
                        foreach (Product product in jig.TestItems)
                        {
                            ProductObject p = product.BindingObject as ProductObject;
                            if (p != null)
                                names.Add(p.Name );
                        }
                    }
                }
            }
            return names;
        }
             
        public void SetProductResult(TestResult result, string errorCode)
        {
            Product pro = this.Owner as Product;
            pro.Result = result;
            pro.ResultCode = errorCode;
        }
    }
}
