#region using

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Eternal.Utils;

#endregion

namespace Eternal.Net {
    public class SocketBase {
        private static   int    _totalbytesEchoed;
        private readonly int    session = 0;
        private readonly int    _buffersize;
        private          int    _bytesRcvd;
        //private          byte[] _rcvBuffer;
        private          byte[] rcvBuffer;

        public SocketBase(int buffersize, int port, IPAddress ip, bool isServer = false, bool startinstadn = false) {
            this._buffersize = buffersize;
            this.rcvBuffer   = new byte[this._buffersize];

            Port      = port;
            IpAddress = ip;

            MainSocked = new SocketContext( null, "SERVER" );
            if ( !startinstadn ) return;

            if ( isServer )
                Server( port );
            else
                Connect( ip, port );
        }

        public bool          Connectet  { get; internal set; }
        public SocketContext MainSocked { get; }


        public IPEndPoint EndPoint  { get; private set; }
        public IPAddress  IpAddress { get; }
        public int        Port      { get; }

        public int                 TotalBytesEchoed => _totalbytesEchoed;
        public List<SocketContext> ClientSockets    { get; } = new List<SocketContext>();

        //client
        public void Connect() => Connect( IpAddress, Port );

        private void Connect(IPAddress ip, int port) {
            MainSocked.PSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

            if ( ip == null ) ip = IPAddress.Any;

            EndPoint = new IPEndPoint( ip, port );

            string fail = null;

            try {
                MainSocked.PSocket.Connect( EndPoint );
            } catch (Exception e) {
                fail = e.Message;
            }

            if ( MainSocked.PSocket.Connected ) {
                Console.WriteLine( " + Connection established!\n" );
                Connectet = true;
            } //connected if
            else {
                Console.WriteLine( " - Connection failed! [{0}].", fail );
                Connectet = false;
                Thread.Sleep( 2500 );
            }

            ClientLoop();
        }

        private void ClientLoop() {
            try {
                while ( ( this._bytesRcvd = MainSocked.PSocket.Receive( this.rcvBuffer, 0, this.rcvBuffer.Length, SocketFlags.None ) ) > 0 ) {
                    var context = MainSocked.ProgressReceive( this.rcvBuffer, this._bytesRcvd );

                    this.OnMessageReceived?.Invoke( context );

                    this.rcvBuffer = new byte[this.rcvBuffer.Length];

                    _totalbytesEchoed += this._bytesRcvd;
                }
            } catch (Exception e) {
                Console.WriteLine( e.Message );
            }
        }

        //server part
        public void Server() => Server( Port );

        private void Server(int Port) {
            MainSocked.PSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

            EndPoint = new IPEndPoint( IPAddress.Any, Port );

            string fail = null;

            try {
                MainSocked.PSocket.Bind( EndPoint );
            } catch (Exception e) {
                fail = e.Message;
            }

            MainSocked.PSocket.Listen( 1000 );
            Console.WriteLine( "Waiting for Connection..." );

            while ( true ) {
                Socket client = null;

                client = MainSocked.PSocket.Accept();

                var Sockclient = new SocketContext( null, null, null, null, client );

                var Accept = new Thread( () => HandleClients( Sockclient ) );
                Accept.Start();
            }
        }

        private void HandleClients(SocketContext context) {
            var prcvBuffer = new byte[this._buffersize];

            try {
                ClientSockets.Add( context );

                Console.Write( " + " + context.PSocket.RemoteEndPoint + " -> Handling Sockclient " );
                Console.Write( $"[{this.session}]\n" );

                context.Send( context, $"Name={context.PLocalName}" );

                //Console.WriteLine(ProgressResive(Encoding.Unicode.GetBytes(h), Encoding.Unicode.GetBytes(h).Length,out var sender));

                int pbytesRcvd;
                while ( ( pbytesRcvd = context.PSocket.Receive( prcvBuffer, 0, prcvBuffer.Length, SocketFlags.None ) ) > 0 ) {
                    var messageContext = context.ProgressReceive( prcvBuffer, pbytesRcvd );

                    Console.WriteLine( $"[Client:{messageContext.PReceiver}]: " + messageContext.PMessage );

                    _totalbytesEchoed += pbytesRcvd;

                    this.OnMessageReceived?.Invoke( messageContext );

                    prcvBuffer = new byte[prcvBuffer.Length];
                }

                context.PSocket.Shutdown( SocketShutdown.Both );
                context.PSocket.Close();
            } catch {
                //
            }
        }

        public event Action<SocketContext> OnMessageReceived;
    }


    public class SocketContext {
        public SocketContext(string pLocalName, string pSender = null, string pReceiver = null, string pMessage = null, Socket pSocket = null) {
            PSender    = pSender    ?? new Random().Next( 100000000, 999999999 ).ToString();
            PReceiver  = pReceiver  ?? new Random().Next( 100000000, 999999999 ).ToString();
            PLocalName = pLocalName ?? new Random().Next( 100000000, 999999999 ).ToString();
            PMessage   = pMessage;
            PSocket    = pSocket ?? new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
        }

        public string PSender    { get; private set; }
        public string PReceiver  { get; private set; }
        public string PLocalName { get; private set; }
        public string PMessage   { get; private set; }
        public Socket PSocket    { get; internal set; }

        public bool ForceName(string name) {
            if ( name.Length != 9 ) return false;
            PLocalName = name;
            return true;
        }

        //Enc/Dec send/reseve
        public void Send(SocketContext sendSocket, string data) {
            if ( PSender == null || PReceiver == null || data == null || PSender.Length != 9 ) return;
            if ( PReceiver == "" ) PReceiver = "UNKNOWN";

            var fullSendDate = StringList.MakeList( new[] { PSender, PReceiver, data } );

            sendSocket.PSocket.Send( Encoding.Unicode.GetBytes( fullSendDate ) );
        }

        public void Send(IEnumerable<SocketContext> sendSockets, string data) {
            foreach ( var sock in sendSockets ) Send( sock, data );
        }

        public SocketContext ProgressReceive(byte[] rcvBuffer, int bytesRcvd) {
            var encode = Encoding.Unicode.GetString( rcvBuffer, 0, bytesRcvd );

            var list = StringList.UnMakeList( encode, out var length, false );
            if ( length == encode.Length ) Console.WriteLine( "Length" );

            return new SocketContext( list[1], list[0], list[1], list[2], PSocket );
        }
    }
}