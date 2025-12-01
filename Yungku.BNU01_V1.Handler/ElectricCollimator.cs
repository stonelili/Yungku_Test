using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Yungku.BNU01_V1.Handler
{
    //   平行光管软件控制的一般流程
    //     1.先后使用AutoConnect2和MX_AutoConnect尝试连接驱动(MX_AutoConnect连接驱动后还需通过MX_IniSlav获取地址数组。
    //     2.通过GetProductType获取光管型号并初始化光管地址和型号的Key-Value键值对(Demo中字典m_btAddr)。
    //     3. 通过Enable对光管使能(V系列光管需要此步骤)。 
    //     4.启动含驱动报警(通过GetErrorCode)和显示位置(GetActPos或GetActPos2)的监控线程。
    //     5.复位：S系列平行光管通过MX_Homing将光管复位到无穷远。
    //     V系列平行光管通过ClearError清除驱动报警-通过通过IsEnabled判断光管是否使能-使能则通过SoftLanding软着陆-通过IsInPosition判断软着陆到位-到位后通过FindIndex搜寻0点-通过IsInPosition判断软着陆是否到位-到位后通过MoveAbs将光管复位到无穷远。
    //     6.通过MoveAbsByFun将光管运动到想要的模拟距离。
    //     7.通过SetLedPwm设置V系列等老款光管的亮度
    //     8.通过MX_SetLedEx设置S系列等新款光管的色温和亮度。
    //     9.在结束使用光管时通过Enable释放光管和分别通过Disconnect、MX_Disconnect断开驱动连接

    class ElectricCollimator//电动平行光管配置函数
    {

        //函数功能：连接驱动器,会自动搜索COM端口号//Function: connect the drive, it will automatically search for the COM port number
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_AutoConnect(byte slaveAddr);
        //函数功能：连接S系列驱动器, 需手动指定COM端口号//Function: Connect the S-series driver, you need to manually specify the COM port number
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_Connect(string Com, byte slaveAddr);
        //函数功能：连接S系列驱动器, 需手动指定波特率//Function function: connect the S-series driver, need to manually specify the baud rate
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MXMU_AutoConnect(int baudrate, byte slaveAddr);
        //函数功能：断开接S系列驱动器, 会自动搜索COM端口号//Function: Disconnect the S-series driver, it will automatically search for the COM port number
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_Disconnect();
        //函数功能：清除可调色温产品的色温参数//Function: Clear the color temperature parameters of color temperature products
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void ClearCCT();
        //函数功能：获取接S系列地址//Function: Get the address of the S-series
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_IniSlave(byte nb, int[] connectAddr);
        //函数功能：获取产品型号//Function function: get product model
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool GetProductType(byte btAddr, [MarshalAs(UnmanagedType.LPStr)]StringBuilder type);
        //函数功能：连接V系列驱动器, 会自动搜索COM端口号（需要指定主机的波特率来通信）//Function: Connect the V-series driver, it will automatically search for the COM port number (the baud rate of the host needs to be specified to communicate)
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool AutoConnect2(uint ulBaudrate, byte btAddr);
        //函数功能：连接V系列驱动器, 需手动指定COM端口号，以及主机波特率//Function: To connect V-series drives, you need to manually specify the COM port number and the host baud rate
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool ManualConnect2(byte btCom, uint ulBaudrate, byte btAddr);
        //函数功能：断开V系列驱动器//Function: Disconnect V-series driver
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool Disconnect();
        //函数功能：获取编码器实际位置mm，Type为型号//Function: Get the actual position mm of the encoder, Type is the model
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool GetActPos(byte btAddr, ref float pnPos, string Type);
        //函数功能：获取模拟位置m，Type为型号//Function: Get the analog position m, Type is the model
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool GetActPos2(byte btAddr, ref float pnPos, string Type, int light = 0);
        //函数功能：S系列复位//S series reposition
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_Homing(byte btAddr);
        //函数功能：V系列驱动器是否连接//Function: whether the V-series drives is connected
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool IsConnected();
        //函数功能：判断V系列是否已经使能//Function: Determine whether the V-series has been enabled
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool IsEnabled(byte btAddr, ref bool bEnable);
        //函数功能：获取V系列(报警)故障代码//Function: Get V-Series (Alarm) Fault Codes
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool GetErrorCode(byte btAddr, ref ushort pusErrorCode);
        //函数功能：清除V系列(故障)报警//Function: Clear V-series(fault) alarm
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool ClearError(byte btAddr);
        //函数功能：使能或去使能V系列电机//Function: enable or disable the V-series motor
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool Enable(byte btAddr, bool bEnable);
        //函数功能：设置V系列伺服模式//Function: Set V series servo mode
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool SetServoMode(byte btAddr, byte ucMode);
        //函数功能：初始化V系列速度和加速度//Function: Initialize V series speed and acceleration
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool iniMotor(byte btAddr);
        //函数功能：设置绝对运动到指定的坐标//Function: Set absolute motion to the specified coordinates
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MoveAbs(byte btAddr, float mm, string Type);
        //函数功能：相对运动一段位移//Function: relative motion for a displacement
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MoveRel(byte btAddr, float mm, string Type);
        //函数功能：V系列运动到无穷远//Function: V series moves to infinity
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MoveInf(byte btAddr, string Type);
        //函数功能：S系列电机运动状态 status 0 - (零位不确定) 定位完成。1 - (零位不确定)定位中。4 - (零位不确定)过流或过热。8 - (零位不确定)软件限位或光电开关限位。2 - (零位确定)定位完成。3 - (零位确定)定位中。6 - (零位确定)过流或过热。10 - (零位确定)软件限位或光电开关限位。//Function: S series motor motion state
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool GetStatus(byte btAddr, ref ushort status, string Type);
        //函数功能：搜索V系列编码器的0相//Function function: search for 0 of V-series encoder
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool FindIndex(byte btAddr, ushort usVel, ushort usAcc, byte dir, bool bWaitDone);
        //函数功能：V系列软着陆//Function: V-series soft landing
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool SoftLanding(byte btAddr, ushort usVel, byte dir, ushort usCurrent, bool bWaitDone);
        //函数功能：判断V系列软是否运动到位//Function function: determine whether the V-series movement is in place
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool IsInPosition(byte btAddr, ref bool pbInPos);
        //函数功能：运动到模拟距离，fX为要运动到的模拟距离(单位mm)，Type为型号//Function: move to the simulation distance, fX is the simulation distance to be moved (unit mm), Type is the model
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MoveAbsByFun(byte btAddr, float fx, string Type, int light = 0);
        //函数功能：运动到模拟距离，fX为要运动到的模拟距离(单位m)，Type为型号//Function: move to the simulation distance, fX is the simulation distance to be moved (unit m), Type is the model
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MoveAbsByFun2M(byte btAddr, float fx, string Type, int light = 0);
        //函数功能：色温不可变系列的光源调节，Type为型号//Function function: light source adjustment for the series of invariable color temperature, Type is the model
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool SetLedPwm(byte btAddr, char Color, ushort pwm, string Type);
        //函数功能：色温可变系列设置色温和照度//Function: Color temperature variable series to set color temperature and illuminance
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int MX_SetLedEx(byte btAddr, int CCT, float Ex, ref float duv);
        //函数功能：快速切换光源
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_LedQuickSwitch(byte btAddr, char Color);
        //函数功能：获得新型光管亮度(从光敏电阻)，未正式上线
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_GetLedEx(byte btAddr, ref float lux);//
        //函数功能：关闭色温可变系列的色温和照度//Function: Turn off the color temperature and illuminance of the color temperature variable series
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_CloseLedEx(byte btAddr);
        //函数功能：读出光管与Z轴的夹角，单位：度
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_WitGetAngle(byte btAddr, ref float Angle);
        //函数功能：校准，把X轴的角度，Y轴的角度校准为零
        [DllImport("electricCollimator_C.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool MX_WitAccCali(byte btAddr);

        public static float DUV = 0.00f;
        public static StringBuilder TYPE = new StringBuilder();
        //public static float DUV
        //{
        //    get { return duv; }
        //    set { duv = value; }
        //}

        private static string productType = "";//产品类型名
        public static string ProductType
        {
            get { return productType; }
            //set { productType = value; }
        }

        private static bool collimatorState = false;
        public static bool CollimatorState
        {
            get { return collimatorState; }
            // set { collimatorState = value; }
        }

        private static int[] connAdd = new int[1];
        public static void CollimatorLink()
        {
            ClearCCT();
            collimatorState = true;//连接成功
            for (byte i = 1; i < 10; i++)//0,9
            {
                try
                {
                    //if (!MX_IniSlave(i, connAdd))
                    //{
                    //    MyApp.GetInstance().Logger.WriteError("[平行光管连接]:连接驱动失败 -平行光管" + i.ToString());
                    //    collimatorState = false;
                    //    return;
                    //}
                    if (!MX_AutoConnect(i))
                    {
                        MyApp.GetInstance().Logger.WriteError("[平行光管连接]:连接端口失败 -平行光管" + i.ToString());
                        collimatorState &= false;
                        return;
                    }
                    else
                    {
                        if (!GetProductType(i, TYPE))
                        {
                            MyApp.GetInstance().Logger.WriteError("[平行光管连接]:获取型号失败，连接驱动失败 -平行光管" + i.ToString());
                            collimatorState &= false;
                            return;
                        }
                        else
                        {
                            productType = TYPE.ToString();
                            #region 开启监控线程
                            //ConnectFlag = true;
                            //if (m_hThreadMonitor == null)
                            //{
                            //    m_hThreadMonitor = new Thread(ThreadStatusMonitor);
                            //    m_hThreadMonitor.Start();
                            //}
                            #endregion
                            collimatorState &= MX_Homing(i);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }


    }
}
