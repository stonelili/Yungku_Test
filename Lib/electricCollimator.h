#ifndef electricCollimator_H
#define electricCollimator_H

#define electricCollimator_C_API __declspec(dllexport)
#ifdef __cplusplus
extern "C"
{
#endif
	electricCollimator_C_API BOOL MX_Homing(BYTE btAddr);//函数功能：S系列复位
	electricCollimator_C_API BOOL MX_AutoConnect(BYTE btAddr);//函数功能：连接S系列驱动器, 会自动搜索COM端口号
	electricCollimator_C_API BOOL MXMU_AutoConnect(int baudrate, BYTE btAddr);//函数功能：连接S系列驱动器, 会自动搜索COM端口号
	electricCollimator_C_API BOOL MX_Connect(char* btCom, BYTE btAddr);//函数功能：连接S系列驱动器, 需手动指定COM端口号
	electricCollimator_C_API BOOL MXMU_Connect(char* btCom, int baudrate, BYTE btAddr);//函数功能：连接S系列驱动器, 需手动指定COM端口号，以及主机波特率
	electricCollimator_C_API BOOL MX_Disconnect(void);//函数功能：断开S系列驱动器, 会自动搜索COM端口号
	electricCollimator_C_API BOOL MX_IniSlave(BYTE nb, int *connectAddr);//函数功能：获取S系列地址
	electricCollimator_C_API int MX_SetLedEx(BYTE btAddr, int CCT, float Ex, float* duv);//函数功能：设置可调色温产品的色温和照度
	electricCollimator_C_API BOOL MX_LedQuickSwitch(BYTE btAddr, char Color);//函数功能：快速切换光源，Color为要要切的光源类型(Color: w表示白光,8表示红外850，9表示940)
	electricCollimator_C_API BOOL MX_CloseLedEx(BYTE btAddr);//函数功能：关闭可调色温产品光源
	electricCollimator_C_API void ClearCCT(void);//函数功能：清除可调色温产品的色温参数
	electricCollimator_C_API BOOL GetProductType(BYTE btAddr, char* type);//函数功能：获取产品型号
	electricCollimator_C_API BOOL AutoConnect2(unsigned int ulBaudrate, BYTE btAddr);//函数功能：连接V系列驱动器, 会自动搜索COM端口号（需要指定主机的波特率来通信）
	electricCollimator_C_API BOOL ManualConnect2(BYTE btCom, unsigned int ulBaudrate, BYTE btAddr);//函数功能：连接V系列驱动器, 需手动指定COM端口号，以及主机波特率
	electricCollimator_C_API BOOL IsConnected(void);//函数功能：检查V系列驱动器是否连接
	electricCollimator_C_API BOOL Disconnect(void);//函数功能：断开V系列驱动器连接
	electricCollimator_C_API BOOL IsEnabled(BYTE btAddr, BOOL* pbEnabled);//函数功能：判断V系列是否已经使能
	electricCollimator_C_API BOOL Enable(BYTE btAddr, BOOL bEnable);//函数功能：V系列的使能或去使能电机
	electricCollimator_C_API BOOL SetServoMode(BYTE btAddr, BYTE ucMode);//函数功能：设置V系列伺服模式
	electricCollimator_C_API BOOL iniMotor(BYTE btAddr);//函数功能：给V系列设置速度和加速度
	electricCollimator_C_API BOOL MoveRel(BYTE btAddr, float mm, char* Type);//函数功能：相对运动一段位移(机械位置，单位mm)
	electricCollimator_C_API BOOL MoveAbs(BYTE btAddr, float mm, char* Type);//函数功能：绝对运动到指定的坐标(机械位置，单位mm)
	electricCollimator_C_API BOOL MoveInf(BYTE btAddr, char* Type);//函数功能：V系列运动到模拟无穷远物距
	electricCollimator_C_API BOOL GetStatus(BYTE btAddr, USHORT *status, char* Type);//函数功能：获得S系列电机运动状态 获得平行光管的电机运动状态 关于状态位的获取请参照Demo，关于状态位的释义请看本文底部
	electricCollimator_C_API BOOL FindIndex(BYTE btAddr, USHORT usVel, USHORT usAcc, BYTE dir, BOOL bWaitDone);//函数功能：V系列搜索编码器的0相
	electricCollimator_C_API BOOL SoftLanding(BYTE btAddr, USHORT usVel, BYTE dir, USHORT usCurrent, BOOL bWaitDone);//函数功能：V系列软着陆
	electricCollimator_C_API BOOL GetActPos(BYTE btAddr, float *pnPos, char* Type);//函数功能：获取编码器实际位置mm，Type为型号
	electricCollimator_C_API BOOL GetActPos2(BYTE btAddr, float *pnPos, char* Type, int light=0);//函数功能：获取模拟位置m，Type为型号，light为所用光源的波长（可见光为0，850为1，940为2）
	electricCollimator_C_API BOOL GetErrorCode(BYTE btAddr, USHORT *pusErrorCode);//函数功能：获取V系列(报警)故障代码
	electricCollimator_C_API BOOL ClearError(BYTE btAddr);//函数功能：清除V系列(故障)报警
	electricCollimator_C_API BOOL IsInPosition(BYTE btAddr, BOOL *pbInPos);//函数功能：判断V系列是否运动到位
	electricCollimator_C_API BOOL MoveAbsByFun(BYTE btAddr, FLOAT fx, char* Type, int light = 0);//函数功能：绝对运动到模拟距离，fX为要运动到的模拟距离(单位mm)，Type为型号(如果是可变工作距型号需要在型号后加下划线工作距出瞳,如PL120STS_8015)，light为所用光源的波长（可见光为0，850为1，940为2）
	electricCollimator_C_API BOOL MoveAbsByFun2M(BYTE btAddr, FLOAT fx, char* Type, int light = 0);//函数功能：绝对运动到模拟距离，fX为要运动到的模拟距离(单位m)，Type为型号(如果是可变工作距型号需要在型号后加下划线工作距出瞳,如PL120STS_8015)，light为所用光源的波长（可见光为0，850为1，940为2）
	electricCollimator_C_API BOOL SetLedPwm(BYTE btAddr, char Color, USHORT pwm, char* Type);//函数功能：光源调节，Type为型号
	electricCollimator_C_API BOOL MX_WitGetAngle(BYTE btAddr, float* Angle);//函数功能:可变工作距系列读出光管与Z轴的夹角(Angle)，单位：度
        electricCollimator_C_API BOOL MX_LedQuickSwitch(BYTE btAddr, char Color);//函数功能：快速切换光源，可传入地址0进行广播群发，Color为要要切的光源类型，Color传入'w'表示快切到白光，Color传入'8'表示快切到850波段红外光，Color传入'9'表示快切到940波段红外光
	//输入参数：btAddr为驱动器地址，bWaitDone为TRUE时，等待到位后，函数才返回;bWaitDone为FALSE时, 不等待到位，函数立即返回
	//返回值：  为TRUE时，函数调用成功
#ifdef __cplusplus
}
#endif
#endif

//状态字 第0位 0：定位完成（电机运行结束） 1：定位中（电机运行中）；
//第1位 0：零位不确定（未触碰光电开关） 1：零位确定（已触碰光电开关）；
//第2位 0：电机未过流/过热 1：电机过流或过热；
//第3位 0：不在限位位置 1：在限位位置；
//第4位 0：姿态模块正常 1：姿态模块异常；
//第5位 0：无编码器 1：有编码器；
//第6位 0：未发生软限位（正常状态） 1：软限位（超过电机行程）
//第7位 暂未定义
//建议对状态字的第2位和第3位做判断当为1时警示(Alarm)给操作员（因为限位位置和电机过流或过热会导致模拟位置不准确）