using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Medimind.Scripts
{
 
    public class NamedPipeBase 
    {
        public enum Type
        {
            Exit = 0xF1,//241
            ForceExit = 0xF2,
            /// <summary>
            /// 프로그램 다시 하기
            /// </summary>
            Retry = 0xF3,
            /// <summary>
            /// 콘텐츠 다시 선택
            /// </summary>
            Reselect = 0xF4,
            /// <summary>
            /// 장비 사용법
            /// </summary>
            HowToUse = 0xF5,
        }

        public bool IsAlive { get { return isAlive; } }

        private bool isAlive = false;

        /// <summary>
        /// 서버측 스레드 생성및 작동 함수
        /// </summary>
        public virtual void Running()
        {
            isAlive = true;
        }

        public virtual void Stop()
        {
            isAlive = false;
        }
    }
    /// <summary>
    /// 정보 수신 전용 클래스
    /// </summary>
    [Serializable]
    public class NamedPipeResponser : NamedPipeBase, IDisposable
    {
        private NamedPipeServerStream pipe;

        private Thread thread;

        private bool isbreaking = false;

#if !MAINAPP
        /// <summary>
        /// 수신된 정보들
        /// </summary>
        public Queue<byte[]> DataQueue { get { return dataQueue; } }
        private Queue<byte[]> dataQueue = new Queue<byte[]>();
#endif
        /// <summary>
        /// 실행
        /// </summary>
        public override void Running()
        {
#if MAINAPP
            QueuePool.log.Add($"Winform NamedPipe Running => {Infomation.PIPE_BASE}");
#else
            DebugWriter.Log("Unity NamedPipe Running.");
#endif
            base.Running();

            thread = new Thread(Response);
            thread.IsBackground = true;
            thread.Start();

            isbreaking = false;

            //Thread.Sleep(100);
        }
        public void Disconnect()
        {
            try
            {
                if (pipe != null && pipe.IsConnected)
                    pipe.Disconnect();
            }
            catch (Win32Exception w32e)
            {
#if MAINAPP
                QueuePool.log.Add($"[Exception] " + w32e.ToString());
#else
                DebugWriter.Error("Exception: " + w32e.ToString(), w32e.StackTrace);
#endif
            }
        }

        /// <summary>
        /// 정지
        /// </summary>
        public override void Stop()
        {
#if MAINAPP
            QueuePool.log.Add("NamedPipe Responser Try Stop");
#else
            DebugWriter.Log("NamedPipe Responser Try Stop");
#endif
            base.Stop();

            isbreaking = true;

            if (pipe != null)
            {
                NamedPipeClientStream npcs = new NamedPipeClientStream(Infomation.PIPE_RESPONSER);
                npcs.Connect();

                Dispose();
            }

            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread.Join();
            }

#if MAINAPP
            QueuePool.log.Add($"NamedPipe Responser Stop");
#else
            DebugWriter.Log("NamedPipe Responser Stop");
#endif
        }
        /// <summary>
        /// 수신 결과 처리
        /// dispose와 close를 하지 않는다.
        /// client에서 request를 하면서 연결해제를 하므로 할 필요가 없다.
        /// </summary>
        /// <param name="_data"></param>
        protected void Response(object _data)
        {
            while (IsAlive)
            {
#if MAINAPP
                QueuePool.log.Add("Response Wait");
#else
                DebugWriter.Log("Response Wait.");
#endif

                try
                {
#if MAINAPP
                    pipe = new NamedPipeServerStream(Infomation.PIPE_RESPONSER);
#else
                    pipe = new NamedPipeServerStream(Database.PIPE_RESPONSER);
#endif
                    if (isbreaking) return;

                    try
                    {
                        pipe.WaitForConnection();
                    }
                    catch (IOException ioe)
                    {
                        if (!isbreaking)
                        {
                            QueuePool.log.Add("[IOException] Pipe 연결이 중지 되었습니다.\n" + ioe.ToString());
                            return;
                        }
                    }

                    if (isbreaking) return;

                    if (pipe.IsConnected && !isbreaking)
                    {
                        byte[] read = StreamAssistent.Read(pipe);

                        if (read.Length != 0)
                        {
#if MAINAPP
                            QueuePool.date.Add(read);
                            QueuePool.log.Add("Read: " + ((Type)read[0]).ToString());
#else
                            dataQueue.Enqueue(read);
#endif
                        }

#if MAINAPP

#else
                        DebugWriter.Log("Response Success.");
#endif
                    }
                }
                catch (Exception e)
                {
#if MAINAPP
                    QueuePool.log.Add("[IOException] " + e.ToString());
#else
                    DebugWriter.Error(e.Message, e.StackTrace);
#endif
                }
                finally
                {
#if MAINAPP
                    QueuePool.log.Add("Response End Read");
#else
                    DebugWriter.Log("Response Read Ended.");
#endif
                }
            }
        }
        public void Dispose()
        {
            if (pipe.IsConnected)
                pipe.Disconnect();
            pipe.Dispose();
            pipe.Close();
        }
    }
    /// <summary>
    /// 정보 전달 전용 클래스
    /// </summary>
    [Serializable]
    public class NamedPipeRequester : NamedPipeBase, IDisposable
    {
        protected NamedPipeClientStream pipe;

        public override void Stop()
        {
            if (pipe != null && pipe.IsConnected)
            {
                pipe.Dispose();
                pipe.Close();
                pipe = null;
            }

            base.Stop();
        }
        /// <summary>
        /// 데이터 전송
        /// </summary>
        /// <param name="_data"></param>
        public void Request(params byte[] _data)
        {
#if MAINAPP
            pipe = new NamedPipeClientStream(Infomation.PIPE_REQUESTER);
#else
            pipe = new NamedPipeClientStream(Database.PIPE_REQUESTER);
#endif

            if (pipe.IsConnected == false)
                pipe.Connect();

            if (pipe.IsConnected)
            {
                StreamAssistent.Write(pipe, _data);

                try
                {
                    pipe.WaitForPipeDrain();

                    pipe.Dispose();
                    pipe.Close();
                    pipe = null;
                }
                catch (Exception ex)
                {
#if MAINAPP
                    QueuePool.log.Add($"[Exception] " + ex.ToString());
#else
                    DebugWriter.Error(ex.Message, ex.StackTrace);
#endif
                }
                finally
                {
#if MAINAPP
                    QueuePool.log.Add($"Request Data[" + Infomation.PIPE_REQUESTER + "]: [" + _data.Length + "]" + Utility.ToString(_data));
#else
                    DebugWriter.Log("Request Done.");
#endif
                }
            }
            else
            {
#if MAINAPP
                QueuePool.log.Add("PipeLine[" + Infomation.PIPE_REQUESTER + "] Connection Fail");
#else
                DebugWriter.Error("PipeLine Connection Fail");
#endif
            }
        }
        public void Dispose()
        {
            pipe.Dispose();
            pipe.Close();
        }
    }
    /// <summary>
    /// 정보 처리 하는 클래스
    /// </summary>
    public class StreamAssistent
    {
        public static byte[] Read(Stream ioStream)
        {
            using (ioStream)
            {
                int len = ioStream.ReadByte() * 256;
                len += ioStream.ReadByte();
                byte[] inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                return inBuffer;
            }
        }

        public static void Write(Stream ioStream, params byte[] data)
        {
            using (ioStream)
            {
                ioStream.WriteByte((byte)(data.Length / 256));
                ioStream.WriteByte((byte)(data.Length & 255));
                ioStream.Write(data, 0, data.Length);

                ioStream.Flush();
            }
        }
    }
}
