```csharp
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

// 自定义消息类
public class MyMessage
{
    public string MessageType { get; set; } // 消息类型
    public byte[] Data { get; set; } // 数据
}

// 编码器
public class MyEncoder : MessageToByteEncoder<MyMessage>
{
    protected override void Encode(IChannelHandlerContext context, MyMessage message, IByteBuffer output)
    {
        // 将消息类型和数据写入缓冲区
        output.WriteString(message.MessageType, Encoding.UTF8);
        output.WriteBytes(message.Data);
    }
}

// 解码器
public class MyDecoder : ByteToMessageDecoder
{
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        // 读取消息类型和数据
        string messageType = input.ReadString(Encoding.UTF8);
        byte[] data = new byte[input.ReadableBytes];
        input.ReadBytes(data);

        // 创建消息对象并添加到输出列表中
        MyMessage message = new MyMessage { MessageType = messageType, Data = data };
        output.Add(message);
    }
}

// 服务器处理器
public class MyServerHandler : SimpleChannelInboundHandler<MyMessage>
{
    protected override void ChannelRead0(IChannelHandlerContext context, MyMessage message)
    {
        // 判断消息类型，并根据不同的类型进行处理
        if (message.MessageType == "text")
        {
            string text = Encoding.UTF8.GetString(message.Data);
            Console.WriteLine($"Received text message: {text}");
        }
        else if (message.MessageType == "image")
        {
            Image image = Image.FromStream(new MemoryStream(message.Data));
            Console.WriteLine($"Received image message: {image.Width} x {image.Height}");
        }
    }
}

// 客户端处理器
public class MyClientHandler : SimpleChannelInboundHandler<MyMessage>
{
    private MyMessage _messageToSend;

    public MyClientHandler(MyMessage messageToSend)
    {
        _messageToSend = messageToSend;
    }

    protected override void ChannelActive(IChannelHandlerContext context)
    {
        // 发送消息
        context.WriteAndFlushAsync(_messageToSend);
    }

    protected override void ChannelRead0(IChannelHandlerContext context, MyMessage message)
    {
        // 判断消息类型，并根据不同的类型进行处理
        if (message.MessageType == "text")
        {
            string text = Encoding.UTF8.GetString(message.Data);
            Console.WriteLine($"Received text message: {text}");
        }
        else if (message.MessageType == "image")
        {
            Image image = Image.FromStream(new MemoryStream(message.Data));
            Console.WriteLine($"Received image message: {image.Width} x {image.Height}");
        }
    }
}

// 服务器
public class MyServer
{
    public void Start()
    {
        var bossGroup = new MultithreadEventLoopGroup(1);
        var workerGroup = new MultithreadEventLoopGroup();

        try
        {
            var bootstrap = new ServerBootstrap()
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new MyEncoder(), new MyDecoder(), new MyServerHandler());
                }));

            IChannel boundChannel = bootstrap.BindAsync(IPAddress.Any, 8888).Result;
            Console.WriteLine($"Server started on {boundChannel.LocalAddress}");
            Console.ReadLine();
        }
        finally
        {
            bossGroup.ShutdownGracefullyAsync().Wait();
            workerGroup.ShutdownGracefullyAsync().Wait();
        }
    }
}

// 客户端
public class MyClient
{
    public void Start()
    {
        var group = new MultithreadEventLoopGroup();

        try
        {
            MyMessage messageToSend = new MyMessage();
            messageToSend.MessageType = "image";
            messageToSend.Data = File.ReadAllBytes("test.jpg");

            var bootstrap = new Bootstrap()
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new MyEncoder(), new MyDecoder(), new MyClientHandler(messageToSend));
                }));

            IChannel boundChannel = bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 8888)).Result;
            Console.WriteLine($"Connected to {boundChannel.RemoteAddress}");

            Console.ReadLine();
        }
        finally
        {
            group.ShutdownGracefullyAsync().Wait();
        }
    }
}

// 测试代码
public class Test
{
    public static void Main()
    {
        // 启动服务器
        var server = new MyServer();
        server.Start();

        // 启动客户端
        var client = new MyClient();
        client.Start();
    }
}


这段代码定义了一个自定义的消息类 MyMessage，它包含了消息类型和数据。同时，还定义了编码器 MyEncoder 和解码器 MyDecoder，用于将消息对象转换为字节流和将字节流转换为消息对象。在服务器处理器 MyServerHandler 和客户端处理器 MyClientHandler 中，根据消息类型进行不同的处理，如果是文本消息，则将字节数组转换为字符串；如果是图片消息，则将字节数组转换为图片对象。在测试代码中，启动了一个服务器和一个客户端，并向服务器发送了一张图片。
```





