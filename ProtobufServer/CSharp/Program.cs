using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Example;


namespace CSharp
{
    class Program
    {
        static TcpListener listener;
        static string ipAddress = "127.0.0.1";
        static int port = 8080;
        static List<ClientState> clients = new List<ClientState>();

        class ClientState
        {
            public MemoryStream readStream = new MemoryStream(1024 * 4);

            public MemoryStream sendStream = new MemoryStream(1024 * 4);

            public TcpClient client;
            public bool isRead;
        }

        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse(ipAddress);
            listener = new TcpListener(address, port);
            listener.Start();
            
            Console.WriteLine("Server ready");
            Console.WriteLine($"{address.ToString()}: {port}");
            Console.WriteLine("Wait run unity 'TestNet' scene connect...");
            AcceptTcpClientAsync();

            while (true)
            {
                for (int i = clients.Count - 1; i >= 0; i--)
                {
                    var state = clients[i];
                    if (!state.client.Connected)
                    {
                        clients.RemoveAt(i);
                        state.client.Dispose();
                    }

                    if (!state.isRead)
                    {
                        if (state.client.Available > 0)
                        {
                            ReadAsync(state);
                        }
                    }


                }
                System.Threading.Thread.Sleep(10);
            }

        }

        static void AcceptTcpClientAsync()
        {
            var acceptTask = listener.AcceptTcpClientAsync();
            acceptTask.ContinueWith(t =>
            {
                ClientState state = new ClientState()
                {
                    client = t.Result,
                };
                clients.Add(state);
                AcceptTcpClientAsync();
            });
        }
        static void ReadAsync(ClientState state)
        {
            byte[] buffer = state.readStream.GetBuffer();
            var stream = state.readStream;
            state.isRead = true;
            stream.SetLength(stream.Capacity);
            state.client.GetStream().BeginRead(buffer, (int)stream.Position, (int)(stream.Length - stream.Position), ReceiveAsync, state);
        }
        static void ReceiveAsync(IAsyncResult result)
        {
            var state = (ClientState)result.AsyncState;
            state.isRead = false;

            int readCount = state.client.GetStream().EndRead(result);
            state.readStream.Position = state.readStream.Position + readCount;


            int oldPos = (int)state.readStream.Position;
            state.readStream.Position = 0;
            var packet = DeserializePacket(state.readStream);
            int packedLength = (int)state.readStream.Position;

            int nextLength = (int)(oldPos - packedLength);
            if (nextLength > 0)
            {
                byte[] tmp = new byte[nextLength];
                state.readStream.Read(tmp, 0, nextLength);

                state.readStream.Position = 0;
                state.readStream.Write(tmp, 0, nextLength);
                state.readStream.Position = nextLength;
            }
            else
            {
                state.readStream.Position = 0;
            }

            switch (packet.Id)
            {
                case 10001:
                    var loginReq = LoginRequest.Parser.ParseFrom(packet.rawData);
                    Console.WriteLine($"LoginRequest  UserName: {loginReq.UserName}, UserPwd: {loginReq.UserPwd}");

                    using (MemoryStream ms2 = new MemoryStream())
                    using (var cos2 = new Google.Protobuf.CodedOutputStream(ms2, true))
                    {
                        loginReq.WriteTo(cos2);
                        cos2.Flush();
                        byte[] buffer = new byte[ms2.Length];
                        Array.Copy(ms2.GetBuffer(), 0, buffer, 0, ms2.Length);
                        var loginReq2 = LoginRequest.Parser.ParseFrom(buffer);

                        var loginReq3 = LoginRequest.Parser.ParseFrom(buffer);
                    }

                    LoginResponse loginRes = new LoginResponse()
                    {
                        ErrorCode = 1,
                        AccessToken = "xxx",
                    };
                    using (MemoryStream ms = new MemoryStream())
                    using (var cos = new Google.Protobuf.CodedOutputStream(ms,false))
                    {
                        loginRes.WriteTo(cos);
                        cos.Flush();
                        byte[] buffer = new byte[ms.Length];
                        Array.Copy(ms.GetBuffer(), 0, buffer, 0, ms.Length);
                        Send(state, new Packet()
                        {
                            Id = 10002,
                            rawData = buffer,
                        });
                    }
                    break;
            }

            if (state.client.Available > 0)
            {
                ReadAsync(state);
            }
        }
        private static readonly MemoryStream m_SerializeCachedStream = new MemoryStream(1024 * 8);
        public static bool useBigEndian = false;

