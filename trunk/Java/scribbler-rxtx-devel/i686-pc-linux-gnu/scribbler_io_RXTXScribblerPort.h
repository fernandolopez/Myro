/* DO NOT EDIT THIS FILE - it is machine generated */
#include <jni.h>
/* Header for class scribbler_io_RXTXScribblerPort */

#ifndef _Included_scribbler_io_RXTXScribblerPort
#define _Included_scribbler_io_RXTXScribblerPort
#ifdef __cplusplus
extern "C" {
#endif
#undef scribbler_io_RXTXScribblerPort_DATABITS_5
#define scribbler_io_RXTXScribblerPort_DATABITS_5 5L
#undef scribbler_io_RXTXScribblerPort_DATABITS_6
#define scribbler_io_RXTXScribblerPort_DATABITS_6 6L
#undef scribbler_io_RXTXScribblerPort_DATABITS_7
#define scribbler_io_RXTXScribblerPort_DATABITS_7 7L
#undef scribbler_io_RXTXScribblerPort_DATABITS_8
#define scribbler_io_RXTXScribblerPort_DATABITS_8 8L
#undef scribbler_io_RXTXScribblerPort_PARITY_NONE
#define scribbler_io_RXTXScribblerPort_PARITY_NONE 0L
#undef scribbler_io_RXTXScribblerPort_PARITY_ODD
#define scribbler_io_RXTXScribblerPort_PARITY_ODD 1L
#undef scribbler_io_RXTXScribblerPort_PARITY_EVEN
#define scribbler_io_RXTXScribblerPort_PARITY_EVEN 2L
#undef scribbler_io_RXTXScribblerPort_PARITY_MARK
#define scribbler_io_RXTXScribblerPort_PARITY_MARK 3L
#undef scribbler_io_RXTXScribblerPort_PARITY_SPACE
#define scribbler_io_RXTXScribblerPort_PARITY_SPACE 4L
#undef scribbler_io_RXTXScribblerPort_STOPBITS_1
#define scribbler_io_RXTXScribblerPort_STOPBITS_1 1L
#undef scribbler_io_RXTXScribblerPort_STOPBITS_2
#define scribbler_io_RXTXScribblerPort_STOPBITS_2 2L
#undef scribbler_io_RXTXScribblerPort_STOPBITS_1_5
#define scribbler_io_RXTXScribblerPort_STOPBITS_1_5 3L
#undef scribbler_io_RXTXScribblerPort_FLOWCONTROL_NONE
#define scribbler_io_RXTXScribblerPort_FLOWCONTROL_NONE 0L
#undef scribbler_io_RXTXScribblerPort_FLOWCONTROL_RTSCTS_IN
#define scribbler_io_RXTXScribblerPort_FLOWCONTROL_RTSCTS_IN 1L
#undef scribbler_io_RXTXScribblerPort_FLOWCONTROL_RTSCTS_OUT
#define scribbler_io_RXTXScribblerPort_FLOWCONTROL_RTSCTS_OUT 2L
#undef scribbler_io_RXTXScribblerPort_FLOWCONTROL_XONXOFF_IN
#define scribbler_io_RXTXScribblerPort_FLOWCONTROL_XONXOFF_IN 4L
#undef scribbler_io_RXTXScribblerPort_FLOWCONTROL_XONXOFF_OUT
#define scribbler_io_RXTXScribblerPort_FLOWCONTROL_XONXOFF_OUT 8L
#undef scribbler_io_RXTXScribblerPort_debug
#define scribbler_io_RXTXScribblerPort_debug 0L
#undef scribbler_io_RXTXScribblerPort_debug_read
#define scribbler_io_RXTXScribblerPort_debug_read 0L
#undef scribbler_io_RXTXScribblerPort_debug_read_results
#define scribbler_io_RXTXScribblerPort_debug_read_results 0L
#undef scribbler_io_RXTXScribblerPort_debug_write
#define scribbler_io_RXTXScribblerPort_debug_write 0L
#undef scribbler_io_RXTXScribblerPort_debug_events
#define scribbler_io_RXTXScribblerPort_debug_events 0L
#undef scribbler_io_RXTXScribblerPort_debug_verbose
#define scribbler_io_RXTXScribblerPort_debug_verbose 0L
/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    Initialize
 * Signature: ()V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_Initialize
  (JNIEnv *, jclass);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    open
 * Signature: (Ljava/lang/String;)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_open
  (JNIEnv *, jobject, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetParity
 * Signature: (I)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetParity
  (JNIEnv *, jobject, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetFlowControlMode
 * Signature: (I)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetFlowControlMode
  (JNIEnv *, jobject, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetSerialPortParams
 * Signature: (IIII)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetSerialPortParams
  (JNIEnv *, jobject, jint, jint, jint, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    setflowcontrol
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_setflowcontrol
  (JNIEnv *, jobject, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    NativegetReceiveTimeout
 * Signature: ()I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_NativegetReceiveTimeout
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    NativeisReceiveTimeoutEnabled
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_NativeisReceiveTimeoutEnabled
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    NativeEnableReceiveTimeoutThreshold
 * Signature: (III)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_NativeEnableReceiveTimeoutThreshold
  (JNIEnv *, jobject, jint, jint, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    isDTR
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_isDTR
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    setDTR
 * Signature: (Z)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_setDTR
  (JNIEnv *, jobject, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    setRTS
 * Signature: (Z)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_setRTS
  (JNIEnv *, jobject, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    setDSR
 * Signature: (Z)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_setDSR
  (JNIEnv *, jobject, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    isCTS
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_isCTS
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    isDSR
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_isDSR
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    isCD
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_isCD
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    isRI
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_isRI
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    isRTS
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_isRTS
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    sendBreak
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_sendBreak
  (JNIEnv *, jobject, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    writeByte
 * Signature: (IZ)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_writeByte
  (JNIEnv *, jobject, jint, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    writeArray
 * Signature: ([BIIZ)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_writeArray
  (JNIEnv *, jobject, jbyteArray, jint, jint, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeDrain
 * Signature: (Z)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeDrain
  (JNIEnv *, jobject, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeavailable
 * Signature: ()I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeavailable
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    readByte
 * Signature: ()I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_readByte
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    readArray
 * Signature: ([BII)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_readArray
  (JNIEnv *, jobject, jbyteArray, jint, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    readTerminatedArray
 * Signature: ([BII[B)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_readTerminatedArray
  (JNIEnv *, jobject, jbyteArray, jint, jint, jbyteArray);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    eventLoop
 * Signature: ()V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_eventLoop
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    interruptEventLoop
 * Signature: ()V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_interruptEventLoop
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetEventFlag
 * Signature: (IIZ)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetEventFlag
  (JNIEnv *, jobject, jint, jint, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeClose
 * Signature: (Ljava/lang/String;)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_nativeClose
  (JNIEnv *, jobject, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticSetSerialPortParams
 * Signature: (Ljava/lang/String;IIII)V
 */
JNIEXPORT void JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticSetSerialPortParams
  (JNIEnv *, jclass, jstring, jint, jint, jint, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticSetDSR
 * Signature: (Ljava/lang/String;Z)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticSetDSR
  (JNIEnv *, jclass, jstring, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticSetDTR
 * Signature: (Ljava/lang/String;Z)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticSetDTR
  (JNIEnv *, jclass, jstring, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticSetRTS
 * Signature: (Ljava/lang/String;Z)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticSetRTS
  (JNIEnv *, jclass, jstring, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticIsDSR
 * Signature: (Ljava/lang/String;)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticIsDSR
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticIsDTR
 * Signature: (Ljava/lang/String;)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticIsDTR
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticIsRTS
 * Signature: (Ljava/lang/String;)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticIsRTS
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticIsCTS
 * Signature: (Ljava/lang/String;)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticIsCTS
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticIsCD
 * Signature: (Ljava/lang/String;)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticIsCD
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticIsRI
 * Signature: (Ljava/lang/String;)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticIsRI
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticGetBaudRate
 * Signature: (Ljava/lang/String;)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticGetBaudRate
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticGetDataBits
 * Signature: (Ljava/lang/String;)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticGetDataBits
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticGetParity
 * Signature: (Ljava/lang/String;)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticGetParity
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeStaticGetStopBits
 * Signature: (Ljava/lang/String;)I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeStaticGetStopBits
  (JNIEnv *, jclass, jstring);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetParityErrorChar
 * Signature: ()B
 */
JNIEXPORT jbyte JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetParityErrorChar
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetParityErrorChar
 * Signature: (B)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetParityErrorChar
  (JNIEnv *, jobject, jbyte);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetEndOfInputChar
 * Signature: ()B
 */
JNIEXPORT jbyte JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetEndOfInputChar
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetEndOfInputChar
 * Signature: (B)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetEndOfInputChar
  (JNIEnv *, jobject, jbyte);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetUartType
 * Signature: (Ljava/lang/String;Z)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetUartType
  (JNIEnv *, jobject, jstring, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetUartType
 * Signature: ()Ljava/lang/String;
 */
JNIEXPORT jstring JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetUartType
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetBaudBase
 * Signature: (I)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetBaudBase
  (JNIEnv *, jobject, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetBaudBase
 * Signature: ()I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetBaudBase
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetDivisor
 * Signature: (I)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetDivisor
  (JNIEnv *, jobject, jint);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetDivisor
 * Signature: ()I
 */
JNIEXPORT jint JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetDivisor
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetLowLatency
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetLowLatency
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetLowLatency
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetLowLatency
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeSetCallOutHangup
 * Signature: (Z)Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeSetCallOutHangup
  (JNIEnv *, jobject, jboolean);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeGetCallOutHangup
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeGetCallOutHangup
  (JNIEnv *, jobject);

/*
 * Class:     scribbler_io_RXTXScribblerPort
 * Method:    nativeClearCommInput
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_scribbler_io_RXTXScribblerPort_nativeClearCommInput
  (JNIEnv *, jobject);

#ifdef __cplusplus
}
#endif
#endif