```csharp

以下是使用 DotNetty 发送和接收图片数据的示例代码：
    
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class ImageServerHandler : ChannelHandlerAdapter
{
    private byte[] imageBytes;

    public override void ChannelActive(IChannelHandlerContext context)
    {
        // 读取图片数据
        using (var image = Image.FromFile("image.jpg"))
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                imageBytes = ms.ToArray();
            }
        }

        // 将图片数据写入 Channel
        var buffer = Unpooled.Buffer(imageBytes.Length);
        buffer.WriteBytes(imageBytes);
        context.WriteAndFlushAsync(buffer);
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        // 接收到图片数据
        var buffer = (IByteBuffer)message;
        var receivedBytes = new byte[buffer.ReadableBytes];
        buffer.ReadBytes(receivedBytes);

        // 将接收到的数据转换为图片
        using (var ms = new MemoryStream(receivedBytes))
        {
            using (var image = Image.FromStream(ms))
            {
                // 在控制台输出图片信息
                Console.WriteLine($"Received image: {image.Width} x {image.Height}, format: {image.RawFormat}");
            }
        }
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        // 异常处理
        Console.WriteLine($"Exception: {exception}");
        context.CloseAsync();
    }
}

```



```csharp
下面是一个使用 DotNetty 发送和接收 Socket 数据的简单示例代码：

using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var group = new MultithreadEventLoopGroup();
        try
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new StringEncoder(Encoding.UTF8), new StringDecoder(Encoding.UTF8), new SocketClientHandler());
                }));

            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            var channel = await bootstrap.ConnectAsync(endPoint);
            await channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(Encoding.UTF8.GetBytes("Hello, DotNetty!")));
            Console.WriteLine("Sent: Hello, DotNetty!");

            Console.ReadLine();
        }
        finally
        {
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }
}

class SocketClientHandler : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        var buffer = message as IByteBuffer;
        if (buffer != null)
        {
            Console.WriteLine("Received: " + buffer.ToString(Encoding.UTF8));
        }
        base.ChannelRead(context, message);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Console.WriteLine("Exception: " + exception);
        base.ExceptionCaught(context, exception);
    }
}

```





```csharp
using System.Net;
using System.Net.Sockets;
using System.Text;

[ApiController]
[Route("[controller]")]
public class SocketController : ControllerBase
{
    [HttpGet]
    public async Task<string> Get()
    {
        string ipAddress = "127.0.0.1";
        int port = 8080;
        string message = "Hello, world!";

        byte[] data = Encoding.ASCII.GetBytes(message);

        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            await socket.ConnectAsync(IPAddress.Parse(ipAddress), port);
            await socket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
        }

        return "Data sent successfully!";
    }
}


using System.Net;
using System.Net.Sockets;
using System.Text;

[ApiController]
[Route("[controller]")]
public class SocketController : ControllerBase
{
    [HttpGet]
    public async Task<string> Get()
    {
        int port = 8080;

        byte[] buffer = new byte[1024];

        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(1);

            Socket clientSocket = await socket.AcceptAsync();

            int bytesRead = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            return message;
        }
    }
}

```