        private static bool? reverseEndian;
        public static bool ReverseEndian
        {
            get
            {
                if (!reverseEndian.HasValue)
                {
                    reverseEndian = false;
                    if (useBigEndian)
                    {
                        if (BitConverter.IsLittleEndian)
                        {
                            reverseEndian = true;
                        }
                    }
                    else
                    {
                        if (!BitConverter.IsLittleEndian)
                        {
                            reverseEndian = true;
                        }
                    }
                }
                return reverseEndian.Value;
            }
        }

        static byte[] GetBytes(int n)
        {
            byte[] bytes = BitConverter.GetBytes(n);
            if (ReverseEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        static int ToInt32(byte[] buffer, int offset = 0)
        {
            if (ReverseEndian)
            {
                Array.Reverse(buffer, offset, 4);
            }
            return BitConverter.ToInt32(buffer, offset);
        }


        public static int SerializePacket(Packet packet, Stream destination)
        {
            m_SerializeCachedStream.SetLength(m_SerializeCachedStream.Capacity); // 此行防止 Array.Copy 的数据无法写入
            m_SerializeCachedStream.Position = 0L;

            //total length
            int headLength = 4;
            m_SerializeCachedStream.Position = headLength;

            m_SerializeCachedStream.Write(GetBytes(packet.Id), 0, 4);

            if (packet.rawData != null)
            {
                var bytes = packet.rawData;
                m_SerializeCachedStream.Write(bytes, 0, bytes.Length);
            }


            int bodyLength = (int)m_SerializeCachedStream.Position - headLength;
            m_SerializeCachedStream.Position = 0;
            m_SerializeCachedStream.Write(GetBytes(bodyLength), 0, 4);

            m_SerializeCachedStream.Position = 0;
            int totalLength = headLength + bodyLength;
            m_SerializeCachedStream.SetLength(totalLength);

            m_SerializeCachedStream.WriteTo(destination);

            return totalLength;
        }

        public static Packet DeserializePacket(Stream stream)
        {
            Packet packet = new Packet();
            byte[] buffer = new byte[8];
            //totalLength
            stream.Read(buffer, 0, 4);
            int bodyLength = ToInt32(buffer);
            byte[] bytes = new byte[bodyLength];
            int offset = 0;
            while (offset < bodyLength)
            {
                int n = stream.Read(bytes, offset, bodyLength - offset);
                if (n > 0)
                    offset += n;
            }

            //msgId
            packet.Id = ToInt32(bytes, 0);
            byte[] rawData = new byte[bodyLength - 4];
            Array.Copy(bytes, 4, rawData, 0, rawData.Length);

            packet.rawData = rawData;
            return packet;
        }

        static void Send(ClientState state, Packet packet)
        {
            state.sendStream.Position = 0;
            state.sendStream.SetLength(0);
            SerializePacket(packet, state.sendStream);

            var stream = state.client.GetStream();
            int length = (int)state.sendStream.Position;
            state.sendStream.Position = 0;
            state.sendStream.SetLength(length);
            state.sendStream.WriteTo(stream);
        }


    }


    public class Packet
    {
        public IPacketHeader header;
        public int Id;
        public object message;
        public byte[] rawData = null;
    }
    public interface IPacketHeader
    {
        int HeaderByteLength { get; set; }

        int PacketLength { get; set; }
    }
}
